using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace AAVRec.Drivers.AAVTimer.VideoCaptureImpl
{
	public static class Profiler
	{
		// TODO: This class is not thread safe! It must use .NET 4 System.Threading lists and objects

		[DllImport("Kernel32.dll")]
		public static extern void QueryPerformanceCounter(ref long ticks);

		[DllImport("Kernel32.dll")]
		public static extern void QueryPerformanceFrequency(ref long frequency);

		[DllImport("Kernel32.dll")]
		public static extern int GetCurrentThreadId();

		[ThreadStatic]
		private static Dictionary<long, List<long>> tlsStoredObjects = new Dictionary<long, List<long>>();

		private static Dictionary<long, List<long>> Measurements
		{
			get
			{
				Dictionary<long, List<long>> measurements = null;

				// Otherwise use the Thread Local Storage
				measurements = tlsStoredObjects;

				return measurements;
			}
		}

		private static long clockFrequency = 0;

		static Profiler()
		{
			QueryPerformanceFrequency(ref clockFrequency);
		}

		public static void ResetAll()
		{
			Measurements.Clear();
		}

		private static long threadStartTicks = -1;
		private static int startThreadId;
		private static int endThreadId;
		private static long startTicks;
		private static long threadEndTicks = -1;
		private static long endTicks;

		public static void ResetThreadCPUCounter()
		{
			QueryPerformanceCounter(ref startTicks);
			startThreadId = GetCurrentThreadId();
			ProcessThread thread = Process.GetCurrentProcess().Threads.Cast<ProcessThread>().FirstOrDefault(t => t.Id == startThreadId);
			if (thread != null) threadStartTicks = thread.TotalProcessorTime.Ticks;
		}

		public static void StopThreadCPUCounter()
		{
			endThreadId = GetCurrentThreadId();
			ProcessThread thread = Process.GetCurrentProcess().Threads.Cast<ProcessThread>().FirstOrDefault(t => t.Id == endThreadId);
			if (thread != null) threadEndTicks = thread.TotalProcessorTime.Ticks;
			QueryPerformanceCounter(ref endTicks);
		}

		public static string GetThreadCPUUsage()
		{
			TimeSpan ts = new TimeSpan(threadEndTicks - threadStartTicks);
			int cpuPercentage = (int)Math.Round(100f * ts.TotalMilliseconds / (Environment.ProcessorCount * (endTicks - startTicks) * 1000f / clockFrequency));
			if (startThreadId != endThreadId)
				return "--";
			else if (cpuPercentage == 0)
				return "0%";
			else
				return string.Format("<{0}%", cpuPercentage);
		}

		public static void ResetTimer(long timerId)
		{
			List<long> timerData = EnsureTimerData(timerId);
			timerData.Clear();
		}

		public static void ResetAndStartTimer(long timerId)
		{
			List<long> timerData = EnsureTimerData(timerId);
			timerData.Clear();

			long ticks = 0;
			QueryPerformanceCounter(ref ticks);

			timerData.Add(ticks);
		}

		public static long GetTicks()
		{
			long ticks = 0;
			QueryPerformanceCounter(ref ticks);
			return ticks;
		}

		public static double TranslateTicksToMilliseconds(long ticksDifference)
		{
			return ticksDifference * 1000.0 / clockFrequency;
		}

		public static void StartTimer(long timerId)
		{
			List<long> timerData = EnsureTimerData(timerId);

			// NOTE: Can only start the timer if the list measurements has an even number of entries i.e. it is not currently running
			if (timerData.Count % 2 == 1)
				throw new InvalidOperationException("Cannot start timer id: " + timerId);

			long ticks = 0;
			QueryPerformanceCounter(ref ticks);

			timerData.Add(ticks);
		}

		public static bool IsTimerRunning(long timerId)
		{
			List<long> timerData;
			if (!Measurements.TryGetValue(timerId, out timerData))
				return false;

			return (timerData.Count % 2) == 1;
		}

		public static void StopTimer(long timerId)
		{
			long ticks = 0;
			QueryPerformanceCounter(ref ticks);

			List<long> timerData = EnsureTimerData(timerId);

			// NOTE: Can only start the timer if the list measurements has an odd number of entries i.e. it must be currently running
			if (timerData.Count % 2 == 0)
				throw new InvalidOperationException("Cannot stop timer id: " + timerId);

			timerData.Add(ticks);
		}

		public static double GetElapsedMillisecondsForTimer(long timerId)
		{
			List<long> timerData = EnsureTimerData(timerId);

			double totalTicks = 0;

			for (int i = 0; i < timerData.Count / 2; i++)
			{
				long startTime = timerData[2 * i];
				long endTime = timerData[(2 * i) + 1];
				totalTicks += endTime - startTime;
			}

			return totalTicks * 1000 / clockFrequency;
		}

		public static void AddExternallyTimedInterval(long timerId, double timedMilliseconds)
		{
			List<long> timerData = EnsureTimerData(timerId);
			long totalTicks = (long)((timedMilliseconds * clockFrequency / 1000));
			timerData.Add(0);
			timerData.Add(totalTicks);
		}

		public static int GetNumberOfResumesForTimer(long timerId)
		{
			List<long> timerData = EnsureTimerData(timerId);
			return timerData.Count / 2;
		}


		private static List<long> EnsureTimerData(long timerId)
		{
			List<long> timerData;
			if (!Measurements.TryGetValue(timerId, out timerData))
			{
				timerData = new List<long>();
				Measurements.Add(timerId, timerData);
			}
			return timerData;
		}
	}
}
