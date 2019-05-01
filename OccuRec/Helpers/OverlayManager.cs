/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OccuRec.Context;
using OccuRec.Drivers;
using OccuRec.FrameAnalysis;
using OccuRec.Properties;
using OccuRec.StateManagement;
using OccuRec.Tracking;
using OccuRec.Utilities.Exceptions;
using ASCOMStandard = ASCOM;

namespace OccuRec.Helpers
{
    public class OverlayManager
    {
        private Queue<string> errorMessagesQueue = new Queue<string>();
        private Queue<string> infoMessagesQueue = new Queue<string>();
        private string currentOcrStamp = null;

        private string currErrorMessageToDisplay = null;
        private DateTime displayErrorMessageUntil = DateTime.MinValue;

        private string currInfoMessageToDisplay = null;
        private DateTime displayInfoMessageUntil = DateTime.MinValue;

        private static Font overlayMessagesFont = new Font(FontFamily.GenericMonospace, 10);

        private int imageWidth;
        private int imageHeight;
	    private int framesWithoutTimestams;

        private bool exposureTimeSupported = true;

	    private OverlayState overlayState;
	    private FrameAnalysisManager analysisManager;
	    private CameraStateManager stateManager;

        private static Pen m_LocationCrossPen = new Pen(Color.FromArgb(90, 255, 0, 0));
        private static Pen[] m_GreyPens = new Pen[256];

        private object syncRoot = new object();

	    private int m_NumCheckedSpectraFrames;

        private SizeF m_TimestampSize = SizeF.Empty;

        static OverlayManager()
        {
            for (int i = 0; i < 256; i++)
            {
                m_GreyPens[i] = new Pen(Color.FromArgb(i, i, i));
            }            
        }

		public OverlayManager(int width, int height, List<string> initializationErrorMessages, FrameAnalysisManager analysisManager, CameraStateManager stateManager)
        {
            imageWidth = width;
            imageHeight = height;
            currentOcrStamp = null;
			this.analysisManager = analysisManager;
			this.stateManager = stateManager;

            foreach (string message in initializationErrorMessages)
                errorMessagesQueue.Enqueue(message);

			m_NumCheckedSpectraFrames = 0;
        }

		public void ChangeOverlayState<TNewState>() where TNewState : OverlayState, new()
		{
			if (overlayState != null)
				overlayState.Finalise();

			overlayState = new TNewState();
			overlayState.Initialise();
		}

        public void OnEvent(int eventId, string eventData)
        {
            if (eventId == 0)
                currentOcrStamp = eventData;
        }

        public void OnError(int errorCode, string errorMessage)
        {
            errorMessagesQueue.Enqueue(errorMessage);
        }

        public void OnInfo(string infoMessage)
        {
            infoMessagesQueue.Enqueue(infoMessage);
        }

        public void Finalise()
        {
            lock (syncRoot)
            {
                errorMessagesQueue.Clear();
                infoMessagesQueue.Clear();

                displayErrorMessageUntil = DateTime.MinValue;
                currErrorMessageToDisplay = null;
            }

			if (overlayState != null)
				overlayState.Finalise();

	        overlayState = null;
        }

	    private SizeF timestampMeasurement = SizeF.Empty;

