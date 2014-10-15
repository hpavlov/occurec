/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

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
        public bool IsFullDisapearance { get; set; }
	    public TrackingType TrackingType { get; set; }
		public double ApertureStartingX { get; set; }
		public double ApertureStartingY { get; set; }
		public double ApertureInPixels { get; set; }
	}
}
