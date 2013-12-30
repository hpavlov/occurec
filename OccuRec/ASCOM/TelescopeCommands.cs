using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using OccuRec.ASCOM.Wrapper.Interfaces;
using OccuRec.ASCOM.Interfaces.Devices;
using OccuRec.Helpers;
using OccuRec.Utilities;

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

        internal static void GetTelescopeCapabilities(Signal signal, ITelescope telescope)
        {
            try
            {
                Action<TelescopeCapabilities> callback = signal.Argument as Action<TelescopeCapabilities>;
                if (telescope != null && telescope.Connected)
                {
                    TelescopeCapabilities state = telescope.GetTelescopeCapabilities();

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

        internal static void PulseGuide(Signal signal, ITelescope telescope)
        {
            try
            {
                var tuple = signal.Argument as Tuple<GuideDirections, PulseRate, int, Action>;

                GuideDirections direction = tuple.Item1;
                PulseRate rate = tuple.Item2;
                int durationMilliseconds = tuple.Item3;
                Action callback = tuple.Item4;

                telescope.PulseGuide(direction, rate, durationMilliseconds);

                ASCOMHelper.SafeCallbackActionCall(callback);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.GetFullStackTrace());
            }
        }
    }
}
