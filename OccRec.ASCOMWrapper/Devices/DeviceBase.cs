using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OccuRec.ASCOM.Interfaces.Devices;

namespace OccuRec.ASCOM.Wrapper.Devices
{
    internal class DeviceBase : IASCOMDevice
    {
		private IASCOMDevice m_IsolatedDevice;

		protected DeviceBase(IASCOMDevice isolatedDevice)
		{
			m_IsolatedDevice = isolatedDevice;
		}

	    protected virtual void OnConnected() { }
        protected virtual void OnDisconnected() { }

	    public bool Connected
		{
			get
			{
			    return m_IsolatedDevice.Connected;
			}
			set
			{
			    m_IsolatedDevice.Connected = value;

			    if (m_IsolatedDevice.Connected)
			        OnConnected();
			    else
			        OnDisconnected();
			}
		}

		public string Description
		{
			get { return m_IsolatedDevice.Description; }
		}

		public string DriverVersion
		{
			get { return m_IsolatedDevice.DriverVersion; }
		}

		public string ProgId
		{
			get { return m_IsolatedDevice.ProgId; }
		}

		public Guid UniqueId
		{
			get { return m_IsolatedDevice.UniqueId; }
		}
    }
}
