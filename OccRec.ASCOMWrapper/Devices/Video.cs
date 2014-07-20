using System;
using System.Collections;
using System.Collections.Generic;
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


        public void Configure()
        {
            m_IsolatedVideo.Configure();
        }

        public int Width
        {
            get { return m_IsolatedVideo.Width; }
        }

        public int Height
        {
            get { return m_IsolatedVideo.Height; }
        }

        public int BitDepth
        {
            get { return m_IsolatedVideo.BitDepth; }
        }

        public ArrayList SupportedActions
        {
            get { return m_IsolatedVideo.SupportedActions; }
        }

        public int CameraState
        {
            get { return m_IsolatedVideo.CameraState; }
        }

        public bool CanConfigureImage
        {
            get { return m_IsolatedVideo.CanConfigureImage; }
        }

        public string VideoFileFormat
        {
            get { return m_IsolatedVideo.VideoFileFormat; }
        }

        public string VideoCodec
        {
            get { return m_IsolatedVideo.VideoCodec; }
        }

        public string Name
        {
            get { return m_IsolatedVideo.Name; }
        }

        public string VideoCaptureDeviceName
        {
            get { return m_IsolatedVideo.VideoCaptureDeviceName; }
        }

        public IASCOMVideoFrame LastVideoFrame
        {
            get { return m_IsolatedVideo.LastVideoFrame; }
        }
    }
}
