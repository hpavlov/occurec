using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Lifetime;
using System.Runtime.Remoting.Services;
using System.Security;
using System.Security.Policy;
using System.Text;
using OccuRec.ASCOM.Interfaces;

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
			return null;
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
