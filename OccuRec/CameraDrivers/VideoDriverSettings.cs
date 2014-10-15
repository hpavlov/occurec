/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace OccuRec.CameraDrivers
{
	public class AllVideoDriverSettings
	{
		[XmlArrayItem("Driver")]
		public List<VideoDriverSettings> Drivers = new List<VideoDriverSettings>();
	}

	public interface IVideoDriverSettings
	{
		string GetProperty(string propName);
		void SetProperty(string propName, string propValue);
	}

	public class VideoDriverSettings : IVideoDriverSettings
	{
		public string DriverName { get; set; }

		[XmlArrayItem("Name")]
		public List<string> PropertyNames = new List<string>();

		[XmlArrayItem("Value")]
		public List<string> PropertyValues = new List<string>();

		public string GetProperty(string propName)
		{
			int idx = PropertyNames.IndexOf(propName);
			if (idx > -1)
				return PropertyValues[idx];

			return null;
		}

		public void SetProperty(string propName, string propValue)
		{
			int idx = PropertyNames.IndexOf(propName);
			if (idx > -1)
				PropertyValues[idx] = propValue;
			else
			{
				PropertyNames.Add(propName);
				PropertyValues.Add(propValue);
			}
		}
	}
}
