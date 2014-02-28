using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OccuRec.Helpers;
using OccuRec.Tracking;

namespace OccuRec.ObservatoryAutomation.StateMachine
{
	internal class IdleState : AutomationState
	{
		public IdleState(AutomationStateMachine stateMachine)
			: base(stateMachine)
		{ }

		internal override void ProcessFrame(VideoFrameWrapper frame, LastTrackedPosition locatedGuidingStar)
		{
			// This is the idle state. Nothing to do.
		}
	}
}
