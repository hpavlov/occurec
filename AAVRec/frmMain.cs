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
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using AAVRec.Drivers;
using AAVRec.Helpers;
using AAVRec.OCR;
using AAVRec.Properties;
using AAVRec.Scheduling;
using AAVRec.StateManagement;

namespace AAVRec
{
	public partial class frmMain : Form, IVideoCallbacks
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

	    private OverlayManager overlayManager = null;
	    private List<string> initializationErrorMessages = new List<string>();
 
		public frmMain()
		{
			InitializeComponent();

			running = true;
			previewOn = true;

		    cameraImage = new CameraImage();

		    stateManager = new CameraStateManager();
            stateManager.CameraDisconnected();

			ThreadPool.QueueUserWorkItem(new WaitCallback(DisplayVideoFrames));

            var att = (AssemblyFileVersionAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyFileVersionAttribute), true)[0];
		    appVersion = att.Version;

#if BETA
            appVersion = string.Concat(att.Version, " [BETA]");
#else
            appVersion = att.Version;
#endif

            Text = string.Format("AAVRec v{0}", appVersion);

            CheckForUpdates(false);

		    UpdateState();
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

				stateManager.CameraConnected(driverInstance, Settings.Default.OcrMaxErrorsPerCameraTestRun, Settings.Default.FileFormat == "AAV");
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

            if (overlayManager != null)
            {
                overlayManager.Finalise();
                overlayManager = null;
            }

			UpdateCameraState(false);
		    tssIntegrationRate.Visible = false;
		    stateManager.CameraDisconnected();
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

			UpdateState();

			pnlVideoControls.Enabled = connected;
		    btnRecord.Enabled = CanRecordNow(connected);
            btnStopRecording.Enabled = CanStopRecordingNow(connected);
			btnImageSettings.Enabled = connected && videoObject != null && videoObject.CanConfigureImage;

		    
			if (videoObject != null)
			{
				lblVideoFormat.Text = videoObject.CameraVideoFormat;

                Text = string.Format("AAVRec v{0} - {1}{2}",
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
                     (Settings.Default.OcrCameraTestModeAav && Settings.Default.FileFormat == "AAV") ||
                     (Settings.Default.OcrCameraTestModeAvi && Settings.Default.FileFormat == "AVI");
			}
			else
			{
                pnlCrossbar.Visible = false;
				lblVideoFormat.Text = "N/A";
                Text = string.Format("AAVRec v{0}", appVersion);
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
		    frmSettings.ShowDialog(this);
		}

		private void miConnect_Click(object sender, EventArgs e)
		{
            if (!Directory.Exists(Settings.Default.OutputLocation))
            {
                MessageBox.Show("Output Video Location is invalid.", "AAVRec", MessageBoxButtons.OK, MessageBoxIcon.Error);

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
			bool isEmptyFrame = frame == null;
			if (!isEmptyFrame)
				isEmptyFrame = useVariantPixels 
					? frame.ImageArrayVariant == null 
					: frame.ImageArray == null;

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
			UpdateState();
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

            if (!string.IsNullOrEmpty(frame.ImageInfo))
            {
                if (frame.IntegrationRate != null)
                    tssIntegrationRate.Text = string.Format("Integration Rate: x{0}", frame.IntegrationRate);
                else
                    tssIntegrationRate.Text = "Integration Rate: ...";
            }

            if (stateManager.OcrErrors > 0)
            {
                tssOcrErr.Text = string.Format("OCR ERR {0}", stateManager.OcrErrors);

                if (!tssOcrErr.Visible)
                    tssOcrErr.Visible = true;
            }
            else
            {
                if (tssOcrErr.Visible)
                    tssOcrErr.Visible = false;
            }

            if (stateManager.DroppedFrames > 0)
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

                                Bitmap bmp = null;

                                cameraImage.SetImageArray(
                                    useVariantPixels
                                        ? frame.ImageArrayVariant
                                        : frame.ImageArray,
                                    imageWidth,
                                    imageHeight,
                                    videoObject.SensorType);

                                bmp = cameraImage.GetDisplayBitmap();

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

		private void UpdateState()
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

			    tsbConnectDisconnect.ToolTipText = "Connect";
			    tsbConnectDisconnect.Image = imageListToolbar.Images[0];
			}
			else
			{
			    UpdateApplicationStateFromCameraState();

				if (!tssFrameNo.Visible) tssFrameNo.Visible = true;				

				tssFrameNo.Text = currentFrameNo.ToString("Current Frame: 0", CultureInfo.InvariantCulture);
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

					tssRecordingFile.Visible = true;
					btnStopRecording.Enabled = true;
					btnRecord.Enabled = false;
				}
                else if (
                    videoObject.State == VideoCameraState.videoCameraRunning && 
                    lbSchedule.Items.Count == 0 && 
                    !stateManager.IsTestingIotaVtiOcr &&
                    (stateManager.CanStartRecording || Settings.Default.IntegrationDetectionTuning))
                {
                    tssRecordingFile.Visible = false;
                    btnStopRecording.Enabled = false;
                    btnRecord.Enabled = true;
                }
                else
                {
                    btnRecord.Enabled = false;
                    btnStopRecording.Enabled = false;
                }

                btnLockIntegration.Enabled = (stateManager.CanLockIntegrationNow && stateManager.IntegrationRate > 0) || stateManager.IsIntegrationLocked;
                if (stateManager.IntegrationRate > 0 && stateManager.IsValidIntegrationRate && !stateManager.IsIntegrationLocked)
                    btnLockIntegration.Text = string.Format("Lock at x{0} Frames", stateManager.IntegrationRate);
                else if (stateManager.IsIntegrationLocked)
                    btnLockIntegration.Text = "Unlock";
                else
                    btnLockIntegration.Text = "Checking Integration ...";

                if (stateManager.IsTestingIotaVtiOcr)
                {
                    btnOcrTesting.Text = "Stop OCR Testing";
                    tssCameraState.Text = "OCR Testing";
                    gbxSchedules.Enabled = false;
                }
                else
                {
                    btnOcrTesting.Text = "Run OCR Testing";
                    UpdateApplicationStateFromCameraState();
                    gbxSchedules.Enabled = true;
                }

				if (stateManager.IsCalibratingIntegration)
				{
					btnCalibrateIntegration.Text = "Cancel Calibration";
					tssCameraState.Text = "Calibrating";
					gbxSchedules.Enabled = false;
				}
				else
				{
					btnCalibrateIntegration.Text = "Calibrate";
					UpdateApplicationStateFromCameraState();
					gbxSchedules.Enabled = true;
				}

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

				UpdateState();

				framesBeforeUpdatingCameraVideoFormat = 4;
			}
		}

