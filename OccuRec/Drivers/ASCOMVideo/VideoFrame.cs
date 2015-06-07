/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

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


        public uint MaxSignalValue
        {
            get { return 255; }
        }
    }
}
