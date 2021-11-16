﻿/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using System.Xml;
using OccuRec.ASCOM.Wrapper;
using OccuRec.ASCOM;
using OccuRec.ASCOM.Interfaces.Devices;
using OccuRec.Config;
using OccuRec.Context;
using OccuRec.Controllers;
using OccuRec.Drivers;
using OccuRec.Drivers.QHYVideo;
using OccuRec.FrameAnalysis;
using OccuRec.Helpers;
using OccuRec.OCR;
using OccuRec.Properties;
using OccuRec.Scheduling;
using OccuRec.StateManagement;
using OccuRec.Tracking;
using OccuRec.Utilities;

namespace OccuRec
{
    public partial class frmMain : Form, IVideoCallbacks
	{
		private VideoWrapper videoObject;

		private int imageWidth;
		private int imageHeight;
		private string recordingfileName;
		private int framesBeforeUpdatingCameraVideoFormat = -1;

	    private CameraStateManager m_StateManager;
	    private FrameAnalysisManager m_AnalysisManager;
	    private string appVersion;
		private IObservatoryController m_ObservatoryController;
	    private OverlayManager m_OverlayManager = null;
	    private List<string> initializationErrorMessages = new List<string>();

        public static List<Point> s_BadPixels = new List<Point>();

        private VideoFrameInteractionController m_VideoFrameInteractionController;
        private VideoRenderingController m_VideoRenderingController;
        
        public frmMain()
		{
			InitializeComponent();

			statusStrip.SizingGrip = false;

            m_StateManager = new CameraStateManager();
            m_StateManager.CameraDisconnected();

		    m_ObservatoryController = new ObservatoryController();
		    m_ObservatoryController.TelescopeConnectionChanged += TelescopeConnectionChanged;
            m_ObservatoryController.FocuserConnectionChanged += FocuserConnectionChanged;
			m_ObservatoryController.VideoConnectionChanged += VideoConnectionChanged;
		    m_ObservatoryController.TelescopeStateUpdated += TelescopeStateUpdated;
		    m_ObservatoryController.FocuserStateUpdated += FocuserStateUpdated;
            m_ObservatoryController.FocuserPositionUpdated += FocuserPositionUpdated;
			m_ObservatoryController.VideoStateUpdated += VideoStateUpdated;
			m_ObservatoryController.VideoError += VideoError;

            tsbAddBadPixelMarkers.Visible = false;

			m_AnalysisManager = new FrameAnalysisManager(m_ObservatoryController);
            m_VideoRenderingController = new VideoRenderingController(this, m_StateManager, m_AnalysisManager);
            m_VideoFrameInteractionController = new VideoFrameInteractionController(this, m_VideoRenderingController);

            Version occuRecVersion = Assembly.GetExecutingAssembly().GetName().Version;
            bool isBeta = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(BetaReleaseAttribute), false).Length == 1;
            appVersion = string.Format("v{0}.{1}{2}", occuRecVersion.Major, occuRecVersion.Minor, isBeta ? " BETA" : "");

            Text = string.Format("OccuRec {0}", appVersion);

            CheckForUpdates(false);

		    UpdateState(null);

			ASCOMClient.Instance.Initialise(Settings.Default.ASCOMLoadInSeparateAppDomain);
			TelescopeConnectionChanged(ASCOMConnectionState.Disconnected);
			FocuserConnectionChanged(ASCOMConnectionState.Disconnected);
			VideoConnectionChanged(ASCOMConnectionState.Disconnected);

		    UpdateASCOMConnectivityState();

            NativeHelpers.SetSystemInformation(Environment.OSVersion.VersionString, NativeHelpers.GetTimerResolution().ToString());

		    InitPerfCounterMonitoring();

            // To collect (and save) own logs, at the time of the start up the status section recodring must be turned on
		    if (!Settings.Default.RecordStatusSectionOnly)
		    {
		        OccuRecSelfTraceListener.Instance.Stop();
		    }
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);

			DisconnectFromCamera();
		    m_VideoRenderingController.Dispose();
			m_ObservatoryController.Dispose();
            ASCOMClient.Instance.Dispose();
		    QHYCameraManager.Instance.Dispose();

