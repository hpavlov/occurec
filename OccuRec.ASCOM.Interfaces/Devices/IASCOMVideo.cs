using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OccuRec.ASCOM.Interfaces.Devices
{
	[Serializable]
	public class VideoState
	{
		public int MinGainIndex { get; set; }
		public int MaxGainIndex { get; set; }
		public int MinGammaIndex { get; set; }
		public int MaxGammaIndex { get; set; }
		public int MinExposureIndex { get; set; }
		public int MaxExposureIndex { get; set; }

		public int GainIndex { get; set; }
		public int GammaIndex  { get; set; }
		public int ExposureIndex { get; set; }
	}

	public interface IASCOMVideo : IASCOMDevice
	{
		VideoState GetCurrentState();
	}
}
