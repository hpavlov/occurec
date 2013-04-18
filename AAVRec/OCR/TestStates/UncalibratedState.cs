using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AAVRec.OCR.TestStates
{
    public class UncalibratedState : StateBase
    {
        public static UncalibratedState Instance = new UncalibratedState();

        private OsdFrameInfo lastGoodTimeStamp;
        private OsdFrameInfo secondLastGoodTimeStamp;


        internal override void TestTimeStamp(StateContext context, OsdFrameInfo frameTimestamp)
        {
            if (lastGoodTimeStamp == null)
            {
                if (frameTimestamp.FrameInfoIsOk())
                    lastGoodTimeStamp = frameTimestamp;
            }
            else 
            {
                if (!frameTimestamp.FrameInfoIsOk() ||
                    lastGoodTimeStamp.SecondField.FieldNumber + 1 != frameTimestamp.FirstField.FieldNumber ||
                    Math.Abs(new TimeSpan(frameTimestamp.FirstField.TimeStamp.Ticks - lastGoodTimeStamp.SecondField.TimeStamp.Ticks).TotalMilliseconds - 20) > 1)
                {
                    lastGoodTimeStamp = null;    
                }
                else
                {
                    secondLastGoodTimeStamp = frameTimestamp;

                    context.LastTimeStamp = secondLastGoodTimeStamp;
                    context.TransitionToState(CalibratedState.Instance);
                }                
            }
        }

        internal override void Reset(StateContext context)
        {
            lastGoodTimeStamp = null;
            secondLastGoodTimeStamp = null;
        }
    }
}
