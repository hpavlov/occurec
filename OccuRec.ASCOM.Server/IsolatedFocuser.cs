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

		public FocuserState GetCurrentState()
		{
			var rv = new FocuserState();

			rv.TempCompAvailable = m_Focuser.TempCompAvailable;
			rv.Temperature = m_Focuser.Temperature;
			rv.IsMoving = m_Focuser.IsMoving;
			rv.MaxIncrement = m_Focuser.MaxIncrement;
			rv.MaxStep = m_Focuser.MaxStep;
			rv.Absolute = m_Focuser.Absolute;
			rv.Link = m_Focuser.Link;
			rv.StepSize = m_Focuser.StepSize;
			rv.TempComp = m_Focuser.TempComp;
			rv.Position = m_Focuser.Position;

			return rv;
		}

		public void Move(int position)
		{
			m_Focuser.Move(position);
		}
	}
}
