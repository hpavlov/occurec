using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using AAVRec.Video.AstroDigitalVideo;

namespace AAVRec.Video
{
    internal class AstroDigitalVideoStream : IDisposable
    {
        public static AstroDigitalVideoStream OpenADVFile(string fileName)
        {
            return new AstroDigitalVideoStream(fileName);
        }

        private string m_FileName;
        private AdvFile m_AdvFile;
        private AdvImageSection m_ImageSection;
        private AdvStatusData m_FrameStatus;
        private DateTime m_MidExposureUtc;
        private float m_Exposure;

        public static void CheckAdvFileFormat(string fileName)
        {
            using (AstroDigitalVideoStream managedStr = AstroDigitalVideoStream.OpenADVFile(fileName))
            {
                managedStr.CheckAdvFileFormatInternal();
            }
        }

        private void CheckAdvFileFormatInternal()
        {
            bool fileIsCorrupted = true;
            bool isADVFormat = false;
            int advFormatVersion = -1;
            DoConsistencyCheck(ref fileIsCorrupted, ref isADVFormat, ref advFormatVersion);

            if (!isADVFormat)
                throw new FormatException("The file is not in ADV format.");

            if (advFormatVersion > 1)
                throw new FormatException("The file ADV version is not supported yet.");

            if (fileIsCorrupted)
                throw new FormatException("The ADV file may be corrupted.\r\n\r\nTry to recover it from Tools -> Repair ADV File");
        }

        private AstroDigitalVideoStream(string fileName)
        {
            m_FileName = fileName;

            m_AdvFile = AdvFile.OpenFile(fileName);
            m_ImageSection = m_AdvFile.ImageSection;

            CheckAdvFileFormatInternal();
        }

        public int Width
        {
            get { return (int)m_ImageSection.Width; }
        }

        public int Height
        {
            get { return (int)m_ImageSection.Height; }
        }

        public int BitPix
        {
            get { return (int)m_ImageSection.BitsPerPixel; }
        }

        public int FirstFrame
        {
            get { return 0; }
        }

        public int LastFrame
        {
            get { return (int)m_AdvFile.NumberOfFrames; }
        }

        public int CountFrames
        {
            get { return (int)m_AdvFile.NumberOfFrames; }
        }

        public double FrameRate
        {
            get { return 25.0f; }
        }

        public double MillisecondsPerFrame
        {
            get { return 1000 / 25.0f; }
        }

        public void EnsureFrameOpen()
        {

        }

        public void EnsureFrameClose()
        {
            m_AdvFile.Close();
        }

        private ushort[,] prevFramePixels;
        private int prevFrameNo = -2;

        public Bitmap GetFrame(int index)
        {
            if (index < m_AdvFile.NumberOfFrames)
            {
                byte layoutId;
                AdvImageLayout.GetByteMode byteMode;

                m_AdvFile.GetFrameImageSectionHeader(index, out layoutId, out byteMode);

                AdvImageLayout layout = m_ImageSection.GetImageLayoutFromLayoutId(layoutId);

                if (layout.IsDiffCorrLayout && byteMode == AdvImageLayout.GetByteMode.DiffCorrBytes && prevFrameNo != index - 1)
                {
                    // Move back and find the nearest previous key frame
                    int keyFrameIdx = index;
                    do
                    {
                        keyFrameIdx--;
                        m_AdvFile.GetFrameImageSectionHeader(keyFrameIdx, out layoutId, out byteMode);
                    }
                    while (keyFrameIdx > 0 && byteMode != AdvImageLayout.GetByteMode.KeyFrameBytes);

                    object[] keyFrameData = m_AdvFile.GetFrameSectionData(keyFrameIdx, null);

                    prevFramePixels = ((AdvImageData)keyFrameData[0]).ImageData;


                    if (layout.DiffCorrFrame == DiffCorrFrameMode.PrevFrame)
                    {
                        for (int i = keyFrameIdx + 1; i < index; i++)
                        {
                            object[] frameData = m_AdvFile.GetFrameSectionData(i, prevFramePixels);

                            prevFramePixels = ((AdvImageData)frameData[0]).ImageData;

                        }
                    }
                }

                object[] data;

                data = m_AdvFile.GetFrameSectionData(index, prevFramePixels);

                AdvImageData imageData = (AdvImageData)data[0];
                m_FrameStatus = (AdvStatusData)data[1];
                m_MidExposureUtc = imageData.MidExposureUtc;
                m_Exposure = imageData.ExposureMilliseconds;

                if (prevFramePixels == null)
                    prevFramePixels = new ushort[m_ImageSection.Width, m_ImageSection.Height];

                for (int x = 0; x < m_ImageSection.Width; x++)
                    for (int y = 0; y < m_ImageSection.Height; y++)
                    {
                        prevFramePixels[x, y] = imageData.ImageData[x, y];
                    }

                prevFrameNo = index;

                Bitmap rv = m_ImageSection.CreateBitmap(imageData);

                return rv;
            }
            else
                return null;
        }

        public int RecommendedBufferSize
        {
            get { return 4; }
        }

        public string Engine { get { return "ADV.MNGD"; } }

        public void Dispose()
        {
            EnsureFrameClose();
        }

        public bool IsNativeStream { get { return false; } }

        private static DateTime REFERENCE_DATETIME = new DateTime(2010, 1, 1, 0, 0, 0, 0);

        public void DoConsistencyCheck(ref bool fileIsCorrupted, ref bool isADVFormat, ref int advFormatVersion)
        {
            fileIsCorrupted = m_AdvFile.IsCorrupted;

            string fstsTypeStr;
            if (!m_AdvFile.AdvFileTags.TryGetValue("FSTF-TYPE", out fstsTypeStr))
                isADVFormat = false;
            else
            {
                isADVFormat = fstsTypeStr == "ADV";

                if (isADVFormat)
                {
                    string advVersionStr;
                    if (!m_AdvFile.AdvFileTags.TryGetValue("ADV-VERSION", out advVersionStr))
                        advFormatVersion = -1;
                    else if (!int.TryParse(advVersionStr, out advFormatVersion))
                        advFormatVersion = -1;                    
                }
                else
                {
                    isADVFormat = fstsTypeStr == "AAV";

                    if (isADVFormat)
                    {
                        string advVersionStr;
                        if (!m_AdvFile.AdvFileTags.TryGetValue("AAV-VERSION", out advVersionStr))
                            advFormatVersion = -1;
                        else if (!int.TryParse(advVersionStr, out advFormatVersion))
                            advFormatVersion = -1;
                    }
                    
                }
            }

        }
    }

}
