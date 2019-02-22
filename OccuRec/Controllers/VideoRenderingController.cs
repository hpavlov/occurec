/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using OccuRec.Context;
using OccuRec.Drivers;
using OccuRec.FrameAnalysis;
using OccuRec.Helpers;
using OccuRec.Properties;
using OccuRec.StateManagement;
using OccuRec.Tracking;
using OccuRec.Utilities;

namespace OccuRec.Controllers
{
	public enum DisplayIntensifyMode
	{
		Off,
		Lo,
		Hi
	}

	
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

		private bool m_DisplayHueIntensityMode;
		private bool m_DisplayInvertedMode;
        private bool m_DisplaySaturationCheckMode;
		private DisplayIntensifyMode m_DisplayIntensifyMode;

		public VideoRenderingController(frmMain mainForm, CameraStateManager stateManager, FrameAnalysisManager analysisManager)
        {
            m_MainForm = mainForm;
            this.stateManager = stateManager;
			this.analysisManager = analysisManager;

            running = true;
            previewOn = true;

			m_DisplayIntensifyMode = Settings.Default.DisplayIntensifyMode;
			m_DisplayInvertedMode = Settings.Default.UseInvertedDisplayMode;
			m_DisplayHueIntensityMode = Settings.Default.UseHueIntensityDisplayMode;
            m_DisplaySaturationCheckMode = Settings.Default.UseSaturationCheckDisplayMode;

			m_MainForm.tsmiHueIntensity.Checked = m_DisplayHueIntensityMode;
			m_MainForm.tsmiInverted.Checked = m_DisplayInvertedMode;
            m_MainForm.tsmiSaturation.Checked = m_DisplaySaturationCheckMode;
			m_MainForm.tsmiOff.Checked = m_DisplayIntensifyMode == DisplayIntensifyMode.Off;
			m_MainForm.tsmiLo.Checked = m_DisplayIntensifyMode == DisplayIntensifyMode.Lo;
			m_MainForm.tsmiHigh.Checked = m_DisplayIntensifyMode == DisplayIntensifyMode.Hi;

		    UpdateDisplayModeStatusLabel();

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
                                lastDisplayedVideoFrameNumber = frameWrapper.UniqueFrameId;

                                Bitmap bmp;

                                var pixels = frame.ImageArray as int[,];
                                if (pixels != null && videoObject.Connected && videoObject.SensorType == SensorType.Monochrome)
                                {
                                    cameraImage.SetImageArray(
                                       pixels,
                                       imageWidth,
                                       imageHeight,
                                       videoObject.SensorType);

                                    if (m_DisplayIntensifyMode != DisplayIntensifyMode.Off || m_DisplayInvertedMode || m_DisplayHueIntensityMode || m_DisplaySaturationCheckMode)
                                    {
                                        bmp = cameraImage.GetDisplayBitmap(m_DisplayIntensifyMode, m_DisplayInvertedMode, m_DisplayHueIntensityMode, m_DisplaySaturationCheckMode, Settings.Default.SaturationWarning);    
                                    }
                                    else
                                    {
                                        bmp = cameraImage.GetDisplayBitmap();    
                                    }
                                }
                                else
                                {
                                    bmp = frame.PreviewBitmap;

                                    if (bmp == null)
                                    {
                                        cameraImage.SetImageArray(
                                            frame.ImageArray,
                                            imageWidth,
                                            imageHeight,
                                            videoObject.SensorType);

                                        bmp = cameraImage.GetDisplayBitmap();
                                    }

                                    if (frame.ImageArray == null)
                                    {
                                        frameWrapper.ImageArray = cameraImage.GetImageArray(bmp, SensorType.Monochrome, LumaConversionMode.R, Settings.Default.HorizontalFlip, Settings.Default.VerticalFlip);
                                    }
                                }

                                stateManager.ProcessFrame(frameWrapper);

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

        private void UpdateDisplayModeStatusLabel()
        {
            string label = "Display Mode";
            if (m_DisplayIntensifyMode != DisplayIntensifyMode.Off)
            {
                label += m_DisplayIntensifyMode == DisplayIntensifyMode.Hi ? " - Hi" : " - Lo";
            }
            m_MainForm.tsbtnDisplayMode.Text = label;
        }

		public void SetDisplayIntensifyMode(DisplayIntensifyMode newMode)
		{
			m_DisplayIntensifyMode = newMode;

			Settings.Default.DisplayIntensifyMode = newMode;
			Settings.Default.Save();

		    UpdateDisplayModeStatusLabel();
		}

		public void SetDisplayInvertMode(bool inverted)
		{
			m_DisplayInvertedMode = inverted;

			Settings.Default.UseInvertedDisplayMode = inverted;
			Settings.Default.Save();
		}

		public void SetDisplayHueMode(bool hueSelected)
		{
			m_DisplayHueIntensityMode = hueSelected;

			Settings.Default.UseHueIntensityDisplayMode = hueSelected;
			Settings.Default.Save();
		}

        public void SetDisplaySaturationMode(bool saturationSelected)
		{
            m_DisplaySaturationCheckMode = saturationSelected;

            Settings.Default.UseSaturationCheckDisplayMode = saturationSelected;
			Settings.Default.Save();
		}        

        public void Dispose()
        {
            running = false;
	        analysisManager.Dispose();
        }
    }
}
