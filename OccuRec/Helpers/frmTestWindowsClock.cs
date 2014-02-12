using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using OccuRec.Properties;

namespace OccuRec.Helpers
{
	public partial class frmTestWindowsClock : Form
	{
		private bool m_Running = false;
		private int m_CheckPeriodSeconds = 20;
		private List<double> m_AllWinTimeDiffs = new List<double>();

		public frmTestWindowsClock()
		{
			InitializeComponent();
		}

		private void btnStartStopTest_Click(object sender, EventArgs e)
		{
			if (!m_Running)
			{
				m_AllWinTimeDiffs.Clear();
				m_CheckPeriodSeconds = (int) nudFrequency.Value;

				ThreadPool.QueueUserWorkItem(TestWorker);

				nudFrequency.Enabled = false;
				lbMeasurementsWinTime.Items.Clear();
				lblAverageDiff.Text = "";
				btnStartStopTest.Text = "Stop Test";
			}
			else
			{
				m_Running = false;
				btnStartStopTest.Text = "Run Test";
				nudFrequency.Enabled = true;
			}
		}

		private void AddMeasurement(TimeSpan ts, float occuRecTimeDiff, float occuRecTimeDiffErr, float currMaxError)
		{
			lbMeasurementsWinTime.Items.Add(string.Format("WinAccu: {0}\tOccuRecAccu: {1} +/- {2}\tNTPAccu:{3}", ts.TotalMilliseconds.ToString("0.0"), occuRecTimeDiff.ToString("0.0"), occuRecTimeDiffErr.ToString("0.0"), currMaxError.ToString("0.0")));
			m_AllWinTimeDiffs.Add(ts.TotalMilliseconds);

			if (m_AllWinTimeDiffs.Count > 1)
			{
				double average = m_AllWinTimeDiffs.Average();
				double sumResiduals = m_AllWinTimeDiffs.Select(x => (x - average) * (x - average)).Sum();
				double variance = Math.Sqrt(sumResiduals / (m_AllWinTimeDiffs.Count - 1));

				lblAverageDiff.Text = string.Format("{0} ms (1-sigma Error = {1} ms)", average.ToString("0.0"), variance.ToString("0.00"));
			}
		}

		private void TestWorker(object state)
		{
			m_Running = true;
			DateTime nextCheckTime = DateTime.UtcNow;

			while (m_Running)
			{
				if (DateTime.UtcNow > nextCheckTime)
				{
					try
					{
						float occuRecTimeDiff = 0;
						float occuRecTimeDiffErr = 0;
						float latencyInMilliseconds;
						DateTime networkUTCTime = NTPClient.GetNetworkTime(Settings.Default.NTPServer, true, out latencyInMilliseconds, ref occuRecTimeDiff, ref occuRecTimeDiffErr);
						NTPClient.SetTime(networkUTCTime);
						double maxError;
						DateTime ntpUTCNow = NTPTimeKeeper.UtcNow(out maxError);
						TimeSpan ts = new TimeSpan(DateTime.UtcNow.Ticks - ntpUTCNow.Ticks);

						Invoke(new Action<TimeSpan, float, float, float>((span, diff, diffErr, maxErr) => AddMeasurement(span, diff, diffErr, maxErr)), ts, occuRecTimeDiff, occuRecTimeDiffErr, latencyInMilliseconds);
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
			var rv = new StringBuilder();
			for (int i = 0; i < lbMeasurementsWinTime.Items.Count; i++)
			{
				rv.AppendLine(Convert.ToString(lbMeasurementsWinTime.Items[i]));
			}
			Clipboard.SetText(rv.ToString());
		}
	}
}
