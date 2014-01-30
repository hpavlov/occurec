using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using OccuRec.Helpers;
using OccuRec.Properties;

namespace OccuRec.Config.Panels
{
	public partial class ucNTPTime : SettingsPanel
	{
		public ucNTPTime()
		{
			InitializeComponent();
		}

		public override void LoadSettings()
		{
			tbxNTPServer.Text = Settings.Default.NTPServer;
		}

		public override void SaveSettings()
		{
			Settings.Default.NTPServer = tbxNTPServer.Text;
		}

		private void llblFindNTP_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
            Process.Start("http://support.ntp.org/bin/view/Servers/WebHome");
		}

	    private Exception m_Exception;
        private float m_Latency;

        private void btnTestNTP_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            ThreadPool.UnsafeQueueUserWorkItem(CheckLatency, null);
        }

        private void CheckCompleted()
        {

            if (m_Exception != null)
                MessageBox.Show(this, m_Exception.Message, "OccuRec", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
                MessageBox.Show(this,
                string.Format("The latency to {0} is {1} ms.\r\n\r\nThis will be a typical 3-Sigma timing error if this server is used to time video.", tbxNTPServer.Text, m_Latency.ToString("0.0")),
                "OccuRec", MessageBoxButtons.OK, MessageBoxIcon.Information);

            Cursor = Cursors.Default;
        }

        private void CheckLatency(object state)
        {
            m_Latency = float.NaN;
            m_Exception = null;
            try
            {
                m_Latency = 0;
                float latency;
                DateTime dt = NTPClient.GetNetworkTime(tbxNTPServer.Text, out latency);
                m_Latency += latency;

                dt = NTPClient.GetNetworkTime(tbxNTPServer.Text, out latency);
                m_Latency += latency;

                dt = NTPClient.GetNetworkTime(tbxNTPServer.Text, out latency);
                m_Latency += latency;

                m_Latency /= 3;
            }
            catch (Exception ex)
            {
                m_Exception = ex;
            }
            finally
            {
                this.Invoke(new Action(CheckCompleted));
            }
        }
	}
}