		private void btnStopRecording_Click(object sender, EventArgs e)
		{
			if (videoObject != null)
			{
				videoObject.StopRecording();

				UpdateState();
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
                    stateManager.ToggleIotaVtiOcrTesting();
                else if (Settings.Default.FileFormat == "AVI")
                {
                    if (videoObject != null)
                        videoObject.ExecuteAction("ToggleIotaVtiOcrTesting", null);
                }

                UpdateState();
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
								// NOTE: If the integration hasn't been locked then don't start recording

								ScheduleEntry nextEntry = Scheduler.GetNextEntry();
								if (nextEntry != null && nextEntry.Action == ScheduledAction.StopRecording)
									Scheduler.RemoveOperation(nextEntry.OperaionId);

								overlayManager.OnError(100, "Failed to start AAV recording. Integration is not locked.");

								UpdateState();
							}
							else
							{
								string fileName = FileNameGenerator.GenerateFileName(Settings.Default.FileFormat == "AAV");
								recordingfileName = videoObject.StartRecording(fileName);
								UpdateState();								
							}
                        }
                        break;

                    case ScheduledAction.StopRecording:
                        if (videoObject != null && videoObject.State == VideoCameraState.videoCameraRecording)
                        {
                            videoObject.StopRecording();
                            UpdateState();
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
			if (eventCode == MSG_ID_NEW_AAVREC_UPDATE_AVAILABLE)
            {
                pnlNewVersionAvailable.Visible = true;
                m_ShowNegativeResultMessage = false;
            }
			else if (eventCode == MSG_ID_NO_AAVREC_UPDATES_AVAILABLE)
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

		private byte MSG_ID_NEW_AAVREC_UPDATE_AVAILABLE = 13;
		private byte MSG_ID_NO_AAVREC_UPDATES_AVAILABLE = 14;

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
					Invoke(new OnUpdateEventDelegate(OnUpdateEvent), MSG_ID_NEW_AAVREC_UPDATE_AVAILABLE);
				}
				else
				{
					Trace.WriteLine(string.Format("There are no new {0}updates.", Settings.Default.AcceptBetaUpdates ? "beta " : ""), "Update");
					Invoke(new OnUpdateEventDelegate(OnUpdateEvent), MSG_ID_NO_AAVREC_UPDATES_AVAILABLE); 
				}

