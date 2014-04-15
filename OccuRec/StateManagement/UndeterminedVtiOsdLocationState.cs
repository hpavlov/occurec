using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using OccuRec.Context;
using OccuRec.Helpers;
using OccuRec.Properties;
using OccuRec.Utilities;

namespace OccuRec.StateManagement
{
	public class UndeterminedVtiOsdLocationState : CameraState
	{
        public static UndeterminedVtiOsdLocationState Instance = new UndeterminedVtiOsdLocationState();

		private int m_FromLine;
		private int m_ToLine;
		private uint[] m_OddFieldPixels;
		private uint[] m_EvenFieldPixels;
		private uint[] m_OddFieldPixelsPreProcessed;
		private uint[] m_EvenFieldPixelsPreProcessed;
		private uint[] m_OddFieldPixelsDebugNoLChD;
		private uint[] m_EvenFieldPixelsDebugNoLChD;
		private int m_FieldAreaHeight;
		private int m_FieldAreaWidth;
		private bool m_IotaVtiTvSafeMode;
	    private volatile Bitmap m_BitmapFrame = null;
	    private object m_SyncLock = new object();
	    private bool m_ConfigureMessageSent = false;
        private bool m_ConfirmMessageSent = false;

		private int m_AttemptedFrames = 0;
	    private CameraStateManager m_StateManager;

		public bool VtiOsdAutomaticDetectionFailed
		{
			get { return m_AttemptedFrames > 10; }
		}

        public override void InitialiseState(CameraStateManager stateManager)
		{
            base.InitialiseState(stateManager);

			m_AttemptedFrames = 0;
            m_StateManager = stateManager;

            m_ConfigureMessageSent = false;
            m_ConfirmMessageSent = false;

			Settings.Default.PreserveVtiOsdFirstRawAuto = 0;
			Settings.Default.PreserveVtiOsdLastRawAuto = 0;

			// We want frames to be output one by one while configuring the VTI-OSD location
			NativeHelpers.UnlockIntegration();
		}

		public override void ProcessFrame(CameraStateManager stateManager, Helpers.VideoFrameWrapper frame)
		{
            if (frame.ImageArray is int[,] && frame.PreviewBitmap != null)
                ProcessFrame(stateManager, (int[,])frame.ImageArray, frame.PreviewBitmap.Width, frame.PreviewBitmap.Height);
            else if (frame.PreviewBitmap != null)
			{
                var pixels = (int[,])ImageUtils.GetPixelArray(frame.PreviewBitmap.Width, frame.PreviewBitmap.Height, frame.PreviewBitmap);
                ProcessFrame(stateManager, pixels, frame.PreviewBitmap.Width, frame.PreviewBitmap.Height);
			}
		}

        private void ProcessFrame(CameraStateManager stateManager, int[,] pixels, int width, int height)
        {
            if (pixels != null)
            {
                int imageWidth = pixels.GetLength(1);
                int imageHeight = pixels.GetLength(0);

                uint[] data = new uint[imageWidth * imageHeight];

                if (imageWidth == width && imageHeight == height)
                {
                    for (int y = 0; y < imageHeight; y++)
                    {
                        for (int x = 0; x < imageWidth; x++)
                        {
                            data[x + y*imageWidth] = (uint) pixels[y, x];
                        }
                    }
                }
                else if (imageWidth == height && imageHeight == width)
                {
                    imageWidth = width;
                    imageHeight = height;

                    for (int y = 0; y < imageHeight; y++)
                    {
                        for (int x = 0; x < imageWidth; x++)
                        {
                            data[x + y * imageWidth] = (uint)pixels[x, y];
                        }
                    }
                }

                
                if (LocateTimestampPosition(data, imageWidth, imageHeight))
                {
                    Trace.WriteLine(string.Format("OSD Located at {0}-{1} (Width: {2}, Height: {3})", m_FromLine, m_ToLine, imageWidth, imageHeight));
#if DEBUG
					if (Settings.Default.SimulateFailedVtiOsdDetection)
					{
						m_AttemptedFrames++;
						return;
					}
#endif
                    Settings.Default.PreserveVtiOsdFirstRawAuto = m_FromLine;
                    Settings.Default.PreserveVtiOsdLastRawAuto = m_ToLine;

                    NativeHelpers.SetupTimestampPreservation(true, Settings.Default.PreserveVtiOsdFirstRawAuto, (Settings.Default.PreserveVtiOsdLastRawAuto - Settings.Default.PreserveVtiOsdFirstRawAuto) / 2);

                    // Keep showing the AssumedVtiOsd lines for another 5 sec for user's visual confirmation that they are correct
                    OccuRecContext.Current.ShowAssumedVtiOsdPositionUntil = DateTime.Now.AddSeconds(5);

                    stateManager.ChangeState(UndeterminedIntegrationCameraState.Instance);
                }
                else
                {
                    m_AttemptedFrames++;
                }

                if (VtiOsdAutomaticDetectionFailed)
                {
                    if (!Settings.Default.PreserveVTIUserSpecifiedValues)
                    {
                        if (!m_ConfigureMessageSent)
                            stateManager.VideoObject.OnInfo("Please configure the VTI-OSD position.");

                        m_ConfigureMessageSent = true;
                    }
                    else
                    {
                        if (!m_ConfirmMessageSent)
                            stateManager.VideoObject.OnInfo("Please confirm the VTI-OSD position.");

                        m_ConfirmMessageSent = true;
                    }
                }
            }
        }

