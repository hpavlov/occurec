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

namespace OccuRec.Config.Panels
{
	public partial class ucObservatoryControl : SettingsPanel
	{
		internal ObservatoryController ObservatoryController;
		private bool m_Initialised = false;

		public ucObservatoryControl()
		{
			InitializeComponent();
		}

		public override void LoadSettings()
		{
			cbxLiveTelescopeMode.Checked = Settings.Default.ASCOMConnectWhenRunning;
			cbxUseAppDomainIsolation.Checked = Settings.Default.ASCOMLoadInSeparateAppDomain;
			tbxFocuser.Text = Settings.Default.ASCOMProgIdFocuser;
			tbxTelescope.Text = Settings.Default.ASCOMProgIdTelescope;

			m_Initialised = true;
			UpdateASCOMControlsState();
		}

		public override void SaveSettings()
		{
			Settings.Default.ASCOMProgIdFocuser = tbxFocuser.Text;
			Settings.Default.ASCOMProgIdTelescope = tbxTelescope.Text;
			Settings.Default.ASCOMConnectWhenRunning = cbxLiveTelescopeMode.Checked;
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
			if (!cbxLiveTelescopeMode.Checked && ObservatoryController != null && m_Initialised)
				ObservatoryController.DisconnectASCOMDevices();

			UpdateASCOMControlsState();
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
	}
}
