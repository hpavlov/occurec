/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using OccuRec.ASCOM.Wrapper;
using OccuRec.ASCOM.Wrapper.Devices;
using OccuRec.ASCOM.Wrapper.Interfaces;
using OccuRec.ASCOM.Interfaces.Devices;
using OccuRec.CameraDrivers;
using OccuRec.Helpers;
using OccuRec.Properties;
using OccuRec.Utilities;
using OccuRec.CameraDrivers.WAT910BD;

namespace OccuRec.ASCOM
{
    public enum ASCOMConnectionState
    {
        Connecting,
        Connected,
        Disconnecting,
        Disconnected,
		Engaged,
		Ready,
        NotResponding,
        Errored
    }

	public enum CallType
	{
		Sync,
		Async
	}

	public class ObservatoryControllerCallbackArgs : EventArgs
	{
        public ObservatoryControllerCallbackArgs(object returnValue)
        {
            ReturnValue = returnValue;
        }

	    public object ReturnValue;
	}

	public delegate void CallbackAction(ObservatoryControllerCallbackArgs args);

	public class ObservatoryControlException : Exception
	{
		public ObservatoryControlException(string message)
			: base(message)
		{ }
	}

	public interface IObservatoryController : IDisposable
	{
        event Action<ASCOMConnectionState> TelescopeConnectionChanged;
        event Action<ASCOMConnectionState> FocuserConnectionChanged;
		event Action<ASCOMConnectionState> VideoConnectionChanged;
		event Action<TelescopeState> TelescopeStateUpdated;
	    event Action<TelescopeEquatorialPosition> TelescopePositionChanged;
		event Action<TelescopeCapabilities> TelescopeCapabilitiesKnown;
		event Action<FocuserState> FocuserStateUpdated;
        event Action<FocuserPosition> FocuserPositionUpdated;
		event Action<VideoState> VideoStateUpdated;
        event Action<string> VideoError;

		TelescopeEquatorialPosition CurrentTelescopePosition();
		VideoState CurrentVideoState();

		bool IsConnectedToObservatory();
		bool IsConnectedToTelescope();
		bool IsConnectedToFocuser();
		bool IsConnectedToVideoCamera();
		string ConnectedFocuserDriverName();
		string ConnectedTelescopeDriverName();
		string ConnectedVideoCameraDriverName();
		
		Guid? ControlTelescopeLock(bool isEngaged, Guid? clientId);
		Guid? ControlFocuserLock(bool isEngaged, Guid? clientId);

		void DisconnectTelescope(CallType callType = CallType.Async, Guid? clientId = null, CallbackAction callback = null, Control callbackUIControl = null);
		void DisconnectFocuser(CallType callType = CallType.Async, Guid? clientId = null, CallbackAction callback = null, Control callbackUIControl = null);
		void DisconnectVideoCamera(CallType callType = CallType.Async, Guid? clientId = null, CallbackAction callback = null, Control callbackUIControl = null);
		void TryConnectFocuser(CallType callType = CallType.Async, Guid? clientId = null, CallbackAction callback = null, Control callbackUIControl = null);
		void TryConnectTelescope(CallType callType = CallType.Async, Guid? clientId = null, CallbackAction callback = null, Control callbackUIControl = null);
		void TryConnectVideoCamera(CallType callType = CallType.Async, Guid? clientId = null, CallbackAction callback = null, Control callbackUIControl = null);
		void TelescopePulseGuide(GuideDirections direction, PulseRate pulseRate, CallType callType = CallType.Async, Guid? clientId = null, CallbackAction callback = null, Control callbackUIControl = null);
        void TelescopeSlewTo(double raHours, double deDeg, CallType callType = CallType.Async, Guid? clientId = null, CallbackAction callback = null, Control callbackUIControl = null);
		void TelescopeSlewNearBy(double distanceInArcSec, GuideDirections direction, CallType callType = CallType.Async, Guid? clientId = null, CallbackAction callback = null, Control callbackUIControl = null);
		void TelescopeSyncToCoordinates(double raHours, double deDeg, CallType callType = CallType.Async, Guid? clientId = null, CallbackAction callback = null, Control callbackUIControl = null);
        void GetTelescopeState(CallType callType = CallType.Async, Guid? clientId = null, CallbackAction callback = null, Control callbackUIControl = null);
		void TelescopeSetSlewRate(double degreesPerSecond, CallType callType = CallType.Async, Guid? clientId = null, CallbackAction callback = null, Control callbackUIControl = null);
		void TelescopeStopSlewing(CallType callType = CallType.Async, Guid? clientId = null, CallbackAction callback = null, Control callbackUIControl = null);
		void TelescopeStartSlewing(GuideDirections direction, CallType callType = CallType.Async, Guid? clientId = null, CallbackAction callback = null, Control callbackUIControl = null);

		void FocuserMoveIn(FocuserStepSize stepSize, CallType callType = CallType.Async, Guid? clientId = null, CallbackAction callback = null, Control callbackUIControl = null);
		void FocuserMoveOut(FocuserStepSize stepSize, CallType callType = CallType.Async, Guid? clientId = null, CallbackAction callback = null, Control callbackUIControl = null);
		void FocuserMove(int step, CallType callType = CallType.Async, Guid? clientId = null, CallbackAction callback = null, Control callbackUIControl = null);
		void FocuserSetTempComp(bool useTempComp, CallType callType = CallType.Async, Guid? clientId = null, CallbackAction callback = null, Control callbackUIControl = null);
		void GetFocuserState(CallType callType = CallType.Async, Guid? clientId = null, CallbackAction callback = null, Control callbackUIControl = null);
		void PerformTelescopePingActions(CallType callType = CallType.Async, Guid? clientId = null, CallbackAction callback = null, Control callbackUIControl = null);
		void PerformFocuserPingActions(CallType callType = CallType.Async, Guid? clientId = null, CallbackAction callback = null, Control callbackUIControl = null);