        public void ProcessFrame(Graphics g, VideoFrameWrapper frame)
        {
			if (overlayState != null)
				overlayState.ProcessFrame(g);

            ProcessErrorMessages(g);
            ProcessInfoMessages(g);

			if (Settings.Default.OcrSimulatorTestMode && !Settings.Default.OcrSimulatorNativeCode)
			{
				// Only show OCR timestamp when running in managed simulated test mode
				string ocrStampToDisplay = currentOcrStamp;
				currentOcrStamp = null;
				if (ocrStampToDisplay != null)
				{
					framesWithoutTimestams = 0;
					timestampMeasurement = g.MeasureString(ocrStampToDisplay, overlayMessagesFont);

					g.FillRectangle(Brushes.DarkSlateGray, imageWidth - timestampMeasurement.Width - 9, imageHeight - timestampMeasurement.Height - 39, timestampMeasurement.Width + 6, timestampMeasurement.Height + 6);
					g.DrawString(ocrStampToDisplay, overlayMessagesFont, Brushes.Lime, imageWidth - timestampMeasurement.Width - 6, imageHeight - timestampMeasurement.Height - 36);
				}
				else
				{
					framesWithoutTimestams++;
					if (framesWithoutTimestams < 100)
						g.FillRectangle(Brushes.DarkSlateGray, imageWidth - timestampMeasurement.Width - 9, imageHeight - timestampMeasurement.Height - 39, timestampMeasurement.Width + 6, timestampMeasurement.Height + 6);
					else
						timestampMeasurement = SizeF.Empty;
				}				
			}

			if (TrackingContext.Current.IsTracking)
			{
			    Pen pen;
				if (TrackingContext.Current.TargetStar != null)
				{
					float aperture = (float)TrackingContext.Current.TargetStarConfig.ApertureInPixels;

					if (TrackingContext.Current.TargetStar.X > aperture && TrackingContext.Current.TargetStar.X < imageWidth - aperture &&
						TrackingContext.Current.TargetStar.Y > aperture && TrackingContext.Current.TargetStar.Y < imageHeight - aperture)
					{
					    pen = TrackingContext.Current.TargetStarConfig.IsFixedAperture ? Pens.Orange : Pens.Turquoise;
					    if (!TrackingContext.Current.GuidingStar.IsLocated) pen = Pens.DarkGray;
						g.DrawEllipse(pen, TrackingContext.Current.TargetStar.X - aperture, TrackingContext.Current.TargetStar.Y - aperture, 2 * aperture, 2 * aperture);

						if (TrackingContext.Current.TargetStar.HasSaturatedPixels)
						{
							float apertureOuter = aperture + 2.5f;
							g.DrawEllipse(Pens.Red, TrackingContext.Current.TargetStar.X - apertureOuter, TrackingContext.Current.TargetStar.Y - apertureOuter, 2 * apertureOuter, 2 * apertureOuter);
						}
					}
				}

				if (TrackingContext.Current.GuidingStar != null)
				{
					float aperture = (float)TrackingContext.Current.GuidingStarConfig.ApertureInPixels;

					if (TrackingContext.Current.GuidingStar.X > aperture && TrackingContext.Current.GuidingStar.X < imageWidth - aperture &&
						TrackingContext.Current.GuidingStar.Y > aperture && TrackingContext.Current.GuidingStar.Y < imageHeight - aperture)
					{
					    pen = TrackingContext.Current.GuidingStar.IsLocated ? Pens.Lime : Pens.DarkGray;
                        g.DrawEllipse(pen, TrackingContext.Current.GuidingStar.X - aperture, TrackingContext.Current.GuidingStar.Y - aperture, 2 * aperture, 2 * aperture);

						if (TrackingContext.Current.GuidingStar.HasSaturatedPixels)
						{
							float apertureOuter = aperture + 2.5f;
							g.DrawEllipse(Pens.Red, TrackingContext.Current.GuidingStar.X - apertureOuter, TrackingContext.Current.GuidingStar.Y - apertureOuter, 2 * apertureOuter, 2 * apertureOuter);
						}
					}

					if (Settings.Default.SpectraUseAid)
					{
						if (!float.IsNaN(TrackingContext.Current.SpectraAngleDeg))
						{
						    m_NumCheckedSpectraFrames++;

						    RectangleF originalVideoFrame = new RectangleF(0, 0, imageWidth, imageHeight);

						    var mapper = new RotationMapper(imageWidth, imageHeight, TrackingContext.Current.SpectraAngleDeg);
						    float halfWidth = (float) TrackingContext.Current.GuidingStar.FWHM;

						    PointF p0 = mapper.GetDestCoords(TrackingContext.Current.GuidingStar.X, TrackingContext.Current.GuidingStar.Y);
						    for (float i = p0.X - mapper.MaxDestDiagonal; i < p0.X + mapper.MaxDestDiagonal; i++)
						    {
						        PointF p1 = mapper.GetSourceCoords(i, p0.Y - halfWidth);
						        PointF p2 = mapper.GetSourceCoords(i + 1, p0.Y - halfWidth);
						        if (originalVideoFrame.Contains(p1) && originalVideoFrame.Contains(p2)) g.DrawLine(Pens.Red, p1, p2);

						        PointF p3 = mapper.GetSourceCoords(i, p0.Y + halfWidth);
						        PointF p4 = mapper.GetSourceCoords(i + 1, p0.Y + halfWidth);
						        if (originalVideoFrame.Contains(p3) && originalVideoFrame.Contains(p4)) g.DrawLine(Pens.Red, p3, p4);
						    }

						    PlotStarSpectra(g, frame);
						}
					}
				}
                else
                {
                    if (m_StackedSpectraList.Count > 0) m_StackedSpectraList.Clear();
                }

				analysisManager.DisplayData(g, imageWidth, imageHeight);
			}


			if (stateManager.VtiOsdPositionUnknown || OccuRecContext.Current.ShowAssumedVtiOsdPosition)
			{
				if (Settings.Default.PreserveVtiOsdFirstRawAuto + Settings.Default.PreserveVtiOsdLastRawAuto > 0)
					g.DrawRectangle(Pens.Lime, 0, Settings.Default.PreserveVtiOsdFirstRawAuto, imageWidth - 3, Settings.Default.PreserveVtiOsdLastRawAuto - Settings.Default.PreserveVtiOsdFirstRawAuto - 3);
				else
					g.DrawRectangle(Pens.Lime, 0, Settings.Default.PreserveVTIFirstRow, imageWidth - 3, Settings.Default.PreserveVTILastRow - Settings.Default.PreserveVTIFirstRow - 3);

				if (OccuRecContext.Current.SecondsRemainingToShowAssumedVtiOsdPosition > 0)
				{
					string outText = OccuRecContext.Current.SecondsRemainingToShowAssumedVtiOsdPosition.ToString();
					SizeF pos = g.MeasureString(outText, s_VtiOsdFont);
					if (Settings.Default.PreserveVtiOsdFirstRawAuto > 0)
						g.DrawString(outText, s_VtiOsdFont, Brushes.Lime, imageWidth - 6 - pos.Width, Settings.Default.PreserveVtiOsdFirstRawAuto);
					else
						g.DrawString(outText, s_VtiOsdFont, Brushes.Lime, imageWidth - 6 - pos.Width, Settings.Default.PreserveVTIFirstRow);
				}
				
			}

            if (Settings.Default.DisplayLocationCross)
            {
                if (m_LocationCrossPen.Color.A != Settings.Default.LocationCrossTransparency)
                {
                    if (m_LocationCrossPen != null)
                        m_LocationCrossPen.Dispose();
                    m_LocationCrossPen = new Pen(Color.FromArgb(Settings.Default.LocationCrossTransparency, 255, 0, 0));
                }
                g.DrawLine(m_LocationCrossPen, 0, Settings.Default.LocationCrossY, Settings.Default.LocationCrossX - 6, Settings.Default.LocationCrossY);
                g.DrawLine(m_LocationCrossPen, Settings.Default.LocationCrossX, 0, Settings.Default.LocationCrossX, Settings.Default.LocationCrossY - 6);
                g.DrawLine(m_LocationCrossPen, Settings.Default.LocationCrossX + 6, Settings.Default.LocationCrossY, imageWidth, Settings.Default.LocationCrossY);
                g.DrawLine(m_LocationCrossPen, Settings.Default.LocationCrossX, Settings.Default.LocationCrossY + 6, Settings.Default.LocationCrossX, imageHeight);
                g.DrawEllipse(m_LocationCrossPen, Settings.Default.LocationCrossX - 6, Settings.Default.LocationCrossY - 6, 12, 12);
            }

            if (frame != null)
            {
                if (exposureTimeSupported)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(frame.ExposureStartTime))
                        {
                            if (m_TimestampSize == SizeF.Empty)
                            {
                                m_TimestampSize = g.MeasureString(frame.ExposureStartTime, s_VtiOsdFont);
                            }

                            if (m_TimestampSize != SizeF.Empty)
                            {
                                g.DrawString(frame.ExposureStartTime, s_VtiOsdFont, Brushes.Lime, imageWidth - m_TimestampSize.Width - 10, 10);
                            }
                        }
                    }
                    catch (ASCOMStandard.PropertyNotImplementedException)
                    {
                        exposureTimeSupported = false;
                    }
                }

