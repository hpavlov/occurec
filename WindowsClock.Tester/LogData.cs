/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace WindowsClock.Tester
{
	public class LogEntry
	{
		public float WinAccu;
		public float WinAccuNorm;
		public float OccuRecAccu;
		public float OccuRecErr;
		public float TimeDrift;
		public float TimeDriftErr;
		public float NTPAccu;
		public bool NTPTimeUpdate;
	}

	public class LogData
	{
		public List<LogEntry> Data = new List<LogEntry>(); 

		public LogData(string fileName)
		{
			string[] allLines = File.ReadAllLines(fileName);
			if (allLines.Length > 1)
			{
				if (allLines[0].IndexOf("GPSTrackedSatellites") > -1)
                    ParseStatuChannelExport(allLines);	
				else if (allLines[0].IndexOf("GpsTimeAccu") > -1)
					ParseContentHTCC(allLines);	
				else
					ParseContent(allLines);	
			}
			
		}

        private void ParseStatuChannelExport(string[] content)
        {
            Data.Clear();

            //FrameNo,OCRStartTimestamp,OCREndTimestamp,NTPStartTimestamp,NTPEndTimestamp,StartTimestampSecondary,EndTimestampSecondary,NTPTimestampError,GPSTrackedSatellites,GPSAlmanacStatus,GPSFixStatus
            //"0","01-Jan-2010 00:00:00.000","01-Jan-2010 00:00:00.000","01-Jan-2010 00:00:00.000","01-Jan-2010 00:00:00.000","01-Jan-2010 00:00:00.000","01-Jan-2010 00:00:00.000","0","0","Uncertain","No Fix"
            //"1","26-Feb-2014 07:20:16.215","26-Feb-2014 07:20:16.255","26-Feb-2014 07:20:16.245","26-Feb-2014 07:20:16.245","26-Feb-2014 07:20:16.176","26-Feb-2014 07:20:16.176","220","9","Good","P Fix"
            //"2","26-Feb-2014 07:20:16.255","26-Feb-2014 07:20:16.295","26-Feb-2014 07:20:16.245","26-Feb-2014 07:20:16.285","26-Feb-2014 07:20:16.167","26-Feb-2014 07:20:16.207","220","9","Good","P Fix"

            for (int i = 1; i < content.Length; i++)
            {
                string[] tokens = content[i].Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries);

                if (tokens.Length == 11 || tokens.Length == 9)
                {
                    int frameNo = int.Parse(tokens[0].Trim('"'), CultureInfo.InvariantCulture);
                    DateTime ocrTime = DateTime.ParseExact(tokens[1].Trim('"'), "dd-MMM-yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture);
                    DateTime ntpTime = DateTime.ParseExact(tokens[3].Trim('"'), "dd-MMM-yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture);
                    double ntpDiff = new TimeSpan(ocrTime.Ticks - ntpTime.Ticks).TotalMilliseconds;
                    double winDiff = 0;
                    if (tokens.Length == 11)
                    {
                        DateTime winTime = DateTime.ParseExact(tokens[5].Trim('"'), "dd-MMM-yyyy HH:mm:ss.fff", CultureInfo.InvariantCulture);
                        winDiff = new TimeSpan(ocrTime.Ticks - winTime.Ticks).TotalMilliseconds;
                    }

                    Data.Add(new LogEntry()
                    {
                        WinAccu = (long)winDiff,
                        WinAccuNorm = (long)winDiff,
                        OccuRecAccu = (long)ntpDiff,
                        OccuRecErr = 1,
                        TimeDrift = 0,
                        TimeDriftErr = 0,
                        NTPAccu = 1,
                        NTPTimeUpdate = true
                    });
                }
            }
        }
		private void ParseContentHTCC(string[] content)
		{
			Data.Clear();

			//WinAccu		GpsTimeAccu	OccuRecAccu	Error	NTPAccu	NTPLatency
			//5.9		0.7		6.4		31.0	-9.6+/-1.51	57.3
			//4.8		0.8		5.4		31.0	-9.6+/-1.51	57.3
			//4.7		0.8		5.4		31.0	-9.6+/-1.51	57.3
			//6.1		0.7		7.0		31.0	2.6+/-1.64	57.0

			for (int i = 1; i < content.Length; i++)
			{
				string[] tokens = content[i].Split(new char[] {'\t', ' '}, StringSplitOptions.RemoveEmptyEntries);
				if (tokens.Length == 6)
				{
					float winAccu = float.Parse(tokens[0], CultureInfo.InvariantCulture);
					float gpsAccu = float.Parse(tokens[1], CultureInfo.InvariantCulture);
					float occuRecAccu = float.Parse(tokens[2], CultureInfo.InvariantCulture);
					float occuRecErr = float.Parse(tokens[3], CultureInfo.InvariantCulture);

					float ntpCorr = 0;
					float ntpCorrErr = 0;

					string[] diffCorrToks = tokens[4].Split("/".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
					if (diffCorrToks.Length == 2)
					{
						ntpCorr = float.Parse(diffCorrToks[0].TrimEnd('+'), CultureInfo.InvariantCulture);
						ntpCorrErr = float.Parse(diffCorrToks[1].TrimStart('-'), CultureInfo.InvariantCulture);
					}

					float ntpLatency = float.Parse(tokens[5].TrimEnd('*'), CultureInfo.InvariantCulture);

					Data.Add(new LogEntry()
					{
						WinAccu = winAccu,
						WinAccuNorm = winAccu,
						OccuRecAccu = occuRecAccu,
						OccuRecErr = gpsAccu,
						TimeDrift = 0,
						TimeDriftErr = 0,
						NTPAccu = gpsAccu,
						NTPTimeUpdate = true
					});
				}
			}
		}


		private void ParseContent(string[] content)
		{
			Data.Clear();

			//	WinAccu		WinAccuNorm	OccuRecAccu	Error	DriftCorr	NTPAccu
			//	87.0		92.4		8.0		16.0	0.0+/-0.00	17.6
			//	-13.5		-14.2		6.5		16.0	0.0+/-0.00	16.9
			//	-10.1		-11.8		4.5		16.0	0.0+/-0.00	17.0
			//	-18.9		-19.1		8.0		16.0	0.0+/-0.00	15.4*		

			for (int i = 1; i < content.Length; i++)
			{
				string[] tokens = content[i].Split(new char[] {'\t', ' '}, StringSplitOptions.RemoveEmptyEntries);
				if (tokens.Length == 6)
				{
					float winAccu = float.Parse(tokens[0], CultureInfo.InvariantCulture);
					float winAccuNorm = float.Parse(tokens[1], CultureInfo.InvariantCulture);
					float occuRecAccu = float.Parse(tokens[2], CultureInfo.InvariantCulture);
					float occuRecErr = float.Parse(tokens[3], CultureInfo.InvariantCulture);

					float diffCorr = 0;
					float diffCorrErr = 0;

					string[] diffCorrToks = tokens[4].Split("/".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
					if (diffCorrToks.Length == 2)
					{
						diffCorr = float.Parse(diffCorrToks[0].TrimEnd('+'), CultureInfo.InvariantCulture);
						diffCorrErr = float.Parse(diffCorrToks[1].TrimStart('-'), CultureInfo.InvariantCulture);
					}

					float ntpAccu = float.Parse(tokens[5].TrimEnd('*'), CultureInfo.InvariantCulture);
					bool ntpRefChanged = tokens[5].IndexOf("*", StringComparison.InvariantCultureIgnoreCase) > -1;

					Data.Add(new LogEntry()
					{
						WinAccu = winAccu,
						WinAccuNorm = winAccuNorm,
						OccuRecAccu = occuRecAccu,
						OccuRecErr = occuRecErr,
						TimeDrift = diffCorr,
						TimeDriftErr = diffCorrErr,
						NTPAccu = ntpAccu,
						NTPTimeUpdate = ntpRefChanged
					});
				}
			}
		}

	}
}
