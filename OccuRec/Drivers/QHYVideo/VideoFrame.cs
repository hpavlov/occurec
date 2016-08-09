using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using OccuRec.Helpers;

namespace OccuRec.Drivers.QHYVideo
{
    public class VideoFrame : IVideoFrame, IDisposable
    {
        private int m_FrameNo;
        private ImageHeader m_Header;

        private int[,] pixels;
        private object[,] pixelsVariant;
        private Bitmap m_PreviewBitmap;

        public VideoFrame(byte[] pixelBytes, int width, int height, int frameNo, bool variant)
        {
            m_FrameNo = frameNo;
            m_Header = new ImageHeader(m_FrameNo, pixelBytes);

            if (variant)
            {
                pixelsVariant = new object[height, width];
                pixels = null;
            }
            else
            {
                pixels = new int[height, width];
                pixelsVariant = null;
            }

            m_PreviewBitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            ImageUtils.ProduceBitmap(pixelBytes, width, height, m_PreviewBitmap);

            if (m_Header.Latitude != 0 && m_Header.Longitude != 0)
            {
                // TODO: Parse the coordinates correctly
                ImageInfo = string.Format("LAT:{0};LONG:{1}", m_Header.Latitude, m_Header.Longitude);
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