		private void LocateTopAndBottomLineOfTimestamp(uint[] preProcessedPixels, int imageWidth, int fromHeight, int toHeight, out int bestTopPosition, out int bestBottomPosition)
		{
			int bestTopScope = -1;
			bestBottomPosition = -1;
			bestTopPosition = -1;
			int bestBottomScope = -1;


			for (int y = fromHeight + 1; y < toHeight - 1; y++)
			{
				int topScore = 0;
				int bottomScore = 0;

				for (int x = 0; x < imageWidth; x++)
				{
					if (preProcessedPixels[x + imageWidth * (y + 1)] < 127 && preProcessedPixels[x + imageWidth * y] > 127)
					{
						topScore++;
					}

					if (preProcessedPixels[x + imageWidth * (y - 1)] < 127 && preProcessedPixels[x + imageWidth * y] > 127)
					{
						bottomScore++;
					}
				}

				if (topScore > bestTopScope)
				{
					bestTopScope = topScore;
					bestTopPosition = y;
				}

				if (bottomScore > bestBottomScope)
				{
					bestBottomScope = bottomScore;
					bestBottomPosition = y;
				}
			}
		}

		private bool LocateTimestampPosition(uint[] data, int frameWidth, int frameHeight)
		{
			uint[] preProcessedPixels = new uint[data.Length];
			Array.Copy(data, preProcessedPixels, data.Length);

			// Process the image
			uint median = preProcessedPixels.Median();
			for (int i = 0; i < preProcessedPixels.Length; i++)
			{
				int darkCorrectedValue = (int)preProcessedPixels[i] - (int)median;
				if (darkCorrectedValue < 0) darkCorrectedValue = 0;
				preProcessedPixels[i] = (uint)darkCorrectedValue;
			}

			if (median > 250)
			{
				//InitiazliationError = "The background is too bright.";
				return false;
			}

			uint[] blurResult = BitmapFilter.GaussianBlur(preProcessedPixels, 8, frameWidth, frameHeight);
			uint average = 128;
			uint[] sharpenResult = BitmapFilter.Sharpen(blurResult, 8, frameWidth, frameHeight, out average);

			// Binerize and Inverse
			for (int i = 0; i < sharpenResult.Length; i++)
			{
				sharpenResult[i] = sharpenResult[i] > average ? (uint)0 : (uint)255;
			}
			uint[] denoised = BitmapFilter.Denoise(sharpenResult, 8, frameWidth, frameHeight, out average, false);

			for (int i = 0; i < denoised.Length; i++)
			{
				preProcessedPixels[i] = denoised[i] < 127 ? (uint)0 : (uint)255;
			}

			int bestBottomPosition = -1;
			int bestTopPosition = -1;
			LocateTopAndBottomLineOfTimestamp(
				preProcessedPixels,
				frameWidth,
				frameHeight / 2 + 1,
				frameHeight,
				out bestTopPosition,
				out bestBottomPosition);

            if (bestBottomPosition - bestTopPosition < 10 || bestBottomPosition - bestTopPosition > 60 || bestTopPosition < 0 || bestBottomPosition > frameHeight)
			{
				//InitiazliationError = "Cannot locate the OSD timestamp on the frame.";
				return false;
			}

			m_FromLine = bestTopPosition - 1;
			m_ToLine = bestBottomPosition + 3;
			if (m_ToLine > frameHeight)
				m_ToLine = frameHeight - 2;

			if ((m_ToLine - m_FromLine) % 2 == 1)
			{
				if (m_FromLine % 2 == 1)
					m_FromLine--;
				else
					m_ToLine++;
			}

			#region We need to make sure that the two fields have the same top and bottom lines

			// Create temporary arrays so the top/bottom position per field can be further refined
			m_FieldAreaHeight = (m_ToLine - m_FromLine) / 2;
			m_FieldAreaWidth = frameWidth;
			m_OddFieldPixels = new uint[frameWidth * m_FieldAreaHeight];
			m_EvenFieldPixels = new uint[frameWidth * m_FieldAreaHeight];
			m_OddFieldPixelsPreProcessed = new uint[frameWidth * m_FieldAreaHeight];
			m_EvenFieldPixelsPreProcessed = new uint[frameWidth * m_FieldAreaHeight];
			m_OddFieldPixelsDebugNoLChD = new uint[frameWidth * m_FieldAreaHeight];
			m_EvenFieldPixelsDebugNoLChD = new uint[frameWidth * m_FieldAreaHeight];

			int[] DELTAS = new int[] { 0, -1, 1 };
			int fromLineBase = m_FromLine;
			int toLineBase = m_ToLine;
			bool matchFound = false;

			PrepareOsdVideoFields(data, frameWidth);

			for (int deltaIdx = 0; deltaIdx < DELTAS.Length; deltaIdx++)
			{
				m_FromLine = fromLineBase + DELTAS[deltaIdx];
				m_ToLine = toLineBase + DELTAS[deltaIdx];

				int bestBottomPositionOdd = -1;
				int bestTopPositionOdd = -1;
				int bestBottomPositionEven = -1;
				int bestTopPositionEven = -1;

				LocateTopAndBottomLineOfTimestamp(
					m_OddFieldPixelsPreProcessed,
					m_FieldAreaWidth, 1, m_FieldAreaHeight - 1,
					out bestTopPositionOdd, out bestBottomPositionOdd);

				LocateTopAndBottomLineOfTimestamp(
					m_EvenFieldPixelsPreProcessed,
					m_FieldAreaWidth, 1, m_FieldAreaHeight - 1,
					out bestTopPositionEven, out bestBottomPositionEven);

				if (bestBottomPositionOdd == bestBottomPositionEven &&
					bestTopPositionOdd == bestTopPositionEven)
				{
					matchFound = true;
					m_FromLine = fromLineBase;
					m_ToLine = toLineBase;

					break;
				}
			}
			#endregion

			m_IotaVtiTvSafeMode = m_ToLine + (m_ToLine - m_FromLine) / 2 < frameHeight;

			return matchFound;
		}

