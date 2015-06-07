/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using OccuRec.Drivers;
using OccuRec.Helpers;
using OccuRec.Properties;
using OccuRec.Tracking;
using OccuRec.Utilities;

namespace OccuRec.Controllers
{
    public class VideoFrameInteractionController
    {
        internal enum VideoFrameInteractiveState
        {
            None,
            SelectingGuidingStar,
            SelectingTargetStar,
            SelectingStarSpectra
        }

        private frmMain m_MainForm;
        private VideoRenderingController m_VideoRenderingController;

        private VideoFrameInteractiveState m_VideoFrameInteractiveState = VideoFrameInteractiveState.None;

        public VideoFrameInteractionController(frmMain mainForm, VideoRenderingController videoRenderingController)
        {
            m_VideoRenderingController = videoRenderingController;

            m_MainForm = mainForm;
            m_MainForm.picVideoFrame.MouseDown +=picVideoFrame_MouseClick;

			TrackingContext.Current.Reset();
        }

        void picVideoFrame_MouseClick(object sender, MouseEventArgs e)
        {
			bool shiftHeld = (Control.ModifierKeys & Keys.Shift) == Keys.Shift;
			bool controlHeld = (Control.ModifierKeys & Keys.Control) == Keys.Control;

            if (e.Button != MouseButtons.Left)
            {
                ChangeVideoFrameInteractiveState(VideoFrameInteractiveState.None);
            }
            else
            {
                ThreadPool.QueueUserWorkItem(SetupSelectedObjectInNonUIThread, new Tuple<Point, bool, bool>(e.Location, shiftHeld, controlHeld));
            }
        }

	    private int m_Bpp;

		internal void OnNewVideoSource(VideoWrapper videWrapper)
		{
			m_Bpp = int.Parse(videWrapper.CameraBitDepth);
		}

        private void SetupSelectedObjectInNonUIThread(object state)
        {
            // We need to do this on a non UI thread so we can block the thread waiting for the current image to be provided
            try
            {
                var typedState = (Tuple<Point, bool, bool>)state;

                if (m_VideoFrameInteractiveState == VideoFrameInteractiveState.SelectingGuidingStar)
                {
	                if (SelectGuidingStar(typedState.Item1, typedState.Item2, typedState.Item3))
		                m_MainForm.Invoke(new Action(() => ChangeVideoFrameInteractiveState(VideoFrameInteractiveState.None)));

                }
                else if (m_VideoFrameInteractiveState == VideoFrameInteractiveState.SelectingTargetStar)
                {
                    if (SelectingTargetStar(typedState.Item1, typedState.Item2, typedState.Item3))
						m_MainForm.Invoke(new Action(() => ChangeVideoFrameInteractiveState(VideoFrameInteractiveState.None)));
                }
                else if (m_VideoFrameInteractiveState == VideoFrameInteractiveState.SelectingStarSpectra)
                {
                    if (SelectingStarSpectra(typedState.Item1, typedState.Item2, typedState.Item3))
                        m_MainForm.Invoke(new Action(() => ChangeVideoFrameInteractiveState(VideoFrameInteractiveState.None)));
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.GetFullStackTrace());
            }
        }

