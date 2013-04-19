using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AAVRec.StateManagement
{
    public class IotaVtiOcrTestingState : CameraState
    {
        public static IotaVtiOcrTestingState Instance = new IotaVtiOcrTestingState();

        private IotaVtiOcrTestingState()
        { }
    }
}