		void SetExternalCameraDriver(IOccuRecCameraController cameraDriver);
		bool HasVideoCamera { get; }
		bool Supports5ButtonOSD { get; }
		void CameraOSDUp(CallType callType = CallType.Async, Guid? clientId = null, CallbackAction callback = null, Control callbackUIControl = null);
		void CameraOSDDown(CallType callType = CallType.Async, Guid? clientId = null, CallbackAction callback = null, Control callbackUIControl = null);
		void CameraOSDLeft(CallType callType = CallType.Async, Guid? clientId = null, CallbackAction callback = null, Control callbackUIControl = null);
		void CameraOSDRight(CallType callType = CallType.Async, Guid? clientId = null, CallbackAction callback = null, Control callbackUIControl = null);
		void CameraOSDSet(CallType callType = CallType.Async, Guid? clientId = null, CallbackAction callback = null, Control callbackUIControl = null);
		void GetCameraState(CallType callType = CallType.Async, Guid? clientId = null, CallbackAction callback = null, Control callbackUIControl = null);
		void CameraGammaDown(CallType callType = CallType.Async, Guid? clientId = null, CallbackAction callback = null, Control callbackUIControl = null);
		void CameraGammaUp(CallType callType = CallType.Async, Guid? clientId = null, CallbackAction callback = null, Control callbackUIControl = null);
		void CameraGainDown(CallType callType = CallType.Async, Guid? clientId = null, CallbackAction callback = null, Control callbackUIControl = null);
		void CameraGainUp(CallType callType = CallType.Async, Guid? clientId = null, CallbackAction callback = null, Control callbackUIControl = null);
		void CameraExposureDown(CallType callType = CallType.Async, Guid? clientId = null, CallbackAction callback = null, Control callbackUIControl = null);
		void CameraExposureUp(CallType callType = CallType.Async, Guid? clientId = null, CallbackAction callback = null, Control callbackUIControl = null);
	}

	internal class ObservatoryController : ThreadIsolatedInvoker, IObservatoryController
	{
		private ITelescope m_ConnectedTelescope = null;
		private IFocuser m_ConnectedFocuser = null;
		private IVideoWrapper m_ConnectedVideo = null;

		private IOccuRecCameraController m_CameraDriver = null;

		private TelescopeEquatorialPosition m_CurrentTelescopePosition = null;
		private VideoState m_CurrentVideoState = null;

        public event Action<ASCOMConnectionState> TelescopeConnectionChanged;
        public event Action<ASCOMConnectionState> FocuserConnectionChanged;
		public event Action<ASCOMConnectionState> VideoConnectionChanged;
        public event Action<TelescopeState> TelescopeStateUpdated;
	    public event Action<TelescopeEquatorialPosition> TelescopePositionChanged;
		public event Action<TelescopeCapabilities> TelescopeCapabilitiesKnown;
        public event Action<FocuserState> FocuserStateUpdated;
        public event Action<FocuserPosition> FocuserPositionUpdated;
		public event Action<VideoState> VideoStateUpdated;
        public event Action<string> VideoError;

		public TelescopeEquatorialPosition CurrentTelescopePosition()
		{
			if (IsConnectedToTelescope())
				return m_CurrentTelescopePosition;
			else
				return null;
		}

		public VideoState CurrentVideoState()
		{
			if (IsConnectedToVideoCamera())
				return m_CurrentVideoState;
			else
				return null;
		}

		public void SetExternalCameraDriver(IOccuRecCameraController cameraDriver)
		{
            if (m_CameraDriver != cameraDriver)
            {
                if (m_CameraDriver != null)
                    m_CameraDriver.OnError -= m_CameraDriver_OnError;

                m_CameraDriver = cameraDriver;
                m_CameraDriver.OnError += m_CameraDriver_OnError;
            }
		}

        void m_CameraDriver_OnError(DriverErrorEventArgs e)
        {
            EventHelper.RaiseEvent(VideoError, string.Format("{0}: {1}", e.CommandId, e.ErrorMessage));
        }

		public bool HasVideoCamera
		{
			get { return m_CameraDriver != null; }
		}

		public bool Supports5ButtonOSD
		{
			get
			{
				return m_CameraDriver != null && m_CameraDriver.Supports5ButtonOSD;
			}
		}

		public void CameraOSDUp(CallType callType = CallType.Async, Guid? clientId = null, CallbackAction callback = null, Control callbackUIControl = null)
		{
			IsolatedAction(() =>
			{
				try
				{
					if (m_CameraDriver != null && m_CameraDriver.Connected)
					{
						m_CameraDriver.OSDUp();
					}
				}
				catch (Exception ex)
				{
					OnVideoErrored();;
					Trace.WriteLine(ex.GetFullStackTrace());

					return ex;
				}

				return null;
			},
			callType, callback, callbackUIControl);			
		}

		public void CameraOSDDown(CallType callType = CallType.Async, Guid? clientId = null, CallbackAction callback = null, Control callbackUIControl = null)
		{
			IsolatedAction(() =>
			{
				try
				{
					if (m_CameraDriver != null && m_CameraDriver.Connected)
					{
						m_CameraDriver.OSDDown();
					}
				}
				catch (Exception ex)
				{
					OnVideoErrored(); ;
					Trace.WriteLine(ex.GetFullStackTrace());

					return ex;
				}

				return null;
			},
			callType, callback, callbackUIControl);		
		}

		public void CameraOSDLeft(CallType callType = CallType.Async, Guid? clientId = null, CallbackAction callback = null, Control callbackUIControl = null)
		{
			IsolatedAction(() =>
			{
				try
				{
					if (m_CameraDriver != null && m_CameraDriver.Connected)
					{
						m_CameraDriver.OSDLeft();
					}
				}
				catch (Exception ex)
				{
					OnVideoErrored(); ;
					Trace.WriteLine(ex.GetFullStackTrace());

					return ex;
				}

				return null;
			},
			callType, callback, callbackUIControl);	
		}

		public void CameraOSDRight(CallType callType = CallType.Async, Guid? clientId = null, CallbackAction callback = null, Control callbackUIControl = null)
		{
			IsolatedAction(() =>
			{
				try
				{
					if (m_CameraDriver != null && m_CameraDriver.Connected)
					{
						m_CameraDriver.OSDRight();
					}
				}
				catch (Exception ex)
				{
					OnVideoErrored(); ;
					Trace.WriteLine(ex.GetFullStackTrace());

					return ex;
				}

				return null;
			},
			callType, callback, callbackUIControl);	
		}

