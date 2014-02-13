using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OccuRec.ASCOM.Interfaces.Devices;

namespace OccuRec.CameraDrivers
{
	public interface IOccuRecCameraController
	{
		bool Connected { get; set; }
		string Description { get; }
		VideoState GetCurrentState();
		bool RequiresConfiguration { get; }
		void ConfigureConnectionSettings(IWin32Window parent);
	}
}
