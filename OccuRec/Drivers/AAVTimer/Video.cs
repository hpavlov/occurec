//tabs=4
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using OccuRec.Drivers;
using OccuRec.Drivers.AAVTimer.VideoCaptureImpl;
using OccuRec.Helpers;
using OccuRec.Utilities;

namespace OccuRec.Drivers.AAVTimer
{
    public class Video : IVideo, ISupportsCrossbar
	{
		public static string DRIVER_DESCRIPTION = "AAV Capture";

		private VideoCaptureImpl.VideoCapture camera;
        private IVideoCallbacks callbacksObject;

		public Video()
		{
			Properties.Settings.Default.Reload();

			camera = new VideoCaptureImpl.VideoCapture();
		}

		public bool Connected
		{
			get { return camera.IsConnected; }
			set
			{
				if (value != camera.IsConnected)
				{
					if (value)
					{
						if (camera.LocateCaptureDevice())
							camera.EnsureConnected();
					}
					else
						camera.EnsureDisconnected();
				}
			}
		}

		public string Description
		{
			get { return DRIVER_DESCRIPTION; }
		}

		public string DriverInfo
		{
			get
			{
				return string.Format(
					@"ADV Analogue Timer Video Driver ver {0}",
					Assembly.GetExecutingAssembly().GetName().Version);
			}
		}

		public string DriverVersion
		{
			get
			{
				Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
				return String.Format(CultureInfo.InvariantCulture, "{0}.{1}", version.Major, version.Minor);
			}
		}

		public short InterfaceVersion
		{
			get { return 1; }
		}

		public string Name
		{
			get { return DRIVER_DESCRIPTION; }
		}

		public string VideoCaptureDeviceName
		{
			get
			{
				return camera.DeviceName;
			}
		}

		public void SetupDialog()
		{ }

        public void SetCallbacks(IVideoCallbacks callbacksObject)
        {
            this.callbacksObject = callbacksObject;
            camera.CallbacksObject = callbacksObject;
        }

		private void AssertConnected()
		{
			if (!camera.IsConnected)
			    throw new NotConnectedException();
		}

