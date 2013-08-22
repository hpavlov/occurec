using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using OccuRec.Helpers;
using OccuRec.Properties;

namespace OccuRec.Drivers
{
    public class BasicVideoFrame : IVideoFrame
    {
        private long? frameNumber;
        private string imageInfo;
        private double? exposureDuration;
        private string exposureStartTime;
        private object pixels;
        private object pixelsVariant;

        private static int s_Counter = 0;

        internal static BasicVideoFrame FakeFrame(int width, int height)
        {
            var rv = new BasicVideoFrame();
            s_Counter++;
            rv.frameNumber = s_Counter;

            rv.pixels = new int[0, 0];
            return rv;
        }

        internal static BasicVideoFrame CreateFrameVariant(int width, int height, Bitmap cameraFrame, int fameNumber)
        {
            return InternalCreateFrame(width, height, cameraFrame, fameNumber, true, FrameProcessingStatus.Empty);
        }

        internal static BasicVideoFrame CreateFrame(int width, int height, Bitmap cameraFrame, int fameNumber)
        {
            return InternalCreateFrame(width, height, cameraFrame, fameNumber, false, FrameProcessingStatus.Empty);
        }

        internal static BasicVideoFrame CreateFrameVariant(int width, int height, Bitmap cameraFrame, int fameNumber, FrameProcessingStatus status)
        {
            return InternalCreateFrame(width, height, cameraFrame, fameNumber, true, status);
        }

        internal static BasicVideoFrame CreateFrame(int width, int height, Bitmap cameraFrame, int fameNumber, FrameProcessingStatus status)
        {
            return InternalCreateFrame(width, height, cameraFrame, fameNumber, false, status);
        }

        private static BasicVideoFrame InternalCreateFrame(int width, int height, Bitmap cameraFrame, int fameNumber, bool variant, FrameProcessingStatus status)
        {
            var rv = new BasicVideoFrame();

            rv.pixels = ImageUtils.GetPixelArray(cameraFrame);

            rv.pixelsVariant = null;

            // TODO: Set these from the unmanaged OCR data, when native OCR is running

            rv.frameNumber = fameNumber;

            if (status.Equals(FrameProcessingStatus.Empty))
            {
                rv.exposureStartTime = null;
                rv.exposureDuration = null;
                rv.imageInfo = null;
            }
            else
            {
	            if (status.StartExposureSystemTime > 0)
	            {
		            try
		            {
			            rv.exposureStartTime = 
							new DateTime(status.StartExposureSystemTime).ToString("yyyy/MM/dd HH:mm:ss ffff - ") +
							new DateTime(status.EndExposureSystemTime).ToString("yyyy/MM/dd HH:mm:ss ffff | ") + 
							status.StartExposureFrameNo.ToString("0 - ") +
							status.EndExposureFrameNo.ToString("0");
		            }
		            catch { }

					try
					{
						rv.exposureDuration = new TimeSpan(status.EndExposureSystemTime - status.StartExposureSystemTime).TotalMilliseconds;
					}
					catch { }
	            }
	            else
	            {
		            rv.exposureStartTime = null;
		            rv.exposureDuration = null;
	            }

				rv.imageInfo = string.Format("INT:{0};SFID:{1};EFID:{2};CTOF:{3};UFID:{4};IFID:{5};DRPD:{6}", 
					status.DetectedIntegrationRate,
					status.StartExposureFrameNo,
					status.EndExposureFrameNo,
					status.CurrentSignatureRatio, 
					status.CameraFrameNo,
					status.IntegratedFrameNo,
					status.DropedFramesSinceIntegrationLock);

                if (status.PerformedAction > 0)
                {
                    rv.imageInfo += string.Format(";ACT:{0};ACT%:{1}", status.PerformedAction, status.PerformedActionProgress);
                }

				if (status.OcrWorking > 0)
				{
					rv.imageInfo += string.Format(";ORER:{0}", status.OcrErrorsSinceLastReset);
				}

				if (status.UserIntegratonRateHint > 0)
				{
					rv.imageInfo += string.Format(";USRI:{0}", status.UserIntegratonRateHint);
				}				
            }

            return rv;
        }

        public object ImageArray
        {
            get
            {
                return pixels;
            }
        }

        public object ImageArrayVariant
        {
            get
            {
                return pixelsVariant;
            }
        }

        public long FrameNumber
        {
            get
            {
                if (frameNumber.HasValue)
                    return frameNumber.Value;

                return -1;
            }
        }

        public double ExposureDuration
        {
            [DebuggerStepThrough]
            get
            {
				if (Settings.Default.OcrSimulatorTestMode &&
				    Settings.Default.OcrSimulatorNativeCode)
				{
					if (exposureDuration.HasValue)
						return exposureDuration.Value;
					else
						return 0;
				}
				else
					throw new PropertyNotImplementedException("Current camera doesn't support frame timing.");
            }
        }
        public string ExposureStartTime
        {
            [DebuggerStepThrough]
            get
            {
				if (Settings.Default.OcrSimulatorTestMode &&
					Settings.Default.OcrSimulatorNativeCode)
				{
					if (exposureStartTime != string.Empty)
						return exposureStartTime;
					else
						return null;
				}
				else
					throw new PropertyNotImplementedException("Current camera doesn't support frame timing.");
			}
        }

        public string ImageInfo
        {
            get { return imageInfo; }
        }

    }
}
