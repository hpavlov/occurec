using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OccuRec.Helpers;
using OccuRec.OCR;
using OccuRec.Properties;

namespace OccuRec
{
	public partial class frmSettings : Form
	{
	    private OcrSettings m_OCRSettings;

		public frmSettings()
		{
			InitializeComponent();

		    m_OCRSettings = OcrSettings.Instance;

#if !DEBUG
		    tabControl.TabPages.Remove(tabDebug);
		    tabControl.TabPages.Remove(tabTelControl);
#endif

			nudSignDiffRatio.Value = Math.Min(50, Math.Max(1, (decimal)Settings.Default.MinSignatureDiffRatio));
            nudMinSignDiff.Value = Math.Min(10, Math.Max(0, (decimal)Settings.Default.MinSignatureDiff));
		    cbxGraphDebugMode.Checked = Settings.Default.VideoGraphDebugMode;
            tbxOutputLocation.Text = Settings.Default.OutputLocation;
            cbxTimeInUT.Checked = Settings.Default.DisplayTimeInUT;

		    tbxSimlatorFilePath.Text = Settings.Default.SimulatorFilePath;

            cbxOcrCameraTestModeAav.Checked = Settings.Default.OcrCameraAavTestMode;
            nudMaxErrorsPerTestRun.Value = Settings.Default.OcrMaxErrorsPerCameraTestRun;
            cbxOcrSimlatorTestMode.Checked = Settings.Default.OcrSimulatorTestMode;
            cbxSimulatorRunOCR.Checked = Settings.Default.SimulatorRunOCR;
            tbxNTPServer.Text = Settings.Default.NTPServer;
		    rbNativeOCR.Checked = Settings.Default.OcrSimulatorNativeCode;
			nudGammaDiff.Value = (decimal)Settings.Default.GammaDiff;

		    cbxImageLayoutMode.Items.Clear();
            cbxImageLayoutMode.Items.Add(AavImageLayout.CompressedRaw);
            cbxImageLayoutMode.Items.Add(AavImageLayout.CompressedDiffCodeNoSigns);
            cbxImageLayoutMode.Items.Add(AavImageLayout.CompressedDiffCodeWithSigns);
            cbxImageLayoutMode.Items.Add(AavImageLayout.UncompressedRaw);

            cbxImageLayoutMode.SelectedIndex = cbxImageLayoutMode.Items.IndexOf(Settings.Default.AavImageLayout);

		    cbxFrameProcessingMode.SelectedIndex = Settings.Default.UsesBufferedFrameProcessing ? 0 : 1;
            cbDebugIntegration.Checked = Settings.Default.IntegrationDetectionTuning;
			nudCalibrIntegrRate.Value = Settings.Default.CalibrationIntegrationRate;

			cbxWarnForFileSystemIssues.Checked = Settings.Default.WarnForFileSystemIssues;

		    cbForceIntegrationRateRestrictions.Checked = Settings.Default.ForceIntegrationRatesRestrictions;

            UpdateControls();
		}

        private void btnOK_Click(object sender, EventArgs e)
        {
            Settings.Default.VideoGraphDebugMode = cbxGraphDebugMode.Checked;
			Settings.Default.MinSignatureDiffRatio = (float)nudSignDiffRatio.Value;
            Settings.Default.MinSignatureDiff = (float)nudMinSignDiff.Value;
			Settings.Default.GammaDiff = (float)nudGammaDiff.Value;

            if (!Directory.Exists(tbxOutputLocation.Text))
            {
                MessageBox.Show("Output location must be an existing directory.");
                tbxOutputLocation.Focus();
                return;
            }

			if (FileNameGenerator.CheckAndWarnForFileSystemLimitation(tbxOutputLocation.Text, MessageBoxButtons.OKCancel) == DialogResult.Cancel)
			{
				tbxOutputLocation.Focus();
				return;				
			}

            Settings.Default.OutputLocation = tbxOutputLocation.Text;
            Settings.Default.DisplayTimeInUT = cbxTimeInUT.Checked;

            Settings.Default.SimulatorFilePath = tbxSimlatorFilePath.Text;

            Settings.Default.OcrCameraAavTestMode = cbxOcrCameraTestModeAav.Checked;
            Settings.Default.OcrMaxErrorsPerCameraTestRun = (int)nudMaxErrorsPerTestRun.Value;
            Settings.Default.OcrSimulatorTestMode = cbxOcrSimlatorTestMode.Checked;
            Settings.Default.SimulatorRunOCR = cbxSimulatorRunOCR.Checked;
            Settings.Default.NTPServer = tbxNTPServer.Text;
            Settings.Default.OcrSimulatorNativeCode = rbNativeOCR.Checked;
            Settings.Default.AavImageLayout = (AavImageLayout)cbxImageLayoutMode.SelectedItem;

            Settings.Default.UsesBufferedFrameProcessing = cbxFrameProcessingMode.SelectedIndex == 0;
            Settings.Default.IntegrationDetectionTuning = cbDebugIntegration.Checked;
	        Settings.Default.CalibrationIntegrationRate = (int) nudCalibrIntegrRate.Value;

            Settings.Default.WarnForFileSystemIssues = cbxWarnForFileSystemIssues.Checked;
            Settings.Default.ForceIntegrationRatesRestrictions = cbForceIntegrationRateRestrictions.Checked;

            Settings.Default.Save();

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnBrowseOutputFolder_Click(object sender, EventArgs e)
        {
            folderBrowserDialog.SelectedPath = tbxOutputLocation.Text;

            if (folderBrowserDialog.ShowDialog(this) == DialogResult.OK)
            {
                tbxOutputLocation.Text = folderBrowserDialog.SelectedPath;
            }
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
