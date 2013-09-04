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
#endif

			nudSignDiffRatio.Value = Math.Min(50, Math.Max(1, (decimal)Settings.Default.MinSignatureDiffRatio));
            nudMinSignDiff.Value = Math.Min(10, Math.Max(0, (decimal)Settings.Default.MinSignatureDiff));
		    cbxGraphDebugMode.Checked = Settings.Default.VideoGraphDebugMode;
            tbxOutputLocation.Text = Settings.Default.OutputLocation;
            cbxTimeInUT.Checked = Settings.Default.DisplayTimeInUT;

		    tbxSimlatorFilePath.Text = Settings.Default.SimulatorFilePath;

            cbxOcrCameraTestModeAav.Checked = Settings.Default.OcrCameraTestModeAav;
            nudMaxErrorsPerTestRun.Value = Settings.Default.OcrMaxErrorsPerCameraTestRun;
            cbxOcrSimlatorTestMode.Checked = Settings.Default.OcrSimulatorTestMode;
            cbxSimulatorRunOCR.Checked = Settings.Default.SimulatorRunOCR;
            tbxNTPServer.Text = Settings.Default.NTPServer;
		    rbNativeOCR.Checked = Settings.Default.OcrSimulatorNativeCode;
			nudPreserveTSTop.Value = Settings.Default.PreserveTSLineTop;
			nudPreserveTSHeight.Value = Settings.Default.PreserveTSAreaHeight;
			nudGammaDiff.Value = (decimal)Settings.Default.GammaDiff;

		    cbxImageLayoutMode.Items.Clear();
            cbxImageLayoutMode.Items.Add(AavImageLayout.CompressedRaw);
            cbxImageLayoutMode.Items.Add(AavImageLayout.CompressedDiffCodeNoSigns);
            cbxImageLayoutMode.Items.Add(AavImageLayout.CompressedDiffCodeWithSigns);
            cbxImageLayoutMode.Items.Add(AavImageLayout.UncompressedRaw);

            cbxEnableAAVIOTAVTIOCR.Checked = Settings.Default.AavOcrEnabled;

            cbxImageLayoutMode.SelectedIndex = cbxImageLayoutMode.Items.IndexOf(Settings.Default.AavImageLayout);

		    cbxFrameProcessingMode.SelectedIndex = Settings.Default.UsesBufferedFrameProcessing ? 0 : 1;
            cbDebugIntegration.Checked = Settings.Default.IntegrationDetectionTuning;
			nudCalibrIntegrRate.Value = Settings.Default.CalibrationIntegrationRate;

			cbxWarnForFileSystemIssues.Checked = Settings.Default.WarnForFileSystemIssues;

			OcrSettings.Instance.Configurations
				.Where(x => !x.Hidden)
				.ToList()
				.ForEach(x => cbxOCRConfigurations.Items.Add(x.Name));

            if (!string.IsNullOrEmpty(Settings.Default.SelectedOcrConfiguration))
            {
                int selectedIndex = cbxOCRConfigurations.Items.IndexOf(Settings.Default.SelectedOcrConfiguration);
                cbxOCRConfigurations.SelectedIndex = selectedIndex;
            }

            UpdateControls();
		}

        private void frmSettings_Load(object sender, EventArgs e)
        {
            //lblArea1Config.Text = string.Format("T:{0}; L:{1}; W:{2}; H:{3}", m_OCRSettings.TimeStampArea1.Top, m_OCRSettings.TimeStampArea1.Left, m_OCRSettings.TimeStampArea1.Width, m_OCRSettings.TimeStampArea1.Height);
            //lblArea2Config.Text = string.Format("T:{0}; L:{1}; W:{2}; H:{3}", m_OCRSettings.TimeStampArea2.Top, m_OCRSettings.TimeStampArea2.Left, m_OCRSettings.TimeStampArea2.Width, m_OCRSettings.TimeStampArea2.Height);
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

            Settings.Default.OcrCameraTestModeAav = cbxOcrCameraTestModeAav.Checked;
            Settings.Default.OcrMaxErrorsPerCameraTestRun = (int)nudMaxErrorsPerTestRun.Value;
            Settings.Default.OcrSimulatorTestMode = cbxOcrSimlatorTestMode.Checked;
            Settings.Default.SimulatorRunOCR = cbxSimulatorRunOCR.Checked;
            Settings.Default.NTPServer = tbxNTPServer.Text;
            Settings.Default.OcrSimulatorNativeCode = rbNativeOCR.Checked;
            Settings.Default.AavImageLayout = (AavImageLayout)cbxImageLayoutMode.SelectedItem;
            Settings.Default.AavOcrEnabled = cbxEnableAAVIOTAVTIOCR.Checked;
			Settings.Default.PreserveTSLineTop = (int)nudPreserveTSTop.Value;
			Settings.Default.PreserveTSAreaHeight = (int)nudPreserveTSHeight.Value;			

            Settings.Default.UsesBufferedFrameProcessing = cbxFrameProcessingMode.SelectedIndex == 0;
            Settings.Default.IntegrationDetectionTuning = cbDebugIntegration.Checked;
	        Settings.Default.CalibrationIntegrationRate = (int) nudCalibrIntegrRate.Value;

			Settings.Default.SelectedOcrConfiguration = (string)cbxOCRConfigurations.SelectedItem;
			Settings.Default.WarnForFileSystemIssues = cbxWarnForFileSystemIssues.Checked;

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

		private void cbxEnableAAVIOTAVTIOCR_CheckedChanged(object sender, EventArgs e)
		{
			pnlSelectOCRConfig.Enabled = cbxEnableAAVIOTAVTIOCR.Checked;
		}
	}
}
