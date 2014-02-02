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
		    cbxDisplayTargetLightCurve.Checked = Settings.Default.OverlayDrawTargetLightCurve;
            cbxDisplayTargetPSF.Checked = Settings.Default.OverlayDrawTargetStarFSP;
            cbxDisplayGuidingPSF.Checked = Settings.Default.OverlayDrawGuidingStarFSP;
		}

		public override void SaveSettings()
		{
			Settings.Default.OutputLocation = tbxOutputLocation.Text;
			Settings.Default.DisplayTimeInUT = cbxTimeInUT.Checked;
			Settings.Default.WarnForFileSystemIssues = cbxWarnForFileSystemIssues.Checked;
            Settings.Default.OverlayDrawTargetLightCurve = cbxDisplayTargetLightCurve.Checked;
            Settings.Default.OverlayDrawTargetStarFSP = cbxDisplayTargetPSF.Checked;
            Settings.Default.OverlayDrawGuidingStarFSP = cbxDisplayGuidingPSF.Checked;
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
	}
}
