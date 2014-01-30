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

		public AstroImage(IVideoFrame videoFrame, int width, int height)
		{
            m_Pixels = (int[,]) videoFrame.ImageArray;

			m_Width = width;
			m_Height = height;
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
