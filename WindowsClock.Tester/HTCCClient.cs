using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;

namespace WindowsClock.Tester
{
	public class HTCCClient
	{
		private SerialPort m_CommPort;

		public HTCCClient(SerialPort commPort)
		{
			m_CommPort = commPort;
		}

		public DateTime TimeActionInUTC(Action actionToTime, out float htccLatency)
		{
			byte[] tsCommand = new byte[] { (byte)'C' };

			m_CommPort.Write(tsCommand, 0, 1);
			actionToTime();
			m_CommPort.Write(tsCommand, 0, 1);

			byte[] twoResponses = new byte[30];

			for (int i = 0; i < 30; i++)
			{
				int btRead = m_CommPort.ReadByte();
				twoResponses[i] = (byte)btRead;
			}

			long startTicks = ExtractHtccTime(twoResponses, 0).Ticks;
			long endTicks = ExtractHtccTime(twoResponses, 15).Ticks;

			htccLatency = (float)new TimeSpan(endTicks - startTicks).TotalMilliseconds;
			return new DateTime((startTicks + endTicks) / 2);
		}

		private DateTime ExtractHtccTime(byte[] rawData, int startIndex)
		{
			int timestampUtcYear = 2000 + rawData[startIndex + 2];
			int timestampUtcMonth = rawData[startIndex + 3];
			int timestampUtcDay = rawData[startIndex + 4];
			int timestampUtcHours = rawData[startIndex + 5];
			int timestampUtcMinutes = rawData[startIndex + 6];
			int timestampUtcSecond = rawData[startIndex + 7];
			int timestampUtcFractionalSecond10000 = rawData[startIndex + 8] * 100 + rawData[9];

			DateTime rv = new DateTime(timestampUtcYear, timestampUtcMonth, timestampUtcDay, timestampUtcHours, timestampUtcMinutes, timestampUtcSecond, (int)Math.Round(timestampUtcFractionalSecond10000 / 10.0));
			return rv;
		}
	}
}
