using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using AAVRec.Drivers;
using AAVRec.Helpers;
using AAVRec.OCR.TestStates;
using AAVRec.Properties;

namespace AAVRec.OCR
{
    internal interface IOcrTester
    {
        string Initialize(int imageWidth, int imageHeight);
        void Reset();
        OsdFrameInfo ProcessFrame(int[,] pixels);
        void DisableOcr();
    }

    internal class NativeOcrTester : IOcrTester
    {
        public string Initialize(int imageWidth, int imageHeight)
        {
            return NativeHelpers.SetupOcr();            
        }

        public void Reset()
        {
            
        }

        public OsdFrameInfo ProcessFrame(int[,] pixels)
        {
            //throw new NotImplementedException();
            return new OsdFrameInfo(new OsdFieldInfo(), new OsdFieldInfo());
        }

        public void DisableOcr()
        {
            NativeHelpers.DisableOcr();            
        }
    }

    internal class ManagedOcrTester : IOcrTester
    {
        private List<OcredChar> ocredCharsOdd = new List<OcredChar>();
        private List<OcredChar> ocredCharsEven = new List<OcredChar>();
        private OcrZoneChecker zoneChecker;
        private OcrCharRecognizer charRecognizer;

	    private ICameraImage cameraImage;
        private StateContext testContext;
        private bool generateDebugImages;
        private bool ocrEnabled = false;

        public string Initialize(int imageWidth, int imageHeight)
        {
            for (int i = 0; i < OcrSettings.Instance.Alignment.CharPositions.Count; i++)
            {
                int leftPos = OcrSettings.Instance.Alignment.CharPositions[i];
                var ocredChar = new OcredChar(i, leftPos, OcrSettings.Instance.Alignment.CharWidth, OcrSettings.Instance.Alignment.CharHeight);
                ocredChar.PopulateZones(OcrSettings.Instance.Zones);
                ocredCharsOdd.Add(ocredChar);

                ocredChar = new OcredChar(i, leftPos, OcrSettings.Instance.Alignment.CharWidth, OcrSettings.Instance.Alignment.CharHeight);
                ocredChar.PopulateZones(OcrSettings.Instance.Zones);
                ocredCharsEven.Add(ocredChar);
            }

            zoneChecker = new OcrZoneChecker(
                OcrSettings.Instance.Alignment.Width,
                OcrSettings.Instance.Alignment.Height,
                OcrSettings.Instance.Zones,
                OcrSettings.Instance.Alignment.CharPositions);

            charRecognizer = new OcrCharRecognizer(
                OcrSettings.Instance.Zones,
                OcrSettings.Instance.CharDefinitions);

            cameraImage = new CameraImage();
            testContext = new StateContext();
            generateDebugImages = false;

            return null;
        }

        private string outputDebugFolder;
        private int outputDebugFileCounter = 0;

        public void Reset()
        {
            testContext.Reset();

            outputDebugFolder = Path.GetFullPath(string.Format("{0}\\{1}-{2}", Settings.Default.OcrDebugOutputFolder, DateTime.Now.ToString("dd-MMM-HHmm-"), Guid.NewGuid()));
            Directory.CreateDirectory(outputDebugFolder);
            outputDebugFileCounter = 0;
            ocrEnabled = true;
        }

        private static Font s_DebugFont = new Font(FontFamily.GenericMonospace, 14, FontStyle.Bold, GraphicsUnit.Pixel);

