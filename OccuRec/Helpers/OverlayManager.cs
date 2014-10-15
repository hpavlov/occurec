/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OccuRec.Context;
using OccuRec.FrameAnalysis;
using OccuRec.Properties;
using OccuRec.StateManagement;
using OccuRec.Tracking;

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

	    private OverlayState overlayState;
	    private FrameAnalysisManager analysisManager;
	    private CameraStateManager stateManager;

        private object syncRoot = new object();

		public OverlayManager(int width, int height, List<string> initializationErrorMessages, FrameAnalysisManager analysisManager, CameraStateManager stateManager)
        {
            imageWidth = width;
            imageHeight = height;
            currentOcrStamp = null;
			this.analysisManager = analysisManager;
			this.stateManager = stateManager;

            foreach (string message in initializationErrorMessages)
                errorMessagesQueue.Enqueue(message);
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

        public void ProcessFrame(Graphics g)
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
