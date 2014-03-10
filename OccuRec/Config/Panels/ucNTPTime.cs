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
			cbxRecordNTPTimeStamps.Checked = Settings.Default.RecordNTPTimeStampInAAV;
			cbxRecordSecondaryTimeStamps.Checked = Settings.Default.RecordSecondaryTimeStampInAav;

			tbxNTPServer.Text = Settings.Default.NTPServer1;
			tbxNTPServer2.Text = Settings.Default.NTPServer2;
			tbxNTPServer3.Text = Settings.Default.NTPServer3;
			tbxNTPServer4.Text = Settings.Default.NTPServer4;
			nudHardwareLatencyCorrection.SetNUDValue(Settings.Default.NTPTimingHardwareCorrection);
			nudNumberOfNTPRequestsPerUpdate.SetNUDValue(Settings.Default.NumberOfNTPRequestsPerUpdate);

			gbxNTPTime.Enabled = cbxRecordNTPTimeStamps.Checked;
		}

		public override void SaveSettings()
		{
			Settings.Default.NTPServer1 = tbxNTPServer.Text.Trim().ToLower();
			Settings.Default.NTPServer2 = tbxNTPServer2.Text.Trim().ToLower();
			Settings.Default.NTPServer3 = tbxNTPServer3.Text.Trim().ToLower();
			Settings.Default.NTPServer4 = tbxNTPServer4.Text.Trim().ToLower();
			Settings.Default.NTPTimingHardwareCorrection = (int)nudHardwareLatencyCorrection.Value;
			Settings.Default.NumberOfNTPRequestsPerUpdate = (int)nudNumberOfNTPRequestsPerUpdate.Value;
			Settings.Default.RecordNTPTimeStampInAAV = cbxRecordNTPTimeStamps.Checked;
			Settings.Default.RecordSecondaryTimeStampInAav = cbxRecordSecondaryTimeStamps.Checked;
		}

		public override bool ValidateSettings()
		{
			if (cbxRecordNTPTimeStamps.Checked)
			{
				string server1 = tbxNTPServer.Text.Trim().ToLower();
				string server2 = tbxNTPServer2.Text.Trim().ToLower();
				string server3 = tbxNTPServer3.Text.Trim().ToLower();
				string server4 = tbxNTPServer4.Text.Trim().ToLower();

				if (server1 == string.Empty)
				{
					ShowMissingServerErrorMessage("NTP Server 1");
					return false;
				}

                if (server2 == string.Empty && (server3 != string.Empty || server4 != string.Empty))
				{
					ShowMissingServerErrorMessage("NTP Server 2");
					return false;
				}

                if (server3 == string.Empty && server4 != string.Empty)
				{
					ShowMissingServerErrorMessage("NTP Server 3");
					return false;
				}

				if (server1 == server2)
				{
					ShowDuplicatedServerErrorMessage(server1);
					return false;
				}
				if (server1 == server3)
				{
					ShowDuplicatedServerErrorMessage(server1);
					return false;
				}
				if (server1 == server4)
				{
					ShowDuplicatedServerErrorMessage(server1);
					return false;
				}
                if (server2 != string.Empty && server2 == server3)
				{
					ShowDuplicatedServerErrorMessage(server2);
					return false;
				}
                if (server2 != string.Empty && server2 == server4)
				{
					ShowDuplicatedServerErrorMessage(server2);
					return false;
				}
                if (server3 != string.Empty && server3 == server4)
				{
					ShowDuplicatedServerErrorMessage(server3);
					return false;
				}
			}

			return true;
		}

		private void ShowDuplicatedServerErrorMessage(string serverName)
		{
			MessageBox.Show(string.Format("Server {0} is listed more than once. Each server must be unique.", serverName), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		private void ShowMissingServerErrorMessage(string serverName)
		{
			MessageBox.Show(string.Format("Server {0} is not specified.", serverName), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
				if (ValidateSettings())
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
			string[] ntpServerList = new string[] { tbxNTPServer.Text, tbxNTPServer2.Text, tbxNTPServer3.Text, tbxNTPServer4.Text };
			var workingServers = new List<string>();

	        try
	        {
		        for (int attempts = 0; attempts < 3; attempts++)
		        {
			        workingServers.Clear();
			        for (int i = 0; i < 4; i++)
			        {
				        try
				        {
					        float latencyInMilliseconds;
					        DateTime initialTime = NTPClient.GetNetworkTime(ntpServerList[i], true, out latencyInMilliseconds);
					        NTPClient.SetTime(initialTime);

					        Trace.WriteLine(string.Format("Latency to {0} is {1} ms.", ntpServerList[i], latencyInMilliseconds.ToString("0.0")));
					        workingServers.Add(ntpServerList[i]);
				        }
				        catch
				        {
				        }

				        Thread.Sleep(100);
			        }

			        if (workingServers.Count >= 3)
				        break;
		        }
	        }
	        catch
	        { }

			if (workingServers.Count == 0)
			{
				try
				{
					this.Invoke(new Action<float?, Exception>(OnNewLatencyMeasurement), new object[] { null, new Exception(string.Format("None of the specified servers are responding: {1}", workingServers.Count, string.Join(", ", workingServers.ToArray()))) });
				}
				catch (InvalidOperationException) { }

				return;
			}

	        float avrgLatency;
			while (m_CheckingLatency)
			{
				avrgLatency = float.NaN;
				try
				{
					avrgLatency = 0;
					float latency;
					int aliveServers = 0;
					bool timeUpdated;
					NTPClient.GetNetworkTimeFromMultipleServers(workingServers.ToArray(), out latency, out aliveServers, out timeUpdated);

					avrgLatency = latency;
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

		private void cbxRecordNTPTimeStamps_CheckedChanged(object sender, EventArgs e)
		{
			gbxNTPTime.Enabled = cbxRecordNTPTimeStamps.Checked;
		}
	}
}
