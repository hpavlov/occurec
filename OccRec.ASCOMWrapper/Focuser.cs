using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OccuRec.ASCOM.Interfaces;

namespace OccRec.ASCOMWrapper
{
	internal class Focuser : IASCOMFocuser
	{
		private IASCOMFocuser m_IsolatedFocuser;

		internal Focuser(IASCOMFocuser isolatedFocuser)
		{
			m_IsolatedFocuser = isolatedFocuser;
		}

		public bool Connected
		{
			get { return m_IsolatedFocuser.Connected; }
			set { m_IsolatedFocuser.Connected = value; }
		}

		public string Description
		{
			get { return m_IsolatedFocuser.Description; }
		}

		public string DriverVersion
		{
			get { return m_IsolatedFocuser.DriverVersion; }
		}
	}
}
