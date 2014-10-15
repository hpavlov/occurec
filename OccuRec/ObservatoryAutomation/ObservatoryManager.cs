/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OccuRec.ASCOM;
using OccuRec.Helpers;
using OccuRec.ObservatoryAutomation.StateMachine;
using OccuRec.ObservatoryAutomation.StateMachine.PulseGuiding;
using OccuRec.Tracking;

namespace OccuRec.ObservatoryAutomation
{
	internal class ObservatoryManager
	{
		private IObservatoryController m_ObservatoryController;
		private AutomationStateMachine m_StateMachine;

		private bool m_RunAutoFocusNow = false;
		private bool m_IsAutoFocusing = false;

		public ObservatoryManager(IObservatoryController observatoryController)
		{
			m_ObservatoryController = observatoryController;
			m_StateMachine = new AutomationStateMachine(observatoryController);
		}

		public void TriggerAutoFocusing()
		{
			if (!m_IsAutoFocusing)
				m_RunAutoFocusNow = true;
		}

		public bool TriggerPulseGuidingCalibration()
		{
			if (!m_StateMachine.CanTriggerExternalStateChangeNow)
				return false;

			Guid? lockId = m_ObservatoryController.ControlTelescopeLock(true, null);
			if (lockId == null)
				return false;

			m_StateMachine.ChangeState(new PgcInitialState(m_StateMachine, lockId.Value));

			return true;
		}

		public bool IsPulseGuidingCalibrated()
		{
			// TODO:

			return false;
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
