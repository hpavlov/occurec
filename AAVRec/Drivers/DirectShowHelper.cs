using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AAVRec.Properties;
using DirectShowLib;

namespace AAVRec.Drivers
{
    public static class DirectShowHelper
    {
        public static void SetupTunerAndCrossbar(ICaptureGraphBuilder2 graphBuilder, IBaseFilter deviceFilter)
        {
            if (Settings.Default.UsesTunerCrossbar)
            {
                object o;

                int hr = graphBuilder.FindInterface(null, null, deviceFilter, typeof(IAMTVTuner).GUID, out o);
                if (hr >= 0)
                {
                    hr = graphBuilder.FindInterface(null, null, deviceFilter, typeof(IAMCrossbar).GUID, out o);
                    if (hr >= 0)
                    {
                        IAMCrossbar crossbar = (IAMCrossbar)o;

                        if (crossbar != null)
                        {
                            hr = crossbar.Route(Settings.Default.CrossbarOutputPin, Settings.Default.CrossbarInputPin);
                            DsError.ThrowExceptionForHR(hr);
                        }
                    }
                }
            }
        }
    }
}
