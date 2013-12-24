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
using OccRec.ASCOMWrapper;
using OccuRec.ASCOM;
using OccuRec.ASCOM.Interfaces.Devices;
using OccuRec.Config;
using OccuRec.Drivers;
using OccuRec.Helpers;
using OccuRec.OCR;
using OccuRec.Properties;
using OccuRec.Scheduling;
using OccuRec.StateManagement;

namespace OccuRec
{
    public partial class frmMain : Form, IVideoCallbacks, IASCOMDeviceCallbacks
	{
		private VideoWrapper videoObject;
		private bool running = false;

		private int imageWidth;
		private int imageHeight;
		private bool useVariantPixels;
		private string recordingfileName;
		private int framesBeforeUpdatingCameraVideoFormat = -1;

	    private CameraStateManager stateManager;
	    private ICameraImage cameraImage;
	    private string appVersion;
	    private TelescopeController telescopeController;
	    private OverlayManager overlayManager = null;
	    private List<string> initializationErrorMessages = new List<string>();
 
		public frmMain()
		{
			InitializeComponent();

			statusStrip.SizingGrip = false;

			running = true;
			previewOn = true;

		    cameraImage = new CameraImage();

		    stateManager = new CameraStateManager();
            stateManager.CameraDisconnected();

			ThreadPool.QueueUserWorkItem(new WaitCallback(DisplayVideoFrames));
		    telescopeController = new TelescopeController(this, this);

            var att = (AssemblyFileVersionAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyFileVersionAttribute), true)[0];
		    appVersion = att.Version;

#if BETA
            appVersion = string.Concat(att.Version, " [BETA]");
#else
            appVersion = att.Version;
#endif

            Text = string.Format("OccuRec v{0}", appVersion);

            CheckForUpdates(false);

		    UpdateState(null);

#if DEBUG
			// NOTE: Used to test integration detection logs against the currently used algorithm
			tbsAddTarget.Visible = true;
#endif
            ASCOMClient.Instance.Initialise();
            TelescopeConnectionChanged(ASCOMConnectionState.Disconnected);
            FocuserConnectionChanged(ASCOMConnectionState.Disconnected);
            telescopeController.CheckASCOMConnections();
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
			running = false;
		}

        public void OnError(int errorCode, string errorMessage)
        {
            if (overlayManager != null)
                overlayManager.OnError(errorCode, errorMessage);
            else
                initializationErrorMessages.Add(errorMessage);
        }

        public void OnEvent(int eventId, string eventData)
        {
            if (eventId == 1)
                stateManager.RegisterOcrError();

            if (overlayManager != null)
                overlayManager.OnEvent(eventId, eventData);
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
                        driverInstance = new Drivers.AAVTimer.Video();
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
			videoObject = new VideoWrapper(driverInstance, this);

			try
			{
				Cursor = Cursors.WaitCursor;
				initializationErrorMessages.Clear();

				videoObject.Connected = true;

				if (videoObject.Connected)
				{
					imageWidth = videoObject.Width;
					imageHeight = videoObject.Height;
					pictureBox.Image = new Bitmap(imageWidth, imageHeight);

					ResizeVideoFrameTo(imageWidth, imageHeight);
					tssIntegrationRate.Visible = Settings.Default.IsIntegrating && Settings.Default.FileFormat == "AAV";
					pnlAAV.Visible = Settings.Default.FileFormat == "AAV";

					overlayManager = new OverlayManager(videoObject.Width, videoObject.Height, initializationErrorMessages);
				}

                stateManager.CameraConnected(driverInstance, overlayManager, Settings.Default.OcrMaxErrorsPerCameraTestRun, Settings.Default.FileFormat == "AAV");
				UpdateScheduleDisplay();
			}
			finally
			{
				Cursor = Cursors.Default;
			}


			pictureBox.Width = videoObject.Width;
			pictureBox.Height = videoObject.Height;

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
		    stateManager.CameraDisconnected();

            if (overlayManager != null)
            {
                overlayManager.Finalise();
                overlayManager = null;
            }
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
			var frmSettings = new frmSettings();

		    if (frmSettings.ShowDialog(this) == DialogResult.OK)
		        telescopeController.CheckASCOMConnections();
		}

