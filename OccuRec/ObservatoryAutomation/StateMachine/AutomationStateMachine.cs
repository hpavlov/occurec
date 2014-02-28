﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OccuRec.ASCOM;
using OccuRec.Helpers;
using OccuRec.Tracking;

namespace OccuRec.ObservatoryAutomation.StateMachine
{
	internal class AutomationStateMachine
	{
		private AutomationState m_CurrentState;
		private IObservatoryController m_ObservatoryController;

		internal IObservatoryController ObservatoryController
		{
			get { return m_ObservatoryController; }
		}

		public AutomationStateMachine(IObservatoryController observatoryController)
		{
			m_ObservatoryController = observatoryController;
			ChangeState(new IdleState(this));
		}

		public void ProcessFrame(VideoFrameWrapper frame, LastTrackedPosition locatedGuidingStar)
		{
			m_CurrentState.ProcessFrame(frame, locatedGuidingStar);
		}

		public bool CanTriggerExternalStateChangeNow
		{
			get { return m_CurrentState is IdleState; }
		}

		public void ChangeState(AutomationState newState)
		{
			if (m_CurrentState != null)
				m_CurrentState.Finalise();

			m_CurrentState = newState;

			if (m_CurrentState != null)
				m_CurrentState.Initialise();
		}
	}
}
