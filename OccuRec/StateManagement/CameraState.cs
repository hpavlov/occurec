/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

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
			// NOTE: Because frames may be skipped as it could take too long to process them in OccuRec
			//       and not all frames will be checked by this code, it is possible that integration consistency detection
			//       implemented here will not work. The case of locking at x1 has been explicitely hacked to alway work regardless of this limitation

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
					if (lastIntegratedFrameNumber == frame.IntegratedFrameNo - 1 || lastIntegratedFrameIntegration == 1)
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