		private void PrepareOsdVideoFields(uint[] data, int frameWidth)
		{
			for (int y = m_FromLine; y < m_ToLine; y++)
			{
				bool isOddLine = y % 2 == 1;
				int lineNo = (y - m_FromLine) / 2;
				if (isOddLine)
					Array.Copy(data, y * m_FieldAreaWidth, m_OddFieldPixels, lineNo * m_FieldAreaWidth, m_FieldAreaWidth);
				else
					Array.Copy(data, y * m_FieldAreaWidth, m_EvenFieldPixels, lineNo * m_FieldAreaWidth, m_FieldAreaWidth);
			}

			PrepareOsdArea(m_OddFieldPixels, m_OddFieldPixelsPreProcessed, m_OddFieldPixelsDebugNoLChD, frameWidth, m_FieldAreaHeight);
			PrepareOsdArea(m_EvenFieldPixels, m_EvenFieldPixelsPreProcessed, m_EvenFieldPixelsDebugNoLChD, frameWidth, m_FieldAreaHeight);
		}

		private void PrepareOsdArea(uint[] dataIn, uint[] dataOut, uint[] dataDebugNoLChD, int width, int height)
		{
			// Split into fields only in the region where IOTA-VTI could be, Then threat as two separate images, and for each of them do:
			// 1) Gaussian blur (small) BitmapFilter.LOW_PASS_FILTER_MATRIX
			// 2) Sharpen BitmapFilter.SHARPEN_MATRIX
			// 3) Binarize - get Average, all below change to 0, all above change to Max (256)
			// 4) De-noise BitmapFilter.DENOISE_MATRIX

			uint median = dataIn.Median();
			for (int i = 0; i < dataIn.Length; i++)
			{
				int darkCorrectedValue = (int)dataIn[i] - (int)median;
				if (darkCorrectedValue < 0) darkCorrectedValue = 0;
				dataIn[i] = (uint)darkCorrectedValue;
			}

			uint[] blurResult = BitmapFilter.GaussianBlur(dataIn, 8, width, height);

			uint average = 128;
			uint[] sharpenResult = BitmapFilter.Sharpen(blurResult, 8, width, height, out average);

			// Binerize and Inverse
			for (int i = 0; i < sharpenResult.Length; i++)
			{
				sharpenResult[i] = sharpenResult[i] > average ? (uint)0 : (uint)255;
			}

			uint[] denoised = BitmapFilter.Denoise(sharpenResult, 8, width, height, out average, false);

			for (int i = 0; i < denoised.Length; i++)
			{
				dataOut[i] = denoised[i] < 127 ? (uint)0 : (uint)255;
			}

			Array.Copy(dataOut, dataDebugNoLChD, dataOut.Length);
		}

		public static Bitmap ConstructBitmapFromBitmapPixels(uint[] pixels, int width, int height)
		{
			var bmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);

			// GDI+ still lies to us - the return format is BGR, NOT RGB.
			BitmapData bmData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

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
						byte color;
						int index = x + y * width;
						color = (byte)(pixels[index]);

						p[0] = color;
						p[1] = color;
						p[2] = color;

						p += 3;
					}
					p += nOffset;
				}
			}

			bmp.UnlockBits(bmData);

			return bmp;
		}
	}
}
