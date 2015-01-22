/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
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

// LX200 Commands
//R – Slew Rate Commands

//:RG# Set Slew rate to Guiding Rate (slowest)
// Returns: Nothing

//:RC# Set Slew rate to Centering rate (2 nd slowest)
// Returns: Nothing

//:RM# Set Slew rate to Find Rate (2 nd Fastest)
// Returns: Nothing

//:RS# Set Slew rate to max (fastest)
// Returns: Nothing
 
//:RADD.D# 
// Set RA/Azimuth Slew rate to DD.D degrees per second [LX200GPS Only]
// Returns: Nothing

//:REDD.D#
// Set Dec/Elevation Slew rate to DD.D degrees per second [LX200GPS only]
// Returns: Nothing

 
//:Me#  Move Telescope East at current slew rate
// Returns: Nothing

//:Mn#  Move Telescope North at current slew rate
// Returns: Nothing

//:Ms#  Move Telescope South at current slew rate
// Returns: Nothing

//:Mw#  Move Telescope West at current slew rate
// Returns: Nothing
 
// Q – Movement Commands
//:Q# Halt all current slewing
// Returns:Nothing

//:Qe# Halt eastward Slews
// Returns:  Nothing

//:Qn# Halt northward Slews
// Returns:  Nothing

//:Qs# Halt southward Slews
// Returns:  Nothing

//:Qw# Halt westward Slews
// Returns:  Nothing 

		private void SlewEast()
		{
			SendBlindCommand("Me");
		}

		private void SlewWest()
		{
			SendBlindCommand("Mw");
		}

		private void SlewNorth()
		{
			SendBlindCommand("Mw");
		}

		private void SlewSouth()
		{
			SendBlindCommand("Mw");
		}

		private void HaltSlew()
		{
			SendBlindCommand("Q");
		}

		private void SetSlewRateToCentering()
		{
			SendBlindCommand("RC");
		}

		private void SetSlewRateInDegrees(double degrees)
		{
			degrees = Math.Abs(degrees);
			if (degrees > 1) degrees = 1; // Max Slew Rate for this driver

			string degString = degrees.ToString("00.0", CultureInfo.InvariantCulture);

			SendBlindCommand("RA" + degString);
			SendBlindCommand("RE" + degString);
		}

		private void SendBlindCommand(string command)
		{
			try
			{
				m_Telescope.CommandBlind(command, false);
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex.GetFullStackTrace());
			}
		}

		public void StopSlewing()
		{
			Trace.WriteLine(string.Format("OccuRec: ASCOMServer::{0}(Telescope)::StopSlewing()", ProgId));

			HaltSlew();
		}

		public void StartSlewing(GuideDirections direction)
		{
			Trace.WriteLine(string.Format("OccuRec: ASCOMServer::{0}(Telescope)::StartSlewing({1})", ProgId, direction));

			switch (direction)
			{
				case GuideDirections.guideEast:
					SlewEast();
					break;

				case GuideDirections.guideWest:
					SlewWest();
					break;

				case GuideDirections.guideNorth:
					SlewNorth();
					break;

				case GuideDirections.guideSouth:
					SlewSouth();
					break;
			}
			
		}

		public void StartSlewingWest()
		{
			Trace.WriteLine(string.Format("OccuRec: ASCOMServer::{0}(Telescope)::StartSlewingWest()", ProgId));

			SlewWest();
		}

		public void StartSlewingNorth()
		{
			Trace.WriteLine(string.Format("OccuRec: ASCOMServer::{0}(Telescope)::StartSlewingNorth()", ProgId));

			SlewNorth();
		}

		public void StartSlewingSouth()
		{
			Trace.WriteLine(string.Format("OccuRec: ASCOMServer::{0}(Telescope)::StartSlewingSouth()", ProgId));

			SlewSouth();
		}
		
		public void SetSlewRate(double degreesPerSecond)
		{
			Trace.WriteLine(string.Format("OccuRec: ASCOMServer::{0}(Telescope)::SetSlewRate({1})", ProgId, degreesPerSecond));

			if (double.IsNaN(degreesPerSecond))
				SetSlewRateToCentering();
			else
				SetSlewRateInDegrees(degreesPerSecond);
		}


	}
}
