using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OccuRec.ASCOM.Interfaces;
using OccuRec.ASCOM.Interfaces.Devices;

namespace OccRec.ASCOMWrapper.Devices
{
    internal class Telescope : IASCOMTelescope
    {
        private IASCOMTelescope m_IsolatedTelescope;

        internal Telescope(IASCOMTelescope isolatedTelescope)
		{
            m_IsolatedTelescope = isolatedTelescope;
		}

		public bool Connected
		{
			get { return m_IsolatedTelescope.Connected; }
			set { m_IsolatedTelescope.Connected = value; }
		}

		public string Description
		{
			get { return m_IsolatedTelescope.Description; }
		}

		public string DriverVersion
		{
			get { return m_IsolatedTelescope.DriverVersion; }
		}

        public string ProgId
        {
            get { return m_IsolatedTelescope.ProgId; }
        }

        public TelescopeState GetCurrentState()
        {
            return m_IsolatedTelescope.GetCurrentState();
        }
    }
}
