using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using ASCOM;
using OccuRec.ASCOM.Interfaces;
using OccuRec.ASCOM.Interfaces.Devices;
using OccuRec.ASCOM.Interfaces.System;
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
        private static string VIDEO_DEVICE_TYPE = "Video";

		private List<IsolatedDevice> m_ReferencedObjects = new List<IsolatedDevice>(); 

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

        public string ChooseVideo()
        {
            var chooser = new ASCOMUtilities.Chooser();
            chooser.DeviceType = VIDEO_DEVICE_TYPE;
            string progId = chooser.Choose(null);

            return progId;
        }

		public IASCOMFocuser CreateFocuser(string progId)
		{
			var focuser = new IsolatedFocuser(progId);
			m_ReferencedObjects.Add(focuser);
			return focuser;
		}

        public IASCOMTelescope CreateTelescope(string progId)
        {
            var telescope = new IsolatedTelescope(progId);
            m_ReferencedObjects.Add(telescope);
            return telescope;
        }

		public IASCOMVideo CreateVideo(string progId)
		{
			var video = new IsolatedVideo(progId);
			m_ReferencedObjects.Add(video);
			return video;
		}

        public void ConfigureFocuser(string progId)
        {
            using (var focuser = new IsolatedFocuser(progId))
            {
                focuser.Configure();
            }
        }

        public void ConfigureTelescope(string progId)
        {
            using (var telescope = new IsolatedTelescope(progId))
            {
                telescope.Configure();
            }
        }

        public void ConfigureVideo(string progId)
        {
            using (var video = new IsolatedVideo(progId))
            {
                video.Configure();
            }
        }

		public void Initialise(IOccuRecHost host)
		{
			RemotingConfiguration.RegisterWellKnownServiceType(typeof(ASCOMHelper), "OccuRec.ASCOM.Server.Helper", WellKnownObjectMode.Singleton);
		}

		public void ReleaseDevice(Guid deviceId)
		{
			IsolatedDevice device = m_ReferencedObjects.SingleOrDefault(x => x.UniqueId == deviceId);
			if (device != null)
			{
				device.Dispose();
				m_ReferencedObjects.Remove(device);
			}
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
