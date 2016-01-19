using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ASCOM.DeviceInterface;
using ASCOM.DriverAccess;
using ASCOM.Utilities.Video;

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
		private long m_LastVideoFrameNumber;
		private double m_LastVideoFrameExposureDuration;
		private string m_LastVideoFrameStartTime;
		private object m_SyncLock = new object();
		private bool m_IsRecording;

		public CCDVideoImpl(string cameraId)
		{
			m_CCDCamera = new Camera(cameraId);
			m_Running = true;
			m_IsRecording = false;
			m_Exposure = null;
			m_LastVideoFrame = null;
			m_LastVideoFrameNumber = 0;
			m_LastVideoFrameExposureDuration = 0;
			m_LastVideoFrameStartTime = null;
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

		private CCDVideoFrame m_LastRetrievedVideoFrame = null;
		private CameraImage m_CameraImageHelper = new CameraImage();

		public IVideoFrame GetLastVideoFrame()
		{
			CCDVideoFrame rv = null;
			bool buildPreviewFrame = false;

			lock (m_SyncLock)
			{
				if (m_LastRetrievedVideoFrame != null && m_LastRetrievedVideoFrame.FrameNumber == m_LastVideoFrameNumber)
				{
					// No new frames yet so return the last one
					rv = m_LastRetrievedVideoFrame;
				}
				else if (m_LastVideoFrame != null)
				{
					rv = new CCDVideoFrame();
					rv.ImageArray = m_LastVideoFrame;
					rv.FrameNumber = m_LastVideoFrameNumber;
					rv.ExposureDuration = m_LastVideoFrameExposureDuration;
					rv.ExposureStartTime = m_LastVideoFrameStartTime;
					buildPreviewFrame = true;
				}
			}

			if (buildPreviewFrame)
			{
				int height = ((int[,])rv.ImageArray).GetLength(0);
				int width = ((int[,])rv.ImageArray).GetLength(1);

				// Simulate a star at position (100, 100) for debugging purposes
				for (int x = -10; x < 11; x++)
				for (int y = -10; y < 11; y++)
				{
					double dVal = 180 * Math.Exp(-(x*x + y*y)/8.0);
					((int[,])rv.ImageArray)[100 + x, 100 + y] += (int)dVal;
				}
				m_CameraImageHelper.SetImageArray((int[,])rv.ImageArray, width, height, SensorType.Monochrome);
				rv.PreviewBitmap = m_CameraImageHelper.GetDisplayBitmapBytes();

				m_LastRetrievedVideoFrame = rv;
			}

			return rv;
		}

		private void FramePumpThread(object state)
		{
			double lastTriggeredExposure;

			while (m_Running)
			{
				if (m_Connected && m_Exposure != null)
				{
					try
					{
						// Wait for the CCD camera to become ready if currently exposing
						while (m_CCDCamera.CameraState == CameraStates.cameraExposing)
							Thread.Sleep(5);

						object lastImageArray = null;
						string lastStartTime = null;
						double lastExposureDuration = 0;

						if (m_CCDCamera.ImageReady)
						{
							// TODO: Take NTP Time

							lastStartTime = m_CCDCamera.LastExposureStartTime;
							lastExposureDuration = m_CCDCamera.LastExposureDuration;

							// Download the CCD image if there is one
							lastImageArray = m_CCDCamera.ImageArray;
						}

						// Triger new exposure
						lastTriggeredExposure = m_Exposure.Value;
						m_CCDCamera.StartExposure(lastTriggeredExposure, true);

						if (lastImageArray != null)
						{
							lock (m_SyncLock)
							{
								// Set the last video frame if there is one
								m_LastVideoFrame = (int[,])lastImageArray;
								m_LastVideoFrameNumber++;

								m_LastVideoFrameExposureDuration = lastExposureDuration;
								m_LastVideoFrameStartTime = lastStartTime;
							}
						}

						if (m_IsRecording)
						{
							// TODO: If in recording state then save the current frame if there is a new one
						}
					}
					catch (DriverException dex)
					{
						// TODO: Prepare a frame with the error message displayed
						Trace.WriteLine(dex);
					}
				}

				Thread.Sleep(1);
			}
		}
	}
}
