using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using AAVRec.Helpers;
using AAVRec.Properties;
using DirectShowLib;

namespace AAVRec.Drivers.DirectShowCapture.VideoCaptureImpl
{
	internal class VideoCapture
	{
		private DsDevice videoInputDevice;
		private SystemCodecEntry videoCompressor;

		private DirectShowCapture dsCapture = new DirectShowCapture();

		private float frameRate;
		private int imageWidth;
		private int imageHeight;

		private ICameraImage cameraImageHelper = new CameraImage();

		private VideoCameraState cameraState = VideoCameraState.videoCameraIdle;

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
			FindInputAndCompressorToUse(out videoInputDevice, out videoCompressor);

			return videoInputDevice != null;
		}

		public void EnsureConnected()
		{
			if (!IsConnected)
			{
				dsCapture.CloseResources();

				// TODO: Set a preferred frameRate and image size stored in the configuration
                dsCapture.SetupPreviewOnlyGraph(videoInputDevice, new VideoFormatHelper.SupportedVideoFormat(Settings.Default.SelectedVideoFormat), ref frameRate, ref imageWidth, ref imageHeight);

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
			if (IsConnected && (videoInputDevice != null || videoCompressor != null))
			{
				DsDevice inputDevice;
				SystemCodecEntry compressor;

				FindInputAndCompressorToUse(out inputDevice, out compressor);

				bool reconnect = false;

				if (inputDevice != null && 
					videoInputDevice != null && 
					videoInputDevice.DevicePath != inputDevice.DevicePath)
				{
					// We have a new video device. Will need to reconnect to it
					reconnect = true;
				}

				if (compressor != null &&
					videoCompressor != null &&
					videoCompressor.Codec != compressor.Codec)
				{
					// We have a new compressor device. Will need to reconnect
					reconnect = true;
				}

				if (reconnect)
				{
					EnsureDisconnected();

					videoInputDevice = inputDevice;
					videoCompressor = compressor;

					EnsureConnected();
				}
			}
		}

		private void FindInputAndCompressorToUse(out DsDevice inputDevice, out SystemCodecEntry compressor)
		{
			inputDevice = null;
			compressor = null;

			List<DsDevice> allInputDevices = new List<DsDevice>(DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice));

			if (!string.IsNullOrEmpty(Settings.Default.PreferredCaptureDevice))
				inputDevice = allInputDevices.FirstOrDefault(x => x.Name == Settings.Default.PreferredCaptureDevice);
			else if (allInputDevices.Count > 0)
				inputDevice = allInputDevices[0];

			compressor = VideoCodecs.GetSupportedVideoCodec(Settings.Default.PreferredCompressorDevice);
		}

		public static SensorType SimulatedSensorType
		{
			get { return SensorType.Color; }
		}

		public bool GetCurrentFrame(out VideoCameraFrame cameraFrame)
		{
			long frameId;
			Bitmap bmp = dsCapture.GetNextFrame(out frameId);

			if (bmp != null)
			{
				using (bmp)
				{
                    object pixels = cameraImageHelper.GetImageArray(bmp, SimulatedSensorType, (LumaConversionMode)Settings.Default.MonochromePixelsType, Settings.Default.FlipHorizontally, Settings.Default.FlipVertically);

					cameraFrame = new VideoCameraFrame()
					{
						FrameNumber = frameId,
						Pixels = pixels,
						ImageLayout = VideoFrameLayout.Color
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
				dsCapture.CloseResources();

            dsCapture.SetupFileRecorderGraph(videoInputDevice, videoCompressor, new VideoFormatHelper.SupportedVideoFormat(Settings.Default.SelectedVideoFormat), ref frameRate, ref imageWidth, ref imageHeight, preferredFileName);

			dsCapture.Start();

			cameraState = dsCapture.IsRunning ? VideoCameraState.videoCameraRecording : VideoCameraState.videoCameraIdle;

			return preferredFileName;
		}

		public void StopRecordingVideoFile()
		{
			EnsureDisconnected();

			EnsureConnected();
		}

		public string GetUsedAviFourCC()
		{
			return videoCompressor.FourCC;
		}

		public void ShowDeviceProperties()
		{
			if (IsConnected && videoInputDevice != null)
			{
				dsCapture.ShowDeviceProperties();
			}
		}

        public void ConnectToCrossbarSource(int inputPinIndex)
        {
            dsCapture.ConnectToCrossbarSource(inputPinIndex);
        }

        public void LoadCrossbarSources(ComboBox comboBox)
        {
            dsCapture.LoadCrossbarSources(comboBox);
        }

        public string ToggleIotaVtiOcrTesting()
        {
            dsCapture.ToggleIotaVtiOcrTesting();

            return null;
        }
	}
}
