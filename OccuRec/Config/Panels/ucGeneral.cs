/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OccuRec.Helpers;
using OccuRec.Properties;
using OccuRec.Utilities;

namespace OccuRec.Config.Panels
{
	public partial class ucGeneral : SettingsPanel
	{
		public ucGeneral()
		{
			InitializeComponent();
		}

		public override void LoadSettings()
		{
			tbxOutputLocation.Text = Settings.Default.OutputLocation;
			cbxTimeInUT.Checked = Settings.Default.DisplayTimeInUT;
			cbxWarnForFileSystemIssues.Checked = Settings.Default.WarnForFileSystemIssues;
			cbxWarnFreeSpace.Checked = Settings.Default.WarnOnFreeDiskSpaceLeft;
			cbxWarnFAT16.Checked = Settings.Default.WarnOnFAT16Usage;
			nudWarnGBFreeLeft.SetNUDValue(Settings.Default.WarnMinDiskFreeSpaceGb);
		}

		public override void SaveSettings()
		{
			Settings.Default.OutputLocation = tbxOutputLocation.Text;
			Settings.Default.DisplayTimeInUT = cbxTimeInUT.Checked;
			Settings.Default.WarnForFileSystemIssues = cbxWarnForFileSystemIssues.Checked;
			Settings.Default.WarnOnFreeDiskSpaceLeft = cbxWarnFreeSpace.Checked; 
			Settings.Default.WarnOnFAT16Usage = cbxWarnFAT16.Checked;
			Settings.Default.WarnMinDiskFreeSpaceGb = (float) nudWarnGBFreeLeft.Value;
		}

		public override bool ValidateSettings()
		{
			if (!Directory.Exists(tbxOutputLocation.Text))
			{
				MessageBox.Show("Output location must be an existing directory.");
				tbxOutputLocation.Focus();
				return false;
			}

			if (FileNameGenerator.CheckAndWarnForFileSystemLimitation(tbxOutputLocation.Text, MessageBoxButtons.OKCancel) == DialogResult.Cancel)
			{
				tbxOutputLocation.Focus();
				return false;
			}

			return true;
		}

		private void btnBrowseOutputFolder_Click(object sender, EventArgs e)
		{
			folderBrowserDialog.SelectedPath = tbxOutputLocation.Text;

			if (folderBrowserDialog.ShowDialog(this) == DialogResult.OK)
			{
				tbxOutputLocation.Text = folderBrowserDialog.SelectedPath;
			}
		}

		private void cbxWarnForFileSystemIssues_CheckedChanged(object sender, EventArgs e)
		{
			gbxFSWarnings.Enabled = cbxWarnForFileSystemIssues.Checked;
		}
	}
}
