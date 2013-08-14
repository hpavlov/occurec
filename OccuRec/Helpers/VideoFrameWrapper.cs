using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OccuRec.Drivers;

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

        public VideoFrameWrapper(IVideoFrame videoFrame)
        {
            this.videoFrame = videoFrame;
            
            UniqueFrameId = -1;

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
                    }
                }
            }

            if (UniqueFrameId == -1)
                UniqueFrameId = videoFrame.FrameNumber;
        }

        public object ImageArray
        {
            get { return videoFrame.ImageArray; }
        }

        public object ImageArrayVariant
        {
            get { return videoFrame.ImageArrayVariant; }
        }

        public long FrameNumber
        {
            get { return videoFrame.FrameNumber; }
        }

        public double ExposureDuration
        {
            get { return videoFrame.ExposureDuration; }
        }

        public string ExposureStartTime
        {
            get { return videoFrame.ExposureStartTime; }
        }

        public string ImageInfo
        {
            get { return videoFrame.ImageInfo; }
        }
    }
}
