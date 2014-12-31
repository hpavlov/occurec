/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OccuRec.ASCOM.Interfaces.Devices
{
    [Serializable]
    public class FocuserPosition
    {
        public bool Absolute;
        public int Position;    
    }

	[Serializable]
	public class FocuserState
	{
		public bool TempCompAvailable;

		public double Temperature;

		public bool IsMoving;

		public bool Absolute;

		public int MaxIncrement;

		public double MaxStep;

		public double StepSize;

		public bool TempComp;

		public int Position;
	}

    public interface IASCOMFocuser : IASCOMDevice
    {
	    FocuserState GetCurrentState();
        FocuserPosition GetCurrentPosition();
	    void Move(int position);
        bool ChangeTempComp(bool tempComp);
    }
}
