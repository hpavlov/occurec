using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OccuRec.ASCOM.Interfaces
{
	public interface IASCOMHelper
	{
		string ChooseTelescope();
		string ChooseFocuser();
		IASCOMFocuser CreateFocuser(string progId);
	}
}
