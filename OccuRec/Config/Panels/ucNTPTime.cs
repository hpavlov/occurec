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
using OccuRec.Utilities;

namespace OccuRec.Config.Panels
{
	public partial class ucNTPTime : SettingsPanel
	{
		public ucNTPTime()
		{
			InitializeComponent();
		}

		public ucNTPTime(bool canChangeGrabberSettings)
			: this()
		{
			nudHardwareLatencyCorrection.Enabled = canChangeGrabberSettings;
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			StopLatencyTesting();

			if (disposing && (components != null))
			{
				components.Dispose();
			}
			
			base.Dispose(disposing);			
		}

		public override void LoadSettings()
		{
			tbxNTPServer.Text = Settings.Default.NTPServer;
			nudHardwareLatencyCorrection.SetNUDValue((decimal)Settings.Default.NTPTimingHardwareCorrection);
		}

		public override void SaveSettings()
		{
			Settings.Default.NTPServer = tbxNTPServer.Text;
			Settings.Default.NTPTimingHardwareCorrection = (int)nudHardwareLatencyCorrection.Value;
		}

		private void llblFindNTP_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
            Process.Start("http://support.ntp.org/bin/view/Servers/WebHome");
		}

		private bool m_CheckingLatency = false;

        private void btnTestNTP_Click(object sender, EventArgs e)
        {
			if (m_CheckingLatency)
			{
				StopLatencyTesting();
			}
			else
			{
				StartLatencyTesting();
			}
        }

		private void StartLatencyTesting()
		{
			if (!m_CheckingLatency)
			{
				m_CheckingLatency = true;
				m_LatencyList.Clear();
				DisplayLatencyList();
				ThreadPool.UnsafeQueueUserWorkItem(CheckLatency, null);
				btnTestNTP.Text = "Stop Test";
				lblTestingIndicator.Visible = true;
				indicatorTimer.Enabled = true;
			}
		}

		private void StopLatencyTesting()
		{
			if (m_CheckingLatency)
			{
				m_CheckingLatency = false;
				btnTestNTP.Text = "Test Latency";
				indicatorTimer.Enabled = false;
				lblTestingIndicator.Visible = false; 				
			}
		}

		private List<float> m_LatencyList = new List<float>(); 

        private void OnNewLatencyMeasurement(float? latency, Exception exception)
        {
	        if (exception != null)
	        {
		        MessageBox.Show(this, exception.Message, "NTP Latency Testing Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		        return;
	        }
	        else
	        {
		        if (m_LatencyList.Count >= 9) m_LatencyList.RemoveAt(0);
		        if (latency.HasValue)
			        m_LatencyList.Add(latency.Value);
		        else
		        {
			        // NOTE: A null value mean - we finished with the measurements			        
					StopLatencyTesting();
		        }
	        }

	        DisplayLatencyList();
        }

		private void DisplayLatencyList()
		{
			Label[] labels = new Label[] { lblLatency1, lblLatency2, lblLatency3, lblLatency4, lblLatency5, lblLatency6, lblLatency7, lblLatency8, lblLatency9 };
			for (int i = 0; i < 9; i++)
			{
				if (i < m_LatencyList.Count)
				{
					if (float.IsNaN(m_LatencyList[i]))
					{
						labels[i].Text = "timeout";
					}
					else if (m_LatencyList[i] >= 100)
						labels[i].Text = string.Format("{0}ms", (int)Math.Round(m_LatencyList[i]));
					else
						labels[i].Text = string.Format("{0}ms", m_LatencyList[i].ToString("0.0"));

					labels[i].Visible = true;
				}
				else
				{
					labels[i].Visible = false;
				}
			}

			lblLatencyTitle.Visible = m_LatencyList.Count > 0 || m_CheckingLatency;			
		}

        private void CheckLatency(object state)
        {
			float avrgLatency;
			while (m_CheckingLatency)
			{
				avrgLatency = float.NaN;
				try
				{
					avrgLatency = 0;
					float latency;
					DateTime dt = NTPClient.GetNetworkTime(tbxNTPServer.Text, out latency);
					avrgLatency += latency;

					if (!float.IsNaN(avrgLatency))
					{
						dt = NTPClient.GetNetworkTime(tbxNTPServer.Text, out latency);
						avrgLatency += latency;						
					}

					if (!float.IsNaN(avrgLatency))
					{
						dt = NTPClient.GetNetworkTime(tbxNTPServer.Text, out latency);
						avrgLatency += latency;
					}

					avrgLatency /= 3;
				}
				catch (Exception ex)
				{
					try
					{
						this.Invoke(new Action<float?, Exception>(OnNewLatencyMeasurement), new object[] { avrgLatency, ex });	
					}
					catch (InvalidOperationException) { }					
					break;
				}
				finally
				{
					try
					{
						this.Invoke(new Action<float?, Exception>(OnNewLatencyMeasurement), new object[] {avrgLatency, null});
					}
					catch (InvalidOperationException) { }
				}

				Thread.Sleep(1000);
			}

			try
			{
				this.Invoke(new Action<float?, Exception>(OnNewLatencyMeasurement), new object[] { null, null });
			}
			catch (InvalidOperationException) { }
        }

		private void indicatorTimer_Tick(object sender, EventArgs e)
		{
			lblTestingIndicator.Visible = !lblTestingIndicator.Visible;
		}

		private void btnTestWindowsClock_Click(object sender, EventArgs e)
		{
			var frm = new frmTestWindowsClock();
			frm.StartPosition = FormStartPosition.CenterParent;
			frm.ShowDialog(this);
		}
	}
}
