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
		private string m_NTPServer = "";
		private List<double> m_AllWinTimeDiffs = new List<double>();
		private List<double> m_AllOccuRecTimeDiffs = new List<double>();
		private SerialPort m_HTCCPort;

		public frmTestWindowsClock()
		{
			InitializeComponent();

			cbxCOMPort.Items.AddRange(SerialPort.GetPortNames());
		}

		private void btnStartStopTest_Click(object sender, EventArgs e)
		{
			if (!m_Running)
			{
				m_AllWinTimeDiffs.Clear();
				m_AllOccuRecTimeDiffs.Clear();
				m_CheckPeriodSeconds = (int) nudFrequency.Value;
				m_NTPServer = tbxNTPServer.Text;
				m_HTCCPort = null;
				if (cbxHTCC.Checked && cbxCOMPort.SelectedItem != null)
				{
					m_HTCCPort = new SerialPort();
					m_HTCCPort.PortName = (string) cbxCOMPort.SelectedItem;
					m_HTCCPort.BaudRate = 115200;
					m_HTCCPort.DataBits = 8;
					m_HTCCPort.Parity = Parity.None;
					m_HTCCPort.StopBits = StopBits.One;
					m_HTCCPort.ReadTimeout = 1000;
					tbxMeasurements.Text = "WinAccu\t\tGpsTimeAccu\tOccuRecAccu\tError\tDriftCorr\tNTPAccu\r\n";
				}
				else
					tbxMeasurements.Text = "WinAccu\t\tWinAccuNorm\tOccuRecAccu\tError\tDriftCorr\tNTPAccu\r\n";
				 

				ThreadPool.QueueUserWorkItem(TestWorker);

				nudFrequency.Enabled = false;
				tbxNTPServer.Enabled = false;
				cbxHTCC.Enabled = false;
				cbxCOMPort.Enabled = false;

			
				lblAverageDiff.Text = "";
				btnStartStopTest.Text = "Stop Test";
			}
			else
			{
				m_Running = false;
				btnStartStopTest.Text = "Run Test";
				nudFrequency.Enabled = true;
				tbxNTPServer.Enabled = true;
				cbxHTCC.Enabled = true;
				cbxCOMPort.Enabled = cbxHTCC.Checked;
			}
		}

		private void AddMeasurement(float winTimeDiff, float occuRecTimeDiff, float occuRecTimeDiffErr, float currMaxError, float driftCorr, float driftCorrErr, bool timeUpdated, float httcMaxErr)
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
				driftCorr.ToString("0.0"),
				currMaxError.ToString("0.0"),
				driftCorrErr.ToString("0.00"),
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

			DateTime nextCheckTime = DateTime.UtcNow;

			float occuRecTimeDiff = 0;
			float occuRecTimeDiffErr = 0;
			float occuRecTimeDriftCorr = 0;
			float latencyInMilliseconds;
			double maxError;
			double driftCorr;
			float winTimeDiff = 0;
			bool timeUpdated = false;
			DateTime initialTime;

			for (int i = 0; i < 3; i++)
			{
				try
				{
					initialTime = NTPClient.GetNetworkTime(m_NTPServer, out latencyInMilliseconds);
					NTPClient.SetTime(initialTime);
				}
				catch { }

				Thread.Sleep(1000);				
			}
			
			DateTime winTime = DateTime.MinValue;
			DateTime occuRecTime = DateTime.MinValue;

			bool usesNTPTimeReference = m_HTCCPort != null;
			var httcClient = new HTCCClient(m_HTCCPort);
			float htccLatency = float.NaN;
			float timeDriftStdDev = 0;

			while (m_Running)
			{
				if (DateTime.UtcNow > nextCheckTime)
				{
					winTimeFirst = !winTimeFirst;
					try
					{
						DateTime networkUTCTime = NTPClient.GetNetworkTime(
							m_NTPServer, usesNTPTimeReference, out latencyInMilliseconds,
							ref occuRecTimeDiff, ref occuRecTimeDiffErr, ref occuRecTimeDriftCorr, ref timeDriftStdDev, ref winTimeDiff, out timeUpdated);

						NTPClient.SetTime(networkUTCTime);

						if (httcClient != null)
						{
							double maxErrorMilliseconds = 0;
							double timeDriftCorrectionMilliseconds = 0;
							long occuRecTimeTicks = 0;
							float htccLatency1, htccLatency2;

							long httcTicks = httcClient.TimeActionInUTC(
									() => occuRecTimeTicks = NTPTimeKeeper.UtcNow(out maxErrorMilliseconds, out timeDriftCorrectionMilliseconds, out timeDriftStdDev).Ticks,
									out htccLatency1)
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

							htccLatency = (htccLatency1 + htccLatency2) / 2.0f;
						}

						Invoke(new Action<float, float, float, float, float, float, bool, float>(
							(winDiff, diff, diffErr, maxErr, corr, corrErr, updated, httcMaxErr) =>
							AddMeasurement(winDiff, diff, diffErr, maxErr, corr, corrErr, updated, httcMaxErr)),
							winTimeDiff, occuRecTimeDiff, occuRecTimeDiffErr, latencyInMilliseconds, occuRecTimeDriftCorr, timeDriftStdDev, timeUpdated, htccLatency);
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

		private void cbxHTCC_CheckedChanged(object sender, EventArgs e)
		{
			cbxCOMPort.Enabled = cbxHTCC.Enabled;
		}
	}
}