		public void CameraOSDSet(CallType callType = CallType.Async, Guid? clientId = null, CallbackAction callback = null, Control callbackUIControl = null)
		{
			IsolatedAction(() =>
			{
				try
				{
					if (m_CameraDriver != null && m_CameraDriver.Connected)
					{
						m_CameraDriver.OSDSet();

                        VideoState state = m_CameraDriver.GetCurrentState(CameraStateQuery.All);
						OnVideoState(state);
					}
				}
				catch (Exception ex)
				{
					OnVideoErrored(); ;
					Trace.WriteLine(ex.GetFullStackTrace());

					return ex;
				}

				return null;
			},
			callType, callback, callbackUIControl);	
		}

		public void GetCameraState(CallType callType = CallType.Async, Guid? clientId = null, CallbackAction callback = null, Control callbackUIControl = null)
		{
			IsolatedAction(() =>
			{
				try
				{
					if (m_CameraDriver != null && m_CameraDriver.Connected)
					{
                        VideoState state = m_CameraDriver.GetCurrentState(CameraStateQuery.All);
						OnVideoState(state);
					}
				}
				catch (Exception ex)
				{
					OnVideoErrored();
					Trace.WriteLine(ex.GetFullStackTrace());

					return ex;
				}

				return null;
			},
				callType, callback, callbackUIControl);
		}

		public void CameraGammaDown(CallType callType = CallType.Async, Guid? clientId = null, CallbackAction callback = null, Control callbackUIControl = null)
		{
			IsolatedAction(() =>
			{
				try
				{
					if (m_CameraDriver != null && m_CameraDriver.Connected)
					{
						m_CameraDriver.GammaDown();
                        VideoState state = m_CameraDriver.GetCurrentState(CameraStateQuery.Gamma);
						OnVideoState(state);
					}
				}
				catch (Exception ex)
				{
					OnVideoErrored();
					Trace.WriteLine(ex.GetFullStackTrace());

					return ex;
				}

				return null;
			},
			callType, callback, callbackUIControl);
		}

		public void CameraGammaUp(CallType callType = CallType.Async, Guid? clientId = null, CallbackAction callback = null, Control callbackUIControl = null)
		{
			IsolatedAction(() =>
			{
				try
				{
					if (m_CameraDriver != null && m_CameraDriver.Connected)
					{
						m_CameraDriver.GammaUp();
                        VideoState state = m_CameraDriver.GetCurrentState(CameraStateQuery.Gamma);
						OnVideoState(state);
					}
				}
				catch (Exception ex)
				{
					OnVideoErrored();
					Trace.WriteLine(ex.GetFullStackTrace());

					return ex;
				}

				return null;
			},
			callType, callback, callbackUIControl);
		}

		public void CameraGainDown(CallType callType = CallType.Async, Guid? clientId = null, CallbackAction callback = null, Control callbackUIControl = null)
		{
			IsolatedAction(() =>
			{
				try
				{
					if (m_CameraDriver != null && m_CameraDriver.Connected)
					{
						m_CameraDriver.GainDown();
                        VideoState state = m_CameraDriver.GetCurrentState(CameraStateQuery.Gain);
						OnVideoState(state);
					}
				}
				catch (Exception ex)
				{
					OnVideoErrored();
					Trace.WriteLine(ex.GetFullStackTrace());

					return ex;
				}

				return null;
			},
			callType, callback, callbackUIControl);
		}

		public void CameraGainUp(CallType callType = CallType.Async, Guid? clientId = null, CallbackAction callback = null, Control callbackUIControl = null)
		{
			IsolatedAction(() =>
			{
				try
				{
					if (m_CameraDriver != null && m_CameraDriver.Connected)
					{
						m_CameraDriver.GainUp();
                        VideoState state = m_CameraDriver.GetCurrentState(CameraStateQuery.Gain);
						OnVideoState(state);
					}
				}
				catch (Exception ex)
				{
					OnVideoErrored();
					Trace.WriteLine(ex.GetFullStackTrace());

					return ex;
				}

				return null;
			},
			callType, callback, callbackUIControl);
		}

		public void CameraExposureDown(CallType callType = CallType.Async, Guid? clientId = null, CallbackAction callback = null, Control callbackUIControl = null)
		{
			IsolatedAction(() =>
			{
				try
				{
					if (m_CameraDriver != null && m_CameraDriver.Connected)
					{
						m_CameraDriver.ExposureDown();
                        VideoState state = m_CameraDriver.GetCurrentState(CameraStateQuery.Shutter);
						OnVideoState(state);
					}
				}
				catch (Exception ex)
				{
					OnVideoErrored();
					Trace.WriteLine(ex.GetFullStackTrace());

					return ex;
				}

				return null;
			},
			callType, callback, callbackUIControl);
		}

		public void CameraExposureUp(CallType callType = CallType.Async, Guid? clientId = null, CallbackAction callback = null, Control callbackUIControl = null)
		{
			IsolatedAction(() =>
			{
				try
				{
					if (m_CameraDriver != null && m_CameraDriver.Connected)
					{
						m_CameraDriver.ExposureUp();
                        VideoState state = m_CameraDriver.GetCurrentState(CameraStateQuery.Shutter);
						OnVideoState(state);
					}
				}
				catch (Exception ex)
				{
					OnVideoErrored();
					Trace.WriteLine(ex.GetFullStackTrace());

					return ex;
				}

				return null;
			},
			callType, callback, callbackUIControl);
		}


		public void PerformTelescopePingActions(CallType callType = CallType.Async, Guid? clientId = null, CallbackAction callback = null, Control callbackUIControl = null)
        {
            IsolatedAction(() =>
            {
                try
                {
                    if (m_ConnectedTelescope != null && m_ConnectedTelescope.Connected)
                    {
                        TelescopeState state = m_ConnectedTelescope.GetCurrentState();
                        OnTelescopeState(state);
                    }
                }
                catch (Exception ex)
                {
                    OnTelescopeErrored();
                    Trace.WriteLine(ex.GetFullStackTrace());

                    return ex;
                }

                return null;
            },
            callType, callback, callbackUIControl);
        }

