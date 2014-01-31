using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using OccuRec.ASCOM.Wrapper;
using OccuRec.ASCOM.Wrapper.Devices;
using OccuRec.ASCOM.Wrapper.Interfaces;
using OccuRec.ASCOM.Interfaces.Devices;
using OccuRec.Helpers;
using OccuRec.Properties;
using OccuRec.Utilities;

namespace OccuRec.ASCOM
{
    public enum ASCOMConnectionState
    {
        Connecting,
        Connected,
        Disconnecting,
        Disconnected,
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

	public interface IObservatoryController
	{
        event Action<ASCOMConnectionState> TelescopeConnectionChanged;
        event Action<ASCOMConnectionState> FocuserConnectionChanged;
        event Action<TelescopeState> TelescopeStateUpdated;
        event Action<FocuserState> FocuserStateUpdated;

		bool IsConnectedToObservatory();
		bool IsConnectedToTelescope();
		bool IsConnectedToFocuser();
		void DisconnectTelescope(CallType callType = CallType.Async, CallbackAction callback = null, Control callbackUIControl = null);
		void DisconnectFocuser(CallType callType = CallType.Async, CallbackAction callback = null, Control callbackUIControl = null);
		void TryConnectFocuser(CallType callType = CallType.Async, CallbackAction callback = null, Control callbackUIControl = null);
		void TryConnectTelescope(CallType callType = CallType.Async, CallbackAction callback = null, Control callbackUIControl = null);
        void TelescopePulseGuide(GuideDirections direction, PulseRate pulseRate, CallType callType = CallType.Async, CallbackAction callback = null, Control callbackUIControl = null);
        void FocuserMoveIn(FocuserStepSize stepSize, CallType callType = CallType.Async, CallbackAction callback = null, Control callbackUIControl = null);
        void FocuserMoveOut(FocuserStepSize stepSize, CallType callType = CallType.Async, CallbackAction callback = null, Control callbackUIControl = null);
        void FocuserMove(int step, CallType callType = CallType.Async, CallbackAction callback = null, Control callbackUIControl = null);
        void FocuserSetTempComp(bool useTempComp, CallType callType = CallType.Async, CallbackAction callback = null, Control callbackUIControl = null);
        void GetFocuserState(CallType callType = CallType.Async, CallbackAction callback = null, Control callbackUIControl = null);
	}

	internal class ObservatoryController : ThreadIsolatedInvoker, IObservatoryController, IDisposable
	{
		private ITelescope m_ConnectedTelescope = null;
		private IFocuser m_ConnectedFocuser = null;

        public event Action<ASCOMConnectionState> TelescopeConnectionChanged;
        public event Action<ASCOMConnectionState> FocuserConnectionChanged;
        public event Action<TelescopeState> TelescopeStateUpdated;
        public event Action<FocuserState> FocuserStateUpdated;

