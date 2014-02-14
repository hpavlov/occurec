using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OccuRec.CameraDrivers.WAT910BD
{
	public partial class frmWAT910BDConnectionSettings : Form
	{
		public string DefaultComPort { get; set; }

		public frmWAT910BDConnectionSettings()
		{
			InitializeComponent();
		}

		private void frmWAT910BDConnectionSettings_Load(object sender, EventArgs e)
		{
			cbxCOMPort.Items.AddRange(SerialPort.GetPortNames());
			if (cbxCOMPort.Items.Count > 0)
			{
				int idx = !string.IsNullOrEmpty(DefaultComPort) ? cbxCOMPort.Items.IndexOf(DefaultComPort) : -1;
				cbxCOMPort.SelectedIndex = idx;
			}
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			string selectedComPort = (string) cbxCOMPort.SelectedItem;
			if (string.IsNullOrEmpty(selectedComPort))
			{
				MessageBox.Show(this, "Please select a COM port.", "WAT-910BD Configuration", MessageBoxButtons.OK, MessageBoxIcon.Error);
				cbxCOMPort.Focus();
				return;
			}

			DefaultComPort = selectedComPort;

			DialogResult = DialogResult.OK;
			Close();
		}

		private void llDriversLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("http://www.ftdichip.com/Drivers/VCP.htm");
		}
	}
}
