using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace AAVRec.Helpers
{
    public class OverlayManager
    {
        private Queue<string> errorMessagesQueue = new Queue<string>();
        private string currentOcrStamp = null;

        private string currMessageToDisplay = null;
        private DateTime displayMessageUntil = DateTime.MinValue;

        private static Font overlayMessagesFont = new Font(FontFamily.GenericMonospace, 10);

        private int imageWidth;
        private int imageHeight;
	    private int framesWithoutTimestams;

        private object syncRoot = new object();

        public OverlayManager(int width, int height, List<string> initializationErrorMessages)
        {
            imageWidth = width;
            imageHeight = height;
            currentOcrStamp = null;

            foreach (string message in initializationErrorMessages)
                errorMessagesQueue.Enqueue(message);
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

        public void Finalise()
        {
            lock (syncRoot)
            {
                errorMessagesQueue.Clear();

                displayMessageUntil = DateTime.MinValue;
                currMessageToDisplay = null;
            }
        }

	    private SizeF timestampMeasurement = SizeF.Empty;

        public void ProcessFrame(Graphics g)
        {
            ProcessErrorMessages(g);

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

        private void PrintCurrentErrorMessage(Graphics g)
        {
            SizeF msgMeasurement = g.MeasureString(currMessageToDisplay, overlayMessagesFont);

            g.FillRectangle(Brushes.DarkSlateGray, imageWidth - msgMeasurement.Width - 9, 3, msgMeasurement.Width + 6, msgMeasurement.Height + 6);
            g.DrawString(currMessageToDisplay, overlayMessagesFont, Brushes.OrangeRed, imageWidth - msgMeasurement.Width - 6, 6);
        }

        private void ProcessErrorMessages(Graphics g)
        {
            if (displayMessageUntil != DateTime.MinValue && currMessageToDisplay != null)
            {
                if (DateTime.Now.Ticks > displayMessageUntil.Ticks)
                {
                    displayMessageUntil = DateTime.MinValue;
                    currMessageToDisplay = null;
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
                        currMessageToDisplay = errorMessagesQueue.Dequeue();
                        displayMessageUntil = DateTime.Now.AddSeconds(10);
                        PrintCurrentErrorMessage(g);
                    }                    
                }
            }
        }
    }
}