                if (!string.IsNullOrEmpty(frame.ImageInfo))
                {
                    var dict = new Dictionary<string, string>();
                    foreach (string token in frame.ImageInfo.Split(';'))
                    {
                        string[] tokens = token.Split(':');
                        if (tokens.Length == 2) 
                            dict.Add(tokens[0], tokens[1]);
                    }

                    string gpsStatus;
                    if (dict.TryGetValue("GPS", out gpsStatus))
                    {
                        g.DrawString(gpsStatus, s_VtiOsdFont, Brushes.Yellow, 10, 10);

                        string clockFreq;
                        if (dict.TryGetValue("CLKFRQ", out clockFreq))
                            g.DrawString(clockFreq, s_VtiOsdFont, Brushes.Yellow, 50, 10);

                        string longitude, latitude;
                        if (dict.TryGetValue("LONG", out longitude) && dict.TryGetValue("LAT", out latitude))
                        {
                            g.DrawString(string.Format("Long:{0}, Lat:{1}", longitude, latitude), s_VtiOsdFont, Brushes.Yellow, 110, 10);
                        }

                        string ccdTemp;
                        if (dict.TryGetValue("CCDTMP", out ccdTemp))
                            g.DrawString("CCD Temp: " + ccdTemp + "°C", s_VtiOsdFont, Brushes.Yellow, 400, 10);
                    }
                }
            }
        }

        private void PlotStarSpectra(Graphics g, VideoFrameWrapper frame)
        {
            var astroImg = new AstroImage((int[,])frame.ImageArray, imageWidth, imageHeight, (uint)(frame.IntegrationRate.HasValue ? frame.IntegrationRate.Value * 255 : 255));
            var reader = new SpectraReader(astroImg, TrackingContext.Current.SpectraAngleDeg);
            Spectra spectra = reader.ReadSpectra(TrackingContext.Current.GuidingStar.X, TrackingContext.Current.GuidingStar.Y, (int)TrackingContext.Current.GuidingStar.FWHM);

			StackSpectra(spectra);

	        m_StackedSpectra.MaxPixelValue = (uint)m_StackedSpectra.Points.Max(x => x.ProcessedValue) - 1;
			float xCoeff = imageWidth * 1.0f / (2 * m_StackedSpectra.Points.Count);

	        int firstPixelToDraw = m_StackedSpectra.ZeroOrderPixelNo + m_StackedSpectra.PixelWidth * 3;
			float yCoeff = (imageWidth / 2 - 30) * 1.0f / m_StackedSpectra.Points.Where(x => x.PixelNo > firstPixelToDraw).Max(x => x.ProcessedValue);
			float colorCoeff = 256.0f / m_StackedSpectra.MaxPixelValue;

			List<SpectraPoint> points = new List<SpectraPoint>(m_StackedSpectra.Points);
            int firstPixelNo = points[0].PixelNo;

            g.FillRectangle(Brushes.Black, 0, imageHeight - 30, imageWidth / 2, imageHeight);

	        PointF? prevPoint = null;

            foreach (SpectraPoint point in points)
            {
				float x = xCoeff* (point.PixelNo - firstPixelNo);
                Pen pen;

                if (point.HasSaturatedPixels)
                {
                    pen = Pens.Red;
                }
                else
                {
					byte clr = (byte)(Math.Round(point.ProcessedValue * colorCoeff));
                    pen = m_GreyPens[clr];
                }
                g.DrawLine(pen, x, imageHeight - 30, x, imageHeight);

				if (!float.IsInfinity(yCoeff) && !float.IsNaN(yCoeff))
				{
					if (point.PixelNo > firstPixelToDraw)
					{
						PointF thisPoint = new PointF(x, imageHeight - 30 - yCoeff * point.ProcessedValue);

						if (prevPoint.HasValue)
							g.DrawLine(Pens.Aqua, prevPoint.Value, thisPoint);

						prevPoint = thisPoint;						
					}
				}
            }

            int percBuff = (int)Math.Min(100, Math.Ceiling(100f * m_StackedSpectraList.Count / Settings.Default.SpectraFrameStack));
            g.DrawString(string.Format("{0} %", percBuff), s_VtiOsdFont, Brushes.Aqua, 10 + imageWidth / 2.0f, imageHeight - 15);
        }

	    private List<Spectra> m_StackedSpectraList = new List<Spectra>();
	    private Spectra m_StackedSpectra = new Spectra();

		private void StackSpectra(Spectra spectra)
		{
			while (m_StackedSpectraList.Count > Math.Max(1, Settings.Default.SpectraFrameStack - 1)) m_StackedSpectraList.RemoveAt(0);
			m_StackedSpectraList.Add(spectra);

			m_StackedSpectra.Points.Clear();
			if (m_StackedSpectraList.Count > 0)
			{
				m_StackedSpectra.Points.AddRange(m_StackedSpectraList[0].Points);
				m_StackedSpectra.ZeroOrderPixelNo = m_StackedSpectraList[0].ZeroOrderPixelNo;
				m_StackedSpectra.MaxPixelValue = m_StackedSpectraList[0].MaxPixelValue;
				m_StackedSpectra.PixelWidth = m_StackedSpectraList[0].PixelWidth;
				m_StackedSpectra.MaxSpectraValue = m_StackedSpectraList[0].MaxSpectraValue;
			}

			var originalMasterPoints = new List<SpectraPoint>();
			originalMasterPoints.AddRange(m_StackedSpectra.Points);

			var valueLists = new List<float>[m_StackedSpectra.Points.Count];
			for (int j = 0; j < m_StackedSpectra.Points.Count; j++) valueLists[j] = new List<float>();

			var signalLists = new List<float>[m_StackedSpectra.Points.Count];
			for (int j = 0; j < m_StackedSpectra.Points.Count; j++) signalLists[j] = new List<float>();

            var saturationFlagLists = new bool[m_StackedSpectra.Points.Count];
            for (int j = 0; j < m_StackedSpectra.Points.Count; j++) saturationFlagLists[j] = false;

			for (int i = 1; i < m_StackedSpectraList.Count; i++)
			{
				Spectra nextSpectra = m_StackedSpectraList[i];
				
				int nextSpectraFirstPixelNo = nextSpectra.Points[0].PixelNo;
				int deltaIndex = nextSpectra.ZeroOrderPixelNo - m_StackedSpectra.ZeroOrderPixelNo + m_StackedSpectra.Points[0].PixelNo - nextSpectraFirstPixelNo;

				int bestOffset = 0;

				float bestOffsetValue = float.MaxValue;

				for (int probeOffset = -2; probeOffset <= 2; probeOffset++)
				{
					float currOffsetValue = 0;
					for (int j = 0; j < m_StackedSpectra.Points.Count; j++)
					{
						int indexNextSpectra = deltaIndex + j + probeOffset;
						if (indexNextSpectra >= 0 && indexNextSpectra < nextSpectra.Points.Count)
						{
							currOffsetValue += Math.Abs(originalMasterPoints[j].RawValue - nextSpectra.Points[indexNextSpectra].RawValue);
						}
					}

					if (currOffsetValue < bestOffsetValue)
					{
						bestOffsetValue = currOffsetValue;
						bestOffset = probeOffset;
					}
				}

				for (int j = 0; j < m_StackedSpectra.Points.Count; j++)
				{
					int indexNextSpectra = deltaIndex + j + bestOffset;
					if (indexNextSpectra >= 0 && indexNextSpectra < nextSpectra.Points.Count)
					{
						valueLists[j].Add(nextSpectra.Points[indexNextSpectra].RawValue);
						signalLists[j].Add(nextSpectra.Points[indexNextSpectra].RawValue);
					    if (nextSpectra.Points[indexNextSpectra].HasSaturatedPixels) saturationFlagLists[j] = true;
					}
				}
			}

			for (int i = 0; i < m_StackedSpectra.Points.Count; i++)
			{
				valueLists[i].Sort();
				signalLists[i].Sort();

				m_StackedSpectra.Points[i].RawValue = valueLists[i].Count == 0 ? 0 : valueLists[i][valueLists[i].Count / 2];
				m_StackedSpectra.Points[i].RawSignal = signalLists[i].Count == 0 ? 0 : signalLists[i][signalLists[i].Count / 2];
				m_StackedSpectra.Points[i].RawSignalPixelCount = signalLists[i].Count;
			    m_StackedSpectra.Points[i].HasSaturatedPixels = saturationFlagLists[i];
			}

			ApplyGaussianBlur(Settings.Default.SpectraGaussFWHM);
		}

		private static float FWHM_COEFF = (float)(4 * Math.Log(2));

		private float GetGaussianValue(float fwhm, float distance)
		{
			// FWHM * FWHM = 4 * (2 ln(2)) * c * c => 2*c*c = FWHM*FWHM / (4 * ln(2))
			return (float)Math.Exp(-FWHM_COEFF * distance * distance / (fwhm * fwhm));
		}

		public void ApplyGaussianBlur(float fwhm)
		{
			if (Math.Abs(fwhm) < 0.0001 || float.IsNaN(fwhm))
			{
				foreach (SpectraPoint point in m_StackedSpectra.Points)
					point.ProcessedValue = point.RawValue;
			}
			else
			{
				float[] kernel = new float[21];
				for (int i = 0; i < 10; i++)
				{
					kernel[10 + i] = kernel[10 - i] = GetGaussianValue(fwhm, i);
				}

				for (int i = 0; i < m_StackedSpectra.Points.Count; i++)
				{
					SpectraPoint point = m_StackedSpectra.Points[i];
					float sum = 0;
					float weight = 0;
					for (int j = -10; j <= 10; j++)
					{
						if (j + i > 0 && j + i < m_StackedSpectra.Points.Count - 1)
						{
							weight += kernel[j + 10];
							sum += kernel[j + 10] * m_StackedSpectra.Points[j + i].RawValue;
						}
					}

					if (weight > 0)
						point.ProcessedValue = sum / weight;
					else
						point.ProcessedValue = point.RawValue;
				}
			}
		}

	    private static Font s_VtiOsdFont = new Font(FontFamily.GenericSansSerif, 7.5f, FontStyle.Regular);

        private void PrintCurrentErrorMessage(Graphics g)
        {
            SizeF msgMeasurement = g.MeasureString(currErrorMessageToDisplay, overlayMessagesFont);

            g.FillRectangle(Brushes.DarkSlateGray, imageWidth - msgMeasurement.Width - 9, 3, msgMeasurement.Width + 6, msgMeasurement.Height + 6);
            g.DrawString(currErrorMessageToDisplay, overlayMessagesFont, Brushes.OrangeRed, imageWidth - msgMeasurement.Width - 6, 6);
        }

        private void PrintCurrentInfoMessage(Graphics g)
        {
            SizeF msgMeasurement = g.MeasureString(currInfoMessageToDisplay, overlayMessagesFont);

            g.FillRectangle(Brushes.DarkSlateGray, imageWidth - msgMeasurement.Width - 9, msgMeasurement.Height + 9, msgMeasurement.Width + 6, msgMeasurement.Height + 6);
            g.DrawString(currInfoMessageToDisplay, overlayMessagesFont, Brushes.Lime, imageWidth - msgMeasurement.Width - 6, msgMeasurement.Height + 12);
        }

        private void ProcessErrorMessages(Graphics g)
        {
            if (displayErrorMessageUntil != DateTime.MinValue && currErrorMessageToDisplay != null)
            {
                if (DateTime.Now.Ticks > displayErrorMessageUntil.Ticks)
                {
                    displayErrorMessageUntil = DateTime.MinValue;
                    currErrorMessageToDisplay = null;
                }
                else
                    PrintCurrentErrorMessage(g);
            }
            else
            {
                lock (syncRoot)
                {
                    if (errorMessagesQueue.Count > 0)
                    {
                        currErrorMessageToDisplay = errorMessagesQueue.Dequeue();
                        displayErrorMessageUntil = DateTime.Now.AddSeconds(10);
                        PrintCurrentErrorMessage(g);
                    }
                    else if (infoMessagesQueue.Count > 0)
                    {
                        currInfoMessageToDisplay = infoMessagesQueue.Dequeue();
                        displayInfoMessageUntil = DateTime.Now.AddSeconds(10);
                        PrintCurrentInfoMessage(g);
                    }
                    
                }
            }
        }

        private void ProcessInfoMessages(Graphics g)
        {
            if (displayInfoMessageUntil != DateTime.MinValue && currInfoMessageToDisplay != null)
            {
                if (DateTime.Now.Ticks > displayInfoMessageUntil.Ticks)
                {
                    displayInfoMessageUntil = DateTime.MinValue;
                    currInfoMessageToDisplay = null;
                }
                else
                    PrintCurrentInfoMessage(g);
            }
            else
            {
                lock (syncRoot)
                {
                    if (infoMessagesQueue.Count > 0)
                    {
                        currInfoMessageToDisplay = infoMessagesQueue.Dequeue();
                        displayInfoMessageUntil = DateTime.Now.AddSeconds(10);
                        PrintCurrentInfoMessage(g);
                    }
                }
            }
        }        

		public void MouseMove(MouseEventArgs e)
		{
			if (overlayState != null)
				overlayState.MouseMove(e);
		}

		public void MouseLeave(EventArgs e)
		{
			if (overlayState != null)
				overlayState.MouseLeave(e);
		}

		public void MouseDown(MouseEventArgs e)
		{
			if (overlayState != null)
				overlayState.MouseDown(e);
		}

		public void MouseUp(MouseEventArgs e)
		{
			if (overlayState != null)
				overlayState.MouseUp(e);
		}
    }
}
