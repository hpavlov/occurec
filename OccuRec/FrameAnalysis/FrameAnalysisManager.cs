/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using OccuRec.ASCOM;
using OccuRec.Helpers;
using OccuRec.ObservatoryAutomation;

namespace OccuRec.FrameAnalysis
{
	public class FrameAnalysisManager : IDisposable
	{
		internal TargetSignalMonitor m_TargetSignalMonitor;
		private PlateSolveManager m_PlateSolveManager;
		private ObservatoryManager m_ObservatoryManager;
		private IObservatoryController m_ObservatoryController;

		internal FrameAnalysisManager(IObservatoryController observatoryController)
		{
			m_PlateSolveManager = new PlateSolveManager(observatoryController);
			m_ObservatoryManager = new ObservatoryManager(observatoryController);
			m_TargetSignalMonitor = new TargetSignalMonitor(observatoryController, m_ObservatoryManager);			
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
			m_ObservatoryManager.TriggerAutoFocusing();
		}

		public bool TriggerPulseGuidingCalibration()
		{
			return m_ObservatoryManager.TriggerPulseGuidingCalibration();
		}

		public bool IsPulseGuidingCalibrated()
		{
			return m_ObservatoryManager.IsPulseGuidingCalibrated();
		}

		public void Dispose()
		{
			m_PlateSolveManager.StopBackgroundProcesing();
		}
	}
}
