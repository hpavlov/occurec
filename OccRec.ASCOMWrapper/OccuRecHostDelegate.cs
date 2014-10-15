/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OccuRec.ASCOM.Interfaces;
using OccuRec.ASCOM.Interfaces.System;

namespace OccuRec.ASCOM.Wrapper
{
	[Serializable]
	public class OccuRecHostDelegate : IOccuRecHost
	{
		private string m_TypeName;
		private ASCOMClient m_AscomClient;

		public OccuRecHostDelegate(string typeName, ASCOMClient ascomClient)
		{
			m_AscomClient = ascomClient;
			m_TypeName = typeName;
		}
	}
}
