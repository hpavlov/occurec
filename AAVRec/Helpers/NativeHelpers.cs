using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using AAVRec.OCR;

namespace AAVRec.Helpers
{
	public enum LumaConversionMode
	{
		R = 0,
		G = 1,
		B = 2,
		GrayScale = 3
	}

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    internal class ImageStatus
    {
        //[FieldOffset(0)]
        public long StartExposureTicks;
        //[FieldOffset(8)]
        public long EndExposureTicks;
        //[FieldOffset(16)]
        public long StartExposureFrameNo;
        //[FieldOffset(24)]
        public long EndExposureFrameNo;
        //[FieldOffset(32)]
        public int CountedFrames;
        //[FieldOffset(36)]
        public float CutOffRatio;
        //[FieldOffset(40)]
        public long IntegratedFrameNo;

        public ImageStatus()
        { }

        public ImageStatus(ImageStatus clone)
        {
            
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    struct FrameProcessingStatus
    {
        public long CameraFrameNo;
        public long IntegratedFrameNo;
        public int IntegratedFramesSoFar;
        public float FrameDiffSignature;
        public float CurrentSignatureRatio;
    };

	internal static class NativeHelpers
	{
        private const string AAVREC_CORE_DLL_NAME = "AAVRec.Core.dll";

		[DllImport(AAVREC_CORE_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern int GetBitmapPixels(
			int width, 
			int height, 
			int bpp,
			[In, MarshalAs(UnmanagedType.LPArray)] int[,] pixels,
			[In, Out] byte[] bitmapBytes);

		[DllImport(AAVREC_CORE_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GetColourBitmapPixels(
			int width,
			int height,
			int bpp,
			[In, MarshalAs(UnmanagedType.LPArray)] int[,,] pixels,
			[In, Out] byte[] bitmapBytes);

		[DllImport(AAVREC_CORE_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GetMonochromePixelsFromBitmap(
			int width,
			int height,
			int bpp,
			[In] IntPtr hBitmap,
			[In, Out, MarshalAs(UnmanagedType.LPArray)] int[,] bitmapBytes,
			int mode);

		[DllImport(AAVREC_CORE_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
		private static extern int GetColourPixelsFromBitmap(
			int width,
			int height,
			int bpp,
			[In] IntPtr hBitmap,
			[In, Out, MarshalAs(UnmanagedType.LPArray)] int[,,] bitmapBytes);

        [DllImport(AAVREC_CORE_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SetupCamera(int width, int height, string cameraModel, int monochromeConversionMode, bool flipHorizontally, bool flipVertically, bool isIntegrating, float signDiffFactor);

        [DllImport(AAVREC_CORE_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern int ProcessVideoFrame([In] IntPtr ptrBitmapData, long currentUtcDayAsTicks, [In, Out] ref FrameProcessingStatus frameInfo);

        [DllImport(AAVREC_CORE_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern int GetCurrentImage([In, Out] byte[] bitmapPixels);

        [DllImport(AAVREC_CORE_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern int GetCurrentImageStatus([In, Out] ImageStatus status);

        [DllImport(AAVREC_CORE_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern int StartRecording(string fileName);

        [DllImport(AAVREC_CORE_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern int StopRecording();

        [DllImport(AAVREC_CORE_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SetTimeStampArea1(int top, int left, int width, int height);

        [DllImport(AAVREC_CORE_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern int SetTimeStampArea2(int top, int left, int width, int height);


        [DllImport("Kernel32.dll", EntryPoint = "RtlMoveMemory")]
        public static extern void CopyMemory(IntPtr Destination, IntPtr Source, [MarshalAs(UnmanagedType.U4)] uint Length);

		[DllImport("gdi32.dll")]
		public static extern bool DeleteObject(IntPtr hObject);


        public static Bitmap PrepareBitmapForDisplay(int[,] imageArray, int width, int height)
        {
            return PrepareBitmapForDisplay(imageArray, width, height, false);
        }

        public static Bitmap PrepareBitmapForDisplay(object[,] imageArray, int width, int height)
        {
            return PrepareBitmapForDisplay(imageArray, width, height, true);
        }

		public static Bitmap PrepareColourBitmapForDisplay(int[, ,] imageArray, int width, int height)
		{
			return PrepareColourBitmapForDisplay(imageArray, width, height, false);
		}

		public static Bitmap PrepareColourBitmapForDisplay(object[, ,] imageArray, int width, int height)
		{
			return PrepareColourBitmapForDisplay(imageArray, width, height, true);
		}

		public static object GetMonochromePixelsFromBitmap(Bitmap bitmap, LumaConversionMode conversionMode)
		{
			int[,] bitmapBytes = new int[bitmap.Width, bitmap.Height];

			IntPtr hBitmap = bitmap.GetHbitmap();
			try
			{
				GetMonochromePixelsFromBitmap(bitmap.Width, bitmap.Height, 8, hBitmap, bitmapBytes, (int)conversionMode);
			}
			finally
			{
				DeleteObject(hBitmap);
			}

			return bitmapBytes;
		}

		public static object GetColourPixelsFromBitmap(Bitmap bitmap)
		{
			int[,,] bitmapBytes = new int[bitmap.Width, bitmap.Height, 3];

			IntPtr hBitmap = bitmap.GetHbitmap();
			try
			{
				GetColourPixelsFromBitmap(bitmap.Width, bitmap.Height, 8, hBitmap, bitmapBytes);
			}
			finally
			{
				DeleteObject(hBitmap);
			}

			return bitmapBytes;			
		}

	    private static Bitmap PrepareBitmapForDisplay(object imageArray, int width, int height, bool useVariantPixels)
		{
			Bitmap displayBitmap = null;

			int[,] pixels = new int[height, width];

			if (useVariantPixels)
			{
                Array safeArr = (Array)imageArray;
				Array.Copy(safeArr, pixels, pixels.Length);
			}
			else
			{
                Array safeArr = (Array)imageArray;
				Array.Copy(safeArr, pixels, pixels.Length);
			}

			byte[] rawBitmapBytes = new byte[(width * height * 3) + 40 + 14 + 1];

            GetBitmapPixels(width, height, (int)8, pixels, rawBitmapBytes);

			using (MemoryStream memStr = new MemoryStream(rawBitmapBytes))
			{
				displayBitmap = (Bitmap)Image.FromStream(memStr);
			}

			return displayBitmap;
		}

		private static Bitmap PrepareColourBitmapForDisplay(object imageArray, int width, int height, bool useVariantPixels)
		{
			Bitmap displayBitmap = null;

			int[,,] pixels = new int[height, width, 3];

			if (useVariantPixels)
			{
				Array safeArr = (Array)imageArray;
				Array.Copy(safeArr, pixels, pixels.Length);
			}
			else
			{
				Array safeArr = (Array)imageArray;
				Array.Copy(safeArr, pixels, pixels.Length);
			}

			byte[] rawBitmapBytes = new byte[(width * height * 3) + 40 + 14 + 1];

			GetColourBitmapPixels(width, height, (int)8, pixels, rawBitmapBytes);

			using (MemoryStream memStr = new MemoryStream(rawBitmapBytes))
			{
				displayBitmap = (Bitmap)Image.FromStream(memStr);
			}

			return displayBitmap;
		}

        public static void StartRecordingVideoFile(string fileName)
        {
            StartRecording(fileName);
        }

        public static void StopRecordingVideoFile()
        {
            StopRecording();
        }

        public static FrameProcessingStatus ProcessVideoFrame(IntPtr bitmapData)
        {
            var frameInfo = new FrameProcessingStatus();

            long currentUtcDayAsTicks = DateTime.UtcNow.Ticks;

            

            ProcessVideoFrame(bitmapData, currentUtcDayAsTicks, ref frameInfo);

            Trace.WriteLine(string.Format("Diff Signature: {0} (CameraFrameNo: {1}; IntegratedFrameNo: {2}; CurrentSignatureRatio: {3})", frameInfo.FrameDiffSignature, frameInfo.CameraFrameNo, frameInfo.IntegratedFrameNo, frameInfo.CurrentSignatureRatio));

            return frameInfo;
        }

        private static int imageWidth;
        private static int imageHeight;
	    private static Font s_ErrorFont = new Font(FontFamily.GenericMonospace, 9f, GraphicsUnit.Pixel);

        public static void SetupCamera(string cameraModel, int width, int height, bool flipHorizontally, bool flipVertically, bool isIntegrating, float signDiffFactor)
        {
            imageWidth = width;
            imageHeight = height;

            SetupCamera(width, height, cameraModel, 0, flipHorizontally, flipVertically, isIntegrating, signDiffFactor);
        }

        public static Bitmap GetCurrentImage(out ImageStatus status)
        {
            Bitmap videoFrame = null;
            status = new ImageStatus();

            byte[] bitmapPixels = new byte[3 * imageWidth * imageHeight + 40 + 14 + 1];

            GetCurrentImage(bitmapPixels);
            GetCurrentImageStatus(status);

            using (MemoryStream memStr = new MemoryStream(bitmapPixels))
            {
                try
                {
                    videoFrame = (Bitmap)Bitmap.FromStream(memStr);
                }
                catch (Exception ex)
                {
                    videoFrame = new Bitmap(imageWidth, imageHeight);
                    using (Graphics g = Graphics.FromImage(videoFrame))
                    {
                        g.Clear(Color.White);
                        g.DrawString(ex.Message, s_ErrorFont, Brushes.Red, 10, 10);
                        g.Save();
                    }
                }
            }

            return videoFrame;
        }

        public static void SetupOcrConfig()
        {
            OCRSettings config = OCRSettings.Instance;

            SetTimeStampArea1(config.TimeStampArea1.Top, config.TimeStampArea1.Left, config.TimeStampArea1.Width, config.TimeStampArea1. Height);
            SetTimeStampArea2(config.TimeStampArea2.Top, config.TimeStampArea2.Left, config.TimeStampArea2.Width, config.TimeStampArea2.Height);
        }
	}
}
