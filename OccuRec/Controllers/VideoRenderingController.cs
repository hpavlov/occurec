using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using OccuRec.Drivers;
using OccuRec.FrameAnalysis;
using OccuRec.Helpers;
using OccuRec.Properties;
using OccuRec.StateManagement;
using OccuRec.Tracking;
using OccuRec.Utilities;

namespace OccuRec.Controllers
{
    public class VideoRenderingController : IDisposable
    {
        private frmMain m_MainForm;

        private bool running = true;
        private bool previewOn = true;

        private long lastDisplayedVideoFrameNumber = -1;

        private ICameraImage cameraImage;

        private delegate void PaintVideoFrameDelegate(VideoFrameWrapper frame, Bitmap bmp);

        private int imageWidth;
        private int imageHeight;
        private VideoWrapper videoObject;
        private CameraStateManager stateManager;
	    private FrameAnalysisManager analysisManager;

		public VideoRenderingController(frmMain mainForm, CameraStateManager stateManager, FrameAnalysisManager analysisManager)
        {
            m_MainForm = mainForm;
            this.stateManager = stateManager;
			this.analysisManager = analysisManager;

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

        private MinimalVideoFrame m_LastRenderedFrame = null;

        private bool m_VideoFrameCopyRequested = false;
        private ManualResetEvent m_GetNextVideoFrameSignal = new ManualResetEvent(true);

        internal IVideoFrame GetCurrentFrame()
        {
            m_GetNextVideoFrameSignal.Reset();
            m_LastRenderedFrame = null;
            m_VideoFrameCopyRequested = true;

            m_GetNextVideoFrameSignal.WaitOne();

		    return m_LastRenderedFrame;
        }

	    internal int Width
	    {
			get { return imageWidth; }
	    }

		internal int Height
		{
			get { return imageHeight; }
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
                        IVideoFrame frame = videoObject.LastVideoFrame;

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
										frame.ImageArray,
                                        imageWidth,
                                        imageHeight,
                                        videoObject.SensorType);

                                    bmp = cameraImage.GetDisplayBitmap();
                                }

								analysisManager.ProcessFrame(frameWrapper, bmp);

                                try
                                {
                                    m_MainForm.Invoke(new PaintVideoFrameDelegate(PaintVideoFrameCallback), new object[] {frameWrapper, bmp});
                                }
                                catch (InvalidOperationException)
                                { }
                                catch (IndexOutOfRangeException)
                                { }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex.GetFullStackTrace());

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
                        { }
                        catch (IndexOutOfRangeException)
                        { }
                    }

                }

                Thread.Sleep(1);
                Application.DoEvents();
            }
        }

        private void PaintVideoFrameCallback(VideoFrameWrapper frame, Bitmap bmp)
        {
            if (m_VideoFrameCopyRequested)
            {
                try
                {
                    m_LastRenderedFrame = new MinimalVideoFrame(frame, new Bitmap(bmp));
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.GetFullStackTrace());
                }
                finally
                {
                    m_VideoFrameCopyRequested = false;
                    m_GetNextVideoFrameSignal.Set();
                }
            }

            m_MainForm.PaintVideoFrame(frame, bmp);
        }

        public void Dispose()
        {
            running = false;
	        analysisManager.Dispose();
        }
    }
}
