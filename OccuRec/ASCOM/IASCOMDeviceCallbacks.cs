using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OccuRec.ASCOM.Interfaces.Devices;

namespace OccuRec.ASCOM
{
    public enum ASCOMConnectionState
    {
        Connecting,
        Connected,
        Disconnecting,
        Disconnected,
        NotResponding,
        Errored
    }

    public interface IASCOMDeviceCallbacks
    {
        void TelescopeConnectionChanged(ASCOMConnectionState state);
        void FocuserConnectionChanged(ASCOMConnectionState state);
        void TelescopeStateUpdate(TelescopeState state);
    }
}
