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


		
		public DateTime? ShowAssumedVtiOsdPositionUntil { get; set; }

		public bool ShowAssumedVtiOsdPosition
		{
			get
			{
				bool show = ShowAssumedVtiOsdPositionUntil.HasValue && ShowAssumedVtiOsdPositionUntil.Value > DateTime.Now;

				if (ShowAssumedVtiOsdPositionUntil.HasValue && ShowAssumedVtiOsdPositionUntil.Value < DateTime.Now)
				{
					ShowAssumedVtiOsdPositionUntil = null;
				}

				return show;
			}
		}
	}
}
