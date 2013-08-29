using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using OccuRec.Helpers;
using OccuRec.OCR;
using OccuRec.OCR.TestStates;
using OccuRec.Properties;
using OccuRec.Video;
using OccuRec.Video.AstroDigitalVideo;

namespace OccuRec.Drivers.AAVSimulator.AAVPlayerImpl
{
    class AAVPlayer
    {
        private AstroDigitalVideoStream aavStream;
        private float frameRate;
        private object syncRoot = new object();
        private IOcrTester ocrTester = null;
        
        public bool IsRunning
        {
            get; 
            private set;
        }

        public int ImageWidth
        {
            get { return aavStream.Width; }
        }

        public int ImageHeight
        {
            get { return aavStream.Height; }
        }

        public int BitDepth
        {
            get { return aavStream.BitPix; }
        }

        private bool fullAAVSimulation;
        internal IVideoCallbacks callbacksObject;
        private bool ocrEnabled = false;

        public AAVPlayer(string fileName, float frameRate, bool fullAAVSimulation)
        {
            aavStream = AstroDigitalVideoStream.OpenADVFile(fileName);
            this.frameRate = frameRate;
            this.fullAAVSimulation = fullAAVSimulation;

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
                       (float)Settings.Default.MinSignatureDiffRatio,
                       (float)Settings.Default.MinSignatureDiff,
                       Settings.Default.GammaDiff,
                       OccuRec.Drivers.AVISimulator.Video.DRIVER_DESCRIPTION,
                       "N/A");

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

        public void Start()
        {
            if (!IsRunning)
            {
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
            
            int waitTimeMs = (int)(1000/frameRate);

            while (IsRunning)
            {
                long nextFrameCounterValue = frameCounter + 1;
                FrameProcessingStatus nextFrameStatus = FrameProcessingStatus.Empty;

                Thread.Sleep(waitTimeMs);

                if (Settings.Default.SimulatorRunOCR || fullAAVSimulation)
                {
                    long frameNo = aavStream.FirstFrame + (frameCounter % (aavStream.LastFrame - aavStream.FirstFrame));
                    using (Bitmap bmp = aavStream.GetFrame((int)frameNo))
                    {
                        int[,] pixels = ImageUtils.GetPixelArray(bmp, AdvImageSection.GetPixelMode.Raw8Bit);

                        if (fullAAVSimulation)
                        {
                            if (pixels != null)
                                nextFrameStatus = NativeHelpers.ProcessVideoFrame2(pixels);
                        }

						if (Settings.Default.SimulatorRunOCR &&
							!Settings.Default.OcrSimulatorNativeCode &&
							ocrTester != null)
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

        public bool GetCurrentFrame(out Bitmap bmp, out int frameNumber, out FrameProcessingStatus status)
        {
            if (!IsRunning)
            {
                bmp = null;
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
                    frameNo = aavStream.FirstFrame + (frameCounter % (aavStream.LastFrame - aavStream.FirstFrame));
                    sts = FrameProcessingStatus.Clone(frameStatus);
                });

            frameNumber = (int)frameNo;
            status = sts;

            if (fullAAVSimulation)
            {
                ImageStatus imgStatus;
                bmp = NativeHelpers.GetCurrentImage(out imgStatus);
                status = new FrameProcessingStatus(imgStatus);

            }
            else
            {
                bmp = aavStream.GetFrame(frameNumber);
            }
            
            return true;
        }

        public bool DisableOcr()
        {
            if (ocrEnabled)
            {
                ocrTester.DisableOcr();
                ocrEnabled = false;
                return true;
            }
            else
                return false;
        }
    }
}
