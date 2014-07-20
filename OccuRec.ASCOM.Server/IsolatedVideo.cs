using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
            get { return new IsolatedVideoFrame(m_Video.LastVideoFrame); }
        }
    }
}
