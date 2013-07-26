using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AAVRec.Helpers;
using AAVRec.OCR;
using AAVRec.Properties;

namespace AAVRec
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

		    nudSignDiffFactor.Value = Math.Min(50, Math.Max(1, (decimal)Settings.Default.SignatureDiffFactorEx2));
            nudMinSignDiff.Value = Math.Min(10, Math.Max(0, (decimal)Settings.Default.MinSignatureDiff));
		    cbxGraphDebugMode.Checked = Settings.Default.VideoGraphDebugMode;
            tbxOutputLocation.Text = Settings.Default.OutputLocation;
            cbxTimeInUT.Checked = Settings.Default.DisplayTimeInUT;

		    tbxSimlatorFilePath.Text = Settings.Default.SimulatorFilePath;
            tbxOcrDebugOutputLocation.Text = Settings.Default.OcrDebugOutputFolder;

            cbxOcrCameraTestModeAvi.Checked = Settings.Default.OcrCameraTestModeAvi;
            cbxOcrCameraTestModeAav.Checked = Settings.Default.OcrCameraTestModeAav;
            nudMaxErrorsPerTestRun.Value = Settings.Default.OcrMaxErrorsPerCameraTestRun;
            cbxOcrSimlatorTestMode.Checked = Settings.Default.OcrSimulatorTestMode;
            cbxSimulatorRunOCR.Checked = Settings.Default.SimulatorRunOCR;
            tbxNTPServer.Text = Settings.Default.NTPServer;
		    rbNativeOCR.Checked = Settings.Default.OcrSimulatorNativeCode;
			nudPreserveTSTop.Value = Settings.Default.PreserveTSTopLine;
			nudPreserveTSHeight.Value = Settings.Default.PreserveTSHeight;

		    cbxImageLayoutMode.Items.Clear();
            cbxImageLayoutMode.Items.Add(AavImageLayout.CompressedRaw);
            cbxImageLayoutMode.Items.Add(AavImageLayout.CompressedDiffCodeNoSigns);
            cbxImageLayoutMode.Items.Add(AavImageLayout.CompressedDiffCodeWithSigns);
            cbxImageLayoutMode.Items.Add(AavImageLayout.UncompressedRaw);

            cbxEnableAAVIOTAVTIOCR.Checked = Settings.Default.IotaVtiOcrEnabled;

            cbxImageLayoutMode.SelectedIndex = cbxImageLayoutMode.Items.IndexOf(Settings.Default.AavImageLayout);

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
            Settings.Default.SignatureDiffFactorEx2 = (double) nudSignDiffFactor.Value;
            Settings.Default.MinSignatureDiff = (float)nudMinSignDiff.Value;

            if (!Directory.Exists(tbxOutputLocation.Text))
            {
                MessageBox.Show("Output location must be an existing directory.");
                tbxOutputLocation.Focus();
                return;
            }

            if ((cbxOcrCameraTestModeAvi.Checked || cbxOcrSimlatorTestMode.Checked || cbxOcrCameraTestModeAav.Checked) &&
                !Directory.Exists(tbxOcrDebugOutputLocation.Text))
            {
                MessageBox.Show("Debug output location must be an existing directory when using Camera or Simulator OCR testing.");
                tbxOcrDebugOutputLocation.Focus();
                return;                
            }

            Settings.Default.OutputLocation = tbxOutputLocation.Text;
            Settings.Default.DisplayTimeInUT = cbxTimeInUT.Checked;

            Settings.Default.SimulatorFilePath = tbxSimlatorFilePath.Text;
            Settings.Default.OcrDebugOutputFolder = tbxOcrDebugOutputLocation.Text;

            Settings.Default.OcrCameraTestModeAvi = cbxOcrCameraTestModeAvi.Checked;
            Settings.Default.OcrCameraTestModeAav = cbxOcrCameraTestModeAav.Checked;
            Settings.Default.OcrMaxErrorsPerCameraTestRun = (int)nudMaxErrorsPerTestRun.Value;
            Settings.Default.OcrSimulatorTestMode = cbxOcrSimlatorTestMode.Checked;
            Settings.Default.SimulatorRunOCR = cbxSimulatorRunOCR.Checked;
            Settings.Default.NTPServer = tbxNTPServer.Text;
            Settings.Default.OcrSimulatorNativeCode = rbNativeOCR.Checked;
            Settings.Default.AavImageLayout = (AavImageLayout)cbxImageLayoutMode.SelectedItem;
            Settings.Default.IotaVtiOcrEnabled = cbxEnableAAVIOTAVTIOCR.Checked;
			Settings.Default.PreserveTSTopLine = (int)nudPreserveTSTop.Value;
			Settings.Default.PreserveTSHeight = (int)nudPreserveTSHeight.Value;

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

        private void btnBrowseDebugFolder_Click(object sender, EventArgs e)
        {
            folderBrowserDialog.SelectedPath = tbxOutputLocation.Text;

            if (folderBrowserDialog.ShowDialog(this) == DialogResult.OK)
            {
                tbxOcrDebugOutputLocation.Text = folderBrowserDialog.SelectedPath;
            }
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