		private void miConnect_Click(object sender, EventArgs e)
		{
            if (!Directory.Exists(Settings.Default.OutputLocation))
            {
                MessageBox.Show("Output Video Location is invalid.", "OccuRec", MessageBoxButtons.OK, MessageBoxIcon.Error);

                var frmSettings = new frmSettings();
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

		private long lastDisplayedVideoFrameNumber = -1;
		private bool previewOn = true;

        private delegate void PaintVideoFrameDelegate(VideoFrameWrapper frame, Bitmap bmp);

		private int renderedFrameCounter = 0;
		private long startTicks = 0;
		private long endTicks = 0;

		private double renderFps = double.NaN;
		private long currentFrameNo = 0;


        private void PaintVideoFrame(VideoFrameWrapper frame, Bitmap bmp)
		{
            bool isEmptyFrame = frame == null || bmp == null;

			if (isEmptyFrame)
			{
				using (Graphics g = Graphics.FromImage(pictureBox.Image))
				{
					if (bmp == null)
						g.Clear(Color.Green);
					else
						g.DrawImage(bmp, 0, 0);

                    if (overlayManager != null)
    				    overlayManager.ProcessFrame(g);

					g.Save();
				}

				pictureBox.Invalidate();
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

			using (Graphics g = Graphics.FromImage(pictureBox.Image))
			{
			    g.DrawImage(bmp, 0, 0);

                if (overlayManager != null)
			        overlayManager.ProcessFrame(g);

			    g.Save();
			}

			pictureBox.Invalidate();
			bmp.Dispose();

			if (framesBeforeUpdatingCameraVideoFormat >= 0)
				framesBeforeUpdatingCameraVideoFormat--;

			if (framesBeforeUpdatingCameraVideoFormat == 0)
			{
				lblVideoFormat.Text = videoObject.CameraVideoFormat;
			}

            if (stateManager.ProvidesOcredTimestamps)
            {
				if (stateManager.OcrErrors > 0)
				{
					if (tssOcrErr.Tag == null || (int)tssOcrErr.Tag != 2)
					{
						tssOcrErr.ForeColor = Color.Red;
						tssOcrErr.Tag = (int) 2;
					}

					tssOcrErr.Text = string.Format("OCR ERR {0}", stateManager.OcrErrors);	
					
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

            if (stateManager.DroppedFrames != 0)
            {
                tssDroppedFrames.Text = string.Format("{0} Dropped", stateManager.DroppedFrames);

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
		
		internal class FakeFrame : IVideoFrame
		{
			private static int s_Counter = -1;

			public object ImageArray
			{
				get { return new object(); }
			}

			public object ImageArrayVariant
			{
				get { return null; }
			}

            public Bitmap PreviewBitmap
            {
                get { return null; }
            }

			public long FrameNumber
			{
				get
				{
					s_Counter++;
					return s_Counter;
				}
			}

			public double ExposureDuration
			{
				get { return 0; }
			}

			public string ExposureStartTime
			{
				get { return null; }
			}

			public string ImageInfo
			{
				get { return null; }
			}
		}


		private void DisplayVideoFrames(object state)
		{
			while(running)
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

                                Invoke(new PaintVideoFrameDelegate(PaintVideoFrame), new object[] { frameWrapper, bmp });                                
                            }
						}
					}
                    catch (InvalidOperationException) { }
					catch(Exception ex)
					{
						Trace.WriteLine(ex);

						Bitmap errorBmp = new Bitmap(pictureBox.Width, pictureBox.Height);
						using (Graphics g = Graphics.FromImage(errorBmp))
						{
							g.Clear(Color.MidnightBlue);
							//g.DrawString(ex.Message, debugTextFont, Brushes.Black, 10, 10);
							g.Save();
						}
						try
						{
							Invoke(new PaintVideoFrameDelegate(PaintVideoFrame), new object[] {null, errorBmp});
						}
						catch(InvalidOperationException)
						{
							// InvalidOperationException could be thrown when closing down the app i.e. when the form has been already disposed
						}
					}

				}

				Thread.Sleep(1);
				Application.DoEvents();
			}
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

		private void UpdateState(VideoFrameWrapper frame)
		{
		    if (IsDisposed)
                // It is possible this method to be called during Disposing and we don't need to do anything in that case
		        return;

			if (videoObject == null)
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
			}
			else
			{
			    UpdateApplicationStateFromCameraState();

				if (frame != null)
				{
					if (!tssFrameNo.Visible) tssFrameNo.Visible = true;

					if (stateManager.IsIntegrationLocked)
						tssFrameNo.Text = frame.IntegratedFrameNo.ToString("Integrated Frame: 0", CultureInfo.InvariantCulture);
					else
						tssFrameNo.Text = frame.FrameNumber.ToString("Current Frame: 0", CultureInfo.InvariantCulture);

					if (stateManager.IsIntegrationLocked)
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

					if (fi.Directory != null)
					{
						ulong freeBytes;
						NativeHelpers.GetDriveFreeBytes(fi.Directory.FullName, out freeBytes);

						if (freeBytes < ((ulong)1024 * (ulong)1024 * (ulong)1024 * (ulong)2))
						{
							tssFreeDiskSpace.Visible = true;
							tssFreeDiskSpace.Text = string.Format("{0:0.0} Gb free", 1.0 * freeBytes / (1024 * 1024 * 1024));
						}
						else
							tssFreeDiskSpace.Visible = false;
					}

					tssRecordingFile.Visible = true;
					btnStopRecording.Enabled = true;
					btnRecord.Enabled = false;
				}
                else if (
                    videoObject.State == VideoCameraState.videoCameraRunning && 
                    lbSchedule.Items.Count == 0 && 
                    (stateManager.CanStartRecording || Settings.Default.IntegrationDetectionTuning))
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
						stateManager.CanLockIntegrationNow && 
						stateManager.IntegrationRate > 0 && 
						(frame == null || !frame.PerformedAction.HasValue || frame.PerformedAction.Value == 0)
                     ) 
					|| stateManager.IsIntegrationLocked;

				btnCalibrateIntegration.Visible = !stateManager.IsIntegrationLocked;
				btnManualIntegration.Visible = !stateManager.IsIntegrationLocked;
				if (!stateManager.IsIntegrationLocked && stateManager.PercentDoneDetectingIntegration < 100)
				{
					pbarIntDetPercentDone.Value = stateManager.PercentDoneDetectingIntegration;
					if (!pbarIntDetPercentDone.Visible) pbarIntDetPercentDone.Visible = true;
				}
				else if (pbarIntDetPercentDone.Visible) pbarIntDetPercentDone.Visible = false;



				if (frame != null && frame.PerformedAction.HasValue && frame.PerformedAction.Value > 0)
				{
					// When there is an action in progress, then don't show anything
					btnLockIntegration.Text = "Busy ...";
				}
				else if (stateManager.IntegrationRate > 0 && stateManager.IsValidIntegrationRate && !stateManager.IsIntegrationLocked && stateManager.CanLockIntegrationNow)
                    btnLockIntegration.Text = string.Format("Lock at x{0} Frames", stateManager.IntegrationRate);
                else if (stateManager.IsIntegrationLocked)
                    btnLockIntegration.Text = "Unlock";
                else
                    btnLockIntegration.Text = "Checking Integration ...";

                if (stateManager.IsCalibratingIntegration)
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

				if (stateManager.IsUsingManualIntegration)
					btnManualIntegration.Text = "Automatic";
				else
					btnManualIntegration.Text = "Manual";

                tsbConnectDisconnect.ToolTipText = "Disconnect";
                tsbConnectDisconnect.Image = imageListToolbar.Images[1];
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
			    bool wasLocked = stateManager.IsIntegrationLocked;

				videoObject.StopRecording();

				UpdateState(null);

                if (wasLocked)
                    stateManager.UnlockIntegration();
			}
		}

		private void ResizeVideoFrameTo(int imageWidth, int imageHeight)
		{
			Width = Math.Max(800, (imageWidth - pictureBox.Width) + this.Width);
			Height = Math.Max(600, (imageHeight - pictureBox.Height) + this.Height);
		}

		private void btnImageSettings_Click(object sender, EventArgs e)
		{
			if (videoObject != null)
				videoObject.ConfigureImage();
		}

        private void btnLockIntegration_Click(object sender, EventArgs e)
        {
            if (stateManager.IsIntegrationLocked)
                stateManager.UnlockIntegration();
            else if (stateManager.CanLockIntegrationNow)
                stateManager.LockIntegration();
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
						recordingfileName = stateManager.StartRecordingOCRTestingFile();
                    }
                        
                }

