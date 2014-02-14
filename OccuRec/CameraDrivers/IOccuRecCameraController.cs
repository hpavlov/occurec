using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OccuRec.ASCOM.Interfaces.Devices;

namespace OccuRec.CameraDrivers
{
	public interface IOccuRecCameraController : IDisposable
	{
		IVideoDriverSettings Configuration { get; set; }
		bool Connected { get; set; }
		string DriverName { get; }
		string Description { get; }
		VideoState GetCurrentState();
		bool RequiresConfiguration { get; }
		bool IsConfigured { get; }
		bool ConfigureConnectionSettings(IWin32Window parent);
		void OSDUp();
		void OSDDown();
		void OSDLeft();
		void OSDRight();
		void OSDSet();
		void GammaUp();
		void GammaDown();
		void GainUp();
		void GainDown();
		void ExposureUp();
		void ExposureDown();
	}
}
