using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using AAVRec.Drivers;
using AAVRec.Helpers;

namespace AAVRec.StateManagement
{
    public class CameraStateManager
    {
        private static long MIN_CONSEQUTIVE_FRAMES_TO_LOCK_INTEGRATION = 4;

        private CameraState currentState;
        private IVideo driverInstance;

        public void ProcessFrame(VideoFrameWrapper frame)
        {
            if (currentState != null)
                currentState.ProcessFrame(this, frame);
        }

        public void CameraConnected(IVideo driverInstance)
        {
            this.driverInstance = driverInstance;

            if (driverInstance is Drivers.AAVTimer.Video)
                ChangeState(UndeterminedIntegrationCameraState.Instance);
            else
                ChangeState(NoIntegrationSupportedCameraState.Instance);
        }
        public void ChangeState(CameraState newState)
        {
            if (currentState != null)
            {
                currentState.FinaliseState();
                currentState = null;
            }

            Trace.WriteLine(string.Format("CameraState: Changing to state {0}", newState != null ? newState.GetType().ToString() : "NULL"));

            currentState = newState;

            if (currentState != null)
                currentState.InitialiseState();
        }

        public void CameraDisconnected()
        {
            currentState = null;
            driverInstance = null;
        }

        public bool LockIntegration()
        {
            string result = driverInstance.Action("LockIntegration", null);

            bool boolResult;
            bool.TryParse(result, out boolResult);

            if (boolResult)
            {
                ChangeState(LockedIntegrationCameraState.Instance);

                return true;
            }
            else
                return false;
        }

        public bool UnlockIntegration()
        {
            string result = driverInstance.Action("UnlockIntegration", null);

            bool boolResult;
            bool.TryParse(result, out boolResult);

            if (boolResult)
            {
                ChangeState(UndeterminedIntegrationCameraState.Instance);

                return true;
            }
            else
                return false;
        }

        public bool IsIntegrationLocked
        {
            get { return currentState is LockedIntegrationCameraState; }
        }

        public bool CanLockIntegrationNow
        {
            get
            {
                return
                    currentState is UndeterminedIntegrationCameraState &&
                    currentState.NumberConsequtiveSameIntegrationIntegratedFrames >= MIN_CONSEQUTIVE_FRAMES_TO_LOCK_INTEGRATION;
            }
        }

        public bool CanStartRecording
        {
            get
            {
                return
                    currentState is LockedIntegrationCameraState ||
                    currentState is NoIntegrationSupportedCameraState;
            }
        }

        public int IntegrationRate
        {
            get
            {
                if (currentState is IKnowsIntegrationRate)
                    return ((IKnowsIntegrationRate)currentState).CurrentIntegrationRate;
                else
                    return 0;
            }
        }

        public bool ToggleIotaVtiOcrTesting()
        {
            if (!IsTestingIotaVtiOcr && currentState is UndeterminedIntegrationCameraState)
            {
                string result = driverInstance.Action("StartIotaVtiOcrTesting", null);

                bool boolResult;
                bool.TryParse(result, out boolResult);

                if (boolResult)
                {
                    ChangeState(IotaVtiOcrTestingState.Instance);

                    return true;
                }
                else
                    return false;
            }
            else if (IsTestingIotaVtiOcr && currentState is IotaVtiOcrTestingState)
            {
                string result = driverInstance.Action("StopIotaVtiOcrTesting", null);

                bool boolResult;
                bool.TryParse(result, out boolResult);

                if (boolResult)
                {
                    ChangeState(UndeterminedIntegrationCameraState.Instance);

                    return true;
                }
                else
                    return false;
            }

            return false;
        }

        public bool IsTestingIotaVtiOcr
        {
            get { return currentState is IotaVtiOcrTestingState; }
        }
    }
}
