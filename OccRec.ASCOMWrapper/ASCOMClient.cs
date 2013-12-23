using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Lifetime;
using System.Runtime.Remoting.Services;
using System.Security;
using System.Security.Policy;
using System.Text;
using OccRec.ASCOMWrapper.Devices;
using OccuRec.ASCOM.Interfaces;
using OccuRec.ASCOM.Interfaces.Devices;

namespace OccRec.ASCOMWrapper
{
	internal class RemotingClientSponsor : MarshalByRefObject, ISponsor
	{
		public TimeSpan Renewal(ILease lease)
		{
			TimeSpan tsLease = TimeSpan.FromMinutes(5);
			lease.Renew(tsLease);
			return tsLease;
		}

		public override object InitializeLifetimeService()
		{
			return null;
		}
	}

	[Serializable]
	public class ASCOMClient : IDisposable
	{
		public static ASCOMClient Instance = new ASCOMClient();

		internal readonly List<DeviceClient> DeviceClients = new List<DeviceClient>();

		private ASCOMHelper m_ASCOMHelper;

		internal RemotingClientSponsor RemotingClientSponsor = new RemotingClientSponsor();

		public void Initialise()
		{
			TrackingServices.RegisterTrackingHandler(new TrackingHandler());

			m_ASCOMHelper = new ASCOMHelper(this);
			DeviceClients.Add(m_ASCOMHelper);			
		}


		internal void RegisterLifetimeService(MarshalByRefObject obj)
		{
			ILease lease = (ILease)obj.GetLifetimeService();
			if (lease != null)
				lease.Register(RemotingClientSponsor);
		}

		public string ChooseFocuser()
		{
			return m_ASCOMHelper.ChooseFocuser();
		}

		public string ChooseTelescope()
		{
			return m_ASCOMHelper.ChooseTelescope();
		}

		public IASCOMFocuser CreateFocuser(string progId)
		{
			IASCOMFocuser isolatedFocuser = m_ASCOMHelper.CreateFocuser(progId);
			RegisterLifetimeService(isolatedFocuser as MarshalByRefObject);

			return new Focuser(isolatedFocuser);
		}

		public IASCOMTelescope CreateTelescope(string progId)
		{
            IASCOMTelescope isolatedTelescope = m_ASCOMHelper.CreateTelescope(progId);
            RegisterLifetimeService(isolatedTelescope as MarshalByRefObject);

            return new Telescope(isolatedTelescope);
		}

        public void DisconnectTelescope(IASCOMTelescope telescope)
        {
            try
            {
                if (telescope.Connected)
                    telescope.Connected = false;
            }
            catch
            { }

            ReleaseDevice(telescope);
        }

        public void DisconnectFocuser(IASCOMFocuser fpcuser)
        {
            try
            {
                if (fpcuser.Connected)
                    fpcuser.Connected = false;
            }
            catch
            { }

            ReleaseDevice(fpcuser);
        }

        public void ReleaseDevice(object deviceInstance)
        {
	        DeviceBase device = deviceInstance as DeviceBase;
			if (device != null)
			{
				m_ASCOMHelper.ReleaseDevice(device.UniqueId);
			}
			else
			{
				foreach (DeviceClient client in DeviceClients)
				{
					if (object.ReferenceEquals(client, deviceInstance))
					{
						DeviceClients.Remove(client);
						client.Dispose();
					}
				}
			}
        }

		public void Dispose()
		{
			foreach (DeviceClient client in DeviceClients)
			{
				try
				{
					client.Dispose();
				}
				catch (Exception ex)
				{
					Trace.WriteLine(ex);
				}
			}
		}
	}
}
