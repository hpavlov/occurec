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

			m_WAT910Driver.OnCommandExecutionCompleted += m_WAT910Driver_OnCommandExecutionCompleted;
            m_WAT910Driver.OnSerialComms += m_WAT910Driver_OnSerialComms;
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

		void m_WAT910Driver_OnCommandExecutionCompleted(WAT910DBEventArgs e)
		{
			if (!e.IsSuccessful && !e.ReceivedResponseFromCamera)
                textBox1.AppendText(string.Format("ERR:{0}\r\n", e.ErrorMessage));

			EnableDisableControls(true);
		}

        void m_WAT910Driver_OnSerialComms(SerialCommsEventArgs e)
        {
            string message = FormatBytesHex(e.Data);
            if (e.Sent)
                textBox1.AppendText("SENT: " + message + "\r\n");
            else
				textBox1.AppendText("RCVD: " + message + " " + e.Message + "\r\n");
        }

        private string FormatBytesHex(byte[] data)
        {
            var output = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                output.AppendFormat("{0} ", data[i].ToString("x2").ToUpper());
            }
            return output.ToString();
        }

		private void EnableDisableControls(bool enable)
		{
			gbxCameraControl.Enabled = enable;

		    lblGain.Text = m_WAT910Driver.Gain;
			btnGainDown.Enabled = m_WAT910Driver.GainIndex > m_WAT910Driver.MinGainIndex;
			btnGainUp.Enabled = m_WAT910Driver.GainIndex < m_WAT910Driver.MaxGainIndex;

			lblExposure.Text = m_WAT910Driver.Exposure;
			btnExposureDown.Enabled = m_WAT910Driver.ExposureIndex > m_WAT910Driver.MinExposureIndex;
			btnExposureUp.Enabled = m_WAT910Driver.ExposureIndex < m_WAT910Driver.MaxExposureIndex;

			lblGamma.Text = m_WAT910Driver.Gamma;
			btnGammaDown.Enabled = m_WAT910Driver.GammaIndex > m_WAT910Driver.MinGammaIndex;
			btnGammaUp.Enabled = m_WAT910Driver.GammaIndex < m_WAT910Driver.MaxGammaIndex;
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
			EnableDisableControls(false);
			Invoke(new Action(() => m_WAT910Driver.InitialiseCamera()));
		}

		private void button2_Click(object sender, EventArgs e)
		{
			EnableDisableControls(false);
			Invoke(new Action(() => m_WAT910Driver.OSDCommandUp()));
		}

		private void button3_Click(object sender, EventArgs e)
		{
			EnableDisableControls(false);
			Invoke(new Action(() => m_WAT910Driver.OSDCommandDown()));
		}

		private void button1_Click(object sender, EventArgs e)
		{
			EnableDisableControls(false);
			Invoke(new Action(() => m_WAT910Driver.OSDCommandLeft()));
		}

		private void button4_Click(object sender, EventArgs e)
		{
			EnableDisableControls(false);
			Invoke(new Action(() => m_WAT910Driver.OSDCommandRight()));
		}

		private void button5_Click(object sender, EventArgs e)
		{
			EnableDisableControls(false);
			Invoke(new Action(() => m_WAT910Driver.OSDCommandSet()));
		}

        private void btnGainUp_Click(object sender, EventArgs e)
        {
            EnableDisableControls(false);
            Invoke(new Action(() => m_WAT910Driver.GainUp()));
        }

        private void btnGainDown_Click(object sender, EventArgs e)
        {
            EnableDisableControls(false);
            Invoke(new Action(() => m_WAT910Driver.GainDown()));
        }

		private void btnExposureDown_Click(object sender, EventArgs e)
		{

			EnableDisableControls(false);
			Invoke(new Action(() => m_WAT910Driver.ExposureDown()));
		}

		private void btnExposureUp_Click(object sender, EventArgs e)
		{
			EnableDisableControls(false);
			Invoke(new Action(() => m_WAT910Driver.ExposureUp()));
		}

		private void btnGammaDown_Click(object sender, EventArgs e)
		{
			EnableDisableControls(false);
			Invoke(new Action(() => m_WAT910Driver.GammaDown()));
		}

		private void btnGammaUp_Click(object sender, EventArgs e)
		{
			EnableDisableControls(false);
			Invoke(new Action(() => m_WAT910Driver.GammaUp()));
		}

		private void btnReadSettings_Click(object sender, EventArgs e)
		{
			EnableDisableControls(false);
			Invoke(new Action(() => m_WAT910Driver.ReadCurrentCameraSettings()));
		}

		private void btnReadState_Click(object sender, EventArgs e)
		{
			EnableDisableControls(false);
			Invoke(new Action(() => m_WAT910Driver.ReadCurrentCameraState()));
		}
	}
}
