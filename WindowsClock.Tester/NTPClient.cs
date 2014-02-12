using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace WindowsClock.Tester
{
    public class NTPClient
    {
	    private static object s_SyncLock = new object();

	    public static DateTime GetNetworkTime(string ntpServer, out float latencyInMilliseconds)
	    {
		    float occuRecTimeDiff = 0;
			float occuRecTimeDiffErr = 0;
		    float occuRecTimeDriftCorr = 0;
		    float winTimeDiff = 0;
			return GetNetworkTime(ntpServer, false, out latencyInMilliseconds, ref occuRecTimeDiff, ref occuRecTimeDiffErr, ref occuRecTimeDriftCorr, ref winTimeDiff);
	    }

	    // http://stackoverflow.com/questions/1193955/how-to-query-an-ntp-server-using-c
		public static DateTime GetNetworkTime(string ntpServer, bool timeAgainstOccuRecKeptTime, out float latencyInMilliseconds,
			ref float occuRecTimeDiff, ref float occuRecTimeDiffErr, ref float occuRecTimeDriftCorr, ref float winTimeDiff)
        {
	        NTPTimeKeeper.AttemptingNTPTimeUpdate();

            // NTP message size - 16 bytes of the digest (RFC 2030)
            var ntpData = new byte[48];

            //Setting the Leap Indicator, Version Number and Mode values
            ntpData[0] = 0x1B; //LI = 0 (no warning), VN = 3 (IPv4 only), Mode = 3 (Client Mode)

            var addresses = Dns.GetHostEntry(ntpServer).AddressList;

            //The UDP port number assigned to NTP is 123
            var ipEndPoint = new IPEndPoint(addresses[0], 123);
            //NTP uses UDP
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            socket.Connect(ipEndPoint);
	        long startTicks = 0;
			long endTicks = 0;
			long clockFrequency = 0;
            latencyInMilliseconds = 0;

			Profiler.QueryPerformanceFrequency(ref clockFrequency);

			#region timeAgainstOccuRecKeptTime
			double maxError = 0;
			double timeDriftCorrectionMilliseconds = 0;
			DateTime sentTimeOccuRec = DateTime.MinValue;
			DateTime rcvdTimeOccuRec = DateTime.MinValue;
			DateTime sentTimeWindows = DateTime.MinValue;
			DateTime rcvdTimeWindows = DateTime.MinValue;
			if (timeAgainstOccuRecKeptTime)
			{
				sentTimeOccuRec = NTPTimeKeeper.UtcNow(out maxError, out timeDriftCorrectionMilliseconds);
				sentTimeWindows = DateTime.UtcNow;
			}
			#endregion

			Profiler.QueryPerformanceCounter(ref startTicks);
            socket.Send(ntpData);
	        socket.ReceiveTimeout = 3000;
			try
			{
				socket.Receive(ntpData);

				Profiler.QueryPerformanceCounter(ref endTicks);
				
				#region timeAgainstOccuRecKeptTime
				if (timeAgainstOccuRecKeptTime)
				{
					rcvdTimeOccuRec = NTPTimeKeeper.UtcNow(out maxError, out timeDriftCorrectionMilliseconds);
					rcvdTimeWindows = DateTime.UtcNow;
				}
				#endregion

				latencyInMilliseconds += (endTicks - startTicks) * 1000.0f / clockFrequency;
			}
			catch (SocketException sex)
			{
				Trace.WriteLine(sex.GetFullStackTrace());

				// http://msdn.microsoft.com/en-us/library/windows/desktop/ms740668(v=vs.85).aspx
				if (sex.ErrorCode == 995 || /* WSA_OPERATION_ABORTED 995 */
				    sex.ErrorCode == 10060 /* WSAETIMEDOUT 10060 */)
				{
					latencyInMilliseconds = float.NaN;
				}
				else
					throw;
			}

            socket.Close();

            //Offset to get to the "Transmit Timestamp" field (time at which the reply 
            //departed the server for the client, in 64-bit timestamp format."
            const byte serverReplyTime = 40;

            //Get the seconds part
            ulong intPart = BitConverter.ToUInt32(ntpData, serverReplyTime);

            //Get the seconds fraction
            ulong fractPart = BitConverter.ToUInt32(ntpData, serverReplyTime + 4);

            //Convert From big-endian to little-endian
            intPart = SwapEndianness(intPart);
            fractPart = SwapEndianness(fractPart);

            ulong milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);

	        if (milliseconds == 0)
		        throw new InvalidOperationException("NTP Server returned an empty response.");

            //**UTC** time
			DateTime networkDateTime = (new DateTime(1900, 1, 1)).AddMilliseconds((long)milliseconds);

			#region timeAgainstOccuRecKeptTime
			if (timeAgainstOccuRecKeptTime)
			{
				DateTime occuRecTime = new DateTime((rcvdTimeOccuRec.Ticks + sentTimeOccuRec.Ticks) / 2);
				DateTime winTime = new DateTime((rcvdTimeWindows.Ticks + sentTimeWindows.Ticks) / 2);

				occuRecTimeDiff = (float)new TimeSpan(occuRecTime.Ticks - networkDateTime.Ticks).TotalMilliseconds;
				occuRecTimeDiffErr = (float) maxError;
				occuRecTimeDriftCorr = (float) timeDriftCorrectionMilliseconds;

				winTimeDiff = (float)new TimeSpan(winTime.Ticks - networkDateTime.Ticks).TotalMilliseconds;
			}
			#endregion

			lock (s_SyncLock)
			{
				NTPTimeKeeper.ProcessNTPResponce(startTicks, endTicks, clockFrequency, networkDateTime);	
			}			

            return networkDateTime;
        }

        // stackoverflow.com/a/3294698/162671
        static uint SwapEndianness(ulong x)
        {
            return (uint)(((x & 0x000000ff) << 24) +
                           ((x & 0x0000ff00) << 8) +
                           ((x & 0x00ff0000) >> 8) +
                           ((x & 0xff000000) >> 24));
        }

        [DllImport("kernel32.dll")]
        private extern static void GetSystemTime(ref SYSTEMTIME lpSystemTime);

        [DllImport("kernel32.dll")]
        private extern static uint SetSystemTime(ref SYSTEMTIME lpSystemTime);


        public struct SYSTEMTIME
        {
            public ushort wYear;
            public ushort wMonth;
            public ushort wDayOfWeek;
            public ushort wDay;
            public ushort wHour;
            public ushort wMinute;
            public ushort wSecond;
            public ushort wMilliseconds;
        }

        public static void SetTime(DateTime newUtcTime)
        {
            // Call the native GetSystemTime method 
            // with the defined structure.
            SYSTEMTIME systime = new SYSTEMTIME();
            GetSystemTime(ref systime);

            systime.wYear = (ushort)newUtcTime.Year;
            systime.wMonth = (ushort)newUtcTime.Month;
            systime.wDay = (ushort)newUtcTime.Day;
            systime.wHour = (ushort)newUtcTime.Hour;
            systime.wMinute = (ushort)newUtcTime.Minute;
            systime.wSecond = (ushort)newUtcTime.Second;
            systime.wMilliseconds = (ushort)newUtcTime.Millisecond;

            SetSystemTime(ref systime);
        }
    }

	// 
	// Research Article: Internal Clock Drift Estimation in Computer Clusters
	// http://www.hindawi.com/journals/jcnc/2008/583162/

	public class NTPTimeKeeper
	{
		private static long s_ReferenceTicks = -1;
		private static long s_ReferenceDateTimeTicks = -1;
		private static long s_FirstReferenceTicks = long.MaxValue;
		private static long s_10MinAgoReferenceTicks = long.MaxValue;
		private static long s_FirstReferenceDateTimeTicks = -1;
		private static long s_10MinAgoReferenceDateTimeTicks = -1;
		private static DateTime s_ReferenceDateTime;
		private static long s_ReferenceFrequency = -1;
		private static long s_ReferenceMaxError = -1;
		private static double s_TimeDriftPerMinuteMilleseconds = 0;

		private static DateTime s_FirtstAttemptedDateTimeUpdate = DateTime.MinValue;
		private static List<Tuple<DateTime, long>> s_AllNTPReferenceTimes = new List<Tuple<DateTime, long>>();

		private static object s_SyncLock = new object();
		private static LinearRegression s_LR = new LinearRegression();
		private static long s_LRZeroX = 0;
		private static long s_LRZeroY = 0;
		private static double s_LR3SigmaMs = 0;

		public static void AttemptingNTPTimeUpdate()
		{
			if (s_FirtstAttemptedDateTimeUpdate == DateTime.MinValue)
				s_FirtstAttemptedDateTimeUpdate = DateTime.UtcNow;
		}

		public static void ProcessNTPResponce(long startTicks, long endTicks, long frequency, DateTime utcTime)
		{
            long maxError = (long)(0.5 + (endTicks - startTicks) * 1000.0f / frequency);
		    if (maxError <= 0) return;

			long currTicks = startTicks + (maxError / 2);

			if (s_ReferenceFrequency != frequency || s_FirstReferenceTicks > currTicks)
			{
				// This is the first measurement
				bool isFirstReference = s_ReferenceFrequency != frequency;
				UpdateTimeReference(currTicks, frequency, utcTime, maxError);

				if (isFirstReference)
					Trace.WriteLine(string.Format("OccuRec: First time reference set. Max error is {0} ms.", s_ReferenceMaxError.ToString("0.0")));
				else
					Trace.WriteLine(string.Format("OccuRec: Reference is set after QPC overflow. Max error is {0} ms.", s_ReferenceMaxError.ToString("0.0")));
			}
			else
			{
 				if (maxError > 1000)
 				{
 					Trace.WriteLine(string.Format("OccuRec: Current NTP time reference has a max error of {0} ms. Ignored as this is more than 1 sec.", maxError.ToString("0.0")));
 				}
				else if (maxError > 10 * s_ReferenceMaxError)
				{
					Trace.WriteLine(string.Format("OccuRec: Current NTP time reference has a max error of {0} ms. Ignored as this is more than 10 times the current reference max error of {1} ms", maxError.ToString("0.0"), s_ReferenceMaxError.ToString("0.0")));
				}
				else
 				{
					// * Only apply drift corrections if we have info for more than 5 minutes
					// * Always keep the 'window' for determining the time drift correction to no more than 30 min (i.e. the trend may change for periods larger than 30 min)
					// * We don't want to end up in a situation where a single very accurate NTP reference done 2 hours ago is never updated. For this reason
					//   if the difference between the total drift since the last NTP reference time and now when computer using (1) time drift based on the 
					//   first ever received NTP reference and now and (2) time drift based on the NTP reference received 30 min ago and now, plus the 
					//   max error of the last NTP reference is bigger than the current max error of the NTP reference. This will mean that the NTP reference will not be 
					//   updated because of a time drift during the first 30 min after OccuRec has been started

					double tsSinceFirstReference = new TimeSpan(utcTime.Ticks - s_10MinAgoReferenceDateTimeTicks).TotalMinutes;
					double timeDriftErrorMilliseconds = 0;
					if (tsSinceFirstReference > 5)
					{
						double timeDriftMilleseconds;
						//ComputeTimeDriftFromTwoEndPoints(currTicks, frequency, utcTime, tsSinceFirstReference, out timeDriftMilleseconds, out timeDriftErrorMilliseconds);

						ComputeTimeDriftFromAllPoints(out timeDriftMilleseconds, out timeDriftErrorMilliseconds);

						// Remove all saved NTP references older than 10min
						for (int i = s_AllNTPReferenceTimes.Count - 1; i >= 0; i--)
						{
							if (s_AllNTPReferenceTimes[i].Item1.AddMinutes(10).Ticks < utcTime.Ticks)
							{
								Trace.WriteLine(string.Format("OccuRec: Removing NTP reference saved at {0} local time as it is too old.", s_AllNTPReferenceTimes[i].Item1.ToLocalTime().ToString("HH:mm:ss")));
								s_AllNTPReferenceTimes.RemoveAt(i);
							}
						}

						if (s_AllNTPReferenceTimes.Count > 0 && s_AllNTPReferenceTimes[0].Item1.Ticks > s_10MinAgoReferenceDateTimeTicks)
						{
							Trace.WriteLine(string.Format("OccuRec: Sliding the 30 min NTP reference window to start at {0}", s_AllNTPReferenceTimes[0].Item1.ToLocalTime().ToString("HH:mm:ss")));
							// Our 30MinAgo reference is too old now. We have a newer reference that it still at least 10 min go. We want to use this reference i.e. 'slide' the window forward	
							s_10MinAgoReferenceDateTimeTicks = s_AllNTPReferenceTimes[0].Item1.Ticks;
							s_10MinAgoReferenceTicks = s_AllNTPReferenceTimes[0].Item2;
						}
					}

					if (maxError < s_ReferenceMaxError + timeDriftErrorMilliseconds)
					{
						Trace.WriteLine(string.Format(
							timeDriftErrorMilliseconds == 0
								? "OccuRec: Time reference updated. Current measurement's error of {0} ms is smaller that the last reference's error of {1} ms"
								: "OccuRec: Time reference updated. Current measurement's error of {0} ms is smaller that the last reference's error of {1} ms +/-{2} ms time-drift error.",
							maxError.ToString("0.0"),
							s_ReferenceMaxError.ToString("0.0"),
							timeDriftErrorMilliseconds.ToString("0.0")));

						UpdateTimeReference(currTicks, frequency, utcTime, maxError);
					}

					s_AllNTPReferenceTimes.Add(new Tuple<DateTime, long>(utcTime, currTicks));
				}
			}
		}

		private static void ComputeTimeDriftFromAllPoints(out double timeDriftMilleseconds, out double timeDriftErrorMilliseconds)
		{
			timeDriftMilleseconds = 0;
			timeDriftErrorMilliseconds = 0;

			if (s_AllNTPReferenceTimes.Count > 5)
			{
				
					// DELTA-NTP-TIME[y] = f(DELTA-QPC-TICKS[x])
					var lr = new LinearRegression();
					try
					{
						long firstNTPTicks = s_10MinAgoReferenceDateTimeTicks;
						long firstQPCTcisk = s_10MinAgoReferenceTicks;

						for (int i = 1; i < s_AllNTPReferenceTimes.Count; i++)
						{
							lr.AddDataPoint(s_AllNTPReferenceTimes[i].Item2 - firstQPCTcisk, s_AllNTPReferenceTimes[i].Item1.Ticks - firstNTPTicks);
						}
						lr.Solve();

						timeDriftErrorMilliseconds = new TimeSpan((long)(3 * lr.StdDev)).TotalMilliseconds;
						timeDriftMilleseconds = 0;

						lock (s_SyncLock)
						{
							s_LR = lr;
							s_LRZeroX = firstQPCTcisk;
							s_LRZeroY = firstNTPTicks;
							s_LR3SigmaMs = timeDriftErrorMilliseconds;
						}

						
					}
					catch (Exception ex)
					{
						Trace.WriteLine(ex.GetFullStackTrace());
					}					

			}
		}

		private static void ComputeTimeDriftFromTwoEndPoints(long currTicks, long frequency, DateTime utcTime, double tsSinceFirstReference, out double timeDriftMilleseconds, out double timeDriftErrorMilliseconds)
		{
			timeDriftErrorMilliseconds = 0;

			double totalPassedMinutesNTP = ((currTicks - s_10MinAgoReferenceTicks) * 1000.0f / frequency) / 60000;
			double sectionRatio = 1.0 * (utcTime.Ticks - s_ReferenceDateTimeTicks) / (utcTime.Ticks - s_10MinAgoReferenceDateTimeTicks);
			timeDriftMilleseconds = sectionRatio * Math.Abs(totalPassedMinutesNTP - tsSinceFirstReference) * 60000;
			s_TimeDriftPerMinuteMilleseconds = timeDriftMilleseconds / totalPassedMinutesNTP;

			double totalPassedMinutesNTPBase0 = double.NaN;
			double timeDriftMillesecondsBase0 = double.NaN;
			if (s_10MinAgoReferenceTicks != s_FirstReferenceTicks)
			{
				totalPassedMinutesNTPBase0 = ((currTicks - s_FirstReferenceTicks) * 1000.0f / frequency) / 60000;
				double sectionRatioBase0 = 1.0 * (utcTime.Ticks - s_ReferenceDateTimeTicks) / (utcTime.Ticks - s_FirstReferenceDateTimeTicks);
				double tsSinceFirstReferenceBase0 = new TimeSpan(utcTime.Ticks - s_FirstReferenceDateTimeTicks).TotalMinutes;
				timeDriftMillesecondsBase0 = sectionRatioBase0 * Math.Abs(totalPassedMinutesNTPBase0 - tsSinceFirstReferenceBase0) * 60000;

				timeDriftErrorMilliseconds = Math.Abs(timeDriftMillesecondsBase0 - timeDriftMilleseconds);
			}

			if (!double.IsNaN(totalPassedMinutesNTPBase0))
			{
				Trace.WriteLine(string.Format(
					timeDriftErrorMilliseconds > 0
						? "OccuRec: Current time drift for {0} min is {1} ms +/- {2} ms ({3} ms/min). Time drift for {4} min is {5} ms."
						: "OccuRec: Current time drift for {0} min is {1} ms ({3} ms/min). Time drift for {4} min is {5} ms.",
					totalPassedMinutesNTP.ToString("0.0"),
					timeDriftMilleseconds.ToString("0.0"),
					timeDriftErrorMilliseconds.ToString("0.0"),
					s_TimeDriftPerMinuteMilleseconds.ToString("0.00"),
					totalPassedMinutesNTPBase0.ToString("0.0"),
					timeDriftMillesecondsBase0.ToString("0.0")));
			}
			else
			{
				Trace.WriteLine(string.Format(
					timeDriftErrorMilliseconds > 0
						? "OccuRec: Current time drift for {0} min is {1} ms +/- {2} ms ({3} ms/min)."
						: "OccuRec: Current time drift for {0} min is {1} ms ({3} ms/min).",
					totalPassedMinutesNTP.ToString("0.0"),
					timeDriftMilleseconds.ToString("0.0"),
					timeDriftErrorMilliseconds.ToString("0.0"),
					s_TimeDriftPerMinuteMilleseconds.ToString("0.00")));
			}
			
		}

		public static Color GetCurrentNTPStatusColour(out string statusMessage)
		{
			// If there is no first reference set and OccuRec has been started for more than 1 min, this is status Red  
			if (s_FirtstAttemptedDateTimeUpdate > DateTime.MinValue && 				
			    s_FirstReferenceTicks == long.MaxValue)
			{
				double secondsSinceFirstTimeUpdateAttempt = new TimeSpan(DateTime.UtcNow.Ticks - s_FirtstAttemptedDateTimeUpdate.Ticks).TotalSeconds;

				if (secondsSinceFirstTimeUpdateAttempt > 60)
				{
					statusMessage = "NTP time has been never updated. Check your internet connection and NTP server settings.";
					return Color.Red;
				}
				else
				{
					// This is how we tell that no NTP status is yet known
					statusMessage = null;
					return SystemColors.Control;					
				}
			}

			// If the current error is bigger then 1 sec then this is status Red
			if (s_ReferenceMaxError > 1000)
			{
				statusMessage = "Current NTP time error is larger than 1 sec.";
				return Color.Red;
			}
			
			// If the current error is between 250ms and 1 sec this is status OrangeRed
			if (s_ReferenceMaxError > 250)
			{
				statusMessage = "Current NTP time error is larger than 250 ms but smaller than 1 sec.";
				return Color.OrangeRed;
			}


			// If the current error is between 100ms and 250ms this is status DarkGoldenrod
			if (s_ReferenceMaxError > 100)
			{
				statusMessage = "Current NTP time error is larger than 100 ms but smaller than 250 ms.";
				return Color.DarkGoldenrod;
			}

			// If the interval between the first reference and now is more than 5 min and curent error is less than 100ms, this is status Green
			if (s_ReferenceMaxError <= 100)
			{
				statusMessage = "Current NTP time error is smaller than 100 ms.";
				return Color.Green;
			}

			statusMessage = null;
			return SystemColors.Control;
		}

		private static void UpdateTimeReference(long currTicks, long frequency, DateTime utcTime, long maxError)
		{
			s_ReferenceFrequency = frequency;
			s_ReferenceTicks = currTicks;
			s_ReferenceDateTime = utcTime;
			s_ReferenceDateTimeTicks = utcTime.Ticks;
			s_ReferenceMaxError = maxError;

			if (s_FirstReferenceTicks > s_ReferenceTicks)
			{
				s_FirstReferenceTicks = s_ReferenceTicks;
				s_FirstReferenceDateTimeTicks = s_ReferenceDateTimeTicks;
				s_10MinAgoReferenceTicks = s_ReferenceTicks;
				s_10MinAgoReferenceDateTimeTicks = s_ReferenceDateTimeTicks;
				s_TimeDriftPerMinuteMilleseconds = 0;
				s_AllNTPReferenceTimes.Clear();
				s_AllNTPReferenceTimes.Add(new Tuple<DateTime, long>(utcTime, currTicks));
			}
		}

		public static DateTime UtcNow(out double maxErrorMilliseconds, out double timeDriftCorrectionMilliseconds)
		{
			if (s_ReferenceFrequency > 0)
			{
				long ticksNow = 0;
				Profiler.QueryPerformanceCounter(ref ticksNow);
				double millsecondsFromReferenceFrame = (ticksNow - s_ReferenceTicks)*1000.0f/s_ReferenceFrequency;
				maxErrorMilliseconds = s_ReferenceMaxError;

				timeDriftCorrectionMilliseconds = 0;
				
				DateTime utcNowNoDriftCorrection = s_ReferenceDateTime
					.AddMilliseconds(millsecondsFromReferenceFrame); // Add the elapsed milliseconds to the NTP reference time					


				// timeDriftCorrectionMilliseconds = s_TimeDriftPerMinuteMilleseconds * millsecondsFromReferenceFrame / 60000;

				lock (s_SyncLock)
				{
					if (s_LR != null && s_LR.IsSolved)
					{
						long lrYVal = (long)s_LR.ComputeY(ticksNow - s_LRZeroX);
						long ticksDriftFromZeroY = new DateTime(lrYVal + s_LRZeroY).Ticks - utcNowNoDriftCorrection.Ticks;
						timeDriftCorrectionMilliseconds = (new TimeSpan(ticksDriftFromZeroY).TotalMilliseconds) * (utcNowNoDriftCorrection.Ticks - s_ReferenceDateTime.Ticks) / (utcNowNoDriftCorrection.Ticks - s_LRZeroY);
					}
				}

				return utcNowNoDriftCorrection.AddMilliseconds(timeDriftCorrectionMilliseconds); // Correct for CPU clock time drift
			}
			else
			{
				maxErrorMilliseconds = 60*1000;
				timeDriftCorrectionMilliseconds = 0;
				return DateTime.UtcNow;
			}
		}
	}
}
