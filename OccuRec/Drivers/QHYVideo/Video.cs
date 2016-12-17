using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Adv;
using OccuRec.Helpers;
using OccuRec.Utilities.Exceptions;
using OccuRec.Video.AstroDigitalVideo;
using AdvImageData = Adv.AdvImageData;
using Extensions = OccuRec.Utilities.Extensions;

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

        private string[] m_SupportedGammas = new string[] { "N/A" };
        private double[] m_SupportedGammaValues = new double[] { 1 };

        private int m_CurrentExposureIndex = 9;

        private int m_PreferredBinning;
        private int m_PreferredBpp;
        private bool m_PreferredGPS;
        private bool m_UseCooling;

        private VOXFreqFitter m_VOXFreqFitter;

        public Video(string qhyCameraId, int binning, int bpp, bool useGps, bool useCooling)
        {
            m_CameraId = qhyCameraId;
            m_PreferredBinning = binning;
            m_PreferredBpp = bpp;
            m_PreferredGPS = useGps;
            m_UseCooling = useCooling;

            m_VideoThread = new Thread(VideoGrabberWorker);

            m_VOXFreqFitter = new VOXFreqFitter(2000 /* TODO: This should be configuration item */);
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

                        if (m_GPS && m_PreferredGPS)
                        {
                            QHYPInvoke.CHECK(QHYPInvoke.SetQHYCCDParam(m_Handle, CONTROL_ID.CAM_GPS, 1));

                            var val = m_VOXFreqFitter.Initialize();
                            QHYPInvoke.SetQHYCCDGPSVCOXFreq(m_Handle, val);
                        }

                        if (m_Bits16 && m_PreferredBpp == 16)
                        {
                            QHYPInvoke.CHECK(QHYPInvoke.SetQHYCCDParam(m_Handle, CONTROL_ID.CONTROL_USBTRAFFIC, 30));
                            QHYPInvoke.CHECK(QHYPInvoke.SetQHYCCDParam(m_Handle, CONTROL_ID.CONTROL_TRANSFERBIT, 16));
                            QHYPInvoke.CHECK(QHYPInvoke.SetQHYCCDBitsMode(m_Handle, 16));
                        }
                        else
                        {
                            QHYPInvoke.CHECK(QHYPInvoke.SetQHYCCDParam(m_Handle, CONTROL_ID.CONTROL_USBTRAFFIC, 0));
                            QHYPInvoke.CHECK(QHYPInvoke.SetQHYCCDParam(m_Handle, CONTROL_ID.CONTROL_TRANSFERBIT, 8));
                            QHYPInvoke.CHECK(QHYPInvoke.SetQHYCCDBitsMode(m_Handle, 8));
                        }

                        bool binning2x2 = false;

                        if (!m_Binning2x2 || m_PreferredBinning == 1)
                        {
                            QHYPInvoke.CHECK(QHYPInvoke.SetQHYCCDBinMode(m_Handle, 1, 1));
                        }
                        else if (m_Binning2x2 && m_PreferredBinning == 2)
                        {
                            QHYPInvoke.CHECK(QHYPInvoke.SetQHYCCDBinMode(m_Handle, 2, 2));
                            binning2x2 = true;
                        }

                        QHYPInvoke.CHECK(QHYPInvoke.GetQHYCCDChipInfo(m_Handle, ref m_ChipWidth, ref m_ChipHeight, ref m_Width, ref m_Height, ref m_PixelWidth, ref m_PixelHeight, ref m_Bpp));

                        if (binning2x2)
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
                        if (m_Handle != IntPtr.Zero)
                        {
                            QHYPInvoke.CancelQHYCCDExposingAndReadout(m_Handle);

                            lock (m_SyncLock)
                            {
                                m_Connected = false;
                            }

                            QHYPInvoke.StopQHYCCDLive(m_Handle);

                            int result = QHYPInvoke.CloseQHYCCD(m_Handle);
                            if (result != QHYCCDResult.QHYCCD_SUCCESS)
                            {
                                // Log error??
                            }
                        }
                        m_Handle = IntPtr.Zero;

                        if (m_VideoThread.IsAlive)
                        {
                            m_VideoThread.Join(1000);

                            if (m_VideoThread.IsAlive)
                                m_VideoThread.Abort();
                        }
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
        private double m_MinGamma;
        private double m_MaxGamma;
        private double m_GammaStep;
        private double m_CurrentGamma = 1;
        private int m_CurrentGammaIndex;
        private int? m_ChangeGammaTo = null;

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

            if (QHYPInvoke.SUCCEEDED(QHYPInvoke.GetQHYCCDParamMinMaxStep(m_Handle, CONTROL_ID.CONTROL_GAMMA, ref min, ref max, ref step)))
            {
                m_MinGamma = (short)Math.Ceiling(min);
                m_MaxGamma = (short)Math.Floor(max);
                m_GammaStep = (short)Math.Ceiling(step);

                if (m_MinGamma <= 0.2 && m_MaxGamma >= 2.0)
                {
                    m_SupportedGammas = new string[] { "0.20", "0.25", "0.30", "0.35", "0.40", "0.45", "0.50", "0.55", "0.60", "0.65", "0.70", "0.75", "0.80", "0.85", "0.90", "0.95", "1.00", "1.05", "1.10", "1.15", "1.20", "1.25", "1.30", "1.35", "1.40", "1.45", "1.50" };
                    m_SupportedGammaValues = new double[] { 0.20, 0.25, 0.30, 0.35, 0.40, 0.45, 0.50, 0.55, 0.60, 0.65, 0.70, 0.75, 0.80, 0.85, 0.90, 0.95, 1.00, 1.05, 1.10, 1.15, 1.20, 1.25, 1.30, 1.35, 1.40, 1.45, 1.50 };
                    m_CurrentGamma = 1;
                    m_CurrentGammaIndex = 16;
                }
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
                if (m_LastVideoFrame == null || m_LastVideoFrame.FrameNumber != m_FrameNo)
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
                            m_LastVideoFrame = new VideoFrame(m_CurrentBytes, m_Width, m_Height, m_Bpp, m_FrameNo, false, m_CCDTemp);
                            if (!double.IsNaN(m_LastVideoFrame.Longitude) && !double.IsNaN(m_LastVideoFrame.Latitude))
                            {
                                m_GPSLongitude = m_LastVideoFrame.Longitude;
                                m_GPSLatitude = m_LastVideoFrame.Latitude;
                            }
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
                }

                return m_LastVideoFrame;
            }
        }

        public IVideoFrame LastVideoFrameImageArrayVariant
        {
            get
            {
                if (m_LastVideoFrame == null || m_LastVideoFrame.FrameNumber != m_FrameNo)
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

                        m_LastVideoFrame = new VideoFrame(m_CurrentBytes, m_Width, m_Height, m_Bpp, m_FrameNo, true, m_CCDTemp);
                        if (!double.IsNaN(m_LastVideoFrame.Longitude) && !double.IsNaN(m_LastVideoFrame.Latitude))
                        {
                            m_GPSLongitude = m_LastVideoFrame.Longitude;
                            m_GPSLatitude = m_LastVideoFrame.Latitude;
                        }
                    }
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

        private string m_PreferredFileName = null;
        private AdvRecorder m_Recorder = null;

        public string StartRecordingVideoFile(string preferredFileName)
        {
            if (m_CameraState == VideoCameraState.videoCameraRunning)
            {
                m_CameraState = VideoCameraState.videoCameraRecording;
                m_PreferredFileName = Path.ChangeExtension(preferredFileName, ".adv");
                return m_PreferredFileName;
            }

            return null;
        }

        public void StopRecordingVideoFile()
        {
            if (m_CameraState == VideoCameraState.videoCameraRecording)
            {
                lock (m_SyncLock)
                {
                    if (m_Recorder != null)
                    {
                        m_Recorder.FinishRecording();
                        m_Recorder = null;
                    }
                }
                
                m_CameraState = VideoCameraState.videoCameraRunning;
            }
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
            get
            {
                return m_ChangeGammaTo ?? m_CurrentGammaIndex;
            }
            set
            {
                int newValue = value;
                int oldValue = m_ChangeGammaTo ?? m_CurrentGammaIndex;
                if (newValue != oldValue)
                {
                    AssertConnected();
                    m_ChangeGammaTo = newValue;
                }
            }
        }

        public System.Collections.ArrayList Gammas
        {
            get { return new ArrayList(m_SupportedGammas); }
        }

        public bool CanConfigureImage
        {
            get { return true; }
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

        private double m_CCDTemp = double.NaN;
        private double m_GPSLongitude;
        private double m_GPSLatitude;
        private byte[] m_CurrentBytes;
        private int m_CCDTempTagId;
        private int m_TimeStampFlagTagId;
        private int m_EstimatedTimeStampErrorMillisecondsTagId;
        private bool m_PrevVoxFreqIsGood;

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
            var stopwatch = new Stopwatch();

            m_GPSLongitude = double.NaN;
            m_GPSLatitude = double.NaN;
            m_CCDTemp = double.NaN;
            if (m_UseCooling) stopwatch.Start();

            while (m_Connected)
            {
                try
                {
                    rv = QHYPInvoke.GetQHYCCDLiveFrame(m_Handle, ref w, ref h, ref bpp, ref channels, imageBytes);
                    if (rv >= 0)
                    {
                        m_FrameNo++;

                        var header = new ImageHeader(m_FrameNo, imageBytes, bpp);

                        if (m_CameraState == VideoCameraState.videoCameraRecording)
                        {
                            lock (m_SyncLock)
                            {
                                if (m_Recorder == null)
                                {
                                    m_Recorder = new AdvRecorder();

                                    Version occuRecVersion = Assembly.GetExecutingAssembly().GetName().Version;
                                    bool isBeta = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(BetaReleaseAttribute), false).Length == 1;

                                    // Start Recording 
                                    m_Recorder.FileMetaData.RecorderSoftwareName = "OccuRec";
                                    m_Recorder.FileMetaData.RecorderSoftwareVersion = string.Format("{0}.{1}{2}", occuRecVersion.Major, occuRecVersion.Minor, isBeta ? " BETA" : "");

                                    m_Recorder.FileMetaData.CameraModel = m_CameraModel;
                                    m_Recorder.FileMetaData.CameraSensorInfo = m_CameraModel;

                                    if (!double.IsNaN(m_GPSLongitude) && !double.IsNaN(m_GPSLatitude))
                                        m_Recorder.LocationData.SetLocation(m_GPSLongitude, m_GPSLatitude, double.NaN, "ALTITUDE-UNAVAILABLE");

                                    m_Recorder.StatusSectionConfig.RecordGain = true;
                                    m_Recorder.StatusSectionConfig.RecordGamma = true;
                                    m_Recorder.StatusSectionConfig.RecordSystemTime = true;
                                    m_Recorder.StatusSectionConfig.RecordFixStatus = true;
                                    m_Recorder.StatusSectionConfig.RecordVideoCameraFrameId = true;

                                    m_TimeStampFlagTagId = m_Recorder.StatusSectionConfig.AddDefineTag("TimeStampFlag", Adv2TagType.Int8);
                                    m_EstimatedTimeStampErrorMillisecondsTagId = m_Recorder.StatusSectionConfig.AddDefineTag("EstimatedTimeStampErrorMilliseconds", Adv2TagType.Real);
                                    if (m_UseCooling)
                                        m_CCDTempTagId = m_Recorder.StatusSectionConfig.AddDefineTag("CCDTemperature", Adv2TagType.Real);

                                    m_Recorder.ImageConfig.SetImageParameters((ushort)m_Width, (ushort)m_Height, (byte)m_Bpp, null);

                                    m_Recorder.StartRecordingNewFile(m_PreferredFileName, 1000000, true);

                                    m_PrevVoxFreqIsGood = false;
                                }

                                var statusChannel = new AdvRecorder.AdvStatusEntry()
                                {
                                    Gain = m_CurrentGain,
                                    Gamma = (float)m_CurrentGamma,
                                    VideoCameraFrameId = header.SeqNumber,
                                    SystemTime = AdvTimeStamp.FromDateTime(DateTime.Now)
                                };

                                if (m_UseCooling)
                                    statusChannel.AdditionalStatusTags = new object[] { (byte)0, 0, (float)m_CCDTemp };
                                else
                                    statusChannel.AdditionalStatusTags = new object[] { (byte)0, 0 };

                                if (header.GpsTimeAvailable)
                                {
                                    if (header.Latitude != 0 && header.Longitude != 0)
                                        statusChannel.FixStatus = FixStatus.GFix;
                                    else
                                        statusChannel.FixStatus = FixStatus.PFix;
                                }
                                else
                                    statusChannel.FixStatus = FixStatus.NoFix;

                                if (header.MaxClock == 10000500 && m_PrevVoxFreqIsGood)
                                {
                                    // NOTE: Add Potentially Bad GPS Timestamp Flag
                                    statusChannel.AdditionalStatusTags[0] = (byte)1;
                                }
                                else
                                {
                                    // NOTE: Add GPS Timestamp Bias Estimate
                                    double biasMSPerSec = (header.MaxClock - 10000000) * 0.5 / 1000.0;
                                    double biasMS = biasMSPerSec * (header.EndTime - header.StartTime).TotalSeconds;
                                    statusChannel.AdditionalStatusTags[1] = (float)biasMS;
                                }

                                m_PrevVoxFreqIsGood = header.MaxClock != 10000500;
/* TODO
[2548] System.InvalidCastException : Specified cast is not valid. 
[2548]    at Adv.AdvRecorder.BeginVideoFrame(AdvStream advStream, Nullable`1 startClockTicks, Nullable`1 endClockTicks, AdvTimeStamp startUtcTimeStamp, AdvTimeStamp endUtcTimeStamp, AdvStatusEntry metadata) 
[2548]    at Adv.AdvRecorder.AddFrame(AdvStream advStream, Byte[] pixels, Boolean compressIfPossible, Nullable`1 preferredCompression, Nullable`1 startClockTicks, Nullable`1 endClockTicks, AdvTimeStamp startUtcTimeStamp, AdvTimeStamp endUtcTimeStamp, AdvStatusEntry metadata, AdvImageData imageData) 
[2548]    at Adv.AdvRecorder.AddVideoFrame(Byte[] pixels, Boolean compressIfPossible, Nullable`1 preferredCompression, AdvTimeStamp startUtcTimeStamp, AdvTimeStamp endUtcTimeStamp, AdvStatusEntry metadata, AdvImageData imageData) 
[2548]    at OccuRec.Drivers.QHYVideo.Video.VideoGrabberWorker(Object state) in f:\WORK\OccuRec\OccuRec\Drivers\QHYVideo\Video.cs:line 714 
[2548] -------------------------------------------------------------------------------------------------- 
                                */
                                m_Recorder.AddVideoFrame(
                                    imageBytes,
                                    true, PreferredCompression.Lagarith16,
                                    AdvTimeStamp.FromDateTime(header.StartTime),
                                    AdvTimeStamp.FromDateTime(header.EndTime),
                                    statusChannel,
                                    bpp == 8 ? AdvImageData.PixelDepth8Bit : AdvImageData.PixelDepth16Bit);
                            }
                        }

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

                        if (m_ChangeGammaTo.HasValue)
                        {
                            double gamma = m_SupportedGammaValues[m_ChangeGammaTo.Value];
                            rv = QHYPInvoke.SetQHYCCDParam(m_Handle, CONTROL_ID.CONTROL_GAMMA, gamma);
                            if (QHYPInvoke.SUCCEEDED(rv))
                            {
                                m_CurrentGamma = gamma;
                                m_CurrentGammaIndex = m_ChangeGammaTo.Value;
                                m_ChangeGammaTo = null;
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

                        var val = m_VOXFreqFitter.GetNextValue((uint)header.MaxClock);
                        if (val.HasValue)
                            QHYPInvoke.SetQHYCCDGPSVCOXFreq(m_Handle, val.Value);
                    }

                    if (m_UseCooling && stopwatch.ElapsedMilliseconds > 1000)
                    {
                        m_CCDTemp = QHYPInvoke.GetQHYCCDParam(m_Handle, CONTROL_ID.CONTROL_CURTEMP);

                        rv = QHYPInvoke.ControlQHYCCDTemp(m_Handle, -50);
                        if (!QHYPInvoke.SUCCEEDED(rv))
                        {
                            Trace.WriteLine("QHYPInvoke.ControlQHYCCDTemp() Failed!");
                        }
                        stopwatch.Reset();
                        stopwatch.Start();


                    }

                    Thread.Sleep(0);
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(Extensions.GetFullStackTrace(ex));
                }
            }
        }
    }
}
