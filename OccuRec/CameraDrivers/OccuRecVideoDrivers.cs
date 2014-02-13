using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OccuRec.CameraDrivers.WAT910BD;

namespace OccuRec.CameraDrivers
{
	public class OccuRecVideoDrivers
	{
		public static List<IOccuRecCameraController> GetAvailableDriversForCamera(string cameraName)
		{
			var rv = new List<IOccuRecCameraController>();

			if (cameraName == WAT910BDCameraDriver.CAMERA_NAME)
			{
				rv.Add(new WAT910BDCameraDriver());
			}

			return rv;
		}
	}
}
