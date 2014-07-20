using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using OccuRec.ASCOM.Interfaces.Devices;

namespace OccuRec.Drivers.ASCOMVideo
{
    public class VideoFrame : IVideoFrame, IDisposable
    {
        private IASCOMVideoFrame m_ASCOMVideoFrame;

        public VideoFrame(IASCOMVideoFrame ascomVideoFrame)
        {
            m_ASCOMVideoFrame = ascomVideoFrame;
        }

        public object ImageArray
        {
            get { return m_ASCOMVideoFrame.ImageArray; }
        }

        public object ImageArrayVariant
        {
            get { return m_ASCOMVideoFrame.ImageArrayVariant; }
        }

        public Bitmap PreviewBitmap
        {
            get
            {
                byte[] bitmapBytes = m_ASCOMVideoFrame.PreviewBitmap;
                if (bitmapBytes == null)
                    return null;
                else
                    return (Bitmap)Bitmap.FromStream(new MemoryStream(bitmapBytes));
            }
        }

        public long FrameNumber
        {
            get { return m_ASCOMVideoFrame.FrameNumber; }
        }

        public double ExposureDuration
        {
            get { return m_ASCOMVideoFrame.ExposureDuration; }
        }

        public string ExposureStartTime
        {
            get { return m_ASCOMVideoFrame.ExposureStartTime; }
        }

        public string ImageInfo
        {
            get { return m_ASCOMVideoFrame.ImageInfo; }
        }

        public void Dispose()
        {
            m_ASCOMVideoFrame.Dispose();
        }
    }
}
