using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using OccuRec.Helpers;

namespace OccuRec.FrameAnalysis
{
	public class FrameAnalysisManager : IDisposable
	{
		internal TargetSignalMonitor TargetSignalMonitor = new TargetSignalMonitor();
	
		public void ProcessFrame(VideoFrameWrapper frame)
		{
			TargetSignalMonitor.ProcessFrame(frame);

			PlateSolveManager.Instance.ProcessFrame(frame);
		}

		public void DisplayData(Graphics g, int imageWidth, int imageHeight)
		{
			TargetSignalMonitor.DisplayData(g, imageWidth, imageHeight);
		}

		public void Dispose()
		{
			PlateSolveManager.Instance.StopBackgroundProcesing();
		}
	}
}
