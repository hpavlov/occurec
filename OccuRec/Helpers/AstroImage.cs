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

namespace OccuRec.Helpers
{
	internal class AstroImage
	{
		private int[,] m_Pixels;
		private int m_Width;
		private int m_Height;

	    public int Width
	    {
            get { return m_Width; }
	    }

        public int Height
        {
            get { return m_Height; }
        }

	    public int GetPixel(int x, int y)
	    {
	        return m_Pixels[y, x];
	    }

        public uint MaxSignalValue { get; private set; }


	    public AstroImage(IVideoFrame videoFrame, int width, int height, uint maxSignalValue = 255)
            : this((int[,])videoFrame.ImageArray, width, height, maxSignalValue)
		{
		}

        public AstroImage(int[,] imageArray, int width, int height, uint maxSignalValue = 255)
        {
            m_Pixels = imageArray;

            m_Width = width;
            m_Height = height;
            MaxSignalValue = maxSignalValue;
        }

		public uint[,] GetMeasurableAreaPixels(int xCenter, int yCenter)
		{
			return GetMeasurableAreaPixels(xCenter, yCenter, 17);
		}

		public uint[,] GetMeasurableAreaPixels(int xCenter, int yCenter, int matrixSize)
		{
			uint[,] pixels = new uint[matrixSize, matrixSize];

			int height = m_Height;
			int width = m_Width;

			int x0 = xCenter;
			int y0 = yCenter;

			int halfWidth = matrixSize / 2;

			for (int x = x0 - halfWidth; x <= x0 + halfWidth; x++)
				for (int y = y0 - halfWidth; y <= y0 + halfWidth; y++)
				{
					uint byteVal = 0;

					if (x >= 0 && x < width && y >= 0 & y < height)
					{
						byteVal = (uint)m_Pixels[y, x]; /* Y and X are reversed because of the layout in which the pixels are stored */
					}

					pixels[x - x0 + halfWidth, y - y0 + halfWidth] = byteVal;
				}

			return pixels;
		}
	}
}
