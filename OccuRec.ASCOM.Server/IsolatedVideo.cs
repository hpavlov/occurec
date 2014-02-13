using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OccuRec.ASCOM.Interfaces.Devices;

namespace OccuRec.ASCOM.Server
{
	[Serializable]
	public class IsolatedVideo : IsolatedDevice, IASCOMVideo
	{
		private global::ASCOM.DriverAccess.Video m_Video;

		internal IsolatedVideo(string progId)
		{
            m_Video = new global::ASCOM.DriverAccess.Video(progId);

			SetIsolatedDevice(m_Video, progId);
		}

		public VideoState GetCurrentState()
		{
			var state = new VideoState();

			// TODO: Populate the state from the m_Video object

			return state;
		}
	}
}
