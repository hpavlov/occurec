using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using AAVRec.Helpers;

namespace AAVRec.Drivers
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
            return InternalCreateFrame(width, height, cameraFrame, fameNumber, true);
        }

        internal static BasicVideoFrame CreateFrame(int width, int height, Bitmap cameraFrame, int fameNumber)
        {
            return InternalCreateFrame(width, height, cameraFrame, fameNumber, false);
        }

        private static BasicVideoFrame InternalCreateFrame(int width, int height, Bitmap cameraFrame, int fameNumber, bool variant)
        {
            var rv = new BasicVideoFrame();

            rv.pixels = ImageUtils.GetPixelArray(cameraFrame);

            rv.pixelsVariant = null;

            // TODO: Set these from the unmanaged OCR data, when native OCR is running

            rv.frameNumber = fameNumber;
            rv.exposureStartTime = null;
            rv.exposureDuration = null;
            rv.imageInfo = null;
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
                if (exposureDuration.HasValue)
                    return exposureDuration.Value;

                throw new PropertyNotImplementedException("Current camera doesn't support frame timing.");
            }
        }
        public string ExposureStartTime
        {
            [DebuggerStepThrough]
            get
            {
                if (exposureStartTime != null)
                    return exposureStartTime;

                throw new PropertyNotImplementedException("Current camera doesn't support frame timing.");
            }
        }

        public string ImageInfo
        {
            get { return imageInfo; }
        }

    }
}
