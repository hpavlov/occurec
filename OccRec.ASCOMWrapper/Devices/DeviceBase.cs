using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OccuRec.ASCOM.Interfaces.Devices;

namespace OccRec.ASCOMWrapper.Devices
{
	internal class DeviceBase : IASCOMDevice
    {
		private IASCOMDevice m_IsolatedDevice;

		protected DeviceBase(IASCOMDevice isolatedDevice)
		{
			m_IsolatedDevice = isolatedDevice;
		}

		public bool Connected
		{
			get { return m_IsolatedDevice.Connected; }
			set { m_IsolatedDevice.Connected = value; }
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
