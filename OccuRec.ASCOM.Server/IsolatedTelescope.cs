using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading;
using ASCOM;
using OccuRec.ASCOM.Interfaces.Devices;
using OccuRec.Utilities;

namespace OccuRec.ASCOM.Server
{
    [Serializable]
	public class IsolatedTelescope : IsolatedDevice, IASCOMTelescope
	{
        private global::ASCOM.DriverAccess.Telescope m_Telescope;

        private TelescopeCapabilities m_TelescopeCapabilities = null;

        internal IsolatedTelescope(string progId)
		{
            m_TelescopeCapabilities = null;

            m_Telescope = new global::ASCOM.DriverAccess.Telescope(progId);
			SetIsolatedDevice(m_Telescope, progId);
		}

        protected override void OnConnected()
        {
            m_TelescopeCapabilities = GetTelescopeCapabilities();
        }

        public void Configure()
        {
            try
            {
                m_Telescope.SetupDialog();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.GetFullStackTrace());
            }
        }

        public TelescopeState GetCurrentState()
        {
            var rv = new TelescopeState();

            try
            {
                rv.AtHome = m_Telescope.AtHome;
                rv.AtPark = m_Telescope.AtPark;

                rv.Altitude = m_Telescope.Altitude;
                rv.Azimuth = m_Telescope.Azimuth;
                rv.RightAscension = m_Telescope.RightAscension;
                rv.Declination = m_Telescope.Declination;

                rv.GuideRateRightAscension = m_Telescope.GuideRateRightAscension;
                rv.GuideRateDeclination = m_Telescope.GuideRateDeclination;
                rv.RightAscensionRate = m_Telescope.RightAscensionRate;
                rv.DeclinationRate = m_Telescope.DeclinationRate;

                try
                {
                    rv.TargetRightAscension = m_Telescope.TargetRightAscension;
                }
                catch (Exception)
                {
                    rv.TargetRightAscension = double.NaN;
                }

                try
                {
                    rv.TargetDeclination = m_Telescope.TargetDeclination;
                }
                catch (Exception)
                {
                    rv.TargetDeclination = double.NaN;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.GetFullStackTrace());
            }

            return rv;
        }

        public TelescopeCapabilities GetTelescopeCapabilities()
        {
            if (m_TelescopeCapabilities == null)
                m_TelescopeCapabilities = GetTelescopeCapabilitiesInternal();

            return m_TelescopeCapabilities;
        }

        private TelescopeCapabilities GetTelescopeCapabilitiesInternal()
        {
            var rv = new TelescopeCapabilities();

            try
            {
                rv.CanFindHome = m_Telescope.CanFindHome;
                rv.CanPark = m_Telescope.CanPark;
                rv.CanPulseGuide = m_Telescope.CanPulseGuide;
                rv.CanSetDeclinationRate = m_Telescope.CanSetDeclinationRate;
                rv.CanSetRightAscensionRate = m_Telescope.CanSetRightAscensionRate;
                rv.CanSetGuideRates = m_Telescope.CanSetGuideRates;
                rv.CanSync = m_Telescope.CanSync;
                rv.DoesRefraction = m_Telescope.DoesRefraction;
                rv.DefaultGuideRateRightAscension = m_Telescope.GuideRateRightAscension;
                rv.DefaultGuideRateDeclination = m_Telescope.GuideRateDeclination;
				rv.FocalLengthMeters = m_Telescope.FocalLength;
				rv.ApertureMeters = m_Telescope.ApertureDiameter;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.GetFullStackTrace());
            }

            return rv;
        }

        public void PulseGuide(GuideDirections direction, int durationMilliseconds)
        {
            try
            {
                Trace.WriteLine(string.Format("OccuRec: ASCOMServer::{0}(Telescope)::PulseGuide({1}, {2})", ProgId, direction, durationMilliseconds));
                m_Telescope.PulseGuide((global::ASCOM.DeviceInterface.GuideDirections)direction, durationMilliseconds);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.GetFullStackTrace());
            }

            while (m_Telescope.IsPulseGuiding)
            {
                Thread.Sleep(100);
            }
        }

        public void SetGuideRateRightAscension(double newRate)
        {
            if (m_Telescope.CanSetGuideRates)
            {
                Trace.WriteLine(string.Format("OccuRec: ASCOMServer::{0}(Telescope)::GuideRateRightAscension = {1}", ProgId, newRate));
                try
                {
                    m_Telescope.GuideRateRightAscension = newRate;
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.GetFullStackTrace());
                }
            }
        }

        public void SetGuideRateDeclination(double newRate)
        {
            if (m_Telescope.CanSetGuideRates)
            {
                Trace.WriteLine(string.Format("OccuRec: ASCOMServer::{0}(Telescope)::GuideRateDeclination = {1}", ProgId, newRate));
                try
                {
                    m_Telescope.GuideRateDeclination = newRate;
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.GetFullStackTrace());
                }
            }
        }
	}
}
