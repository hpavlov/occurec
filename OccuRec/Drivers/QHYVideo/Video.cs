using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading;
using OccuRec.Helpers;
using OccuRec.Utilities;
using OccuRec.Utilities.Exceptions;

namespace OccuRec.Drivers.QHYVideo
{
    internal class Video : IVideo
    {
        private string m_CameraId;
        private IntPtr m_Handle = IntPtr.Zero;
        private int m_ImageSize;
        private string m_CameraModel;

        private VideoCameraState m_CameraState;

        private bool m_Connected = false;

        private int m_Width = 0;
        private int m_Height = 0;
        private int m_Bpp = 0;

        private double m_ChipWidth;
        private double m_ChipHeight;
        private double m_PixelWidth;
        private double m_PixelHeight;

        private Thread m_VideoThread;
        private int m_FrameNo;

        private string[] m_SupportedExposures = new string[] { "1 ms", "2 ms", "5 ms", "10 ms", "20 ms", "40 ms", "80 ms", "160 ms", "320 ms", "640 ms", "1 sec", "1.28 sec", "2 sec", "3 sec", "4 sec", "5 sec", "8 sec", "10 sec" };
        private int[] m_SupportedExposureMilliseconds = new int[] { 1, 2, 5, 10, 20, 40, 80, 160, 320, 640, 1000, 1280, 2000, 3000, 4000, 5000, 8000, 10000 };

        private int m_CurrentExposureIndex = 9;

        public Video(string qhyCameraId)
        {
            m_CameraId = qhyCameraId;
            m_VideoThread = new Thread(VideoGrabberWorker);
        }

        public bool Connected
        {
            get { return m_Connected; }
            set
            {
                if (value != m_Connected)
                {
                    if (!m_Connected)
                    {
                        m_Handle = QHYPInvoke.OpenQHYCCD(m_CameraId);
                        
                        CheckCameraFeatures();

                        int deviceId = QHYPInvoke.GetQHYCCDType(m_Handle);
                        m_CameraModel = DeviceIds.GetCameraModel(deviceId);

                        QHYPInvoke.CHECK(QHYPInvoke.SetQHYCCDStreamMode(m_Handle, StreamMode.Live));
                        QHYPInvoke.CHECK(QHYPInvoke.InitQHYCCD(m_Handle));


                        QHYPInvoke.CHECK(QHYPInvoke.SetQHYCCDParam(m_Handle, CONTROL_ID.CONTROL_SPEED, 2));
                        QHYPInvoke.CHECK(QHYPInvoke.SetQHYCCDParam(m_Handle, CONTROL_ID.CONTROL_EXPOSURE, m_SupportedExposureMilliseconds[m_CurrentExposureIndex] * 1000));
                        QHYPInvoke.CHECK(QHYPInvoke.SetQHYCCDParam(m_Handle, CONTROL_ID.CONTROL_GAIN, m_CurrentGain));
                        QHYPInvoke.CHECK(QHYPInvoke.SetQHYCCDParam(m_Handle, CONTROL_ID.CONTROL_OFFSET, 10));

                        if (m_GPS)
                            QHYPInvoke.CHECK(QHYPInvoke.SetQHYCCDParam(m_Handle, CONTROL_ID.CAM_GPS, 1));

                        // TODO: Binning and BitDepth should be selected during connection

                        if (!m_Binning2x2)
                        {
                            QHYPInvoke.CHECK(QHYPInvoke.SetQHYCCDBinMode(m_Handle, 1, 1));
                            QHYPInvoke.CHECK(QHYPInvoke.SetQHYCCDParam(m_Handle, CONTROL_ID.CONTROL_USBTRAFFIC, 0));
                            QHYPInvoke.CHECK(QHYPInvoke.SetQHYCCDParam(m_Handle, CONTROL_ID.CONTROL_TRANSFERBIT, 8));
                        }
                        else
                        {
                            QHYPInvoke.CHECK(QHYPInvoke.SetQHYCCDParam(m_Handle, CONTROL_ID.CONTROL_USBTRAFFIC, 0));
                            //QHYPInvoke.CHECK(QHYPInvoke.SetQHYCCDParam(m_Handle, CONTROL_ID.CONTROL_TRANSFERBIT, 16));
                            //QHYPInvoke.CHECK(QHYPInvoke.SetQHYCCDBitsMode(m_Handle, 16));
                            QHYPInvoke.CHECK(QHYPInvoke.SetQHYCCDBinMode(m_Handle, 2, 2));
                        }

                        QHYPInvoke.CHECK(QHYPInvoke.GetQHYCCDChipInfo(m_Handle, ref m_ChipWidth, ref m_ChipHeight, ref m_Width, ref m_Height, ref m_PixelWidth, ref m_PixelHeight, ref m_Bpp));

                        if (m_Binning2x2)
                        {
                            m_Width /= 2;
                            m_Height /= 2;
                            QHYPInvoke.CHECK(QHYPInvoke.SetQHYCCDResolution(m_Handle, 0, 0, m_Width, m_Height));
                        }

                        m_ImageSize = QHYPInvoke.GetQHYCCDMemLength(m_Handle);

                        int rv = QHYPInvoke.BeginQHYCCDLive(m_Handle);
                        if (QHYPInvoke.SUCCEEDED(rv))
                        {
                            m_CameraState = VideoCameraState.videoCameraRunning;
                            m_Connected = true;

                            m_VideoThread.Start();
                        }
                    }
                    else
                    {
                        m_Connected = true;
                        if (m_VideoThread.IsAlive)
                        {
                            m_VideoThread.Join(1000);

                            if (m_VideoThread.IsAlive)
                                m_VideoThread.Abort();
                        }

                        if (m_Handle != IntPtr.Zero)
                        {
                            QHYPInvoke.StopQHYCCDLive(m_Handle);

                            int result = QHYPInvoke.CloseQHYCCD(m_Handle);
                            if (result != QHYCCDResult.QHYCCD_SUCCESS)
                            {
                                // Log error??
                            }
                        }
                        m_Handle = IntPtr.Zero;
                    }
                }
            }
        }

