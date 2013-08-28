using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OccuRec.OCR.TestStates
{
    public class MisstakenCharacterRecord
    {
        public string ExpectedChar;
        public string RecognizedChar;
        public int Position;
    }

    public class CalibratedState : StateBase
    {
        public static CalibratedState Instance = new CalibratedState();
        private OsdFrameInfo lastTimeStamp;
        private List<MisstakenCharacterRecord> correctedChars = new List<MisstakenCharacterRecord>();

        private static long FIELD_LENGTH_IN_TICKS = new TimeSpan(0, 0, 0, 0, 20).Ticks;

        private TestFrameResult suggestedTimeStampTestResult = TestFrameResult.Undefined;

        private void RequestFullTimeStampDump()
        {
            suggestedTimeStampTestResult = TestFrameResult.ErrorSaveScreenShotImages;
        }

        internal override void Reset(StateContext context)
        {
            lastTimeStamp = context.LastTimeStamp;
            correctedChars.Clear();
        }

        internal override TestFrameResult TestTimeStamp(StateContext context, OsdFrameInfo frameTimestamp)
        {
            if (lastTimeStamp.SecondField.FieldNumber + 1 == frameTimestamp.FirstField.FieldNumber &&
                Math.Abs(new TimeSpan(frameTimestamp.FirstField.TimeStamp.Ticks - lastTimeStamp.SecondField.TimeStamp.Ticks).TotalMilliseconds - 20) <= 1)
            {
                // All good 
                lastTimeStamp = frameTimestamp;
                return TestFrameResult.Okay;
            }

            suggestedTimeStampTestResult = TestFrameResult.Undefined;

            // Find, record and try to solve problems
            #region Frame/Field Number Corrections
            long expectedFieldNumber = lastTimeStamp.SecondField.FieldNumber + 1;
            int attempts = 5;
            while (expectedFieldNumber != frameTimestamp.FirstField.FieldNumber && attempts > 0)
            {
                // Problem with first frame number
                long correctedFieldNo = CorrectField(expectedFieldNumber, frameTimestamp.FirstField.FieldNumber);
                frameTimestamp.FirstField.FieldNumber = correctedFieldNo;
                frameTimestamp.FrameNumber = correctedFieldNo;
                attempts--;
            }

            attempts = 5;
            expectedFieldNumber = frameTimestamp.FirstField.FieldNumber + 1;
            while (expectedFieldNumber != frameTimestamp.SecondField.FieldNumber && attempts > 0)
            {
                // Problem with first frame number
                long correctedFieldNo = CorrectField(expectedFieldNumber, frameTimestamp.SecondField.FieldNumber);
                frameTimestamp.SecondField.FieldNumber = correctedFieldNo;
                attempts--;
            }
            #endregion

            #region TimeStamp Corrections
            long expectedTicks = lastTimeStamp.SecondField.TimeStamp.Ticks + FIELD_LENGTH_IN_TICKS;
            while (Math.Abs(new TimeSpan(frameTimestamp.FirstField.TimeStamp.Ticks - expectedTicks).TotalMilliseconds) > 1)
            {
                // Correct the timestamp of the first field
                long correctedTicks = CorrectTimeStamp(expectedTicks, frameTimestamp.FirstField.TimeStamp.Ticks);
                frameTimestamp.FirstField.TimeStamp = new DateTime(correctedTicks);
            }
            #endregion

            frameTimestamp.ReCalculate();
            lastTimeStamp = frameTimestamp;

            return suggestedTimeStampTestResult;
        }

        private long CorrectField(long expectedFieldNumber, long detectedFieldNo)
        {
            int totalDigits = 1 + (int)Math.Log10(expectedFieldNumber);
            int problemWithDigitAtPosition = totalDigits - (int)((expectedFieldNumber - detectedFieldNo) / 10);

            if (problemWithDigitAtPosition <= 0 || problemWithDigitAtPosition > totalDigits)
            {
                // There is undetected digit from the 'detected' timestamp
                RequestFullTimeStampDump();
                return expectedFieldNumber;
            }
            else
            {
                char expectedChar = expectedFieldNumber.ToString()[problemWithDigitAtPosition - 1];
                char ocredChar = detectedFieldNo.ToString()[problemWithDigitAtPosition - 1];

                //var logEntry = new MisstakenCharacterRecord()
                //{
                //    ExpectedChar = expectedChar + "",
                //    RecognizedChar = ocredChar + "",
                //    Position = problemWithDigitAtPosition
                //};

                //correctedChars.Add(logEntry);

                char[] correctedFieldNumCharArray = detectedFieldNo.ToString().ToCharArray();
                correctedFieldNumCharArray[problemWithDigitAtPosition - 1] = expectedChar;
                return long.Parse(new string(correctedFieldNumCharArray));                        
            }
        }

        private long CorrectTimeStamp(long expectedTicks, long detectedTicks)
        {
            string expectedTimeStamp = new DateTime(expectedTicks).ToString("HH:mm:ss ffff");
            string detectedTimeStamp = new DateTime(detectedTicks).ToString("HH:mm:ss ffff");

            var timeSpan = new TimeSpan(expectedTicks - detectedTicks);
            if (Math.Abs(timeSpan.TotalSeconds) < 1)
            {
                // Problem in the fractional seconds
            }
            else if (Math.Abs(timeSpan.TotalSeconds) >= 1 && Math.Abs(timeSpan.TotalSeconds) < 60)
            {
                // Problem in the seconds
                char[] expectedSeconds = expectedTimeStamp.Substring(6, 2).ToCharArray();
                char[] detectedSeconds = detectedTimeStamp.Substring(6, 2).ToCharArray();
            }
            else if (Math.Abs(timeSpan.TotalSeconds) >= 60 && Math.Abs(timeSpan.TotalSeconds) < 3600)
            {
                // Problem in the minutes
            }
            else if (Math.Abs(timeSpan.TotalSeconds) >= 3600 && Math.Abs(timeSpan.TotalSeconds) < 3600 * 24)
            {
                // Problem in the hours
            }
            else
            {
                // The problem must be in the way the DATE part has been added to the timestamps
                // NOTE: This cannot be corrected properly 
                return expectedTicks;
            }

            return expectedTicks;
        }
    }
}
