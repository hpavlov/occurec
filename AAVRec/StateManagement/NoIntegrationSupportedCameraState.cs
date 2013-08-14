using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OccuRec.StateManagement
{
    public class NoIntegrationSupportedCameraState : CameraState
    {
        public static NoIntegrationSupportedCameraState Instance = new NoIntegrationSupportedCameraState();

        private NoIntegrationSupportedCameraState()
        { }

        public override void ProcessFrame(CameraStateManager stateManager, Helpers.VideoFrameWrapper frame)
        {
            // Nothing to do
        }
    }
}
