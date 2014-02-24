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
        TelescopeCapabilities GetTelescopeCapabilities();
        void PulseGuide(GuideDirections direction, int durationMilliseconds);
        void SetGuideRateRightAscension(double newRate);
        void SetGuideRateDeclination(double newRate);
    }
}
