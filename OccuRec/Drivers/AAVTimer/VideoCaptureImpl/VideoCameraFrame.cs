using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using OccuRec.Helpers;

namespace OccuRec.Drivers.AAVTimer.VideoCaptureImpl
{
	internal class VideoCameraFrame
	{
		public object Pixels;
	    public Bitmap PreviewBitmap;
		public long FrameNumber;

        public long UniqueFrameNumber;

		public VideoFrameLayout ImageLayout;

		public ImageStatus ImageStatus;
	}
}
