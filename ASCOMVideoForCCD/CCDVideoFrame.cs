using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASCOM.DeviceInterface;

namespace ASCOM.GenericCCDCamera
{
	internal class CCDVideoFrame : IVideoFrame
	{
		public double ExposureDuration
		{
			get { throw new System.NotImplementedException(); }
		}

		public string ExposureStartTime
		{
			get { throw new System.NotImplementedException(); }
		}

		public long FrameNumber
		{
			get { throw new System.NotImplementedException(); }
		}

		public object ImageArray
		{
			get { throw new System.NotImplementedException(); }
		}

		public System.Collections.ArrayList ImageMetadata
		{
			get { throw new System.NotImplementedException(); }
		}

		public byte[] PreviewBitmap
		{
			get { throw new System.NotImplementedException(); }
		}
	}
}
