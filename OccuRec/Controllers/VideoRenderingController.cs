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
using OccuRec.StateManagement;

namespace OccuRec.Controllers
{
    public class VideoRenderingController : IDisposable
    {
        private frmMain m_MainForm;

        private bool useVariantPixels;

        private bool running = true;
        private bool previewOn = true;

        private long lastDisplayedVideoFrameNumber = -1;

        private ICameraImage cameraImage;

        private delegate void PaintVideoFrameDelegate(VideoFrameWrapper frame, Bitmap bmp);

        private int imageWidth;
        private int imageHeight;
        private VideoWrapper videoObject;
        private CameraStateManager stateManager;

        public VideoRenderingController(frmMain mainForm, CameraStateManager stateManager)
        {
            m_MainForm = mainForm;
            this.stateManager = stateManager;

            running = true;
            previewOn = true;

            cameraImage = new CameraImage();

            ThreadPool.QueueUserWorkItem(new WaitCallback(DisplayVideoFrames));
        }

        internal VideoWrapper ConnectToDriver(IVideo driverInstance)
        {
            videoObject = new VideoWrapper(driverInstance, m_MainForm);

            videoObject.Connected = true;

            if (videoObject.Connected)
            {
                imageWidth = videoObject.Width;
                imageHeight = videoObject.Height;
            }

            return videoObject;
        }

        internal IVideoFrame GetCurrentFrame()
        {
            // TODO: Provide a thread safe 'cached' version of the current IVideoFrame
            throw new NotImplementedException();
        }

        private void DisplayVideoFrames(object state)
        {
            while (running)
            {
                if (videoObject != null &&
                    videoObject.IsConnected &&
                    previewOn)
                {
                    try
                    {
                        IVideoFrame frame = useVariantPixels
                                ? videoObject.LastVideoFrameVariant
                                : videoObject.LastVideoFrame;

                        if (frame != null)
                        {

                            var frameWrapper = new VideoFrameWrapper(frame);

                            if (frameWrapper.UniqueFrameId == -1 || frameWrapper.UniqueFrameId != lastDisplayedVideoFrameNumber)
                            {
                                stateManager.ProcessFrame(frameWrapper);

                                lastDisplayedVideoFrameNumber = frameWrapper.UniqueFrameId;

                                Bitmap bmp = frame.PreviewBitmap;

                                if (bmp == null)
                                {
                                    cameraImage.SetImageArray(
                                        useVariantPixels
                                            ? frame.ImageArrayVariant
                                            : frame.ImageArray,
                                        imageWidth,
                                        imageHeight,
                                        videoObject.SensorType);

                                    bmp = cameraImage.GetDisplayBitmap();
                                }

                                m_MainForm.Invoke(new PaintVideoFrameDelegate(m_MainForm.PaintVideoFrame), new object[] { frameWrapper, bmp });
                            }
                        }
                    }
                    catch (InvalidOperationException) { }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex);

                        Bitmap errorBmp = new Bitmap(m_MainForm.picVideoFrame.Width, m_MainForm.picVideoFrame.Height);
                        using (Graphics g = Graphics.FromImage(errorBmp))
                        {
                            g.Clear(Color.MidnightBlue);
                            //g.DrawString(ex.Message, debugTextFont, Brushes.Black, 10, 10);
                            g.Save();
                        }
                        try
                        {
                            m_MainForm.Invoke(new PaintVideoFrameDelegate(m_MainForm.PaintVideoFrame), new object[] { null, errorBmp });
                        }
                        catch (InvalidOperationException)
                        {
                            // InvalidOperationException could be thrown when closing down the app i.e. when the form has been already disposed
                        }
                    }

                }

                Thread.Sleep(1);
                Application.DoEvents();
            }
        }

        public void Dispose()
        {
            running = false;
        }
    }
}
