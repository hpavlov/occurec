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
            get { throw new NotImplementedException(); }
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

        public string Action(string ActionName, string ActionParameters)
        {
            throw new NotImplementedException();
        }

        public ArrayList SupportedActions
        {
            get { return m_ASCOMVideo.SupportedActions; }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public double ExposureMax
        {
            get { throw new NotImplementedException(); }
        }

        public double ExposureMin
        {
            get { throw new NotImplementedException(); }
        }

        public VideoCameraFrameRate FrameRate
        {
            get { throw new NotImplementedException(); }
        }

        public System.Collections.ArrayList SupportedIntegrationRates
        {
            get { throw new NotImplementedException(); }
        }

        public int IntegrationRate
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
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
            get { throw new NotImplementedException(); }
        }

        public SensorType SensorType
        {
            get { throw new NotImplementedException(); }
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
            get { throw new NotImplementedException(); }
        }

        public double PixelSizeY
        {
            get { throw new NotImplementedException(); }
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
            get { throw new NotImplementedException(); }
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
            get { return (VideoCameraState)m_ASCOMVideo.CameraState; }
        }

        public short GainMax
        {
            get { throw new NotImplementedException(); }
        }

        public short GainMin
        {
            get { throw new NotImplementedException(); }
        }

        public short Gain
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public System.Collections.ArrayList Gains
        {
            get { throw new NotImplementedException(); }
        }

        public int Gamma
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public System.Collections.ArrayList Gammas
        {
            get { throw new NotImplementedException(); }
        }

        public bool CanConfigureImage
        {
            get { return m_ASCOMVideo.CanConfigureImage; }
        }

        public void ConfigureImage()
        {
            throw new NotImplementedException();
        }
    }
}
