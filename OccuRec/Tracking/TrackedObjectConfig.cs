using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OccuRec.Tracking
{
	public enum TrackingType
	{
		GuidingStar,
		ComparisonStar,
		OccultedStar
	}

	internal class TrackedObjectConfig
	{
		public bool IsFixedAperture { get; set; }
		public TrackingType TrackingType { get; set; }
		public double ApertureStartingX { get; set; }
		public double ApertureStartingY { get; set; }
		public double ApertureInPixels { get; set; }
	}
}