				Settings.Default.LastCheckedForUpdates = m_LastUpdateTime;
				Settings.Default.Save();
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex, "Update");
			}
		}

		private string aavRecUpdateServerVersion = null;

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

					int latestVersion = UpdateManager.CurrentlyInstalledAAVRecVersion();
					XmlNode latestVersionNode = null;

					foreach (XmlNode updateNode in xmlDoc.SelectNodes("/AAVRec/Update"))
					{
						int Version = int.Parse(updateNode.Attributes["Version"].Value);
						if (latestVersion < Version)
						{
							Trace.WriteLine("Update location: " + updateUri.ToString());
							Trace.WriteLine("Current version: " + latestVersion.ToString());
							Trace.WriteLine("New version: " + Version.ToString());

							XmlNode aavRecUpdateNode = xmlDoc.SelectSingleNode("/AAVRec/AAVRecUpdate");
							if (aavRecUpdateNode != null)
							{
								aavRecUpdateServerVersion = aavRecUpdateNode.Attributes["Version"].Value;
								Trace.WriteLine("AAVRecUpdate new version: " + aavRecUpdateServerVersion);
							}
							else
								aavRecUpdateServerVersion = null;

							latestVersion = Version;
							latestVersionNode = updateNode;
						}
					}

					foreach (XmlNode updateNode in xmlDoc.SelectNodes("/AAVRec/ModuleUpdate[@MustExist = 'false']"))
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

							XmlNode aavRecUpdateNode = xmlDoc.SelectSingleNode("/AAVRec/AAVRecUpdate");
							if (aavRecUpdateNode != null)
							{
								aavRecUpdateServerVersion = aavRecUpdateNode.Attributes["Version"].Value;
								Trace.WriteLine("AAVRecUpdate new version: " + aavRecUpdateServerVersion);
							}
							else
								aavRecUpdateServerVersion = null;

							latestVersion = Version;
							latestVersionNode = updateNode;
						}
					}

					XmlNode cfgUpdateNode = xmlDoc.SelectSingleNode("/AAVRec/ConfigurationUpdate");
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
			RunAAVRecUpdater();
		}

		private void pnlNewVersionAvailable_Click(object sender, EventArgs e)
		{
			pnlNewVersionAvailable.Enabled = false;
			pnlNewVersionAvailable.IsLink = false;
			pnlNewVersionAvailable.Tag = pnlNewVersionAvailable.Text;
			pnlNewVersionAvailable.Text = "Update started ...";
			statusStrip.Update();

			RunAAVRecUpdater();

			Close();
		}

		private void RunAAVRecUpdater()
		{
			try
			{
				string currentPath = AppDomain.CurrentDomain.BaseDirectory;
				string updaterFileName = Path.GetFullPath(currentPath + "\\AAVRecUpdate.zip");

				int currUpdVer = UpdateManager.CurrentlyInstalledAAVRecUpdateVersion();
				int servUpdVer = int.Parse(aavRecUpdateServerVersion);
				if (!File.Exists(Path.GetFullPath(currentPath + "\\AAVRecUpdate.exe")) || /* If there is no AAVRecUpdate.exe*/
					(
						statusStrip.Tag != null &&  /* Or it is an older version ... */
						servUpdVer > currUpdVer)
					)
				{
					if (servUpdVer > currUpdVer)
						Trace.WriteLine(string.Format("Update required for 'AAVRecUpdate.exe': local version: {0}; server version: {1}", currUpdVer, servUpdVer));

					Uri updateUri = new Uri(UpdateManager.UpdateLocation + "/AAVRecUpdate.zip");

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
								MessageBox.Show(this, uex.Message + "\r\n\r\nYou may need to run AAVRec as administrator to complete the update.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
						MessageBox.Show("There was an error trying to download the AAVRecUpdate program. Please ensure that you have an active internet connection and try again later.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

				updaterFileName = Path.GetFullPath(currentPath + "\\AAVRecUpdate.exe");

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
				Trace.WriteLine(ex.GetFullErrorDescription(), "Update");
				pnlNewVersionAvailable.Enabled = true;
			}
		}
		#endregion

		private void miHelpIndex_Click(object sender, EventArgs e)
		{
			Process.Start("http://www.hristopavlov.net/AAVRec");
		}

		private void miYahooGroup_Click(object sender, EventArgs e)
		{
			Process.Start("http://tech.groups.yahoo.com/group/AAVRec");
		}

		private void miOpenFile_Click(object sender, EventArgs e)
		{
			if (videoObject == null)
			{
				if (openFileDialog.ShowDialog(this) == DialogResult.OK)
				{
					Settings.Default.SimulatorFilePath = openFileDialog.FileName;
				    Settings.Default.OcrSimulatorTestMode = false;
                    Settings.Default.OcrCameraTestModeAvi = false;
                    Settings.Default.OcrCameraTestModeAav = false;
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
				stateManager.CancelIntegrationCalibration();
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
			var dict = new Dictionary<float, List<float>>();
			dict.Add(0.1f,
			         new List<double>(new double[]
				         {
					         0.2133789, 0.8325195, 0.1630859, 0.1635742, 0.1586914, 0.168457, 0.1582031, 0.1347656, 0.140625, 0.862793,
					         0.1201172, 0.1386719, 0.140625, 0.1342773, 0.1293945, 0.1245117, 0.1333008, 0.8041992, 0.1240234,
					         0.1176758, 0.1254883, 0.1333008, 0.1269531, 0.137207
				         }).Select(x => (float)x).ToList());
			
dict.Add(0.15f,new List<double>(new double[] { 0.1445313,0.8183594,0.1298828,0.1357422,0.152832,0.1259766,0.1591797,0.1293945,0.1425781,0.875,0.1416016,0.1547852,0.1464844,0.1420898,0.1362305,0.1362305,0.1489258,0.7983398,0.1411133,0.1367188,0.1337891,0.1259766,0.1352539,0.1513672}).Select(x => (float)x).ToList());
dict.Add(0.25f, new List<double>(new double[]{ 0.1308594,0.7705078,0.1474609,0.1411133,0.1279297,0.1274414,0.1533203,0.1367188,0.1411133,0.9023438,0.1362305,0.1401367,0.1503906,0.1401367,0.1699219,0.1450195,0.1469727,0.8959961,0.1376953,0.1459961,0.1367188,0.1235352,0.1347656,0.1318359}).Select(x => (float)x).ToList());
dict.Add(0.35f,new List<double>(new double[] { 0.1381836,0.8500977,0.1342773,0.1362305,0.1362305,0.1323242,0.1152344,0.1401367,0.1220703,0.8642578,0.1547852,0.1630859,0.2021484,0.1923828,0.1811523,0.2050781,0.1850586,0.8037109,0.2011719,0.199707,0.1987305,0.1948242,0.2133789,0.2211914}).Select(x => (float)x).ToList());
dict.Add(0.55f,new List<double>(new double[] { 0.2207031,0.8417969,0.2255859,0.2314453,0.21875,0.2285156,0.2270508,0.2451172,0.2426758,0.9086914,0.2290039,0.21875,0.2260742,0.2172852,0.2348633,0.2568359,0.2402344,0.8623047,0.2260742,0.2114258,0.2294922,0.2016602,0.2226563,0.2148438}).Select(x => (float)x).ToList());
dict.Add(0.75f, new List<double>(new double[]{ 0.2026367,0.8759766,0.2080078,0.2163086,0.2197266,0.215332,0.215332,0.2094727,0.2182617,0.7685547,0.2231445,0.2177734,0.2036133,0.2167969,0.2148438,0.2011719,0.2158203,0.8398438,0.2036133,0.2094727,0.1958008,0.1791992,0.1884766,0.1767578}).Select(x => (float)x).ToList());
dict.Add(1f, new List<double>(new double[] { 0.1806641,0.8637695,0.1850586,0.1938477,0.1987305,0.1953125,0.1967773,0.1943359,0.2041016,0.9326172,0.1943359,0.199707,0.1982422,0.2011719,0.2036133,0.1953125,0.1933594,0.8818359,0.1879883,0.2089844,0.1914063,0.2001953,0.2041016,0.1953125}).Select(x => (float)x).ToList());
dict.Add(2f, new List<double>(new double[] {  0.2060547,0.9086914,0.2285156,0.2094727,0.2158203,0.199707,0.2133789,0.2207031,0.2182617,0.8793945,0.2099609,0.2158203,0.2241211,0.2421875,0.2607422,0.2729492,0.2714844,0.8344727,0.2871094,0.2900391,0.2915039,0.2861328,0.2978516,0.2978516}).Select(x => (float)x).ToList());
dict.Add(3f, new List<double>(new double[] { 0.3061523,0.8193359,0.2929688,0.3115234,0.3071289,0.3051758,0.3349609,0.3364258,0.3349609,0.8505859,0.3349609,0.3442383,0.3496094,0.3530273,0.3613281,0.3515625,0.3564453,0.8603516,0.3803711,0.3369141,0.3388672,0.3540039,0.340332,0.3637695}).Select(x => (float)x).ToList());
dict.Add(4f, new List<double>(new double[] { 0.34375, 0.871582, 0.3730469, 0.3486328, 0.3564453, 0.3540039, 0.3364258, 0.3632813, 0.3486328, 0.902832, 0.4052734, 0.3955078, 0.3876953, 0.3828125, 0.3989258, 0.4003906, 0.3984375, 0.9741211, 0.4287109, 0.4125977, 0.4199219, 0.4335938, 0.4316406, 0.4458008 }).Select(x => (float)x).ToList());


			var det = new IntegrationDetectionCalibrator(dict);
			det.Calibrate();
		}

	}
}
