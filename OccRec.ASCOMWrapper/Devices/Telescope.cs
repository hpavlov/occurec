using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using OccuRec.ASCOM.Wrapper.Interfaces;
using OccuRec.ASCOM.Interfaces;
using OccuRec.ASCOM.Interfaces.Devices;
using OccuRec.Utilities;

namespace OccuRec.ASCOM.Wrapper.Devices
{
    internal class Telescope : DeviceBase, ITelescope
    {
        private IASCOMTelescope m_IsolatedTelescope;
        private bool m_CapabilitiesKnown;
        private TelescopeCapabilities m_TelescopeCapabilities;
        private double m_DefaultGuideRateRightAscension = double.NaN;
        private double m_DefaultGuideRateDeclination = double.NaN;

        private float m_PulseSlowestRate;
        private float m_PulseSlowRate;
        private float m_PulseFastRate;

        internal Telescope(IASCOMTelescope isolatedTelescope, float slowestRate, float slowRate, float fastRate)
			: base(isolatedTelescope)
		{
            m_IsolatedTelescope = isolatedTelescope;
            m_CapabilitiesKnown = false;

            m_PulseSlowestRate = slowestRate;
            m_PulseSlowRate = slowRate;
            m_PulseFastRate = fastRate;

            if (float.IsNaN(m_PulseSlowestRate)) m_PulseSlowestRate = 1.0f;
            if (float.IsNaN(m_PulseSlowRate)) m_PulseSlowestRate = 10.0f;
            if (float.IsNaN(m_PulseFastRate)) m_PulseSlowestRate = 100.0f;
		}

        protected override void OnConnected()
        {
            GetTelescopeCapabilities(true);
        }

        private void GetTelescopeCapabilities(bool force)
        {
            if (force || !m_CapabilitiesKnown)
            {
                m_TelescopeCapabilities = m_IsolatedTelescope.GetTelescopeCapabilities();
                if (m_TelescopeCapabilities != null)
                {
                    Trace.WriteLine(m_TelescopeCapabilities.AsSerialized().OuterXml);
                    m_DefaultGuideRateRightAscension = m_TelescopeCapabilities.DefaultGuideRateRightAscension;
                    m_DefaultGuideRateDeclination = m_TelescopeCapabilities.DefaultGuideRateDeclination;
                    m_CapabilitiesKnown = true;
                }
            }
        }

        private void EnsureRequestedGuideRate(PulseRate rate)
        {
            Trace.WriteLine(string.Format("EnsureRequestedGuideRate({0})", rate));
            Trace.WriteLine(string.Format("m_PulseSlowestRate:{0}, m_PulseSlowRate:{1}, m_PulseFastRate:{2}", m_PulseSlowestRate, m_PulseSlowRate, m_PulseFastRate));
            Trace.WriteLine(string.Format("m_DefaultGuideRateDeclination:{0}, m_DefaultGuideRateRightAscension:{1}", m_DefaultGuideRateDeclination, m_DefaultGuideRateRightAscension));

            TelescopeState state = GetCurrentState();

            double declFactor = state.GuideRateDeclination / m_DefaultGuideRateDeclination;
            double requestedFactor = 0;
            if (rate == PulseRate.Slowest)
                requestedFactor = m_PulseSlowestRate;
            else if (rate == PulseRate.Slow)
                requestedFactor = m_PulseSlowRate;
            else if (rate == PulseRate.Fast)
                requestedFactor = m_PulseFastRate;

            Trace.WriteLine(string.Format("declFactor:{0}; requestedFactor:{1}", declFactor, requestedFactor));

            if (Math.Abs(declFactor - requestedFactor) > 0.5)
            {
                double newRateDEC = requestedFactor * m_DefaultGuideRateDeclination;
                SetGuideRateDeclination(newRateDEC);

                double newRateRA = requestedFactor * m_DefaultGuideRateRightAscension;
                SetGuideRateRightAscension(newRateRA);
            }
        }

        public void PulseGuide(GuideDirections direction, PulseRate rate, int durationMilliseconds)
        {
            EnsureRequestedGuideRate(rate);
            PulseGuide(direction, durationMilliseconds);
        }

        public TelescopeState GetCurrentState()
        {
            GetTelescopeCapabilities(false);

            return m_IsolatedTelescope.GetCurrentState();
        }

        public TelescopeCapabilities GetTelescopeCapabilities()
        {
            return m_IsolatedTelescope.GetTelescopeCapabilities();
        }

        public void PulseGuide(GuideDirections direction, int durationMilliseconds)
        {
            m_IsolatedTelescope.PulseGuide(direction, durationMilliseconds);
        }

        public void SetGuideRateRightAscension(double newRate)
        {
            m_IsolatedTelescope.SetGuideRateRightAscension(newRate);
        }

        public void SetGuideRateDeclination(double newRate)
        {
            m_IsolatedTelescope.SetGuideRateDeclination(newRate);
        }
    }
}
