using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OccuRec.Context
{
	public class OccuRecContext
	{
		public static OccuRecContext Current = new OccuRecContext();

		public bool IsAAV { get; set; }
		public bool IsConnected { get; set; }
	}
}
