using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using ASCOM;
using ASCOM.DeviceInterface;
using OccuRec.ASCOM.Interfaces.Devices;
using OccuRec.Utilities;

namespace OccuRec.ASCOM.Server
{
	[Serializable]
	public class IsolatedVideo : IsolatedDevice, IASCOMVideo
	{
		private global::ASCOM.DriverAccess.Video m_Video;

		internal IsolatedVideo(string progId)
		{
            m_Video = new global::ASCOM.DriverAccess.Video(progId);

			SetIsolatedDevice(m_Video, progId);
		}

		public VideoState GetCurrentState()
		{
			var state = new VideoState();

			// TODO: Populate the state from the m_Video object

			return state;
		}

        public void Configure()
        {
            try
            {
                m_Video.SetupDialog();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.GetFullStackTrace());
            }
        }

        public int Width
        {
            get { return m_Video.Width; }
        }

        public int Height
        {
            get { return m_Video.Height; }
        }

        public int BitDepth
        {
            get { return m_Video.BitDepth; }
        }

	    public bool CanConfigureImage
	    {
            get { return m_Video.CanConfigureDeviceProperties; }
	    }

	    public ArrayList SupportedActions
	    {
            get { return m_Video.SupportedActions; }
	    }

	    public int CameraState
	    {
            get { return (int)m_Video.CameraState; }
	    }

        public string VideoFileFormat
        {
            get { return m_Video.VideoFileFormat; }
        }

        public string VideoCodec
        {
            get { return m_Video.VideoCodec; }
        }

        public string Name
        {
            get { return m_Video.Name; }
        }

        public string VideoCaptureDeviceName
        {
            get { return m_Video.VideoCaptureDeviceName; }
        }

        public IASCOMVideoFrame LastVideoFrame
        {
			[DebuggerStepThrough]
            get
            {
				try
				{
					return new IsolatedVideoFrame(m_Video.LastVideoFrame);
				}
				catch (NotConnectedException)
				{
					return null;
				}
            }
        }

		public short InterfaceVersion
		{
			get { return m_Video.InterfaceVersion; }
		}


		public double ExposureMax
		{
			get { return m_Video.ExposureMax; }
		}

		public double ExposureMin
		{
			get { return m_Video.ExposureMin; }
		}

		public int FrameRate
		{
			get { return (int)m_Video.FrameRate; }
		}

		public ArrayList SupportedIntegrationRates
		{
			get { return m_Video.SupportedIntegrationRates; }
		}

		public int IntegrationRate
		{
			get { return m_Video.IntegrationRate; }
			set { m_Video.IntegrationRate = value; }
		}

		public string Action(string actionName, string actionParameters)
		{
			return m_Video.Action(actionName, actionParameters);
		}

		public string StartRecordingVideoFile(string preferredFileName)
		{
			return m_Video.StartRecordingVideoFile(preferredFileName);
		}

		public void StopRecordingVideoFile()
		{
			m_Video.StopRecordingVideoFile();
		}

		public void ConfigureImage()
		{
			m_Video.ConfigureDeviceProperties();
		}

		public string SensorName
		{
			get { return m_Video.SensorName; }
		}

		public int SensorType
		{
			get { return (int)m_Video.SensorType; }
		}

		public double PixelSizeX
		{
			get { return m_Video.PixelSizeX; }
		}

		public double PixelSizeY
		{
			get { return m_Video.PixelSizeY; }
		}

		public int VideoFramesBufferSize
		{
			get { return m_Video.VideoFramesBufferSize; }
		}

		public short GainMax
		{
			get { return m_Video.GainMax; }
		}

		public short GainMin
		{
			get { return m_Video.GainMin; }
		}

		public short Gain
		{
			get { return m_Video.Gain; }
			set { m_Video.Gain = value; }
		}

		public ArrayList Gains
		{
			get { return m_Video.Gains; }
		}

		public short Gamma
		{
			get { return m_Video.Gamma; }
			set { m_Video.Gamma = value; }
		}

		public ArrayList Gammas
		{
			get { return m_Video.Gammas; }
		}
    }
}
