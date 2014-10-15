/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OccuRec.StateManagement
{
	public class ExternallyManagedCameraState : CameraState
    {
		public static ExternallyManagedCameraState Instance = new ExternallyManagedCameraState();

		private ExternallyManagedCameraState()
        { }

        public override void ProcessFrame(CameraStateManager stateManager, Helpers.VideoFrameWrapper frame)
        {
            // Nothing to do
        }
    }
}
