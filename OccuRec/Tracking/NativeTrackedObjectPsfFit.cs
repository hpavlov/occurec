using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using OccuRec.Properties;

namespace OccuRec.Tracking
{
	internal class NativeTrackedObjectPsfFit : ITrackedObjectPsfFit
	{
		private float m_R0;
		private float m_R02;
		private float m_IBackground;
		private float m_IStarMax;
		private bool m_IsAsymmetric;
		private uint m_Saturation;

		private double[] m_Residuals;

		public NativeTrackedObjectPsfFit(int bpp)
		{
			switch (bpp)
			{
				case 8:
					m_Saturation = Settings.Default.Saturation8Bit;
					break;

				case 12:
					m_Saturation = Settings.Default.Saturation12Bit;
					break;

				case 14:
					m_Saturation = Settings.Default.Saturation14Bit;
					break;

				default:
					m_Saturation = Settings.Default.Saturation8Bit;
					break;
			}
		}

		public void LoadFromNativePsfFitInfo(NativePsfFitInfo psfInfo, double[] residuals)
		{
			XCenter = psfInfo.XCenter;
			YCenter = psfInfo.YCenter;
			FWHM = psfInfo.FWHM;
			IMax = psfInfo.IMax;
			I0 = psfInfo.I0;
			X0 = psfInfo.X0;
			Y0 = psfInfo.Y0;
			MatrixSize = psfInfo.MatrixSize;
			IsSolved = psfInfo.IsSolved == 1;

			m_R0 = psfInfo.R0;
			m_R02 = psfInfo.R02;
			m_IsAsymmetric = psfInfo.IsAsymmetric == 1;

			m_IBackground = psfInfo.I0;
			m_IStarMax = psfInfo.IMax - psfInfo.I0;

			m_Residuals = residuals;
		}

		public double XCenter { get; private set; }

		public double YCenter { get; private set; }

		public double FWHM { get; private set; }

		public double IMax { get; private set; }

		public double I0 { get; private set; }

		public double X0 { get; private set; }

		public double Y0 { get; private set; }

		public int MatrixSize { get; private set; }

		public bool IsSolved { get; private set; }

		private double GetPSFValueInternal(double x, double y)
		{
			return m_IBackground + m_IStarMax * Math.Exp(-((x - X0) * (x - X0) + (y - Y0) * (y - Y0)) / (m_R0 * m_R0));
		}

		private double GetPSFValueInternalAsymetric(double x, double y)
		{
			return m_IBackground + m_IStarMax * Math.Exp(-(x - X0) * (x - X0) / (m_R0 * m_R0) + (y - Y0) * (y - Y0) / (m_R02 * m_R02));
		}

		public double GetPSFValueAt(double x, double y)
		{
			return m_IsAsymmetric
				? GetPSFValueInternalAsymetric(x, y)
				: GetPSFValueInternal(x, y);
		}

		public double GetResidualAt(int x, int y)
		{
			return m_Residuals[y * MatrixSize + x];
		}

		public void DrawDataPixels(System.Drawing.Graphics g, System.Drawing.Rectangle rect, float aperture, System.Drawing.Pen aperturePen, int bpp)
		{
			DrawDataPixels(g, rect, aperture, aperturePen, bpp, this);
		}

		public void DrawGraph(System.Drawing.Graphics g, System.Drawing.Rectangle rect, int bpp)
		{
			DrawGraph(g, rect, bpp, this, m_Saturation);
		}

