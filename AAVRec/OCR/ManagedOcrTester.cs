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
        string Initialize(OcrConfiguration ocrConfig, int imageWidth, int imageHeight);
        void Reset();
        OsdFrameInfo ProcessFrame(int[,] pixels, long frameNo);
        void DisableOcr();
        void DisableOcrErrorReporting();
    }

    internal class NativeOcrTester : IOcrTester
    {
		public string Initialize(OcrConfiguration ocrConfig, int imageWidth, int imageHeight)
        {
			string initMsg = NativeHelpers.SetupBasicOcrMetrix(ocrConfig);
            if (initMsg == null)
				NativeHelpers.SetupOcr(ocrConfig);

            return initMsg;
        }

        public void Reset()
        {
            
        }

        public OsdFrameInfo ProcessFrame(int[,] pixels, long frameNo)
        {
            //throw new NotImplementedException();
            return new OsdFrameInfo(new OsdFieldInfo(), new OsdFieldInfo());
        }

        public void DisableOcr()
        {
            NativeHelpers.DisableOcr();            
        }

        public void DisableOcrErrorReporting()
        {
            // TODO
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
        private bool ocrErrorReporting = false;
	    private OcrConfiguration ocrConfig;

        public string Initialize(OcrConfiguration ocrConfig, int imageWidth, int imageHeight)
        {
	        this.ocrConfig = ocrConfig;

			for (int i = 0; i < ocrConfig.Alignment.CharPositions.Count; i++)
            {
				int leftPos = ocrConfig.Alignment.CharPositions[i];
				var ocredChar = new OcredChar(i, leftPos, ocrConfig.Alignment.CharWidth, ocrConfig.Alignment.CharHeight);
				ocredChar.PopulateZones(ocrConfig.Zones);
                ocredCharsOdd.Add(ocredChar);

				ocredChar = new OcredChar(i, leftPos, ocrConfig.Alignment.CharWidth, ocrConfig.Alignment.CharHeight);
				ocredChar.PopulateZones(ocrConfig.Zones);
                ocredCharsEven.Add(ocredChar);
            }

            zoneChecker = new OcrZoneChecker(
				ocrConfig,
				ocrConfig.Alignment.Width,
				ocrConfig.Alignment.Height,
				ocrConfig.Zones,
				ocrConfig.Alignment.CharPositions);

            charRecognizer = new OcrCharRecognizer(
				ocrConfig.Zones,
				ocrConfig.CharDefinitions);

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
            
            outputDebugFileCounter = 0;
            ocrEnabled = true;
            ocrErrorReporting = true;
        }

		private void EnsureOutputDirectory()
		{
			Directory.CreateDirectory(outputDebugFolder);
		}

        private static Font s_DebugFont = new Font(FontFamily.GenericMonospace, 14, FontStyle.Bold, GraphicsUnit.Pixel);

        public OsdFrameInfo ProcessFrame(int[,] pixels, long frameNo)
        {
            if (!ocrEnabled)
                return null;

            int IMAGE_HEIGHT = pixels.GetLength(0);
            int IMAGE_WIDTH = pixels.GetLength(1);

			int[,] tsPixelsOdd = new int[42, IMAGE_WIDTH];
			int[,] tsPixelsEven = new int[42, IMAGE_WIDTH];

			int OCR_LINES_FROM = ocrConfig.Alignment.FrameTopOdd;
			int OCR_LINES_TO = ocrConfig.Alignment.FrameTopEven + 2 * ocrConfig.Alignment.CharHeight;

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

	        OsdFieldInfo oddFieldInfo = null;
			OsdFieldInfo evenFieldInfo = null;

			try
			{
				oddFieldInfo = OsdFieldInfoExtractor.ExtractFieldInfo(ocredCharsOdd);
				evenFieldInfo = OsdFieldInfoExtractor.ExtractFieldInfo(ocredCharsEven);
			}
			catch (Exception)
			{
				oddFieldInfo = new OsdFieldInfo() { FieldNumber = 1 };
				evenFieldInfo = new OsdFieldInfo() { FieldNumber = 2 };
			}

            var frameInfo = new OsdFrameInfo(oddFieldInfo, evenFieldInfo);

            TestFrameResult testResult = testContext.TestTimeStamp(frameInfo);

            if (ocrErrorReporting && testResult == TestFrameResult.ErrorSaveScreenShotImages && oddFieldInfo.FieldNumber >= 0 && evenFieldInfo.FieldNumber >= 0)
            {
                Bitmap bmpOdd = CreateBitmapFromPixels(tsPixelsOdd);
                Bitmap bmpEven = CreateBitmapFromPixels(tsPixelsEven);

                int MAX_OFF_VALUE = median + (OcrCharRecognizer.MIN_ON_VALUE - median) / 4;

                using (Graphics g = Graphics.FromImage(bmpOdd))
                {
                    foreach (OcredChar ocredChar in ocredCharsOdd)
                    {
                        g.DrawString(ocredChar.RecognizedChar + "", s_DebugFont, Brushes.Yellow, ocredChar.LeftFrom + 4, 26);
                        double[] zoneValues = ocredChar.ComputeZones();
                        for (int i = 0; i < zoneValues.Length; i++)
                        {
                            int left = ocredChar.LeftFrom + 7 + i;
                            g.DrawLine(i % 2 == 0 ? Pens.Blue : Pens.Red, left, 39, left, 42);
                            Pen zonePen = Pens.Black;
                            if (zoneValues[i] >= OcrCharRecognizer.MIN_ON_VALUE)
                                zonePen = Pens.White;
                            else if (zoneValues[i] > MAX_OFF_VALUE)
                                zonePen = Pens.LightSalmon;
                            g.DrawLine(zonePen, left, 40, left, 42);
                            g.DrawLine(i % 2 == 0 ? Pens.Blue : Pens.Red, left, 41, left, 42);
                        }
                    }

                    g.Save();
                }

                using (Graphics g = Graphics.FromImage(bmpEven))
                {
                    foreach (OcredChar ocredChar in ocredCharsEven)
                    {
                        g.DrawString(ocredChar.RecognizedChar + "", s_DebugFont, Brushes.Yellow, ocredChar.LeftFrom + 4, 26);
                        double[] zoneValues = ocredChar.ComputeZones();
                        for (int i = 0; i < zoneValues.Length; i++)
                        {
                            int left = ocredChar.LeftFrom + 7 + i;
                            g.DrawLine(i % 2 == 0 ? Pens.Blue : Pens.Red, left, 39, left, 42);
                            Pen zonePen = Pens.Black;
                            if (zoneValues[i] >= OcrCharRecognizer.MIN_ON_VALUE)
                                zonePen = Pens.White;
                            else if (zoneValues[i] > MAX_OFF_VALUE)
                                zonePen = Pens.LightSalmon;
                            g.DrawLine(zonePen, left, 40, left, 42);
                            g.DrawLine(i % 2 == 0 ? Pens.Blue : Pens.Red, left, 41, left, 42);
                        }
                    }

                    g.Save();
                }

                outputDebugFileCounter++;
	            EnsureOutputDirectory();
                bmpOdd.Save(Path.GetFullPath(string.Format("{0}\\{1}-odd-ts({2}).bmp", outputDebugFolder, frameNo, outputDebugFileCounter)));
                bmpEven.Save(Path.GetFullPath(string.Format("{0}\\{1}-even-ts({2}).bmp", outputDebugFolder, frameNo, outputDebugFileCounter)));                   
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

        public void DisableOcrErrorReporting()
        {
            ocrErrorReporting = false;
        }
    }
}
