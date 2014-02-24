using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using OccuRec.ASCOM;
using OccuRec.ASCOM.Interfaces.Devices;
using OccuRec.Astrometry.StarCatalogues;
using OccuRec.Helpers;
using OccuRec.Properties;
using OccuRec.Tracking;
using OccuRec.Utilities;

namespace OccuRec.FrameAnalysis
{
	internal class PlateSolveManager
	{
		private static double ASSUMED_MAX_PIXEL_SIZE_MICRONS = 10.0;

		private Thread m_PlateSolvingThread = null;
		private object m_SyncLock = new object();
		private bool m_Running = false;
		private bool m_WaitingForFrameToSolve = false;
		private bool m_TelescopePositionIsKnown = false;
		private bool m_TelescopeIsConnected = false;
		private bool m_FOVKnown = false;
		private int[,] m_CurrentFramePixels = null;
		private double m_RAHours;
		private double m_DEDegrees;
		private double m_BaseLimitingMagnitde;
		private double m_FocalLengthMillimeters;
		private double m_FieldOfViewDegrees;
		private float m_CurrentEpoch;

		private StarCatalogueFacade m_StarCatalogueFacade;
		private IObservatoryController m_ObservatoryController;

		public PlateSolveManager(IObservatoryController observatoryController)
		{
			observatoryController.TelescopeConnectionChanged += observatoryController_TelescopeConnectionChanged;
			observatoryController.TelescopeStateUpdated += observatoryController_TelescopeStateUpdated;
			observatoryController.TelescopeCapabilitiesKnown += observatoryController_TelescopeCapabilitiesKnown;

			m_CurrentEpoch = (float)(2000.0 + new TimeSpan(DateTime.UtcNow.Ticks - new DateTime(2000, 0, 0).Ticks).TotalDays / 365.25);

			m_PlateSolvingThread = new Thread(PlateSolvingBackgroundProcessing);
			m_PlateSolvingThread.Priority = ThreadPriority.Lowest;
			m_PlateSolvingThread.Start();
		}

		void observatoryController_TelescopeCapabilitiesKnown(TelescopeCapabilities capabilities)
		{
			m_FocalLengthMillimeters = capabilities.FocalLengthMeters / 1000.0;
			if (Settings.Default.FocalReducerUsed) m_FocalLengthMillimeters /= Settings.Default.FocalReducerValue;
			
			m_BaseLimitingMagnitde = 3 /* from video integration */ + 5 /* sky limiting mag */ + 5 * Math.Log10(capabilities.ApertureMeters / 10.0);

			m_TelescopeIsConnected = true;
			m_FOVKnown = false;
		}

		void observatoryController_TelescopeStateUpdated(TelescopeState state)
		{
			if (Math.Abs(m_RAHours - state.RightAscension)/240 > 2 || Math.Abs(m_DEDegrees - state.Declination)/3600 > 2)
			{
				m_RAHours = state.RightAscension;
				m_DEDegrees = state.Declination;

				m_TelescopePositionIsKnown = false;
			}
			else
			{
				m_TelescopePositionIsKnown = true;
			}
		}

		void observatoryController_TelescopeConnectionChanged(ASCOMConnectionState connectionState)
		{
			if (connectionState != ASCOMConnectionState.Connected)
			{
				m_TelescopeIsConnected = false;
				m_FOVKnown = false;
			}
		}

		public void ProcessFrame(VideoFrameWrapper frame, Bitmap bmp)
		{			
			lock (m_SyncLock)
			{
				if (m_WaitingForFrameToSolve && m_TelescopeIsConnected && m_TelescopePositionIsKnown)
				{
					var mvf = new MinimalVideoFrame(frame, bmp);
					m_CurrentFramePixels = mvf.ImageArray as int[,];

					if (!m_FOVKnown)
					{
						// http://www.wilmslowastro.com/software/formulae.htm
						// arc sec per pixel = pixel size [um] * 206.3 / focal length [mm]

						int width = m_CurrentFramePixels.GetLength(0);
						int height = m_CurrentFramePixels.GetLength(1);
						m_FieldOfViewDegrees = (Math.Sqrt(width * width + height * height) * 206.3 * ASSUMED_MAX_PIXEL_SIZE_MICRONS / m_FocalLengthMillimeters) / 3600;
					}

					m_WaitingForFrameToSolve = false;
				}					
			}
		}

		public void TelescopeFinishedSlewing(double raHours, double deDegrees)
		{
			m_RAHours = raHours;
			m_DEDegrees = deDegrees;
			m_TelescopePositionIsKnown = true;
		}

		public void TelescopeStartedSlewing()
		{
			m_TelescopePositionIsKnown = false;
		}

		private void PlateSolvingBackgroundProcessing(object state)
		{
			m_Running = true;
			m_WaitingForFrameToSolve = false;
			m_CurrentFramePixels = null;

			try
			{
				if (Settings.Default.StarCatalog != StarCatalog.NotSpecified &&
					Directory.Exists(Settings.Default.StarCatalogLocation))
				{
					m_StarCatalogueFacade = new StarCatalogueFacade(Settings.Default.StarCatalog, Settings.Default.StarCatalogLocation);
					string catLoc = Settings.Default.StarCatalogLocation;
					if (!m_StarCatalogueFacade.VerifyCurrentCatalogue(Settings.Default.StarCatalog, ref catLoc))
					{
						m_StarCatalogueFacade = null;
					}
				}

				while (m_Running)
				{
					int[,] framePixels = null;
					lock (m_SyncLock)
					{
						framePixels = m_CurrentFramePixels;
					}

					if (framePixels != null)
					{
						try
						{
							List<IStar> starsInFOV = m_StarCatalogueFacade.GetStarsInRegion(m_RAHours * 15, m_DEDegrees * 15, m_FieldOfViewDegrees * 2.5, m_BaseLimitingMagnitde, m_CurrentEpoch);

							// TODO: Need an algorithm to find locate candidate features (the StarMap alternative)
							//       *Make sure the lines configured for VTI OSD preservation are excluded in the star mapping process

							// TODO: Solve the plate. Should have a sensible timeout so this doesn't go in endless attempts
						}
						catch(Exception ex)
						{
							Trace.WriteLine(ex.GetFullStackTrace());
						}						
						finally
						{
							lock (m_SyncLock)
							{
								m_CurrentFramePixels = null;
								m_WaitingForFrameToSolve = true;
							}
						}
					}

					Thread.Sleep(100);
				}
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex.GetFullStackTrace());
				m_Running = false;
			}
		}

		internal void StopBackgroundProcesing()
		{
			if (m_PlateSolvingThread != null)
			{
				lock (m_SyncLock)
				{
					if (m_PlateSolvingThread != null)
					{
						m_Running = false;

						if (m_PlateSolvingThread.IsAlive)
							m_PlateSolvingThread.Join(1000);

						if (m_PlateSolvingThread.IsAlive)
							m_PlateSolvingThread.Abort();

						m_PlateSolvingThread = null;
					}
				}
			}
		}

	}
}
