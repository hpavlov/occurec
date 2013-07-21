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
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
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

		    Text = string.Format("AAVRec v{0}", appVersion);
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

	    private Queue<string> errorMessagesQueue = new Queue<string>();

        public void OnError(int errorCode, string errorMessage)
        {
            errorMessagesQueue.Enqueue(errorMessage);
        }

		private void ConnectToCamera()
		{
		    var chooser = new frmChooseCamera();
            if (chooser.ShowDialog(this) == DialogResult.OK)
            {
                if (!string.IsNullOrEmpty(Settings.Default.FileFormat))
                {
                    IVideo driverInstance;
                    if (Settings.Default.FileFormat == "AAV")
                        driverInstance = new Drivers.AAVTimer.Video();
                    else if (Settings.Default.FileFormat == "AVI")
                        driverInstance = new Drivers.DirectShowCapture.Video();
                    else
                    {
                        if (".avi".Equals(Path.GetExtension(Settings.Default.SimulatorFilePath), StringComparison.InvariantCultureIgnoreCase))
                            driverInstance = new Drivers.AVISimulator.Video();
                        else
                            driverInstance = new Drivers.AAVSimulator.Video();
                    }

                    videoObject = new VideoWrapper(driverInstance, this);

                    try
                    {
                        Cursor = Cursors.WaitCursor;
                        videoObject.Connected = true;

                        if (videoObject.Connected)
                        {
                            imageWidth = videoObject.Width;
                            imageHeight = videoObject.Height;
                            pictureBox.Image = new Bitmap(imageWidth, imageHeight);

                            ResizeVideoFrameTo(imageWidth, imageHeight);
                            tssIntegrationRate.Visible = Settings.Default.IsIntegrating && Settings.Default.FileFormat == "AAV";
                            pnlAAV.Visible = Settings.Default.FileFormat == "AAV";
                        }

                        stateManager.CameraConnected(driverInstance);
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
            }
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
		}

		private void UpdateCameraState(bool connected)
		{
			pnlVideoControls.Enabled = connected;
			miConnect.Enabled = !connected;
			miDisconnect.Enabled = connected;
            pnlOcrTesting.Visible = false;

			UpdateState();

			pnlVideoControls.Enabled = connected;
			btnRecord.Enabled = connected && videoObject != null && videoObject.State == VideoCameraState.videoCameraRunning && lbSchedule.Items.Count == 0;
            btnStopRecording.Enabled = connected && videoObject != null && videoObject.State == VideoCameraState.videoCameraRecording && lbSchedule.Items.Count == 0;
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

	    private string currMessageToDisplay = null;
	    private DateTime displayMessageUntil = DateTime.MinValue;

        private static Font errorMessagesFont = new Font(FontFamily.GenericMonospace, 10);

        private void PrintCurrentErrorMessage(Graphics g)
        {
            SizeF msgMeasurement = g.MeasureString(currMessageToDisplay, errorMessagesFont);

            g.FillRectangle(Brushes.DarkSlateGray, pictureBox.Image.Width - msgMeasurement.Width - 9, 3, msgMeasurement.Width + 6, msgMeasurement.Height + 6);
            g.DrawString(currMessageToDisplay, errorMessagesFont, Brushes.Yellow, pictureBox.Image.Width - msgMeasurement.Width - 6, 6);
        }

        private void ProcessErrorMessages(Graphics g)
        {
            if (displayMessageUntil != DateTime.MinValue && currMessageToDisplay != null)
            {
                if (DateTime.Now.Ticks > displayMessageUntil.Ticks)
                {
                    displayMessageUntil = DateTime.MinValue;
                    currMessageToDisplay = null;
                }
                else
                    PrintCurrentErrorMessage(g);
            }
            else
            {
                if (errorMessagesQueue.Count > 0)
                {
                    currMessageToDisplay = errorMessagesQueue.Dequeue();
                    displayMessageUntil = DateTime.Now.AddSeconds(5);
                    PrintCurrentErrorMessage(g);
                }
            }
        }

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

				    ProcessErrorMessages(g);

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

                ProcessErrorMessages(g);

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
							g.Clear(Color.Tomato);
							g.DrawString(ex.Message, debugTextFont, Brushes.Black, 10, 10);
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
			if (videoObject == null)
			{
				tssCameraState.Text = "Disconnected";
				tssFrameNo.Text = string.Empty;
				tssDisplayRate.Text = string.Empty;
				tssFrameNo.Visible = false;
				tssDisplayRate.Visible = false;
			}
			else
			{
			    UpdateApplicationStateFromCameraState();

				if (!tssFrameNo.Visible) tssFrameNo.Visible = true;				

				tssFrameNo.Text = currentFrameNo.ToString("Current Frame: 0", CultureInfo.InvariantCulture);
				if (!double.IsNaN(renderFps))
				{
					if (!tssDisplayRate.Visible) tssDisplayRate.Visible = true;
					tssDisplayRate.Text = renderFps.ToString("Display Rate: 0 fps");
				}
				else
					tssDisplayRate.Text = "Display Rate: N/A";

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
                    stateManager.CanStartRecording)
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
                if (stateManager.IntegrationRate > 0 && !stateManager.IsIntegrationLocked)
                    btnLockIntegration.Text = string.Format("Lock at x{0}", stateManager.IntegrationRate);
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
                            string fileName = FileNameGenerator.GenerateFileName(Settings.Default.FileFormat == "AAV");
                            recordingfileName = videoObject.StartRecording(fileName);
                            Console.Beep();
                            UpdateState();
                        }
                        break;

                    case ScheduledAction.StopRecording:
                        if (videoObject != null && videoObject.State == VideoCameraState.videoCameraRecording)
                        {
                            videoObject.StopRecording();
                            Console.Beep();
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
                DateTime networkUTCTime = NTPClient.GetNetworkTime();
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

	}
}
