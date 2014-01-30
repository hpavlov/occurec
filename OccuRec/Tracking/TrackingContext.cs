using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OccuRec.Helpers;
using OccuRec.Properties;

namespace OccuRec.Tracking
{
	public enum TrackingFrequency
	{
		EveryFrame,
		EverySecondFrame,
		EveryThirdFrame,
		EveryFourthFrame,
		EveryFifthFrame,
		EveryTenthFrame,
		EveryTwentiethFrame
	}

	internal class LastTrackedPosition
	{
		public readonly int Bpp;
		public float X;
		public float Y;
		public float FWHM;
		public bool IsFixed;
        public bool IsFullDisapearance;
        public bool IsLocated;
		public bool HasSaturatedPixels;
		public float Measurement;

		public NativeTrackedObjectPsfFit PsfFit;

		public LastTrackedPosition(int bpp)
		{
			Bpp = bpp;
			PsfFit = new NativeTrackedObjectPsfFit(bpp);
		}
	}

	internal class TrackingContext
	{
		public static TrackingContext Current = new TrackingContext();

		private TrackingContext()
		{
			Reset();
		}

		public void Reset()
		{
			LastTrackedFrameNo = -1;
			GuidingStar = null;
			TargetStar = null;
			IsTracking = false;
			TrackedObjectId = -1;
			GuidingObjectId = -1;
		}

		public bool IsTracking { get; private set; }

		internal int TrackedObjectId { get; private set; }

		internal int GuidingObjectId { get; private set; }

		public long LastTrackedFrameNo { get; private set; }
		
		public LastTrackedPosition GuidingStar;

		public LastTrackedPosition TargetStar;

		internal TrackedObjectConfig GuidingStarConfig;

		internal TrackedObjectConfig TargetStarConfig;

		public void ReConfigureNativeTracking(int width, int height)
		{
			// Stop running the tracking on new video frames
			IsTracking = false;
			NativeHelpers.StopTracking();

			// Configure the native tracker
			NativeTracking.ConfigureNativeTracker();

			int numObjects = 0;
			if (GuidingStar != null) numObjects++;
			if (TargetStar != null) numObjects++;

            NativeTracking.InitNewTracker(width, height, numObjects, TargetStar != null && TargetStar.IsFullDisapearance);

			TrackedObjectId = -1;
			GuidingObjectId = -1;

			if (TargetStar != null)
			{
				TrackedObjectId = 0;
				TargetStarConfig = new TrackedObjectConfig()
				{
					IsFixedAperture = TargetStar.IsFixed,
                    IsFullDisapearance = TargetStar.IsFullDisapearance,
					TrackingType = TrackingType.OccultedStar,
					ApertureStartingX = TargetStar.X,
					ApertureStartingY = TargetStar.Y,
					ApertureInPixels = TargetStar.FWHM * Settings.Default.TrackingApertureInFWHM
				};

				NativeTracking.ConfigureTrackedObject(TrackedObjectId, TargetStarConfig);
			}

			if (GuidingStar != null)
			{
				GuidingObjectId = TargetStar != null ? 1 : 0;
				GuidingStarConfig = new TrackedObjectConfig()
				{
					IsFixedAperture = false,
					TrackingType = TrackingType.GuidingStar,
					ApertureStartingX = GuidingStar.X,
					ApertureStartingY = GuidingStar.Y,
					ApertureInPixels = GuidingStar.FWHM * Settings.Default.TrackingApertureInFWHM
				};

				NativeTracking.ConfigureTrackedObject(GuidingObjectId, GuidingStarConfig);
			}

			NativeTracking.InitialiseNewTracking();

			// Start running the tracking on new video frames
			int frequency = GetFrequencyFromTrackingFrequency((TrackingFrequency)Settings.Default.TrackingFrequency);

			NativeHelpers.StartTracking(TrackedObjectId, GuidingObjectId, frequency,
				TargetStarConfig != null ? (float)TargetStarConfig.ApertureInPixels : 0,
				GuidingStarConfig != null ? (float)GuidingStarConfig.ApertureInPixels : 0,
				Settings.Default.TrackingInnerRadiusOfAnnulus, Settings.Default.TrackingMinNumberPixelsInAnnulus);

			IsTracking = true;
		}

