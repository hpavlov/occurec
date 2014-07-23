using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OccuRec.ASCOM.Wrapper;
using OccuRec.ASCOM.Wrapper.Interfaces;

namespace OccuRec.Drivers.ASCOMVideo
{
    public class Video : IVideo
    {
        private string m_ProgId;
        private IVideoWrapper m_ASCOMVideo;
        private IVideoCallbacks m_CallbacksObject;

        public Video(string progId)
        {
            m_ProgId = progId;
            m_ASCOMVideo = ASCOMClient.Instance.CreateVideo(m_ProgId);
        }

        public bool Connected
        {
            get { return m_ASCOMVideo.Connected; }
            set { m_ASCOMVideo.Connected = value; }
        }

        public string Description
        {
            get { return m_ASCOMVideo.Description; }
        }

        public string DriverInfo
        {
            get { return m_ASCOMVideo.DriverVersion; }
        }

        public string DriverVersion
        {
            get { return m_ASCOMVideo.DriverVersion; }
        }

        public short InterfaceVersion
        {
			get { return m_ASCOMVideo.InterfaceVersion; }
        }

        public string Name
        {
            get { return m_ASCOMVideo.Name; }
        }

        public string VideoCaptureDeviceName
        {
            get { return m_ASCOMVideo.VideoCaptureDeviceName; }
        }

        public void SetupDialog()
        {
            m_ASCOMVideo.Configure();
        }

        public void SetCallbacks(IVideoCallbacks callbacksObject)
        {
            m_CallbacksObject = callbacksObject;
        }

        public string Action(string actionName, string actionParameters)
        {
	        return m_ASCOMVideo.Action(actionName, actionParameters);
        }

        public ArrayList SupportedActions
        {
            get { return m_ASCOMVideo.SupportedActions; }
        }

        public void Dispose()
        {
			if (m_ASCOMVideo != null)
			{
				m_ASCOMVideo.Connected = false;
				ASCOMClient.Instance.ReleaseDevice(m_ASCOMVideo);
				m_ASCOMVideo = null;
			}
        }

        public double ExposureMax
        {
			get { return m_ASCOMVideo.ExposureMax; }
        }

        public double ExposureMin
        {
			get { return m_ASCOMVideo.ExposureMin; }
        }

        public VideoCameraFrameRate FrameRate
        {
			get { return (VideoCameraFrameRate)m_ASCOMVideo.FrameRate; }
        }

        public ArrayList SupportedIntegrationRates
        {
			get { return m_ASCOMVideo.SupportedIntegrationRates; }
        }

        public int IntegrationRate
        {
			get { return m_ASCOMVideo.IntegrationRate; }
			set { m_ASCOMVideo.IntegrationRate = value; }
        }

        private VideoFrame m_LastVideoFrame = null;
        private object m_SyncLock = new object();

        public IVideoFrame LastVideoFrame
        {
            get
            {
                lock (m_SyncLock)
                {
                    if (m_LastVideoFrame != null)
                    {
                        try
                        {
                            m_LastVideoFrame.Dispose();
                        }
                        finally
                        {
                            m_LastVideoFrame = null;
                        }
                    }

                    m_LastVideoFrame = new VideoFrame(m_ASCOMVideo.LastVideoFrame);
                }

                return m_LastVideoFrame;
            }
        }

        public IVideoFrame LastVideoFrameImageArrayVariant
        {
            get
            {
                lock (m_SyncLock)
                {
                    if (m_LastVideoFrame != null)
                    {
                        try
                        {
                            m_LastVideoFrame.Dispose();
                        }
                        finally
                        {
                            m_LastVideoFrame = null;
                        }
                    }

                    m_LastVideoFrame = new VideoFrame(m_ASCOMVideo.LastVideoFrame);
                }

                return m_LastVideoFrame;
            }
        }

        public string SensorName
        {
			get { return m_ASCOMVideo.SensorName; }
        }

        public SensorType SensorType
        {
			get { return (SensorType)m_ASCOMVideo.SensorType; }
        }

        public int CameraXSize
        {
			get { throw new NotImplementedException(); }
        }

        public int CameraYSize
        {
			get { throw new NotImplementedException(); }
        }

        public int Width
        {
            get { return m_ASCOMVideo.Width; }
        }

        public int Height
        {
            get { return m_ASCOMVideo.Height; }
        }

        public double PixelSizeX
        {
			get { return m_ASCOMVideo.PixelSizeX; }
        }

        public double PixelSizeY
        {
			get { return m_ASCOMVideo.PixelSizeY; }
        }

        public int BitDepth
        {
            get { return m_ASCOMVideo.BitDepth; }
        }

        public string VideoCodec
        {
            get { return m_ASCOMVideo.VideoCodec; }
        }

        public string VideoFileFormat
        {
            get { return m_ASCOMVideo.VideoFileFormat; }
        }

        public int VideoFramesBufferSize
        {
			get { return m_ASCOMVideo.VideoFramesBufferSize; }
        }

        public string StartRecordingVideoFile(string preferredFileName)
        {
			return m_ASCOMVideo.StartRecordingVideoFile(preferredFileName);
        }

        public void StopRecordingVideoFile()
        {
	        m_ASCOMVideo.StopRecordingVideoFile();
        }

        public VideoCameraState CameraState
        {
            get { return (VideoCameraState)m_ASCOMVideo.CameraState; }
        }

        public short GainMax
        {
			get { return m_ASCOMVideo.GainMax; }
        }

        public short GainMin
        {
			get { return m_ASCOMVideo.GainMin; }
        }

        public short Gain
        {
			get { return m_ASCOMVideo.Gain; }
            set { m_ASCOMVideo.Gain = value; }
        }

        public ArrayList Gains
        {
			get { return m_ASCOMVideo.Gains; }
        }

        public int Gamma
        {
			get { return m_ASCOMVideo.Gamma; }
			set { m_ASCOMVideo.Gamma = (short)value; }
        }

        public ArrayList Gammas
        {
			get { return m_ASCOMVideo.Gammas; }
        }

        public bool CanConfigureImage
        {
            get { return m_ASCOMVideo.CanConfigureImage; }
        }

        public void ConfigureImage()
        {
	        m_ASCOMVideo.ConfigureImage();
        }
    }
}
