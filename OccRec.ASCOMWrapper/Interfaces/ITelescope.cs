using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OccuRec.ASCOM.Interfaces.Devices;

namespace OccuRec.ASCOM.Wrapper.Interfaces
{
    public enum PulseRate
    {
        Slowest,
        Slow,
        Fast
    }

    public interface ITelescope : IASCOMTelescope
    {
        void PulseGuide(GuideDirections direction, PulseRate rate, int durationMilliseconds);
    }
}
