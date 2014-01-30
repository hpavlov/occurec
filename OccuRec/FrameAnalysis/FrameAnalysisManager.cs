using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using OccuRec.Helpers;

namespace OccuRec.FrameAnalysis
{
	public class FrameAnalysisManager
	{
		internal TargetSignalMonitor TargetSignalMonitor = new TargetSignalMonitor();

		public void ProcessFrame(VideoFrameWrapper frame)
		{
			TargetSignalMonitor.ProcessFrame(frame);

			// TODO: Add other analysis classes
		}

		public void DisplayData(Graphics g, int imageWidth, int imageHeight)
		{
			TargetSignalMonitor.DisplayData(g, imageWidth, imageHeight);
		}
	}
}
