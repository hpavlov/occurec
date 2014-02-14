using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using OccuRec.CameraDrivers.WAT910BD;
using OccuRec.Utilities;

namespace OccuRec.CameraDrivers
{
	public class OccuRecVideoDrivers
	{
		public static List<IOccuRecCameraController> GetAvailableDriversForCamera(string cameraName)
		{
			var rv = new List<IOccuRecCameraController>();

			if (cameraName == WAT910BDCameraController.CAMERA_NAME)
			{
				rv.Add(new WAT910BDCameraController());
			}

			return rv;
		}

		public static VideoDriverSettings GetDriverSettings(IOccuRecCameraController driver)
		{
			AllVideoDriverSettings allSet = GetCurrentSettings();
			VideoDriverSettings settings = allSet.Drivers.SingleOrDefault(x => x.DriverName == driver.DriverName);
			if (settings == null)
				settings = new VideoDriverSettings() { DriverName = driver.DriverName };

			return settings;
		}

		public static void SetDriverSettings(IOccuRecCameraController driver)
		{
			AllVideoDriverSettings allSet = GetCurrentSettings();
			VideoDriverSettings settings = allSet.Drivers.SingleOrDefault(x => x.DriverName == driver.DriverName);
			if (settings == null)
			{
				settings = new VideoDriverSettings() {DriverName = driver.DriverName};
				allSet.Drivers.Add(settings);
			}

			VideoDriverSettings newSettings = (VideoDriverSettings) driver.Configuration;

			settings.PropertyNames.Clear();
			settings.PropertyValues.Clear();

			for (int i = 0; i < newSettings.PropertyNames.Count; i++)
			{
				settings.PropertyNames.Add(newSettings.PropertyNames[i]);
				settings.PropertyValues.Add(newSettings.PropertyValues[i]);
			}

			SaveCurrentSettings(allSet);
		}

		private static AllVideoDriverSettings GetCurrentSettings()
		{
			var rv = new AllVideoDriverSettings();
			try
			{
				rv = Properties.Settings.Default.VideoDriversCommSettings.Deserialize<AllVideoDriverSettings>();
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex.GetFullStackTrace());
			}

			return rv;
		}

		private static void SaveCurrentSettings(AllVideoDriverSettings newSettings)
		{
			if (newSettings == null)
				Properties.Settings.Default.VideoDriversCommSettings = null;
			else
				Properties.Settings.Default.VideoDriversCommSettings = newSettings.Serialize().OuterXml;

			Properties.Settings.Default.Save();
		}
	}
}
