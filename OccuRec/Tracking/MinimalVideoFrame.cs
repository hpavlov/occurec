/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using OccuRec.Drivers;
using OccuRec.Helpers;

namespace OccuRec.Tracking
{
	internal class MinimalVideoFrame : IVideoFrame
	{
        public MinimalVideoFrame(VideoFrameWrapper source, Bitmap previewBitmap)
		{
			FrameNumber = source.FrameNumber;
			ExposureDuration = double.NaN;
			ExposureStartTime = null;
			ImageInfo = source.ImageInfo;

            if (source.ImageArray is int[,])
            {
                ImageArray = source.ImageArray;
            }
            else if (source.ImageArray != null)
                throw new NotSupportedException("Unsupported ImageArray Format.");
            else if (source.ImageArray == null)
            {
                if (previewBitmap != null)
                    ConstructFromBitmap(previewBitmap);
                else
                    throw new NotSupportedException("Both ImageArray and PreviewBitmap are null!");
            }
		}

        private void ConstructFromBitmap(Bitmap bmp)
        {
            //using (Bitmap bmp = (Bitmap)previewBitmap.Clone())
            {
                ImageArray = new int[bmp.Height, bmp.Width];

                // GDI+ still lies to us - the return format is BGR, NOT RGB.
                BitmapData bmData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

                int stride = bmData.Stride;
                System.IntPtr Scan0 = bmData.Scan0;

                unsafe
                {
                    byte* p = (byte*)(void*)Scan0;

                    int nOffset = stride - bmp.Width * 3;

                    for (int y = 0; y < bmp.Height; ++y)
                    {
                        for (int x = 0; x < bmp.Width; ++x)
                        {
                            byte blue = p[0];
                            byte green = p[1];
                            byte red = p[2];

                            byte val = red; // GetColourChannelValue(channel, red, green, blue);

                            ((int[,])ImageArray)[y, x] = val;

                            p += 3;
                        }
                        p += nOffset;
                    }
                }

                bmp.UnlockBits(bmData);
            }
        }
		public object ImageArray { get; private set; }

		public object ImageArrayVariant
		{
			get { return null; }
		}

		public System.Drawing.Bitmap PreviewBitmap
		{
			get { return null; }
		}

		public long FrameNumber { get; private set; }

		public double ExposureDuration { get; private set; }

		public string ExposureStartTime { get; private set; }

		public string ImageInfo { get; private set; }
	}
}
