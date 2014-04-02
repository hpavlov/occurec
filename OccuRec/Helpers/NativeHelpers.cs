using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using OccuRec.OCR;
using OccuRec.Properties;
using OccuRec.Tracking;
using OccuRec.Utilities;

namespace OccuRec.Helpers
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
        CompressedRaw = 4,
		StatusSectionOnly = 5
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
		public int OcrWorking;
		public int OcrErrorsSinceLastReset;
	    public int UserIntegratonRateHint;
        public int TrkdTargetIsLocated;
		public float TrkdTargetXPos;
		public float TrkdTargetYPos;
		public int TrkdTargetIsTracked;
		public float TrkdTargetMeasurement;
	    public int TrkdTargetHasSaturatedPixels;
        public int TrkdGuidingIsLocated;
		public float TrkdGuidingXPos;
		public float TrkdGuidingYPos;
		public int TrkdGuidingIsTracked;
		public float TrkdGuidingMeasurement;
		public int TrkdGuidingHasSaturatedPixels;

		public NativePsfFitInfo TrkdTargetPsfInfo = new NativePsfFitInfo();

		public NativePsfFitInfo TrkdGuidingPsfInfo = new NativePsfFitInfo();

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 290)]
		public double[] TrkdTargetResiduals;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 290)]
		public double[] TrkdGuidingResiduals;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct FrameProcessingStatus
    {
		// TODO: Why are we using a struct that duplicates the ImageStatus ??

        public static FrameProcessingStatus Empty = new FrameProcessingStatus();

        public long CameraFrameNo;
        public long IntegratedFrameNo;
		public int CountedFrames;
        public float FrameDiffSignature;
        public float CurrentSignatureRatio;
        public int PerformedAction;
        public float PerformedActionProgress;
		public int DetectedIntegrationRate;
		public int DropedFramesSinceIntegrationLock;
		public long StartExposureSystemTime;
		public long EndExposureSystemTime;
		public long StartExposureFrameNo;
		public long EndExposureFrameNo;
		public int OcrWorking;
		public int OcrErrorsSinceLastReset;
	    public int UserIntegratonRateHint;
        public int TrkdTargetIsLocated;
		public float TrkdTargetXPos;
		public float TrkdTargetYPos;
		public int TrkdTargetIsTracked;
		public float TrkdTargetMeasurement;
	    public int TrkdTargetHasSaturatedPixels;
        public int TrkdGuidingIsLocated;
		public float TrkdGuidingXPos;
		public float TrkdGuidingYPos;
		public int TrkdGuidingIsTracked;
		public float TrkdGuidingMeasurement;
		public int TrkdGuidingHasSaturatedPixels;
		
		public NativePsfFitInfo TrkdTargetPsfInfo;
		public NativePsfFitInfo TrkdGuidingPsfInfo;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 290)]
		public double[] TrkdTargetResiduals;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 290)]
		public double[] TrkdGuidingResiduals;

		public static FrameProcessingStatus Clone(FrameProcessingStatus cloneFrom)
        {
            var rv = new FrameProcessingStatus();
            rv.CameraFrameNo = cloneFrom.CameraFrameNo;
            rv.IntegratedFrameNo = cloneFrom.IntegratedFrameNo;
			rv.CountedFrames = cloneFrom.CountedFrames;
            rv.FrameDiffSignature = cloneFrom.FrameDiffSignature;
            rv.CurrentSignatureRatio = cloneFrom.CurrentSignatureRatio;
            rv.PerformedAction = cloneFrom.PerformedAction;
            rv.PerformedActionProgress = cloneFrom.PerformedActionProgress;
            rv.TrkdGuidingXPos = cloneFrom.TrkdGuidingXPos;
            rv.TrkdGuidingYPos = cloneFrom.TrkdGuidingYPos;
			rv.TrkdGuidingIsTracked = cloneFrom.TrkdGuidingIsTracked;
            rv.TrkdGuidingMeasurement = cloneFrom.TrkdGuidingMeasurement;
            rv.TrkdTargetXPos = cloneFrom.TrkdTargetXPos;
            rv.TrkdTargetYPos = cloneFrom.TrkdTargetYPos;
			rv.TrkdTargetIsTracked = cloneFrom.TrkdTargetIsTracked;
            rv.TrkdTargetMeasurement = cloneFrom.TrkdTargetMeasurement;
            rv.TrkdTargetIsLocated = cloneFrom.TrkdTargetIsLocated;
            rv.TrkdGuidingIsLocated = cloneFrom.TrkdGuidingIsLocated;
			rv.TrkdTargetHasSaturatedPixels = cloneFrom.TrkdTargetHasSaturatedPixels;
			rv.TrkdGuidingHasSaturatedPixels = cloneFrom.TrkdGuidingHasSaturatedPixels;
			rv.TrkdTargetResiduals = cloneFrom.TrkdTargetResiduals;
			rv.TrkdGuidingResiduals = cloneFrom.TrkdGuidingResiduals;

            return rv;
        }

		public FrameProcessingStatus(ImageStatus imgStatus)
		{
			FrameDiffSignature = 0;
			CameraFrameNo = imgStatus.UniqueFrameNo;
			CurrentSignatureRatio = imgStatus.CutOffRatio;
			IntegratedFrameNo = imgStatus.IntegratedFrameNo;
			CountedFrames = imgStatus.CountedFrames;
			DetectedIntegrationRate = imgStatus.DetectedIntegrationRate;
			DropedFramesSinceIntegrationLock = imgStatus.DropedFramesSinceIntegrationLock;
			PerformedAction = imgStatus.PerformedAction;
			PerformedActionProgress = imgStatus.PerformedActionProgress;
			StartExposureSystemTime = imgStatus.StartExposureSystemTime;
			EndExposureSystemTime = imgStatus.EndExposureSystemTime;
			StartExposureFrameNo = imgStatus.StartExposureFrameNo;
			EndExposureFrameNo = imgStatus.EndExposureFrameNo;
			OcrWorking = imgStatus.OcrWorking;
			OcrErrorsSinceLastReset = imgStatus.OcrErrorsSinceLastReset;
			UserIntegratonRateHint = imgStatus.UserIntegratonRateHint;
			TrkdGuidingXPos = imgStatus.TrkdGuidingXPos;
			TrkdGuidingYPos = imgStatus.TrkdGuidingYPos;
			TrkdGuidingIsTracked = imgStatus.TrkdGuidingIsTracked;
			TrkdGuidingMeasurement = imgStatus.TrkdGuidingMeasurement;
			TrkdTargetXPos = imgStatus.TrkdTargetXPos;
			TrkdTargetYPos = imgStatus.TrkdTargetYPos;
			TrkdTargetIsTracked = imgStatus.TrkdTargetIsTracked;
			TrkdTargetMeasurement = imgStatus.TrkdTargetMeasurement;
            TrkdTargetIsLocated = imgStatus.TrkdTargetIsLocated;
            TrkdGuidingIsLocated = imgStatus.TrkdGuidingIsLocated;
			TrkdTargetHasSaturatedPixels = imgStatus.TrkdTargetHasSaturatedPixels;
			TrkdGuidingHasSaturatedPixels = imgStatus.TrkdGuidingHasSaturatedPixels;
			TrkdTargetResiduals = imgStatus.TrkdTargetResiduals;
			TrkdGuidingResiduals = imgStatus.TrkdGuidingResiduals;
			TrkdTargetPsfInfo = imgStatus.TrkdTargetPsfInfo;
			TrkdGuidingPsfInfo = imgStatus.TrkdGuidingPsfInfo;
		}
    };

	internal static class NativeHelpers
	{
        private const string OCCUREC_CORE_DLL_NAME = "OccuRec.Core.dll";

		[DllImport(OCCUREC_CORE_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern int GetBitmapPixels(
			int width, 
			int height, 
			int bpp,
			[In, MarshalAs(UnmanagedType.LPArray)] int[,] pixels,
			[In, Out] byte[] bitmapBytes);

		[DllImport(OCCUREC_CORE_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GetColourBitmapPixels(
			int width,
			int height,
			int bpp,
			[In, MarshalAs(UnmanagedType.LPArray)] int[,,] pixels,
			[In, Out] byte[] bitmapBytes);

		[DllImport(OCCUREC_CORE_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GetMonochromePixelsFromBitmap(
			int width,
			int height,
			int bpp,
			int flipMode,
			[In] IntPtr hBitmap,
			[In, Out, MarshalAs(UnmanagedType.LPArray)] int[,] bitmapBytes,
			short mode);

		[DllImport(OCCUREC_CORE_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GetColourPixelsFromBitmap(
			int width,
			int height,
			int bpp,
			int flipMode,
			[In] IntPtr hBitmap,
			[In, Out, MarshalAs(UnmanagedType.LPArray)] int[,,] bitmapBytes);

        [DllImport(OCCUREC_CORE_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SetupCamera(
            int width, 
            int height, 
            string cameraModel, 
            int monochromeConversionMode,
            bool flipHorizontally, 
            bool flipVertically,
            bool isIntegrating);

	    [DllImport(OCCUREC_CORE_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SetupGrabberInfo(string grabberName, string videoMode, float videoFrameRate, int hardwareTimingCorrection);

	    [DllImport(OCCUREC_CORE_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
		private static extern int SetupIntegrationDetection(float differenceRatio, float minSignDiff, float diffGamma);

	    [DllImport(OCCUREC_CORE_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
		private static extern int SetupAav(int imageLayout, int bpp, int usesBufferedMode, int integrationDetectionTuning, string occuRecVersion, int recordNtpTimestamp, int recordSecondaryTimestamp);

		[DllImport(OCCUREC_CORE_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
		private static extern int SetupNtpDebugParams(int debugParam1, float debugParam2);

        [DllImport(OCCUREC_CORE_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
		private static extern int ProcessVideoFrame([In] IntPtr ptrBitmapData, long currentUtcDayAsTicks, long currentNtpTimeAsTicks, double ntpBasedTimeError, long currentSecondaryTimeAsTicks, [In, Out] ref FrameProcessingStatus frameInfo);

        [DllImport(OCCUREC_CORE_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
		private static extern int ProcessVideoFrame2([In, MarshalAs(UnmanagedType.LPArray)] int[,] pixel, long currentUtcDayAsTicks, long currentNtpTimeAsTicks, double ntpBasedTimeError, long currentSecondaryTimeAsTicks, [In, Out] ref FrameProcessingStatus frameInfo);

        [DllImport(OCCUREC_CORE_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern int GetCurrentImage([In, Out] byte[] bitmapPixels);

        [DllImport(OCCUREC_CORE_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern int GetCurrentImageStatus([In, Out] ImageStatus status);

        [DllImport(OCCUREC_CORE_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern int StartRecording(string fileName);

        [DllImport(OCCUREC_CORE_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern int StopRecording();

		[DllImport(OCCUREC_CORE_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
		private static extern int StartOcrTesting(string fileName);

        [DllImport(OCCUREC_CORE_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern int LockIntegration(bool doLock);

		[DllImport(OCCUREC_CORE_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
		private static extern int SetManualIntegrationHint(int hintRate);

		[DllImport(OCCUREC_CORE_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
		private static extern int ControlIntegrationCalibration(int cameraIntegration);

        [DllImport(OCCUREC_CORE_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern int GetIntegrationCalibrationDataConfig([In, Out] ref int gammasLength, [In, Out] ref int signaturesPerCycle);

        [DllImport(OCCUREC_CORE_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern int GetIntegrationCalibrationData([In, MarshalAs(UnmanagedType.LPArray)] float[] rawSignatures, [In, MarshalAs(UnmanagedType.LPArray)] float[] gammas);

        [DllImport(OCCUREC_CORE_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern int InitNewIntegrationPeriodTesting(float differenceRatio, float minimumDifference);

        [DllImport(OCCUREC_CORE_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern int TestNewIntegrationPeriod(long frameNo, float diffSignature, [In, Out] ref bool isNew);

		[DllImport(OCCUREC_CORE_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
		private static extern int SetupIntegrationPreservationArea(bool preserveVti, int areaTopOdd, int areaTopEven, int areaHeight);

        [DllImport(OCCUREC_CORE_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
		private static extern int SetupOcrAlignment(int width, int height, int frameTopOdd, int frameTopEven, int charWidth, int charHeight, int numberOfCharPositions, int numberOfZones, int zoneMode, [In, MarshalAs(UnmanagedType.LPArray)] int[] pixelsInZones);

        [DllImport(OCCUREC_CORE_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
	    private static extern int SetupOcrZoneMatrix([In, MarshalAs(UnmanagedType.LPArray)] int[,] matrix);

	    [DllImport(OCCUREC_CORE_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
	    private static extern int SetupOcrChar(char character, int fixedPosition);

        [DllImport(OCCUREC_CORE_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SetupOcrCharDefinitionZone(char character, int zoneId, int zoneValue, int zonePixelsCount);

        [DllImport(OCCUREC_CORE_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern int DisableOcrProcessing();

		[DllImport(OCCUREC_CORE_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
		private static extern int EnableTracking(int targetObjectId, int guidingObjectId, int frequency, float targetAperture, float guidingAperture, float innerRadiusOfBackgroundApertureInSignalApertures, long numberOfPixelsInBackgroundAperture);

		[DllImport(OCCUREC_CORE_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
		private static extern int DisableTracking();

        [DllImport("Kernel32.dll", EntryPoint = "RtlMoveMemory")]
        public static extern void CopyMemory(IntPtr Destination, IntPtr Source, [MarshalAs(UnmanagedType.U4)] uint Length);

		[DllImport("gdi32.dll")]
		public static extern bool DeleteObject(IntPtr hObject);

		[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool GetDiskFreeSpaceEx(string lpDirectoryName, out ulong lpFreeBytesAvailable, out ulong lpTotalNumberOfBytes, out ulong lpTotalNumberOfFreeBytes);

		public static bool GetDriveFreeBytes(string folderName, out ulong freespace)
		{
			freespace = 0;
			if (string.IsNullOrEmpty(folderName))
			{
				throw new ArgumentNullException("folderName");
			}

			if (!folderName.EndsWith("\\"))
			{
				folderName += '\\';
			}

			ulong free = 0, dummy1 = 0, dummy2 = 0;

			if (GetDiskFreeSpaceEx(folderName, out free, out dummy1, out dummy2))
			{
				freespace = free;
				return true;
			}
			else
			{
				return false;
			}
		}


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

		public static object GetMonochromePixelsFromBitmap(Bitmap bitmap, LumaConversionMode conversionMode, short flipMode)
		{
			int[,] bitmapBytes = new int[bitmap.Width, bitmap.Height];

			IntPtr hBitmap = bitmap.GetHbitmap();
			try
			{
				GetMonochromePixelsFromBitmap(bitmap.Width, bitmap.Height, 8, flipMode, hBitmap, bitmapBytes, (short)conversionMode);
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
				GetColourPixelsFromBitmap(bitmap.Width, bitmap.Height, 8, flipMode, hBitmap, bitmapBytes);
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

		public static void StartOcrTestRecording(string fileName)
		{
			StartOcrTesting(fileName);
		}

        public static FrameProcessingStatus ProcessVideoFrame2(int[,] pixels)
        {
			double ntpBasedTimeError;
			long currentNtpTimeAsTicks = NTPTimeKeeper.UtcNow(out ntpBasedTimeError).AddMilliseconds(-1 * Settings.Default.NTPTimingHardwareCorrection).Ticks;
	        long currentSecondaryTimeAsTicks = DateTime.UtcNow.Ticks;

            var frameInfo = new FrameProcessingStatus();
	        frameInfo.TrkdTargetResiduals = new double[290];
			frameInfo.TrkdGuidingResiduals = new double[290];
			frameInfo.TrkdTargetPsfInfo = new NativePsfFitInfo();
			frameInfo.TrkdGuidingPsfInfo = new NativePsfFitInfo();

            long currentUtcDayAsTicks = DateTime.UtcNow.Date.Ticks;

			ProcessVideoFrame2(pixels, currentUtcDayAsTicks, currentNtpTimeAsTicks, ntpBasedTimeError, currentSecondaryTimeAsTicks, ref frameInfo);

            return frameInfo;
        }

        public static FrameProcessingStatus ProcessVideoFrame(IntPtr bitmapData)
        {
			// Get the NTP time from the internal NTP syncronised high precision clock
			double ntpBasedTimeError;
			long currentNtpTimeAsTicks = NTPTimeKeeper.UtcNow(out ntpBasedTimeError).AddMilliseconds(-1 * Settings.Default.NTPTimingHardwareCorrection).Ticks;
			long currentSecondaryTimeAsTicks = DateTime.UtcNow.Ticks;

            var frameInfo = new FrameProcessingStatus();
			frameInfo.TrkdTargetResiduals = new double[290];
			frameInfo.TrkdGuidingResiduals = new double[290];
			frameInfo.TrkdTargetPsfInfo = new NativePsfFitInfo();
			frameInfo.TrkdGuidingPsfInfo = new NativePsfFitInfo();

            long currentUtcDayAsTicks = DateTime.UtcNow.Date.Ticks;

            ProcessVideoFrame(bitmapData, currentUtcDayAsTicks, currentNtpTimeAsTicks, ntpBasedTimeError, currentSecondaryTimeAsTicks, ref frameInfo);

            return frameInfo;
        }

        private static int imageWidth;
        private static int imageHeight;
	    private static Font s_ErrorFont = new Font(FontFamily.GenericMonospace, 9f, GraphicsUnit.Pixel);

        public static void SetupCamera(string cameraModel, int width, int height, 
			bool flipHorizontally, bool flipVertically,
			bool isIntegrating, float differenceRatio, float minSignDiff, float gammaDiff,
			string grabberName, string videoMode, double frameRate)
        {
            imageWidth = width;
            imageHeight = height;

			SetupCamera(width, height, cameraModel, 0, flipHorizontally, flipVertically, isIntegrating);
			SetupIntegrationDetection(differenceRatio, minSignDiff, gammaDiff);
			SetupGrabberInfo(grabberName, videoMode, (float)frameRate, Settings.Default.NTPTimingHardwareCorrection);
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
				Settings.Default.Use16BitAAV ? 16 : 8,
                Settings.Default.UsesBufferedFrameProcessing ? 1 : 0,
                Settings.Default.IntegrationDetectionTuning ? 1 : 0,
                string.Format("OccuRec v{0}", ASSEMBLY_FILE_VERSION.Version),
				Settings.Default.RecordNTPTimeStampInAAV ? 1 : 0,
				Settings.Default.RecordSecondaryTimeStampInAavFile ? 1 : 0);

			SetupNtpDebugParams(Settings.Default.NTPDebugConfigValue1, Settings.Default.NTPDebugConfigValue2);
        }

		public static string SetupTimestampPreservation(bool enabled, int areaTopOdd, int areaHeight)
		{
			int hr = SetupIntegrationPreservationArea(
				enabled,
				areaTopOdd,
				areaTopOdd + 1,
				areaHeight);

			//int hr = SetupIntegrationPreservationArea(
			//	Settings.Default.PreserveVTIEnabled, 
			//	Settings.Default.PreserveVTIFirstRow, 
			//	Settings.Default.PreserveVTIFirstRow + 1, 
			//	(Settings.Default.PreserveVTILastRow - Settings.Default.PreserveVTIFirstRow - 1) / 2);

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
				true,
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
				 (int)ocrConfig.Mode,
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

		public static void StopTracking()
		{
			DisableTracking();
		}

		public static void StartTracking(int targetObjectId, int guidingObjectId, int frequency, float targetAperture, float guidingAperture, float innerRadiusOfBackgroundApertureInSignalApertures, long numberOfPixelsInBackgroundAperture)
		{
			EnableTracking(targetObjectId, guidingObjectId, frequency, targetAperture, guidingAperture, innerRadiusOfBackgroundApertureInSignalApertures, numberOfPixelsInBackgroundAperture);
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
                    try
                    {
                        videoFrame = new Bitmap(imageWidth, imageHeight);
                        using (Graphics g = Graphics.FromImage(videoFrame))
                        {
                            g.Clear(Color.White);
                            g.DrawString(ex.Message, s_ErrorFont, Brushes.Red, 10, 10);
                            g.Save();
                        }

                    }
                    catch(Exception ex2)
                    {
                        Trace.WriteLine(ex2.GetFullStackTrace());
                    }

                    Trace.WriteLine(ex.GetFullStackTrace());
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

		public static void SetManualIntegrationRateHint(int hintRate)
		{
			SetManualIntegrationHint(hintRate);
		}
	}
}