		[DebuggerStepThrough]
		public string Action(string ActionName, string ActionParameters)
		{
            if (string.Compare(ActionName, "LockIntegration", StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                AssertConnected();
                return camera.LockIntegration().ToString(CultureInfo.InvariantCulture);
            }
            else if (string.Compare(ActionName, "UnlockIntegration", StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                AssertConnected();
                return camera.UnlockIntegration().ToString(CultureInfo.InvariantCulture);
            }
            else if (string.Compare(ActionName, "StartOcrTesting", StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                AssertConnected();
                return StartRecordingOcrTestFile(ActionParameters);
            }
            else if (string.Compare(ActionName, "StopOcrTesting", StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                AssertConnected();
                StopRecordingVideoFile();
                return true.ToString();
            }
            else if (string.Compare(ActionName, "DisableOcr", StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                AssertConnected();
                return camera.DisableOcr().ToString(CultureInfo.InvariantCulture);
            }
			else if (string.Compare(ActionName, "IntegrationCalibration", StringComparison.InvariantCultureIgnoreCase) == 0)
			{
				AssertConnected();
				int cameraIntegration = int.Parse(ActionParameters);
				return camera.StartIntegrationCalibration(cameraIntegration).ToString(CultureInfo.InvariantCulture);
			}
			else if (string.Compare(ActionName, "CancelIntegrationCalibration", StringComparison.InvariantCultureIgnoreCase) == 0)
			{
				AssertConnected();
				return camera.StopIntegrationCalibration().ToString(CultureInfo.InvariantCulture);
			}

		    return null;
		}

		public System.Collections.ArrayList SupportedActions
		{
			get
			{
				return new ArrayList(new string[]
				    {
				        "LockIntegration", "UnlockIntegration", "StartOcrTesting", "StopOcrTesting", 
                        "DisableOcr", "IntegrationCalibration", "CancelIntegrationCalibration"
				    });
			}
		}

		public void Dispose()
		{
			if (camera != null && camera.IsConnected)
			    camera.EnsureDisconnected();

			camera = null;
		}

		private double GetCameraExposureFromFrameRate()
		{
			return 1000.0 / camera.FrameRate;
		}

		public double ExposureMax
		{
			get { return GetCameraExposureFromFrameRate(); }
		}

		public double ExposureMin
		{
			get { return GetCameraExposureFromFrameRate(); }
		}

		public VideoCameraFrameRate FrameRate
		{
			get
			{
				if (Math.Abs(camera.FrameRate - 29.97) < 0.5)
					return VideoCameraFrameRate.NTSC;
				else if (Math.Abs(camera.FrameRate - 25) < 0.5)
					return VideoCameraFrameRate.PAL;
				else
					return VideoCameraFrameRate.Variable;
			}
		}

		public System.Collections.ArrayList SupportedIntegrationRates
		{
			[DebuggerStepThrough]
			get
			{
				throw new PropertyNotImplementedException("SupportedIntegrationRates");
			}
		}

		public int IntegrationRate
		{
			[DebuggerStepThrough]
			get
			{
				throw new PropertyNotImplementedException("IntegrationRate");
			}

			[DebuggerStepThrough]
			set
			{
				throw new PropertyNotImplementedException("IntegrationRate");
			}
		}

		public IVideoFrame LastVideoFrame
		{
			get
			{
				AssertConnected();

				VideoCameraFrame cameraFrame;

				if (camera.GetCurrentFrame(out cameraFrame))
				{
					VideoFrame rv = VideoFrame.CreateFrame(camera.ImageWidth, camera.ImageHeight, cameraFrame);
					return rv;
				}
				else
					throw new InvalidOperationException("No video frames are available.");
			}
		}

		public IVideoFrame LastVideoFrameImageArrayVariant
		{
			get
			{
				AssertConnected();

				VideoCameraFrame cameraFrame;

				if (camera.GetCurrentFrame(out cameraFrame))
				{
					VideoFrame rv = VideoFrame.CreateFrameVariant(camera.ImageWidth, camera.ImageHeight, cameraFrame);
					return rv;
				}
				else
					throw new InvalidOperationException("No video frames are available.");
			}
		}

		public string SensorName
		{
			[DebuggerStepThrough]
			get
			{
				throw new PropertyNotImplementedException("SensorName");
			}
		}

		public SensorType SensorType
		{
			get
			{
				return VideoCapture.SimulatedSensorType;
			}
		}

		public int CameraXSize
		{
			[DebuggerStepThrough]
			get
			{
				throw new PropertyNotImplementedException("CameraXSize");
			}
		}

		public int CameraYSize
		{
			[DebuggerStepThrough]
			get
			{
				throw new PropertyNotImplementedException("CameraYSize");
			}
		}

		public double PixelSizeX
		{
			[DebuggerStepThrough]
			get
			{
				throw new PropertyNotImplementedException("PixelSizeX");
			}
		}

		public int Width
		{
			get
			{
				AssertConnected();

				return camera.ImageWidth;
			}
		}

		public int Height
		{
			get
			{
				AssertConnected();

				return camera.ImageHeight;
			}
		}

		public double PixelSizeY
		{
			[DebuggerStepThrough]
			get
			{
				throw new PropertyNotImplementedException("PixelSizeY");
			}
		}

		public int BitDepth
		{
			get
			{
				AssertConnected();

				return camera.BitDepth;
			}
		}

		public string VideoCodec
		{
			get
			{
				return "AAV";
			}
		}

		public string VideoFileFormat
		{
			get { return "AAV"; }
		}

		public int VideoFramesBufferSize
		{
			get
			{
				return 1;
			}
		}

        public string StartRecordingOcrTestFile(string PreferredFileName)
        {
            AssertConnected();

            try
            {
                VideoCameraState currentState = camera.GetCurrentCameraState();

                if (currentState == VideoCameraState.videoCameraRecording)
                    throw new InvalidOperationException("The camera is already recording.");
                else if (currentState != VideoCameraState.videoCameraRunning)
                    throw new InvalidOperationException("The current state of the video camera doesn't allow a recording operation to begin right now.");

                string directory = Path.GetDirectoryName(PreferredFileName);
                string fileName = Path.GetFileName(PreferredFileName);

                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(fileName);

                if (File.Exists(PreferredFileName))
                    throw new DriverException(string.Format("File '{0}' already exists. Video can be recorded only in a non existing file.", PreferredFileName));

                return camera.StartOcrTestRecordingVideoFile(PreferredFileName);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.GetFullStackTrace());
                throw new DriverException("Error starting the recording: " + ex.Message, ex);
            }
        }

		public string StartRecordingVideoFile(string PreferredFileName)
		{
			AssertConnected();

			try
			{
				VideoCameraState currentState = camera.GetCurrentCameraState();

				if (currentState == VideoCameraState.videoCameraRecording)
					throw new InvalidOperationException("The camera is already recording.");
				else if (currentState != VideoCameraState.videoCameraRunning)
					throw new InvalidOperationException("The current state of the video camera doesn't allow a recording operation to begin right now.");

				string directory = Path.GetDirectoryName(PreferredFileName);
				string fileName = Path.GetFileName(PreferredFileName);

				if (!Directory.Exists(directory))
					Directory.CreateDirectory(fileName);

				if (File.Exists(PreferredFileName))
					throw new DriverException(string.Format("File '{0}' already exists. Video can be recorded only in a non existing file.", PreferredFileName));

				return camera.StartRecordingVideoFile(PreferredFileName);
			}
			catch (Exception ex)
			{
                Trace.WriteLine(ex.GetFullStackTrace());
				throw new DriverException("Error starting the recording: " + ex.Message, ex);
			}
		}

		public void StopRecordingVideoFile()
		{
			AssertConnected();

			try
			{
				VideoCameraState currentState = camera.GetCurrentCameraState();

				if (currentState != VideoCameraState.videoCameraRecording)
					throw new InvalidOperationException("The camera is currently not recording.");

				camera.StopRecordingVideoFile();

			}
			catch (Exception ex)
			{
                Trace.WriteLine(ex.GetFullStackTrace());
				throw new DriverException("Error stopping the recording: " + ex.Message, ex);
			}
		}

		public VideoCameraState CameraState
		{
			get
			{
				AssertConnected();

				return camera.GetCurrentCameraState();
			}
		}

		public short GainMax
		{
			[DebuggerStepThrough]
			get
			{
				throw new PropertyNotImplementedException("GainMax");
			}
		}

		public short GainMin
		{
			[DebuggerStepThrough]
			get
			{
				throw new PropertyNotImplementedException("GainMin");
			}
		}

		public short Gain
		{
			[DebuggerStepThrough]
			get
			{
				throw new PropertyNotImplementedException("Gain");
			}

			[DebuggerStepThrough]
			set
			{
				throw new PropertyNotImplementedException("Gain");
			}
		}

		public System.Collections.ArrayList Gains
		{
			[DebuggerStepThrough]
			get
			{
				throw new PropertyNotImplementedException("Gains");
			}
		}

		public int Gamma
		{
			[DebuggerStepThrough]
			get
			{
				throw new PropertyNotImplementedException("Gamma");
			}

			[DebuggerStepThrough]
			set
			{
				throw new PropertyNotImplementedException("Gamma");
			}
		}

		public System.Collections.ArrayList Gammas
		{
			[DebuggerStepThrough]
			get
			{
				throw new PropertyNotImplementedException("Gammas");
			}
		}


        public bool CanConfigureImage
        {
            get { return true; }
        }

        [DebuggerStepThrough]
        public void ConfigureImage()
        {
            AssertConnected();

            camera.ShowDeviceProperties();
        }

        public void ConnectToCrossbarSource(int inputPinIndex)
        {
            AssertConnected();

            camera.ConnectToCrossbarSource(inputPinIndex);
        }

        public void LoadCrossbarSources(ComboBox comboBox)
        {
            AssertConnected();

            camera.LoadCrossbarSources(comboBox);
        }
    }
}