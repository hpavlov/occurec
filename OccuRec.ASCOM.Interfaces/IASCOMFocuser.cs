using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OccuRec.ASCOM.Interfaces
{
	public interface IASCOMFocuser
	{
		bool Connected { get; set; }
		string Description { get; }
		string DriverVersion { get; }
	}
}
