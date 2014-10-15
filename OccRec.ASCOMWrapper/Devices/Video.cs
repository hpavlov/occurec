/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using OccuRec.ASCOM.Interfaces.Devices;
using OccuRec.ASCOM.Wrapper.Interfaces;

namespace OccuRec.ASCOM.Wrapper.Devices
{
	internal class Video : DeviceBase, IVideoWrapper
	{
		private IASCOMVideo m_IsolatedVideo;

		internal Video(IASCOMVideo isolatedVideo)
			: base(isolatedVideo)
		{
			m_IsolatedVideo = isolatedVideo;
		}
	
		public VideoState GetCurrentState()
		{
			return m_IsolatedVideo.GetCurrentState();
		}


		[DebuggerStepThrough]
        public void Configure()
        {
            m_IsolatedVideo.Configure();
        }

        public int Width
        {
			[DebuggerStepThrough]
            get { return m_IsolatedVideo.Width; }
        }

        public int Height
        {
			[DebuggerStepThrough]
            get { return m_IsolatedVideo.Height; }
        }

        public int BitDepth
        {
			[DebuggerStepThrough]
            get { return m_IsolatedVideo.BitDepth; }
        }

        public ArrayList SupportedActions
        {
			[DebuggerStepThrough]
            get { return m_IsolatedVideo.SupportedActions; }
        }

        public int CameraState
        {
			[DebuggerStepThrough]
            get { return m_IsolatedVideo.CameraState; }
        }

        public bool CanConfigureImage
        {
			[DebuggerStepThrough]
            get { return m_IsolatedVideo.CanConfigureImage; }
        }

        public string VideoFileFormat
        {
			[DebuggerStepThrough]
            get { return m_IsolatedVideo.VideoFileFormat; }
        }

        public string VideoCodec
		{
			[DebuggerStepThrough]
            get { return m_IsolatedVideo.VideoCodec; }
        }

        public string Name
        {
			[DebuggerStepThrough]
            get { return m_IsolatedVideo.Name; }
        }

        public string VideoCaptureDeviceName
        {
			[DebuggerStepThrough]
            get { return m_IsolatedVideo.VideoCaptureDeviceName; }
        }

        public IASCOMVideoFrame LastVideoFrame
        {
			[DebuggerStepThrough]
            get { return m_IsolatedVideo.LastVideoFrame; }
        }

		public short InterfaceVersion
		{
			[DebuggerStepThrough]
			get { return m_IsolatedVideo.InterfaceVersion; }
		}

		public double ExposureMax
		{
			[DebuggerStepThrough]
			get { return m_IsolatedVideo.ExposureMax; }
		}

		public double ExposureMin
		{
			[DebuggerStepThrough]
			get { return m_IsolatedVideo.ExposureMin; }
		}

		public int FrameRate
		{
			[DebuggerStepThrough]
			get { return (int)m_IsolatedVideo.FrameRate; }
		}

		public ArrayList SupportedIntegrationRates
		{
			[DebuggerStepThrough]
			get { return m_IsolatedVideo.SupportedIntegrationRates; }
		}

		public int IntegrationRate
		{
			[DebuggerStepThrough]
			get { return m_IsolatedVideo.IntegrationRate; }

			[DebuggerStepThrough]
			set { m_IsolatedVideo.IntegrationRate = value; }
		}

		[DebuggerStepThrough]
		public string Action(string actionName, string actionParameters)
		{
			return m_IsolatedVideo.Action(actionName, actionParameters);
		}

		[DebuggerStepThrough]
		public string StartRecordingVideoFile(string preferredFileName)
		{
			return m_IsolatedVideo.StartRecordingVideoFile(preferredFileName);
		}

		[DebuggerStepThrough]
		public void StopRecordingVideoFile()
		{
			m_IsolatedVideo.StopRecordingVideoFile();
		}

		[DebuggerStepThrough]
		public void ConfigureImage()
		{
			m_IsolatedVideo.ConfigureImage();
		}

		public string SensorName
		{
			[DebuggerStepThrough]
			get { return m_IsolatedVideo.SensorName; }
		}

		public int SensorType
		{
			[DebuggerStepThrough]
			get { return (int)m_IsolatedVideo.SensorType; }
		}

		public double PixelSizeX
		{
			[DebuggerStepThrough]
			get { return m_IsolatedVideo.PixelSizeX; }
		}

		public double PixelSizeY
		{
			[DebuggerStepThrough]
			get { return m_IsolatedVideo.PixelSizeY; }
		}

		public int VideoFramesBufferSize
		{
			[DebuggerStepThrough]
			get { return m_IsolatedVideo.VideoFramesBufferSize; }
		}

		public short GainMax
		{
			[DebuggerStepThrough]
			get { return m_IsolatedVideo.GainMax; }
		}

		public short GainMin
		{
			[DebuggerStepThrough]
			get { return m_IsolatedVideo.GainMin; }
		}

		public short Gain
		{
			[DebuggerStepThrough]
			get { return m_IsolatedVideo.Gain; }

			[DebuggerStepThrough]
			set { m_IsolatedVideo.Gain = value; }
		}

		public ArrayList Gains
		{
			[DebuggerStepThrough]
			get { return m_IsolatedVideo.Gains; }
		}

		public short Gamma
		{
			[DebuggerStepThrough]
			get { return m_IsolatedVideo.Gamma; }

			[DebuggerStepThrough]
			set { m_IsolatedVideo.Gamma = value; }
		}

		public ArrayList Gammas
		{
			[DebuggerStepThrough]
			get { return m_IsolatedVideo.Gammas; }
		}
    }
}
