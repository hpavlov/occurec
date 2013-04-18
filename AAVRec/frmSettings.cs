using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
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

		    nudSignDiffFactor.Value = Math.Min(50, Math.Max(1, (decimal)Settings.Default.SignatureDiffFactorEx2));
            nudMinSignDiff.Value = Math.Min(10, Math.Max(0, (decimal)Settings.Default.MinSignatureDiff));
		    cbxGraphDebugMode.Checked = Settings.Default.VideoGraphDebugMode;
            tbxOutputLocation.Text = Settings.Default.OutputLocation;
            cbxTimeInUT.Checked = Settings.Default.DisplayTimeInUT;

		    tbxSimlatorFilePath.Text = Settings.Default.SimulatorFilePath;
            tbxOcrDebugOutputLocation.Text = Settings.Default.OcrDebugOutputFolder;
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

            Settings.Default.OutputLocation = tbxOutputLocation.Text;
            Settings.Default.DisplayTimeInUT = cbxTimeInUT.Checked;

            Settings.Default.SimulatorFilePath = tbxSimlatorFilePath.Text;
            Settings.Default.OcrDebugOutputFolder = tbxOcrDebugOutputLocation.Text;

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
	}
}
