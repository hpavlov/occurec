/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OccuRec.ASCOM.Interfaces.Devices;

namespace OccuRec.ASCOM.Interfaces
{
	public interface IASCOMHelper
	{
		string ChooseTelescope();
		string ChooseFocuser();
	    string ChooseVideo();
	    void ConfigureFocuser(string progId);
        void ConfigureTelescope(string progId);
        void ConfigureVideo(string progId);
		IASCOMFocuser CreateFocuser(string progId);
        IASCOMTelescope CreateTelescope(string progId);
		IASCOMVideo CreateVideo(string progId);
		void ReleaseDevice(Guid deviceId);
	}
}
