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
	    private long lastReadFrameNumber;
	    protected int lastIntegratedFrameIntegration;
		protected int numberOfDroppedFrames = 0;

	    protected long readFrameNo = 0;

        private long numberConsequtiveSameIntegrationIntegratedFrames;

        public virtual void InitialiseState(CameraStateManager stateManager)
        {
            ResetIntegrationStats();
        }

        protected void ResetIntegrationStats()
        {
            lastIntegratedFrameNumber = -1;
			lastReadFrameNumber = -1;
            lastIntegratedFrameIntegration = -1;
            numberConsequtiveSameIntegrationIntegratedFrames = 0;
	        readFrameNo = -1;
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
				readFrameNo++;

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
	                if (lastReadFrameNumber == readFrameNo - 1)
		                numberConsequtiveSameIntegrationIntegratedFrames++;
	                else
		                numberConsequtiveSameIntegrationIntegratedFrames = 0;
                }
                else
                {
                    numberConsequtiveSameIntegrationIntegratedFrames = 0;
                    if (frame.IntegrationRate != null)
                        lastIntegratedFrameIntegration = frame.IntegrationRate.Value;
                }

				lastReadFrameNumber = readFrameNo;
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
