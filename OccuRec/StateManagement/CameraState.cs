using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using OccuRec.Helpers;

namespace OccuRec.StateManagement
{
    public interface IKnowsIntegrationRate
    {
        int CurrentIntegrationRate { get; }
    }

    public abstract class CameraState
    {
        private long lastIntegratedFrameNumber;
	    protected int lastIntegratedFrameIntegration;
		protected int numberOfDroppedFrames = 0;

        private long numberConsequtiveSameIntegrationIntegratedFrames;

        public virtual void InitialiseState(CameraStateManager stateManager)
        {
            ResetIntegrationStats();
        }

        protected void ResetIntegrationStats()
        {
            lastIntegratedFrameNumber = -1;
            lastIntegratedFrameIntegration = -1;
            numberConsequtiveSameIntegrationIntegratedFrames = 0;
        }

		internal int GetNumberOfDroppedFrames()
		{
			return numberOfDroppedFrames;
		}

        public virtual void FinaliseState()
        { }

        public virtual void ProcessFrame(CameraStateManager stateManager, Helpers.VideoFrameWrapper frame)
        {
            if (lastIntegratedFrameNumber != frame.FrameNumber)
            {
                if (lastIntegratedFrameIntegration <= 0)
                {
                    if (frame.IntegrationRate != null)
                    {
                        lastIntegratedFrameIntegration = frame.IntegrationRate.Value;
                        numberConsequtiveSameIntegrationIntegratedFrames = 0;                 
                    }
                    else
                    {
                        lastIntegratedFrameIntegration = -1;
                        numberConsequtiveSameIntegrationIntegratedFrames = 0;
                    }
                }
                else if (frame.IntegrationRate != null && lastIntegratedFrameIntegration == frame.IntegrationRate.Value)
                {
					if (lastIntegratedFrameNumber == frame.IntegratedFrameNo - 1)
	                    numberConsequtiveSameIntegrationIntegratedFrames++;
                }
                else
                {
                    numberConsequtiveSameIntegrationIntegratedFrames = 0;
                    if (frame.IntegrationRate != null)
                        lastIntegratedFrameIntegration = frame.IntegrationRate.Value;
                }

                lastIntegratedFrameNumber = frame.IntegratedFrameNo;
            }
        }

        internal long NumberConsequtiveSameIntegrationIntegratedFrames
        {
            get { return numberConsequtiveSameIntegrationIntegratedFrames; }
        }

        public int CurrentIntegrationRate
        {
            get { return lastIntegratedFrameIntegration; }
        }
    }
}