        private bool SelectGuidingStar(Point location, bool shiftHeld, bool controlHeld)
        {
            IVideoFrame currentVideoFrame = m_VideoRenderingController.GetCurrentFrame();
            if (currentVideoFrame != null)
            {
				// Find the object at the location and set it as a guiding star                
				var astroImg = new AstroImage(currentVideoFrame, m_VideoRenderingController.Width, m_VideoRenderingController.Height);
	            uint[,] areaPixels = astroImg.GetMeasurableAreaPixels(location.X, location.Y);

	            PSFFit psfFit = new PSFFit(location.X, location.Y);
				psfFit.Fit(areaPixels);
				if (psfFit.IsSolved && psfFit.Certainty > Settings.Default.TrackingMinGuidingCertainty)
				{
					TrackingContext.Current.GuidingStar = new LastTrackedPosition(m_Bpp)
					{
						FWHM = (float)psfFit.FWHM,
						X = (float)psfFit.XCenter,
						Y = (float)psfFit.YCenter,
						IsFixed = false
					};

					TrackingContext.Current.ReConfigureNativeTracking(m_VideoRenderingController.Width, m_VideoRenderingController.Height);
				    TrackingContext.Current.SpectraAngleDeg = float.NaN;

					return true;
				}
				else
				{
					// NOTE: Too faint to be used as a guiding star
					MessageBox.Show(m_MainForm, "This object is not bright enought for a Guiding star.", "OccuRec", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
				
            }

            return false;
        }

		private bool SelectingTargetStar(Point location, bool shiftHeld, bool controlHeld)
        {
			IVideoFrame currentVideoFrame = m_VideoRenderingController.GetCurrentFrame();
			if (currentVideoFrame != null)
			{      
				var astroImg = new AstroImage(currentVideoFrame, m_VideoRenderingController.Width, m_VideoRenderingController.Height);
				uint[,] areaPixels = astroImg.GetMeasurableAreaPixels(location.X, location.Y);

				PSFFit psfFit = new PSFFit(location.X, location.Y);
				psfFit.Fit(areaPixels);

				TrackingContext.Current.TargetStar = new LastTrackedPosition(m_Bpp)
				{
					FWHM = (float)psfFit.FWHM,
					X = (float)psfFit.XCenter,
					Y = (float)psfFit.YCenter
				};

				TrackingContext.Current.TargetStar.IsFixed = !psfFit.IsSolved || psfFit.Certainty < Settings.Default.TrackingMinForcedFixedObjCertainty || controlHeld;
			    TrackingContext.Current.TargetStar.IsFullDisapearance = shiftHeld;
				TrackingContext.Current.ReConfigureNativeTracking(m_VideoRenderingController.Width, m_VideoRenderingController.Height);

				return true;

			}

			return false;
        }

        private bool SelectingStarSpectra(Point location, bool shiftHeld, bool controlHeld)
        {
            IVideoFrame currentVideoFrame = m_VideoRenderingController.GetCurrentFrame();
            if (currentVideoFrame != null)
            {
                var astroImg = new AstroImage(currentVideoFrame, m_VideoRenderingController.Width, m_VideoRenderingController.Height, currentVideoFrame.MaxSignalValue);
                uint[,] areaPixels = astroImg.GetMeasurableAreaPixels(location.X, location.Y);

                PSFFit psfFit = new PSFFit(location.X, location.Y);
                psfFit.Fit(areaPixels);

                if (psfFit.IsSolved && psfFit.Certainty > Settings.Default.TrackingMinGuidingCertainty)
                {
                    float angle = LocateSpectraAngle(psfFit, astroImg);

                    if (!float.IsNaN(angle))
                    {
                        TrackingContext.Current.GuidingStar = new LastTrackedPosition(m_Bpp)
                        {
                            FWHM = (float)psfFit.FWHM,
                            X = (float)psfFit.XCenter,
                            Y = (float)psfFit.YCenter,
                            IsFixed = false
                        };

                        TrackingContext.Current.ReConfigureNativeTracking(m_VideoRenderingController.Width, m_VideoRenderingController.Height);
                        TrackingContext.Current.SpectraAngleDeg = angle;

                        return true;
                    }
                }
            }

            return false;
        }

        internal float LocateSpectraAngle(PSFFit selectedStar, AstroImage image)
        {
            float x0 = (float)selectedStar.XCenter;
            float y0 = (float)selectedStar.YCenter;
            uint brigthness20 = (uint)(selectedStar.Brightness / 5);
            uint bgFromPsf = (uint)(selectedStar.I0);

            int minDistance = (int)(10 * selectedStar.FWHM);
            int clearDist = (int)(2 * selectedStar.FWHM);

            float width = image.Width;
            float height = image.Height;

            uint[] angles = new uint[360];
            uint[] sums = new uint[360];
            uint[] pixAbove50Perc = new uint[360];

            int diagonnalPixels = (int)Math.Ceiling(Math.Sqrt(image.Width * image.Width + image.Height * image.Height));
            for (int i = 0; i < 360; i++)
            {
                var mapper = new RotationMapper(image.Width, image.Height, i);
                PointF p1 = mapper.GetDestCoords(x0, y0);
                float x1 = p1.X;
                float y1 = p1.Y;

                uint rowSum = 0;
                uint pixAbove50 = 0;
                uint pixAbove50Max = 0;
                bool prevPixAbove50 = false;

                for (int d = minDistance; d < diagonnalPixels; d++)
                {
                    PointF p = mapper.GetSourceCoords(x1 + d, y1);

                    if (p.X >= 0 && p.X < width && p.Y >= 0 && p.Y < height)
                    {
                        uint value = (uint)image.GetPixel((int)p.X, (int)p.Y);
                        rowSum += value;
                        PointF pu = mapper.GetSourceCoords(x1 + d, y1 + clearDist);
                        PointF pd = mapper.GetSourceCoords(x1 + d, y1 - clearDist);
                        if (pu.X >= 0 && pu.X < width && pu.Y >= 0 && pu.Y < height &&
                            pd.X >= 0 && pd.X < width && pd.Y >= 0 && pd.Y < height)
                        {
                            uint value_u = (uint)image.GetPixel((int)pu.X, (int)pu.Y);
                            uint value_d = (uint)image.GetPixel((int)pd.X, (int)pd.Y);
                            if ((value - bgFromPsf) > brigthness20 && value > value_u && value > value_d)
                            {
                                if (prevPixAbove50) pixAbove50++;
                                prevPixAbove50 = true;
                            }
                            else
                            {
                                prevPixAbove50 = false;
                                if (pixAbove50Max < pixAbove50) pixAbove50Max = pixAbove50;
                                pixAbove50 = 0;
                            }
                        }
                        else
                        {
                            prevPixAbove50 = false;
                            if (pixAbove50Max < pixAbove50) pixAbove50Max = pixAbove50;
                            pixAbove50 = 0;
                        }
                    }
                }

                angles[i] = (uint)i;
                sums[i] = rowSum;
                pixAbove50Perc[i] = pixAbove50Max;
            }

            Array.Sort(pixAbove50Perc, angles);

            uint roughAngle = angles[359];

            if (pixAbove50Perc[358] * 2 > pixAbove50Perc[359])
                return float.NaN;

            uint bestSum = 0;
            float bestAngle = 0f;

            for (float a = roughAngle - 1; a < roughAngle + 1; a += 0.02f)
            {
                var mapper = new RotationMapper(image.Width, image.Height, a);
                PointF p1 = mapper.GetDestCoords(x0, y0);
                float x1 = p1.X;
                float y1 = p1.Y;

                uint rowSum = 0;

                for (int d = minDistance; d < diagonnalPixels; d++)
                {
                    PointF p = mapper.GetSourceCoords(x1 + d, y1);

                    if (p.X >= 0 && p.X < width && p.Y >= 0 && p.Y < height)
                    {
                        uint pixVal = (uint)image.GetPixel((int)p.X, (int)p.Y);
                        rowSum += pixVal;
                    }
                }

                if (rowSum > bestSum)
                {
                    bestSum = rowSum;
                    bestAngle = a;
                }
            }

            return bestAngle;
        }

        private void ChangeVideoFrameInteractiveState(VideoFrameInteractiveState newState)
        {
            if (newState == VideoFrameInteractiveState.None)
            {
                m_MainForm.picVideoFrame.Cursor = Cursors.Default;
                m_MainForm.tbsAddTarget.Checked = false;
                m_MainForm.tsbInsertSpectra.Checked = false;
                m_MainForm.tsbAddGuidingStar.Checked = false;
            }
            else if (newState == VideoFrameInteractiveState.SelectingTargetStar)
            {
                m_MainForm.picVideoFrame.Cursor = Cursors.Cross;
                m_MainForm.tbsAddTarget.Checked = true;
                m_MainForm.tsbInsertSpectra.Checked = false;
                m_MainForm.tsbAddGuidingStar.Checked = false;
            }
            else if (newState == VideoFrameInteractiveState.SelectingGuidingStar)
            {
                m_MainForm.picVideoFrame.Cursor = Cursors.Cross;
                m_MainForm.tbsAddTarget.Checked = false;
                m_MainForm.tsbInsertSpectra.Checked = false;
                m_MainForm.tsbAddGuidingStar.Checked = true;
            }
            else if (newState == VideoFrameInteractiveState.SelectingStarSpectra)
            {
                m_MainForm.picVideoFrame.Cursor = Cursors.Cross;
                m_MainForm.tbsAddTarget.Checked = false;
                m_MainForm.tsbAddGuidingStar.Checked = false;
                m_MainForm.tsbInsertSpectra.Checked = true;
            }


            m_VideoFrameInteractiveState = newState;
        }

        public void ToggleSelectGuidingStar()
        {
            if (m_VideoFrameInteractiveState == VideoFrameInteractiveState.SelectingGuidingStar)
                ChangeVideoFrameInteractiveState(VideoFrameInteractiveState.None);
            else
                ChangeVideoFrameInteractiveState(VideoFrameInteractiveState.SelectingGuidingStar);
        }

        public void ToggleSelectTargetStar()
        {
            if (m_VideoFrameInteractiveState == VideoFrameInteractiveState.SelectingTargetStar)
                ChangeVideoFrameInteractiveState(VideoFrameInteractiveState.None);
            else
                ChangeVideoFrameInteractiveState(VideoFrameInteractiveState.SelectingTargetStar);
        }

        public void ToggleInsertStarSpectra()
        {
           if (m_VideoFrameInteractiveState ==  VideoFrameInteractiveState.SelectingStarSpectra)
               ChangeVideoFrameInteractiveState(VideoFrameInteractiveState.None);
           else
               ChangeVideoFrameInteractiveState(VideoFrameInteractiveState.SelectingStarSpectra);
        }

		public void RemoveTrackedObjects()
		{
			TrackingContext.Current.TargetStar = null;
			TrackingContext.Current.GuidingStar = null;
			ChangeVideoFrameInteractiveState(VideoFrameInteractiveState.None);
			TrackingContext.Current.ReConfigureNativeTracking(m_VideoRenderingController.Width, m_VideoRenderingController.Height);			
		}
    }
}
