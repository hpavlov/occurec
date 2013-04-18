using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AAVRec.OCR.TestStates
{
    public enum TestFrameResult
    {
        Undefined = 0,
        Okay = 1,
        ErrorSaveScreenShotImages = 2
    }

    public class StateContext
    {
        private StateBase currentState;

        public OsdFrameInfo LastTimeStamp;

        public StateContext()
        {
            Reset();
        }

        public void Reset()
        {
            currentState = UncalibratedState.Instance;
            currentState.Reset(this);

            LastTimeStamp = null;
        }

        public void TransitionToState(StateBase newState)
        {
            currentState = newState;
            newState.Reset(this);
        }

        public TestFrameResult TestTimeStamp(OsdFrameInfo frameTimestamp)
        {
            currentState.TestTimeStamp(this, frameTimestamp);

            if (currentState is CalibratedState)
            {
                if (!frameTimestamp.FrameInfoIsOk())
                    return TestFrameResult.ErrorSaveScreenShotImages;
            }
            else
                return TestFrameResult.Undefined;

            return TestFrameResult.Okay;
        }
    }
}
