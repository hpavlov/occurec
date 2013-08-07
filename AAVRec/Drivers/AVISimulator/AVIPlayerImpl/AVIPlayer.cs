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

        private bool fullAAVSimulation;
        private IOcrTester ocrTester = null;
        private bool ocrEnabled = false;

        public bool IsRunning
        {
            get;
            private set;
        }

        internal IVideoCallbacks callbacksObject;

        public AVIPlayer(string fileName, float frameRate, bool fullAAVSimulation)
        {
            this.m_FileName = fileName;
            this.frameRate = frameRate;
            this.fullAAVSimulation = fullAAVSimulation;

            OpenVideoFile();

            IsRunning = false;
            string errorMessage;

	        OcrConfiguration ocrConfig = OcrSettings.Instance[Settings.Default.SelectedOcrConfiguration];

            if (fullAAVSimulation)
            {
                NativeHelpers.SetupCamera(
                       Settings.Default.CameraModel,
                       ImageWidth, ImageHeight,
                       Settings.Default.FlipHorizontally,
                       Settings.Default.FlipVertically,
                       Settings.Default.IsIntegrating,
                       (float)Settings.Default.SignatureDiffFactorEx2,
                       (float)Settings.Default.MinSignatureDiff,
					   Settings.Default.GammaDiff);

                NativeHelpers.SetupAav(Settings.Default.AavImageLayout);

	            if (Settings.Default.SimulatorRunOCR)
	            {
					if (ocrConfig.Alignment.Width == ImageWidth && ocrConfig.Alignment.Height == ImageHeight)
						errorMessage = NativeHelpers.SetupBasicOcrMetrix(ocrConfig);
					else
					{
						errorMessage = "Video file incompatible with OCR configuration.";
						Settings.Default.SimulatorRunOCR = false;
					}
	            }
	            else
		            errorMessage = NativeHelpers.SetupTimestampPreservation(ImageWidth, ImageHeight);

	            if (errorMessage != null && callbacksObject != null)
                    callbacksObject.OnError(-1, errorMessage);                
            }

			if (Settings.Default.SimulatorRunOCR)
			{
				if (Settings.Default.OcrSimulatorNativeCode)
					ocrTester = new NativeOcrTester();
				else
					ocrTester = new ManagedOcrTester();

				errorMessage = ocrTester.Initialize(ocrConfig, ImageWidth, ImageHeight);

				if (errorMessage != null && callbacksObject != null)
					callbacksObject.OnError(-1, errorMessage);
				else
					ocrEnabled = true;
			}
        }

        public int ImageWidth { get; private set; }

        public int ImageHeight { get; private set; }

        public int BitDepth { get; private set; }

        private void EnsureMediaDet()
        {
            NonBlockingLock.ExclusiveLock(
                NonBlockingLock.LOCK_ID_CloseInterfaces,
                () =>
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
                });

            //lock (syncRoot)
            //{
            //    if (m_MediaDet != null)
            //        Marshal.ReleaseComObject(m_MediaDet);
            //    m_MediaDet = null;

            //    m_MediaDet = (IMediaDet)new MediaDet();
            //    DsError.ThrowExceptionForHR(m_MediaDet.put_Filename(m_FileName));

            //    // find the video stream in the file
            //    int index = 0;
            //    Guid type = Guid.Empty;
            //    while (type != MediaType.Video)
            //    {
            //        m_MediaDet.put_CurrentStream(index++);
            //        m_MediaDet.get_StreamType(out type);
            //    }
            //}            
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
				if (ocrEnabled)
					ocrTester.Reset();

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
        private FrameProcessingStatus frameStatus;

        private void Run(object state)
        {
            int waitTimeMs = (int)(1000 / frameRate);

            while (IsRunning)
            {
                long nextFrameCounterValue = frameCounter + 1;
                FrameProcessingStatus nextFrameStatus = FrameProcessingStatus.Empty;

                Thread.Sleep(waitTimeMs);

                if (Settings.Default.SimulatorRunOCR || fullAAVSimulation)
                {
                    long frameNo = 0 + (frameCounter%m_FrameCount);
                    double frameTime = ConvertFrameNumberToSeconds((int) frameNo);
                    using (Bitmap bmp = GetImageAtTime(frameTime))
                    {
                        int[,] pixels = ImageUtils.GetPixelArray(bmp, AdvImageSection.GetPixelMode.Raw8Bit);

                        if (fullAAVSimulation)
                        {
                            if (pixels != null)
                                nextFrameStatus = NativeHelpers.ProcessVideoFrame2(pixels);
                        }

                        if (Settings.Default.SimulatorRunOCR)
                        {
                            OsdFrameInfo frameInfo = ocrTester.ProcessFrame(pixels, frameNo);
                            if (callbacksObject != null && frameInfo != null)
                            {
                                callbacksObject.OnEvent(0, frameInfo.ToDisplayString());
                                if (!frameInfo.FrameInfoIsOk())
                                    callbacksObject.OnEvent(1, null);
                            }
                        }
                    }
                }

                NonBlockingLock.Lock(
                    NonBlockingLock.LOCK_ID_BufferCB,
                    () =>
                    {
                        frameCounter = nextFrameCounterValue;
                        frameStatus = FrameProcessingStatus.Clone(nextFrameStatus);
                    });
            }
        }

        public bool GetCurrentFrame(out Bitmap cameraFrame, out int frameNumber, out FrameProcessingStatus status)
        {
            if (!IsRunning)
            {
                cameraFrame = null;
                frameNumber = -1;
                status = FrameProcessingStatus.Empty;

                return false;
            }

            long frameNo = 0;
            var sts = new FrameProcessingStatus();

            NonBlockingLock.Lock(
                NonBlockingLock.LOCK_ID_GetNextFrame,
                () =>
                {
                    frameNo = 0 + (frameCounter % m_FrameCount);
                    sts = FrameProcessingStatus.Clone(frameStatus);
                });

            frameNumber = (int)frameNo;
            status = sts;

            if (fullAAVSimulation)
            {
                ImageStatus imgStatus;
                cameraFrame = NativeHelpers.GetCurrentImage(out imgStatus);
                status.CameraFrameNo = imgStatus.UniqueFrameNo;
                status.CurrentSignatureRatio = imgStatus.CutOffRatio;
                status.IntegratedFrameNo = imgStatus.IntegratedFrameNo;
                status.IntegratedFramesSoFar = imgStatus.CountedFrames;
            }
            else
            {
                double frameTime = ConvertFrameNumberToSeconds(frameNumber);
                cameraFrame = GetImageAtTime(frameTime);
            }
            

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

                            NonBlockingLock.Lock(
                                NonBlockingLock.LOCK_ID_GetNextFrame,
                                () =>
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
                                });

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

        public bool DisableOcr()
        {
            if (ocrEnabled)
            {
                ocrTester.DisableOcrErrorReporting();
                ocrEnabled = false;
                return true;
            }
            else
                return false;
        }

        public bool LockIntegration()
        {
            if (fullAAVSimulation)
            {
                NativeHelpers.LockIntegration();
                return true;
            }
            else
                return false;
        }

        public bool UnlockIntegration()
        {
            if (fullAAVSimulation)
            {
                NativeHelpers.UnlockIntegration();
                return true;
            }
            else
                return false;
        }
    }
}
