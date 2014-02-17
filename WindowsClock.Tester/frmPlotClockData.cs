using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsClock.Tester
{
	public partial class frmPlotClockData : Form
	{
		private LogData m_Data;

		public frmPlotClockData()
		{
			InitializeComponent();

			m_Data = null;
		}

		private void miFileOpen_Click(object sender, EventArgs e)
		{
			if (openFileDialog.ShowDialog(this) == DialogResult.OK)
			{
				m_Data = new LogData(openFileDialog.FileName);

				float maxWinAccu = m_Data.Data.MaxNth(x => x.WinAccu, 10);
				float minWinAccu = m_Data.Data.MinNth(x => x.WinAccu, 10);
				float maxOccuRecAccu = m_Data.Data.MaxNth(x => x.OccuRecAccu, 10);
				float minOccuRecAccu = m_Data.Data.MinNth(x => x.OccuRecAccu, 10);
				float maxNTPAccu = m_Data.Data.MaxNth(x => x.NTPAccu, 10);
				float minNTPAccu = m_Data.Data.MinNth(x => x.NTPAccu, 10);

				nudMinPlot.Value = (decimal)Math.Min(Math.Min(minWinAccu, minOccuRecAccu), minNTPAccu);
				nudMaxPlot.Value = (decimal)Math.Max(Math.Max(maxWinAccu, maxOccuRecAccu), maxNTPAccu);

				PlotData(m_Data);
			}
		}

		private void PlotData(LogData data)
		{
			float minPlot = (float)nudMinPlot.Value;
			float maxPlot = (float)nudMaxPlot.Value;

			float midPlot = (minPlot + maxPlot) / 2;
			float plotRange = 1.1f * (maxPlot - minPlot);

			float scaleY = pboxPlot.Height /plotRange;
			float scaleX = pboxPlot.Width / (1.1f * data.Data.Count);
			float zeroY = (pboxPlot.Height / 2) - pboxPlot.Height * 0.05f - midPlot * scaleY;
			float zeroX = pboxPlot.Width * 0.05f;
			float maxX = pboxPlot.Width * 1.05f;

			pboxPlot.Image = new Bitmap(pboxPlot.Width, pboxPlot.Height, PixelFormat.Format24bppRgb);

			using (Graphics g = Graphics.FromImage(pboxPlot.Image))
			{
				g.Clear(Color.WhiteSmoke);

				for (int i = 0; i < data.Data.Count - 1; i++)
				{
					g.DrawLine(Pens.DodgerBlue, zeroX + i * scaleX, zeroY + scaleY * (data.Data[i].NTPAccu), zeroX + i * scaleX, zeroY - scaleY * (data.Data[i].NTPAccu));

					g.DrawLine(Pens.ForestGreen, zeroX + i * scaleX, zeroY + scaleY * (data.Data[i].OccuRecAccu - data.Data[i].OccuRecErr), zeroX + i * scaleX, zeroY + scaleY * (data.Data[i].OccuRecAccu + data.Data[i].OccuRecErr));

					g.DrawLine(Pens.Red, zeroX + i * scaleX, zeroY + scaleY * (data.Data[i].WinAccu - data.Data[i].OccuRecErr), zeroX + i * scaleX, zeroY + scaleY * (data.Data[i].WinAccu + data.Data[i].OccuRecErr));
				}

				for (int i = -4000; i < 4000; i+=50)
				{
					g.DrawLine(Pens.Gray, zeroX, zeroY + scaleY * i, maxX, zeroY + scaleY * i);
				}
			}
		}

		private void frmPlotClockData_Resize(object sender, EventArgs e)
		{
			PlotData(m_Data);
		}

		private void btnRePlot_Click(object sender, EventArgs e)
		{
			PlotData(m_Data);
		}
	}

	public static class ListExtensions
	{
		public static float MaxNth<TSource>(this List<TSource> list, Func<TSource, float> selector, int n)
		{
			if (list.Count < n)
				return list.Min(selector);

			var copy = new List<float>();
			copy.AddRange(list.Select(selector));
			copy.Sort();

			if (n < copy.Count)
				return copy[copy.Count - 1 - n];
			else
				return copy[0];
		}

		public static float MinNth<TSource>(this List<TSource> list, Func<TSource, float> selector, int n)
		{
			if (list.Count < n)
				return list.Max(selector);

			var copy = new List<float>();
			copy.AddRange(list.Select(selector));
			copy.Sort();

			if (n < copy.Count)
				return copy[n];
			else
				return copy[copy.Count - 1];
		}
	}
}

