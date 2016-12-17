/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using OccuRec.Drivers;
using OccuRec.Utilities.Exceptions;

namespace OccuRec.Helpers
{
    public class VideoFrameWrapper
    {
        private IVideoFrame videoFrame;

        public long UniqueFrameId { get; private set; }
		public long IntegratedFrameNo { get; private set; }
		public int? IntegrationRate { get; private set; }		
        public long? StartExposureFrameNo { get; private set; }
        public long? EndExposureFrameNo { get; private set; }
        public float? CutOffRatio { get; private set; }
        public int? PerformedAction { get; private set; }
        public float? PerformedActionProgress { get; private set; }
		public int? DroppedFramesSinceLocked { get; private set; }
		public int? OcrErrorsSinceReset { get; private set; }
		public int ManualIntegrationRateHint { get; private set; }
		
        public VideoFrameWrapper(IVideoFrame videoFrame)
        {
            this.videoFrame = videoFrame;
            
            UniqueFrameId = -1;
	        ManualIntegrationRateHint = 0;

            if (!string.IsNullOrEmpty(videoFrame.ImageInfo))
            {
                string[] tokens = videoFrame.ImageInfo.Split(';');

                foreach (string token in tokens)
                {
                    string[] nvpair = token.Split(':');
                    if (nvpair.Length == 2)
                    {
                        if (nvpair[0] == "INT")
                            IntegrationRate = int.Parse(nvpair[1]);

                        if (nvpair[0] == "CTOF")
                            CutOffRatio = float.Parse(nvpair[1]);

                        if (nvpair[0] == "UFID")
                            UniqueFrameId = long.Parse(nvpair[1]);

                        if (nvpair[0] == "SFID")
                            StartExposureFrameNo = long.Parse(nvpair[1]);

                        if (nvpair[0] == "EFID")
                            EndExposureFrameNo = long.Parse(nvpair[1]);

						if (nvpair[0] == "IFID")
                            IntegratedFrameNo = long.Parse(nvpair[1]);

						if (nvpair[0] == "DRPD")
                            DroppedFramesSinceLocked = int.Parse(nvpair[1]);

                        if (nvpair[0] == "ACT")
                            PerformedAction = int.Parse(nvpair[1]);

                        if (nvpair[0] == "ACT%")
                            PerformedActionProgress = float.Parse(nvpair[1]);

						if (nvpair[0] == "ORER")
							OcrErrorsSinceReset = int.Parse(nvpair[1]);

						if (nvpair[0] == "USRI")
							ManualIntegrationRateHint = int.Parse(nvpair[1]);
                    }
                }
            }

            if (UniqueFrameId == -1)
                UniqueFrameId = videoFrame.FrameNumber;
        }

        public long FrameNumber
        {
            get { return videoFrame.FrameNumber; }
        }

        public double ExposureDuration
        {
            get { return videoFrame.ExposureDuration; }
        }

        private static bool m_TimingUnsupported = false;

        public string ExposureStartTime
        {
            get
            {
                if (m_TimingUnsupported) return null;

                try
                {
                    return videoFrame.ExposureStartTime;
                }
                catch (PropertyNotImplementedException)
                {
                    m_TimingUnsupported = true;
                    return null;
                }
                
            }
        }

        public string ImageInfo
        {
            get { return videoFrame.ImageInfo; }
        }

        private object imageArray = null;

        public object ImageArray
        {
            get { return imageArray ?? videoFrame.ImageArray; }
            set { imageArray = value; }
        }

		public Bitmap PreviewBitmap
		{
			get { return videoFrame.PreviewBitmap; }
		}
    }
}
