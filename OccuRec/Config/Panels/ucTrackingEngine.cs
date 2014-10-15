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
	public partial class ucTrackingEngine : SettingsPanel
	{
		public ucTrackingEngine()
		{
			InitializeComponent();
		}

		public override void LoadSettings()
		{
			nudMaxElongation.SetNUDValue((double)Settings.Default.TrackingMaxElongation);
			cbxTestPSFElongation.Checked = Settings.Default.TrackingCheckElongation;
			nudMaxElongation.Enabled = cbxTestPSFElongation.Checked;

			nudMinFWHM.SetNUDValue((double)Settings.Default.TrackingMinFWHM);
			nudMaxFWHM.SetNUDValue((double)Settings.Default.TrackingMaxFWHM);
			nudDetectionCertainty.SetNUDValue((double)Settings.Default.TrackingMinCertainty);
			nudGuidingStarDetectionCertainty.SetNUDValue((double)Settings.Default.TrackingMinGuidingStarCertainty);
		}

		public override void SaveSettings()
		{
			Settings.Default.TrackingMaxElongation = (double)nudMaxElongation.Value;
			Settings.Default.TrackingCheckElongation = cbxTestPSFElongation.Checked;

			Settings.Default.TrackingMinFWHM = (double)nudMinFWHM.Value;
			Settings.Default.TrackingMaxFWHM = (double)nudMaxFWHM.Value;
			Settings.Default.TrackingMinCertainty = (double)nudDetectionCertainty.Value;
			Settings.Default.TrackingMinGuidingStarCertainty = (double)nudGuidingStarDetectionCertainty.Value;
		}

		private void cbxTestPSFElongation_CheckedChanged(object sender, EventArgs e)
		{
			nudMaxElongation.Enabled = cbxTestPSFElongation.Checked;
		}
	}
}
