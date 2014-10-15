/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace OccuRec.OCR
{
    public class OsdFieldInfo
    {
        public DateTime TimeStamp = DateTime.MinValue;
        public long FieldNumber = -1;
        public int NumSatellites = -1;
        public string GpsFixStyatus = string.Empty;
        public bool GpsAlmanacOk = false;
    }

    public class OsdFrameInfo
    {
        public OsdFieldInfo FirstField;
        public OsdFieldInfo SecondField;

        public DateTime StartTime = DateTime.MinValue;
        public DateTime EndTime = DateTime.MaxValue;
        public long FrameNumber = -1;
        public int NumSatellites = -1;
        public string GpsFixStyatus = string.Empty;
        public bool GpsAlmanacOk = false;

        private bool bothFieldNumbersAndTimeStampsAreBad = false;

        public OsdFrameInfo(OsdFieldInfo oddField, OsdFieldInfo evenField)
        {
            ReCalculate(oddField, evenField);
        }

        public void ReCalculate()
        {
            ReCalculate(FirstField, SecondField);
        }

        private void ReCalculate(OsdFieldInfo field1, OsdFieldInfo field2)
        {
            bothFieldNumbersAndTimeStampsAreBad = false;

            if (Math.Abs(field1.FieldNumber - field2.FieldNumber) == 1)
                FrameNumber = Math.Min(field1.FieldNumber, field2.FieldNumber);

            GpsFixStyatus = field1.GpsFixStyatus;
            GpsAlmanacOk = field1.GpsAlmanacOk && field2.GpsAlmanacOk;
            NumSatellites = field1.NumSatellites;

            if (Math.Abs(field1.FieldNumber - field2.FieldNumber) == 1)
            {
                // Field numbers of the consequtive frames are OK. Use them to determine which frame is first and which is second
                if (field1.FieldNumber < field2.FieldNumber)
                {
                    StartTime = field1.TimeStamp;
                    EndTime = field2.TimeStamp;

                    FirstField = field1;
                    SecondField = field2;
                }
                else
                {
                    StartTime = field2.TimeStamp;
                    EndTime = field1.TimeStamp;

                    FirstField = field2;
                    SecondField = field1;
                }                
            }
            else if (Math.Abs(new TimeSpan(field1.TimeStamp.Ticks - field2.TimeStamp.Ticks).TotalMilliseconds) - 20 <= 1)
            {
                // Timestamps are OK. Use them to determine which frame is first and which is second
                if (field1.TimeStamp.Ticks < field2.TimeStamp.Ticks)
                {
                    StartTime = field1.TimeStamp;
                    EndTime = field2.TimeStamp;

                    FirstField = field1;
                    SecondField = field2;                    
                }
                else
                {
                    StartTime = field2.TimeStamp;
                    EndTime = field1.TimeStamp;

                    FirstField = field2;
                    SecondField = field1;
                }
            }
            else
            {
                bothFieldNumbersAndTimeStampsAreBad = true;
                FirstField = field1;
                SecondField = field2; 
            }
        }

        public bool FrameInfoIsOk()
        {
            if (bothFieldNumbersAndTimeStampsAreBad) return false;

            if (FrameNumber == -1) return false;
			if (GpsFixStyatus != "P" && GpsFixStyatus != "G" && GpsFixStyatus != "N") return false;
            if (NumSatellites == -1) return false;

			if (NumSatellites == 0 && GpsFixStyatus != "N") return false;
			if (NumSatellites > 0 && GpsFixStyatus == "N") return false;

            var exposure = new TimeSpan(EndTime.Ticks - StartTime.Ticks);
            if (exposure.TotalMinutes <= 0) return false;
            if (Math.Abs(exposure.TotalMilliseconds - 20) > 1) return false;

            if (FirstField.FieldNumber + 1 != SecondField.FieldNumber) return false;

            return true;
        }

        public string ToDisplayString()
        {
            return string.Format("{0}{1}{2} {3} {4} - {5} {6}", 
                FirstField.NumSatellites,
                FirstField.GpsFixStyatus,
                FirstField.GpsAlmanacOk ? " " : "W",
                FirstField.TimeStamp.ToString("HH:mm:ss.ffff"),
                FirstField.FieldNumber,
                SecondField.TimeStamp.ToString("HH:mm:ss.ffff"),
                FirstField.FieldNumber);
        }
    }


    public static class OsdFieldInfoExtractor
    {
        public static OsdFieldInfo ExtractFieldInfo(List<OcredChar> ocredChars)
        {
            var rv = new OsdFieldInfo();

            var output = new StringBuilder();
            foreach (OcredChar ocredChar in ocredChars)
            {
	            if (ocredChar.RecognizedChar != '\0')
		            output.Append(ocredChar.RecognizedChar);
	            else
	            {
					if (ocredChar.CharId == 0) ocredChar.FailedToRecognizeCorrectly = true;
		            if (ocredChar.CharId == 1) ocredChar.FailedToRecognizeCorrectly = true;
					if (ocredChar.CharId >= 3 && ocredChar.CharId <= 8) ocredChar.FailedToRecognizeCorrectly = true;
					if (ocredChar.CharId == 17) ocredChar.FailedToRecognizeCorrectly = true;

		            output.Append(" ");
	            }
            }

            string charsOnly = output.ToString();

            rv.GpsFixStyatus = charsOnly[0] + "";
            if (!int.TryParse(charsOnly[1] + "", out rv.NumSatellites))
	            ocredChars[1].FailedToRecognizeCorrectly = true;
            else
            {
	            if (rv.GpsFixStyatus == "N" && rv.NumSatellites != 0)
					ocredChars[1].FailedToRecognizeCorrectly = true;
				else if (rv.GpsFixStyatus == "G" && (rv.NumSatellites < 1 || rv.NumSatellites > 3))
					ocredChars[1].FailedToRecognizeCorrectly = true;
				else if (rv.GpsFixStyatus == "P" && rv.NumSatellites < 4)
					ocredChars[1].FailedToRecognizeCorrectly = true;
            }
            
            int hh = 0;
            int.TryParse(charsOnly.Substring(3, 2).Trim(), out hh);

            int mm = 0;
            int.TryParse(charsOnly.Substring(5, 2).Trim(), out mm);

            int ss = 0;
            int.TryParse(charsOnly.Substring(7, 2).Trim(), out ss);
            
            int ms1 = 0;
            if (!int.TryParse(charsOnly.Substring(9, 4).Trim(), out ms1))
            {
	            string strToParse = charsOnly.Substring(9, 4);
				if (strToParse.Trim().Length > 0)
	            {
					for (int i = 0; i < strToParse.Length; i++)
					{
						if (strToParse[i] == ' ')
							ocredChars[9 + i].FailedToRecognizeCorrectly = true;
					}
	            }
            }			

            int ms2 = 0;
            if (!int.TryParse(charsOnly.Substring(13, 4).Trim(), out ms2))
            {
				string strToParse = charsOnly.Substring(9, 4);
				if (strToParse.Trim().Length > 0)
				{
					for (int i = 0; i < strToParse.Length; i++)
					{
						if (strToParse[i] == ' ')
							ocredChars[13 + i].FailedToRecognizeCorrectly = true;
					}
				}	            
            }

	        string fieldNoStr = charsOnly.Substring(17).TrimEnd();
	        if (string.IsNullOrEmpty(fieldNoStr))
		        rv.FieldNumber = -1;
	        else
	        {
		        if (!long.TryParse(fieldNoStr, out rv.FieldNumber))
		        {

			        for (int i = 0; i < fieldNoStr.Length; i++)
			        {
				        if (fieldNoStr[i] == ' ')
					        ocredChars[17 + i].FailedToRecognizeCorrectly = true;
			        }			        
		        }
	        }

	        Trace.WriteLine(string.Format("{0}{1}{2}|{3}{4}:{5}{6}:{7}{8} {9}{10}{11}{12}{13}{14}{15}{16}|{17}{18}{19}{20}{21}{22}",
                charsOnly[0], charsOnly[1], charsOnly[2],
				charsOnly[3], charsOnly[4],
				charsOnly[5], charsOnly[6],
				charsOnly[7], charsOnly[8],
				charsOnly[9], charsOnly[10],charsOnly[11], charsOnly[12],
				charsOnly[13], charsOnly[14],charsOnly[15], charsOnly[16],
				charsOnly[17], charsOnly[18], charsOnly[19], charsOnly[20], charsOnly[21], charsOnly[22]));

			rv.TimeStamp = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, hh, mm, ss);

			if (ms1 != 0)
				rv.TimeStamp = rv.TimeStamp.AddMilliseconds(ms1 / 10);
			else
				rv.TimeStamp = rv.TimeStamp.AddMilliseconds(ms2 / 10);

            return rv;
        }
    }
}
