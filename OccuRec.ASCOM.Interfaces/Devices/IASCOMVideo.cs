using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OccuRec.ASCOM.Interfaces.Devices
{
	[Serializable]
	public class VideoState
	{
		
	}

	public interface IASCOMVideo : IASCOMDevice
	{
		VideoState GetCurrentState();
	}
}
