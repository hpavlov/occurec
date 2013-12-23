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
		IASCOMFocuser CreateFocuser(string progId);
        IASCOMTelescope CreateTelescope(string progId);
	}
}
