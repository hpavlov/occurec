using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using OccuRec.Drivers.DirectShowCapture.VideoCaptureImpl;

namespace OccuRec.Drivers.DirectShowCapture
{
	public enum VideoFrameLayout
	{
		Monochrome,
		Color,
		BayerRGGB
	}

	public enum MonochromePixelMode
	{
		R,
		G,
		B,
		GrayScale
	}

	public class VideoFrame : IVideoFrame
	{
		private long? frameNumber;
		private string imageInfo;
		private double? exposureDuration;
		private string exposureStartTime;
		private object pixels;
		private object pixelsVariant;

		private static int s_Counter = 0;

		internal static VideoFrame FakeFrame(int width, int height)
		{
			var rv = new VideoFrame();
			s_Counter++;
			rv.frameNumber = s_Counter;

			rv.pixels = new int[0, 0];
			return rv;
		}

		internal static VideoFrame CreateFrameVariant(int width, int height, VideoCameraFrame cameraFrame)
		{
			return InternalCreateFrame(width, height, cameraFrame, true);
		}

		internal static VideoFrame CreateFrame(int width, int height, VideoCameraFrame cameraFrame)
		{
			return InternalCreateFrame(width, height, cameraFrame, false);
		}

		private static VideoFrame InternalCreateFrame(int width, int height, VideoCameraFrame cameraFrame, bool variant)
		{
			var rv = new VideoFrame();

			if (cameraFrame.ImageLayout == VideoFrameLayout.Monochrome)
			{
				if (variant)
				{
					rv.pixelsVariant = new object[height, width];
					rv.pixels = null;
				}
				else
				{
					rv.pixels = new int[height, width];
					rv.pixelsVariant = null;
				}

				if (variant)
					Array.Copy((int[,])cameraFrame.Pixels, (object[,])rv.pixelsVariant, ((int[,])cameraFrame.Pixels).Length);
				else
					rv.pixels = (int[,])cameraFrame.Pixels;
			}
			else if (cameraFrame.ImageLayout == VideoFrameLayout.Color)
			{
				if (variant)
				{
					rv.pixelsVariant = new object[height, width, 3];
					rv.pixels = null;
				}
				else
				{
					rv.pixels = new int[height, width, 3];
					rv.pixelsVariant = null;
				}

				if (variant)
					Array.Copy((int[, ,])cameraFrame.Pixels, (object[, ,])rv.pixelsVariant, ((int[, ,])cameraFrame.Pixels).Length);
				else
					rv.pixels = (int[, ,])cameraFrame.Pixels;
			}
			else if (cameraFrame.ImageLayout == VideoFrameLayout.BayerRGGB)
			{
				throw new NotSupportedException();
			}
			else
				throw new NotSupportedException();

			rv.frameNumber = cameraFrame.FrameNumber;
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

