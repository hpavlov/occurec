/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

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

        public TelescopeGuideRate GetGuideRate()
        {
            var rv = new TelescopeGuideRate();
            var sw = new Stopwatch();

            try
            {
                sw.Start();

                rv.GuideRateRightAscension = m_Telescope.GuideRateRightAscension;
                rv.GuideRateDeclination = m_Telescope.GuideRateDeclination;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.GetFullStackTrace());
            }
            finally
            {
                sw.Stop();
                Trace.WriteLine(string.Format("Telescope.GetGuideRate() took {0} ms", (int)sw.ElapsedMilliseconds));
            }

            return rv;            
        }

        public TelescopeEquatorialPosition GetEquatorialPosition()
        {
            var rv = new TelescopeEquatorialPosition();
            var sw = new Stopwatch();

            try
            {
                sw.Start();

                rv.RightAscension = m_Telescope.RightAscension;
                rv.Declination = m_Telescope.Declination;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.GetFullStackTrace());
            }
            finally
            {
                sw.Stop();
                Trace.WriteLine(string.Format("Telescope.GetEquatorialPosition() took {0} ms", (int)sw.ElapsedMilliseconds));
            }

            return rv;            
        }


        public TelescopeState GetCurrentState()
        {
            var rv = new TelescopeState();
            var sw = new Stopwatch();

            try
            {
                sw.Start();

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
            finally
            {
                sw.Stop();
                Trace.WriteLine(string.Format("Telescope.GetCurrentState() took {0} ms", (int)sw.ElapsedMilliseconds));
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
            var sw = new Stopwatch();

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
            finally
            {
                sw.Stop();
                Trace.WriteLine(string.Format("Telescope.GetTelescopeCapabilitiesInternal() took {0} ms", (int)sw.ElapsedMilliseconds));
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

        public void SlewTo(double raHours, double deDeg)
        {
            Trace.WriteLine(string.Format("OccuRec: ASCOMServer::{0}(Telescope)::SlewTo({1}, {2})", ProgId, raHours, deDeg));
            try
            {
                m_Telescope.SlewToCoordinates(raHours, deDeg); 
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.GetFullStackTrace());
            }
        }

		public void SyncToCoordinates(double raHours, double deDeg)
		{
			Trace.WriteLine(string.Format("OccuRec: ASCOMServer::{0}(Telescope)::SyncToCoordinates({1}, {2})", ProgId, raHours, deDeg));
			try
			{
				m_Telescope.SyncToCoordinates(raHours, deDeg);
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex.GetFullStackTrace());
			}
		}


		public void SlewNearBy(double distanceArSecs, GuideDirections direction)
		{
			Trace.WriteLine(string.Format("OccuRec: ASCOMServer::{0}(Telescope)::SlewNearBy({1}, {2})", ProgId, distanceArSecs, direction));
			try
			{
				double rightAscension = m_Telescope.RightAscension;
				double declination = m_Telescope.Declination;
				switch (direction)
				{
					case GuideDirections.guideEast:
						rightAscension -= (distanceArSecs / 54000.0);
						if (rightAscension < 0) rightAscension += 24;
						break;

					case GuideDirections.guideWest:
						rightAscension += (distanceArSecs / 54000.0);
						if (rightAscension > 24) rightAscension -= 24;
						break;

					case GuideDirections.guideNorth:
						declination += (distanceArSecs / 3600.0);
						if (declination > 90) declination = 90;
						break;

					case GuideDirections.guideSouth:
						declination -= (distanceArSecs / 3600.0);
						if (declination < -90) declination = -90;
						break;
				}

				m_Telescope.SlewToCoordinates(rightAscension, declination);
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex.GetFullStackTrace());
			}
		}
	}
}
