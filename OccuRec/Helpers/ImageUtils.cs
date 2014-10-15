/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using OccuRec.Video.AstroDigitalVideo;

namespace OccuRec.Helpers
{
    public class ImageUtils
    {
		public static int[,] GetPixelArray(int width, int height, Bitmap b)
        {
            return GetPixelArray(width, height, b, AdvImageSection.GetPixelMode.Raw8Bit);
        }

		public static int[,] GetPixelArray(int width, int height, Bitmap b, AdvImageSection.GetPixelMode mode)
        {
            bool stretch = mode != AdvImageSection.GetPixelMode.Raw8Bit;
            uint maxval = 256;
            if (stretch)
            {
                if (mode == AdvImageSection.GetPixelMode.StretchTo12Bit)
                    maxval = 4096;
                else if (mode == AdvImageSection.GetPixelMode.StretchTo16Bit)
                    maxval = ushort.MaxValue;
            }

            Random rnd = new Random((int)DateTime.Now.Ticks);

            int[,] rv = new int[height, width];

			if (b != null)
			{
				// GDI+ still lies to us - the return format is BGR, NOT RGB.
				BitmapData bmData = b.LockBits(new Rectangle(0, 0, b.Width, b.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

				try
				{
					int stride = bmData.Stride;
					System.IntPtr Scan0 = bmData.Scan0;

					unsafe
					{
						byte* p = (byte*)(void*)Scan0;

						int nOffset = stride - b.Width * 3;

						for (int y = 0; y < b.Height; ++y)
						{
							for (int x = 0; x < b.Width; ++x)
							{
								ushort red = p[2];
								if (stretch)
								{
									uint stretchedVal = Math.Max(0, Math.Min(maxval, (uint)(red * maxval / 256.0 + (2 * (rnd.NextDouble() - 0.5) * maxval / 256.0))));
									red = (ushort)stretchedVal;
								}

								rv[y, x] = red;

								p += 3;
							}

							p += nOffset;
						}
					}
				}
				finally
				{
					b.UnlockBits(bmData);
				}				
			}

            return rv;
        }
    }
}
