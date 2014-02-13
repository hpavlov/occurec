using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Lifetime;
using System.Runtime.Remoting.Services;
using System.Security;
using System.Security.Policy;
using System.Text;
using OccuRec.ASCOM.Wrapper.Devices;
using OccuRec.ASCOM.Wrapper.Interfaces;
using OccuRec.ASCOM.Interfaces;
using OccuRec.ASCOM.Interfaces.Devices;

namespace OccuRec.ASCOM.Wrapper
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
        private static TraceSwitch TraceSwitchASCOMClient = new TraceSwitch("ASCOMClient", "ASCOMServer tracing.");

		public static ASCOMClient Instance = new ASCOMClient();

		internal readonly List<DeviceClient> DeviceClients = new List<DeviceClient>();

		private ASCOMHelper m_ASCOMHelper;

		internal RemotingClientSponsor RemotingClientSponsor = new RemotingClientSponsor();

		public void Initialise(bool useIsolationMode)
		{
            if (TraceSwitchASCOMClient.TraceVerbose)
                Trace.WriteLine("OccuRec: ASCOMClient::Initialise()");

			TrackingServices.RegisterTrackingHandler(new TrackingHandler());

			m_ASCOMHelper = new ASCOMHelper(this, useIsolationMode);
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
            if (TraceSwitchASCOMClient.TraceVerbose)
                Trace.WriteLine("OccuRec: ASCOMClient::ChooseFocuser()");

			return m_ASCOMHelper.ChooseFocuser();
		}

		public string ChooseTelescope()
		{
            if (TraceSwitchASCOMClient.TraceVerbose)
                Trace.WriteLine("OccuRec: ASCOMClient::ChooseTelescope()");

			return m_ASCOMHelper.ChooseTelescope();
		}

        public void ConfigureFocuser(string progId)
        {
            if (TraceSwitchASCOMClient.TraceVerbose)
                Trace.WriteLine("OccuRec: ASCOMClient::ConfigureFocuser()");

            m_ASCOMHelper.ConfigureFocuser(progId);
        }

        public void ConfigureTelescope(string progId)
        {
            if (TraceSwitchASCOMClient.TraceVerbose)
                Trace.WriteLine("OccuRec: ASCOMClient::ConfigureTelescope()");

            m_ASCOMHelper.ConfigureTelescope(progId);
        }

        public IFocuser CreateFocuser(string progId, int largeStepSize = -1, int smallStepSize = -1, int smallestStepSize = -1)
		{
            if (TraceSwitchASCOMClient.TraceVerbose)
                Trace.WriteLine(string.Format("OccuRec: ASCOMClient::CreateFocuser('{0}')", progId));

			IASCOMFocuser isolatedFocuser = m_ASCOMHelper.CreateFocuser(progId);
			RegisterLifetimeService(isolatedFocuser as MarshalByRefObject);

            return new Focuser(isolatedFocuser, largeStepSize, smallStepSize, smallestStepSize);
		}

        public ITelescope CreateTelescope(string progId, float slowestRate = float.NaN, float slowRate = float.NaN, float fastRate = float.NaN)
		{
            if (TraceSwitchASCOMClient.TraceVerbose)
                Trace.WriteLine(string.Format("OccuRec: ASCOMClient::CreateTelescope('{0}')", progId));

            IASCOMTelescope isolatedTelescope = m_ASCOMHelper.CreateTelescope(progId);
            RegisterLifetimeService(isolatedTelescope as MarshalByRefObject);

            return new Telescope(isolatedTelescope, slowestRate, slowRate, fastRate);
		}

		public IVideo CreateVideo(string progId)
		{
            if (TraceSwitchASCOMClient.TraceVerbose)
				Trace.WriteLine(string.Format("OccuRec: ASCOMClient::CreateVideo('{0}')", progId));

            IASCOMVideo isolatedVideo = m_ASCOMHelper.CreateVideo(progId);
			RegisterLifetimeService(isolatedVideo as MarshalByRefObject);

			return new Video(isolatedVideo);
		}
		

        public void DisconnectTelescope(IASCOMTelescope telescope)
        {
            try
            {
                if (TraceSwitchASCOMClient.TraceVerbose)
                    Trace.WriteLine(string.Format("OccuRec: ASCOMClient::DisconnectTelescope('{0}')", telescope.UniqueId));

                if (telescope.Connected)
                    telescope.Connected = false;
            }
            catch (Exception ex)
            {
                if (TraceSwitchASCOMClient.TraceError)
                    Trace.WriteLine(ex);
            }

            ReleaseDevice(telescope);
        }

        public void DisconnectFocuser(IFocuser focuser)
        {
            try
            {
                if (TraceSwitchASCOMClient.TraceVerbose)
                    Trace.WriteLine(string.Format("OccuRec: ASCOMClient::DisconnectFocuser('{0}')", focuser.UniqueId));

                if (focuser.Connected)
                    focuser.Connected = false;
            }
            catch (Exception ex)
            {
                if (TraceSwitchASCOMClient.TraceError)
                    Trace.WriteLine(ex);
            }

            ReleaseDevice(focuser);
        }

		public void DisconnectVideo(IVideo video)
		{
			try
			{
				if (TraceSwitchASCOMClient.TraceVerbose)
					Trace.WriteLine(string.Format("OccuRec: ASCOMClient::DisconnectVideo('{0}')", video.UniqueId));

				if (video.Connected)
					video.Connected = false;
			}
			catch (Exception ex)
			{
				if (TraceSwitchASCOMClient.TraceError)
					Trace.WriteLine(ex);
			}

			ReleaseDevice(video);
		}

        public void ReleaseDevice(object deviceInstance)
        {
	        DeviceBase device = deviceInstance as DeviceBase;
			if (device != null)
			{
                if (TraceSwitchASCOMClient.TraceVerbose)
                    Trace.WriteLine(string.Format("OccuRec: ASCOMClient::ReleaseDevice('{0}')", device.UniqueId));

				m_ASCOMHelper.ReleaseDevice(device.UniqueId);
			}
			else
			{
                if (TraceSwitchASCOMClient.TraceVerbose)
                    Trace.WriteLine("OccuRec: ASCOMClient::ReleaseDevice(ALL)");

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
            if (TraceSwitchASCOMClient.TraceVerbose)
                Trace.WriteLine("OccuRec: ASCOMClient::Dispose()");

			foreach (DeviceClient client in DeviceClients)
			{
				try
				{
					client.Dispose();
				}
				catch (Exception ex)
				{
                    if (TraceSwitchASCOMClient.TraceError)
					    Trace.WriteLine(ex);
				}
			}

            if (m_ASCOMHelper != null)
                m_ASCOMHelper.Dispose();
		}
	}
}
