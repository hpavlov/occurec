using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AAVRec.StateManagement
{
    public class LockedIntegrationCameraState : CameraState
    {
        public static LockedIntegrationCameraState Instance = new LockedIntegrationCameraState();

	    private long lastIntegratedFrameId = -1;
	    private int lockedIntegrationRate = 0;

        private LockedIntegrationCameraState()
        { }

        public override void InitialiseState()
        {
			lockedIntegrationRate = lastIntegratedFrameIntegration;
	        numberOfDroppedFrames = 0;

	        // We don't call the base class in order not to stuff up the stats			
        }

		public override void ProcessFrame(CameraStateManager stateManager, Helpers.VideoFrameWrapper frame)
		{
			base.ProcessFrame(stateManager, frame);

			if (frame.IntegrationRate.HasValue && frame.IntegrationRate.Value > 1)
			{
				if (lastIntegratedFrameId != -1)
				{
					if (lastIntegratedFrameId + 1 == frame.IntegratedFrameNo)
					{
						int droppedFrames = lockedIntegrationRate - frame.IntegrationRate.Value;
						numberOfDroppedFrames += droppedFrames;
					}
				}
				else
				{
					lastIntegratedFrameId = frame.IntegratedFrameNo;
				}
			}			
		}
    }
}
