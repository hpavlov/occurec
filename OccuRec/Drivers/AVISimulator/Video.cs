using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using OccuRec.Drivers.AAVTimer.VideoCaptureImpl;
using OccuRec.Drivers.AVISimulator.AVIPlayerImpl;
using OccuRec.Helpers;

namespace OccuRec.Drivers.AVISimulator
{
    public class Video : IVideo
    {
        public static string DRIVER_DESCRIPTION = "AVI Player";

        private AVIPlayer player;
        private bool fullAAVSimulation;

        public Video(bool fullAAVSimulation)
        {
            Properties.Settings.Default.Reload();

            this.fullAAVSimulation = fullAAVSimulation;
            player = new AVIPlayer(Properties.Settings.Default.SimulatorFilePath, Properties.Settings.Default.SimulatorFrameRate, fullAAVSimulation);
        }

        public bool Connected
        {
            get { return player.IsRunning; }
            set
            {
                if (value != player.IsRunning)
                {
                    if (value)
                    {
                        player.Start();
                        cameraState = VideoCameraState.videoCameraRunning;
                    }
                    else
                    {
                        player.Stop();
                        cameraState = VideoCameraState.videoCameraIdle;
                    }
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
                    @"AVI Player Driver ver {0}",
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
                return "AVI Playback";
            }
        }

        public void SetupDialog()
        { }

        public void SetCallbacks(IVideoCallbacks callbacksObject)
        {
            player.callbacksObject = callbacksObject;
        }

        private void AssertConnected()
        { }

        [DebuggerStepThrough]
        public string Action(string ActionName, string ActionParameters)
        {
            if (string.Compare(ActionName, "DisableOcr", StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                AssertConnected();
                return player.DisableOcr().ToString(CultureInfo.InvariantCulture);
            }

            if (fullAAVSimulation)
            {
                if (string.Compare(ActionName, "LockIntegration", StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    AssertConnected();
                    return player.LockIntegration().ToString(CultureInfo.InvariantCulture);
                }
                else if (string.Compare(ActionName, "UnlockIntegration", StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    AssertConnected();
                    return player.UnlockIntegration().ToString(CultureInfo.InvariantCulture);
                }
				if (string.Compare(ActionName, "IntegrationCalibration", StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					AssertConnected();
					int cameraIntegration = int.Parse(ActionParameters);
					return player.StartIntegrationCalibration(cameraIntegration).ToString(CultureInfo.InvariantCulture);
				}
				else if (string.Compare(ActionName, "CancelIntegrationCalibration", StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					AssertConnected();
					return player.StopIntegrationCalibration().ToString(CultureInfo.InvariantCulture);
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
            }
            
            throw new NotImplementedException();
        }

        public System.Collections.ArrayList SupportedActions
        {
            get
            {
                if (fullAAVSimulation)
					return new ArrayList(new string[] { "LockIntegration", "UnlockIntegration", "DisableOcr", "IntegrationCalibration", "CancelIntegrationCalibration", "StartOcrTesting", "StopOcrTesting", });
                else
                    return new ArrayList(new string[] { "DisableOcr" });
            }
        }


		private string StartRecordingOcrTestFile(string preferredFileName)
		{
			if (fullAAVSimulation)
			{
				if (cameraState == VideoCameraState.videoCameraRunning)
				{
					string directory = Path.GetDirectoryName(preferredFileName);
					string fileName = Path.GetFileName(preferredFileName);

					if (!Directory.Exists(directory))
						Directory.CreateDirectory(fileName);

					if (File.Exists(preferredFileName))
						throw new DriverException(string.Format("File '{0}' already exists. Video can be recorded only in a non existing file.", preferredFileName));

                    NativeHelpers.StartOcrTestRecording(preferredFileName);
					cameraState = VideoCameraState.videoCameraRecording;

					return preferredFileName;
				}
				else
					throw new DriverException("Camera not running.");
			}
			else
				throw new NotSupportedException();
		}

        public void Dispose()
        {
            if (player != null && player.IsRunning)
                player.Stop();

            player = null;
        }

        private double GetCameraExposureFromFrameRate()
        {
            return 40;
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

                Bitmap cameraFrame;
                int frameNumber;
                FrameProcessingStatus status;

                if (player.GetCurrentFrame(out cameraFrame, out frameNumber, out status))
                {
                    BasicVideoFrame rv = BasicVideoFrame.CreateFrame(player.ImageWidth, player.ImageHeight, cameraFrame, frameNumber, status);
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

                Bitmap cameraFrame;
                int frameNumber;
                FrameProcessingStatus status;

                if (player.GetCurrentFrame(out cameraFrame, out frameNumber, out status))
                {
                    BasicVideoFrame rv = BasicVideoFrame.CreateFrameVariant(player.ImageWidth, player.ImageHeight, cameraFrame, frameNumber, status);
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

                return player.ImageWidth;
            }
        }

        public int Height
        {
            get
            {
                AssertConnected();

                return player.ImageHeight;
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

                return player.BitDepth;
            }
        }

        public string VideoCodec
        {
            get
            {
                return fullAAVSimulation ? "AAV" : "N/A";
            }
        }

        public string VideoFileFormat
        {
            get { return fullAAVSimulation ? "AAV" : "N/A"; }
        }

        public int VideoFramesBufferSize
        {
            get
            {
                return 1;
            }
        }

        private VideoCameraState cameraState = VideoCameraState.videoCameraIdle;

        public string StartRecordingVideoFile(string PreferredFileName)
        {
            if (fullAAVSimulation)
            {
                if (cameraState == VideoCameraState.videoCameraRunning)
                {
                    string directory = Path.GetDirectoryName(PreferredFileName);
                    string fileName = Path.GetFileName(PreferredFileName);

                    if (!Directory.Exists(directory))
                        Directory.CreateDirectory(fileName);

                    if (File.Exists(PreferredFileName))
                        throw new DriverException(string.Format("File '{0}' already exists. Video can be recorded only in a non existing file.", PreferredFileName));

                    NativeHelpers.StartRecordingVideoFile(PreferredFileName);

                    cameraState = VideoCameraState.videoCameraRecording;

                    return PreferredFileName;                    
                }
                else
                    throw new DriverException("Camera not running.");
            }
            else
                throw new NotSupportedException();
        }

        public void StopRecordingVideoFile()
        {
            if (fullAAVSimulation && cameraState == VideoCameraState.videoCameraRecording)
            {
                NativeHelpers.StopRecordingVideoFile();

                cameraState = VideoCameraState.videoCameraRunning;
            }
        }

        public VideoCameraState CameraState
        {
            get
            {
                AssertConnected();

                return cameraState;
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
            get { return false; }
        }

        [DebuggerStepThrough]
        public void ConfigureImage()
        { }
    }
}
