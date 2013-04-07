using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AAVRec.StateManagement
{
    public class LockedIntegrationCameraState : CameraState
    {
        public static LockedIntegrationCameraState Instance = new LockedIntegrationCameraState();

        private LockedIntegrationCameraState()
        { }

        public override void InitialiseState()
        {
            // We don't call the base class in order not to stuff up the stats
        }
    }
}
