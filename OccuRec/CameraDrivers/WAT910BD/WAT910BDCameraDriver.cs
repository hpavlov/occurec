using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OccuRec.CameraDrivers.WAT910BD
{
	internal class WAT910BDCameraDriver : IOccuRecCameraController
	{
		internal static string CAMERA_NAME = "WAT-910BD";
		internal static string DRIVER_NAME = "WAT-910BD Nano Driver";

		internal const string PROP_COM_PORT = "COM-PORT";

		public IVideoDriverSettings Configuration { get; set; }

		public bool Connected
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public string Description
		{
			get { return DRIVER_NAME; }
		}

		public string DriverName
		{
			get { return DRIVER_NAME; }
		}

		public ASCOM.Interfaces.Devices.VideoState GetCurrentState()
		{
			throw new NotImplementedException();
		}

		public bool RequiresConfiguration 
		{
			get { return true; }
		}

		public bool IsConfigured
		{
			get
			{
				return
					Configuration != null &&
					!string.IsNullOrEmpty(Configuration.GetProperty(PROP_COM_PORT));
			}
		}

		public bool ConfigureConnectionSettings(IWin32Window parent)
		{
			var frm = new frmWAT910BDConnectionSettings();
			frm.DefaultComPort = Configuration.GetProperty(PROP_COM_PORT);
			frm.StartPosition = FormStartPosition.CenterParent;
			if (frm.ShowDialog(parent) == DialogResult.OK)
			{
				Configuration.SetProperty(PROP_COM_PORT, frm.DefaultComPort);
				return true;
			}

			return false;
		}

		public override string ToString()
		{
			return DRIVER_NAME;
		}
	}
}
