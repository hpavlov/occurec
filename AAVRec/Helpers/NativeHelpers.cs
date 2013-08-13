using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using AAVRec.OCR;
using AAVRec.Properties;

namespace AAVRec.Helpers
{
	public enum LumaConversionMode
	{
		R = 0,
		G = 1,
		B = 2,
		GrayScale = 3
	}

    public enum AavImageLayout
    {
        Unknown = 0,
        UncompressedRaw = 1,
        CompressedDiffCodeNoSigns = 2,
        CompressedDiffCodeWithSigns = 3,
        CompressedRaw = 4
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SYSTEMTIME
    {
        [MarshalAs(UnmanagedType.U2)]
        public short Year;
        [MarshalAs(UnmanagedType.U2)]
        public short Month;
        [MarshalAs(UnmanagedType.U2)]
        public short DayOfWeek;
        [MarshalAs(UnmanagedType.U2)]
        public short Day;
        [MarshalAs(UnmanagedType.U2)]
        public short Hour;
        [MarshalAs(UnmanagedType.U2)]
        public short Minute;
        [MarshalAs(UnmanagedType.U2)]
        public short Second;
        [MarshalAs(UnmanagedType.U2)]
        public short Milliseconds;

        public SYSTEMTIME(DateTime dt)
        {
            dt = dt.ToUniversalTime();  // SetSystemTime expects the SYSTEMTIME in UTC
            Year = (short)dt.Year;
            Month = (short)dt.Month;
            DayOfWeek = (short)dt.DayOfWeek;
            Day = (short)dt.Day;
            Hour = (short)dt.Hour;
            Minute = (short)dt.Minute;
            Second = (short)dt.Second;
            Milliseconds = (short)dt.Millisecond;
        }


        public static SYSTEMTIME MinValue = new SYSTEMTIME(1601, 1, 1);
        public static SYSTEMTIME MaxValue = new SYSTEMTIME(30827, 12, 31, 23, 59, 59, 999);

        public SYSTEMTIME(short year, short month, short day, short hour = 0, short minute = 0, short second = 0, short millisecond = 0)
        {
           Year = year;
           Month = month;
           Day = day;
           Hour = hour;
           Minute = minute;
           Second = second;
           Milliseconds = millisecond;
           DayOfWeek = 0;
        }

        public static bool operator ==(SYSTEMTIME s1, SYSTEMTIME s2)
        {
            return (s1.Year == s2.Year && s1.Month == s2.Month && s1.Day == s2.Day && s1.Hour == s2.Hour && s1.Minute == s2.Minute && s1.Second == s2.Second && s1.Milliseconds == s2.Milliseconds);
        }

        public static bool operator !=(SYSTEMTIME s1, SYSTEMTIME s2)
        {
            return !(s1 == s2);
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    internal class ImageStatus
    {
        public long StartExposureSystemTime;
        public long EndExposureSystemTime;
        public long StartExposureFrameNo;
        public long EndExposureFrameNo;
        public int CountedFrames;
        public float CutOffRatio;
        public long IntegratedFrameNo;
        public long UniqueFrameNo;
        public int PerformedAction;
        public float PerformedActionProgress;
		public int DetectedIntegrationRate;
	    public int DropedFramesSinceIntegrationLock;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct FrameProcessingStatus
    {
        public static FrameProcessingStatus Empty = new FrameProcessingStatus();

        public long CameraFrameNo;
        public long IntegratedFrameNo;
        public int IntegratedFramesSoFar;
        public float FrameDiffSignature;
        public float CurrentSignatureRatio;
        public int PerformedAction;
        public float PerformedActionProgress;

        public static FrameProcessingStatus Clone(FrameProcessingStatus cloneFrom)
        {
            var rv = new FrameProcessingStatus();
            rv.CameraFrameNo = cloneFrom.CameraFrameNo;
            rv.IntegratedFrameNo = cloneFrom.IntegratedFrameNo;
            rv.IntegratedFramesSoFar = cloneFrom.IntegratedFramesSoFar;
            rv.FrameDiffSignature = cloneFrom.FrameDiffSignature;
            rv.CurrentSignatureRatio = cloneFrom.CurrentSignatureRatio;
            rv.PerformedAction = cloneFrom.PerformedAction;
            rv.PerformedActionProgress = cloneFrom.PerformedActionProgress;
            return rv;
        }
    };

	internal static class NativeHelpers
	{
        private const string AAVREC_CORE_DLL_NAME = "AAVRec.Core.dll";

		[DllImport(AAVREC_CORE_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern int GetBitmapPixels(
			int width, 
			int height, 
			int bpp,
			[In, MarshalAs(UnmanagedType.LPArray)] int[,] pixels,
			[In, Out] byte[] bitmapBytes);

		[DllImport(AAVREC_CORE_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GetColourBitmapPixels(
			int width,
			int height,
			int bpp,
			[In, MarshalAs(UnmanagedType.LPArray)] int[,,] pixels,
			[In, Out] byte[] bitmapBytes);

		[DllImport(AAVREC_CORE_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GetMonochromePixelsFromBitmap(
			int width,
			int height,
			int bpp,
			[In] IntPtr hBitmap,
			[In, Out, MarshalAs(UnmanagedType.LPArray)] int[,] bitmapBytes,
			int mode);

		[DllImport(AAVREC_CORE_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GetColourPixelsFromBitmap(
			int width,
			int height,
			int bpp,
			[In] IntPtr hBitmap,
			[In, Out, MarshalAs(UnmanagedType.LPArray)] int[,,] bitmapBytes,
            short flipMode);

        [DllImport(AAVREC_CORE_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SetupCamera(
            int width, 
            int height, 
            string cameraModel, 
            int monochromeConversionMode,
            bool flipHorizontally, 
            bool flipVertically,
            bool isIntegrating);

	    [DllImport(AAVREC_CORE_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
		private static extern int SetupIntegrationDetection(float differenceRatio, float minSignDiff, float diffGamma);

	    [DllImport(AAVREC_CORE_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SetupAav(int imageLayout, int usesBufferedMode, int integrationDetectionTuning, string aavRecVersion);

        [DllImport(AAVREC_CORE_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern int ProcessVideoFrame([In] IntPtr ptrBitmapData, long currentUtcDayAsTicks, [In, Out] ref FrameProcessingStatus frameInfo);

        [DllImport(AAVREC_CORE_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern int ProcessVideoFrame2([In, MarshalAs(UnmanagedType.LPArray)] int[,] pixel, long currentUtcDayAsTicks, [In, Out] ref FrameProcessingStatus frameInfo);

        [DllImport(AAVREC_CORE_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern int GetCurrentImage([In, Out] byte[] bitmapPixels);

        [DllImport(AAVREC_CORE_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern int GetCurrentImageStatus([In, Out] ImageStatus status);

        [DllImport(AAVREC_CORE_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern int StartRecording(string fileName);

        [DllImport(AAVREC_CORE_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern int StopRecording();

        [DllImport(AAVREC_CORE_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern int LockIntegration(bool doLock);

		[DllImport(AAVREC_CORE_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
		private static extern int ControlIntegrationCalibration(int cameraIntegration);

        [DllImport(AAVREC_CORE_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern int GetIntegrationCalibrationDataConfig([In, Out] ref int gammasLength, [In, Out] ref int signaturesPerCycle);

        [DllImport(AAVREC_CORE_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern int GetIntegrationCalibrationData([In, MarshalAs(UnmanagedType.LPArray)] float[] rawSignatures, [In, MarshalAs(UnmanagedType.LPArray)] float[] gammas);

        [DllImport(AAVREC_CORE_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern int InitNewIntegrationPeriodTesting(float differenceRatio, float minimumDifference);

        [DllImport(AAVREC_CORE_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern int TestNewIntegrationPeriod(long frameNo, float diffSignature, [In, Out] ref bool isNew);

		[DllImport(AAVREC_CORE_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
		private static extern int SetupIntegrationPreservationArea(int areaTopOdd, int areaTopEven, int areaHeight);

        [DllImport(AAVREC_CORE_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
		private static extern int SetupOcrAlignment(int width, int height, int frameTopOdd, int frameTopEven, int charWidth, int charHeight, int numberOfCharPositions, int numberOfZones, [In, MarshalAs(UnmanagedType.LPArray)] int[] pixelsInZones);

        [DllImport(AAVREC_CORE_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
	    private static extern int SetupOcrZoneMatrix([In, MarshalAs(UnmanagedType.LPArray)] int[,] matrix);

	    [DllImport(AAVREC_CORE_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
	    private static extern int SetupOcrChar(char character, int fixedPosition);

        [DllImport(AAVREC_CORE_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SetupOcrCharDefinitionZone(char character, int zoneId, int zoneValue, int zonePixelsCount);

        [DllImport(AAVREC_CORE_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern int DisableOcrProcessing();

        [DllImport("Kernel32.dll", EntryPoint = "RtlMoveMemory")]
        public static extern void CopyMemory(IntPtr Destination, IntPtr Source, [MarshalAs(UnmanagedType.U4)] uint Length);

		[DllImport("gdi32.dll")]
		public static extern bool DeleteObject(IntPtr hObject);


        public static Bitmap PrepareBitmapForDisplay(int[,] imageArray, int width, int height)
        {
            return PrepareBitmapForDisplay(imageArray, width, height, false);
        }

        public static Bitmap PrepareBitmapForDisplay(object[,] imageArray, int width, int height)
        {
            return PrepareBitmapForDisplay(imageArray, width, height, true);
        }

		public static Bitmap PrepareColourBitmapForDisplay(int[, ,] imageArray, int width, int height)
		{
			return PrepareColourBitmapForDisplay(imageArray, width, height, false);
		}

		public static Bitmap PrepareColourBitmapForDisplay(object[, ,] imageArray, int width, int height)
		{
			return PrepareColourBitmapForDisplay(imageArray, width, height, true);
		}

		public static object GetMonochromePixelsFromBitmap(Bitmap bitmap, LumaConversionMode conversionMode)
		{
			int[,] bitmapBytes = new int[bitmap.Width, bitmap.Height];

			IntPtr hBitmap = bitmap.GetHbitmap();
			try
			{
				GetMonochromePixelsFromBitmap(bitmap.Width, bitmap.Height, 8, hBitmap, bitmapBytes, (int)conversionMode);
			}
			finally
			{
				DeleteObject(hBitmap);
			}

			return bitmapBytes;
		}

		public static object GetColourPixelsFromBitmap(Bitmap bitmap, short flipMode)
		{
			int[,,] bitmapBytes = new int[bitmap.Width, bitmap.Height, 3];

			IntPtr hBitmap = bitmap.GetHbitmap();
			try
			{
                GetColourPixelsFromBitmap(bitmap.Width, bitmap.Height, 8, hBitmap, bitmapBytes, (short)flipMode);
			}
			finally
			{
				DeleteObject(hBitmap);
			}

			return bitmapBytes;			
		}

	    private static Bitmap PrepareBitmapForDisplay(object imageArray, int width, int height, bool useVariantPixels)
		{
			Bitmap displayBitmap = null;

			int[,] pixels = new int[height, width];

			if (useVariantPixels)
			{
                Array safeArr = (Array)imageArray;
				Array.Copy(safeArr, pixels, pixels.Length);
			}
			else
			{
                Array safeArr = (Array)imageArray;
				Array.Copy(safeArr, pixels, pixels.Length);
			}

			byte[] rawBitmapBytes = new byte[(width * height * 3) + 40 + 14 + 1];

            GetBitmapPixels(width, height, (int)8, pixels, rawBitmapBytes);

			using (MemoryStream memStr = new MemoryStream(rawBitmapBytes))
			{
				displayBitmap = (Bitmap)Image.FromStream(memStr);
			}

			return displayBitmap;
		}

		private static Bitmap PrepareColourBitmapForDisplay(object imageArray, int width, int height, bool useVariantPixels)
		{
			Bitmap displayBitmap = null;

			int[,,] pixels = new int[height, width, 3];

			if (useVariantPixels)
			{
				Array safeArr = (Array)imageArray;
				Array.Copy(safeArr, pixels, pixels.Length);
			}
			else
			{
				Array safeArr = (Array)imageArray;
				Array.Copy(safeArr, pixels, pixels.Length);
			}

			byte[] rawBitmapBytes = new byte[(width * height * 3) + 40 + 14 + 1];

			GetColourBitmapPixels(width, height, (int)8, pixels, rawBitmapBytes);

			using (MemoryStream memStr = new MemoryStream(rawBitmapBytes))
			{
				displayBitmap = (Bitmap)Image.FromStream(memStr);
			}

			return displayBitmap;
		}

        public static void StartRecordingVideoFile(string fileName)
        {
            StartRecording(fileName);
        }

        public static void StopRecordingVideoFile()
        {
            StopRecording();
        }

        public static FrameProcessingStatus ProcessVideoFrame2(int[,] pixels)
        {
            var frameInfo = new FrameProcessingStatus();

            long currentUtcDayAsTicks = DateTime.UtcNow.Ticks;


            ProcessVideoFrame2(pixels, currentUtcDayAsTicks, ref frameInfo);

            return frameInfo;
        }

        public static FrameProcessingStatus ProcessVideoFrame(IntPtr bitmapData)
        {
            var frameInfo = new FrameProcessingStatus();

            long currentUtcDayAsTicks = DateTime.UtcNow.Ticks;

            ProcessVideoFrame(bitmapData, currentUtcDayAsTicks, ref frameInfo);

            return frameInfo;
        }

        private static int imageWidth;
        private static int imageHeight;
	    private static Font s_ErrorFont = new Font(FontFamily.GenericMonospace, 9f, GraphicsUnit.Pixel);

        public static void SetupCamera(string cameraModel, int width, int height, 
			bool flipHorizontally, bool flipVertically,
			bool isIntegrating, float differenceRatio, float minSignDiff, float gammaDiff)
        {
            imageWidth = width;
            imageHeight = height;

			SetupCamera(width, height, cameraModel, 0, flipHorizontally, flipVertically, isIntegrating);
			SetupIntegrationDetection(differenceRatio, minSignDiff, gammaDiff);
        }

        public static void ReconfigureIntegrationDetection(float differenceRatio, float minSignDiff, float gammaDiff)
        {
			SetupIntegrationDetection(differenceRatio, minSignDiff, gammaDiff);
        }

		private static AssemblyFileVersionAttribute ASSEMBLY_FILE_VERSION = (AssemblyFileVersionAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyFileVersionAttribute), true)[0];

        public static void SetupAav(AavImageLayout imageLayout)
        {
            SetupAav(
                (int)imageLayout, 
                Settings.Default.UsesBufferedFrameProcessing ? 1 : 0,
                Settings.Default.IntegrationDetectionTuning ? 1 : 0,
                string.Format("AAVRec v{0}", ASSEMBLY_FILE_VERSION.Version));
        }

		public static string SetupTimestampPreservation(int width, int height)
		{
			int hr = SetupIntegrationPreservationArea(
				Settings.Default.PreserveTSLineTop,
				Settings.Default.PreserveTSLineTop + 1,
				Settings.Default.PreserveTSAreaHeight);

			if (hr != 0)
				return "Could not configure the timestamp preservation.";
			else
				return null;
		}

		public static string SetupBasicOcrMetrix(OcrConfiguration ocrConfig)
		{
			int[] zonePixels = ocrConfig.Zones.Any()
				? ocrConfig.Zones.Select(x => x.Pixels.Count).ToArray()
				: new int[0];

			if (ocrConfig.Zones.Any(x => x.ZoneId < 0))
				return "ZoneIds must be greater or equal than zero.";

			if (ocrConfig.Zones.Any(x => x.ZoneId >= ocrConfig.Zones.Count))
				return "Each ZoneId must equal the index of the zone in the zone list.";

			SetupIntegrationPreservationArea(
				ocrConfig.Alignment.FrameTopOdd,
				ocrConfig.Alignment.FrameTopEven,
				ocrConfig.Alignment.CharHeight);

            int hr = SetupOcrAlignment(
				 ocrConfig.Alignment.Width,
				 ocrConfig.Alignment.Height,
				 ocrConfig.Alignment.FrameTopOdd,
				 ocrConfig.Alignment.FrameTopEven,
				 ocrConfig.Alignment.CharWidth,
				 ocrConfig.Alignment.CharHeight,
				 ocrConfig.Alignment.CharPositions.Count,
				 ocrConfig.Zones.Count,
				 zonePixels);

	        if (hr != 0)
	            return "OCR config is incompatible with the current device.";
	        else
	            return null;
	    }

		public static void SetupOcr(OcrConfiguration ocrConfig)
        {
            // Build the ocr zone matrix in managed world using the OcrZoneChecker
            var zoneChecker = new OcrZoneChecker(
				ocrConfig,
				ocrConfig.Alignment.Width,
				ocrConfig.Alignment.Height,
				ocrConfig.Zones,
				ocrConfig.Alignment.CharPositions);

			foreach (CharDefinition charDef in ocrConfig.CharDefinitions)
            {
                SetupOcrChar(charDef.Character[0], charDef.FixedPosition.HasValue ? charDef.FixedPosition.Value : -1);

                foreach (ZoneSignature zoneSignt in charDef.ZoneSignatures)
                {
	                OcrZone zone = ocrConfig.Zones.Single(z => z.ZoneId == zoneSignt.ZoneId);
					int pixelsInZone = zone.Pixels.Count;
                    SetupOcrCharDefinitionZone(charDef.Character[0], zoneSignt.ZoneId, (int)zoneSignt.ZoneValue, pixelsInZone);
                }
            }

			//for (int y = 0; y < ocrConfig.Alignment.Height; y++) 
			//{
			//	for (int x = 0; x < ocrConfig.Alignment.Width; x++)
			//	{
			//		if (zoneChecker.OcrPixelMap[y, x] != 0)
			//		{
			//			int charId;
			//			bool isOddField;
			//			int zoneId;
			//			int zonePixelId;

			//			OcrZoneChecker.UnpackValue(zoneChecker.OcrPixelMap[y, x], out charId, out isOddField, out zoneId, out zonePixelId);
			//			Trace.WriteLine(string.Format("[{0},{1}] = {2}|({3},{4},{5},{6})", x, y, zoneChecker.OcrPixelMap[y, x], charId, isOddField ? "O" : "E", zoneId, zonePixelId));
			//		}
			//	}
			//}

			SetupOcrZoneMatrix(zoneChecker.OcrPixelMap);
        }

        public static void DisableOcr()
        {
            DisableOcrProcessing();
        }

        public static Bitmap GetCurrentImage(out ImageStatus status)
        {
            Bitmap videoFrame = null;
            status = new ImageStatus();

            byte[] bitmapPixels = new byte[3 * imageWidth * imageHeight + 40 + 14 + 1];

            GetCurrentImage(bitmapPixels);
            GetCurrentImageStatus(status);

            using (MemoryStream memStr = new MemoryStream(bitmapPixels))
            {
                try
                {
                    videoFrame = (Bitmap)Bitmap.FromStream(memStr);
                }
                catch (Exception ex)
                {
                    videoFrame = new Bitmap(imageWidth, imageHeight);
                    using (Graphics g = Graphics.FromImage(videoFrame))
                    {
                        g.Clear(Color.White);
                        g.DrawString(ex.Message, s_ErrorFont, Brushes.Red, 10, 10);
                        g.Save();
                    }
                }
            }

            return videoFrame;
        }

		public static bool LockIntegration()
        {
            int hr = LockIntegration(true);
            return hr >= 0;
        }

        public static bool UnlockIntegration()
        {
            int hr = LockIntegration(false);
            return hr >= 0;
        }

		public static bool StartIntegrationCalibration(int cameraIntegration)
		{
			int hr = ControlIntegrationCalibration(cameraIntegration);
			return hr >= 0;		
		}

		public static bool StopIntegrationCalibration()
		{
			int hr = ControlIntegrationCalibration(0);
			return hr >= 0;
		}

        public static Dictionary<float, List<float>> GetIntegrationCalibrationData()
        {
            var rv = new Dictionary<float, List<float>>();

            int gammasLength = 0;
            int signaturesPerCycle = 0;

            GetIntegrationCalibrationDataConfig(ref gammasLength, ref signaturesPerCycle);

            var rawSignatures = new float[signaturesPerCycle * gammasLength];
            var gammas = new float[gammasLength];

            GetIntegrationCalibrationData(rawSignatures, gammas);

            for (int i = 0; i < gammasLength; i++)
            {
                List<float> signatures = rawSignatures.Skip(i * signaturesPerCycle).Take(signaturesPerCycle).ToList();
                rv.Add(gammas[i], signatures);
            }

            return rv;
        }

		public static bool InitIntegrationDetectionTesting(float differenceRatio, float minimumDifference)
        {
			int rv = InitNewIntegrationPeriodTesting(differenceRatio, minimumDifference);
            return rv >= 0;
        }

	    public static bool IntegrationDetectionTestNextFrame(long frameNo, float diffSignature)
	    {
	        bool isNew = false;
            TestNewIntegrationPeriod(frameNo, diffSignature, ref isNew);
	        return isNew;
	    }
	}
}
