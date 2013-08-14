using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OccuRec.StateManagement
{
    public class UndeterminedIntegrationCameraState : CameraState, IKnowsIntegrationRate
    {
        public static UndeterminedIntegrationCameraState Instance = new UndeterminedIntegrationCameraState();

        private UndeterminedIntegrationCameraState()
        { }
    }
}
