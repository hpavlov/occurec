using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using ASCOM;
using OccuRec.ASCOM.Interfaces;
using NotImplementedException = System.NotImplementedException;
using ASCOMUtilities = ASCOM.Utilities;
using ASCOMDriverAccess = ASCOM.DriverAccess;

namespace OccuRec.ASCOM.Server
{
	[Serializable]
	public class ASCOMHelper : MarshalByRefObject, IASCOMHelper, IIsolatedDevice
	{
		private static string TELESCOPE_DEVICE_TYPE = "Telescope";
		private static string FOCUSER_DEVICE_TYPE = "Focuser";

		private List<IDisposable> m_ReferencedObjects = new List<IDisposable>(); 

		public string ChooseFocuser()
		{
			var chooser = new ASCOMUtilities.Chooser();
			chooser.DeviceType = FOCUSER_DEVICE_TYPE;
			string progId = chooser.Choose(null);

			return progId;
		}

		public string ChooseTelescope()
		{
			var chooser = new ASCOMUtilities.Chooser();
			chooser.DeviceType = TELESCOPE_DEVICE_TYPE;
			string progId = chooser.Choose(null);

			return progId;
		}

		public IASCOMFocuser CreateFocuser(string progId)
		{
			var focuser = new IsolatedFocuser(progId);
			m_ReferencedObjects.Add(focuser);
			return focuser;
		}

		public void Initialise(IOccuRecHost host)
		{
			RemotingConfiguration.RegisterWellKnownServiceType(typeof(ASCOMHelper), "OccuRec.ASCOM.Server.Helper", WellKnownObjectMode.Singleton);
		}

		public void Finalise()
		{
			foreach (IDisposable referencedObj in m_ReferencedObjects)
			{
				referencedObj.Dispose();
			}
			m_ReferencedObjects.Clear();

			RemotingServices.Disconnect(this);
		}
	}
}
