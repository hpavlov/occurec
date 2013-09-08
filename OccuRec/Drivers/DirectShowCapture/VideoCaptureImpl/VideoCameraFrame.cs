using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace OccuRec.Drivers.DirectShowCapture.VideoCaptureImpl
{
	internal class VideoCameraFrame
	{
		public object Pixels;
	    public Bitmap PreviewBitmap;
		public long FrameNumber;

		public VideoFrameLayout ImageLayout;
	}
}
