using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OccuRec.ASCOM.Interfaces
{
	public interface IIsolatedDevice
	{
		void Initialise(IOccuRecHost host);
		void Finalise();
	}
}
