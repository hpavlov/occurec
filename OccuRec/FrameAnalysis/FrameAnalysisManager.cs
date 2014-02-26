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
		internal TargetSignalMonitor m_TargetSignalMonitor;
		private PlateSolveManager m_PlateSolveManager;
		private AutoFocusingManager m_AutoFocusingManager;
		private IObservatoryController m_ObservatoryController;

		internal FrameAnalysisManager(IObservatoryController observatoryController)
		{
			m_PlateSolveManager = new PlateSolveManager(observatoryController);
			m_AutoFocusingManager = new AutoFocusingManager(observatoryController);
			m_TargetSignalMonitor = new TargetSignalMonitor(observatoryController, m_AutoFocusingManager);			
		}

		public void ProcessFrame(VideoFrameWrapper frame, Bitmap bmp)
		{
			// TODO: Make this processing Asynchronous so the painting is not delayed unnecessary (is this actually possible?)
			m_TargetSignalMonitor.ProcessFrame(frame);

			m_PlateSolveManager.ProcessFrameAsync(frame, bmp);
		}

		public void DisplayData(Graphics g, int imageWidth, int imageHeight)
		{
			m_TargetSignalMonitor.DisplayData(g, imageWidth, imageHeight);
		}

		public void UpdatePulseGuiding(bool autoPulseGuidingRequested)
		{
			m_TargetSignalMonitor.ChangeAutoPulseGuiding(autoPulseGuidingRequested);
		}

		public void TriggerAutoFocusing()
		{
			m_AutoFocusingManager.TriggerAutoFocusing();
		}

		public void Dispose()
		{
			m_PlateSolveManager.StopBackgroundProcesing();
		}
	}
}
