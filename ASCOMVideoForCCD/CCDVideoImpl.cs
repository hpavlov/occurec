using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ASCOM.DeviceInterface;
using ASCOM.DriverAccess;

namespace ASCOM.GenericCCDCamera
{
	internal class CCDVideoImpl : IDisposable
	{
		private Camera m_CCDCamera;
		private Thread m_FramePump;
		private bool m_Running;
		private bool m_Connected;
		private double? m_Exposure;
		private int[,] m_LastVideoFrame;
		private object m_SyncLock = new object();
		private bool m_IsRecording;

		public CCDVideoImpl(string cameraId)
		{
			m_CCDCamera = new Camera(cameraId);
			m_Running = true;
			m_IsRecording = false;
			m_Exposure = null;
			m_LastVideoFrame = null;
			m_Connected = m_CCDCamera.Connected;

			if (m_Connected)
				SetDefaultIntegrationRate();

			m_FramePump = new Thread(FramePumpThread);
			m_FramePump.Start();
			CameraState = VideoCameraState.videoCameraRunning;
		}

		public void Dispose()
		{
			m_Running = false;
			m_FramePump.Join(TimeSpan.FromMinutes(1));
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
					m_Connected = true;
					SetDefaultIntegrationRate();
				}
				else
				{
					m_CCDCamera.Connected = false;
					m_Connected = false;
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

		private ArrayList m_SupportedIntegrationRates = null;
		private List<double> m_SupportedIntegrationExposures = null;

		public ArrayList SupportedIntegrationRates
		{
			get
			{
				BuildSupportedIntegrationRates();
				return m_SupportedIntegrationRates;
			}
		}

		private void BuildSupportedIntegrationRates()
		{
			if (m_SupportedIntegrationRates == null)
			{
				lock (m_SyncLock)
				{
					if (m_SupportedIntegrationRates == null)
					{
						m_SupportedIntegrationRates = new ArrayList();
						m_SupportedIntegrationExposures = new List<double>();

						double expMin = ExposureMin;
						double expMax = ExposureMax;

						if (expMin < 1 && expMax > 1)
						{
							double[] smallExp = new[] { 0.5, 0.25, 0.13, 0.06, 0.03, 0.01 };
							for (int i = 0; i < smallExp.Length; i++)
							{
								if (smallExp[i] > expMin)
								{
									m_SupportedIntegrationExposures.Insert(0, smallExp[i]);
									m_SupportedIntegrationRates.Insert(0, smallExp[i].ToString("0.00"));
								}
							}

							string minExpStr = expMin.ToString("0.00");
							double minExpTrunc = double.Parse(minExpStr);
							if (m_SupportedIntegrationExposures.Count == 0 || minExpTrunc < m_SupportedIntegrationExposures[0])
							{
								m_SupportedIntegrationExposures.Insert(0, expMin);
								m_SupportedIntegrationRates.Insert(0, minExpStr);
							}
						}

						for (int i = 1; i <= 10; i++)
						{
							if (i > expMin && i < expMax)
							{
								m_SupportedIntegrationExposures.Add(i);
								m_SupportedIntegrationRates.Add(i.ToString());
							}
						}

						int[] largeExp = new[] { 15, 20, 25, 30 };
						for (int i = 0; i < largeExp.Length; i++)
						{
							if (largeExp[i] <= expMax)
							{
								m_SupportedIntegrationExposures.Add(largeExp[i]);
								m_SupportedIntegrationRates.Add(largeExp[i].ToString());
							}
						}
					}
				}
			}
		}

		private void SetDefaultIntegrationRate()
		{
			if (m_Exposure != null) return;

			BuildSupportedIntegrationRates();
			for (int i = 0; i < m_SupportedIntegrationExposures.Count; i++)
			{
				if (Math.Abs(m_SupportedIntegrationExposures[i] - 1) < 0.1)
				{
					IntegrationRate = i;
					break;
				}
			}

			if (m_Exposure == null && m_SupportedIntegrationExposures.Count > 0)
				IntegrationRate = 0;
		}

		private int m_IntegrationRate = -1;
		public int IntegrationRate
		{
			get
			{
				return m_IntegrationRate;
			}
			set
			{
				lock (m_SyncLock)
				{
					if (value >= 0 && value < m_SupportedIntegrationExposures.Count)
					{
						m_IntegrationRate = value;
						m_Exposure = m_SupportedIntegrationExposures[m_IntegrationRate];
					}
					else
						throw new DriverException(string.Format("Integration rate must be between {0} and {1}", 0, m_SupportedIntegrationExposures.Count - 1));
				}
			}
		}

		public string CCDDeviceName
		{
			get { return m_CCDCamera.Description; }	
		}

		public VideoCameraState CameraState { get; private set; }

		public IVideoFrame GetLastVideoFrame()
		{
			CCDVideoFrame rv = null;
			lock (m_SyncLock)
			{
				if (m_LastVideoFrame != null)
				{
					// TODO:
				}
			}

			return rv;
		}

		private void FramePumpThread(object state)
		{
			while (m_Running)
			{
				if (m_Connected && m_Exposure != null)
				{
					// TODO: Wait for the CCD camera to become ready
					
					// TODO: Download the CCD image if there is one

					// TODO: Take NTP Time
					// TODO: Triger new exposure
					lock (m_SyncLock)
					{
						// TODO: Set the last video frame if there is one
					}

					if (m_IsRecording)
					{
						// TODO: If in recording state then save the current frame if there is a new one
					}
				}

				Thread.Sleep(1);
			}
		}
	}
}
