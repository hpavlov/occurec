using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OccuRec.Drivers;

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
            m_MainForm.picVideoFrame.MouseClick +=picVideoFrame_MouseClick;
        }

        void picVideoFrame_MouseClick(object sender, MouseEventArgs e)
        {
             if (e.Button != MouseButtons.Left)
             {
                 ChangeVideoFrameInteractiveState(VideoFrameInteractiveState.None);
             }
             else
             {
                 if (m_VideoFrameInteractiveState == VideoFrameInteractiveState.SelectingGuidingStar)
                 {
                     if (SelectGuidingStar(e.Location))
                         ChangeVideoFrameInteractiveState(VideoFrameInteractiveState.None);
                 }
                 else if (m_VideoFrameInteractiveState == VideoFrameInteractiveState.SelectingtTargetStar)
                 {
                     if (SelectingtTargetStar(e.Location))
                         ChangeVideoFrameInteractiveState(VideoFrameInteractiveState.None);
                 }
             }
        }

        private bool SelectGuidingStar(Point location)
        {
            IVideoFrame currentVideoFrame = m_VideoRenderingController.GetCurrentFrame();
            if (currentVideoFrame != null)
            {
                // TODO: Find the object at the location and set it as a guiding star
                // TODO: Inform the overlay manager that there is a new guiding star to display as overlay
                return true;
            }

            return false;
        }

        private bool SelectingtTargetStar(Point location)
        {
            IVideoFrame currentVideoFrame = m_VideoRenderingController.GetCurrentFrame();
            if (currentVideoFrame != null)
            {
                // TODO: Find the object at the location and set it as a guiding star
                // TODO: Inform the overlay manager that there is a new target star to display as overlay

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
