using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using OccuRec.ASCOM.Interfaces.Devices;

namespace OccuRec.ASCOM.Server
{
    [Serializable]
    public class IsolatedTelescope : MarshalByRefObject, IASCOMTelescope, IDisposable
	{
        private global::ASCOM.DriverAccess.Telescope m_Telescope;

        internal IsolatedTelescope(string progId)
		{
            m_Telescope = new global::ASCOM.DriverAccess.Telescope(progId);
            ProgId = progId;
		}

		public bool Connected
		{
            get { return m_Telescope.Connected; }
            set { m_Telescope.Connected = value; }
		}

		public string Description
		{
            get { return m_Telescope.Description; }
		}

		public string DriverVersion
		{
            get { return m_Telescope.DriverVersion; }
		}

        public string ProgId { get; private set; }

        public TelescopeState GetCurrentState()
        {
            var rv = new TelescopeState();

            rv.AtHome = m_Telescope.AtHome;
            rv.AtPark = m_Telescope.AtPark;
            rv.CanFindHome = m_Telescope.CanFindHome;
            rv.CanPark = m_Telescope.CanPark;
            rv.Altitude = m_Telescope.Altitude;
            rv.Azimuth = m_Telescope.Azimuth;

            return rv;
        }
        public void Dispose()
		{
            m_Telescope.Connected = false;
            m_Telescope.Dispose();

			RemotingServices.Disconnect(this);
		}
	}
}
