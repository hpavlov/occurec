using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace OccuRec.Helpers
{
    public class SpectraPoint
    {
        public int PixelNo;
        public float Wavelength;
        public float RawValue;
	    public float RawSignal;
	    public float ProcessedValue;
	    public int RawSignalPixelCount;
	    public float RawBackgroundPerPixel;
        public float RawBackground;
        public float SmoothedValue;
        public bool HasSaturatedPixels;
    }

    public class Spectra
    {
        public int PixelWidth;
        public uint MaxPixelValue;
		public int ZeroOrderPixelNo;
	    public uint MaxSpectraValue;

        public List<SpectraPoint> Points = new List<SpectraPoint>();
    }

    public enum SpectraCombineMethod
    {
        Average,
        Median
    }


	public class SpectraReader
	{
		private AstroImage m_Image;
		private RotationMapper m_Mapper;
		private RectangleF m_SourceVideoFrame;

		private uint[] m_BgValues;
		private uint[] m_BgPixelCount;
		private List<uint>[] m_BgValuesList;

		internal SpectraReader(AstroImage image, float angleDegrees)
		{
			m_Image = image;
			m_Mapper = new RotationMapper(image.Width, image.Height, angleDegrees);
			m_SourceVideoFrame = new RectangleF(0, 0, image.Width, image.Height);
		}

		public Spectra ReadSpectra(float x0, float y0, int halfWidth)
		{
			int bgHalfWidth = halfWidth;
			var rv = new Spectra()
			{
				PixelWidth = 2 * halfWidth,
				MaxPixelValue = (uint)(2 * halfWidth) * m_Image.MaxSignalValue
			};

			int xFrom = int.MaxValue;
			int xTo = int.MinValue;

			// Find the destination pixel range at the destination horizontal
			PointF p1 = m_Mapper.GetDestCoords(x0, y0);
			rv.ZeroOrderPixelNo = (int)Math.Round(p1.X);

			for (float x = p1.X - m_Mapper.MaxDestDiagonal; x < p1.X + m_Mapper.MaxDestDiagonal; x++)
			{
				PointF p = m_Mapper.GetSourceCoords(x, p1.Y);
				if (m_SourceVideoFrame.Contains(p))
				{
					int xx = (int)x;

					if (xx < xFrom) xFrom = xx;
					if (xx > xTo) xTo = xx;
				}
			}

			m_BgValues = new uint[xTo - xFrom + 1];
			m_BgPixelCount = new uint[xTo - xFrom + 1];
			m_BgValuesList = new List<uint>[xTo - xFrom + 1];

			// Get all readings in the range
			for (int x = xFrom; x <= xTo; x++)
			{
				var point = new SpectraPoint();
				point.PixelNo = x;
				point.RawSignalPixelCount = 0;

				for (int z = -halfWidth; z <= halfWidth; z++)
				{
					PointF p = m_Mapper.GetSourceCoords(x, p1.Y + z);
					int xx = (int)Math.Round(p.X);
					int yy = (int)Math.Round(p.Y);

					if (m_SourceVideoFrame.Contains(xx, yy))
					{
						float sum = 0;
						int numPoints = 0;
						for (float kx = -0.4f; kx < 0.5f; kx += 0.2f)
							for (float ky = -0.4f; ky < 0.5f; ky += 0.2f)
							{
								p = m_Mapper.GetSourceCoords(x + kx, p1.Y + ky + z);
								int xxx = (int)Math.Round(p.X);
								int yyy = (int)Math.Round(p.Y);
								if (m_SourceVideoFrame.Contains(xxx, yyy))
								{
									sum += m_Image.GetPixel(xxx, yyy);
									numPoints++;
								}
							}
						point.RawValue += (sum / numPoints);
						point.RawSignalPixelCount++;
					}
				}

				point.RawSignal = point.RawValue;
				rv.Points.Add(point);

				ReadMedianBackgroundForPixelIndex(halfWidth, bgHalfWidth, x, p1.Y, x - xFrom);
			}

			// Apply background
			foreach (SpectraPoint point in rv.Points)
			{
				point.RawBackgroundPerPixel = GetMedianBackgroundValue(point.PixelNo, xFrom, xTo, bgHalfWidth);
				point.RawValue -= point.RawBackgroundPerPixel * point.RawSignalPixelCount;
				if (point.RawValue < 0) point.RawValue = 0;
			}

			rv.MaxSpectraValue = (uint)Math.Ceiling(rv.Points.Where(x => x.PixelNo > rv.ZeroOrderPixelNo + 20).Select(x => x.RawValue).Max());

			return rv;
		}

		private float GetMedianBackgroundValue(int pixelNo, int xFrom, int xTo, int horizontalSpan)
		{
			var allAreaBgPixels = new List<uint>();
			int idxFrom = Math.Max(xFrom, pixelNo - horizontalSpan);
			int idxTo = Math.Min(xTo, pixelNo + horizontalSpan);

			for (int i = idxFrom; i <= idxTo; i++)
			{
				allAreaBgPixels.AddRange(m_BgValuesList[i - xFrom]);
			}

			allAreaBgPixels.Sort();

			return allAreaBgPixels.Count == 0
				? 0
				: allAreaBgPixels[allAreaBgPixels.Count / 2];
		}

		private float GetAverageBackgroundValue(int pixelNo, int xFrom, int xTo, int horizontalSpan)
		{
			int idxFrom = Math.Max(xFrom, pixelNo - horizontalSpan);
			int idxTo = Math.Min(xTo, pixelNo + horizontalSpan);
			float bgSum = 0;
			uint pixCount = 0;
			for (int i = idxFrom; i <= idxTo; i++)
			{
				bgSum += m_BgValues[i - xFrom];
				pixCount += m_BgPixelCount[i - xFrom];
			}
			return pixCount == 0
				? 0
				: bgSum / pixCount;
		}

		private void ReadMedianBackgroundForPixelIndex(int halfWidth, int bgHalfWidth, float x1, float y1, int index)
		{
			var allBgPixels = new List<uint>();

			for (int z = -bgHalfWidth - halfWidth; z < -halfWidth; z++)
			{
				PointF p = m_Mapper.GetSourceCoords(x1, y1 + z);
				int xx = (int)Math.Round(p.X);
				int yy = (int)Math.Round(p.Y);

				if (m_SourceVideoFrame.Contains(xx, yy))
				{
					allBgPixels.Add((uint)m_Image.GetPixel(xx, yy));
				}
			}

			for (int z = halfWidth + 1; z <= halfWidth + bgHalfWidth; z++)
			{
				PointF p = m_Mapper.GetSourceCoords(x1, y1 + z);
				int xx = (int)Math.Round(p.X);
				int yy = (int)Math.Round(p.Y);

				if (m_SourceVideoFrame.Contains(xx, yy))
				{
					allBgPixels.Add((uint)m_Image.GetPixel(xx, yy));
				}
			}

			m_BgValuesList[index] = allBgPixels;
			m_BgPixelCount[index] = 1;
		}

		private void ReadAverageBackgroundForPixelIndex(int halfWidth, int bgHalfWidth, float x1, float y1, int index)
		{
			uint bgValue = 0;
			uint bgPixelCount = 0;

			for (int z = -bgHalfWidth - halfWidth; z < -halfWidth; z++)
			{
				PointF p = m_Mapper.GetSourceCoords(x1, y1 + z);
				int xx = (int)Math.Round(p.X);
				int yy = (int)Math.Round(p.Y);

				if (m_SourceVideoFrame.Contains(xx, yy))
				{
					bgValue += (uint)m_Image.GetPixel(xx, yy);
					bgPixelCount++;
				}
			}

			for (int z = halfWidth + 1; z <= halfWidth + bgHalfWidth; z++)
			{
				PointF p = m_Mapper.GetSourceCoords(x1, y1 + z);
				int xx = (int)Math.Round(p.X);
				int yy = (int)Math.Round(p.Y);

				if (m_SourceVideoFrame.Contains(xx, yy))
				{
					bgValue += (uint)m_Image.GetPixel(xx, yy);
					bgPixelCount++;
				}
			}

			m_BgValues[index] = bgValue;
			m_BgPixelCount[index] = bgPixelCount;
		}
	}

}