		public void PerformFocuserPingActions(CallType callType = CallType.Async, Guid? clientId = null, CallbackAction callback = null, Control callbackUIControl = null)
        {
            IsolatedAction(() =>
            {
                try
                {
                    if (m_ConnectedFocuser != null && m_ConnectedFocuser.Connected)
                    {
                        FocuserState state = m_ConnectedFocuser.GetCurrentState();
                        OnFocuserState(state);
                    }
                }
                catch (Exception ex)
                {
                    OnFocuserErrored();
                    Trace.WriteLine(ex.GetFullStackTrace());

                    return ex;
                }

                return null;
            },
            callType, callback, callbackUIControl);
        }

		private Guid? m_TelescopeEngagedClientId = null;
		private Guid? m_FocuserEngagedClientId = null;
		private object m_SyncLock = new object();

		private void AssertTelescopeEngagement(Guid? clientId)
		{
			bool operationAuthorised = false;

			lock (m_SyncLock)
			{
				operationAuthorised = m_TelescopeEngagedClientId == null || m_TelescopeEngagedClientId == clientId;
			}

			if (!operationAuthorised)
				throw new ObservatoryControlException("The telescope has been engaged by another client and cannot be controller right now.");
		}

		private void AssertFocuserEngagement(Guid? clientId)
		{
			bool operationAuthorised = false;

			lock (m_SyncLock)
			{
				operationAuthorised = m_FocuserEngagedClientId == null || m_FocuserEngagedClientId == clientId;
			}

			if (!operationAuthorised)
				throw new ObservatoryControlException("The focuser has been engaged by another client and cannot be controller right now.");
		}

		public Guid? ControlTelescopeLock(bool isEngaged, Guid? clientId)
		{
			lock (m_SyncLock)
			{
				if (isEngaged)
				{
					if (m_TelescopeEngagedClientId != null)
						return null; // Already engaged
					else
					{
						m_TelescopeEngagedClientId = new Guid();
						OnTelescopeEngaged();
						return m_TelescopeEngagedClientId;
					}
				}
				else
				{
					if (m_TelescopeEngagedClientId != clientId)
						return null; // Not engaged by this client
					else
					{
						m_TelescopeEngagedClientId = null;
						OnTelescopeDisengaged();
						return clientId;
					}
				}
			}
		}

		public Guid? ControlFocuserLock(bool isEngaged, Guid? clientId)
		{
			lock (m_SyncLock)
			{
				if (isEngaged)
				{
					if (m_FocuserEngagedClientId != null)
						return null; // Already engaged
					else
					{
						m_FocuserEngagedClientId = new Guid();
						OnFocuserEngaged();
						return m_FocuserEngagedClientId;
					}
				}
				else
				{
					if (m_FocuserEngagedClientId != clientId)
						return null; // Not engaged by this client
					else
					{
						m_FocuserEngagedClientId = null;
						OnFocuserDisengaged();
						return clientId;
					}
				}
			}
		}

		public void TryConnectTelescope(CallType callType = CallType.Async, Guid? clientId = null, CallbackAction callback = null, Control callbackUIControl = null)
		{
			IsolatedAction(() =>
			{
				try
				{
					if (m_ConnectedTelescope != null && m_ConnectedTelescope.ProgId != Settings.Default.ASCOMProgIdTelescope)
					{
						AssertTelescopeEngagement(clientId);

						ASCOMClient.Instance.DisconnectTelescope(m_ConnectedTelescope);
						m_ConnectedTelescope = null;
					}

					if (m_ConnectedTelescope == null && !string.IsNullOrEmpty(Settings.Default.ASCOMProgIdTelescope))
						m_ConnectedTelescope = ASCOMClient.Instance.CreateTelescope(Settings.Default.ASCOMProgIdTelescope, Settings.Default.TelPulseSlowestRate, Settings.Default.TelPulseSlowRate, Settings.Default.TelPulseFasterRate);

					if (m_ConnectedTelescope != null)
					{
						if (!m_ConnectedTelescope.Connected)
						{
							OnTelescopeConnecting();
							m_ConnectedTelescope.Connected = true;
							OnTelescopeConnected();
						}
                        else
                            OnTelescopeConnected();

						TelescopeCapabilities capabilities = m_ConnectedTelescope.GetTelescopeCapabilities();
						OnTelescopeCapabilitiesKnown(capabilities);

						TelescopeState state = m_ConnectedTelescope.GetCurrentState();
						OnTelescopeState(state);

					    return state;
					}

				    return null;
				}
				catch (Exception ex)
				{
					OnTelescopeErrored();
					Trace.WriteLine(ex.GetFullStackTrace());

				    return ex;
				}
			},
			callType, callback, callbackUIControl);
		}

		public void TryConnectFocuser(CallType callType = CallType.Async, Guid? clientId = null, CallbackAction callback = null, Control callbackUIControl = null)
		{
			IsolatedAction(() =>
			{
				try
				{
					if (m_ConnectedFocuser != null && m_ConnectedFocuser.ProgId != Settings.Default.ASCOMProgIdFocuser)
					{
						AssertFocuserEngagement(clientId);

						ASCOMClient.Instance.DisconnectFocuser(m_ConnectedFocuser);
						m_ConnectedFocuser = null;
					}

					if (m_ConnectedFocuser == null && !string.IsNullOrEmpty(Settings.Default.ASCOMProgIdFocuser))
					{
						m_ConnectedFocuser = ASCOMClient.Instance.CreateFocuser(
							Settings.Default.ASCOMProgIdFocuser,
							Settings.Default.FocuserLargeStep,
							Settings.Default.FocuserSmallStep,
							Settings.Default.FocuserSmallestStep);
					}

					if (m_ConnectedFocuser != null)
					{
						if (!m_ConnectedFocuser.Connected)
						{
							OnFocuserConnecting();
							m_ConnectedFocuser.Connected = true;
							OnFocuserConnected();
						}
                        else
                            OnFocuserConnected();

						FocuserState state = m_ConnectedFocuser.GetCurrentState();
						OnFocuserState(state);

                        return state;
					}

				    return null;
				}
				catch (Exception ex)
				{
					OnFocuserErrored();
					Trace.WriteLine(ex.GetFullStackTrace());

				    return ex;
				}

			},
			callType, callback, callbackUIControl);
		}

