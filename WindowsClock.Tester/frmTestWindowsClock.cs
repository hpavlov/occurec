using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;


namespace WindowsClock.Tester
{
	public partial class frmTestWindowsClock : Form
	{
		private bool m_Running = false;
		private int m_CheckPeriodSeconds = 20;
		private int m_NTPUpdatePeriodSeconds = 60;
		private string m_NTPServer1 = "";
        private string m_NTPServer2 = "";
        private string m_NTPServer3 = "";
        private string m_NTPServer4 = "";
		private List<double> m_AllWinTimeDiffs = new List<double>();
		private List<double> m_AllOccuRecTimeDiffs = new List<double>();
		private SerialPort m_HTCCPort;

		public frmTestWindowsClock()
		{
			InitializeComponent();

			cbxCOMPort.Items.AddRange(SerialPort.GetPortNames());
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
			base.Dispose(disposing);

			if (m_HTCCPort != null)
			{
				if (m_HTCCPort.IsOpen) m_HTCCPort.Close();
				m_HTCCPort = null;
			}
		}


		private void btnStartStopTest_Click(object sender, EventArgs e)
		{
			if (!m_Running)
			{
				if (string.IsNullOrEmpty((string)cbxCOMPort.SelectedItem))
				{
					MessageBox.Show(this, "You must select the COM port for HTCC timer", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				m_AllWinTimeDiffs.Clear();
				m_AllOccuRecTimeDiffs.Clear();
				m_CheckPeriodSeconds = (int) nudFrequency.Value;
				m_NTPUpdatePeriodSeconds = (int) nudFrequencyNTP.Value;
				m_NTPServer1 = tbxNTPServer.Text;
                m_NTPServer2 = tbxNTPServer2.Text;
                m_NTPServer3 = tbxNTPServer3.Text;
                m_NTPServer4 = tbxNTPServer4.Text;

				if (m_HTCCPort != null)
				{
					if (m_HTCCPort.IsOpen) m_HTCCPort.Close();
					m_HTCCPort = null;
				}

				m_HTCCPort = new SerialPort();
				m_HTCCPort.PortName = (string)cbxCOMPort.SelectedItem;
				m_HTCCPort.BaudRate = 115200;
				m_HTCCPort.DataBits = 8;
				m_HTCCPort.Parity = Parity.None;
				m_HTCCPort.StopBits = StopBits.One;
				m_HTCCPort.ReadTimeout = 1000;
				m_HTCCPort.Open();
				tbxMeasurements.Text = "WinAccu\t\tGpsTimeAccu\tOccuRecAccu\tError\tNTPAccu\tNTPLatency\r\n";
				 

				ThreadPool.QueueUserWorkItem(TestWorker);

				nudFrequency.Enabled = false;
				nudFrequencyNTP.Enabled = false;
				tbxNTPServer.Enabled = false;
                tbxNTPServer2.Enabled = false;
                tbxNTPServer3.Enabled = false;
                tbxNTPServer4.Enabled = false;
				cbxCOMPort.Enabled = false;

			
				lblAverageDiff.Text = "";
				btnStartStopTest.Text = "Stop Test";
			}
			else
			{
				m_Running = false;
				btnStartStopTest.Text = "Run Test";
				nudFrequency.Enabled = true;
				nudFrequencyNTP.Enabled = true;
				tbxNTPServer.Enabled = true;
                tbxNTPServer2.Enabled = true;
                tbxNTPServer3.Enabled = true;
                tbxNTPServer4.Enabled = true;
				cbxCOMPort.Enabled = true;
			}
		}

        private void Error(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

		private void AddMeasurement(float winTimeDiff, float occuRecTimeDiff, float occuRecTimeDiffErr, float currMaxError, float ntpAccu, float ntpAccuErr, bool timeUpdated, float httcMaxErr)
		{
			double val1, val2;
			float timeDriftStdDev;
			double winTimeDiffNormPri;
			if (float.IsNaN(httcMaxErr))
			{
				DateTime occuRecNormPri = NTPTimeKeeper.UtcNow(out val1, out val2, out timeDriftStdDev);
				DateTime winNormPri = DateTime.UtcNow;
				TimeSpan ts = new TimeSpan(winNormPri.Ticks - occuRecNormPri.Ticks);
				winTimeDiffNormPri = ts.TotalMilliseconds + occuRecTimeDiff;
			}
			else
				winTimeDiffNormPri = httcMaxErr;
			

			tbxMeasurements.Text += string.Format("{0}\t\t{1}\t\t{2}\t\t{3}\t{4}+/-{6}\t{5}{7}\r\n",
				winTimeDiff.ToString("0.0"),
				winTimeDiffNormPri.ToString("0.0"), 
				occuRecTimeDiff.ToString("0.0"), 
				occuRecTimeDiffErr.ToString("0.0"),
				ntpAccu.ToString("0.0"),
				currMaxError.ToString("0.0"),
				ntpAccuErr.ToString("0.00"),
				timeUpdated ? "*" : "");

			m_AllWinTimeDiffs.Add(winTimeDiff);

			if (m_AllWinTimeDiffs.Count > 1)
			{
				double average = m_AllWinTimeDiffs.Average();
				double sumResiduals = m_AllWinTimeDiffs.Select(x => (x - average) * (x - average)).Sum();
				double variance = Math.Sqrt(sumResiduals / (m_AllWinTimeDiffs.Count - 1));

				lblAverageDiff.Text = string.Format("{0} ms (1-sigma Error = {1} ms)", average.ToString("0.0"), variance.ToString("0.00"));
			}

			m_AllOccuRecTimeDiffs.Add(occuRecTimeDiff);

			if (m_AllOccuRecTimeDiffs.Count > 1)
			{
				double average = m_AllOccuRecTimeDiffs.Average();
				double sumResiduals = m_AllOccuRecTimeDiffs.Select(x => (x - average) * (x - average)).Sum();
				double variance = Math.Sqrt(sumResiduals / (m_AllOccuRecTimeDiffs.Count - 1));

				lblAverageDiffOccuRec.Text = string.Format("{0} ms (1-sigma Error = {1} ms)", average.ToString("0.0"), variance.ToString("0.00"));
			}
			
		}

		private void TestWorker(object state)
		{
			m_Running = true;
			bool winTimeFirst = true;

			Thread.CurrentThread.Priority = ThreadPriority.Highest;
			//Process.GetCurrentProcess().ProcessorAffinity = (IntPtr)1;

			DateTime nextNTPUpdateTime = DateTime.UtcNow;
			DateTime nextCheckTime = DateTime.UtcNow;

			float occuRecTimeDiff = 0;
			float occuRecTimeDiffErr = 0;
			float occuRecTimeDriftCorr = 0;
			float latencyInMilliseconds = 0;
			double maxError;
			double driftCorr;
			float winTimeDiff = 0;
			bool timeUpdated = false;
			DateTime initialTime;

		    string[] ntpServerList = new string[] {m_NTPServer1, m_NTPServer2, m_NTPServer3, m_NTPServer4};
		    var workingServers = new List<string>();
		    for (int attempts = 0; attempts < 3; attempts++)
		    {
                workingServers.Clear();
                for (int i = 0; i < 4; i++)
                {
                    try
                    {
                        initialTime = NTPClient.GetNetworkTime(ntpServerList[i], out latencyInMilliseconds);
                        NTPClient.SetTime(initialTime);
                        Trace.WriteLine(string.Format("Latency to {0} is {1} ms.", ntpServerList[i], latencyInMilliseconds.ToString("0.0")));
                        workingServers.Add(ntpServerList[i]);
                    }
                    catch (Exception e)
                    {

                    }

                    Thread.Sleep(100);
                }

                if (workingServers.Count >= 3)
		            break;
		    }

            if (workingServers.Count < 3)
            {
                Invoke(new Action<string>((err) => Error(err)), "Less than 3 NTP servers are responding. Cannot continue the test!");
            }
            else
            {
                DateTime winTime = DateTime.MinValue;
                DateTime occuRecTime = DateTime.MinValue;

                bool usesNTPTimeReference = m_HTCCPort != null;
                var httcClient = m_HTCCPort != null ? new HTCCClient(m_HTCCPort) : null;
                float htccLatency = float.NaN;
                float timeDriftStdDev = 0;
                string[] ntpServers = workingServers.ToArray();

                while (m_Running)
                {
					if (DateTime.UtcNow > nextNTPUpdateTime)
                    {
                        winTimeFirst = !winTimeFirst;
                        try
                        {
                            DateTime networkUTCTime = NTPClient.GetNetworkTime(ntpServers, out latencyInMilliseconds, out timeUpdated);

                            NTPClient.SetTime(networkUTCTime);
                        }
                        catch (Exception ex)
                        {
                            Trace.WriteLine(ex);
                        }

						nextNTPUpdateTime = DateTime.UtcNow.AddSeconds(m_NTPUpdatePeriodSeconds);
                    }

					if (DateTime.UtcNow > nextCheckTime)
					{
						try
						{
							double maxErrorMilliseconds = 0;
							double timeDriftCorrectionMilliseconds = 0;
							long occuRecTimeTicks = 0;
							float htccLatency1 = 0;
							float htccLatency2 = 0;
							double ntpAccu = 0;
							double ntpAccuErr = 0;

							long httcTicks = httcClient.TimeActionInUTC(
									() => occuRecTimeTicks = NTPTimeKeeper.UtcNowNoDriftCorrection(out maxErrorMilliseconds, out ntpAccu, out ntpAccuErr).Ticks,
									out htccLatency)
								.Ticks;
							TimeSpan timeDiff = new TimeSpan(occuRecTimeTicks - httcTicks);
							occuRecTimeDiff = (float)timeDiff.TotalMilliseconds;
							occuRecTimeDiffErr = (float)maxErrorMilliseconds;
							occuRecTimeDriftCorr = (float)timeDriftCorrectionMilliseconds;

							long winTimeTicks = 0;
							httcTicks = httcClient.TimeActionInUTC(
									() => winTimeTicks = DateTime.UtcNow.Ticks,
									out htccLatency2)
								.Ticks;
							winTimeDiff = (float)new TimeSpan(winTimeTicks - httcTicks).TotalMilliseconds;

							Invoke(new Action<float, float, float, float, float, float, bool, float>(
								(winDiff, diff, diffErr, maxErr, accu, accuErr, updated, httcMaxErr) =>
								AddMeasurement(winDiff, diff, diffErr, maxErr, accu, accuErr, updated, httcMaxErr)),
								winTimeDiff, occuRecTimeDiff, occuRecTimeDiffErr, latencyInMilliseconds, (float)ntpAccu, (float)ntpAccuErr, timeUpdated, htccLatency);
						}
						catch (Exception ex)
						{
							Trace.WriteLine(ex);
						}

						nextCheckTime = DateTime.UtcNow.AddSeconds(m_CheckPeriodSeconds);
					}

                    Thread.Sleep(200);
                }
            }
		}

		private void copyAllToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Clipboard.SetText(tbxMeasurements.Text);
		}

		private void btnPlotData_Click(object sender, EventArgs e)
		{
			var frm = new frmPlotClockData();
			frm.StartPosition = FormStartPosition.CenterParent;
			frm.ShowDialog(this);
		}
	}
}