		    StopPerfCounterMonitoring();
		}

        public void OnError(int errorCode, string errorMessage)
        {
            if (m_OverlayManager != null)
                m_OverlayManager.OnError(errorCode, errorMessage);
            else
                initializationErrorMessages.Add(errorMessage);
        }

        public void OnInfo(string infoMessage)
        {
            if (m_OverlayManager != null)
                m_OverlayManager.OnInfo(infoMessage);
        }

        public void OnEvent(int eventId, string eventData)
        {
            if (eventId == 1)
                m_StateManager.RegisterOcrError();

            if (m_OverlayManager != null)
                m_OverlayManager.OnEvent(eventId, eventData);
        }


        private void tsbConnectDisconnect_Click(object sender, EventArgs e)
        {
            if (videoObject == null)
                ConnectToCamera();
            else
                DisconnectFromCamera();
        }

		private void ConnectToCamera()
		{
		    var chooser = new frmChooseCamera();
			chooser.StartPosition = FormStartPosition.CenterParent;
            if (chooser.ShowDialog(this) == DialogResult.OK)
            {

				IVideo driverInstance = null;

				if (OccuRecContext.Current.IsAAV)
				{
					if (Settings.Default.FileSimulation)
					{
						if (".avi".Equals(Path.GetExtension(Settings.Default.SimulatorFilePath), StringComparison.InvariantCultureIgnoreCase))
                        {	
                            if (Settings.Default.SimulatorRunOCR)
                                // OCR simulation only done in AAV mode
                                driverInstance = new Drivers.AVISimulator.Video(true);
                            else
                                driverInstance = new Drivers.DirectShowSimulator.Video(true);}
						else
							driverInstance = new Drivers.AAVSimulator.Video(true);
					}
					else
					{
						driverInstance = new Drivers.AAVTimer.Video();
						if (chooser.CameraControlDriver != null)
						{
							m_ObservatoryController.SetExternalCameraDriver(chooser.CameraControlDriver);
							VideoConnectionChanged(ASCOMConnectionState.Disconnected);
						}
					}

					
				}
				else
				{
					if (Settings.Default.FileSimulation)
					{
						if (".avi".Equals(Path.GetExtension(Settings.Default.SimulatorFilePath), StringComparison.InvariantCultureIgnoreCase))
                        {
                            if (Settings.Default.SimulatorRunOCR)
                                // OCR simulation only done in AAV mode
                                driverInstance = new Drivers.AVISimulator.Video(false);
                            else
                                driverInstance = new Drivers.DirectShowSimulator.Video(true);
                        }
						else
							driverInstance = new Drivers.AAVSimulator.Video(false);
					}
					else
					{
						// TODO:
						MessageBox.Show("ASCOM Video is not implemented yet!");
					}
				}

	            if (driverInstance != null)
		            ConnectToDriver(driverInstance);
            }
		}

		private void ConnectToDriver(IVideo driverInstance)
		{
			try
			{
				Cursor = Cursors.WaitCursor;
				initializationErrorMessages.Clear();

				m_PrevStateWasDisconnected = true;

                videoObject = m_VideoRenderingController.ConnectToDriver(driverInstance);

				if (videoObject.Connected)
				{
					imageWidth = videoObject.Width;
					imageHeight = videoObject.Height;
					picVideoFrame.Image = new Bitmap(imageWidth, imageHeight);

					ResizeVideoFrameTo(imageWidth, imageHeight);
					tssIntegrationRate.Visible = Settings.Default.IsIntegrating && OccuRecContext.Current.IsAAV;
					pnlAAV.Visible = OccuRecContext.Current.IsAAV;
					tsbtnDisplayMode.Visible = true;

					if (videoObject.SupporstFreeStyleGain) videoObject.SetFreeRangeGainIntervals(0);

					m_OverlayManager = new OverlayManager(videoObject.Width, videoObject.Height, initializationErrorMessages, m_AnalysisManager, m_StateManager);
					m_VideoFrameInteractionController.OnNewVideoSource(videoObject);

					OccuRecContext.Current.IsConnected = true;

                    if (OccuRecContext.Current.FailedToSetRequestedMode)
                    {
                        var formatToSet = new VideoFormatHelper.SupportedVideoFormat(Settings.Default.SelectedVideoFormat);
                        var formatSet = new VideoFormatHelper.SupportedVideoFormat(OccuRecContext.Current.VideoModeSet);
                        var errorExplanation = "this mode";
                        if (formatToSet.IsNtscFrameRate() && formatSet.IsPalFrameRate())
                            errorExplanation = "NTSC";
                        else if (formatToSet.IsPalFrameRate() && formatSet.IsNtscFrameRate())
                            errorExplanation = "PAL";

                        MessageBox.Show(
                            this,
                            string.Format("There was an error trying to use a video mode: {0}\r\n\r\nInstead the grabber set the video mode to: {1}\r\n\r\nThis error most likely indicates that your frame grabber or your camera doesn't support {2}.", formatToSet.ToString(), formatSet.ToString(), errorExplanation),
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }

                    if (!string.IsNullOrEmpty(OccuRecContext.Current.StandardVideoModeSet))
                    {
                        tssStandardVideoMode.Text = OccuRecContext.Current.StandardVideoModeSet;
                        tssStandardVideoMode.Visible = true;
                    }
                    else
                        tssStandardVideoMode.Visible = false;

					if (Settings.Default.RecordStatusSectionOnly)
						MessageBox.Show(
							this,
							"The 'Record Status Section Only' flag is currently enabled. No video images will be recorded.",
							"OccuRec",
							MessageBoxButtons.OK,
							MessageBoxIcon.Warning);
				}

				m_StateManager.CameraConnected(driverInstance, videoObject, m_OverlayManager, Settings.Default.OcrMaxErrorsPerCameraTestRun, OccuRecContext.Current.IsAAV);
				UpdateScheduleDisplay();
			}
			finally
			{
				Cursor = Cursors.Default;

				if (videoObject == null || !videoObject.Connected)
				{
					foreach (string error in initializationErrorMessages)
					{
						MessageBox.Show(this, error, "OccuRec", MessageBoxButtons.OK, MessageBoxIcon.Error);
					}					
				}
			}


			picVideoFrame.Width = videoObject.Width;
			picVideoFrame.Height = videoObject.Height;

			UpdateCameraState(true);
			ucVideoControl.Initialize(videoObject);
		}

		private void DisconnectFromCamera()
		{
			if (videoObject != null)
			{
			    if (videoObject.State == VideoCameraState.videoCameraRecording)
			    {
			        videoObject.StopRecording();
			        if (Settings.Default.RecordStatusSectionOnly)
			        {
			            OccuRecSelfTraceListener.Instance.SaveLog(Path.ChangeExtension(recordingfileName, "log"));
			        }
			    }

			    videoObject.Disconnect();
				videoObject = null;
			}

			UpdateCameraState(false);
		    tssIntegrationRate.Visible = false;
			tsbtnDisplayMode.Visible = false;
            tssStandardVideoMode.Visible = false;

		    m_StateManager.CameraDisconnected();

            if (m_OverlayManager != null)
            {
                m_OverlayManager.Finalise();
                m_OverlayManager = null;
            }

			if (m_ObservatoryController.IsConnectedToVideoCamera())
				m_ObservatoryController.DisconnectVideoCamera();

			m_PrevStateWasDisconnected = true;
			OccuRecContext.Current.IsConnected = false;
		}

        private bool CanRecordNow(bool connected)
        {
            return connected && 
                videoObject != null && 
                (videoObject.State == VideoCameraState.videoCameraRunning || Settings.Default.IntegrationDetectionTuning)&& 
                lbSchedule.Items.Count == 0;
        }

        private bool CanStopRecordingNow(bool connected)
        {
            return connected && 
                videoObject != null && 
                videoObject.State == VideoCameraState.videoCameraRecording && 
                lbSchedule.Items.Count == 0;
        }

		private void UpdateCameraState(bool connected)
		{
			pnlVideoControls.Enabled = connected;
			miConnect.Enabled = !connected;
			miDisconnect.Enabled = connected;
            miOpenFile.Enabled = !connected;

            pnlOcrTesting.Visible = false;

			UpdateState(null);

			pnlVideoControls.Enabled = connected;
		    btnRecord.Enabled = CanRecordNow(connected);
            btnStopRecording.Enabled = CanStopRecordingNow(connected);
			btnImageSettings.Enabled = connected && videoObject != null && videoObject.CanConfigureImage;

			ucVideoControl.Visible = connected && videoObject != null && videoObject.AllowsCameraControl;
		    
			if (videoObject != null)
			{
				lblVideoFormat.Text = videoObject.CameraVideoFormat;

                Text = string.Format("OccuRec {0} - {1}{2}",
                        appVersion,
						videoObject.DeviceName, 
						videoObject.VideoCaptureDeviceName != null
							? string.Format(" ({0})", videoObject.VideoCaptureDeviceName) 
							: string.Empty);

                if (Settings.Default.UsesTunerCrossbar && videoObject.SupportsCrossbar)
                {
                    cbxCrossbarInput.Items.Clear();
                    cbxCrossbarInput.SelectedIndexChanged -= new EventHandler(cbxCrossbarInput_SelectedIndexChanged);
                    try
                    {
                        videoObject.LoadCrossbarSources(cbxCrossbarInput);
                    }
                    finally
                    {
                        cbxCrossbarInput.SelectedIndexChanged += new EventHandler(cbxCrossbarInput_SelectedIndexChanged);
                        Cursor = Cursors.Default;
                    }  

                    pnlCrossbar.Visible = true;
                }

                pnlOcrTesting.Visible =
					 (Settings.Default.OcrCameraAavTestMode && OccuRecContext.Current.IsAAV);
			}
			else
			{
                pnlCrossbar.Visible = false;
				lblVideoFormat.Text = "N/A";
                Text = string.Format("OccuRec {0}", appVersion);
			}
		}

        private void cbxCrossbarInput_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedItem = cbxCrossbarInput.SelectedItem as CrossbarHelper.CrossbarPinEntry;

            if (videoObject.SupportsCrossbar && selectedItem != null)
            {
                videoObject.ConnectToCrossbarSource(selectedItem.PinIndex);
            }
        }

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			DisconnectFromCamera();

			Close();
		}

		private void miConfigure_Click(object sender, EventArgs e)
		{
            var frmSettings = new frmSettings(m_ObservatoryController, videoObject == null);

		    frmSettings.ShowDialog(this);
            UpdateASCOMConnectivityState();
			UpdateNTPConnectivityState();
			m_LastKnownGoodNTPServersInitialised = false;
			UpdateObservingAidControls();
		}

		private void miConnect_Click(object sender, EventArgs e)
		{
            if (!Directory.Exists(Settings.Default.OutputLocation))
            {
                MessageBox.Show("Output Video Location is invalid.", "OccuRec", MessageBoxButtons.OK, MessageBoxIcon.Error);

                var frmSettings = new frmSettings(m_ObservatoryController, true);

                frmSettings.ShowDialog(this);
            }
            else
			    ConnectToCamera();
		}

		private void miDisconnect_Click(object sender, EventArgs e)
		{
			DisconnectFromCamera();
		}

		private static Font debugTextFont = new Font(FontFamily.GenericMonospace, 10);

		private int renderedFrameCounter = 0;
		private long startTicks = 0;
		private long endTicks = 0;

		private double renderFps = double.NaN;
		private long currentFrameNo = 0;


        internal void PaintVideoFrame(VideoFrameWrapper frame, Bitmap bmp)
		{
            bool isEmptyFrame = frame == null || bmp == null;

			if (isEmptyFrame)
			{
				if (picVideoFrame.Image != null)
				{
					using (Graphics g = Graphics.FromImage(picVideoFrame.Image))
					{
						if (bmp == null)
							g.Clear(Color.Green);
						else
							g.DrawImage(bmp, 0, 0);

						if (m_OverlayManager != null)
                            m_OverlayManager.ProcessFrame(g, frame);

						g.Save();
					}					
				}

				picVideoFrame.Invalidate();
				return;
			}

	        currentFrameNo = frame.FrameNumber;
			UpdateState(frame);
			renderedFrameCounter++;

			if (renderedFrameCounter == 20)
			{
				renderedFrameCounter = 0;
				endTicks = DateTime.Now.Ticks;
				if (startTicks != 0)
				{
					renderFps = 20.0 / new TimeSpan(endTicks - startTicks).TotalSeconds;
				}
				startTicks = DateTime.Now.Ticks;
			}

			if (picVideoFrame.Image != null)
			{
				using (Graphics g = Graphics.FromImage(picVideoFrame.Image))
				{
					g.DrawImage(bmp, 0, 0);

					if (m_OverlayManager != null)
                        m_OverlayManager.ProcessFrame(g, frame);

					g.Save();
				}				
			}

			picVideoFrame.Invalidate();
			bmp.Dispose();

			if (framesBeforeUpdatingCameraVideoFormat >= 0)
				framesBeforeUpdatingCameraVideoFormat--;

			if (framesBeforeUpdatingCameraVideoFormat == 0)
			{
				lblVideoFormat.Text = videoObject.CameraVideoFormat;
			}

            if (m_StateManager.ProvidesOcredTimestamps)
            {
				if (m_StateManager.OcrErrors > 0)
				{
					if (tssOcrErr.Tag == null || (int)tssOcrErr.Tag != 2)
					{
						tssOcrErr.ForeColor = Color.Red;
						tssOcrErr.Tag = (int) 2;
					}

					tssOcrErr.Text = string.Format("OCR ERR {0}", m_StateManager.OcrErrors);	
					
				}
				else
				{
					if (tssOcrErr.Tag == null || (int) tssOcrErr.Tag != 1)
					{
						tssOcrErr.Text = "OCR";
						tssOcrErr.ForeColor = Color.Green;
						tssOcrErr.Tag = (int)1;
					}
				}

				if (!tssOcrErr.Visible)
					tssOcrErr.Visible = true;
            }
            else
            {
                if (tssOcrErr.Visible)
                    tssOcrErr.Visible = false;
            }

            if (m_StateManager.DroppedFrames != 0)
            {
                tssDroppedFrames.Text = string.Format("{0} Dropped", m_StateManager.DroppedFrames);

                if (!tssDroppedFrames.Visible)
                    tssDroppedFrames.Visible = true;
            }
            else
            {
                if (tssDroppedFrames.Visible)
                    tssDroppedFrames.Visible = false;
            }
			

            if (frame.PerformedAction.HasValue && frame.PerformedAction.Value > 0 &&
                frame.PerformedActionProgress.HasValue)
            {
                ttsProgressBar.Value = (int) Math.Max(0, Math.Min(100, 100*frame.PerformedActionProgress.Value));
                ttsProgressBar.Visible = true;
            }
            else
                ttsProgressBar.Visible = false;
		}

        private void UpdateApplicationStateFromCameraState()
        {
            switch (videoObject.State)
            {
                case VideoCameraState.videoCameraRunning:
					tssCameraState.Text = "Running";
                    break;

                case VideoCameraState.videoCameraRecording:
		            try
		            {
			            TimeSpan rec = videoObject.RecordingFor;
						if (rec.TotalMinutes < 1)
							tssCameraState.Text = "Recording";
						else
							tssCameraState.Text = string.Format("Recording for {0} min", (int)rec.TotalMinutes);
		            }
		            catch
		            {
						tssCameraState.Text = "Recording";
		            }
                    break;

                case VideoCameraState.videoCameraError:
                    tssCameraState.Text = "Error";
                    break;
            }
        }

		private void EnsureSchedulesState(bool enabled)
		{
			if (gbxSchedules.Enabled != enabled) 
				gbxSchedules.Enabled = enabled;
		}

	    private bool? m_PrevStateWasDisconnected = false;

		private bool ChangedToDisconnectedState()
		{
			if (!m_PrevStateWasDisconnected.HasValue)
			{
				m_PrevStateWasDisconnected = videoObject == null;
				return videoObject == null;
			}
			else if (!m_PrevStateWasDisconnected.Value && videoObject == null)
			{
				m_PrevStateWasDisconnected = true;
				return true;
			}

			return false;
		}

		private bool ChangedToConnectedState()
		{
			if (!m_PrevStateWasDisconnected.HasValue)
			{
				m_PrevStateWasDisconnected = videoObject == null;
				return videoObject != null;
			}
			else if (m_PrevStateWasDisconnected.Value && videoObject != null)
			{
				m_PrevStateWasDisconnected = false;
				return true;
			}

			return false;
		}

		bool CameraSupportsSoftwareControl()
		{
			// TODO: Determine this from the ASCOM object

			return false;
		}

		private void UpdateNTPStatus()
		{
			string statusText;
			if (Settings.Default.NTPTimeStampsInAAVEnabled)
			{
				Color clr = NTPTimeKeeper.GetCurrentNTPStatusColour(out statusText);
				if (statusText == null)
				{
					if (tssNTP.Visible)
						tssNTP.Visible = false;

					tssNTP.ToolTipText = string.Empty;
				}
				else
				{
					if (!tssNTP.Visible)
						tssNTP.Visible = true;

					tssNTP.ForeColor = clr;
					tssNTP.ToolTipText = string.Empty;
				}
			}
			else
			{
				tssNTP.Visible = false;
			}
		}

		private void UpdateState(VideoFrameWrapper frame)
		{
		    if (IsDisposed)
                // It is possible this method to be called during Disposing and we don't need to do anything in that case
		        return;

            // TODO: Many of things below only change their state when something changes. Rather than always resetting their state with each rendered frame, we should really 
            //       use events to update the state!

            if (ChangedToDisconnectedState())
			{
				tssCameraState.Text = "Disconnected";
				tssFrameNo.Text = string.Empty;
				tssDisplayRate.Text = string.Empty;
				tssFrameNo.Visible = false;
				tssDisplayRate.Visible = false;
				tssIntegrationRate.Visible = false;
				tssDroppedFrames.Visible = false;
				tssOcrErr.Visible = false;

				tsbConnectDisconnect.ToolTipText = "Connect";
				tsbConnectDisconnect.Image = imageListToolbar.Images[0];

				tbsAddTarget.Visible = false;
				tsbAddGuidingStar.Visible = false;
                tbsInsertSpectra.Visible = false;
				tbsClearTargets.Visible = false;
                tsbAddBadPixelMarkers.Visible = false;
				tssToolBorder.Visible = false;

				//tsbCamControl.Enabled = false;
			}
			else if (ChangedToConnectedState())
			{
				tbsAddTarget.Visible = videoObject.SupportsTargetTracking;
				tbsClearTargets.Visible = videoObject.SupportsTargetTracking;
				tsbAddGuidingStar.Visible = videoObject.SupportsTargetTracking;
				tbsInsertSpectra.Visible = videoObject.SupportsTargetTracking && Settings.Default.SpectraUseAid;
				tssToolBorder.Visible = videoObject.SupportsTargetTracking;
			    
				tbsAddTarget.Enabled = false;
				tbsClearTargets.Enabled = false;
				tsbAddGuidingStar.Enabled = true;
                tbsInsertSpectra.Enabled = true;
                tsbAddBadPixelMarkers.Enabled = Settings.Default.EnableBadPixelsControl;
                tsbAddBadPixelMarkers.Visible = Settings.Default.EnableBadPixelsControl;

                TrackingContext.Current.Reset();
				TrackingContext.Current.ReConfigureNativeTracking(videoObject.Width, videoObject.Height);

                //tsbCamControl.Enabled = CameraSupportsSoftwareControl();

                tsbConnectDisconnect.ToolTipText = "Disconnect";
                tsbConnectDisconnect.Image = imageListToolbar.Images[1];
			}

			if (videoObject != null)
			{
			    UpdateApplicationStateFromCameraState();

				if (frame != null)
				{
					if (!tssFrameNo.Visible) tssFrameNo.Visible = true;

					if (m_StateManager.IsIntegrationLocked)
						tssFrameNo.Text = frame.IntegratedFrameNo.ToString("Integrated Frame: 0", CultureInfo.InvariantCulture);
					else
						tssFrameNo.Text = frame.FrameNumber.ToString("Current Frame: 0", CultureInfo.InvariantCulture);

					if (m_StateManager.IsIntegrationLocked)
					{
						if (!string.IsNullOrEmpty(frame.ImageInfo))
						{
							if (frame.IntegrationRate != null)
								tssIntegrationRate.Text = string.Format("Integration Rate: x{0}", frame.IntegrationRate);
							else
								tssIntegrationRate.Text = "Integration Rate: ...";

							if (!tssIntegrationRate.Visible) tssIntegrationRate.Visible = true;
						}
						else
						{
							if (tssIntegrationRate.Visible) tssIntegrationRate.Visible = false;
						}
					}
					else
					{
						if (tssIntegrationRate.Visible) tssIntegrationRate.Visible = false;
					}
				}

				tbsAddTarget.Enabled = TrackingContext.Current.GuidingStar != null;
				tbsClearTargets.Enabled = TrackingContext.Current.GuidingStar != null;
				tsbAddGuidingStar.Enabled = true;
			    tbsInsertSpectra.Enabled = true;
#if DEBUG
				if (!double.IsNaN(renderFps))
				{
					if (!tssDisplayRate.Visible) tssDisplayRate.Visible = true;
					tssDisplayRate.Text = renderFps.ToString("Display Rate: 0 fps");
				}
				else
					tssDisplayRate.Text = "Display Rate: N/A";
#endif
				if (videoObject.State == VideoCameraState.videoCameraRecording && File.Exists(recordingfileName))
				{
					var fi = new FileInfo(recordingfileName);
					tssRecordingFile.Text = string.Format("{0} ({1:0.0} Mb)", fi.Name, 1.0 * fi.Length / (1024 * 1024));

					if (Settings.Default.WarnForFileSystemIssues && Settings.Default.WarnOnFreeDiskSpaceLeft && fi.Directory != null)
					{
						ulong freeBytes;
						NativeHelpers.GetDriveFreeBytes(fi.Directory.FullName, out freeBytes);

						if (freeBytes < ((ulong)1024 * (ulong)1024 * (ulong)1024 * (ulong)Settings.Default.WarnMinDiskFreeSpaceGb))
						{
							tssFreeDiskSpace.Visible = true;
							tssFreeDiskSpace.Text = string.Format("{0:0.0} Gb free", 1.0 * freeBytes / (1024 * 1024 * 1024));
						}
						else
							tssFreeDiskSpace.Visible = false;
					}
					else
						tssFreeDiskSpace.Visible = false;

					tssRecordingFile.Visible = true;
				    if (!btnStopRecording.Enabled)
				    {
                        btnStopRecording.Enabled = true;
                        lbSchedule.Focus();
				    }
					btnRecord.Enabled = false;
				}
                else if (
                    videoObject.State == VideoCameraState.videoCameraRunning && 
                    lbSchedule.Items.Count == 0 && 
                    (m_StateManager.CanStartRecording || Settings.Default.IntegrationDetectionTuning))
                {
                    tssRecordingFile.Visible = false;
					tssFreeDiskSpace.Visible = false;
                    btnStopRecording.Enabled = false;
                    btnRecord.Enabled = true;
                }
                else
                {
                    tssRecordingFile.Visible = false;
					tssFreeDiskSpace.Visible = false;
                    btnRecord.Enabled = false;
                    btnStopRecording.Enabled = false;
                }

				if (videoObject.State == VideoCameraState.videoCameraRunning && OccuRecContext.Current.IsAAV)
				{
					if (m_StateManager.VtiOsdPositionUnknown)
					{
						if (pnlAAV.Visible) pnlAAV.Visible = false;

						if (!pnlVtiOsd.Visible)
						{
							pnlVtiOsd.Visible = true;
							if (!Settings.Default.PreserveVTIUserSpecifiedValues)
							{
								Settings.Default.PreserveVTIFirstRow = videoObject.Height - 28;
								Settings.Default.PreserveVTILastRow = videoObject.Height;
							}
						}

						if (tssVTIOSD.Visible ^ !Settings.Default.PreserveVTIUserSpecifiedValues)
							tssVTIOSD.Visible = !Settings.Default.PreserveVTIUserSpecifiedValues;
					}
					else
					{
						if (!pnlAAV.Visible) pnlAAV.Visible = true;
						if (pnlVtiOsd.Visible) pnlVtiOsd.Visible = false;
						if (tssVTIOSD.Visible) tssVTIOSD.Visible = false;
					}

					if (btnConfirmUserVtiOsd.Enabled ^ Settings.Default.PreserveVTIUserSpecifiedValues)
					{
						btnConfirmUserVtiOsd.Enabled = Settings.Default.PreserveVTIUserSpecifiedValues;
					}
				}
				else
				{
					if (pnlAAV.Visible) pnlAAV.Visible = false;
					if (pnlVtiOsd.Visible) pnlVtiOsd.Visible = false;
					if (tssVTIOSD.Visible) tssVTIOSD.Visible = false;
				}

				btnLockIntegration.Enabled = 
					(
						m_StateManager.CanLockIntegrationNow && 
						m_StateManager.IntegrationRate > 0 &&
						m_StateManager.IsValidIntegrationRate && 
						(frame == null || !frame.PerformedAction.HasValue || frame.PerformedAction.Value == 0)
                     ) 
					|| m_StateManager.IsIntegrationLocked;

				btnCalibrateIntegration.Visible = !m_StateManager.IsIntegrationLocked;
				btnManualIntegration.Visible = !m_StateManager.IsIntegrationLocked;
				if (!m_StateManager.IsIntegrationLocked && m_StateManager.PercentDoneDetectingIntegration < 100)
				{
					pbarIntDetPercentDone.Value = m_StateManager.PercentDoneDetectingIntegration;
					if (!pbarIntDetPercentDone.Visible) pbarIntDetPercentDone.Visible = true;
				}
				else if (pbarIntDetPercentDone.Visible) pbarIntDetPercentDone.Visible = false;

				pnlOneStacking.Visible = m_StateManager.IsIntegrationLocked && m_ManualIntegration == 1;

				if (frame != null && frame.PerformedAction.HasValue && frame.PerformedAction.Value > 0)
				{
					// When there is an action in progress, then don't show anything
					btnLockIntegration.Text = "Busy ...";
				}
				else if (m_StateManager.IntegrationRate > 0 && m_StateManager.IsValidIntegrationRate && !m_StateManager.IsIntegrationLocked && m_StateManager.CanLockIntegrationNow)
                    btnLockIntegration.Text = string.Format("Lock at x{0} Frames", m_StateManager.IntegrationRate);
                else if (m_StateManager.IsIntegrationLocked)
                    btnLockIntegration.Text = "Unlock";
                else
                    btnLockIntegration.Text = "Checking Integration ...";

                if (m_StateManager.IsCalibratingIntegration)
				{
					btnCalibrateIntegration.Text = "Cancel Calibration";
					tssCameraState.Text = "Calibrating";
					EnsureSchedulesState(false);
				}
				else if (frame != null && frame.PerformedAction.HasValue)
				{
					if (frame.PerformedAction.Value == 2)
					{
						// Checking Manually Entered Integration
						tssCameraState.Text = "Busy";
					}
				}
                else
                {
                    btnCalibrateIntegration.Text = "Calibrate";
					btnOcrTesting.Text = "Run OCR Testing";
                    UpdateApplicationStateFromCameraState();
					EnsureSchedulesState(true);
                }

				if (m_StateManager.IsUsingManualIntegration)
					btnManualIntegration.Text = "Automatic";
				else
					btnManualIntegration.Text = "Manual";
			}
		}

        private void StartRecording()
        {
            if (videoObject != null)
            {
                NativeHelpers.CurrentTargetInfo = tbxTargetName.Text;

                string fileName = FileNameGenerator.GenerateFileName(OccuRecContext.Current.IsAAV);
                recordingfileName = videoObject.StartRecording(fileName);

                UpdateState(null);

                framesBeforeUpdatingCameraVideoFormat = 4;

                if (Settings.Default.RecordStatusSectionOnly)
                {
                    restartRecTimer.Interval = Settings.Default.StatusSectionOnlyRestartRecordingIntervalMins * 60 * 1000;
                    restartRecTimer.Enabled = true;
                }
            }
        }

        private void btnRecord_Click(object sender, EventArgs e)
        {
            StartRecording();
        }

		private void StopRecording()
		{
			if (videoObject != null)
			{
			    bool wasLocked = m_StateManager.IsIntegrationLocked;

                restartRecTimer.Enabled = false;

				videoObject.StopRecording();

			    if (Settings.Default.RecordStatusSectionOnly)
			    {
                    OccuRecSelfTraceListener.Instance.SaveLog(Path.ChangeExtension(recordingfileName, "log"));    
			    }

				// If the next scheduled operation was to stop the recoring, but the recording was stopped manually
				// then remove the scheduled stopping too.
				ScheduleEntry entry = Scheduler.GetNextEntry();
				if (entry != null && entry.Action == ScheduledAction.StopRecording)
				{
					Scheduler.RemoveOperation(entry.OperaionId);
					UpdateScheduleDisplay();
				}

				UpdateState(null);

				if (wasLocked)
				{
					m_StateManager.UnlockIntegration();
					lblOneStack.Text = "No Stacking";
					NativeHelpers.SetStackRate(0);	
				}
			}
		}

        private void btnStopRecording_Click(object sender, EventArgs e)
        {
            StopRecording();
        }

        private void ResizeVideoFrameTo(int imageWidth, int imageHeight)
		{
			Width = Math.Max(800, (imageWidth - picVideoFrame.Width) + this.Width);
			Height = Math.Max(600, (imageHeight - picVideoFrame.Height) + this.Height);
		}

		private void btnImageSettings_Click(object sender, EventArgs e)
		{
			if (videoObject != null)
				videoObject.ConfigureImage();
		}

        private void btnLockIntegration_Click(object sender, EventArgs e)
        {
	        if (m_StateManager.IsIntegrationLocked)
	        {
		        if (m_StateManager.UnlockIntegration())
		        {
					lblOneStack.Text = "No Stacking";
					NativeHelpers.SetStackRate(0);			        
		        }
	        }
	        else if (m_StateManager.CanLockIntegrationNow)
		        m_StateManager.LockIntegration();
        }


        private void btnOcrTesting_Click(object sender, EventArgs e)
        {
            if (Scheduler.GetAllSchedules().Count > 0)
                MessageBox.Show("OCR testing cannot be done if there are scheduled tasks.");
            else
            {
				if (OccuRecContext.Current.IsAAV)
                {
                    if (videoObject != null)
                    {
						recordingfileName = m_StateManager.StartRecordingOCRTestingFile();
                    }
                        
                }

                UpdateState(null);
            } 
        }

        private void btnAddSchedule_Click(object sender, EventArgs e)
        {
            var frm = new frmAddScheduleEntry(m_ObservatoryController, m_AnalysisManager);
            if (frm.ShowDialog(this) == DialogResult.OK)
            {
                UpdateScheduleDisplay();
            }
        }

        private void UpdateScheduleDisplay()
        {
            lbSchedule.Items.Clear();

            foreach(ScheduleEntry entry in Scheduler.GetAllSchedules())
            {
                lbSchedule.Items.Add(entry);
            }
        }

	    private DateTime nextNTPSyncTime = DateTime.MinValue;
        private DateTime nextNTPTimeOneMinCheckUTC = DateTime.MinValue;
		private DateTime nextObservatoryCheckUTC = DateTime.MinValue;

        private void timerScheduler_Tick(object sender, EventArgs e)
        {
	        CheckAndActionScheduledEntries();

			if (m_StateManager.IsRecordingOcrTestFile && videoObject != null && videoObject.State == VideoCameraState.videoCameraRecording)
			{
				// NOTE: If we have been recording OCR test file and there are more than MaxErrorCount OCR errors then stop the recording
				if (m_StateManager.OcrErrors > Settings.Default.OcrMaxErrorsPerCameraTestRun)
				{
					m_StateManager.StopRecordingOCRTestingFile();
					UpdateState(null);
				}
			}

			if (nextObservatoryCheckUTC <= DateTime.UtcNow)
            {
				if (Settings.Default.ObservatoryStatusPingRateSeconds > 0)
				{
					m_ObservatoryController.PerformTelescopePingActions();
					m_ObservatoryController.PerformFocuserPingActions();					
				}

                if (Settings.Default.ObservatoryStatusPingRateSeconds > 0)
					nextObservatoryCheckUTC = DateTime.UtcNow.AddSeconds(Settings.Default.ObservatoryStatusPingRateSeconds);
				else
					// Check again in a minute if Settings.Default.ObservatoryStatusPingRateSeconds has changed
					nextObservatoryCheckUTC = DateTime.UtcNow.AddMinutes(1);
            }

	        if (nextNTPTimeOneMinCheckUTC <= DateTime.UtcNow)
	        {
				if (Settings.Default.NTPTimeStampsInAAVEnabled)
				{
					ThreadPool.QueueUserWorkItem(UpdateNTPTimeReferenceForTimestampingVideo);
				}

				nextNTPTimeOneMinCheckUTC = DateTime.UtcNow.AddMinutes(1);
	        }

	        if (nextNTPSyncTime < DateTime.UtcNow)
			{
				// NOTE: Simply update the system time from 'time.windows.com' once every 30 min
				ThreadPool.QueueUserWorkItem(UpdateTimeFromNTPServer);

				nextNTPSyncTime = DateTime.UtcNow.AddMinutes(30);
			}
        }

		private void CheckAndActionScheduledEntries()
		{
			ScheduledAction actionToTake = Scheduler.CheckSchedules();

			if (actionToTake != ScheduledAction.None)
			{
				switch (actionToTake)
				{
					case ScheduledAction.StartRecording:
						if (videoObject != null && videoObject.State == VideoCameraState.videoCameraRunning)
						{
							if (OccuRecContext.Current.IsAAV && !m_StateManager.IsIntegrationLocked)
							{
								// If the integration hasn't been locked then try to lock it
								m_StateManager.LockIntegration();
							}

                            StartRecording();
						}
						break;

					case ScheduledAction.StopRecording:
						if (videoObject != null && videoObject.State == VideoCameraState.videoCameraRecording)
						{
                            StopRecording();
						}
						break;

					case ScheduledAction.AutoFocus:
						if (videoObject != null && videoObject.State == VideoCameraState.videoCameraRunning && m_ObservatoryController.IsConnectedToFocuser())
						{
							m_AnalysisManager.TriggerAutoFocusing();
							UpdateState(null);
						}
						break;

					case ScheduledAction.EnablePulseGuiding:
						m_AnalysisManager.UpdatePulseGuiding(true);
						UpdateState(null);
						break;

					case ScheduledAction.DisablePulseGuiding:
						m_AnalysisManager.UpdatePulseGuiding(false);
						UpdateState(null);
						break;
				}

				UpdateScheduleDisplay();
			}

			ScheduleEntry entry = Scheduler.GetNextEntry();
			if (entry != null)
			{
				double remainingSeconds;
				lblSecheduleWhatsNext.Text = entry.GetRemainingTime(out remainingSeconds);
				pnlNextScheduledAction.Visible = true;
			}
			else
			{
				pnlNextScheduledAction.Visible = false;
			}			
		}

	    private bool m_LastKnownGoodNTPServersInitialised = false;
	    private string[] m_LastKnownGoodNTPServers = null;

		private void UpdateNTPTimeReferenceForTimestampingVideo(object state)
		{
			// Set highest priority
			Thread.CurrentThread.Priority = ThreadPriority.Highest;
			// Then trigger a context switch
			Thread.Sleep(20);

			if (!m_LastKnownGoodNTPServersInitialised)
			{
				NTPClient.PrepareForGettingNetworkTimeFromMultipleServers();

				string[] ntpServerList = new string[] { Settings.Default.NTPServer1, Settings.Default.NTPServer2, Settings.Default.NTPServer3, Settings.Default.NTPServer4 };
                ntpServerList = ntpServerList.Cast<string>().Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
				var workingServers = new List<string>();
				for (int attempts = 0; attempts < 3; attempts++)
				{
					workingServers.Clear();
					for (int i = 0; i < 4; i++)
					{
						try
						{
							float latencyInMilliseconds;
							DateTime initialTime = NTPClient.GetNetworkTime(ntpServerList[i], true, out latencyInMilliseconds);
							NTPClient.SetTime(initialTime);
							
							Trace.WriteLine(string.Format("Latency to {0} is {1} ms.", ntpServerList[i], latencyInMilliseconds.ToString("0.0")));
							workingServers.Add(ntpServerList[i]);
						}
						catch
						{ }

						Thread.Sleep(100);
					}

					if (workingServers.Count >= 1)
						break;
				}

				m_LastKnownGoodNTPServers = workingServers.ToArray();
				m_LastKnownGoodNTPServersInitialised = m_LastKnownGoodNTPServers.Length >= 1;
			}

			if (m_LastKnownGoodNTPServers != null && m_LastKnownGoodNTPServersInitialised)
			{
				try
				{
					float latencyInMilliseconds;
					int aliveServers;
					bool timeUpdated;
					NTPClient.GetNetworkTimeFromMultipleServers(m_LastKnownGoodNTPServers, out latencyInMilliseconds, out aliveServers, out timeUpdated);
				}
                catch(LinearRegressionException)
                { }
				catch (Exception ex)
				{
					Trace.WriteLine(ex.GetFullStackTrace());
				}

				try
				{
					Invoke(new Action(UpdateNTPStatus));
				}
				catch
				{ }					
			}		
		}

        private void UpdateTimeFromNTPServer(object state)
        {
	        for (int i = 0; i < 3; i++)
	        {
				try
				{
					float latencyInMilliseconds;
					DateTime networkUTCTime = NTPClient.GetNetworkTime("time.windows.com", false, out latencyInMilliseconds);
					NTPClient.SetTime(networkUTCTime);

					break;
				}
				catch (Exception ex)
				{
					Trace.WriteLine(ex);
				}		        
	        }

        }

        private void btnClearSchedule_Click(object sender, EventArgs e)
        {
            Scheduler.ClearSchedules();
            UpdateScheduleDisplay();
        }

		#region Software Version Update

		internal delegate void OnUpdateEventDelegate(int eventCode);

		internal void OnUpdateEvent(int eventCode)
		{
			if (eventCode == MSG_ID_NEW_OCCUREC_UPDATE_AVAILABLE)
            {
                pnlNewVersionAvailable.Visible = true;
                m_ShowNegativeResultMessage = false;
            }
			else if (eventCode == MSG_ID_NO_OCCUREC_UPDATES_AVAILABLE)
            {
                if (m_ShowNegativeResultMessage)
                {
                    MessageBox.Show(
                        string.Format("There are no new {0}updates.", Settings.Default.AcceptBetaUpdates ? "beta " : ""), 
                        "Information", 
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }

                m_ShowNegativeResultMessage = false;
            }			
		}

		private bool m_ShowNegativeResultMessage = false;
		private DateTime m_LastUpdateTime;
		private Thread m_CheckForUpdatesThread;

		private byte MSG_ID_NEW_OCCUREC_UPDATE_AVAILABLE = 13;
		private byte MSG_ID_NO_OCCUREC_UPDATES_AVAILABLE = 14;

		private void miCheckForUpdates_Click(object sender, EventArgs e)
		{
			m_ShowNegativeResultMessage = true;
			CheckForUpdates(true);
		}

		public void CheckForUpdates(bool manualCheck)
		{
			if (
					m_CheckForUpdatesThread == null ||
					!m_CheckForUpdatesThread.IsAlive
				)
			{
				IntPtr handleHack = this.Handle;

				m_CheckForUpdatesThread = new Thread(new ParameterizedThreadStart(CheckForUpdates));
				m_CheckForUpdatesThread.Start(handleHack);
			}
		}

		private void CheckForUpdates(object state)
		{
			try
			{
				m_LastUpdateTime = DateTime.Now;

				int serverConfigVersion;
				if (NewUpdatesAvailable(out serverConfigVersion) != null)
				{
					Trace.WriteLine("There is a new update.", "Update");
					Invoke(new OnUpdateEventDelegate(OnUpdateEvent), MSG_ID_NEW_OCCUREC_UPDATE_AVAILABLE);
				}
				else
				{
					Trace.WriteLine(string.Format("There are no new {0}updates.", Settings.Default.AcceptBetaUpdates ? "beta " : ""), "Update");
					Invoke(new OnUpdateEventDelegate(OnUpdateEvent), MSG_ID_NO_OCCUREC_UPDATES_AVAILABLE); 
				}

				Settings.Default.LastCheckedForUpdates = m_LastUpdateTime;
				Settings.Default.Save();
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex, "Update");
			}
		}

		private string occuRecUpdateServerVersion = null;

		public XmlNode NewUpdatesAvailable(out int configUpdateVersion)
		{
			configUpdateVersion = -1;
			Uri updateUri = new Uri(UpdateManager.UpdatesXmlFileLocation);

			HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(updateUri);
			httpRequest.Method = "GET";
			httpRequest.Timeout = 30000; //30 sec

			HttpWebResponse response = null;

			try
			{
				response = (HttpWebResponse)httpRequest.GetResponse();

				string updateXml = null;

				Stream streamResponse = response.GetResponseStream();

				try
				{
					using (TextReader reader = new StreamReader(streamResponse))
					{
						updateXml = reader.ReadToEnd();
					}
				}
				finally
				{
					streamResponse.Close();
				}

				if (updateXml != null)
				{
					XmlDocument xmlDoc = new XmlDocument();
					xmlDoc.LoadXml(updateXml);

					int latestVersion = UpdateManager.CurrentlyInstalledOccuRecVersion();
                    Trace.WriteLine("Current OccuRec Version: " + latestVersion.ToString(), "Update");
					XmlNode latestVersionNode = null;

					foreach (XmlNode updateNode in xmlDoc.SelectNodes("/OccuRec/Update"))
					{
						int Version = int.Parse(updateNode.Attributes["Version"].Value);
                        Trace.WriteLine("Server OccuRec Version: " + Version.ToString(), "Update");
						if (latestVersion < Version)
						{
							Trace.WriteLine("Update location: " + updateUri.ToString());
							Trace.WriteLine("Current version: " + latestVersion.ToString());
							Trace.WriteLine("New version: " + Version.ToString());

							XmlNode occuRecUpdateNode = xmlDoc.SelectSingleNode("/OccuRec/OccuRecUpdate");
							if (occuRecUpdateNode != null)
							{
								occuRecUpdateServerVersion = occuRecUpdateNode.Attributes["Version"].Value;
								Trace.WriteLine("OccuRecUpdate new version: " + occuRecUpdateServerVersion);
							}
							else
								occuRecUpdateServerVersion = null;

							latestVersion = Version;
							latestVersionNode = updateNode;
						}
					}

					foreach (XmlNode updateNode in xmlDoc.SelectNodes("/OccuRec/ModuleUpdate[@MustExist = 'false']"))
					{
						if (updateNode.Attributes["Version"] == null) continue;

						int Version = int.Parse(updateNode.Attributes["Version"].Value);
						latestVersion = UpdateManager.CurrentlyInstalledModuleVersion(updateNode.Attributes["File"].Value);

						if (latestVersion < Version)
						{
							Trace.WriteLine("Update location: " + updateUri.ToString());
							Trace.WriteLine("Module: " + updateNode.Attributes["File"].Value);
							Trace.WriteLine("Current version: " + latestVersion.ToString());
							Trace.WriteLine("New version: " + Version.ToString());

							XmlNode occuRecUpdateNode = xmlDoc.SelectSingleNode("/OccuRec/OccuRecUpdate");
							if (occuRecUpdateNode != null)
							{
								occuRecUpdateServerVersion = occuRecUpdateNode.Attributes["Version"].Value;
								Trace.WriteLine("OccuRecUpdate new version: " + occuRecUpdateServerVersion);
							}
							else
								occuRecUpdateServerVersion = null;

							latestVersion = Version;
							latestVersionNode = updateNode;
						}
					}

					XmlNode cfgUpdateNode = xmlDoc.SelectSingleNode("/OccuRec/ConfigurationUpdate");
					if (cfgUpdateNode != null)
					{
						XmlNode cfgUpdVer = cfgUpdateNode.Attributes["Version"];
						if (cfgUpdVer != null)
						{
							configUpdateVersion = int.Parse(cfgUpdVer.InnerText);
						}
					}


					return latestVersionNode;
				}
			}
			finally
			{
				// Close response
				if (response != null)
					response.Close();
			}

			return null;
		}

		public void ForceUpdateSynchronously()
		{
			RunOccuRecUpdater();
		}

		private void pnlNewVersionAvailable_Click(object sender, EventArgs e)
		{
			pnlNewVersionAvailable.Enabled = false;
			pnlNewVersionAvailable.IsLink = false;
			pnlNewVersionAvailable.Tag = pnlNewVersionAvailable.Text;
			pnlNewVersionAvailable.Text = "Update started ...";
			statusStrip.Update();

			RunOccuRecUpdater();

			Close();
		}

		private void RunOccuRecUpdater()
		{
			try
			{
				string currentPath = AppDomain.CurrentDomain.BaseDirectory;
				string updaterFileName = Path.GetFullPath(currentPath + "\\OccuRecUpdate.zip");

				int currUpdVer = UpdateManager.CurrentlyInstalledOccuRecUpdateVersion();
				int servUpdVer = int.Parse(occuRecUpdateServerVersion);
				if (!File.Exists(Path.GetFullPath(currentPath + "\\OccuRecUpdate.exe")) || /* If there is no OccuRecUpdate.exe*/
					(
						statusStrip.Tag != null &&  /* Or it is an older version ... */
						servUpdVer > currUpdVer)
					)
				{
					if (servUpdVer > currUpdVer)
						Trace.WriteLine(string.Format("Update required for 'OccuRecUpdate.exe': local version: {0}; server version: {1}", currUpdVer, servUpdVer));

					Uri updateUri = new Uri(UpdateManager.UpdateLocation + "/OccuRecUpdate.zip");

					HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(updateUri);
					httpRequest.Method = "GET";
					httpRequest.Timeout = 1200000; //1200 sec = 20 min

					HttpWebResponse response = null;

					try
					{
						response = (HttpWebResponse)httpRequest.GetResponse();

						Stream streamResponse = response.GetResponseStream();

						try
						{
							try
							{
								if (File.Exists(updaterFileName))
									File.Delete(updaterFileName);

								using (BinaryReader reader = new BinaryReader(streamResponse))
								using (BinaryWriter writer = new BinaryWriter(new FileStream(updaterFileName, FileMode.Create)))
								{
									byte[] chunk = null;
									do
									{
										chunk = reader.ReadBytes(1024);
										writer.Write(chunk);
									}
									while (chunk != null && chunk.Length == 1024);

									writer.Flush();
								}
							}
							catch (UnauthorizedAccessException uex)
							{
								MessageBox.Show(this, uex.Message + "\r\n\r\nYou may need to run OccuRec as administrator to complete the update.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
							}
							catch (Exception ex)
							{
								Trace.WriteLine(ex);
							}

							if (File.Exists(updaterFileName))
							{
								string tempOutputDir = Path.ChangeExtension(Path.GetTempFileName(), "");
								Directory.CreateDirectory(tempOutputDir);
								try
								{
									string zippedFileName = updaterFileName;
									ZipUnZip.UnZip(updaterFileName, tempOutputDir, true);
									string[] files = Directory.GetFiles(tempOutputDir);
									updaterFileName = Path.ChangeExtension(updaterFileName, ".exe");
									System.IO.File.Copy(files[0], updaterFileName, true);
									System.IO.File.Delete(zippedFileName);
								}
								finally
								{
									Directory.Delete(tempOutputDir, true);
								}
							}
						}
						finally
						{
							streamResponse.Close();
						}
					}
					catch (WebException)
					{
						MessageBox.Show("There was an error trying to download the OccuRecUpdate program. Please ensure that you have an active internet connection and try again later.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
					finally
					{
						// Close response
						if (response != null)
							response.Close();
					}
				}

				// Make sure after the update is completed a new check is done. 
				// This will check for Add-in updates 
				Settings.Default.LastCheckedForUpdates = DateTime.Now.AddDays(-15);
				Settings.Default.Save();

				updaterFileName = Path.GetFullPath(currentPath + "\\OccuRecUpdate.exe");

				if (File.Exists(updaterFileName))
				{
					var processInfo = new ProcessStartInfo();

					if (System.Environment.OSVersion.Version.Major > 5)
						// UAC Elevate as Administrator for Windows Vista, Win7 and later
						processInfo.Verb = "runas";

					processInfo.FileName = updaterFileName;
                    processInfo.Arguments = string.Format("{0}", Settings.Default.AcceptBetaUpdates ? "beta" : "full");
					Process.Start(processInfo);
				}
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex.GetFullStackTrace(), "Update");
				pnlNewVersionAvailable.Enabled = true;
			}
		}
		#endregion

		private void miHelpIndex_Click(object sender, EventArgs e)
		{
			Process.Start("http://www.hristopavlov.net/OccuRec");
		}

		private void miYahooGroup_Click(object sender, EventArgs e)
		{
            Process.Start("https://groups.yahoo.com/neo/groups/OccuRec/info");
		}

		private void miOpenFile_Click(object sender, EventArgs e)
		{
			if (videoObject == null)
			{
				if (openFileDialog.ShowDialog(this) == DialogResult.OK)
				{
					Settings.Default.SimulatorFilePath = openFileDialog.FileName;
				    Settings.Default.OcrSimulatorTestMode = false;
                    Settings.Default.OcrCameraAavTestMode = false;
                    Settings.Default.SimulatorRunOCR = false;
					Settings.Default.Save();

					IVideo driverInstance = new Drivers.AVISimulator.Video(true); 

	                ConnectToDriver(driverInstance);
				}
			}
        }

		private void btnCalibrateIntegration_Click(object sender, EventArgs e)
		{
			if (m_StateManager.IsCalibratingIntegration)
			{
				m_StateManager.CancelIntegrationCalibration(true);
			}
			else if (
				videoObject != null &&
				MessageBox.Show(
					this, 
					string.Format("Put your integrating camera in x{0} video frames integraton mode and press 'OK' to start the calibration process.", Settings.Default.CalibrationIntegrationRate), 
					"Integration Detection Calibration", 
					MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.OK)
			{
				m_StateManager.BeginIntegrationCalibration(4);
			}
		}

	    private int m_ManualIntegration = -1;

		private void btnManualIntegration_Click(object sender, EventArgs e)
		{
			if (m_StateManager.IsUsingManualIntegration)
			{
				NativeHelpers.SetManualIntegrationRateHint(0);
				m_ManualIntegration = -1;
			}
			else
			{
				var frm = new frmChooseManualIntegrationRate();
				if (frm.ShowDialog(this) == DialogResult.OK)
				{
					int manualIntegrationRate = (int)frm.nudIntegrationRate.Value;
					NativeHelpers.SetManualIntegrationRateHint(manualIntegrationRate);
					m_ManualIntegration = manualIntegrationRate;
				}
			}

			// Reinitialize so the integration detection stats (previously detected integration) are reset 
			m_StateManager.ReInitializeState();
		}

		private void frmMain_Load(object sender, EventArgs e)
		{

            if (Settings.Default.WarnForFileSystemIssues)
			{
			    if (Directory.Exists(Settings.Default.OutputLocation))
			    {
                    if (Settings.Default.WarnOnFreeDiskSpaceLeft)
                    {
                        ulong freeBytes;
                        NativeHelpers.GetDriveFreeBytes(Settings.Default.OutputLocation, out freeBytes);

                        string directoryRoot = Directory.GetDirectoryRoot(Settings.Default.OutputLocation);

                        if (freeBytes < ((ulong)1024 * (ulong)1024 * (ulong)1024 * (ulong)Settings.Default.WarnMinDiskFreeSpaceGb))
                        {
                            MessageBox.Show(
                                string.Format("There is only {0:0.0} Gb left on drive {1}\r\n\r\nThere may not be enough disk space to record a video!",
                                1.0 * freeBytes / (1024 * 1024 * 1024),
                                directoryRoot),
                                "OccuRec",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                        }
                    }

                    if (Settings.Default.WarnOnFAT16Usage)
                        FileNameGenerator.CheckAndWarnForFileSystemLimitation();
                }
                else
                {
                    MessageBox.Show(
                        string.Format("Configured output video location '{0}' is incorrect or is not accessible!", Settings.Default.OutputLocation),
                        "OccuRec",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }

            PowerManagement.CheckPowerSettings(this);

            int currVersion = UpdateManager.CurrentlyInstalledOccuRecVersion();
            if (Settings.Default.ReleaseNotesDisplayedForVersion < currVersion)
            {
                DisplayReleaseNotes();

                Settings.Default.ReleaseNotesDisplayedForVersion = currVersion;
                Settings.Default.Save();
            }
		}

		private void pictureBox_MouseMove(object sender, MouseEventArgs e)
		{
			if (m_OverlayManager != null)
				m_OverlayManager.MouseMove(e);
		}

		private void pictureBox_MouseLeave(object sender, EventArgs e)
		{
			if (m_OverlayManager != null)
				m_OverlayManager.MouseLeave(e);
		}

		private void pictureBox_MouseDown(object sender, MouseEventArgs e)
		{
			if (m_OverlayManager != null)
				m_OverlayManager.MouseDown(e);
		}

		private void pictureBox_MouseUp(object sender, MouseEventArgs e)
		{
			if (m_OverlayManager != null)
				m_OverlayManager.MouseUp(e);
		}

        private void RefreshASCOMStatusControls(ASCOMConnectionState state, ToolStripStatusLabel label)
        {
            switch (state)
            {
                case ASCOMConnectionState.Disconnected:
                    label.Visible = false;
                    break;
                case ASCOMConnectionState.Connecting:
				case ASCOMConnectionState.Engaged:
                    label.ForeColor = Color.Goldenrod;
                    label.Visible = true;
                    break;
                case ASCOMConnectionState.Connected:
				case ASCOMConnectionState.Ready:
                    label.ForeColor = Color.Green;
                    label.Visible = true;
                    break;				
                case ASCOMConnectionState.Disconnecting:
                    label.ForeColor = Color.SlateGray;
                    label.Visible = true;
                    break;
                case ASCOMConnectionState.NotResponding:
                    label.ForeColor = Color.Black;
                    label.Visible = true;
                    break;

                case ASCOMConnectionState.Errored:
                    label.ForeColor = Color.Red;
                    label.Visible = true;
                    break;
            }

			tsbTelControl.Visible = ObservatoryController.IsASCOMPlatformInstalled;
			tsbFocControl.Visible = ObservatoryController.IsASCOMPlatformInstalled;
			tsbCamControl.Visible = m_ObservatoryController.HasVideoCamera || ObservatoryController.IsASCOMPlatformVideoAvailable;

	        UpdateASCOMConnectivityState();
        }

        public void TelescopeConnectionChanged(ASCOMConnectionState state)
        {
            RefreshASCOMStatusControls(state, tssASCOMTelescope);
            if (state == ASCOMConnectionState.Connected || state == ASCOMConnectionState.Ready)
            {
                tsbTelControl.Text = "Control Telescope";
                tsbTelControl.Enabled = true;
            }
			else if (state == ASCOMConnectionState.Disconnected || state == ASCOMConnectionState.Engaged)
            {
                tsbTelControl.Text = "Connect to Telescope";
                tsbTelControl.Enabled = true;
                m_LastTelescopeState = null;
                UpdateConnectedDevicesState();
            }
            else
            {
                tsbTelControl.Enabled = false;
            }
        }

        public void FocuserConnectionChanged(ASCOMConnectionState state)
        {
            RefreshASCOMStatusControls(state, tssASCOMFocuser);
			if (state == ASCOMConnectionState.Connected || state == ASCOMConnectionState.Ready)
            {
                tsbFocControl.Text = "Control Focuser";
                tsbFocControl.Enabled = true;
            }
			else if (state == ASCOMConnectionState.Disconnected || state == ASCOMConnectionState.Engaged)
            {
                tsbFocControl.Text = "Connect to Focuser";
                tsbFocControl.Enabled = true;
                m_LastFocuserState = null;
                UpdateConnectedDevicesState();
            }
            else
            {
                tsbFocControl.Enabled = false;
            }
        }

		void VideoConnectionChanged(ASCOMConnectionState state)
		{
			RefreshASCOMStatusControls(state, tssCameraControl);
			if (state == ASCOMConnectionState.Connected || state == ASCOMConnectionState.Ready)
			{
                tsbCamControl.Text = "Control Camera";
				tsbCamControl.Enabled = true;
			}
			else if (state == ASCOMConnectionState.Disconnected || state == ASCOMConnectionState.Engaged)
			{
                tsbCamControl.Text = "Connect to Camera";
				tsbCamControl.Enabled = true;
				m_LastVideoCameraState = null;
			}
			else
			{
				tsbCamControl.Enabled = false;
			}
		}

        public void TelescopeStateUpdated(TelescopeState state)
        {
            m_LastTelescopeState = state;
            UpdateConnectedDevicesState();

#if DEBUG
            Trace.WriteLine(state.AsSerialized().OuterXml);
#endif
        }

        public void FocuserStateUpdated(FocuserState state)
        {
            m_LastFocuserState = state;
            UpdateConnectedDevicesState();

#if DEBUG
            Trace.WriteLine(state.AsSerialized().OuterXml);
#endif
        }

        public void FocuserPositionUpdated(FocuserPosition position)
        {

        }

		void VideoStateUpdated(VideoState state)
		{
			m_LastVideoCameraState = state;
		    UpdateConnectedDevicesState();

#if DEBUG
			Trace.WriteLine(state.AsSerialized().OuterXml);
#endif
		}

		void VideoError(string obj)
		{
			RefreshASCOMStatusControls(ASCOMConnectionState.Errored, tssCameraControl);
		}

        private TelescopeState m_LastTelescopeState = null;
        private FocuserState m_LastFocuserState = null;
	    private VideoState m_LastVideoCameraState = null;

        private void UpdateConnectedDevicesState()
        {
            string stateStr = string.Empty;

            if (m_LastFocuserState != null && !double.IsNaN(m_LastFocuserState.Temperature) && m_LastTelescopeState != null)
                stateStr = string.Format("Alt: {0}°| Az: {1}° | Temp: {2}° {3}", (int)Math.Round(m_LastTelescopeState.Altitude), (int)Math.Round(m_LastTelescopeState.Azimuth), m_LastFocuserState.Temperature.ToString("0.0"), Settings.Default.FocuserTemperatureIn);
            else if (m_LastTelescopeState != null)
                stateStr = string.Format("Alt: {0}° | Az: {1}°", (int)Math.Round(m_LastTelescopeState.Altitude), (int)Math.Round(m_LastTelescopeState.Azimuth));
            else if (m_LastFocuserState != null && !double.IsNaN(m_LastFocuserState.Temperature))
                stateStr = string.Format("Temp: {0}° {1}", m_LastFocuserState.Temperature.ToString("0.0"), Settings.Default.FocuserTemperatureIn);
            else
                stateStr = string.Empty;

            if (m_LastVideoCameraState != null)
            {
                string cameraState = string.Format("{0} ({1}, {2})", m_LastVideoCameraState.Exposure, m_LastVideoCameraState.Gain, m_LastVideoCameraState.Gamma);
                if (string.IsNullOrWhiteSpace(stateStr))
                    stateStr += cameraState;
                else
                    stateStr += " | " + cameraState;
            }

            tslTelFocStatus.Text = stateStr;
        }

		private void DetectionTesting()
		{
			Regex rex = new Regex("FRID:(\\d+).*DF:([0-9\\.]+)");

			NativeHelpers.InitIntegrationDetectionTesting(0.269f, 0.269f);
			string[] fileLines = File.ReadAllLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "INTLOG.LOG"));
			foreach (string line in fileLines)
			{
				Match match = rex.Match(line);
				if (match.Success)
				{
					int frameNo = int.Parse(match.Groups[1].Value);
					float diff = float.Parse(match.Groups[2].Value);
					NativeHelpers.IntegrationDetectionTestNextFrame(frameNo, diff);
				}
			}			
		}

		private static frmCameraControl s_FormCameraControl = null;

		private void tsbCamControl_Click(object sender, EventArgs e)
		{
			if (!m_ObservatoryController.IsConnectedToVideoCamera())
			{
				m_ObservatoryController.TryConnectVideoCamera();
				tsbCamControl.Enabled = false;
			}
			else
			{
				if (s_FormCameraControl != null)
				{
					try
					{
						if (!s_FormCameraControl.Visible)
							s_FormCameraControl.Show(this);
					}
					catch (Exception ex)
					{
						s_FormCameraControl = null;
					}
				}

				if (s_FormCameraControl == null)
				{
					s_FormCameraControl = new frmCameraControl();
					s_FormCameraControl.ObservatoryController = m_ObservatoryController;
					s_FormCameraControl.Show(this);
				}
			}

			if (s_FormCameraControl != null && s_FormCameraControl.Visible)
			{
				s_FormCameraControl.Left = this.Right;
				s_FormCameraControl.Top = this.Top;
			}
		}

	    private static frmFocusControl s_FormFocuserControl = null;

		private void tsbFocControl_Click(object sender, EventArgs e)
		{
		    if (!m_ObservatoryController.IsConnectedToFocuser())
		    {
		        m_ObservatoryController.TryConnectFocuser();
                tsbFocControl.Enabled = false;
		    }
		    else
		    {
                if (s_FormFocuserControl != null)
                {
                    try
                    {
                        if (!s_FormFocuserControl.Visible)
                            s_FormFocuserControl.Show(this);
                    }
                    catch (Exception ex)
                    {
                        s_FormFocuserControl = null;
                    }
                }

                if (s_FormFocuserControl == null)
                {
                    s_FormFocuserControl = new frmFocusControl();
                    s_FormFocuserControl.ObservatoryController = m_ObservatoryController;
                    s_FormFocuserControl.Show(this);
                }
		    }

			if (s_FormFocuserControl != null && s_FormFocuserControl.Visible)
			{
				s_FormFocuserControl.Left = this.Right;
				s_FormFocuserControl.Top = this.Top;
			}
		}

        private static frmTelescopeControl s_FormTelescopeControl = null;

        private void tsbTelControl_Click(object sender, EventArgs e)
        {
            if (!m_ObservatoryController.IsConnectedToTelescope())
            {
                m_ObservatoryController.TryConnectTelescope();
                tsbTelControl.Enabled = false;
            }
            else
            {
                if (s_FormTelescopeControl != null)
                {
                    try
                    {
                        if (!s_FormTelescopeControl.Visible)
                            s_FormTelescopeControl.Show(this);
                    }
                    catch (Exception ex)
                    {
                        s_FormTelescopeControl = null;
                    }
                }

                if (s_FormTelescopeControl == null)
                {
                    s_FormTelescopeControl = new frmTelescopeControl(m_AnalysisManager);
                    s_FormTelescopeControl.ObservatoryController = m_ObservatoryController;
                    s_FormTelescopeControl.Show(this);
                }
            }

			if (s_FormTelescopeControl != null && s_FormTelescopeControl.Visible)
			{
				s_FormTelescopeControl.Left = this.Right;
				s_FormTelescopeControl.Top = this.Top;
			}
        }

        private void tsbAddGuidingStar_Click(object sender, EventArgs e)
        {
            m_VideoFrameInteractionController.ToggleSelectGuidingStar();
        }

        private void tbsAddTarget_Click(object sender, EventArgs e)
        {
            m_VideoFrameInteractionController.ToggleSelectTargetStar();
        }

		private void tsbClearTargets_Click(object sender, EventArgs e)
		{
			m_VideoFrameInteractionController.RemoveTrackedObjects();
		}

        private void tsbInsertSpectra_Click(object sender, EventArgs e)
        {
            m_VideoFrameInteractionController.ToggleInsertStarSpectra();
        }

		private void UpdateNTPConnectivityState()
		{
			UpdateNTPStatus();
		}

        private void UpdateASCOMConnectivityState()
        {
            tsbFocControl.Enabled = !string.IsNullOrEmpty(Settings.Default.ASCOMProgIdFocuser);
            tsbTelControl.Enabled = !string.IsNullOrEmpty(Settings.Default.ASCOMProgIdTelescope);
	        tsbCamControl.Enabled = m_ObservatoryController.HasVideoCamera;
        }

	    private void UpdateObservingAidControls()
	    {
		    if (Settings.Default.SpectraUseAid)
		    {
				if (videoObject != null)
					tbsInsertSpectra.Visible = true;
		    }
		    else
		    {
			    tbsInsertSpectra.Visible = false;
				m_VideoFrameInteractionController.RemoveTrackedObjects();
		    }

            if (Settings.Default.EnableBadPixelsControl)
            {
                if (videoObject != null)
                    tsbAddBadPixelMarkers.Enabled = true;
                    tsbAddBadPixelMarkers.Visible = true;
            }
            else
            {
                tsbAddBadPixelMarkers.Enabled = false;
                tsbAddBadPixelMarkers.Visible = false;
            }

        }

		private void frmMain_Shown(object sender, EventArgs e)
		{
			if (!Settings.Default.MPLv2LicenseAgreementAccepted)
			{
				var frm = new frmLicenseAgreement();
				frm.StartPosition = FormStartPosition.CenterParent;
				if (frm.ShowDialog(this) == DialogResult.OK)
				{
					Settings.Default.MPLv2LicenseAgreementAccepted = true;
					Settings.Default.Save();
				}
				else
					Application.Exit();
			}
		}

		private void fileToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
		{
			miASCOMConnect.Enabled = ObservatoryController.IsASCOMPlatformInstalled &&
			                         ObservatoryController.IsASCOMPlatformVideoAvailable;
		}

		private void btnUpdateVtiOsd_Click(object sender, EventArgs e)
		{
			var frm = new frmConfigureVtiOsdLines();
			frm.SetFrameHeight(m_VideoRenderingController.Height);
			frm.ShowDialog(this);
		}

		private void btnConfirmUserVtiOsd_Click(object sender, EventArgs e)
		{
			NativeHelpers.SetupTimestampPreservation(true, Settings.Default.PreserveVTIFirstRow, Settings.Default.PreserveVTILastRow - Settings.Default.PreserveVTIFirstRow);

			// Keep showing the AssumedVtiOsd lines for another 5 sec for user's visual confirmation that they are correct
			OccuRecContext.Current.ShowAssumedVtiOsdPositionUntil = DateTime.Now.AddSeconds(5);

			m_StateManager.ChangeState(UndeterminedIntegrationCameraState.Instance);
		}

		private void frmMain_Move(object sender, EventArgs e)
		{
			Form[] satelliteForms = new Form[] { s_FormCameraControl, s_FormFocuserControl, s_FormTelescopeControl };

			foreach (Form frm in satelliteForms)
			{
				if (frm != null &&
					frm.Visible)
				{
					frm.Top = this.Top;
					frm.Left = this.Right;
				}				
			}
		}

		private void miASCOMConnect_Click(object sender, EventArgs e)
		{
            string progId = ASCOMClient.Instance.ChooseVideo();

            if (!string.IsNullOrEmpty(progId))
            {
               var driverInstance = new Drivers.ASCOMVideo.Video(progId);
               ConnectToDriver(driverInstance);
            }	
		}

		private void DisplayIntensifyModeClicked(object sender, EventArgs e)
		{
			var currItem = sender as ToolStripMenuItem;
			if (currItem != null && !currItem.Checked)
			{
				tsmiOff.Checked = false;
				tsmiLo.Checked = false;
				tsmiHigh.Checked = false;

				currItem.Checked = true;

				DisplayIntensifyMode newMode = tsmiOff.Checked
												   ? DisplayIntensifyMode.Off
												   : (tsmiHigh.Checked ? DisplayIntensifyMode.Hi : DisplayIntensifyMode.Lo);

			    m_VideoRenderingController.SetDisplayIntensifyMode(newMode);
			}
		}

		private void tsmiHueIntensity_Click(object sender, EventArgs e)
		{
			m_VideoRenderingController.SetDisplayHueMode(tsmiHueIntensity.Checked);
		}

		private void tsmiInverted_Click(object sender, EventArgs e)
		{
			m_VideoRenderingController.SetDisplayInvertMode(tsmiInverted.Checked);
		}


        private void tsmiSaturationCheck_Click(object sender, EventArgs e)
        {
            m_VideoRenderingController.SetDisplaySaturationMode(tsmiSaturation.Checked);
        }

		private void btnOneStack_Click(object sender, EventArgs e)
		{
			var frm = new frmOneStacking();
			frm.FrameRate = videoObject.FrameRate;
			frm.SetCurrentStackingRate(NativeHelpers.GetStackingRate());
			DialogResult result = frm.ShowDialog(this);
			if (result == DialogResult.OK)
			{
				if (frm.StackRate == 0)
				{
					lblOneStack.Text = "No Stacking";
					NativeHelpers.SetStackRate(0);
				}
				else
				{
					lblOneStack.Text = string.Format("Stack of {0}", frm.StackRate);
					NativeHelpers.SetStackRate(frm.StackRate);
				}
			}
			else if (result == DialogResult.Ignore)
			{
				lblOneStack.Text = "No Stacking";
				NativeHelpers.SetStackRate(0);				
			}
		}

        private void miAbout_Click(object sender, EventArgs e)
        {
            var frm = new frmAbout();
            frm.StartPosition = FormStartPosition.CenterParent;
            frm.ShowDialog(this);
        }

        private void btnNoOSD_Click(object sender, EventArgs e)
        {
            NativeHelpers.SetupTimestampPreservation(false, imageHeight - 2, 2);
            NativeHelpers.DisableOcr();

            m_StateManager.ChangeState(UndeterminedIntegrationCameraState.Instance);
        }

        private void miConnectQHYCCD_Click(object sender, EventArgs e)
        {
            var frm = new frmChooseQHYCamera();
            frm.StartPosition = FormStartPosition.CenterParent;
            if (frm.ShowDialog(this) == DialogResult.OK)
            {
                OccuRecContext.Current.IsQHY = true;

                var driverInstance = new Drivers.QHYVideo.Video(frm.CameraId, frm.BinningMode, frm.BPP, frm.UseGPS, frm.UseCooling);

                //m_ObservatoryController.SetExternalCameraDriver(driverInstance);
                VideoConnectionChanged(ASCOMConnectionState.Disconnected);

                ConnectToDriver(driverInstance);
            }
        }

        private void miReleaseNotes_Click(object sender, EventArgs e)
        {
            DisplayReleaseNotes();
        }

        private void DisplayReleaseNotes()
        {
            string filePath = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + @"\ReleaseNotes.txt");
            if (File.Exists(filePath))
            {
                try
                {
                    Process.Start(filePath);
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.GetFullStackTrace());
                }
            }
            else
                MessageBox.Show("Could not find: " + filePath, "Tangra", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private System.Timers.Timer m_PerfCntTimer;

        static PerformanceCounter s_PerfCntDisk = new PerformanceCounter("PhysicalDisk", "% Disk Time", "_Total");
        static PerformanceCounter s_PerfCntCpu = new PerformanceCounter("Processor Information", "% Processor Time", "_Total");
        static PerformanceCounter s_PerfCntMemory = new PerformanceCounter("Memory", "Available MBytes", null);

        private void InitPerfCounterMonitoring()
        {
            m_PerfCntTimer = new System.Timers.Timer(1000);

            m_PerfCntTimer.Elapsed += OnTimedEvent;
            m_PerfCntTimer.AutoReset = true;
            m_PerfCntTimer.Enabled = true;
        }

        private void StopPerfCounterMonitoring()
        {
            m_PerfCntTimer.Stop();
            m_PerfCntTimer.Dispose();
        }

        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            var diskUsage = (byte)Math.Round(s_PerfCntDisk.NextValue());
            var cpuUsage = (byte)Math.Round(s_PerfCntCpu.NextValue());
            var freeMemoryMB = (int)s_PerfCntMemory.NextValue();

            NativeHelpers.SetSystemPerformanceValues(cpuUsage, diskUsage, freeMemoryMB);
        }

        private void restartRecTimer_Tick(object sender, EventArgs e)
        {
            videoObject.StopRecording();
            OccuRecSelfTraceListener.Instance.SaveLog(Path.ChangeExtension(recordingfileName, "log"));

            string fileName = FileNameGenerator.GenerateFileName(OccuRecContext.Current.IsAAV);
            recordingfileName = videoObject.StartRecording(fileName);
        }

        private void tsbAddBadPixelMarkers_Click(object sender, EventArgs e)
        {
            // toggle the state
            Settings.Default.DisplayBadPixelsMarkers = !Settings.Default.DisplayBadPixelsMarkers;

            if (Settings.Default.DisplayBadPixelsMarkers)
            {
                // turn off so the bad pixels aren't displayed until the preparation has been finished
                Settings.Default.DisplayBadPixelsMarkers = false;

                // check the bad pixel file still exists (in case it was moved/deleted since it was selected in the settings form by the user)
                if (!File.Exists(Settings.Default.BadPixelsFileName))
                {
                    string caption = "Bad Pixels File Missing";
                    string message = "The file containing the bad pixels specified in the OccuRec Settings>Observing Aids>Bad Pixels form no longer exists so OccuRec can't overlay any bad pixel markers.";
                    MessageBoxButtons messageBoxButtons = MessageBoxButtons.OK;
                    MessageBoxIcon messageBoxIcon = MessageBoxIcon.Warning;
                    MessageBox.Show(message, caption, messageBoxButtons, messageBoxIcon);
                    tsbAddBadPixelMarkers.Enabled = false;
                    tsbAddBadPixelMarkers.Visible = false;
                    Settings.Default.EnableBadPixelsControl = false;
                    return;
                }

                s_BadPixels.Clear();
                try
                {
                    StreamReader streamReader = new StreamReader(Settings.Default.BadPixelsFileName);
                    using (streamReader)
                    {
                        string line;
                        while ((line = streamReader.ReadLine()) != null)
                        {
                            try
                            {
                                string[] coordinatesString = line.Split(',');
                                int x = Convert.ToInt32(coordinatesString[0]);
                                int y = Convert.ToInt32(coordinatesString[1]);

                                s_BadPixels.Add(new Point(x, y));
                            }
                            catch (FormatException)
                            {
                                string caption = "Bad Pixels File: Format Exception";
                                string message = String.Format("Failed to convert the line '{0}' to pixel coordinates as it was in the wrong format", line);
                                MessageBoxButtons messageBoxButtons = MessageBoxButtons.OK;
                                MessageBoxIcon messageBoxIcon = MessageBoxIcon.Warning;
                                MessageBox.Show(message, caption, messageBoxButtons, messageBoxIcon);
                                tsbAddBadPixelMarkers.Enabled = false;
                                tsbAddBadPixelMarkers.Visible = false;
                                Settings.Default.EnableBadPixelsControl = false;
                                return;
                            }
                            catch (IndexOutOfRangeException)
                            {
                                string caption = "Bad Pixels File: Missing Coordinate";
                                string message = String.Format("Failed to convert the line '{0}' to pixel coordinates as it was in the wrong format", line);
                                MessageBoxButtons messageBoxButtons = MessageBoxButtons.OK;
                                MessageBoxIcon messageBoxIcon = MessageBoxIcon.Warning;
                                MessageBox.Show(message, caption, messageBoxButtons, messageBoxIcon);
                                tsbAddBadPixelMarkers.Enabled = false;
                                tsbAddBadPixelMarkers.Visible = false;
                                Settings.Default.EnableBadPixelsControl = false;
                                return;
                            }
                        }
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    string caption = "Bad Pixels File: Unauthorised Access";
                    string message = "For some reason Windows won't allow OccuRec to access the Bad Pixels file.";
                    MessageBoxButtons messageBoxButtons = MessageBoxButtons.OK;
                    MessageBoxIcon messageBoxIcon = MessageBoxIcon.Warning;
                    MessageBox.Show(message, caption, messageBoxButtons, messageBoxIcon);
                    tsbAddBadPixelMarkers.Enabled = false;
                    tsbAddBadPixelMarkers.Visible = false;
                    Settings.Default.EnableBadPixelsControl = false;
                    return;
                }
                catch (IOException)
                {
                    string caption = "Bad Pixels File: IO error";
                    string message = "Something went wrong with reading the Bad Pixels file.";
                    MessageBoxButtons messageBoxButtons = MessageBoxButtons.OK;
                    MessageBoxIcon messageBoxIcon = MessageBoxIcon.Warning;
                    MessageBox.Show(message, caption, messageBoxButtons, messageBoxIcon);
                    tsbAddBadPixelMarkers.Enabled = false;
                    tsbAddBadPixelMarkers.Visible = false;
                    Settings.Default.EnableBadPixelsControl = false;
                    return;
                }

                // check there has been at least one successful bad pixel read from the file
                if (s_BadPixels.Count < 1)
                {
                    string caption = "Bad Pixels File is empty";
                    string message = "The bad pixels file doesn't have the coordinates of any bad pixels in it.";
                    MessageBoxButtons messageBoxButtons = MessageBoxButtons.OK;
                    MessageBoxIcon messageBoxIcon = MessageBoxIcon.Warning;
                    MessageBox.Show(message, caption, messageBoxButtons, messageBoxIcon);
                    tsbAddBadPixelMarkers.Enabled = false;
                    tsbAddBadPixelMarkers.Visible = false;
                    Settings.Default.EnableBadPixelsControl = false;
                    return;
                }

                foreach (Point badPixel in s_BadPixels)
                {
                    if (badPixel.X < 0 || badPixel.Y < 0 || badPixel.X > imageWidth || badPixel.Y > imageHeight)
                    {
                        string caption = "Bad Pixels File: invalid coordinates";
                        string message = String.Format("The bad pixels file has some coordinates ({0},{1}) outside the image.", badPixel.X, badPixel.Y);
                        MessageBoxButtons messageBoxButtons = MessageBoxButtons.OK;
                        MessageBoxIcon messageBoxIcon = MessageBoxIcon.Warning;
                        MessageBox.Show(message, caption, messageBoxButtons, messageBoxIcon);
                        tsbAddBadPixelMarkers.Enabled = false;
                        tsbAddBadPixelMarkers.Visible = false;
                        Settings.Default.EnableBadPixelsControl = false;
                        return;
                    }
                }

                tsbAddBadPixelMarkers.ToolTipText = "Stop overlaying the bad pixels locations on the video";
                tsbAddBadPixelMarkers.Image = imageListToolbar.Images[3];

                // turn back on now that the preparation has been finished
                Settings.Default.DisplayBadPixelsMarkers = true;
            }
            else
            {
                s_BadPixels.Clear();

                tsbAddBadPixelMarkers.ToolTipText = "Overlay the bad pixels locations on the video";
                tsbAddBadPixelMarkers.Image = imageListToolbar.Images[2];
            }
        }
    }
}
