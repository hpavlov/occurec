using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OccuRec.ASCOM.Interfaces.Devices
{
    public interface IASCOMDevice
    {
        bool Connected { get; set; }
        string Description { get; }
        string DriverVersion { get; }
        string ProgId { get; }
    }
}
