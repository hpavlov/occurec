using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using AAVRec.Helpers;

namespace AAVRec.StateManagement
{
    public interface IKnowsIntegrationRate
    {
        int CurrentIntegrationRate { get; }
    }

    public abstract class CameraState
    {
        private long lastIntegratedFrameNumber;
        private int lastIntegratedFrameIntegration;
        private long numberConsequtiveSameIntegrationIntegratedFrames;

        public virtual void InitialiseState()
        {
            ResetIntegrationStats();
        }

        protected void ResetIntegrationStats()
        {
            lastIntegratedFrameNumber = -1;
            lastIntegratedFrameIntegration = -1;
            numberConsequtiveSameIntegrationIntegratedFrames = 0;            
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
                    numberConsequtiveSameIntegrationIntegratedFrames++;
                }
                else
                {
                    numberConsequtiveSameIntegrationIntegratedFrames = 0;
                    if (frame.IntegrationRate != null)
                        lastIntegratedFrameIntegration = frame.IntegrationRate.Value;
                }

                lastIntegratedFrameNumber = frame.FrameNumber;
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