		public bool IsConnectedToObservatory()
		{
			return m_ConnectedTelescope != null || m_ConnectedFocuser != null;
		}

		public bool IsConnectedToTelescope()
		{
			return m_ConnectedTelescope != null;
		}

		public string ConnectedTelescopeDriverName()
		{
			return m_ConnectedTelescope != null ? m_ConnectedTelescope.Description : string.Empty;
		}

		public bool IsConnectedToFocuser()
		{
			return m_ConnectedFocuser != null;
		}

		public string ConnectedFocuserDriverName()
		{
			return m_ConnectedFocuser != null ? m_ConnectedFocuser.Description : string.Empty;
		}

		public void DisconnectTelescope(CallType callType = CallType.Async, Guid? clientId = null, CallbackAction callback = null, Control callbackUIControl = null)
		{
			IsolatedAction(() =>
			{
				try
				{
					if (m_ConnectedTelescope != null)
					{
						AssertTelescopeEngagement(clientId);

						ASCOMClient.Instance.DisconnectTelescope(m_ConnectedTelescope);
						m_ConnectedTelescope = null;
					}
				}
				catch (Exception ex)
				{
					OnTelescopeErrored();
					Trace.WriteLine(ex.GetFullStackTrace());

				    return ex;
				}

				OnTelescopeDisconnected();

                return null;
			},
			callType, callback, callbackUIControl);
		}

		public void DisconnectFocuser(CallType callType = CallType.Async, Guid? clientId = null, CallbackAction callback = null, Control callbackUIControl = null)
		{
			IsolatedAction(() =>
			{
				try
				{
					if (m_ConnectedFocuser != null)
					{
						AssertFocuserEngagement(clientId);

						ASCOMClient.Instance.DisconnectFocuser(m_ConnectedFocuser);
						m_ConnectedFocuser = null;
					}
				}
				catch (Exception ex)
				{
					OnFocuserErrored();
					Trace.WriteLine(ex.GetFullStackTrace());

                    return ex;
				}

				OnFocuserDisconnected();

			    return null;
			},
			callType, callback, callbackUIControl);
		}

        public void GetTelescopeState(CallType callType = CallType.Async, Guid? clientId = null, CallbackAction callback = null, Control callbackUIControl = null)
        {
            IsolatedAction(() =>
            {
                try
                {
                    if (m_ConnectedTelescope != null && m_ConnectedTelescope.Connected)
                    {
                        AssertTelescopeEngagement(clientId);

                        TelescopeEquatorialPosition position = m_ConnectedTelescope.GetEquatorialPosition();

                        OnTelescopePosition(position);

                        return position;
                    }

                    return null;
                }
                catch (Exception ex)
                {
                    OnTelescopeErrored();
                    Trace.WriteLine(ex.GetFullStackTrace());

                    return ex;
                }
            },
            callType, callback, callbackUIControl);
        }

		public void TelescopeSetSlewRate(double degreesPerSecond, CallType callType = CallType.Async, Guid? clientId = null, CallbackAction callback = null, Control callbackUIControl = null)
		{
			IsolatedAction((deg) =>
			{
				try
				{
					if (m_ConnectedTelescope != null && m_ConnectedTelescope.Connected)
					{
						AssertTelescopeEngagement(clientId);

						m_ConnectedTelescope.SetSlewRate(deg);

						TelescopeEquatorialPosition position = m_ConnectedTelescope.GetEquatorialPosition();

						OnTelescopePosition(position);

						return position;
					}

					return null;
				}
				catch (Exception ex)
				{
					OnTelescopeErrored();
					Trace.WriteLine(ex.GetFullStackTrace());

					return ex;
				}
			},
			degreesPerSecond, callType, callback, callbackUIControl);
		}

		public void TelescopeStopSlewing(CallType callType = CallType.Async, Guid? clientId = null, CallbackAction callback = null, Control callbackUIControl = null)
		{
			IsolatedAction(() =>
			{
				try
				{
					if (m_ConnectedTelescope != null && m_ConnectedTelescope.Connected)
					{
						AssertTelescopeEngagement(clientId);

						m_ConnectedTelescope.StopSlewing();

						TelescopeEquatorialPosition position = m_ConnectedTelescope.GetEquatorialPosition();

						OnTelescopePosition(position);

						return position;
					}

					return null;
				}
				catch (Exception ex)
				{
					OnTelescopeErrored();
					Trace.WriteLine(ex.GetFullStackTrace());

					return ex;
				}
			},
			callType, callback, callbackUIControl);
		}

		public void TelescopeStartSlewing(GuideDirections direction, CallType callType = CallType.Async, Guid? clientId = null, CallbackAction callback = null, Control callbackUIControl = null)
		{
			IsolatedAction((dir) =>
			{
				try
				{
					if (m_ConnectedTelescope != null && m_ConnectedTelescope.Connected)
					{
						AssertTelescopeEngagement(clientId);

						m_ConnectedTelescope.StartSlewing(dir);

						TelescopeEquatorialPosition position = m_ConnectedTelescope.GetEquatorialPosition();

						OnTelescopePosition(position);

						return position;
					}

					return null;
				}
				catch (Exception ex)
				{
					OnTelescopeErrored();
					Trace.WriteLine(ex.GetFullStackTrace());

					return ex;
				}
			},
			direction, callType, callback, callbackUIControl);
		}

		public void TelescopeSlewNearBy(double distanceInArcSec, GuideDirections direction, CallType callType = CallType.Async, Guid? clientId = null, CallbackAction callback = null, Control callbackUIControl = null)
		{
			IsolatedAction((distance, dir) =>
			{
				try
				{
					if (m_ConnectedTelescope != null && m_ConnectedTelescope.Connected)
					{
						AssertTelescopeEngagement(clientId);

						m_ConnectedTelescope.SlewNearBy(distance, dir);

						TelescopeEquatorialPosition position = m_ConnectedTelescope.GetEquatorialPosition();

						OnTelescopePosition(position);

						return position;
					}

					return null;
				}
				catch (Exception ex)
				{
					OnTelescopeErrored();
					Trace.WriteLine(ex.GetFullStackTrace());

					return ex;
				}
			},
			distanceInArcSec, direction, callType, callback, callbackUIControl);
		}

