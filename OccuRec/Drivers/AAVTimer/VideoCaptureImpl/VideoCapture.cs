﻿/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using OccuRec.Helpers;
using OccuRec.Properties;
using DirectShowLib;
using OccuRec.Context;

namespace OccuRec.Drivers.AAVTimer.VideoCaptureImpl
{
	internal class VideoCapture
	{
		private DsDevice videoInputDevice;

		private DirectShowCapture dsCapture = new DirectShowCapture();

		private float frameRate;
		private int imageWidth;
		private int imageHeight;

		private ICameraImage cameraImageHelper = new CameraImage();

		private VideoCameraState cameraState = VideoCameraState.videoCameraRunning;

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
                OccuRecContext.Current.FailedToSetRequestedMode = false;
			    OccuRecContext.Current.StandardVideoModeSet = null;

				dsCapture.CloseResources();
                Trace.WriteLine(string.Format("VideoCapture: User selected video mode: {0}", Settings.Default.SelectedVideoFormat));

			    var formatToSet = new VideoFormatHelper.SupportedVideoFormat(Settings.Default.SelectedVideoFormat);
                Trace.WriteLine(string.Format("VideoCapture: Attempting to set video mode: {0}", formatToSet.AsSerialized()));

                dsCapture.SetupGraph(videoInputDevice, Settings.Default.AavOcrEnabled, formatToSet, ref frameRate, ref imageWidth, ref imageHeight);

                var formatSet = new VideoFormatHelper.SupportedVideoFormat() { FrameRate = frameRate, Width = imageWidth, Height = imageHeight };

                Trace.WriteLine(string.Format("VideoCapture: Actual video mode set: {0}", formatSet.ToString()));

			    if (Math.Abs(formatToSet.FrameRate - frameRate) > 0.01 || formatToSet.Width != imageWidth || formatToSet.Height != imageHeight)
			    {
			        OccuRecContext.Current.FailedToSetRequestedMode = true;
			    }

			    OccuRecContext.Current.VideoModeSet = formatSet.AsSerialized();
                if (formatSet.IsPal())
			        OccuRecContext.Current.StandardVideoModeSet = "PAL";
                else if (formatSet.IsNtsc())
                    OccuRecContext.Current.StandardVideoModeSet = "NTSC";

				dsCapture.Start();

				cameraState = VideoCameraState.videoCameraRunning;
			}
		}

		public void EnsureDisconnected()
		{
			dsCapture.Pause();

			dsCapture.CloseResources();

			cameraState = VideoCameraState.videoCameraRunning;
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
				object pixels = cameraImageHelper.GetImageArray(bmp, SimulatedSensorType, LumaConversionMode.R, Settings.Default.HorizontalFlip, Settings.Default.VerticalFlip);
                //object pixels = ImageUtils.GetPixelArray(bmp.Width, bmp.Height, bmp);
                cameraFrame = new VideoCameraFrame()
                {
                    FrameNumber = status.IntegratedFrameNo,
                    UniqueFrameNumber = status.UniqueFrameNo,
                    ImageStatus = status,
					Pixels = pixels,
                    PreviewBitmap = bmp,
                    ImageLayout = VideoFrameLayout.Monochrome
                };

				return true;
			}

			cameraFrame = null;
			return false;
		}

		public VideoCameraState GetCurrentCameraState()
		{
			return cameraState;
		}


        public string StartOcrTestRecordingVideoFile(string preferredFileName)
		{
			if (dsCapture.IsRunning)
			{
                if (Path.GetExtension(preferredFileName) != ".aav")
                    preferredFileName = Path.ChangeExtension(preferredFileName, ".aav");

                NativeHelpers.StartOcrTestRecording(preferredFileName);

				cameraState = VideoCameraState.videoCameraRecording;

				return preferredFileName;
			}

			throw new InvalidOperationException();
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
            bool rv = NativeHelpers.UnlockIntegration();

            NativeHelpers.SetManualIntegrationRateHint(0);
            NativeHelpers.SetStackRate(1);

            return rv;
        }

        public void ConnectToCrossbarSource(int inputPinIndex)
        {
            dsCapture.ConnectToCrossbarSource(inputPinIndex);
        }

        public void LoadCrossbarSources(ComboBox comboBox)
        {
            dsCapture.LoadCrossbarSources(comboBox);
        }

        public bool DisableOcr()
        {
            return dsCapture.DisableOcr();
        }

		public bool StartIntegrationCalibration(int cameraIntegration)
		{
			return NativeHelpers.StartIntegrationCalibration(cameraIntegration);
		}

		public bool StopIntegrationCalibration()
		{
			return NativeHelpers.StopIntegrationCalibration();
		}
	}
}
