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

            if (Settings.Default.OcrSimulatorNativeCode)
                ocrTester = new NativeOcrTester();
            else
                ocrTester = new ManagedOcrTester();

            string errorMessage = ocrTester.Initialize(OcrSettings.Instance[Settings.Default.SelectedOcrConfiguration], ImageWidth, ImageHeight);

            if (errorMessage != null && callbacksObject != null)
                callbacksObject.OnError(-1, errorMessage);
            else
                ocrEnabled = true;
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

        private void Run(object state)
        {
            
            int waitTimeMs = (int)(1000/frameRate);

            while (IsRunning)
            {
                lock (syncRoot)
                {
                    frameCounter++;    
                }                

                Thread.Sleep(waitTimeMs);

                if (Settings.Default.SimulatorRunOCR)
                {
                    long frameNo = aavStream.FirstFrame + (frameCounter % (aavStream.LastFrame - aavStream.FirstFrame));
                    using (Bitmap bmp = aavStream.GetFrame((int)frameNo))
                    {
                        int[,] pixels = ImageUtils.GetPixelArray(bmp, AdvImageSection.GetPixelMode.Raw8Bit);
                        ocrTester.ProcessFrame(pixels, frameNo);
                    }
                }
            }
        }

        public bool GetCurrentFrame(out Bitmap bmp, out int frameNumber)
        {
            if (!IsRunning)
            {
                bmp = null;
                frameNumber = -1;
                return false;
            }

            long frameNo;

            lock (syncRoot)
            {
                frameNo = aavStream.FirstFrame + (frameCounter % (aavStream.LastFrame - aavStream.FirstFrame));
                
            }

            frameNumber = (int) frameNo;
            bmp = aavStream.GetFrame(frameNumber);
            
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
