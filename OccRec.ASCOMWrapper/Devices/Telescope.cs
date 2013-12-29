using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OccRec.ASCOMWrapper.Interfaces;
using OccuRec.ASCOM.Interfaces;
using OccuRec.ASCOM.Interfaces.Devices;

namespace OccRec.ASCOMWrapper.Devices
{
    internal class Telescope : DeviceBase, ITelescope
    {
        private IASCOMTelescope m_IsolatedTelescope;

        internal Telescope(IASCOMTelescope isolatedTelescope)
			: base(isolatedTelescope)
		{
            m_IsolatedTelescope = isolatedTelescope;
		}

        public TelescopeState GetCurrentState()
        {
            return m_IsolatedTelescope.GetCurrentState();
        }
    }
}
