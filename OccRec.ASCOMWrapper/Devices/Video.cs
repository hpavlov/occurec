using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OccuRec.ASCOM.Interfaces.Devices;
using OccuRec.ASCOM.Wrapper.Interfaces;

namespace OccuRec.ASCOM.Wrapper.Devices
{
	internal class Video : DeviceBase, IVideo
	{
		private IASCOMVideo m_IsolatedVideo;

		internal Video(IASCOMVideo isolatedVideo)
			: base(isolatedVideo)
		{
			m_IsolatedVideo = isolatedVideo;
		}
	
		public VideoState GetCurrentState()
		{
			return m_IsolatedVideo.GetCurrentState();
		}
	}
}
