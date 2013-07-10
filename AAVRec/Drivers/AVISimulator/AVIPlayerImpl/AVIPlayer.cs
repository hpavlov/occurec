using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using AAVRec.Helpers;
using AAVRec.OCR;
using AAVRec.Properties;
using AAVRec.Video.AstroDigitalVideo;
using DirectShowLib;
using DirectShowLib.DES;

namespace AAVRec.Drivers.AVISimulator.AVIPlayerImpl
{
    class AVIPlayer
    {
        private string m_FileName;
        private float frameRate;
        private object syncRoot = new object();

        private IMediaDet m_MediaDet;
        private double m_MediaLength;
        private VideoInfoHeader m_VideoInfo;
        private IntPtr m_BufferPtr = IntPtr.Zero;
        private int m_BufferSize;
        private int m_FrameCount;
        private double m_FrameRate;

        private ManagedOcrTester ocrTester = null;

        public bool IsRunning
        {
            get;
            private set;
        }

        public AVIPlayer(string fileName, float frameRate)
        {
            this.m_FileName = fileName;
            this.frameRate = frameRate;

            OpenVideoFile();

            IsRunning = false;
            ocrTester = new ManagedOcrTester();
        }

        public int ImageWidth { get; private set; }

        public int ImageHeight { get; private set; }

        public int BitDepth { get; private set; }

        private void EnsureMediaDet()
        {
            lock (syncRoot)
            {
                if (m_MediaDet != null)
                    Marshal.ReleaseComObject(m_MediaDet);
                m_MediaDet = null;

                m_MediaDet = (IMediaDet)new MediaDet();
                DsError.ThrowExceptionForHR(m_MediaDet.put_Filename(m_FileName));

                // find the video stream in the file
                int index = 0;
                Guid type = Guid.Empty;
                while (type != MediaType.Video)
                {
                    m_MediaDet.put_CurrentStream(index++);
                    m_MediaDet.get_StreamType(out type);
                }
            }            
        }

        private void OpenVideoFile()
        {
            AMMediaType mediaType = null;

            try
            {
                EnsureMediaDet();

                // retrieve some measurements from the video
                m_MediaDet.get_FrameRate(out m_FrameRate);

                mediaType = new AMMediaType();
                m_MediaDet.get_StreamMediaType(mediaType);
                m_VideoInfo = (VideoInfoHeader)Marshal.PtrToStructure(mediaType.formatPtr, typeof(VideoInfoHeader));
                DsUtils.FreeAMMediaType(mediaType);
                mediaType = null;

                ImageWidth = m_VideoInfo.BmiHeader.Width;
                ImageHeight = m_VideoInfo.BmiHeader.Height;

                m_MediaDet.get_StreamLength(out m_MediaLength);
                m_FrameCount = (int)(m_FrameRate * m_MediaLength);


                m_MediaDet.GetBitmapBits(0, out m_BufferSize, IntPtr.Zero, ImageWidth, ImageHeight);
                m_BufferPtr = Marshal.AllocHGlobal(m_BufferSize);
            }
            catch (Exception ex)
            {
                if (mediaType != null)
                    DsUtils.FreeAMMediaType(mediaType);

                throw;
            }
        }

        public double ConvertFrameNumberToSeconds(int frameNumber)
        {
            return (frameNumber / m_FrameRate);
        }

        public int ConvertSecondsToFrameNumber(double seconds)
        {
            return (int)Math.Floor(seconds * m_FrameRate);
        }

        public void Start()
        {
            if (!IsRunning)
            {
                IsRunning = true;
                ThreadPool.QueueUserWorkItem(new WaitCallback(Run));
            }
        }

        public void Stop()
        {
            if (IsRunning)
            {
                IsRunning = false;
            }
        }

        private long frameCounter = 0;

        private void Run(object state)
        {

            int waitTimeMs = (int)(1000 / frameRate);

            while (IsRunning)
            {
                lock (syncRoot)
                {
                    frameCounter++;
                }

                Thread.Sleep(waitTimeMs);

                if (Settings.Default.SimulatorRunOCR)
                {
                    long frameNo = 0 + (frameCounter % m_FrameCount);
                    double frameTime = ConvertFrameNumberToSeconds((int)frameNo);
                    using (Bitmap bmp = GetImageAtTime(frameTime))
                    {
                        int[,] pixels = ImageUtils.GetPixelArray(bmp, AdvImageSection.GetPixelMode.Raw8Bit);
                        ocrTester.ProcessFrame(pixels);
                    }
                }
            }
        }

        public bool GetCurrentFrame(out Bitmap cameraFrame, out int frameNumber)
        {
            if (!IsRunning)
            {
                cameraFrame = null;
                frameNumber = -1;
                return false;
            }

            long frameNo;

            lock (syncRoot)
            {
                frameNo = 0 + (frameCounter % m_FrameCount);

            }

            frameNumber = (int)frameNo;
            double frameTime = ConvertFrameNumberToSeconds(frameNumber);

            cameraFrame = GetImageAtTime(frameTime);

            return true;
        }

        public Bitmap GetImageAtTime(double seconds)
        {
            if (seconds <= m_MediaLength)
            {
                if (m_MediaDet != null)
                {

                    Bitmap returnValue = null;

                    try
                    {
                        bool recreateMediaDet = false;
                        for (int i = 0; i < 2; i++)
                        {

                            lock (syncRoot)
                            {
                                try
                                {
                                    m_MediaDet.GetBitmapBits(seconds, out m_BufferSize, m_BufferPtr, ImageWidth, ImageHeight);
                                    recreateMediaDet = false;
                                }
                                catch (InvalidCastException)
                                {
                                    if (recreateMediaDet)
                                        throw;
                                    else
                                        recreateMediaDet = true;
                                }                                
                            }

                            if (recreateMediaDet)
                            {
                                EnsureMediaDet();
                            }
                            else
                                break;
                        }

                        // compose a bitmap from the data in the managed buffer	
                        unsafe
                        {
                            returnValue = new Bitmap(ImageWidth, ImageHeight, PixelFormat.Format24bppRgb);
                            BitmapData imageData = returnValue.LockBits(new Rectangle(0, 0, ImageWidth, ImageHeight), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                            int* imagePtr = (int*)imageData.Scan0;

                            int bitmapHeaderSize = Marshal.SizeOf(m_VideoInfo.BmiHeader);
                            int* sourcePtr = (int*)((byte*)m_BufferPtr.ToPointer() + bitmapHeaderSize);

                            for (int i = 0; i < (m_BufferSize - bitmapHeaderSize) / 4; i++)
                            {
                                *imagePtr = *sourcePtr;
                                imagePtr++;
                                sourcePtr++;
                            }

                            returnValue.UnlockBits(imageData);
                            returnValue.RotateFlip(RotateFlipType.Rotate180FlipX); // DirectShow stores pixels in a different order than Bitmaps do
                        }
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex);
                    }

                    return returnValue;
                }
            }

            return null;
        }
    }
}