		public void TryConnectTelescope(CallType callType = CallType.Async, CallbackAction callback = null, Control callbackUIControl = null)
		{
			IsolatedAction(() =>
			{
				try
				{
					if (m_ConnectedTelescope != null && m_ConnectedTelescope.ProgId != Settings.Default.ASCOMProgIdTelescope)
					{
						ASCOMClient.Instance.DisconnectTelescope(m_ConnectedTelescope);
						m_ConnectedTelescope = null;
					}

					if (m_ConnectedTelescope == null && !string.IsNullOrEmpty(Settings.Default.ASCOMProgIdTelescope))
						m_ConnectedTelescope = ASCOMClient.Instance.CreateTelescope(Settings.Default.ASCOMProgIdTelescope, Settings.Default.TelPulseSlowestRate, Settings.Default.TelPulseSlowRate, Settings.Default.TelPulseFastRate);

					if (m_ConnectedTelescope != null)
					{
						if (!m_ConnectedTelescope.Connected)
						{
							OnTelescopeConnecting();
							m_ConnectedTelescope.Connected = true;
							OnTelescopeConnected();
						}

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

		public void TryConnectFocuser(CallType callType = CallType.Async, CallbackAction callback = null, Control callbackUIControl = null)
		{
			IsolatedAction(() =>
			{
				try
				{
					if (m_ConnectedFocuser != null && m_ConnectedFocuser.ProgId != Settings.Default.ASCOMProgIdFocuser)
					{
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

		public bool IsConnectedToFocuser()
		{
			return m_ConnectedFocuser != null;
		}

		public void DisconnectTelescope(CallType callType = CallType.Async, CallbackAction callback = null, Control callbackUIControl = null)
		{
			IsolatedAction(() =>
			{
				try
				{
					if (m_ConnectedTelescope != null)
					{
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

		public void DisconnectFocuser(CallType callType = CallType.Async, CallbackAction callback = null, Control callbackUIControl = null)
		{
			IsolatedAction(() =>
			{
				try
				{
					if (m_ConnectedFocuser != null)
					{
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

        public void TelescopePulseGuide(GuideDirections direction, PulseRate pulseRate, CallType callType = CallType.Async, CallbackAction callback = null, Control callbackUIControl = null)
        {
            IsolatedAction((x, y) =>
            {
                try
                {
                    if (m_ConnectedTelescope != null)
                    {
                        m_ConnectedTelescope.PulseGuide(x, y, Settings.Default.TelPulseDuration);
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

        public void FocuserMoveIn(FocuserStepSize stepSize, CallType callType = CallType.Async, CallbackAction callback = null, Control callbackUIControl = null)
        {
            IsolatedAction((x) =>
            {
                try
                {
                    if (m_ConnectedFocuser != null && m_ConnectedFocuser.Connected)
                    {
                        m_ConnectedFocuser.MoveIn(x);

                        FocuserState state = m_ConnectedFocuser.GetCurrentState();

                        OnFocuserState(state);

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
            stepSize, callType, callback, callbackUIControl);
            
        }

        public void FocuserMoveOut(FocuserStepSize stepSize, CallType callType = CallType.Async, CallbackAction callback = null, Control callbackUIControl = null)
        {
            IsolatedAction((x) =>
            {
                try
                {
                    if (m_ConnectedFocuser != null && m_ConnectedFocuser.Connected)
                    {
                        m_ConnectedFocuser.MoveOut(x);

                        FocuserState state = m_ConnectedFocuser.GetCurrentState();

                        OnFocuserState(state);

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
            stepSize, callType, callback, callbackUIControl);
        }

        public void FocuserMove(int step, CallType callType = CallType.Async, CallbackAction callback = null, Control callbackUIControl = null)
        {
            IsolatedAction((x) =>
            {
                try
                {
                    if (m_ConnectedFocuser != null && m_ConnectedFocuser.Connected)
                    {
                        m_ConnectedFocuser.Move(x);

                        FocuserState state = m_ConnectedFocuser.GetCurrentState();

                        OnFocuserState(state);

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
            step, callType, callback, callbackUIControl);
        }

        public void FocuserSetTempComp(bool useTempComp, CallType callType = CallType.Async, CallbackAction callback = null, Control callbackUIControl = null)
        {
            IsolatedAction((x) =>
            {
                try
                {
                    if (m_ConnectedFocuser != null && m_ConnectedFocuser.Connected)
                    {
                        m_ConnectedFocuser.ChangeTempComp(x);

                        FocuserState state = m_ConnectedFocuser.GetCurrentState();

                        OnFocuserState(state);

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
            useTempComp, callType, callback, callbackUIControl);
        }

        public void GetFocuserState(CallType callType = CallType.Async, CallbackAction callback = null, Control callbackUIControl = null)
        {
            IsolatedAction(() =>
            {
                try
                {
                    if (m_ConnectedFocuser != null && m_ConnectedFocuser.Connected)
                    {
                        FocuserState state = m_ConnectedFocuser.GetCurrentState();

                        OnFocuserState(state);

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



		#region UI Callbacks
		private void OnTelescopeConnecting()
		{
            RaiseEvent(TelescopeConnectionChanged, ASCOMConnectionState.Connecting);
		}

		private void OnFocuserConnecting()
		{
            RaiseEvent(FocuserConnectionChanged, ASCOMConnectionState.Connecting);
		}

		private void OnTelescopeConnected()
		{
            RaiseEvent(TelescopeConnectionChanged, ASCOMConnectionState.Connected);
		}

		private void OnFocuserConnected()
		{
            RaiseEvent(FocuserConnectionChanged, ASCOMConnectionState.Connected);
		}

		private void OnTelescopeDisconnected()
		{
            RaiseEvent(TelescopeConnectionChanged, ASCOMConnectionState.Disconnected);
		}

		private void OnFocuserDisconnected()
		{
            RaiseEvent(FocuserConnectionChanged, ASCOMConnectionState.Disconnected);
		}

		private void OnTelescopeErrored()
		{
            RaiseEvent(TelescopeConnectionChanged, ASCOMConnectionState.Errored);
		}

		private void OnFocuserErrored()
		{
            RaiseEvent(FocuserConnectionChanged, ASCOMConnectionState.Errored);
		}

		private void OnTelescopeState(TelescopeState state)
		{
            RaiseEvent(TelescopeStateUpdated, state);
		}

		private void OnFocuserState(FocuserState state)
		{
            RaiseEvent(FocuserStateUpdated, state);
		}

		#endregion

		public void Dispose()
		{ }
	}
}
