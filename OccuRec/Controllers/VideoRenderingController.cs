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

namespace OccuRec.Controllers
{
    public class VideoRenderingController
    {
        private frmMain m_MainForm;

        public VideoRenderingController(frmMain mainForm)
        {
            
        }

        private void DisplayVideoFrames(object state)
        {
            //while (running)
            //{
            //    if (videoObject != null &&
            //        videoObject.IsConnected &&
            //        previewOn)
            //    {
            //        try
            //        {
            //            IVideoFrame frame = useVariantPixels
            //                    ? videoObject.LastVideoFrameVariant
            //                    : videoObject.LastVideoFrame;

            //            if (frame != null)
            //            {

            //                var frameWrapper = new VideoFrameWrapper(frame);

            //                if (frameWrapper.UniqueFrameId == -1 || frameWrapper.UniqueFrameId != lastDisplayedVideoFrameNumber)
            //                {
            //                    stateManager.ProcessFrame(frameWrapper);

            //                    lastDisplayedVideoFrameNumber = frameWrapper.UniqueFrameId;

            //                    Bitmap bmp = frame.PreviewBitmap;

            //                    if (bmp == null)
            //                    {
            //                        cameraImage.SetImageArray(
            //                            useVariantPixels
            //                                ? frame.ImageArrayVariant
            //                                : frame.ImageArray,
            //                            imageWidth,
            //                            imageHeight,
            //                            videoObject.SensorType);

            //                        bmp = cameraImage.GetDisplayBitmap();
            //                    }

            //                    Invoke(new frmMain.PaintVideoFrameDelegate(PaintVideoFrame), new object[] { frameWrapper, bmp });
            //                }
            //            }
            //        }
            //        catch (InvalidOperationException) { }
            //        catch (Exception ex)
            //        {
            //            Trace.WriteLine(ex);

            //            Bitmap errorBmp = new Bitmap(picVideoFrame.Width, picVideoFrame.Height);
            //            using (Graphics g = Graphics.FromImage(errorBmp))
            //            {
            //                g.Clear(Color.MidnightBlue);
            //                //g.DrawString(ex.Message, debugTextFont, Brushes.Black, 10, 10);
            //                g.Save();
            //            }
            //            try
            //            {
            //                Invoke(new frmMain.PaintVideoFrameDelegate(PaintVideoFrame), new object[] { null, errorBmp });
            //            }
            //            catch (InvalidOperationException)
            //            {
            //                // InvalidOperationException could be thrown when closing down the app i.e. when the form has been already disposed
            //            }
            //        }

            //    }

            //    Thread.Sleep(1);
            //    Application.DoEvents();
            //}
        }
    }
}
