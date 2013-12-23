using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using OccuRec.ASCOM.Interfaces;
using OccuRec.ASCOM.Interfaces.Devices;

namespace OccuRec.ASCOM.Server
{
	[Serializable]
	public class IsolatedFocuser : IsolatedDevice, IASCOMFocuser
	{
		private global::ASCOM.DriverAccess.Focuser m_Focuser;

		internal IsolatedFocuser(string progId)
		{
			m_Focuser = new global::ASCOM.DriverAccess.Focuser(progId);
			SetIsolatedDevice(m_Focuser, progId);
		}
	}
}
