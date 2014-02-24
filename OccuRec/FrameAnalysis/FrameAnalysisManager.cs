using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using OccuRec.ASCOM;
using OccuRec.Helpers;

namespace OccuRec.FrameAnalysis
{
	public class FrameAnalysisManager : IDisposable
	{
		internal TargetSignalMonitor TargetSignalMonitor = new TargetSignalMonitor();
		private PlateSolveManager m_PlateSolveManager;
		private IObservatoryController m_ObservatoryController;

		internal FrameAnalysisManager(IObservatoryController observatoryController)
		{
			m_PlateSolveManager = new PlateSolveManager(observatoryController);
		}

		public void ProcessFrame(VideoFrameWrapper frame, Bitmap bmp)
		{
			TargetSignalMonitor.ProcessFrame(frame);

			m_PlateSolveManager.ProcessFrame(frame, bmp);
		}

		public void DisplayData(Graphics g, int imageWidth, int imageHeight)
		{
			TargetSignalMonitor.DisplayData(g, imageWidth, imageHeight);
		}

		public void Dispose()
		{
			m_PlateSolveManager.StopBackgroundProcesing();
		}
	}
}
