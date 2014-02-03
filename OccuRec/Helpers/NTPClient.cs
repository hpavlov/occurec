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

            var milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);

            //**UTC** time
            var networkDateTime = (new DateTime(1900, 1, 1)).AddMilliseconds((long)milliseconds);

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
}
