using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OccuRec.ASCOM.Interfaces.Devices
{
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
	    void Move(int position);
        bool ChangeTempComp(bool tempComp);
    }
}
