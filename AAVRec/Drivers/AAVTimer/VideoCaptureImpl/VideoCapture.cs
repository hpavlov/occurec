using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using AAVRec.Drivers.DirectShowCapture;
using AAVRec.Helpers;
using AAVRec.Properties;
using DirectShowLib;

namespace AAVRec.Drivers.AAVTimer.VideoCaptureImpl
{
	internal class VideoCapture
	{
		private DsDevice videoInputDevice;

		private DirectShowCapture dsCapture = new DirectShowCapture();

		private float frameRate;
		private int imageWidth;
		private int imageHeight;

		private ICameraImage cameraImageHelper = new CameraImage();

		private VideoCameraState cameraState = VideoCameraState.videoCameraIdle;

	    private IVideoCallbacks callbacksObject;
        internal IVideoCallbacks CallbacksObject
	    {
            get { return callbacksObject; }
            set
	        {
                callbacksObject = value;
                dsCapture.callbacksObject = value;
	        }
	    }

		public bool IsConnected
		{
			get { return dsCapture.IsRunning; }
		}

		public string DeviceName
		{
			get
			{
				if (videoInputDevice != null)
					return videoInputDevice.Name;
				else
					return string.Empty;
			}
		}

		public int ImageWidth
		{
			get { return imageWidth; }
		}

		public int ImageHeight
		{
			get { return imageHeight; }
		}

		public float FrameRate
		{
			get { return frameRate; }
		}

		public int BitDepth
		{
			get { return 8; }
		}

		public bool LocateCaptureDevice()
		{
			videoInputDevice = FindInputAndCompressorToUse();

			return videoInputDevice != null;
		}

		public void EnsureConnected()
		{
			if (!IsConnected)
			{
				dsCapture.CloseResources();

				// TODO: Set a preferred frameRate and image size stored in the configuration

                dsCapture.SetupGraph(videoInputDevice, Settings.Default.IotaVtiOcrEnabled, new VideoFormatHelper.SupportedVideoFormat(Settings.Default.SelectedVideoFormat), ref frameRate, ref imageWidth, ref imageHeight);

				dsCapture.Start();

				cameraState = dsCapture.IsRunning ? VideoCameraState.videoCameraRunning : VideoCameraState.videoCameraIdle;
			}
		}

		public void EnsureDisconnected()
		{
			dsCapture.Pause();

			dsCapture.CloseResources();

			cameraState = VideoCameraState.videoCameraIdle;
		}

		public void ReloadSettings()
		{
			if (IsConnected && videoInputDevice != null)
			{
				DsDevice inputDevice = FindInputAndCompressorToUse();

				if (inputDevice != null && 
					videoInputDevice != null && 
					videoInputDevice.DevicePath != inputDevice.DevicePath)
				{
					EnsureDisconnected();

					videoInputDevice = inputDevice;

					EnsureConnected();
				}
			}
		}

		private DsDevice FindInputAndCompressorToUse()
		{
			DsDevice inputDevice = null;

			List<DsDevice> allInputDevices = new List<DsDevice>(DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice));

			if (!string.IsNullOrEmpty(Settings.Default.PreferredCaptureDevice))
				inputDevice = allInputDevices.FirstOrDefault(x => x.Name == Settings.Default.PreferredCaptureDevice);
			else if (allInputDevices.Count > 0)
				inputDevice = allInputDevices[0];

			return inputDevice;
		}

		public static SensorType SimulatedSensorType
		{
			get
			{
                return SensorType.Monochrome;
			}
		}

		public bool GetCurrentFrame(out VideoCameraFrame cameraFrame)
		{
		    ImageStatus status;
			Bitmap bmp = dsCapture.GetNextFrame(out status);

			if (bmp != null)
			{
				using (bmp)
				{
                    object pixels = cameraImageHelper.GetImageArray(bmp, SimulatedSensorType, Settings.Default.MonochromePixelsType, Settings.Default.FlipHorizontally, Settings.Default.FlipVertically);


					cameraFrame = new VideoCameraFrame()
					{
                        FrameNumber = status.IntegratedFrameNo,
                        UniqueFrameNumber = status.UniqueFrameNo,
                        ImageStatus = status,
						Pixels = pixels,                        
						ImageLayout = VideoFrameLayout.Monochrome
					};
				}

				return true;
			}

			cameraFrame = null;
			return false;
		}

		public VideoCameraState GetCurrentCameraState()
		{
			return cameraState;
		}

		public string StartRecordingVideoFile(string preferredFileName)
		{
			if (dsCapture.IsRunning)
			{
                if (Path.GetExtension(preferredFileName) != ".aav")
                    preferredFileName = Path.ChangeExtension(preferredFileName, ".aav");

				NativeHelpers.StartRecordingVideoFile(preferredFileName);

				cameraState = VideoCameraState.videoCameraRecording;

				return preferredFileName;
			}

			throw new InvalidOperationException();
		}

		public void StopRecordingVideoFile()
		{
			NativeHelpers.StopRecordingVideoFile();

			EnsureDisconnected();
			EnsureConnected();
		}

        public void ShowDeviceProperties()
        {
            if (IsConnected && videoInputDevice != null)
            {
                dsCapture.ShowDeviceProperties();
            }
        }

        public bool LockIntegration()
        {
            return NativeHelpers.LockIntegration();
        }

        public bool UnlockIntegration()
        {
            return NativeHelpers.UnlockIntegration();
        }

        public void ConnectToCrossbarSource(int inputPinIndex)
        {
            dsCapture.ConnectToCrossbarSource(inputPinIndex);
        }

        public void LoadCrossbarSources(ComboBox comboBox)
        {
            dsCapture.LoadCrossbarSources(comboBox);
        }

        public bool StartIotaVtiOcrTesting()
        {
            return true;
        }

        public bool StopIotaVtiOcrTesting()
        {
            return true;
        }

        public bool DisableOcr()
        {
            return dsCapture.DisableOcr();
        }
	}
}