		public static void DrawGraph(Graphics g, Rectangle rect, int bpp, ITrackedObjectPsfFit trackedPsf, uint saturation)
		{
			float margin = 6.0f;
			double maxZ = 256;

			if (bpp == 14) maxZ = 16384;
			else if (bpp == 12) maxZ = 4096;

			int totalSteps = 100;

			float halfWidth = (float)trackedPsf.MatrixSize / 2.0f;
			float step = (float)(trackedPsf.MatrixSize * 1.0 / totalSteps);

			float yScale = (float)((rect.Height * 1.0 - 2 * margin) / (maxZ - trackedPsf.I0));
			float xScale = (float)(rect.Width * 1.0 - 2 * margin) / (halfWidth * 2);

			float xPrev = float.NaN;
			float yPrev = float.NaN;

			Brush bgBrush = SystemBrushes.ControlDarkDark;
			Brush includedPointsBrush = Brushes.Yellow;
			Brush excludedPointBrush = Brushes.Gray;

			g.FillRectangle(bgBrush, rect);

			if (!trackedPsf.IsSolved)
				return;

			for (int x = 0; x < trackedPsf.MatrixSize; x++)
				for (int y = 0; y < trackedPsf.MatrixSize; y++)
				{
					double z0 = trackedPsf.GetPSFValueAt(x, y);
					double z = z0 + trackedPsf.GetResidualAt(x, y);
					double d = Math.Sqrt((x - trackedPsf.X0) * (x - trackedPsf.X0) + (y - trackedPsf.Y0) * (y - trackedPsf.Y0));

					int sign = Math.Sign(x - trackedPsf.X0); if (sign == 0) sign = 1;

					float xVal = (float)(margin + (halfWidth + sign * d) * xScale);
					float yVal = rect.Height - margin - (float)(z - trackedPsf.I0) * yScale;

					if (xVal >= 0 && xVal <= rect.Width && yVal >= 0 && yVal <= rect.Height)
					{
						Brush pointBrush = z >= saturation ? excludedPointBrush : includedPointsBrush;
						g.FillRectangle(pointBrush, rect.Left + xVal - 1, rect.Top + yVal - 1, 3, 3);
					}
				}

			for (float width = -halfWidth; width < halfWidth; width += step)
			{
				float z = (float)trackedPsf.GetPSFValueAt(width + trackedPsf.X0, trackedPsf.Y0);
				float x = margin + (width + halfWidth) * xScale;
				float y = rect.Height - margin - (z - (float)trackedPsf.I0) * yScale;

				if (!float.IsNaN(xPrev) &&
					y > margin && yPrev > margin &&
					y < rect.Height && yPrev < rect.Height)
				{
					g.DrawLine(Pens.LimeGreen, rect.Left + xPrev, rect.Top + yPrev, rect.Left + x, rect.Top + y);
				}

				xPrev = x;
				yPrev = y;
			}
		}

		public static void DrawDataPixels(Graphics g, Rectangle rect, float aperture, Pen aperturePen, int bpp, ITrackedObjectPsfFit trackedPsf)
		{
			try
			{
				if (rect.Width != rect.Height) return;

				int coeff = rect.Width / trackedPsf.MatrixSize;
				if (coeff == 0) return;

				int size = trackedPsf.MatrixSize;

				for (int x = 0; x < size; x++)
				{
					for (int y = 0; y < size; y++)
					{
						double z = Math.Round(trackedPsf.GetPSFValueAt(x, y) + trackedPsf.GetResidualAt(x, y));

						if (bpp == 14)
							z = z * 255.0f / 16383;
						else if (bpp == 12)
							z = z * 255.0f / 4095;

						byte val = (byte)(Math.Max(0, Math.Min(255, z)));
						g.FillRectangle(AllGrayBrushes.GrayBrush(val), rect.Left + x * coeff, rect.Top + y * coeff, coeff, coeff);
					}
				}

				if (aperture > 0)
					g.DrawEllipse(
						aperturePen,
						rect.Left + ((float)trackedPsf.X0 - aperture + 0.5f) * coeff,
						rect.Top + ((float)trackedPsf.Y0 - aperture + 0.5f) * coeff,
						aperture * 2 * coeff, aperture * 2 * coeff);
				else if (aperture < 0)
				{
					g.DrawLine(aperturePen, rect.Left + ((float)trackedPsf.X0 - 3 * aperture + 0.5f) * coeff, rect.Top + (float)(trackedPsf.Y0 + 0.5f) * coeff, rect.Left + ((float)trackedPsf.X0 - aperture + 0.5f) * coeff, rect.Top + (float)(trackedPsf.Y0 + 0.5f) * coeff);
					g.DrawLine(aperturePen, rect.Left + ((float)trackedPsf.X0 + 3 * aperture + 0.5f) * coeff, rect.Top + (float)(trackedPsf.Y0 + 0.5f) * coeff, rect.Left + ((float)trackedPsf.X0 + aperture + 0.5f) * coeff, rect.Top + (float)(trackedPsf.Y0 + 0.5f) * coeff);
					g.DrawLine(aperturePen, rect.Left + (float)(trackedPsf.X0 + 0.5f) * coeff, rect.Top + ((float)trackedPsf.Y0 - 3 * aperture + 0.5f) * coeff, rect.Left + (float)(trackedPsf.X0 + 0.5f) * coeff, rect.Top + ((float)trackedPsf.Y0 - aperture + 0.5f) * coeff);
					g.DrawLine(aperturePen, rect.Left + (float)(trackedPsf.X0 + 0.5f) * coeff, rect.Top + ((float)trackedPsf.Y0 + 3 * aperture + 0.5f) * coeff, rect.Left + (float)(trackedPsf.X0 + 0.5f) * coeff, rect.Top + ((float)trackedPsf.Y0 + aperture + 0.5f) * coeff);
				}
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex.ToString());
			}
		}

	}
}