        public OsdFrameInfo ProcessFrame(int[,] pixels)
        {
            if (!ocrEnabled)
                return new OsdFrameInfo(new OsdFieldInfo(), new OsdFieldInfo());

            int IMAGE_HEIGHT = pixels.GetLength(0);
            int IMAGE_WIDTH = pixels.GetLength(1);

			int[,] tsPixelsOdd = new int[42, IMAGE_WIDTH];
			int[,] tsPixelsEven = new int[42, IMAGE_WIDTH];

	        int OCR_LINES_FROM = OcrSettings.Instance.Alignment.FrameTopOdd;
	        int OCR_LINES_TO = OcrSettings.Instance.Alignment.FrameTopEven + 2 * OcrSettings.Instance.Alignment.CharHeight;

            List<int> medianArray = new List<int>();

            for (int y = 0; y < IMAGE_HEIGHT; y++)
            {
                for (int x = 0; x < IMAGE_WIDTH; x++)
                {
                    if (y == 10 || y == 11)
                        medianArray.Add(pixels[y, x]);

                    int packedVal = zoneChecker.OcrPixelMap[y, x];
                    if (packedVal != 0)
                    {
                        int charId;
                        bool isOddField;
                        int zoneId;
                        int zonePixelId;

                        zoneChecker.UnpackValue(packedVal, out charId, out isOddField, out zoneId, out zonePixelId);

                        OcredChar ocredChar = isOddField ? ocredCharsOdd[charId] : ocredCharsEven[charId];

                        ocredChar.Zones[zoneId][zonePixelId] = pixels[y, x];

                        if (isOddField)
                        {
                            int top = (y - OCR_LINES_FROM)/2;
                            tsPixelsOdd[top, x] = pixels[y, x];
                        }
                        else
                        {
                            int top = (y - OCR_LINES_FROM + 1) / 2;
                            tsPixelsEven[top, x] = pixels[y, x];
                        }
                    }

                    if (y >= OCR_LINES_FROM && y <= OCR_LINES_TO)
                    {
                        bool isOddFieldLine = (y - OCR_LINES_FROM) % 2 == 0;

                        if (isOddFieldLine)
                        {
                            int top = 14 + (y - OCR_LINES_FROM) / 2;
                            if (top >= 0 && top < tsPixelsOdd.GetLength(0) && x >= 0 && x < tsPixelsOdd.GetLength(1))
                                tsPixelsOdd[top, x] = pixels[y, x];
                            else
                                throw new IndexOutOfRangeException();
                        }
                        else
                        {
                            int top = 14 + (y - OCR_LINES_FROM + 1) / 2;
                            if (top >= 0 && top < tsPixelsEven.GetLength(0) && x >= 0 && x < tsPixelsEven.GetLength(1))
                                tsPixelsEven[top, x] = pixels[y, x];
                            else
                                throw new IndexOutOfRangeException();
                        }
                    }					

                }
            }

            medianArray.Sort();
            int median = medianArray[medianArray.Count/2];

            for (int i = 0; i < ocredCharsOdd.Count; i++)
            {
                OcredChar ocredChar = ocredCharsOdd[i];
                double[] computedZones = ocredChar.ComputeZones();
                ocredChar.RecognizedChar = charRecognizer.RecognizeChar(computedZones, median, i);
            }

            for (int i = 0; i < ocredCharsEven.Count; i++)
            {
                OcredChar ocredChar = ocredCharsEven[i];
                double[] computedZones = ocredChar.ComputeZones();
                ocredChar.RecognizedChar = charRecognizer.RecognizeChar(computedZones, median, i);
            }

            OsdFieldInfo oddFieldInfo = OsdFieldInfoExtractor.ExtractFieldInfo(ocredCharsOdd);
            OsdFieldInfo evenFieldInfo = OsdFieldInfoExtractor.ExtractFieldInfo(ocredCharsEven);

            var frameInfo = new OsdFrameInfo(oddFieldInfo, evenFieldInfo);

            TestFrameResult testResult = testContext.TestTimeStamp(frameInfo);

            if (testResult == TestFrameResult.ErrorSaveScreenShotImages)
            {
                Bitmap bmpOdd = CreateBitmapFromPixels(tsPixelsOdd);
                Bitmap bmpEven = CreateBitmapFromPixels(tsPixelsEven);

                using (Graphics g = Graphics.FromImage(bmpOdd))
                {
                    foreach (OcredChar ocredChar in ocredCharsOdd)
                    {
                        g.DrawString(ocredChar.RecognizedChar + "", s_DebugFont, Brushes.Yellow, ocredChar.LeftFrom + 4, 26);
                    }

                    g.Save();
                }

                using (Graphics g = Graphics.FromImage(bmpEven))
                {
                    foreach (OcredChar ocredChar in ocredCharsEven)
                    {
                        g.DrawString(ocredChar.RecognizedChar + "", s_DebugFont, Brushes.Yellow, ocredChar.LeftFrom + 4, 26);
                    }

                    g.Save();
                }

                outputDebugFileCounter++;
                bmpOdd.Save(Path.GetFullPath(string.Format("{0}\\{1}-odd-ts.bmp", outputDebugFolder, outputDebugFileCounter)));
                bmpEven.Save(Path.GetFullPath(string.Format("{0}\\{1}-even-ts.bmp", outputDebugFolder, outputDebugFileCounter)));                   
            }

            return frameInfo;
        }

        public Bitmap CreateBitmapFromPixels(int[,] pixels)
        {
            Bitmap b = new Bitmap((int)pixels.GetLength(1), (int)pixels.GetLength(0));

            // GDI+ still lies to us - the return format is BGR, NOT RGB.
            BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int stride = bmData.Stride;
            System.IntPtr Scan0 = bmData.Scan0;

            unsafe
            {
                byte* p = (byte*)(void*)Scan0;

                int nOffset = stride - b.Width * 3;

                for (int y = 0; y < b.Height; ++y)
                {
                    for (int x = 0; x < b.Width; ++x)
                    {
                        byte red = (byte)(pixels[y, x]);

                        p[0] = red;
                        p[1] = red;
                        p[2] = red;

                        p += 3;
                    }

                    p += nOffset;
                }
            }

            b.UnlockBits(bmData);

            return b;
        }

        public void DisableOcr()
        {
            ocrEnabled = false;
        }
    }
}
