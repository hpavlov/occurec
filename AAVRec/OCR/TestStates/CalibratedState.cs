using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AAVRec.OCR.TestStates
{
    public class CalibratedState : StateBase
    {
        public static CalibratedState Instance = new CalibratedState();

        private OsdFrameInfo lastTimeStamp;

        internal override void Reset(StateContext context)
        {
            lastTimeStamp = context.LastTimeStamp;
        }

        internal override void TestTimeStamp(StateContext context, OsdFrameInfo frameTimestamp)
        {
            if (lastTimeStamp.SecondField.FieldNumber + 1 == frameTimestamp.FirstField.FieldNumber &&
                Math.Abs(new TimeSpan(frameTimestamp.FirstField.TimeStamp.Ticks - lastTimeStamp.SecondField.TimeStamp.Ticks).TotalMilliseconds - 20) <= 1)
            {
                // All good 
                lastTimeStamp = frameTimestamp;
                return;
            }

            // Find, record and try to solve problems
            if (lastTimeStamp.SecondField.FieldNumber + 1 == frameTimestamp.FirstField.FieldNumber)
            {
                // Problem with the timestamp
            }
            else
            {
                // Problem with the frame number
            }
        }
    }
}
