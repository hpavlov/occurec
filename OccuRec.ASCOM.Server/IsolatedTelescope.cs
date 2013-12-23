using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using OccuRec.ASCOM.Interfaces.Devices;

namespace OccuRec.ASCOM.Server
{
    [Serializable]
	public class IsolatedTelescope : IsolatedDevice, IASCOMTelescope
	{
        private global::ASCOM.DriverAccess.Telescope m_Telescope;

        internal IsolatedTelescope(string progId)
		{
            m_Telescope = new global::ASCOM.DriverAccess.Telescope(progId);
			SetIsolatedDevice(m_Telescope, progId);
		}

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
	}
}
