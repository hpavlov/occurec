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
			get { throw new NotImplementedException(); }
		}

		public ASCOM.Interfaces.Devices.VideoState GetCurrentState()
		{
			throw new NotImplementedException();
		}

		public bool RequiresConfiguration 
		{
			get { return true; }
		}

		public void ConfigureConnectionSettings(IWin32Window parent)
		{
			var frm = new frmWAT910BDConnectionSettings();
			frm.ShowDialog(parent);
		}

		public override string ToString()
		{
			return DRIVER_NAME;
		}
	}
}
