using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AAVRec.Helpers;

namespace AAVRec.Drivers.AAVTimer.VideoCaptureImpl
{
	internal class VideoCameraFrame
	{
		public object Pixels;
		public long FrameNumber;

		public VideoFrameLayout ImageLayout;

	    public ImageStatus ImageStatus;
	}
}
