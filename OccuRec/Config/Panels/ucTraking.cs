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
using OccuRec.Helpers;
using OccuRec.Properties;
using OccuRec.Utilities;

namespace OccuRec.Config.Panels
{
	public partial class ucTraking : SettingsPanel
	{
		public ucTraking()
		{
			InitializeComponent();
		}

		public override void LoadSettings()
		{
			cbxTrackingFrequency.SetCBXIndex(Settings.Default.TrackingFrequency);
			nudMinCertGuidingStar.SetNUDValue((double)Settings.Default.TrackingMinGuidingCertainty);
			nudMinCertFixedObject.SetNUDValue((double)Settings.Default.TrackingMinForcedFixedObjCertainty);
		}

		public override void SaveSettings()
		{
			Settings.Default.TrackingFrequency = cbxTrackingFrequency.SelectedIndex;
			if (Settings.Default.TrackingFrequency == -1) Settings.Default.TrackingFrequency = 0;
			Settings.Default.TrackingMinGuidingCertainty = (float)nudMinCertGuidingStar.Value;
			Settings.Default.TrackingMinForcedFixedObjCertainty = (float)nudMinCertFixedObject.Value;
		}

	}
}
