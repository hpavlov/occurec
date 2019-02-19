using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Linq;
using System.Text;
using OccuRec.Helpers;
using OccuRec.Utilities;

namespace OccuRec.Drivers.QHYVideo
{
    public class VideoFrame : IVideoFrame, IDisposable
    {
        private int m_FrameNo;
        private ImageHeader m_Header;

        private int[,] pixels;
        private object[,] pixelsVariant;
        private Bitmap m_PreviewBitmap;

        private static int m_LoggedFrameNo = 0;
        private static int m_LastExpMS = 0;
        private static DateTime m_LastEndTime = DateTime.MaxValue;

        public VideoFrame(byte[] pixelBytes, int width, int height, int bpp, int frameNo, bool variant, double ccdTemp)
        {
            m_FrameNo = frameNo;
            m_Header = new ImageHeader(m_FrameNo, pixelBytes);

            if (variant)
            {
                pixelsVariant = new object[height, width];
                pixels = null;

                for (int y = 0; y < height; ++y)
                {
                    for (int x = 0; x < width; ++x)
                    {
                        byte pixelVal = bpp == 16 ? pixelBytes[2 * (y * width + x) + 1] : pixelBytes[y * width + x];
                        pixelsVariant[y, x] = pixelVal;
                    }
                }
            }
            else
            {
                pixels = new int[height, width];
                pixelsVariant = null;

                for (int y = 0; y < height; ++y)
                {
                    for (int x = 0; x < width; ++x)
                    {
                        byte pixelVal = bpp == 16 ? pixelBytes[2 * (y * width + x) + 1] : pixelBytes[y * width + x];
                        pixels[y, x] = pixelVal;
                    }
                }
            }

            m_PreviewBitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            ImageUtils.ProduceBitmap(pixelBytes, width, height, bpp, m_PreviewBitmap);

            if (m_Header.Latitude != 0 && m_Header.Longitude != 0)
            {
                Longitude = m_Header.ParseLongitude;
                Latitude = m_Header.ParseLatitude;

                ImageInfo = string.Format("LAT:{0};LONG:{1};", AstroConvert.ToStringValue(Latitude, "+DD°MM'SS.T\""), AstroConvert.ToStringValue(Longitude, "+DD°MM'SS.T\""));
            }
            else
            {
                Longitude = double.NaN;
                Latitude = double.NaN;
            }

            ImageInfo += string.Format("GPS:{0};CLKFRQ:{1};CCDTMP:{2}", m_Header.GPSStatus, m_Header.MaxClock, ccdTemp.ToString("0.0", CultureInfo.InvariantCulture));

            if (m_LoggedFrameNo != frameNo)
            {
                int currExpMS = (int) Math.Round((m_Header.EndTime - m_Header.StartTime).TotalMilliseconds);
                int msDiff = Math.Abs(m_LastExpMS - currExpMS);
                int msGap = (int)Math.Round((m_Header.StartTime - m_LastEndTime).TotalMilliseconds);
                string flags = msDiff > 5 ? ";LARGE-DIFF" : "";
                if (msGap > 5) flags += ";LARGE-GAP";
                Trace.WriteLine(string.Format("QHY {0} |{1}| GPSFlag:{2};MaxClock:{3};EXPD:{4}ms;GAP:{5}{6}", m_Header.StartTime.ToString("yyyy-MM-dd HH:mm:ss.fff"), string.Join(" ", pixelBytes.Take(44).Select(x => Convert.ToString(x, 16).PadLeft(2, '0'))), m_Header.GPSStatus, m_Header.MaxClock, msDiff, msGap, flags));
                m_LoggedFrameNo = frameNo;
                m_LastExpMS = currExpMS;
                m_LastEndTime = m_Header.EndTime;
            }
        }

        public object ImageArray
        {
            get { return pixels; }
        }

        public object ImageArrayVariant
        {
            get { return pixelsVariant; }
        }

        public Bitmap PreviewBitmap
        {
            get { return m_PreviewBitmap; }
        }

        internal double Longitude { get; private set; }

        internal double Latitude { get; private set; }

        internal long VideoFrameNo
        {
            get { return m_FrameNo; }
        }

        public long FrameNumber
        {
            get { return m_Header.SeqNumber; }
        }

        public double ExposureDuration
        {
            get { return (m_Header.EndTime - m_Header.StartTime).TotalMilliseconds; }
        }

        public string ExposureStartTime
        {
            get { return m_Header.StartTime.ToString("HH:mm:ss.fff"); }
        }

        public string ImageInfo
        {
            get; private set; 
        }

        public uint MaxSignalValue
        {
            get { return 0; }
        }

        public void Dispose()
        {
            if (m_PreviewBitmap != null)
            {
                m_PreviewBitmap.Dispose();
                m_PreviewBitmap = null;
            }
        }
    }
}
