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
            SelectingtTargetStar
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

        private void SetupSelectedObjectInNonUIThread(object state)
        {
            // We need to do this on a non UI thread so we can block the thread waiting for the current image to be provided
            try
            {
                var typedState = (Tuple<Point, bool, bool>)state;

                if (m_VideoFrameInteractiveState == VideoFrameInteractiveState.SelectingGuidingStar)
                {
                    if (SelectGuidingStar(typedState.Item1, typedState.Item2, typedState.Item3))
                        ChangeVideoFrameInteractiveState(VideoFrameInteractiveState.None);
                }
                else if (m_VideoFrameInteractiveState == VideoFrameInteractiveState.SelectingtTargetStar)
                {
                    if (SelectingtTargetStar(typedState.Item1, typedState.Item2, typedState.Item3))
                        ChangeVideoFrameInteractiveState(VideoFrameInteractiveState.None);
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
					TrackingContext.Current.GuidingStar = new LastTrackedPosition()
					{
						FWHM = (float)psfFit.FWHM,
						X = (float)psfFit.XCenter,
						Y = (float)psfFit.YCenter,
						IsFixed = false
					};

					TrackingContext.Current.ReConfigureNativeTracking(m_VideoRenderingController.Width, m_VideoRenderingController.Height);

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

		private bool SelectingtTargetStar(Point location, bool shiftHeld, bool controlHeld)
        {
			IVideoFrame currentVideoFrame = m_VideoRenderingController.GetCurrentFrame();
			if (currentVideoFrame != null)
			{      
				var astroImg = new AstroImage(currentVideoFrame, m_VideoRenderingController.Width, m_VideoRenderingController.Height);
				uint[,] areaPixels = astroImg.GetMeasurableAreaPixels(location.X, location.Y);

				PSFFit psfFit = new PSFFit(location.X, location.Y);
				psfFit.Fit(areaPixels);

				TrackingContext.Current.TargetStar = new LastTrackedPosition()
				{
					FWHM = (float)psfFit.FWHM,
					X = (float)psfFit.XCenter,
					Y = (float)psfFit.YCenter
				};

				TrackingContext.Current.TargetStar.IsFixed = !psfFit.IsSolved || psfFit.Certainty < Settings.Default.TrackingMinForcedFixedObjCertainty || controlHeld;
				TrackingContext.Current.ReConfigureNativeTracking(m_VideoRenderingController.Width, m_VideoRenderingController.Height);

				return true;

			}

			return false;
        }

        private void ChangeVideoFrameInteractiveState(VideoFrameInteractiveState newState)
        {
            if (newState == VideoFrameInteractiveState.None)
            {
                m_MainForm.picVideoFrame.Cursor = Cursors.Default;
                m_MainForm.tbsAddTarget.Checked = false;
                m_MainForm.tsbAddGuidingStar.Checked = false;
            }
            else if (newState == VideoFrameInteractiveState.SelectingtTargetStar)
            {
                m_MainForm.picVideoFrame.Cursor = Cursors.Cross;
                m_MainForm.tbsAddTarget.Checked = true;
                m_MainForm.tsbAddGuidingStar.Checked = false;
            }
            else if (newState == VideoFrameInteractiveState.SelectingGuidingStar)
            {
                m_MainForm.picVideoFrame.Cursor = Cursors.Cross;
                m_MainForm.tbsAddTarget.Checked = false;
                m_MainForm.tsbAddGuidingStar.Checked = true;
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
            if (m_VideoFrameInteractiveState == VideoFrameInteractiveState.SelectingtTargetStar)
                ChangeVideoFrameInteractiveState(VideoFrameInteractiveState.None);
            else
                ChangeVideoFrameInteractiveState(VideoFrameInteractiveState.SelectingtTargetStar);
        }
    }
}
