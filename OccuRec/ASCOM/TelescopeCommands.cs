using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using OccRec.ASCOMWrapper.Interfaces;
using OccuRec.ASCOM.Interfaces.Devices;
using OccuRec.Helpers;

namespace OccuRec.ASCOM
{
    internal static class TelescopeCommands
    {
        internal static void GetTelescopeState(Signal signal, ITelescope telescope)
        {
            try
            {
                Action<TelescopeState> callback = signal.Argument as Action<TelescopeState>;
                if (telescope != null && telescope.Connected)
                {
                    TelescopeState state = telescope.GetCurrentState();

                    ASCOMHelper.SafeCallbackActionCall(callback, state);
                }
                else
                {
                    ASCOMHelper.SafeCallbackActionCall(callback, null);
                }

            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.GetFullStackTrace());
            }
        }
    }
}
