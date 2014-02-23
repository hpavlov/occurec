using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using OccuRec.Helpers;
using OccuRec.Utilities;

namespace OccuRec.FrameAnalysis
{
	internal class PlateSolveManager
	{
		internal static PlateSolveManager Instance = new PlateSolveManager();

		private Thread m_PlateSolvingThread = null;
		private object m_SyncLock = new object();
		private bool m_Running = false;

		public PlateSolveManager()
		{
			m_PlateSolvingThread = new Thread(PlateSolvingBackgroundProcessing);
			m_PlateSolvingThread.Priority = ThreadPriority.Lowest;
			m_PlateSolvingThread.Start();
		}

		public void ProcessFrame(VideoFrameWrapper frame)
		{			
			lock (m_SyncLock)
			{
				// TODO: Add the frame for processing if a new frame is required (i.e. Telescope has reported coordinates and last frame has finished processing)		
			}
		}

		private void PlateSolvingBackgroundProcessing(object state)
		{
			m_Running = true;

			try
			{
				while (m_Running)
				{
					// Pick a new frame here and try to solve it
					// Have a sensible timeout so this doesn't 
					lock (m_SyncLock)
					{
						// TODO:
					}

					Thread.Sleep(100);
				}
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex.GetFullStackTrace());
				m_Running = false;
			}
		}

		internal void StopBackgroundProcesing()
		{
			if (m_PlateSolvingThread != null)
			{
				lock (m_SyncLock)
				{
					if (m_PlateSolvingThread != null)
					{
						m_Running = false;

						if (m_PlateSolvingThread.IsAlive)
							m_PlateSolvingThread.Join(1000);

						if (m_PlateSolvingThread.IsAlive)
							m_PlateSolvingThread.Abort();

						m_PlateSolvingThread = null;
					}
				}
			}
		}

	}
}