                UpdateState(null);
            } 
        }

        private void btnAddSchedule_Click(object sender, EventArgs e)
        {
            var frm = new frmAddScheduleEntry();
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

        private void timerScheduler_Tick(object sender, EventArgs e)
        {
            ScheduledAction actionToTake = Scheduler.CheckSchedules();

            if (actionToTake != ScheduledAction.None)
            {
                switch(actionToTake)
                {
                    case ScheduledAction.StartRecording:
                        if (videoObject != null && videoObject.State == VideoCameraState.videoCameraRunning)
                        {
							if (Settings.Default.FileFormat == "AAV" && !stateManager.IsIntegrationLocked)
							{
								// If the integration hasn't been locked then try to lock it
							    stateManager.LockIntegration();
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
                }

                UpdateScheduleDisplay();
            }

            ScheduleEntry entry = Scheduler.GetNextEntry();
            if (entry != null)
            {
                lblSecheduleWhatsNext.Text = entry.GetRemainingTime();
                pnlNextScheduledAction.Visible = true;
            }
            else
            {
                pnlNextScheduledAction.Visible = false;
            }

            if (nextNTPSyncTime < DateTime.UtcNow)
            {
                ThreadPool.QueueUserWorkItem(UpdateTimeFromNTPServer);

                nextNTPSyncTime = DateTime.UtcNow.AddMinutes(10);
            }

			if (stateManager.IsRecordingOcrTestFile && videoObject != null && videoObject.State == VideoCameraState.videoCameraRecording)
			{
				// NOTE: If we have been recording OCR test file and there are more than MaxErrorCount OCR errors then stop the recording
				if (stateManager.OcrErrors > Settings.Default.OcrMaxErrorsPerCameraTestRun)
				{
					stateManager.StopRecordingOCRTestingFile();
					UpdateState(null);
				}				
			}
        }

        private void UpdateTimeFromNTPServer(object state)
        {
            try
            {
                DateTime networkUTCTime = NTPClient.GetNetworkTime(Settings.Default.NTPServer);
                NTPClient.SetTime(networkUTCTime);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
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
			if (stateManager.IsCalibratingIntegration)
			{
				stateManager.CancelIntegrationCalibration(true);
			}
			else if (
				videoObject != null &&
				MessageBox.Show(
					this, 
					string.Format("Put your integrating camera in x{0} video frames integraton mode and press 'OK' to start the calibration process.", Settings.Default.CalibrationIntegrationRate), 
					"Integration Detection Calibration", 
					MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.OK)
			{
				stateManager.BeginIntegrationCalibration(4);
			}
		}

		private void tbsAddTarget_Click(object sender, EventArgs e)
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

		private void btnManualIntegration_Click(object sender, EventArgs e)
		{
			if (stateManager.IsUsingManualIntegration)
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
				ulong freeBytes;
				NativeHelpers.GetDriveFreeBytes(Settings.Default.OutputLocation, out freeBytes);

				string directoryRoot = Directory.GetDirectoryRoot(Settings.Default.OutputLocation);

				if (freeBytes < ((ulong)1024 * (ulong)1024 * (ulong)1024 * (ulong)2))
				{
					MessageBox.Show(
						string.Format("There is only {0:0.0} Gb left on drive {1}\r\n\r\nThere may not be enough disk space to record a video!",
						1.0 * freeBytes / (1024 * 1024 * 1024),
						directoryRoot), 
						"OccuRec",
						MessageBoxButtons.OK,
						MessageBoxIcon.Warning);
				}

				FileNameGenerator.CheckAndWarnForFileSystemLimitation();
			}
		}

		private void pictureBox_MouseMove(object sender, MouseEventArgs e)
		{
			if (overlayManager != null)
				overlayManager.MouseMove(e);
		}

		private void pictureBox_MouseLeave(object sender, EventArgs e)
		{
			if (overlayManager != null)
				overlayManager.MouseLeave(e);
		}

		private void pictureBox_MouseDown(object sender, MouseEventArgs e)
		{
			if (overlayManager != null)
				overlayManager.MouseDown(e);
		}

		private void pictureBox_MouseUp(object sender, MouseEventArgs e)
		{
			if (overlayManager != null)
				overlayManager.MouseUp(e);
		}

        #region IASCOMDeviceCallbacks
        private void RefreshASCOMStatusBarIcon(ASCOMConnectionState state, ToolStripStatusLabel label)
        {
            switch (state)
            {
                case ASCOMConnectionState.Disconnected:
                    label.Visible = false;
                    break;
                case ASCOMConnectionState.Connecting:
                    label.ForeColor = Color.Goldenrod;
                    label.Visible = true;
                    break;
                case ASCOMConnectionState.Connected:
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
        }
        public void TelescopeConnectionChanged(ASCOMConnectionState state)
        {
            RefreshASCOMStatusBarIcon(state, tssASCOMTelescope);
        }

        public void FocuserConnectionChanged(ASCOMConnectionState state)
        {
            RefreshASCOMStatusBarIcon(state, tssASCOMFocuser);
        }

        public void TelescopeStateUpdate(TelescopeState state)
        {
            tssASCOMTelescope.Text = string.Format("TEL (Alt:{0}|Az:{1})", (int)Math.Round(state.Altitude), (int)Math.Round(state.Azimuth));

            Trace.WriteLine(string.Format("Altitude: {0}\r\nAzimuth: {1}\r\nAtHome: {2}\r\nAtPark: {3}\r\nCanFindHome: {4}\r\nCanPark: {5}", state.Altitude, state.Azimuth, state.AtHome, state.AtPark,
                                          state.CanFindHome, state.CanPark));

        }
        #endregion
    }
}
