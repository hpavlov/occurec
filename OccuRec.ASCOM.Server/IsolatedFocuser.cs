using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using OccuRec.ASCOM.Interfaces;

namespace OccuRec.ASCOM.Server
{
	[Serializable]
	public class IsolatedFocuser : MarshalByRefObject, IASCOMFocuser, IDisposable
	{
		private global::ASCOM.DriverAccess.Focuser m_Focuser;

		internal IsolatedFocuser(string progId)
		{
			m_Focuser = new global::ASCOM.DriverAccess.Focuser(progId);
		}

		public bool Connected
		{
			get { return m_Focuser.Connected; }
			set { m_Focuser.Connected = value; }
		}

		public string Description
		{
			get { return m_Focuser.Description; }
		}

		public string DriverVersion
		{
			get { return m_Focuser.DriverVersion; }
		}

		public void Dispose()
		{
			m_Focuser.Connected = false;
			m_Focuser.Dispose();

			RemotingServices.Disconnect(this);
		}
	}
}