        private bool m_Bits16;
        private bool m_Bits8;
        private bool m_GPS;
        private bool m_Colour;
        private bool m_Binning1x1;
        private bool m_Binning2x2;
        private bool m_Binning3x3;
        private bool m_Binning4x4;

        private short m_MinGain = short.MaxValue;
        private short m_MaxGain = short.MinValue;
        private short m_GainStep = 0;
        private short m_CurrentGain = 0;

        private void CheckCameraFeatures()
        {
            m_Bits16 = QHYPInvoke.IsQHYCCDControlAvailable(m_Handle, CONTROL_ID.CAM_8BITS);
            m_Bits8 = QHYPInvoke.IsQHYCCDControlAvailable(m_Handle, CONTROL_ID.CAM_16BITS);
            m_GPS = QHYPInvoke.IsQHYCCDControlAvailable(m_Handle, CONTROL_ID.CAM_GPS);
            m_Colour = QHYPInvoke.IsQHYCCDControlAvailable(m_Handle, CONTROL_ID.CAM_COLOR);
            m_Binning1x1 = QHYPInvoke.IsQHYCCDControlAvailable(m_Handle, CONTROL_ID.CAM_BIN1X1MODE);
            m_Binning2x2 = QHYPInvoke.IsQHYCCDControlAvailable(m_Handle, CONTROL_ID.CAM_BIN2X2MODE);
            m_Binning3x3 = QHYPInvoke.IsQHYCCDControlAvailable(m_Handle, CONTROL_ID.CAM_BIN3X3MODE);
            m_Binning4x4 = QHYPInvoke.IsQHYCCDControlAvailable(m_Handle, CONTROL_ID.CAM_BIN4X4MODE);

            double min = 0, max = 0, step = 0;
            if (QHYPInvoke.SUCCEEDED(QHYPInvoke.GetQHYCCDParamMinMaxStep(m_Handle, CONTROL_ID.CONTROL_GAIN, ref min, ref max, ref step)))
            {
                m_MinGain = (short)Math.Ceiling(min);
                m_MaxGain = (short)Math.Floor(max);
                m_GainStep = (short)Math.Ceiling(step);
                m_CurrentGain = (short)((m_MinGain + m_MaxGain) / 2);
            }
        }

        public string Description
        {
            get { return "QHY Video Driver"; }
        }

        public string DriverInfo
        {
            get { return "OccuRec's QHY Video Driver"; }
        }

        public string DriverVersion
        {
            get { return "0.1"; }
        }

        public short InterfaceVersion
        {
            get { return 1; }
        }

        public string Name
        {
            get { return "QHY Video Driver"; }
        }

        public string VideoCaptureDeviceName
        {
            get { return m_CameraId; }
        }

        public void SetupDialog()
        {
            // TODO
        }

        public void SetCallbacks(IVideoCallbacks callbacksObject)
        {
            // TODO
        }

        public string Action(string ActionName, string ActionParameters)
        {
            throw new NotImplementedException();
        }

        public System.Collections.ArrayList SupportedActions
        {
            get { return new ArrayList(); }
        }

        public void Dispose()
        {
            
        }

        public double ExposureMax
        {
            get { return 10; }
        }

        public double ExposureMin
        {
            get { return 0.001; }
        }

        public VideoCameraFrameRate FrameRate
        {
            get { return VideoCameraFrameRate.Digital; }
        }


        public System.Collections.ArrayList SupportedIntegrationRates
        {
            get { return new ArrayList(m_SupportedExposures); }
        }

        public int IntegrationRate
        {
            get
            {
                return m_ChangeExposureIndexTo ?? m_CurrentExposureIndex;
            }
            set
            {
                int newIndex = value;
                int oldIndex = m_ChangeExposureIndexTo ?? m_CurrentExposureIndex;
                if (newIndex != oldIndex)
                {
                    AssertConnected();

                    if (newIndex < 0)
                        newIndex = 0;
                    else if (value > m_SupportedExposures.Length - 1)
                        newIndex = m_SupportedExposures.Length - 1;

                    m_ChangeExposureIndexTo = newIndex;
                }
            }
        }

