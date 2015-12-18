using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASCOM.DeviceInterface;
using ASCOM.DriverAccess;

namespace ASCOM.GenericCCDCamera
{
	internal class CCDVideoImpl : IDisposable
	{
		private Camera m_CCDCamera;

		public CCDVideoImpl(string cameraId)
		{
			m_CCDCamera = new Camera(cameraId);
		}

		public void Dispose()
		{
			throw new System.NotImplementedException();
		}

		public bool Connected
		{
			get
			{
				return m_CCDCamera.Connected;
			}
			set
			{
				if (value == m_CCDCamera.Connected)
					return;

				if (value)
				{
					m_CCDCamera.Connected = true;
				}
				else
				{
					m_CCDCamera.Connected = false;
				}
			}
		}

		public double ExposureMax
		{
			get
			{
				return m_CCDCamera.ExposureMax;
			}
		}

		public double ExposureMin
		{
			get
			{
				return m_CCDCamera.ExposureMin;
			}
		}

		public IVideoFrame GetLastVideoFrame()
		{
			return null;
		}
	}
}
