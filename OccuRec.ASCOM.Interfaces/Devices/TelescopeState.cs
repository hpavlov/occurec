using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OccuRec.ASCOM.Interfaces.Devices
{
    [Serializable]
    public class TelescopeState
    {
        public bool AtHome;

        public bool AtPark;

        public bool CanFindHome;

        public bool CanPark;

        public double Altitude;

        public double Azimuth;
    }
}
