using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using OccuRec.ASCOM;
using OccuRec.Helpers;
using OccuRec.Properties;
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

		private Bitmap m_PsfBitmap;
		private Rectangle m_PsfBitmapRect;

		private IObservatoryController m_ObservatoryController;
		private AutoFocusingManager m_AutoFocusingManager;

		private bool m_AutoPulseGuiding = false;

		internal TargetSignalMonitor(IObservatoryController observatoryController, AutoFocusingManager autoFocusingManager)
		{
			m_BufferImage = new Bitmap(204, 54, PixelFormat.Format32bppRgb);

			m_PsfBitmap = new Bitmap(100, 100, PixelFormat.Format32bppRgb);
			m_PsfBitmapRect = new Rectangle(0, 0, m_PsfBitmap.Width, m_PsfBitmap.Height);

			m_ObservatoryController = observatoryController;
			m_AutoFocusingManager = autoFocusingManager;
		}

		public void ProcessFrame(VideoFrameWrapper frame)
		{
			if (TrackingContext.Current.TargetStar != null && 
				TrackingContext.Current.GuidingStar != null)
			{
				if (TrackingContext.Current.TargetStar.IsLocated &&
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

				if (TrackingContext.Current.GuidingStar.IsLocated)
				{
					if (m_AutoPulseGuiding)
					{
						// TODO: Check if a correction needs to be made in the current thread
						// TODO: Issue any pulse guiding commands on a seaprate thread (or asynchronously)

						// TODO: There will ne an issue with the orientation of the video camera field
						//       Will need to find the correct directions (would have found the correct directions) this current session
						//       Probably a button on the Telescope Control screen??
					}

					m_AutoFocusingManager.ProcessFrame(frame, TrackingContext.Current.GuidingStar);
				}
			}
			else if (m_AllMeasurements.Count > 0)
			{
				m_AllMeasurements.Clear();
			}
		}

		public void DisplayData(Graphics g, int imageWidth, int imageHeight)
		{
            if (Settings.Default.OverlayDrawTargetLightCurve && 
                m_AllMeasurements.Count > 2)
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
                        gBuff.Clear(SystemColors.ControlDarkDark);
						gBuff.DrawRectangle(Pens.Blue, 0, 0, 203, 53);

						if (!float.IsNaN(scaleY) && !float.IsInfinity(scaleY))
						{
							for (int i = 0; i < m_AllMeasurements.Count - 1; i++)
							{
								float x1 = 2 + i;
								float y1 = (52 - (m_AllMeasurements[i].NormalizedMeasurement - min) * scaleY);
								float x2 = 2 + i + 1;
								float y2 = (52 - (m_AllMeasurements[i + 1].NormalizedMeasurement - min) * scaleY);

								if (!float.IsNaN(y1) && !float.IsNaN(y2) && !float.IsInfinity(y1) && !float.IsInfinity(y2))
									gBuff.DrawLine(Pens.Turquoise, x1, y1, x2, y2);
							}
						}

						gBuff.Save();
					}

					g.DrawImage(m_BufferImage, imageWidth - 209, 5);
					
				}
			}

            if (Settings.Default.OverlayDrawTargetStarFSP && 
                TrackingContext.Current.TargetStar != null &&
				TrackingContext.Current.TargetStar.IsLocated)
			{
				using (Graphics gPsf = Graphics.FromImage(m_PsfBitmap))
				{
					TrackingContext.Current.TargetStar.PsfFit.DrawGraph(gPsf, m_PsfBitmapRect, TrackingContext.Current.TargetStar.Bpp);
                    gPsf.DrawRectangle(Pens.Blue, 0, 0, m_PsfBitmap.Width - 1, m_PsfBitmap.Height - 1);
					gPsf.Save();
				}

				g.DrawImage(m_PsfBitmap, imageWidth - 5 - m_PsfBitmapRect.Width, 54 + 5 + 5);
			}

            if (Settings.Default.OverlayDrawGuidingStarFSP &&
                TrackingContext.Current.GuidingStar != null &&
                TrackingContext.Current.GuidingStar.IsLocated)
            {
                using (Graphics gPsf = Graphics.FromImage(m_PsfBitmap))
                {
                    TrackingContext.Current.GuidingStar.PsfFit.DrawGraph(gPsf, m_PsfBitmapRect, TrackingContext.Current.GuidingStar.Bpp);
                    gPsf.DrawRectangle(Pens.Blue, 0, 0, m_PsfBitmap.Width - 1, m_PsfBitmap.Height - 1);
                    gPsf.Save();
                }

                g.DrawImage(m_PsfBitmap, imageWidth - 5 - m_PsfBitmapRect.Width, 54 + 5 + 5 + 5 +  m_PsfBitmapRect.Height);
            }
		}

		public void ChangeAutoPulseGuiding(bool autoPulseGuiding)
		{
			m_AutoPulseGuiding = autoPulseGuiding;
		}
	}
}
