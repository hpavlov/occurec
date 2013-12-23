using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using OccRec.ASCOMWrapper;
using OccuRec.ASCOM;
using OccuRec.ASCOM.Interfaces.Devices;
using OccuRec.Helpers;
using OccuRec.OCR;
using OccuRec.Properties;

namespace OccuRec
{
	public partial class frmSettings : Form
	{
	    private OcrSettings m_OCRSettings;

	    internal TelescopeController TelescopeController;
	    private bool m_Initialised = false;

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

		    cbxLiveTelescopeMode.Checked = Settings.Default.ASCOMConnectWhenRunning;
			tbxFocuser.Text = Settings.Default.ASCOMProgIdFocuser;
			tbxTelescope.Text = Settings.Default.ASCOMProgIdTelescope;

            UpdateControls();
			UpdateASCOMControlsState();
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

	        Settings.Default.ASCOMProgIdFocuser = tbxFocuser.Text;
			Settings.Default.ASCOMProgIdTelescope = tbxTelescope.Text;
            Settings.Default.ASCOMConnectWhenRunning = cbxLiveTelescopeMode.Checked;
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

		private void button1_Click(object sender, EventArgs e)
		{
			string progId = ASCOMClient.Instance.ChooseFocuser();

			if (!string.IsNullOrEmpty(progId))
				tbxFocuser.Text = progId;
		}

		private void btnSelectTelescope_Click(object sender, EventArgs e)
		{
			string progId = ASCOMClient.Instance.ChooseTelescope();

			if (!string.IsNullOrEmpty(progId))
				tbxTelescope.Text = progId;
		}

		private void tbxFocuser_TextChanged(object sender, EventArgs e)
		{
			UpdateASCOMControlsState();
		}

		private void UpdateASCOMControlsState()
		{
			btnTestFocuserConnection.Enabled = !string.IsNullOrEmpty(tbxFocuser.Text);
			btnTestTelescopeConnection.Enabled = !string.IsNullOrEmpty(tbxTelescope.Text);

            if (cbxLiveTelescopeMode.Checked)
            {
                btnSelectFocuser.Enabled = false;
                btnSelectTelescope.Enabled = false;
                btnTestFocuserConnection.Enabled = false;
                btnTestTelescopeConnection.Enabled = false;
				btnClearFocuser.Enabled = false;
				btnClearTelescope.Enabled = false;
            }
            else
            {
                btnSelectFocuser.Enabled = true;
                btnSelectTelescope.Enabled = true;
                btnTestFocuserConnection.Enabled = true;
                btnTestTelescopeConnection.Enabled = true;
				btnClearFocuser.Enabled = true;
				btnClearTelescope.Enabled = true;
            }
		}

		private void btnTestFocuserConnection_Click(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(tbxFocuser.Text))
			{
                Cursor = Cursors.WaitCursor;
                ThreadPool.QueueUserWorkItem(GetFocuserInfoWorker, tbxFocuser.Text);
			}
		}

        private void GetFocuserInfoWorker(object state)
        {
            string progId = state as string;
            IASCOMFocuser focuser = null;
            try
            {
                focuser = ASCOMClient.Instance.CreateFocuser(progId);
                focuser.Connected = true;
                Invoke(new Action<string, Exception>(OnFocuserInfoAvailable), string.Format("{0} ver {1}", focuser.Description, focuser.DriverVersion), null);
            }
            catch (Exception ex)
            {
                Invoke(new Action<string, Exception>(OnFocuserInfoAvailable), null, ex);
            }
            finally
            {
                if (focuser != null)
                {
                    focuser.Connected = false;
                    ASCOMClient.Instance.ReleaseDevice(focuser);
                }
            }
        }

        private void OnFocuserInfoAvailable(string info, Exception error)
        {
            if (error != null)
                MessageBox.Show(error.Message);
            else
            {
                lblConnectedFocuserInfo.Text = info;
                lblConnectedFocuserInfo.Visible = true;
            }

            Cursor = Cursors.Default;
        }

		private void tbxTelescope_TextChanged(object sender, EventArgs e)
		{
			UpdateASCOMControlsState();
		}

		private void btnTestTelescopeConnection_Click(object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(tbxTelescope.Text))
			{
                Cursor = Cursors.WaitCursor;
			    ThreadPool.QueueUserWorkItem(GetTelescopeInfoWorker, tbxTelescope.Text);
			}
		}

        private void GetTelescopeInfoWorker(object state)
        {
            string progId = state as string;
            IASCOMTelescope telescope = null;
            try
            {
                telescope = ASCOMClient.Instance.CreateTelescope(progId);
                telescope.Connected = true;
                Invoke(new Action<string, Exception>(OnTelescopeInfoAvailable), string.Format("{0} ver {1}", telescope.Description, telescope.DriverVersion), null);
            }
            catch (Exception ex)
            {
                Invoke(new Action<string, Exception>(OnTelescopeInfoAvailable), null, ex);
            }
            finally
            {
                if (telescope != null)
                {
                    telescope.Connected = false;
                    ASCOMClient.Instance.ReleaseDevice(telescope);
                }
            }
        }

        private void OnTelescopeInfoAvailable(string info, Exception error)
        {
            if (error != null)
                MessageBox.Show(error.Message);
            else
            {
                lblConnectedTelescopeInfo.Text = info;
                lblConnectedTelescopeInfo.Visible = true;
            }

            Cursor = Cursors.Default;
        }

        private void cbxLiveTelescopeMode_CheckedChanged(object sender, EventArgs e)
        {
            if (!cbxLiveTelescopeMode.Checked && TelescopeController != null && m_Initialised)
                TelescopeController.DisconnectASCOMDevices();

            UpdateASCOMControlsState();
        }

        private void frmSettings_Shown(object sender, EventArgs e)
        {
            m_Initialised = true;
        }

		private void btnClearFocuser_Click(object sender, EventArgs e)
		{
			tbxFocuser.Text = string.Empty;
		}

		private void btnClearTelescope_Click(object sender, EventArgs e)
		{
			tbxTelescope.Text = string.Empty;
		}
	}
}
