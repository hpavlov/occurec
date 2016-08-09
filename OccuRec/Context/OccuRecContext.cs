/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

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
        public bool IsQHY { get; set; }
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

		public int SecondsRemainingToShowAssumedVtiOsdPosition
		{
			get
			{
				int val = 0;
				if (ShowAssumedVtiOsdPositionUntil.HasValue)
				{
					val = (int)Math.Round(new TimeSpan(ShowAssumedVtiOsdPositionUntil.Value.Ticks - DateTime.Now.Ticks).TotalSeconds + 0.5);
					if (val <= 0) val = 1;
				}

				return val;
			}
		}
	}
}
