/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using ASCOM.DeviceInterface;
using OccuRec.ASCOM.Interfaces.Devices;
using OccuRec.Utilities;

namespace OccuRec.ASCOM.Server
{
    [Serializable]
    public class IsolatedVideoFrame : MarshalByRefObject, IASCOMVideoFrame, IDisposable
    {
        private IVideoFrame m_VideoFrame;
	    private bool m_HasMetadata = true;

        public IsolatedVideoFrame(IVideoFrame videoFrame)
        {
            m_VideoFrame = videoFrame;
        }

        public object ImageArray
        {
            get { return m_VideoFrame.ImageArray; }
        }

        public object ImageArrayVariant
        {
            get { return m_VideoFrame.ImageArray; }
        }

        public byte[] PreviewBitmap
        {
            get { return m_VideoFrame.PreviewBitmap; }
        }

        public long FrameNumber
        {
            get { return m_VideoFrame.FrameNumber; }
        }

        public double ExposureDuration
        {
            get { return m_VideoFrame.ExposureDuration; }
        }

        public string ExposureStartTime
        {
            get { return m_VideoFrame.ExposureStartTime; }
        }

        public string ImageInfo
        {
            get
            {
				if (m_HasMetadata)
				{
					try
					{
						ArrayList metaData = m_VideoFrame.ImageMetadata;
						if (metaData == null)
							return null;
						else
						{
							var output = new StringBuilder();
							foreach (object item in metaData)
							{
								if (item is KeyValuePair<string, object>)
								{
									string key = ((KeyValuePair<string, object>) item).Key;
									object value = ((KeyValuePair<string, object>) item).Value;
									output.AppendFormat("{0}={1}&", key, Convert.ToString(value));
								}
							}

							return output.ToString();
						}
					}
					catch (Exception ex)
					{
						m_HasMetadata = false;
					}
				}

				return string.Empty;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            m_VideoFrame = null;

            RemotingServices.Disconnect(this);
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
