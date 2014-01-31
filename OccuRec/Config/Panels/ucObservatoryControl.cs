using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using OccuRec.ASCOM.Wrapper;
using OccuRec.ASCOM.Wrapper.Interfaces;
using OccuRec.ASCOM;
using OccuRec.ASCOM.Interfaces.Devices;
using OccuRec.Helpers;
using OccuRec.Properties;
using OccuRec.Utilities;

namespace OccuRec.Config.Panels
{
	public partial class ucObservatoryControl : SettingsPanel
	{
		internal IObservatoryController ObservatoryController;
		private bool m_Initialised = false;

		public ucObservatoryControl()
		{
			InitializeComponent();
		}

		public override void LoadSettings()
		{
			cbxUseAppDomainIsolation.Checked = Settings.Default.ASCOMLoadInSeparateAppDomain;
			tbxFocuser.Text = Settings.Default.ASCOMProgIdFocuser;
			tbxTelescope.Text = Settings.Default.ASCOMProgIdTelescope;
            nudTelescopePingRate.SetNUDValue(Settings.Default.TelescopePingRateSeconds);

			m_Initialised = true;
			UpdateASCOMControlsState(null);
		}

		public override void SaveSettings()
		{
            Settings.Default.TelescopePingRateSeconds = (int)nudTelescopePingRate.Value;
			Settings.Default.ASCOMProgIdFocuser = tbxFocuser.Text;
			Settings.Default.ASCOMProgIdTelescope = tbxTelescope.Text;
			Settings.Default.ASCOMLoadInSeparateAppDomain = cbxUseAppDomainIsolation.Checked;
		}

		private void btnSelectTelescope_Click(object sender, EventArgs e)
		{
			string progId = ASCOMClient.Instance.ChooseTelescope();

			if (!string.IsNullOrEmpty(progId))
				tbxTelescope.Text = progId;
		}

		private void tbxFocuser_TextChanged(object sender, EventArgs e)
		{
			UpdateASCOMControlsState(null);
		}

		private void UpdateASCOMControlsState(ObservatoryControllerCallbackArgs args)
		{
			btnTestFocuserConnection.Enabled = !string.IsNullOrEmpty(tbxFocuser.Text);
			btnTestTelescopeConnection.Enabled = !string.IsNullOrEmpty(tbxTelescope.Text);

            if (ObservatoryController.IsConnectedToTelescope())
			{
				btnSelectTelescope.Enabled = false;
				btnTestTelescopeConnection.Enabled = false;
				btnClearTelescope.Enabled = false;
                btnConfigureTelescope.Enabled = false;
                btnDisconnectTelescope.Visible = true;
			    btnDisconnectTelescope.Enabled = true;
			}
			else
			{
                btnSelectTelescope.Enabled = true;
                btnTestTelescopeConnection.Enabled = true;
                btnClearTelescope.Enabled = true;
                btnConfigureTelescope.Enabled = true;
                btnDisconnectTelescope.Visible = false;
			}

            if (ObservatoryController.IsConnectedToFocuser())
            {
                btnSelectFocuser.Enabled = false;
                btnTestFocuserConnection.Enabled = false;
                btnClearFocuser.Enabled = false;
                btnConfigureFocuser.Enabled = false;
                btnDisconnectFocuser.Visible = true;
                btnDisconnectFocuser.Enabled = true;
            }
            else
            {
                btnSelectFocuser.Enabled = true;
                btnTestFocuserConnection.Enabled = true;
                btnClearFocuser.Enabled = true;
                btnConfigureFocuser.Enabled = true;
                btnDisconnectFocuser.Visible = false;
            }

		    if (ObservatoryController.IsConnectedToObservatory())
		    {
		        cbxUseAppDomainIsolation.Enabled = false;
		    }
		    else
		    {
                cbxUseAppDomainIsolation.Enabled = true;
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
			IFocuser focuser = null;
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
			UpdateASCOMControlsState(null);
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

		private void btnClearFocuser_Click(object sender, EventArgs e)
		{
			tbxFocuser.Text = string.Empty;
		}

		private void btnClearTelescope_Click(object sender, EventArgs e)
		{
			tbxTelescope.Text = string.Empty;
		}

		private void btnSelectFocuser_Click(object sender, EventArgs e)
		{
			string progId = ASCOMClient.Instance.ChooseFocuser();

			if (!string.IsNullOrEmpty(progId))
				tbxFocuser.Text = progId;
		}

        private void btnDisconnectFocuser_Click(object sender, EventArgs e)
        {
            if (ObservatoryController.IsConnectedToFocuser())
            {
                ObservatoryController.DisconnectFocuser(CallType.Async, UpdateASCOMControlsState);
                btnDisconnectFocuser.Enabled = false;
            }
        }

        private void btnDisconnectTelescope_Click(object sender, EventArgs e)
        {
            if (ObservatoryController.IsConnectedToTelescope())
            {
				ObservatoryController.DisconnectTelescope(CallType.Async, UpdateASCOMControlsState);
                btnDisconnectTelescope.Enabled = false;
            }
        }

        private void btnConfigureFocuser_Click(object sender, EventArgs e)
        {

        }

        private void btnConfigureTelescope_Click(object sender, EventArgs e)
        {

        }
	}
}
