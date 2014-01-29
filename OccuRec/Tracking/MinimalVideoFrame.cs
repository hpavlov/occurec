using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OccuRec.Drivers;

namespace OccuRec.Tracking
{
	internal class MinimalVideoFrame : IVideoFrame
	{
		public MinimalVideoFrame(IVideoFrame source)
		{
			FrameNumber = source.FrameNumber;
			ExposureDuration = double.NaN;
			ExposureStartTime = null;
			ImageInfo = source.ImageInfo;

			if (source.ImageArray is int[,])
			{
				ImageArray = source.ImageArray;
			}
			else if (source.ImageArray != null)
				throw new NotSupportedException("Unsupported ImageArray Format.");
		}

		public object ImageArray { get; private set; }

		public object ImageArrayVariant
		{
			get { return null; }
		}

		public System.Drawing.Bitmap PreviewBitmap
		{
			get { return null; }
		}

		public long FrameNumber { get; private set; }

		public double ExposureDuration { get; private set; }

		public string ExposureStartTime { get; private set; }

		public string ImageInfo { get; private set; }
	}
}
