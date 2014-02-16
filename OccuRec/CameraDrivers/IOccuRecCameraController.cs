using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OccuRec.ASCOM.Interfaces.Devices;
using OccuRec.CameraDrivers.WAT910BD;

namespace OccuRec.CameraDrivers
{
    public delegate void DriverErrorCallback(DriverErrorEventArgs e);

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
		bool Supports5ButtonOSD { get; }
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
        event DriverErrorCallback OnError;
	}
}