		public void TelescopeSyncToCoordinates(double raHours, double deDeg, CallType callType = CallType.Async, Guid? clientId = null, CallbackAction callback = null, Control callbackUIControl = null)
		{
			IsolatedAction((ra, dec) =>
			{
				try
				{
					if (m_ConnectedTelescope != null && m_ConnectedTelescope.Connected)
					{
						AssertTelescopeEngagement(clientId);

						m_ConnectedTelescope.SyncToCoordinates(ra, dec);

						TelescopeEquatorialPosition position = m_ConnectedTelescope.GetEquatorialPosition();

						OnTelescopePosition(position);

						return position;
					}

					return null;
				}
				catch (Exception ex)
				{
					OnTelescopeErrored();
					Trace.WriteLine(ex.GetFullStackTrace());

					return ex;
				}
			},
			raHours, deDeg, callType, callback, callbackUIControl);
		}


        public void TelescopeSlewTo(double raHours, double deDeg, CallType callType = CallType.Async, Guid? clientId = null, CallbackAction callback = null, Control callbackUIControl = null)
        {
            IsolatedAction((ra, dec) =>
            {
                try
                {
                    if (m_ConnectedTelescope != null && m_ConnectedTelescope.Connected)
                    {
                        AssertTelescopeEngagement(clientId);

                        m_ConnectedTelescope.SlewTo(ra, dec);

                        TelescopeEquatorialPosition position = m_ConnectedTelescope.GetEquatorialPosition();

                        OnTelescopePosition(position);

                        return position;
                    }

                    return null;
                }
                catch (Exception ex)
                {
                    OnTelescopeErrored();
                    Trace.WriteLine(ex.GetFullStackTrace());

                    return ex;
                }
            },
            raHours, deDeg, callType, callback, callbackUIControl);
        }

		public void TelescopePulseGuide(GuideDirections direction, PulseRate pulseRate, CallType callType = CallType.Async, Guid? clientId = null, CallbackAction callback = null, Control callbackUIControl = null)
        {
            IsolatedAction((x, y) =>
            {
                try
                {
					if (m_ConnectedTelescope != null && m_ConnectedTelescope.Connected)
                    {
						AssertTelescopeEngagement(clientId);

                        m_ConnectedTelescope.PulseGuide(x, y, Settings.Default.TelPulseDuration);

                        TelescopeEquatorialPosition position = m_ConnectedTelescope.GetEquatorialPosition();

                        OnTelescopePosition(position);

                        return position;
                    }

                    return null;
                }
                catch (Exception ex)
                {
                    OnTelescopeErrored();
                    Trace.WriteLine(ex.GetFullStackTrace());

                    return ex;
                }
            },
            direction, pulseRate, callType, callback, callbackUIControl);
        }

		public void FocuserMoveIn(FocuserStepSize stepSize, CallType callType = CallType.Async, Guid? clientId = null, CallbackAction callback = null, Control callbackUIControl = null)
        {
            IsolatedAction((x) =>
            {
                try
                {
                    if (m_ConnectedFocuser != null && m_ConnectedFocuser.Connected)
                    {
						AssertFocuserEngagement(clientId);

                        m_ConnectedFocuser.MoveIn(x);

                        FocuserPosition position = m_ConnectedFocuser.GetCurrentPosition();

                        OnFocuserPosition(position);

                        return position;
                    }

                    return null;
                }
                catch (Exception ex)
                {
                    OnFocuserErrored();
                    Trace.WriteLine(ex.GetFullStackTrace());

                    return ex;
                }
            },
            stepSize, callType, callback, callbackUIControl);
            
        }

		public void FocuserMoveOut(FocuserStepSize stepSize, CallType callType = CallType.Async, Guid? clientId = null, CallbackAction callback = null, Control callbackUIControl = null)
        {
            IsolatedAction((x) =>
            {
                try
                {
                    if (m_ConnectedFocuser != null && m_ConnectedFocuser.Connected)
                    {
						AssertFocuserEngagement(clientId);

                        m_ConnectedFocuser.MoveOut(x);

                        FocuserPosition position = m_ConnectedFocuser.GetCurrentPosition();

                        OnFocuserPosition(position);

                        return position;
                    }

                    return null;
                }
                catch (Exception ex)
                {
                    OnFocuserErrored();
                    Trace.WriteLine(ex.GetFullStackTrace());

                    return ex;
                }
            },
            stepSize, callType, callback, callbackUIControl);
        }

		public void FocuserMove(int step, CallType callType = CallType.Async, Guid? clientId = null, CallbackAction callback = null, Control callbackUIControl = null)
        {
            IsolatedAction((x) =>
            {
                try
                {
                    if (m_ConnectedFocuser != null && m_ConnectedFocuser.Connected)
                    {
						AssertFocuserEngagement(clientId);

                        m_ConnectedFocuser.Move(x);

                        FocuserPosition position = m_ConnectedFocuser.GetCurrentPosition();

                        OnFocuserPosition(position);

                        return position;
                    }

                    return null;
                }
                catch (Exception ex)
                {
                    OnFocuserErrored();
                    Trace.WriteLine(ex.GetFullStackTrace());

                    return ex;
                }
            },
            step, callType, callback, callbackUIControl);
        }

		public void FocuserSetTempComp(bool useTempComp, CallType callType = CallType.Async, Guid? clientId = null, CallbackAction callback = null, Control callbackUIControl = null)
        {
            IsolatedAction((x) =>
            {
                try
                {
                    if (m_ConnectedFocuser != null && m_ConnectedFocuser.Connected)
                    {
						AssertFocuserEngagement(clientId);

                        m_ConnectedFocuser.ChangeTempComp(x);

                        FocuserState state = m_ConnectedFocuser.GetCurrentState();

                        OnFocuserState(state);

                        return state;
                    }

                    return null;
                }
                catch (Exception ex)
                {
                    OnFocuserErrored();
                    Trace.WriteLine(ex.GetFullStackTrace());

                    return ex;
                }
            },
            useTempComp, callType, callback, callbackUIControl);
        }

