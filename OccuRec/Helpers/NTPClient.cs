using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using OccuRec.Drivers.AAVTimer.VideoCaptureImpl;
using OccuRec.Tracking;
using OccuRec.Utilities;

namespace OccuRec.Helpers
{
    public class NTPClient
    {
        // http://stackoverflow.com/questions/1193955/how-to-query-an-ntp-server-using-c
        public static DateTime GetNetworkTime(string ntpServer, out float latencyInMilliseconds)
        {
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

            Profiler.QueryPerformanceCounter(ref startTicks);
            socket.Send(ntpData);
	        socket.ReceiveTimeout = 3000;
			try
			{
				socket.Receive(ntpData);

				Profiler.QueryPerformanceCounter(ref endTicks);

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

            //**UTC** time
			DateTime networkDateTime = (new DateTime(1900, 1, 1)).AddMilliseconds((long)milliseconds);

			NTPTimeKeeper.ProcessNTPResponce(startTicks, endTicks, clockFrequency, networkDateTime);

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

	public class NTPTimeKeeper
	{
		private static long s_ReferenceTicks = -1;
		private static long s_ReferenceDateTimeTicks = -1;
		private static DateTime s_ReferenceDateTime;
		private static long s_ReferenceFrequency = -1;
		private static long s_ReferenceMaxError = -1;
		private static List<long> s_TimeDriftNTPTicks = new List<long>();
		private static List<long> s_TimeDriftQPCTicks = new List<long>();
		private static LinearRegression s_TimeDriftFit = new LinearRegression();
		private static bool s_HasTimeDriftData = false;

		public static void ProcessNTPResponce(long startTicks, long endTicks, long frequency, DateTime utcTime)
		{
            long maxError = (long)(0.5 + (endTicks - startTicks) * 1000.0f / frequency);
		    if (maxError <= 0) return;

			if (s_ReferenceFrequency != frequency)
			{
				// This is the first measurement
				UpdateTimeReference(startTicks, endTicks, frequency, utcTime, maxError);
				Trace.WriteLine(string.Format("OccuRec: First time reference set. Max error is {0}ms.", s_ReferenceMaxError.ToString("0.0")));
			}
			else if (s_TimeDriftNTPTicks.Count > 0)
			{
				ComputeTimeDriftRate(startTicks, endTicks, utcTime);

				if (maxError < s_ReferenceMaxError)
				{
					Trace.WriteLine(string.Format("OccuRec: Time reference updated. Current measurement's error of {0}ms is smaller that the last reference's error of {1}ms.", maxError.ToString("0.0"), s_ReferenceMaxError.ToString("0.0")));
					UpdateTimeReference(startTicks, endTicks, frequency, utcTime, maxError);
				}
				else if (s_HasTimeDriftData)
				{
				    var tsSinceLastUpdate = new TimeSpan(utcTime.Ticks - s_ReferenceDateTimeTicks).TotalMinutes;
                    if (tsSinceLastUpdate > 10)
                    {
                        UpdateTimeReference(startTicks, endTicks, frequency, utcTime, maxError);
                        Trace.WriteLine(string.Format("OccuRec: Time reference updated. No update has been done in the past 10 min. Current error is {0}ms.", maxError.ToString("0.0")));
                    }
				    //// If the error accumulated because of a time drift, since out last reference update, is bigger than the NTP max error, then update the time refernece
				    //long computedQPTTicks = (long)s_TimeDriftFit.ComputeY(utcTime.Ticks);
				    //double timeDriftMilliseconds = Math.Abs(computedQPTTicks - (startTicks + endTicks) / 2) * 1000.0f / frequency;
				    //if (maxError < timeDriftMilliseconds)
				    //{
				    //    Trace.WriteLine(string.Format("OccuRec: Time reference updated. Current measurement's error of {0}ms is smaller that the calculated time drift of {1}ms since the last reference was set.", maxError.ToString("0.0"), timeDriftMilliseconds.ToString("0.0")));
				    //    UpdateTimeReference(startTicks, endTicks, frequency, utcTime, maxError);
				    //}
				}
			}
		}

		private static void UpdateTimeReference(long startTicks, long endTicks, long frequency, DateTime utcTime, long maxError)
		{
			s_ReferenceFrequency = frequency;
			s_ReferenceTicks = (startTicks + endTicks) / 2;
			s_ReferenceDateTime = utcTime;
			s_ReferenceDateTimeTicks = utcTime.Ticks;
			s_ReferenceMaxError = maxError;

			s_TimeDriftQPCTicks.Add(s_ReferenceTicks);
			s_TimeDriftNTPTicks.Add(s_ReferenceDateTimeTicks);
		}

		private static void ComputeTimeDriftRate(long startTicks, long endTicks, DateTime utcTime)
		{
			var timespanSinceLastUpdate = new TimeSpan(utcTime.Ticks - s_TimeDriftNTPTicks[s_TimeDriftNTPTicks.Count - 1]);
			if (timespanSinceLastUpdate.TotalSeconds > 60)
			{
				if (s_TimeDriftNTPTicks.Count > 60)
				{
					s_TimeDriftNTPTicks.RemoveAt(0);
					s_TimeDriftQPCTicks.RemoveAt(0);
				}

				s_TimeDriftQPCTicks.Add((startTicks + endTicks) / 2);
				s_TimeDriftNTPTicks.Add(utcTime.Ticks);

				if (s_TimeDriftNTPTicks.Count > 3)
				{
					// Determine the drift rate
					// [QPC-Ticks] = A * [NTP-Ticks] + B
					s_TimeDriftFit.Reset();
					for (int i = 0; i < s_TimeDriftNTPTicks.Count; i++)
					{
						s_TimeDriftFit.AddDataPoint(s_TimeDriftNTPTicks[i], s_TimeDriftQPCTicks[i]);
					}
                    try
                    {
                        s_TimeDriftFit.Solve();
                        s_HasTimeDriftData = true;
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex.GetFullStackTrace());
                    }
				}
			}
		}

		public static DateTime UtcNow(out double maxErrorMilliseconds)
		{
			if (s_ReferenceFrequency > 0)
			{
				long ticksNow = 0;
				Profiler.QueryPerformanceCounter(ref ticksNow);
				double millsecondsFromReferenceFrame = (ticksNow - s_ReferenceTicks)*1000.0f/s_ReferenceFrequency;
				maxErrorMilliseconds = s_ReferenceMaxError;
				return s_ReferenceDateTime.AddMilliseconds(millsecondsFromReferenceFrame);
			}
			else
			{
				maxErrorMilliseconds = 60*1000;
				return DateTime.UtcNow;
			}
		}
	}
}
