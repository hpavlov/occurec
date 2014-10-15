/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OccuRec.StateManagement
{
	public class LockedIntegrationCameraState : CameraState, IKnowsIntegrationRate
    {
        public static LockedIntegrationCameraState Instance = new LockedIntegrationCameraState();

	    private long lastIntegratedFrameId = -1;
	    private int lockedIntegrationRate = 0;

        private LockedIntegrationCameraState()
        { }

        public override void InitialiseState(CameraStateManager stateManager)
        {
			lockedIntegrationRate = lastIntegratedFrameIntegration;
	        numberOfDroppedFrames = 0;

	        // We don't call the base class in order not to stuff up the stats			
        }

		public override void ProcessFrame(CameraStateManager stateManager, Helpers.VideoFrameWrapper frame)
		{
			if (frame.IntegrationRate.HasValue && frame.IntegrationRate.Value > 1)
				lastIntegratedFrameIntegration = frame.IntegrationRate.Value;

			if (frame.DroppedFramesSinceLocked.HasValue)
				numberOfDroppedFrames = frame.DroppedFramesSinceLocked.Value;
		}
    }
}
