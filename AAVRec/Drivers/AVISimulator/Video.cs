using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using AAVRec.Drivers.AAVTimer.VideoCaptureImpl;
using AAVRec.Drivers.AVISimulator.AVIPlayerImpl;

namespace AAVRec.Drivers.AVISimulator
{
    public class Video : IVideo
    {
        private static string DRIVER_DESCRIPTION = "AVI Player";

        private AVIPlayer player;

        public Video()
        {
            Properties.Settings.Default.Reload();

            player = new AVIPlayer(Properties.Settings.Default.SimulatorFilePath, Properties.Settings.Default.SimulatorFrameRate);
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
                    }
                    else
                        player.Stop();
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
        { }

        private void AssertConnected()
        { }

        [DebuggerStepThrough]
        public string Action(string ActionName, string ActionParameters)
        {
            throw new NotImplementedException();
        }

        public System.Collections.ArrayList SupportedActions
        {
            get
            {
                return new ArrayList(new string[] { "LockIntegration", "UnlockIntegration" });
            }
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

                if (player.GetCurrentFrame(out cameraFrame, out frameNumber))
                {
                    BasicVideoFrame rv = BasicVideoFrame.CreateFrame(player.ImageWidth, player.ImageHeight, cameraFrame, frameNumber);
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

                if (player.GetCurrentFrame(out cameraFrame, out frameNumber))
                {
                    BasicVideoFrame rv = BasicVideoFrame.CreateFrameVariant(player.ImageWidth, player.ImageHeight, cameraFrame, frameNumber);
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
                return "AVI";
            }
        }

        public string VideoFileFormat
        {
            get { return "AVI"; }
        }

        public int VideoFramesBufferSize
        {
            get
            {
                return 1;
            }
        }

        public string StartRecordingVideoFile(string PreferredFileName)
        {
            throw new NotImplementedException();
        }

        public void StopRecordingVideoFile()
        {
            throw new NotImplementedException();
        }

        public VideoCameraState CameraState
        {
            get
            {
                AssertConnected();

                return VideoCameraState.videoCameraRunning;
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