		private int GetFrequencyFromTrackingFrequency(TrackingFrequency frequency)
		{
			switch (frequency)
			{
				case TrackingFrequency.EveryFrame:
					return 1;

				case TrackingFrequency.EverySecondFrame:
					return 2;

				case TrackingFrequency.EveryThirdFrame:
					return 3;

				case TrackingFrequency.EveryFourthFrame:
					return 4;

				case TrackingFrequency.EveryFifthFrame:
					return 5;

				case TrackingFrequency.EveryTenthFrame:
					return 10;

				case TrackingFrequency.EveryTwentiethFrame:
					return 20;

				default:
					return 1;
			}
		}

		public void UpdateFromFrameStatus(long frameNo, FrameProcessingStatus status)
		{
			bool updatedMade = false;

			if (TrackedObjectId != -1 && status.TrkdTargetIsTracked > 0)
			{
				TargetStar.X = status.TrkdTargetXPos;
				TargetStar.Y = status.TrkdTargetYPos;
				TargetStar.FWHM = status.TrkdTargetPsfInfo.FWHM;
				TargetStar.IsLocated = status.TrkdTargetIsLocated > 0;
				TargetStar.HasSaturatedPixels = status.TrkdTargetHasSaturatedPixels > 0;
				TargetStar.Measurement = status.TrkdTargetMeasurement;
				TargetStar.PsfFit.LoadFromNativePsfFitInfo(status.TrkdTargetPsfInfo, status.TrkdTargetResiduals);

				updatedMade = true;
			}

			if (GuidingObjectId != -1 && status.TrkdGuidingIsTracked > 0)
			{
				GuidingStar.X = status.TrkdGuidingXPos;
				GuidingStar.Y = status.TrkdGuidingYPos;
				GuidingStar.FWHM = status.TrkdGuidingPsfInfo.FWHM;
				GuidingStar.IsLocated = status.TrkdGuidingIsLocated > 0;
				GuidingStar.HasSaturatedPixels = status.TrkdGuidingHasSaturatedPixels > 0;
				GuidingStar.Measurement = status.TrkdGuidingMeasurement;
				GuidingStar.PsfFit.LoadFromNativePsfFitInfo(status.TrkdGuidingPsfInfo, status.TrkdGuidingResiduals);

				updatedMade = true;
			}

			if (updatedMade)
				LastTrackedFrameNo = frameNo;
		}

		public void UpdateFromFrameStatus(long frameNo, ImageStatus status)
		{
			bool updatedMade = false;

			if (TrackedObjectId != -1 && status.TrkdTargetIsTracked > 0)
			{
				TargetStar.X = status.TrkdTargetXPos;
				TargetStar.Y = status.TrkdTargetYPos;
				TargetStar.FWHM = status.TrkdTargetPsfInfo.FWHM;
				TargetStar.IsLocated = status.TrkdTargetIsLocated > 0;
				TargetStar.HasSaturatedPixels = status.TrkdTargetHasSaturatedPixels > 0;
				TargetStar.Measurement = status.TrkdTargetMeasurement;
				TargetStar.PsfFit.LoadFromNativePsfFitInfo(status.TrkdTargetPsfInfo, status.TrkdTargetResiduals);

				updatedMade = true;
			}

			if (GuidingObjectId != -1 && status.TrkdGuidingIsTracked > 0)
			{
				GuidingStar.X = status.TrkdGuidingXPos;
				GuidingStar.Y = status.TrkdGuidingYPos;
				GuidingStar.FWHM = status.TrkdGuidingPsfInfo.FWHM;
				GuidingStar.IsLocated = status.TrkdGuidingIsLocated > 0;
				GuidingStar.HasSaturatedPixels = status.TrkdGuidingHasSaturatedPixels > 0;
				GuidingStar.Measurement = status.TrkdGuidingMeasurement;
				GuidingStar.PsfFit.LoadFromNativePsfFitInfo(status.TrkdGuidingPsfInfo, status.TrkdGuidingResiduals);

				updatedMade = true;
			}

			if (updatedMade)
				LastTrackedFrameNo = frameNo;
		}
	}
}
