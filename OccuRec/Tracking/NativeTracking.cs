using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using OccuRec.Properties;

namespace OccuRec.Tracking
{
	[StructLayout(LayoutKind.Explicit)]
	internal class NativeTrackedObjectInfo
	{
		[FieldOffset(0)]
		public float CenterX;
		[FieldOffset(4)]
		public float CenterY;
		[FieldOffset(8)]
		public float LastGoodPositionX;
		[FieldOffset(12)]
		public float LastGoodPositionY;
		[FieldOffset(16)]
		public byte IsLocated;
		[FieldOffset(17)]
		public byte IsOffScreen;
		[FieldOffset(18)]
		public ushort TrackingFlags;
	}

	[StructLayout(LayoutKind.Explicit)]
	internal class NativePsfFitInfo
	{
		[FieldOffset(0)]
		public float XCenter;
		[FieldOffset(4)]
		public float YCenter;
		[FieldOffset(8)]
		public float FWHM;
		[FieldOffset(12)]
		public float IMax;
		[FieldOffset(16)]
		public float I0;
		[FieldOffset(20)]
		public float X0;
		[FieldOffset(24)]
		public float Y0;
		[FieldOffset(28)]
		public byte MatrixSize;
		[FieldOffset(29)]
		public byte IsSolved;

		[FieldOffset(30)]
		public byte IsAsymmetric;
		[FieldOffset(31)]
		public byte Reserved;
		[FieldOffset(32)]
		public float R0;
		[FieldOffset(36)]
		public float R02;
	}

	internal static class NativeTracking
	{
		private const string OCCUREC_CORE_DLL_NAME = "OccuRec.Core.dll";

		[DllImport(OCCUREC_CORE_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
		// DLL_PUBLIC long TrackerSettings(double maxElongation, double minFWHM, double maxFWHM, double minCertainty);
		private static extern int TrackerSettings(double maxElongation, double minFWHM, double maxFWHM, double minCertainty);

		[DllImport(OCCUREC_CORE_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
		// DLL_PUBLIC long TrackerNewConfiguration(long width, long height, long numTrackedObjects, bool isFullDisappearance);
		private static extern int TrackerNewConfiguration(int width, int height, int numTrackedObjects, bool isFullDisappearance);

		[DllImport(OCCUREC_CORE_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
		// DLL_PUBLIC long TrackerNextFrame(long frameId, unsigned long* pixels);
		private static extern int TrackerNextFrame(int frameId, [In, Out] uint[] pixels);

		[DllImport(OCCUREC_CORE_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
		// DLL_PUBLIC long TrackerConfigureObject(long objectId, bool isFixedAperture, bool isOccultedStar, double startingX, double startingY, double apertureInPixels);
		private static extern int TrackerConfigureObject(int objectId, bool isFixedAperture, bool isOccultedStar, double startingX, double startingY, double apertureInPixels);

		[DllImport(OCCUREC_CORE_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
		// DLL_PUBLIC long TrackerGetTargetState(long objectId, NativeTrackedObjectInfo* trackingInfo, NativePsfFitInfo* psfInfo, double* residuals);
		private static extern int TrackerGetTargetState(int objectId, [In, Out] NativeTrackedObjectInfo trackingInfo, [In, Out] NativePsfFitInfo psfInfo, [In, Out] double[] residuals);

		[DllImport(OCCUREC_CORE_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
		//DLL_PUBLIC void ConfigureSaturationLevels(unsigned long saturation8Bit, unsigned long saturation12Bit, unsigned long saturation14Bit);
		private static extern int ConfigureSaturationLevels(ulong saturation8Bit, ulong saturation12Bit, ulong saturation14Bit);

		[DllImport(OCCUREC_CORE_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
		//DLL_PUBLIC long TrackerInitialiseNewTracking();
		private static extern int TrackerInitialiseNewTracking();

		internal static void ConfigureNativeTracker()
		{
			TrackerSettings(
				Settings.Default.TrackingCheckElongation ? Settings.Default.TrackingMaxElongation : 0,
				Settings.Default.TrackingMinFWHM,
				Settings.Default.TrackingMaxFWHM,
				Settings.Default.TrackingMinCertainty);

			ConfigureSaturationLevels(
				Settings.Default.Saturation8Bit,
				Settings.Default.Saturation12Bit,
				Settings.Default.Saturation14Bit);

		}

		private static int s_NumTrackedObjects;

		internal static void InitNewTracker(int width, int height, int numTrackedObjects, bool isFullDisappearance)
		{
			int rv = TrackerNewConfiguration(width, height, numTrackedObjects, isFullDisappearance);

			if (rv == 0)
			{
				s_NumTrackedObjects = numTrackedObjects;
			}
		}

		internal static void InitialiseNewTracking()
		{
			TrackerInitialiseNewTracking();
		}

		internal static void ConfigureTrackedObject(int objectId, TrackedObjectConfig obj)
		{
			TrackerConfigureObject(
				objectId,
				obj.IsFixedAperture,
				obj.TrackingType == TrackingType.OccultedStar,
				obj.ApertureStartingX,
				obj.ApertureStartingY,
				obj.ApertureInPixels);
		}

		internal static bool TrackNextFrame(int frameId, uint[] pixels, List<NativeTrackedObject> managedTrackedObjects)
		{
			int rv = TrackerNextFrame(frameId, pixels);

			for (int i = 0; i < s_NumTrackedObjects; i++)
			{
				var trackingInfo = new NativeTrackedObjectInfo();
				var psfInfo = new NativePsfFitInfo();
				var residuals = new double[35 * 35];

				TrackerGetTargetState(i, trackingInfo, psfInfo, residuals);

				managedTrackedObjects[i].LoadFromNativeData(trackingInfo, psfInfo, residuals);
			}

			return rv == 0;
		}
	}
}
