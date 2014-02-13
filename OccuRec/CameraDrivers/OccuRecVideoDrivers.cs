using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OccuRec.CameraDrivers
{
	public class OccuRecVideoDrivers
	{
		public static List<string> GetAvailableDriversForCamera(string cameraName)
		{
			var rv = new List<string>();

			if (cameraName == "WAT-910BD")
			{
				rv.Add("WAT-910BD Nano Driver");
			}

			return rv;
		}

		public static ICameraController CreateDriverInstance(string driverName)
		{
			throw new NotImplementedException();
		}
	}
}
