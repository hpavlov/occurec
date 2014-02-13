using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OccuRec.CameraDrivers
{
	public interface ICameraController
	{
		bool Connected { get; set; }
	}
}
