using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WAT910BD.Tester.WAT910BDComms;

namespace WAT910BD.Tester
{
	public partial class frmMain : Form
	{
		private WAT910BDDriver m_WAT910Driver = new WAT910BDDriver();

		public frmMain()
		{
			InitializeComponent();

			cbxCOMPort.Items.AddRange(SerialPort.GetPortNames());
			if (cbxCOMPort.Items.Count > 0)
				cbxCOMPort.SelectedIndex = 0;

			btnConnect.Enabled = cbxCOMPort.SelectedIndex > -1;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}

			m_WAT910Driver.Dispose();

			base.Dispose(disposing);
		}


		private void btnConnect_Click(object sender, EventArgs e)
		{
			if (m_WAT910Driver.IsConnected)
			{
				if (m_WAT910Driver.Disconnect())
				{
					btnConnect.Text = "Connect";
					cbxCOMPort.Enabled = true;
					gbxCameraControl.Enabled = false;
				}
			}
			else
			{
				if (m_WAT910Driver.Connect((string) cbxCOMPort.SelectedItem))
				{
					btnConnect.Text = "Disconnect";
					cbxCOMPort.Enabled = false;
					gbxCameraControl.Enabled = true;
				}
			}			
		}

		private void cbxCOMPort_SelectedIndexChanged(object sender, EventArgs e)
		{
			btnConnect.Enabled = !m_WAT910Driver.IsConnected && cbxCOMPort.SelectedIndex > -1;
		}

		private void btnInitialise_Click(object sender, EventArgs e)
		{
			m_WAT910Driver.InitialiseCamera();
		}
	}
}