        private int? m_ChangeExposureIndexTo = null;

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

                    try
                    {
                        m_LastVideoFrame = new VideoFrame(m_CurrentBytes, m_Width, m_Height, m_FrameNo, false);
                    }
                    catch (NotConnectedException)
                    {
                        m_LastVideoFrame = null;
                    }
                    catch (InvalidOperationException)
                    {
                        m_LastVideoFrame = null;
                    }
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

                    m_LastVideoFrame = new VideoFrame(m_CurrentBytes, m_Width, m_Height, m_FrameNo, true);
                }

                return m_LastVideoFrame;
            }
        }

        public string SensorName
        {
            get { return m_CameraModel; }
        }

        public SensorType SensorType
        {
            get { return SensorType.Monochrome; }
        }

        public int CameraXSize
        {
            get { return (int)m_ChipWidth; }
        }

        public int CameraYSize
        {
            get { return (int)m_ChipHeight; }
        }

        public int Width
        {
            get { return m_Width; }
        }

        public int Height
        {
            get { return m_Height; }
        }

        public double PixelSizeX
        {
            get { return m_PixelWidth; }
        }

        public double PixelSizeY
        {
            get { return m_PixelHeight; }
        }

        public int BitDepth
        {
            get { return m_Bpp; }
        }

        public string VideoCodec
        {
            get { return null; }
        }

        public string VideoFileFormat
        {
            get { return "ADV"; }
        }

        public int VideoFramesBufferSize
        {
            get { return 1; }
        }

        public string StartRecordingVideoFile(string preferredFileName)
        {
            throw new NotImplementedException();
        }

        public void StopRecordingVideoFile()
        {
            throw new NotImplementedException();
        }

        public VideoCameraState CameraState
        {
            get { return m_CameraState; }
        }

        public short GainMax
        {
            get { return m_MaxGain; }
        }

        public short GainMin
        {
            get { return m_MinGain; }
        }

        public short Gain
        {
            get
            {
                return m_ChangeGaintTo ?? m_CurrentGain;
            }
            set
            {
                short newValue = value;
                short oldValue = m_ChangeGaintTo ?? m_CurrentGain;
                if (newValue != oldValue)
                {
                    AssertConnected();
                    m_ChangeGaintTo = newValue;
                }
            }
        }

        private short? m_ChangeGaintTo = null;

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
            get { return 0; }
            set
            {
                // TODO:
            }
        }

        public System.Collections.ArrayList Gammas
        {
            get { return new ArrayList(new string[] { "OFF" }); }
        }

        public bool CanConfigureImage
        {
            get { return false; }
        }

        public void ConfigureImage()
        {
            // TODO:
        }

        private void AssertConnected()
        {
            if (!m_Connected)
                throw new NotConnectedException();
        }

        private byte[] m_CurrentBytes;

        private void VideoGrabberWorker(object state)
        {
            int w = 0;
            int h = 0;
            int bpp = 0;
            int channels = 0;
            
            byte[] imageBytes = new byte[m_ImageSize];
            m_CurrentBytes = new byte[m_ImageSize];
            for (int i = 0; i < m_ImageSize; i++)
            {
                imageBytes[i] = 0x66;
            }

            m_FrameNo = 0;
            int rv;

            while (m_Connected)
            {
                try
                {
                    rv = QHYPInvoke.GetQHYCCDLiveFrame(m_Handle, ref w, ref h, ref bpp, ref channels, imageBytes);
                    if (rv >= 0)
                    {
                        m_FrameNo++;
                        lock (m_SyncLock)
                        {
                            Array.Copy(imageBytes, 0, m_CurrentBytes, 0, m_ImageSize);
                        }

                        if (m_ChangeGaintTo.HasValue)
                        {
                            rv = QHYPInvoke.SetQHYCCDParam(m_Handle, CONTROL_ID.CONTROL_GAIN, m_ChangeGaintTo.Value);
                            if (QHYPInvoke.SUCCEEDED(rv))
                            {
                                m_CurrentGain = m_ChangeGaintTo.Value;
                                m_ChangeGaintTo = null;
                            }
                        }

                        if (m_ChangeExposureIndexTo.HasValue)
                        {
                            rv = QHYPInvoke.SetQHYCCDParam(m_Handle, CONTROL_ID.CONTROL_EXPOSURE, m_SupportedExposureMilliseconds[m_ChangeExposureIndexTo.Value] * 1000);
                            if (QHYPInvoke.SUCCEEDED(rv))
                            {
                                m_CurrentExposureIndex = m_ChangeExposureIndexTo.Value;
                                m_ChangeExposureIndexTo = null;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.GetFullStackTrace());
                }
            }
        }
    }
}