		public void GetFocuserState(CallType callType = CallType.Async, Guid? clientId = null, CallbackAction callback = null, Control callbackUIControl = null)
        {
            IsolatedAction(() =>
            {
                try
                {
                    if (m_ConnectedFocuser != null && m_ConnectedFocuser.Connected)
                    {
						// NOTE: Everyone can get the focuser state, even when it has been engaged 

                        FocuserState state = m_ConnectedFocuser.GetCurrentState();

                        OnFocuserState(state);

                        return state;
                    }

                    return null;
                }
                catch (Exception ex)
                {
                    OnFocuserErrored();
                    Trace.WriteLine(ex.GetFullStackTrace());

                    return ex;
                }
            },
            callType, callback, callbackUIControl);
        }



		#region UI Callbacks
		private void OnTelescopeConnecting()
		{
            EventHelper.RaiseEvent(TelescopeConnectionChanged, ASCOMConnectionState.Connecting);
		}

		private void OnFocuserConnecting()
		{
            EventHelper.RaiseEvent(FocuserConnectionChanged, ASCOMConnectionState.Connecting);
		}

		private void OnVideoConnecting()
		{
            EventHelper.RaiseEvent(VideoConnectionChanged, ASCOMConnectionState.Connecting);
		}

		private void OnTelescopeConnected()
		{
            EventHelper.RaiseEvent(TelescopeConnectionChanged, ASCOMConnectionState.Connected);
		}

		private void OnFocuserConnected()
		{
            EventHelper.RaiseEvent(FocuserConnectionChanged, ASCOMConnectionState.Connected);
		}

		private void OnVideoConnected()
		{
            EventHelper.RaiseEvent(VideoConnectionChanged, ASCOMConnectionState.Connected);
		}

		private void OnTelescopeDisconnected()
		{
            EventHelper.RaiseEvent(TelescopeConnectionChanged, ASCOMConnectionState.Disconnected);
		}

		private void OnFocuserDisconnected()
		{
            EventHelper.RaiseEvent(FocuserConnectionChanged, ASCOMConnectionState.Disconnected);
		}

		private void OnVideoDisconnected()
		{
            EventHelper.RaiseEvent(VideoConnectionChanged, ASCOMConnectionState.Disconnected);
		}

		private void OnTelescopeEngaged()
		{
			EventHelper.RaiseEvent(TelescopeConnectionChanged, ASCOMConnectionState.Engaged);
		}

		private void OnTelescopeDisengaged()
		{
			EventHelper.RaiseEvent(TelescopeConnectionChanged, ASCOMConnectionState.Ready);
		}

		private void OnFocuserEngaged()
		{
			EventHelper.RaiseEvent(FocuserConnectionChanged, ASCOMConnectionState.Engaged);
		}

		private void OnFocuserDisengaged()
		{
			EventHelper.RaiseEvent(FocuserConnectionChanged, ASCOMConnectionState.Ready);
		}

        private void OnTelescopePosition(TelescopeEquatorialPosition position)
        {
	        m_CurrentTelescopePosition = position;

	        try
	        {
				NativeHelpers.CurrentTargetInfo = string.Format("RA={0} DE={1}", AstroConvert.ToStringValue(position.RightAscension, "HH MM SS"), AstroConvert.ToStringValue(position.Declination, "+DD MM SS.T"));
	        }
	        catch
	        { }

            EventHelper.RaiseEvent(TelescopePositionChanged, position);
		}
        

		private void OnTelescopeErrored()
		{
            EventHelper.RaiseEvent(TelescopeConnectionChanged, ASCOMConnectionState.Errored);
		}

		private void OnFocuserErrored()
		{
            EventHelper.RaiseEvent(FocuserConnectionChanged, ASCOMConnectionState.Errored);
		}

		private void OnVideoErrored()
		{
            EventHelper.RaiseEvent(VideoConnectionChanged, ASCOMConnectionState.Errored);
		}

		private void OnTelescopeState(TelescopeState state)
		{
            EventHelper.RaiseEvent(TelescopeStateUpdated, state);
		}

		private void OnTelescopeCapabilitiesKnown(TelescopeCapabilities capabilities)
		{
			EventHelper.RaiseEvent(TelescopeCapabilitiesKnown, capabilities);
		}

		private void OnFocuserState(FocuserState state)
		{
            EventHelper.RaiseEvent(FocuserStateUpdated, state);
		}

        private void OnFocuserPosition(FocuserPosition position)
		{
            EventHelper.RaiseEvent(FocuserPositionUpdated, position);
		}

		private static Regex REGEX_FLOATING_POINT_VALUE = new Regex("^[0-9\\.]+$");

		private void OnVideoState(VideoState state)
		{
			m_CurrentVideoState = state;

			try
			{
				NativeHelpers.CurrentCameraGain = float.Parse(REGEX_FLOATING_POINT_VALUE.Match(state.Gain).Value.Trim());
			}
			catch
			{ }

			try
			{
				NativeHelpers.CurrentCameraGamma = float.Parse(REGEX_FLOATING_POINT_VALUE.Match(state.Gamma).Value.Trim());
			}
			catch
			{ }

            EventHelper.RaiseEvent(VideoStateUpdated, state);
		}

		#endregion

		public void Dispose()
		{
			if (m_CameraDriver != null)
			{
				try
				{
					if (m_CameraDriver.Connected)
						m_CameraDriver.Connected = false;
				}
				catch (Exception ex)
				{
					Trace.WriteLine(ex.GetFullStackTrace());
				}

				try
				{
					m_CameraDriver.Dispose();
				}
				catch (Exception ex)
				{
					Trace.WriteLine(ex.GetFullStackTrace());
				}

				m_CameraDriver = null;
			}
		}

		public bool IsConnectedToVideoCamera()
		{
			return
				m_ConnectedVideo != null ||
				(m_CameraDriver != null && m_CameraDriver.Connected);
		}

