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
using System.Windows.Forms;
using System.Xml;
using OccuRec.ASCOM.Wrapper;
using OccuRec.ASCOM;
using OccuRec.ASCOM.Interfaces.Devices;
using OccuRec.CameraDrivers;
using OccuRec.Config;
using OccuRec.Controllers;
using OccuRec.Drivers;
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
			m_ObservatoryController.VideoStateUpdated += VideoStateUpdated;
			m_ObservatoryController.VideoError += VideoError;

			m_AnalysisManager = new FrameAnalysisManager(m_ObservatoryController);
            m_VideoRenderingController = new VideoRenderingController(this, m_StateManager, m_AnalysisManager);
            m_VideoFrameInteractionController = new VideoFrameInteractionController(this, m_VideoRenderingController);

			Version att = Assembly.GetExecutingAssembly().GetName().Version;
			appVersion = string.Format("{0}.{1}.{2}", att.Major, att.Minor, att.MinorRevision);

#if BETA
            appVersion = string.Concat(appVersion, " [BETA]");
#endif

            Text = string.Format("OccuRec v{0}", appVersion);

            CheckForUpdates(false);

		    UpdateState(null);

			ASCOMClient.Instance.Initialise(Settings.Default.ASCOMLoadInSeparateAppDomain);
			TelescopeConnectionChanged(ASCOMConnectionState.Disconnected);
			FocuserConnectionChanged(ASCOMConnectionState.Disconnected);
			VideoConnectionChanged(ASCOMConnectionState.Disconnected);

		    UpdateASCOMConnectivityState();
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
		}

        public void OnError(int errorCode, string errorMessage)
        {
            if (m_OverlayManager != null)
                m_OverlayManager.OnError(errorCode, errorMessage);
            else
                initializationErrorMessages.Add(errorMessage);
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
            if (chooser.ShowDialog(this) == DialogResult.OK)
            {
                if (!string.IsNullOrEmpty(Settings.Default.FileFormat))
                {
                    IVideo driverInstance;
                    if (Settings.Default.FileSimulation)
                    {
                        bool fullAAVSimulation = Settings.Default.FileFormat == "AAV";
                        if (".avi".Equals(Path.GetExtension(Settings.Default.SimulatorFilePath), StringComparison.InvariantCultureIgnoreCase))
                            driverInstance = new Drivers.AVISimulator.Video(fullAAVSimulation);
                        else
                            driverInstance = new Drivers.AAVSimulator.Video(fullAAVSimulation);
                    }
					else if (Settings.Default.FileFormat == "AAV")
					{
						driverInstance = new Drivers.AAVTimer.Video();
						if (chooser.CameraControlDriver != null)
						{
							m_ObservatoryController.SetExternalCameraDriver(chooser.CameraControlDriver);
							VideoConnectionChanged(ASCOMConnectionState.Disconnected);
						}
					}
					else if (Settings.Default.FileFormat == "AVI")
						driverInstance = new Drivers.DirectShowCapture.Video();
					else
						throw new NotSupportedException();

	                ConnectToDriver(driverInstance);
                }                
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
					tssIntegrationRate.Visible = Settings.Default.IsIntegrating && Settings.Default.FileFormat == "AAV";
					pnlAAV.Visible = Settings.Default.FileFormat == "AAV";

					m_OverlayManager = new OverlayManager(videoObject.Width, videoObject.Height, initializationErrorMessages, m_AnalysisManager);
					m_VideoFrameInteractionController.OnNewVideoSource(videoObject);

					if (Settings.Default.RecordStatusSectionOnly)
						MessageBox.Show(
							this,
							"The 'Record Status Section Only' flag is currently enabled. No video images will be recorded.",
							"OccuRec",
							MessageBoxButtons.OK,
							MessageBoxIcon.Warning);
				}

                m_StateManager.CameraConnected(driverInstance, m_OverlayManager, Settings.Default.OcrMaxErrorsPerCameraTestRun, Settings.Default.FileFormat == "AAV");
				UpdateScheduleDisplay();
			}
			finally
			{
				Cursor = Cursors.Default;
			}


			picVideoFrame.Width = videoObject.Width;
			picVideoFrame.Height = videoObject.Height;

			UpdateCameraState(true);
			
		}

		private void DisconnectFromCamera()
		{
			if (videoObject != null)
			{
				videoObject.Disconnect();
				videoObject = null;
			}

			UpdateCameraState(false);
		    tssIntegrationRate.Visible = false;
		    m_StateManager.CameraDisconnected();

            if (m_OverlayManager != null)
            {
                m_OverlayManager.Finalise();
                m_OverlayManager = null;
            }

			if (m_ObservatoryController.IsConnectedToVideoCamera())
				m_ObservatoryController.DisconnectVideoCamera();

			m_PrevStateWasDisconnected = true;
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

		    
			if (videoObject != null)
			{
				lblVideoFormat.Text = videoObject.CameraVideoFormat;

                Text = string.Format("OccuRec v{0} - {1}{2}",
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
                     (Settings.Default.OcrCameraAavTestMode && Settings.Default.FileFormat == "AAV");
			}
			else
			{
                pnlCrossbar.Visible = false;
				lblVideoFormat.Text = "N/A";
                Text = string.Format("OccuRec v{0}", appVersion);
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
							m_OverlayManager.ProcessFrame(g);

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
						m_OverlayManager.ProcessFrame(g);

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
                case VideoCameraState.videoCameraIdle:
                    tssCameraState.Text = "Idle";
                    break;

                case VideoCameraState.videoCameraRunning:
					tssCameraState.Text = "Running";
                    break;

                case VideoCameraState.videoCameraRecording:
                    tssCameraState.Text = "Recording";
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
			if (Settings.Default.RecordNTPTimeStampInAAV)
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
				tsbClearTargets.Visible = false;
				tsSeparator2.Visible = false;

				//tsbCamControl.Enabled = false;
			}
			else if (ChangedToConnectedState())
			{
				tbsAddTarget.Visible = videoObject.SupportsTargetTracking;
				tsbClearTargets.Visible = videoObject.SupportsTargetTracking;
				tsbAddGuidingStar.Visible = videoObject.SupportsTargetTracking;
				tsSeparator2.Visible = videoObject.SupportsTargetTracking;
			    
				tbsAddTarget.Enabled = false;				
				tsbClearTargets.Enabled = false;
				tsbAddGuidingStar.Enabled = true;
				
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

				tbsAddTarget.Enabled = TrackingContext.Current.GuidingStar != null && !Scheduler.HasScheduledTasks();
				tsbClearTargets.Enabled = TrackingContext.Current.GuidingStar != null && !Scheduler.HasScheduledTasks();
				tsbAddGuidingStar.Enabled = !Scheduler.HasScheduledTasks();
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
					btnStopRecording.Enabled = true;
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

				btnLockIntegration.Enabled = 
					(
						m_StateManager.CanLockIntegrationNow && 
						m_StateManager.IntegrationRate > 0 && 
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

		private void btnRecord_Click(object sender, EventArgs e)
		{
			if (videoObject != null)
			{
                string fileName = FileNameGenerator.GenerateFileName(Settings.Default.FileFormat == "AAV");

				recordingfileName = videoObject.StartRecording(fileName);

				UpdateState(null);

				framesBeforeUpdatingCameraVideoFormat = 4;
			}
		}

		private void btnStopRecording_Click(object sender, EventArgs e)
		{
			if (videoObject != null)
			{
			    bool wasLocked = m_StateManager.IsIntegrationLocked;

				videoObject.StopRecording();

				UpdateState(null);

                if (wasLocked)
                    m_StateManager.UnlockIntegration();
			}
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
                m_StateManager.UnlockIntegration();
            else if (m_StateManager.CanLockIntegrationNow)
                m_StateManager.LockIntegration();
        }


        private void btnOcrTesting_Click(object sender, EventArgs e)
        {
            if (Scheduler.GetAllSchedules().Count > 0)
                MessageBox.Show("OCR testing cannot be done if there are scheduled tasks.");
            else
            {
                if (Settings.Default.FileFormat == "AAV")
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
				if (Settings.Default.RecordNTPTimeStampInAAV)
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
							if (Settings.Default.FileFormat == "AAV" && !m_StateManager.IsIntegrationLocked)
							{
								// If the integration hasn't been locked then try to lock it
								m_StateManager.LockIntegration();
							}

							string fileName = FileNameGenerator.GenerateFileName(Settings.Default.FileFormat == "AAV");
							recordingfileName = videoObject.StartRecording(fileName);
							UpdateState(null);
						}
						break;

					case ScheduledAction.StopRecording:
						if (videoObject != null && videoObject.State == VideoCameraState.videoCameraRecording)
						{
							videoObject.StopRecording();
							UpdateState(null);
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

					if (workingServers.Count >= 3)
						break;
				}

				m_LastKnownGoodNTPServers = workingServers.ToArray();
				m_LastKnownGoodNTPServersInitialised = m_LastKnownGoodNTPServers.Length >= 3;
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
					XmlNode latestVersionNode = null;

					foreach (XmlNode updateNode in xmlDoc.SelectNodes("/OccuRec/Update"))
					{
						int Version = int.Parse(updateNode.Attributes["Version"].Value);
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
			Process.Start("http://tech.groups.yahoo.com/group/OccuRec");
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

		private void btnManualIntegration_Click(object sender, EventArgs e)
		{
			if (m_StateManager.IsUsingManualIntegration)
			{
				NativeHelpers.SetManualIntegrationRateHint(0);
			}
			else
			{
				var frm = new frmChooseManualIntegrationRate();
				if (frm.ShowDialog(this) == DialogResult.OK)
				{
					int manualIntegrationRate = (int)frm.nudIntegrationRate.Value;
					NativeHelpers.SetManualIntegrationRateHint(manualIntegrationRate);
				}
			}
		}

		private void frmMain_Load(object sender, EventArgs e)
		{
			if (Settings.Default.WarnForFileSystemIssues && 
				Directory.Exists(Settings.Default.OutputLocation))
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

			tsbTelControl.Enabled = ObservatoryController.IsASCOMPlatformInstalled;
			tsbFocControl.Enabled = ObservatoryController.IsASCOMPlatformInstalled;
			tsbCamControl.Enabled = m_ObservatoryController.HasVideoCamera || ObservatoryController.IsASCOMPlatformVideoAvailable;  
        }

        public void TelescopeConnectionChanged(ASCOMConnectionState state)
        {
            RefreshASCOMStatusControls(state, tssASCOMTelescope);
            if (state == ASCOMConnectionState.Connected || state == ASCOMConnectionState.Ready)
            {
                tsbTelControl.Text = "Telescope Control";
                tsbTelControl.Enabled = true;
            }
			else if (state == ASCOMConnectionState.Disconnected || state == ASCOMConnectionState.Engaged)
            {
                tsbTelControl.Text = "Telescope Connect";
                tsbTelControl.Enabled = true;
                m_LastTelescopeState = null;
                UpdateTelescopeAndFocuserState();
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
                tsbFocControl.Text = "Focuser Control";
                tsbFocControl.Enabled = true;
            }
			else if (state == ASCOMConnectionState.Disconnected || state == ASCOMConnectionState.Engaged)
            {
                tsbFocControl.Text = "Focuser Connect";
                tsbFocControl.Enabled = true;
                m_LastFocuserState = null;
                UpdateTelescopeAndFocuserState();
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
				tsbCamControl.Text = "Camera Control";
				tsbCamControl.Enabled = true;
			}
			else if (state == ASCOMConnectionState.Disconnected || state == ASCOMConnectionState.Engaged)
			{
				tsbCamControl.Text = "Camera Connect";
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
            UpdateTelescopeAndFocuserState();

            Trace.WriteLine(state.AsSerialized().OuterXml);
        }

        public void FocuserStateUpdated(FocuserState state)
        {
            m_LastFocuserState = state;
            UpdateTelescopeAndFocuserState();

            Trace.WriteLine(state.AsSerialized().OuterXml);
        }

		void VideoStateUpdated(VideoState state)
		{
			m_LastVideoCameraState = state;
			Trace.WriteLine(state.AsSerialized().OuterXml);
		}

		void VideoError(string obj)
		{
			RefreshASCOMStatusControls(ASCOMConnectionState.Errored, tssCameraControl);
		}

        private TelescopeState m_LastTelescopeState = null;
        private FocuserState m_LastFocuserState = null;
	    private VideoState m_LastVideoCameraState = null;

        private void UpdateTelescopeAndFocuserState()
        {
            if (m_LastFocuserState != null && !double.IsNaN(m_LastFocuserState.Temperature) && m_LastTelescopeState != null)
                tslTelFocStatus.Text = string.Format("Alt: {0}°| Az: {1}° | Temp: {2}° {3}", (int)Math.Round(m_LastTelescopeState.Altitude), (int)Math.Round(m_LastTelescopeState.Azimuth), m_LastFocuserState.Temperature.ToString("0.0"), Settings.Default.FocuserTemperatureIn);
            else if (m_LastTelescopeState != null)
                tslTelFocStatus.Text = string.Format("Alt: {0}° | Az: {1}°", (int)Math.Round(m_LastTelescopeState.Altitude), (int)Math.Round(m_LastTelescopeState.Azimuth));
            else if (m_LastFocuserState != null && !double.IsNaN(m_LastFocuserState.Temperature))
                tslTelFocStatus.Text = string.Format("Temp: {0}° {1}", m_LastFocuserState.Temperature.ToString("0.0"), Settings.Default.FocuserTemperatureIn);
            else
                tslTelFocStatus.Text = string.Empty;
        }

		private void tsbCrosshair_Click(object sender, EventArgs e)
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

		private void frmMain_Shown(object sender, EventArgs e)
		{
			if (!Settings.Default.LicenseAgreementAccepted)
			{
				var frm = new frmLicenseAgreement();
				frm.StartPosition = FormStartPosition.CenterParent;
				if (frm.ShowDialog(this) == DialogResult.OK)
				{
					Settings.Default.LicenseAgreementAccepted = true;
					Settings.Default.Save();
				}
				else
					Application.Exit();
			}
		}
    }
}
