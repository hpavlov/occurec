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

namespace OccuRec.Config.Panels
{
	public partial class ucDebug : SettingsPanel
	{
		public ucDebug()
		{
			InitializeComponent();
		}

		public override void LoadSettings()
		{			
			tbxSimlatorFilePath.Text = Settings.Default.SimulatorFilePath;

			cbxOcrCameraTestModeAav.Checked = Settings.Default.OcrCameraAavTestMode;
			nudMaxErrorsPerTestRun.Value = Settings.Default.OcrMaxErrorsPerCameraTestRun;
			cbxOcrSimlatorTestMode.Checked = Settings.Default.OcrSimulatorTestMode;
			cbxSimulatorRunOCR.Checked = Settings.Default.SimulatorRunOCR;
			rbNativeOCR.Checked = Settings.Default.OcrSimulatorNativeCode;
			cbxSimulateFailedVtiOsdDetection.Checked = Settings.Default.SimulateFailedVtiOsdDetection;

			UpdateControls();
		}

		public override void SaveSettings()
		{
			Settings.Default.SimulatorFilePath = tbxSimlatorFilePath.Text;

			Settings.Default.OcrCameraAavTestMode = cbxOcrCameraTestModeAav.Checked;
			Settings.Default.OcrMaxErrorsPerCameraTestRun = (int)nudMaxErrorsPerTestRun.Value;
			Settings.Default.OcrSimulatorTestMode = cbxOcrSimlatorTestMode.Checked;
			Settings.Default.SimulatorRunOCR = cbxSimulatorRunOCR.Checked;

			Settings.Default.OcrSimulatorNativeCode = rbNativeOCR.Checked;
			Settings.Default.SimulateFailedVtiOsdDetection = cbxSimulateFailedVtiOsdDetection.Checked; 
		}

		private void btnBrowseSimulatorFile_Click(object sender, EventArgs e)
		{
			if (openAavFileDialog.ShowDialog(this) == DialogResult.OK)
				tbxSimlatorFilePath.Text = openAavFileDialog.FileName;
		}

		private void cbxSimulatorRunOCR_CheckedChanged(object sender, EventArgs e)
		{
			UpdateControls();
		}

		private void UpdateControls()
		{
			rbManagedSim.Enabled = cbxSimulatorRunOCR.Checked;
			rbNativeOCR.Enabled = cbxSimulatorRunOCR.Checked;
		}
	}
}
