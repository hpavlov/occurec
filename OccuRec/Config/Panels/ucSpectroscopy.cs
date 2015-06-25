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
	public partial class ucSpectroscopy : SettingsPanel
	{
		public ucSpectroscopy()
		{
			InitializeComponent();
		}

		public override void LoadSettings()
		{
			cbxUseSpectroscopyAids.Checked = Settings.Default.SpectraUseAid;
			nudDispersion.SetNUDValue(Settings.Default.SpectraDispersion);
			nudBlurFWHM.SetNUDValue(Settings.Default.SpectraGaussFWHM);
			nudFrameStackSize.SetNUDValue(Settings.Default.SpectraFrameStack);
			nudFocusingBand.SetNUDValue(Settings.Default.SpectraFocusLine);

			if (Math.Abs(Settings.Default.SpectraFocusLine - 7605) < 10) cbxFocusingBand.SelectedIndex = 0;
			else if (Math.Abs(Settings.Default.SpectraFocusLine - 6869) < 10) cbxFocusingBand.SelectedIndex = 1;
			else if (Math.Abs(Settings.Default.SpectraFocusLine - 6563) < 10) cbxFocusingBand.SelectedIndex = 2;
			else if (Math.Abs(Settings.Default.SpectraFocusLine - 4861) < 10) cbxFocusingBand.SelectedIndex = 3;
			else if (Math.Abs(Settings.Default.SpectraFocusLine - 4340) < 10) cbxFocusingBand.SelectedIndex = 4;
			else cbxFocusingBand.SelectedIndex = 5;
		}

		public override void SaveSettings()
		{
			Settings.Default.SpectraUseAid = cbxUseSpectroscopyAids.Checked;
			Settings.Default.SpectraDispersion = (float)nudDispersion.Value;
			Settings.Default.SpectraGaussFWHM = (float)nudBlurFWHM.Value;
			Settings.Default.SpectraFrameStack = (int)nudFrameStackSize.Value;
			Settings.Default.SpectraFocusLine = (int)nudFocusingBand.Value;
		}

		private void cbxFocusingBand_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (cbxFocusingBand.SelectedIndex == 5)
			{
				nudFocusingBand.Enabled = true;
			}
			else
			{
				nudFocusingBand.Enabled = false;
				switch (cbxFocusingBand.SelectedIndex)
				{
					case 0:
						nudFocusingBand.Value = 7605;
						break;
					case 1:
						nudFocusingBand.Value = 6869;
						break;
					case 2:
						nudFocusingBand.Value = 6563;
						break;
					case 3:
						nudFocusingBand.Value = 4861;
						break;
					case 4:
						nudFocusingBand.Value = 4340;
						break;
				}
			}
		}

	}
}
