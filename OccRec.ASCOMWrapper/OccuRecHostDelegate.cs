using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OccuRec.ASCOM.Interfaces;

namespace OccRec.ASCOMWrapper
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
