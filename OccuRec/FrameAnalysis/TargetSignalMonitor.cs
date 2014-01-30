using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using OccuRec.Helpers;
using OccuRec.Tracking;

namespace OccuRec.FrameAnalysis
{
	internal class TargetMeasurement
	{
		public float NormalizedMeasurement;
		public long FrameNumber;
	}

	public class TargetSignalMonitor
	{
		private List<TargetMeasurement> m_AllMeasurements = new List<TargetMeasurement>();
		private object m_SyncLock = new object();
		private Bitmap m_BufferImage;

		public TargetSignalMonitor()
		{
			m_BufferImage = new Bitmap(204, 54, PixelFormat.Format32bppRgb);
		}

		public void ProcessFrame(VideoFrameWrapper frame)
		{
			if (TrackingContext.Current.TargetStar != null && 
				TrackingContext.Current.GuidingStar != null && 
				TrackingContext.Current.TargetStar.IsLocated && 
				TrackingContext.Current.GuidingStar.IsLocated)
			{
				float normalizedMeasurement = TrackingContext.Current.TargetStar.Measurement / TrackingContext.Current.GuidingStar.Measurement;

				while (m_AllMeasurements.Count > 200)
				{
					m_AllMeasurements.RemoveAt(0);
				}

				var mea = new TargetMeasurement()
				{
					NormalizedMeasurement = normalizedMeasurement,
					FrameNumber = frame.IntegratedFrameNo
				};

				m_AllMeasurements.Add(mea);
			}
		}

		public void DisplayData(Graphics g, int imageWidth, int imageHeight)
		{
			if (m_AllMeasurements.Count > 2)
			{
				lock (m_SyncLock)
				{
					float max = m_AllMeasurements.Max(x => x.NormalizedMeasurement);
					float min = m_AllMeasurements.Min(x => x.NormalizedMeasurement);

					float mid = (min + max) / 2;

					if (2 * 0.86105f * mid > (max - min))
					{
						// NOTE: The scale must be the bigger of 2 stellar magnitudes for the full height *OR* the max difference
						max = mid + 2 * 0.4305f * mid;
						min = mid - 2 * 0.4305f * mid;
					}

					float scaleY = 50.0f / (max - min);					

					using (Graphics gBuff = Graphics.FromImage(m_BufferImage))
					{
						gBuff.Clear(Color.DimGray);
						gBuff.DrawRectangle(Pens.Blue, 0, 0, 203, 53);

						if (!float.IsNaN(scaleY) && !float.IsInfinity(scaleY))
						{
							for (int i = 0; i < m_AllMeasurements.Count - 1; i++)
							{
								float x1 = 2 + i;
								float y1 = (52 - (m_AllMeasurements[i].NormalizedMeasurement - min) * scaleY);
								float x2 = 2 + i + 1;
								float y2 = (52 - (m_AllMeasurements[i + 1].NormalizedMeasurement - min) * scaleY);

								gBuff.DrawLine(Pens.Turquoise, x1, y1, x2, y2);
							}
						}

						gBuff.Save();
					}

					g.DrawImage(m_BufferImage, imageWidth - 209, 5);
					
				}
			}			
		}
	}
}
