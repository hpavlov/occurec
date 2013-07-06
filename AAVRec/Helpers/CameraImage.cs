using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using AAVRec.Drivers;

namespace AAVRec.Helpers
{
    public interface ICameraImage
    {
        void SetImageArray(object imageArray, int imageWidth, int imageHeight, SensorType sensorType);
        int GetPixel(int x, int y);
	    int GetPixel(int x, int y, int plain);
        IntPtr GetDisplayHBitmap();
        Bitmap GetDisplayBitmap();
        object GetImageArray(Bitmap bmp, SensorType sensorType, LumaConversionMode conversionMode, bool flipHorizontally, bool flipVertically);
    }

    public class CameraImage : ICameraImage
    {
        private object imageArray;
        private int[,] intPixelArray;
        private object[,] objPixelArray;
	    private int[,,] intColourPixelArray;
		private object[,,] objColourPixelArray;
        private int imageWidth;
        private int imageHeight;
        private SensorType sensorType;

        private bool isColumnMajor;
        private bool isRowMajor;

        public void SetImageArray(object imageArray, int imageWidth, int imageHeight, SensorType sensorType)
        {
            this.imageArray = imageArray;
            this.imageWidth = imageWidth;
            this.imageHeight = imageHeight;
            this.sensorType = sensorType;

            objPixelArray = null;
            intPixelArray = null;
	        intColourPixelArray = null;
	        objColourPixelArray = null;

            if (sensorType == SensorType.Monochrome)
            {
                if (imageArray is int[,])
                {
                    intPixelArray = (int[,])imageArray;
                    isColumnMajor = intPixelArray.GetLength(0) == imageWidth;
                    isRowMajor = intPixelArray.GetLength(0) == imageHeight;
                    return;
                }
                else if (imageArray is object[,])
                {
                    objPixelArray = (object[,])imageArray;
                    isColumnMajor = objPixelArray.GetLength(0) == imageWidth;
                    isRowMajor = objPixelArray.GetLength(0) == imageHeight;
                    return;
                }
            }
            else
            {
                // Color sensor type is represented as 3-dimentional array that can be either: [3, height, width], [width, height, 3]
                if (imageArray is int[,,])
                {
	                intColourPixelArray = (int[,,]) imageArray;
					isColumnMajor = intColourPixelArray.GetLength(0) == imageWidth;
					isRowMajor = intColourPixelArray.GetLength(0) == 3;
					return;
                }
                else if (imageArray is object[, ,])
                {
					objColourPixelArray = (object[, ,])imageArray;
					isColumnMajor = objColourPixelArray.GetLength(0) == imageWidth;
					isRowMajor = objColourPixelArray.GetLength(0) == 3;
					return;
                }
            }

            throw new ArgumentException();
        }

        public IntPtr GetDisplayHBitmap()
        {
            IntPtr hBitmap = IntPtr.Zero;
            using(Bitmap bmp = GetDisplayBitmap())
            {
                if (bmp != null)
                {
                    hBitmap = bmp.GetHbitmap();
                    bmp.Dispose();
                }
            }

            return hBitmap;
        }

        public Bitmap GetDisplayBitmap()
        {
			if (sensorType == SensorType.Monochrome)
			{
				if (intPixelArray != null)
					return NativeHelpers.PrepareBitmapForDisplay(intPixelArray, imageWidth, imageHeight);
				else if (objPixelArray != null)
					return NativeHelpers.PrepareBitmapForDisplay(objPixelArray, imageWidth, imageHeight);				
			}
			else if (sensorType == SensorType.Color)
			{
				if (intColourPixelArray != null)
					return NativeHelpers.PrepareColourBitmapForDisplay(intColourPixelArray, imageWidth, imageHeight);
				else if (objColourPixelArray != null)
					return NativeHelpers.PrepareColourBitmapForDisplay(objColourPixelArray, imageWidth, imageHeight);
			}

            return null;
        }

        public int GetPixel(int x, int y)
        {
            if (intPixelArray != null)
            {
                if (isRowMajor)
                    return intPixelArray[y, x];
                else if (isColumnMajor)
                    return intPixelArray[x, y];
            }
            else if (objPixelArray != null)
            {
                if (isRowMajor)
                    return (int)objPixelArray[y, x];
                else if (isColumnMajor)
                    return (int)objPixelArray[x, y];                
            }
			else if (intColourPixelArray != null || objColourPixelArray != null)
			{
				throw new InvalidOperationException("Use the GetPixel(int, int, int) overload to get a pixel value in a colour image.");
			}
			
			throw new InvalidOperationException();
        }

	    public int GetPixel(int x, int y, int plain)
	    {
			if (intPixelArray != null || objPixelArray != null)
			{
				throw new InvalidOperationException("Use the GetPixel(int, int) overload to get a pixel value in a monochrome image.");
			}
			else if (intColourPixelArray != null)
			{
				if (isRowMajor)
					return intColourPixelArray[plain, y, x];
				else if (isColumnMajor)
					return intColourPixelArray[x, y, plain];
			}
			else if (objColourPixelArray != null)
			{
				if (isRowMajor)
					return (int)objColourPixelArray[plain, y, x];
				else if (isColumnMajor)
					return (int)objColourPixelArray[x, y, plain];
			}

			throw new InvalidOperationException();
	    }

	    public object GetImageArray(Bitmap bmp, SensorType sensorType, LumaConversionMode conversionMode, bool flipHorizontally, bool flipVertically)
		{
			this.imageWidth = bmp.Width;
			this.imageHeight = bmp.Height;

	        short flipMode = 0;
	        if (flipHorizontally) flipMode += 1;
            if (flipVertically) flipMode += 2;
			if (sensorType == SensorType.Monochrome)
				return NativeHelpers.GetMonochromePixelsFromBitmap(bmp, conversionMode);
			else if (sensorType == SensorType.Color)
                return NativeHelpers.GetColourPixelsFromBitmap(bmp, flipMode);
			else
				throw new NotSupportedException(string.Format("Sensor type {0} is not currently supported.", sensorType));
		}
    }
}
