/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OccuRec.Properties;
using OccuRec.Utilities;

namespace OccuRec.Config.Panels
{
	public partial class ucLightCurve : SettingsPanel
	{
		public ucLightCurve()
		{
			InitializeComponent();
		}

		public override void LoadSettings()
		{
			nudApertureInFWHM.SetNUDValue((double)Settings.Default.TrackingApertureInFWHM);
			nudInnerAnulusInApertures.SetNUDValue((double)Settings.Default.TrackingInnerRadiusOfAnnulus);
			nudMinimumAnulusPixels.SetNUDValue((decimal)Settings.Default.TrackingMinNumberPixelsInAnnulus);

			cbxDisplayTargetLightCurve.Checked = Settings.Default.OverlayDrawTargetLightCurve;
			cbxDisplayTargetPSF.Checked = Settings.Default.OverlayDrawTargetStarFSP;
			cbxDisplayGuidingPSF.Checked = Settings.Default.OverlayDrawGuidingStarFSP;
		}

		public override void SaveSettings()
		{
			if (Settings.Default.TrackingFrequency == -1) Settings.Default.TrackingFrequency = 0;
			Settings.Default.TrackingApertureInFWHM = (float)nudApertureInFWHM.Value;
			Settings.Default.TrackingInnerRadiusOfAnnulus = (float)nudInnerAnulusInApertures.Value;
			Settings.Default.TrackingMinNumberPixelsInAnnulus = (int)nudMinimumAnulusPixels.Value;

			Settings.Default.OverlayDrawTargetLightCurve = cbxDisplayTargetLightCurve.Checked;
			Settings.Default.OverlayDrawTargetStarFSP = cbxDisplayTargetPSF.Checked;
			Settings.Default.OverlayDrawGuidingStarFSP = cbxDisplayGuidingPSF.Checked;
		}
	}
}
