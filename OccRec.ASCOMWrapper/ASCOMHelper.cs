﻿/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OccuRec.ASCOM.Interfaces;
using OccuRec.ASCOM.Interfaces.Devices;

namespace OccuRec.ASCOM.Wrapper
{
	[Serializable]
    internal class ASCOMHelper : DeviceClient, IASCOMHelper
	{
		private IASCOMHelper m_IsolatedHelper;
		public ASCOMHelper(ASCOMClient client, bool loadIsolated)
		{
			if (loadIsolated)
				LoadInAppDomain("OccuRec.ASCOM.Server.ASCOMHelper, OccuRec.ASCOM.Server", client);
			else
				LoadInCurrentDomain("OccuRec.ASCOM.Server.ASCOMHelper, OccuRec.ASCOM.Server", client);

			m_IsolatedHelper = m_Instance as IASCOMHelper;
		}

		public string ChooseFocuser()
		{
			return m_IsolatedHelper.ChooseFocuser();
		}

		public string ChooseTelescope()
		{
			return m_IsolatedHelper.ChooseTelescope();
		}

        public string ChooseVideo()
        {
            return m_IsolatedHelper.ChooseVideo();
        }

		public IASCOMFocuser CreateFocuser(string progId)
		{
			return m_IsolatedHelper.CreateFocuser(progId);
		}

        public IASCOMTelescope CreateTelescope(string progId)
        {
            return m_IsolatedHelper.CreateTelescope(progId);
        }

		public IASCOMVideo CreateVideo(string progId)
        {
			return m_IsolatedHelper.CreateVideo(progId);
        }

		public void ReleaseDevice(Guid deviceId)
		{
			m_IsolatedHelper.ReleaseDevice(deviceId);
		}

        public void ConfigureFocuser(string progId)
        {
            m_IsolatedHelper.ConfigureFocuser(progId);
        }

        public void ConfigureTelescope(string progId)
        {
            m_IsolatedHelper.ConfigureTelescope(progId);
        }

        public void ConfigureVideo(string progId)
        {
            m_IsolatedHelper.ConfigureVideo(progId);
        }
	}
}
