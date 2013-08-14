using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OccuRec.StateManagement
{
    public class IotaVtiOcrTestingState : CameraState
    {
        public static IotaVtiOcrTestingState Instance = new IotaVtiOcrTestingState();

        private IotaVtiOcrTestingState()
        { }
    }
}
