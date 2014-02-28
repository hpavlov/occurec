using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OccuRec.ASCOM;
using OccuRec.ASCOM.Interfaces.Devices;
using OccuRec.ASCOM.Wrapper.Interfaces;
using OccuRec.Helpers;
using OccuRec.Tracking;

namespace OccuRec.ObservatoryAutomation.StateMachine.PulseGuiding
{
	internal class PgcInitialState : AutomationState
	{
		private Guid m_ObservatoryControlLock;

		private bool m_FirstFrameReceived = false;
		private bool m_PulseIssued = false;
		private bool m_PulseFinished = false;
		private bool m_LastFrameReceived = false;

		internal PgcInitialState(AutomationStateMachine stateMachine, Guid observatoryControlLock)
			: base(stateMachine)
		{
			m_ObservatoryControlLock = observatoryControlLock;
		}

		internal override void Initialise()
		{
			m_FirstFrameReceived = false;
			m_PulseIssued = false;
			m_PulseFinished = false;
			m_LastFrameReceived = false;
		}

		private void ObservatoryController_SlewCompleted(ObservatoryControllerCallbackArgs arg)
		{
			m_PulseFinished = true;
		}

		internal override void ProcessFrame(VideoFrameWrapper frame, LastTrackedPosition locatedGuidingStar)
		{
			if (!m_FirstFrameReceived)
			{
				// TODO: Record the starting position
				m_FirstFrameReceived = true;


				m_StateMachine.ObservatoryController.TelescopePulseGuide(GuideDirections.guideEast, PulseRate.Fast, CallType.Async, m_ObservatoryControlLock, ObservatoryController_SlewCompleted);
				m_PulseIssued = true;
			}
			else if (m_PulseFinished)
			{
				// TODO: Recod last position
			}
		}
	}
}
