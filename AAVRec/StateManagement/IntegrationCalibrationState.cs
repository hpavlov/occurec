using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AAVRec.StateManagement
{
	public class IntegrationCalibrationState : CameraState
	{
		public static IntegrationCalibrationState Instance = new IntegrationCalibrationState();

		private IntegrationCalibrationState()
        { }

        public override void InitialiseState()
        {
        }

		public override void ProcessFrame(CameraStateManager stateManager, Helpers.VideoFrameWrapper frame)
		{
			base.ProcessFrame(stateManager, frame);

		}
	}
}
