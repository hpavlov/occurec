using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OccuRec.ASCOM;
using OccuRec.Helpers;
using OccuRec.Tracking;

namespace OccuRec.FrameAnalysis
{
	internal class ObservatoryManager
	{
		private IObservatoryController m_ObservatoryController;

		private bool m_RunAutoFocusNow = false;
		private bool m_IsAutoFocusing = false;

		public ObservatoryManager(IObservatoryController observatoryController)
		{
			m_ObservatoryController = observatoryController;
		}

		public void TriggerAutoFocusing()
		{
			if (!m_IsAutoFocusing)
				m_RunAutoFocusNow = true;
		}

		public void TriggerPulseGuidingCalibration()
		{
			// TODO:
		}

		public void ProcessFrame(VideoFrameWrapper frame, LastTrackedPosition locatedGuidingStar)
		{
			if (m_RunAutoFocusNow && locatedGuidingStar != null && locatedGuidingStar.IsLocated && m_ObservatoryController.IsConnectedToTelescope())
			{
				// TODO: Start the auto focusing and monitor it. Do we want to use a state machine for this??
			}
			else if (m_IsAutoFocusing && m_ObservatoryController.IsConnectedToTelescope())
			{
				// TODO: Check the effect of the last focuser movement and issue a correction or end the focusing
				// TODO: Use a state machine to manage this.
			}
		}
	}
}
