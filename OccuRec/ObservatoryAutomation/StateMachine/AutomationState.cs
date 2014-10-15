/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OccuRec.Helpers;
using OccuRec.Tracking;

namespace OccuRec.ObservatoryAutomation.StateMachine
{
	internal abstract class AutomationState
	{
		protected AutomationStateMachine m_StateMachine;

		public AutomationState(AutomationStateMachine stateMachine)
		{
			m_StateMachine = stateMachine;
		}

		internal virtual void Initialise() { }

		internal virtual void Finalise() { }

		internal abstract void ProcessFrame(VideoFrameWrapper frame, LastTrackedPosition locatedGuidingStar);
	}
}
