using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using AAVRec.Helpers;
using AAVRec.OCR;
using AAVRec.OCR.TestStates;
using AAVRec.Properties;
using AAVRec.Video;
using AAVRec.Video.AstroDigitalVideo;

namespace AAVRec.Drivers.AAVSimulator.AAVPlayerImpl
{
    class AAVPlayer
    {
        private AstroDigitalVideoStream aavStream;
        private float frameRate;
        private object syncRoot = new object();
        private ManagedOcrTester ocrTester = null;
        
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

        public AAVPlayer(string fileName, float frameRate)
        {
            aavStream = AstroDigitalVideoStream.OpenADVFile(fileName);
            this.frameRate = frameRate;

            IsRunning = false;
            ocrTester = new ManagedOcrTester();
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
                        ocrTester.ProcessFrame(pixels);
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
    }
}