		public string ConnectedVideoCameraDriverName()
		{
			if (m_ConnectedVideo != null)
				return m_ConnectedVideo.Description;
			else if (m_CameraDriver != null)
				return m_CameraDriver.Description;
			else
				return string.Empty;
		}

		public void DisconnectVideoCamera(CallType callType = CallType.Async, Guid? clientId = null, CallbackAction callback = null, Control callbackUIControl = null)
		{
			if (m_ConnectedVideo != null)
				DisconnectASCOMVideoCamera(callType, callback, callbackUIControl);
			else if (m_CameraDriver != null)
				DisconnectOccuRecVideoCamera(callType, callback, callbackUIControl);
		}

		private void DisconnectOccuRecVideoCamera(CallType callType = CallType.Async, CallbackAction callback = null, Control callbackUIControl = null)
		{
			IsolatedAction(() =>
			{
				try
				{
					if (m_CameraDriver != null && m_CameraDriver.Connected)
					{
						m_CameraDriver.Connected = false;
					}
				}
				catch (Exception ex)
				{
					OnVideoErrored();
					Trace.WriteLine(ex.GetFullStackTrace());

					return ex;
				}

				OnVideoDisconnected();

				return null;
			},
						callType, callback, callbackUIControl);
		}
		
		private void DisconnectASCOMVideoCamera(CallType callType = CallType.Async, CallbackAction callback = null, Control callbackUIControl = null)
		{
			IsolatedAction(() =>
			{
				try
				{
					if (m_ConnectedVideo != null)
					{
						ASCOMClient.Instance.DisconnectVideo(m_ConnectedVideo);
						m_ConnectedVideo = null;
					}
				}
				catch (Exception ex)
				{
					OnVideoErrored();
					Trace.WriteLine(ex.GetFullStackTrace());

					return ex;
				}

				OnVideoDisconnected();

				return null;
			},
			callType, callback, callbackUIControl);
		}

		public void TryConnectVideoCamera(CallType callType = CallType.Async, Guid? clientId = null, CallbackAction callback = null, Control callbackUIControl = null)
		{
			if (m_CameraDriver != null)
				TryConnectOccuRecVideo(callType, callback, callbackUIControl);
			else if (m_ConnectedVideo != null || !string.IsNullOrEmpty(Settings.Default.ASCOMProgIdVideo))
				TryConnectASCOMVideo(callType, callback, callbackUIControl);
		}

		private void TryConnectOccuRecVideo(CallType callType = CallType.Async, CallbackAction callback = null, Control callbackUIControl = null)
		{
			IsolatedAction(() =>
			{
				try
				{
					if (m_CameraDriver != null)
					{
						if (!m_CameraDriver.Connected)
						{
							OnVideoConnecting();
							m_CameraDriver.Connected = true;
							OnVideoConnected();
						}
                        else
                            OnVideoConnected();

                        VideoState state = m_CameraDriver.GetCurrentState(CameraStateQuery.All);
						OnVideoState(state);

						return state;
					}

					return null;
				}
				catch (Exception ex)
				{
					OnVideoErrored();
					Trace.WriteLine(ex.GetFullStackTrace());

					return ex;
				}

			},
			callType, callback, callbackUIControl);
		}

		private void TryConnectASCOMVideo(CallType callType = CallType.Async, CallbackAction callback = null, Control callbackUIControl = null)
		{
			IsolatedAction(() =>
			{
				try
				{
					if (m_ConnectedVideo != null && m_ConnectedVideo.ProgId != Settings.Default.ASCOMProgIdVideo)
					{
						ASCOMClient.Instance.DisconnectVideo(m_ConnectedVideo);
						m_ConnectedVideo = null;
					}

					if (m_ConnectedVideo == null && !string.IsNullOrEmpty(Settings.Default.ASCOMProgIdVideo))
					{
						m_ConnectedVideo = ASCOMClient.Instance.CreateVideo(Settings.Default.ASCOMProgIdVideo);
					}

					if (m_ConnectedVideo != null)
					{
						if (!m_ConnectedVideo.Connected)
						{
							OnVideoConnecting();
							m_ConnectedVideo.Connected = true;
							OnVideoConnected();
						}
                        else
                            OnVideoConnected();

						VideoState state = m_ConnectedVideo.GetCurrentState();
						OnVideoState(state);

						return state;
					}

					return null;
				}
				catch (Exception ex)
				{
					OnVideoErrored();
					Trace.WriteLine(ex.GetFullStackTrace());

					return ex;
				}

			},
			callType, callback, callbackUIControl);
		}

		
		public static bool IsASCOMPlatformInstalled
		{
			get
			{
				CheckASCOMPlatformVersion();
				return s_ASCOMPlatformVersion.HasValue;
			}
		}

		public static bool IsASCOMPlatformVideoAvailable
		{
			get
			{
				CheckASCOMPlatformVersion();
				return s_ASCOMPlatformVersion.HasValue && (s_ASCOMPlatformVersion.Value - 6.1 + 0.005 /* Dealing with floating point comparison */ > 0);
			}
		}

		public static string GetInstalledASCOMPlatformVersion()
		{
			CheckASCOMPlatformVersion();

			if (s_ASCOMPlatformVersion.HasValue)
				return s_ASCOMPlatformVersion.Value.ToString(CultureInfo.InvariantCulture);
			else
				return null;
		}

		public static float? s_ASCOMPlatformVersion;

		private static void CheckASCOMPlatformVersion()
		{
			if (!s_ASCOMPlatformVersion.HasValue)
			{
				try
				{
					Type comType = Type.GetTypeFromProgID("ASCOM.Utilities.Util");

					if (comType != null)
					{
						// Create an instance.
						dynamic instance = Activator.CreateInstance(comType);

						string ascomVersionStr = instance.PlatformVersion;
						s_ASCOMPlatformVersion = float.Parse(ascomVersionStr.Replace(",", "."), CultureInfo.InvariantCulture);
					}
				}
				catch (Exception ex)
				{
					Trace.WriteLine(ex.GetFullStackTrace());
				}
			}			
		}
	}
}
