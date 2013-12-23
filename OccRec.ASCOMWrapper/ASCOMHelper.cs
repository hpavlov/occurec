using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OccuRec.ASCOM.Interfaces;

namespace OccRec.ASCOMWrapper
{
	[Serializable]
	internal class ASCOMHelper : DeviceClient
	{
		private IASCOMHelper m_IsolatedHelper;
		public ASCOMHelper(ASCOMClient client)
		{
			LoadInAppDomain("OccuRec.ASCOM.Server.ASCOMHelper, OccuRec.ASCOM.Server", client);
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

		public IASCOMFocuser CreateFocuser(string progId)
		{
			return m_IsolatedHelper.CreateFocuser(progId);
		}
	}
}
