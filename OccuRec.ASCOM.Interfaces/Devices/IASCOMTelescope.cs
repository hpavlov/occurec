using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OccuRec.ASCOM.Interfaces.Devices
{
    public interface IASCOMTelescope : IASCOMDevice
    {
        TelescopeState GetCurrentState();
    }
}
