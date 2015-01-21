/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OccuRec.ASCOM.Interfaces.Devices
{
	[Serializable]
	public class TelescopeCapabilities
	{
		public bool CanFindHome;

		public bool CanPark;

        public bool CanPulseGuide;

        public bool CanSetGuideRates;

        public bool CanSetRightAscensionRate;

        public bool CanSetDeclinationRate;

        public bool CanSync;

	    public bool DoesRefraction;

        public double DefaultGuideRateRightAscension;

        public double DefaultGuideRateDeclination;

		public double FocalLengthMeters;

		public double ApertureMeters;
	}

    [Serializable]
    public class TelescopeGuideRate
    {
        public double GuideRateRightAscension;
        public double GuideRateDeclination; 
    }

    [Serializable]
    public class TelescopeEquatorialPosition
    {
        public double RightAscension;
        public double Declination;
    }

    [Serializable]
    public class TelescopeState
    {
        public bool AtHome;

        public bool AtPark;

        public double Altitude;

        public double Azimuth;

        public double RightAscension;

        public double Declination;

        public double GuideRateRightAscension;

        public double GuideRateDeclination;

        public double TargetRightAscension;

        public double TargetDeclination;

        public double RightAscensionRate;

        public double DeclinationRate;
    }

    [Serializable]
    public enum GuideDirections
    {
        /// <summary>
        /// North (+ declination/altitude).
        /// </summary>
        guideNorth = 0,
        
        /// <summary>
        /// South (- declination/altitude).  
        /// </summary>
 
        guideSouth = 1,
        
        /// <summary>
        /// East (+ right ascension/azimuth).
        /// </summary>
        guideEast = 2,

        /// <summary>
        /// West (- right ascension/azimuth) 
        /// </summary>
        guideWest = 3 
    }

    public interface IASCOMTelescope : IASCOMDevice
    {
        TelescopeState GetCurrentState();
        TelescopeGuideRate GetGuideRate();
        TelescopeEquatorialPosition GetEquatorialPosition();
        TelescopeCapabilities GetTelescopeCapabilities();
        void PulseGuide(GuideDirections direction, int durationMilliseconds);
        void SetGuideRateRightAscension(double newRate);
        void SetGuideRateDeclination(double newRate);
        void SlewTo(double raHours, double deDeg);
	    void SlewNearBy(double distanceArSecs, GuideDirections direction);
		void SyncToCoordinates(double raHours, double deDeg);
    }
}
