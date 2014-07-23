using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OccuRec.StateManagement
{
	public class ExternallyManagedCameraState : CameraState
    {
		public static ExternallyManagedCameraState Instance = new ExternallyManagedCameraState();

		private ExternallyManagedCameraState()
        { }

        public override void ProcessFrame(CameraStateManager stateManager, Helpers.VideoFrameWrapper frame)
        {
            // Nothing to do
        }
    }
}
