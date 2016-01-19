using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASCOM.DeviceInterface;

namespace ASCOM.GenericCCDCamera
{
	internal class CCDVideoFrame : IVideoFrame
	{
		public double ExposureDuration { get; internal set; }

		public string ExposureStartTime { get; internal set; }

		public long FrameNumber { get; internal set; }

		public object ImageArray { get; internal set; }

		public System.Collections.ArrayList ImageMetadata { get; internal set; }

		public byte[] PreviewBitmap { get; internal set; }
	}
}
