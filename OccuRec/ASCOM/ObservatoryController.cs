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
    public enum ControllerSignals
    {
        TryConnectTelescope,
        TryConnectFocuser,
        GetFocuserState,
        GetTelescopeState,
        GetTelescopeCapabilities,
        TelescopePulseGuide,
        MoveFocuserInAndGetState,
        MoveFocuserOutAndGetState,
        MoveFocuserAndGetState,
        FocuserSetTempCompAndGetState
    }

    public struct Signal
	{
		public ControllerSignals Command;
		public object Argument;
	}

    internal class ObservatoryController : ThreadIsolatedInvoker, IDisposable
    {
        private bool m_Active = false;
        private readonly Control m_MainUIThreadControl = null;
        private readonly IASCOMDeviceCallbacks m_CallbacksObject = null;

        private ITelescope m_ConnectedTelescope = null;
        private IFocuser m_ConnectedFocuser = null;
		private ConcurrentQueue<Signal> m_QueuedSignals = new ConcurrentQueue<Signal>();

		public ObservatoryController(Control mainForm, IASCOMDeviceCallbacks callbacks)
        {
            m_MainUIThreadControl = mainForm;
            m_CallbacksObject = callbacks;

            m_Active = true;
            ThreadPool.QueueUserWorkItem(WorkerThread);
        }
        
		public void GetFocuserState(Action<FocuserState> callback)
		{
			SignalGetFocuserState(callback);
		}

		public void FocuserMoveIn(FocuserStepSize stepSize, Action<FocuserState> callback)
		{
            SignalMoveFocuserIn(stepSize, callback);
		}

        public void FocuserMoveOut(FocuserStepSize stepSize, Action<FocuserState> callback)
        {
            SignalMoveFocuserOut(stepSize, callback);
        }

        public void FocuserSetTempComp(bool tempComp, Action<FocuserState> callback)
        {
            SignalFocuserSetTempComp(tempComp, callback);
        }

        public void FocuserMove(int position, Action<FocuserState> callback)
        {
            SignalMoveFocuser(position, callback);
        }

        public void TelescopePulseGuide(GuideDirections direction, PulseRate rate, Action callback)
        {
            SignalTelescopePulseGuide(direction, rate, Settings.Default.TelPulseDuration, callback);
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

        public void TryConnectTelescope()
        {
            SignalTryConnectTelescope();
        }

        public void DisconnectTelescope()
        {
            DisconnectTelescope(null);
        }

        public void DisconnectTelescope(Action onDisconnected)
        {
            if (m_ConnectedTelescope != null)
            {
                m_MainUIThreadControl.Invoke(new Action(delegate()
                {
                    ASCOMClient.Instance.DisconnectTelescope(m_ConnectedTelescope);
                    m_ConnectedTelescope = null;
                    OnTelescopeDisconnected();
                    if (onDisconnected != null) onDisconnected();
                }));
            }
        }

        public void TryConnectFocuser()
        {
            SignalTryConnectFocuser();
        }

        public void DisconnectFocuser()
        {
            DisconnectFocuser(null);
        }

        public void DisconnectFocuser(Action onDisconnected)
        {
            if (m_ConnectedFocuser != null)
            {
                m_MainUIThreadControl.Invoke(new Action(delegate()
                {
                    ASCOMClient.Instance.DisconnectFocuser(m_ConnectedFocuser);
                    m_ConnectedFocuser = null;
                    OnFocuserDisconnected();
                    if (onDisconnected != null) onDisconnected();
                }));
            }
        }

        public void DisconnectASCOMDevices()
        {
            DisconnectTelescope();

            DisconnectFocuser();
        }

        private void WorkerThread(object state)
        {
            DateTime nextOneMinCheckUTC = DateTime.UtcNow;

            while (m_Active)
            {
                if (m_QueuedSignals.Count > 0)
                {
					Signal signal;
                    if (m_QueuedSignals.TryDequeue(out signal))
                    {
                        ProcessSignal(signal);
                    }
                }

                if (nextOneMinCheckUTC <= DateTime.UtcNow)
                {
                    PerformTelescopePingActions();
                    PerformFocuserPingActions();

                    if (Settings.Default.TelescopePingRateSeconds > 0)
                        nextOneMinCheckUTC = DateTime.UtcNow.AddSeconds(Settings.Default.TelescopePingRateSeconds);
                }

                Thread.Sleep(1);
            }
        }

        private void PerformTelescopePingActions()
        {
            if (m_ConnectedTelescope != null && m_ConnectedTelescope.Connected)
            {
                TelescopeState state = m_ConnectedTelescope.GetCurrentState();
                OnTelescopeState(state);
            }
        }

        private void PerformFocuserPingActions()
        {
            if (m_ConnectedFocuser != null && m_ConnectedFocuser.Connected)
            {
                FocuserState state = m_ConnectedFocuser.GetCurrentState();
                OnFocuserState(state);
            }
        }

		private void ProcessSignal(Signal signal)
        {
            if (signal.Command == ControllerSignals.TryConnectTelescope)
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
                    }
                }
                catch (Exception ex)
                {
                    OnTelescopeErrored();
                    Trace.WriteLine(ex.GetFullStackTrace());
                }
            }
			else if (signal.Command == ControllerSignals.TryConnectFocuser)
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
                    }
                }
                catch (Exception ex)
                {
                    OnFocuserErrored();
                    Trace.WriteLine(ex.GetFullStackTrace());
                }
            }
			else if (signal.Command == ControllerSignals.GetFocuserState)
			{
				FocuserCommands.GetFocuserState(signal, m_ConnectedFocuser);
			}
            else if (signal.Command == ControllerSignals.GetTelescopeState)
            {
                TelescopeCommands.GetTelescopeState(signal, m_ConnectedTelescope);
            }
            else if (signal.Command == ControllerSignals.GetTelescopeCapabilities)
            {
                TelescopeCommands.GetTelescopeCapabilities(signal, m_ConnectedTelescope);
            }
            else if (signal.Command == ControllerSignals.TelescopePulseGuide)
            {
                TelescopeCommands.PulseGuide(signal, m_ConnectedTelescope);
            }
			else if (
                signal.Command == ControllerSignals.MoveFocuserInAndGetState ||
                signal.Command == ControllerSignals.MoveFocuserOutAndGetState ||
                signal.Command == ControllerSignals.MoveFocuserAndGetState)
			{
                FocuserCommands.MoveFocuserAndGetState(signal, signal.Command == ControllerSignals.MoveFocuserInAndGetState, m_ConnectedFocuser);
			}
        }

        public void Dispose()
        {
            m_Active = false;
            Thread.Sleep(100);
        }

        private void SignalTryConnectTelescope()
        {
            m_QueuedSignals.Enqueue(new Signal() { Command = ControllerSignals.TryConnectTelescope });
        }

        private void SignalTryConnectFocuser()
        {
			m_QueuedSignals.Enqueue(new Signal() { Command = ControllerSignals.TryConnectFocuser });
        }

		private void SignalGetFocuserState(Action<FocuserState> callback)
		{
			m_QueuedSignals.Enqueue(new Signal() { Command = ControllerSignals.GetFocuserState, Argument = callback });
		}

        private void SignalMoveFocuserIn(FocuserStepSize stepSize, Action<FocuserState> callback)
		{
            m_QueuedSignals.Enqueue(new Signal() { Command = ControllerSignals.MoveFocuserInAndGetState, Argument = new Tuple<FocuserStepSize, Action<FocuserState>>(stepSize, callback) });
		}

        private void SignalMoveFocuserOut(FocuserStepSize stepSize, Action<FocuserState> callback)
        {
            m_QueuedSignals.Enqueue(new Signal() { Command = ControllerSignals.MoveFocuserOutAndGetState, Argument = new Tuple<FocuserStepSize, Action<FocuserState>>(stepSize, callback) });
        }

        private void SignalMoveFocuser(int position, Action<FocuserState> callback)
        {
            m_QueuedSignals.Enqueue(new Signal() { Command = ControllerSignals.MoveFocuserAndGetState, Argument = new Tuple<int, Action<FocuserState>>(position, callback) });
        }

        private void SignalFocuserSetTempComp(bool tempComp, Action<FocuserState> callback)
        {
            m_QueuedSignals.Enqueue(new Signal() { Command = ControllerSignals.FocuserSetTempCompAndGetState, Argument = new Tuple<bool, Action<FocuserState>>(tempComp, callback) });
        }

        private void SignalTelescopePulseGuide(GuideDirections direction, PulseRate rate, int durationMilliseconds, Action callback)
        {
            m_QueuedSignals.Enqueue(new Signal() { Command = ControllerSignals.TelescopePulseGuide, Argument = new Tuple<GuideDirections, PulseRate, int, Action>(direction, rate, durationMilliseconds, callback) });
        }

        private void OnTelescopeConnecting()
        {
            ShieldedInvoke(() =>
                m_MainUIThreadControl.Invoke(
                    new Action(
                        () => m_CallbacksObject.TelescopeConnectionChanged(ASCOMConnectionState.Connecting))
                )
            );
        }

        private void OnFocuserConnecting()
        {
            ShieldedInvoke(() =>
                m_MainUIThreadControl.Invoke(
                    new Action(
                        () => m_CallbacksObject.FocuserConnectionChanged(ASCOMConnectionState.Connecting))
                )
            );
        }

        private void OnTelescopeConnected()
        {
            ShieldedInvoke(() =>
                m_MainUIThreadControl.Invoke(
                    new Action(
                        () => m_CallbacksObject.TelescopeConnectionChanged(ASCOMConnectionState.Connected))
                )
            );
        }

        private void OnFocuserConnected()
        {
            ShieldedInvoke(() =>
                m_MainUIThreadControl.Invoke(
                    new Action(
                        () => m_CallbacksObject.FocuserConnectionChanged(ASCOMConnectionState.Connected))
                )
            );
        }

        private void OnTelescopeDisconnected()
        {
            ShieldedInvoke(() =>
                m_MainUIThreadControl.Invoke(
                    new Action(
                        () => m_CallbacksObject.TelescopeConnectionChanged(ASCOMConnectionState.Disconnected))
                )
            );
        }

        private void OnFocuserDisconnected()
        {
            ShieldedInvoke(() =>
                m_MainUIThreadControl.Invoke(
                    new Action(
                        () => m_CallbacksObject.FocuserConnectionChanged(ASCOMConnectionState.Disconnected))
                )
            );
        }

        private void OnTelescopeErrored()
        {
            ShieldedInvoke(() =>
                m_MainUIThreadControl.Invoke(
                    new Action(
                        () => m_CallbacksObject.TelescopeConnectionChanged(ASCOMConnectionState.Errored))
                )
            );
        }

        private void OnFocuserErrored()
        {
            ShieldedInvoke(() =>
                m_MainUIThreadControl.Invoke(
                    new Action(
                        () => m_CallbacksObject.FocuserConnectionChanged(ASCOMConnectionState.Errored))
                )
            );
        }

        private void OnTelescopeState(TelescopeState state)
        {
            ShieldedInvoke(() =>
                m_MainUIThreadControl.Invoke(
                    new Action(
                        () => m_CallbacksObject.TelescopeStateUpdate(state))
                )
            );
        }

        private void OnFocuserState(FocuserState state)
        {
            ShieldedInvoke(() =>
                m_MainUIThreadControl.Invoke(
                  new Action(
                    () => m_CallbacksObject.FocuserStateUpdated(state))
                )
            );
        }

        private void ShieldedInvoke(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }
        }
    }

	/*

	internal class ObservatoryControllerNew : ThreadIsolatedInvoker, IDisposable
	{
		private readonly Control m_MainUIThreadControl = null;
		private readonly IASCOMDeviceCallbacks m_CallbacksObject = null;

		private ITelescope m_ConnectedTelescope = null;
		private IFocuser m_ConnectedFocuser = null;

		public ObservatoryController(Control mainForm, IASCOMDeviceCallbacks callbacks)
		{
			m_MainUIThreadControl = mainForm;
			m_CallbacksObject = callbacks;			
		}

		public void CheckASCOMConnections()
		{
			if (Settings.Default.ASCOMConnectWhenRunning)
			{
				IsolatedAction(() => TryConnectTelescope());
				IsolatedAction(() => TryConnectFocuser());
			}
		}

		public void TryConnectTelescope()
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
				}
			}
			catch (Exception ex)
			{
				OnTelescopeErrored();
				Trace.WriteLine(ex.GetFullStackTrace());
			}			
		}

		public void TryConnectFocuser()
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
				}
			}
			catch (Exception ex)
			{
				OnFocuserErrored();
				Trace.WriteLine(ex.GetFullStackTrace());
			}			
		}

		public void GetFocuserState(Action<FocuserState> callback)
		{
			IsolatedActionAsync(() =>
				{
					if (m_ConnectedFocuser != null && m_ConnectedFocuser.Connected)
					{
						FocuserState state = m_ConnectedFocuser.GetCurrentState();

						m_MainUIThreadControl.Invoke(new Action<FocuserState, Action<FocuserState>>((x, y) => ASCOMHelper.SafeCallbackActionCall(y, x)), callback, state);
					}
					else
					{
						m_MainUIThreadControl.Invoke(new Action<FocuserState, Action<FocuserState>>((x, y) => ASCOMHelper.SafeCallbackActionCall(y, x)), callback, null);
					}
				});
		}

		private void MoveFocuserAndGetState()
		{
			IsolatedActionAsync(() =>
				{
					try
					{
						if (signal.Command == ControllerSignals.MoveFocuserAndGetState)
						{
							var tuple = signal.Argument as Tuple<int, Action<FocuserState>>;
							Action<FocuserState> callback = tuple != null ? tuple.Item2 as Action<FocuserState> : null;
							int step = tuple != null ? (int)tuple.Item1 : 0;

							if (m_ConnectedFocuser != null && m_ConnectedFocuser.Connected)
							{
								m_ConnectedFocuser.Move(step);

								FocuserState state = m_ConnectedFocuser.GetCurrentState();

								ASCOMHelper.SafeCallbackActionCall(callback, state);
							}
							else
							{
								ASCOMHelper.SafeCallbackActionCall(callback, null);
							}
						}
						else
						{
							var tuple = signal.Argument as Tuple<FocuserStepSize, Action<FocuserState>>;
							Action<FocuserState> callback = tuple != null ? tuple.Item2 as Action<FocuserState> : null;
							FocuserStepSize stepSize = tuple != null ? (FocuserStepSize)tuple.Item1 : FocuserStepSize.Small;

							if (m_ConnectedFocuser != null && m_ConnectedFocuser.Connected)
							{
								if (moveIn)
									m_ConnectedFocuser.MoveIn(stepSize);
								else
									m_ConnectedFocuser.MoveOut(stepSize);

								FocuserState state = m_ConnectedFocuser.GetCurrentState();

								ASCOMHelper.SafeCallbackActionCall(callback, state);
							}
							else
							{
								ASCOMHelper.SafeCallbackActionCall(callback, null);
							}
						}

					}
					catch (Exception ex)
					{
						Trace.WriteLine(ex.GetFullStackTrace());
					}
					
				});
		}

		public void FocuserMoveIn(FocuserStepSize stepSize, Action<FocuserState> callback)
		{
			IsolatedActionAsync(() =>
			{
				if (m_ConnectedFocuser != null && m_ConnectedFocuser.Connected)
				{
					FocuserState state = m_ConnectedFocuser.GetCurrentState();

					m_MainUIThreadControl.Invoke(new Action<FocuserState, Action<FocuserState>>((x, y) => ASCOMHelper.SafeCallbackActionCall(y, x)), callback, state);
				}
				else
				{
					m_MainUIThreadControl.Invoke(new Action<FocuserState, Action<FocuserState>>((x, y) => ASCOMHelper.SafeCallbackActionCall(y, x)), callback, null);
				}
			});

			SignalMoveFocuserIn(stepSize, callback);
		}

		public void FocuserMoveOut(FocuserStepSize stepSize, Action<FocuserState> callback)
		{
			SignalMoveFocuserOut(stepSize, callback);
		}

		public void FocuserSetTempComp(bool tempComp, Action<FocuserState> callback)
		{
			SignalFocuserSetTempComp(tempComp, callback);
		}

		public void FocuserMove(int position, Action<FocuserState> callback)
		{
			SignalMoveFocuser(position, callback);
		}

		public void TelescopePulseGuide(GuideDirections direction, PulseRate rate, Action callback)
		{
			SignalTelescopePulseGuide(direction, rate, Settings.Default.TelPulseDuration, callback);
		}
		private void Test()
		{
			if (signal.Command == ControllerSignals.TryConnectTelescope)
			{
				
			}
			else if (signal.Command == ControllerSignals.TryConnectFocuser)
			{

			}
			else if (signal.Command == ControllerSignals.GetFocuserState)
			{
				
			}
			else if (signal.Command == ControllerSignals.GetTelescopeState)
			{
				TelescopeCommands.GetTelescopeState(signal, m_ConnectedTelescope);
			}
			else if (signal.Command == ControllerSignals.GetTelescopeCapabilities)
			{
				TelescopeCommands.GetTelescopeCapabilities(signal, m_ConnectedTelescope);
			}
			else if (signal.Command == ControllerSignals.TelescopePulseGuide)
			{
				TelescopeCommands.PulseGuide(signal, m_ConnectedTelescope);
			}
			else if (
				signal.Command == ControllerSignals.MoveFocuserInAndGetState ||
				signal.Command == ControllerSignals.MoveFocuserOutAndGetState ||
				signal.Command == ControllerSignals.MoveFocuserAndGetState)
			{
				FocuserCommands.MoveFocuserAndGetState(signal, signal.Command == ControllerSignals.MoveFocuserInAndGetState, m_ConnectedFocuser);
			}			
		}


		#region UI Callbacks
		private void OnTelescopeConnecting()
		{
			ShieldedInvoke(() =>
				m_MainUIThreadControl.Invoke(
					new Action(
						() => m_CallbacksObject.TelescopeConnectionChanged(ASCOMConnectionState.Connecting))
				)
			);
		}

		private void OnFocuserConnecting()
		{
			ShieldedInvoke(() =>
				m_MainUIThreadControl.Invoke(
					new Action(
						() => m_CallbacksObject.FocuserConnectionChanged(ASCOMConnectionState.Connecting))
				)
			);
		}

		private void OnTelescopeConnected()
		{
			ShieldedInvoke(() =>
				m_MainUIThreadControl.Invoke(
					new Action(
						() => m_CallbacksObject.TelescopeConnectionChanged(ASCOMConnectionState.Connected))
				)
			);
		}

		private void OnFocuserConnected()
		{
			ShieldedInvoke(() =>
				m_MainUIThreadControl.Invoke(
					new Action(
						() => m_CallbacksObject.FocuserConnectionChanged(ASCOMConnectionState.Connected))
				)
			);
		}

		private void OnTelescopeDisconnected()
		{
			ShieldedInvoke(() =>
				m_MainUIThreadControl.Invoke(
					new Action(
						() => m_CallbacksObject.TelescopeConnectionChanged(ASCOMConnectionState.Disconnected))
				)
			);
		}

		private void OnFocuserDisconnected()
		{
			ShieldedInvoke(() =>
				m_MainUIThreadControl.Invoke(
					new Action(
						() => m_CallbacksObject.FocuserConnectionChanged(ASCOMConnectionState.Disconnected))
				)
			);
		}

		private void OnTelescopeErrored()
		{
			ShieldedInvoke(() =>
				m_MainUIThreadControl.Invoke(
					new Action(
						() => m_CallbacksObject.TelescopeConnectionChanged(ASCOMConnectionState.Errored))
				)
			);
		}

		private void OnFocuserErrored()
		{
			ShieldedInvoke(() =>
				m_MainUIThreadControl.Invoke(
					new Action(
						() => m_CallbacksObject.FocuserConnectionChanged(ASCOMConnectionState.Errored))
				)
			);
		}

		private void OnTelescopeState(TelescopeState state)
		{
			ShieldedInvoke(() =>
				m_MainUIThreadControl.Invoke(
					new Action(
						() => m_CallbacksObject.TelescopeStateUpdate(state))
				)
			);
		}

		private void OnFocuserState(FocuserState state)
		{
			ShieldedInvoke(() =>
				m_MainUIThreadControl.Invoke(
				  new Action(
					() => m_CallbacksObject.FocuserStateUpdated(state))
				)
			);
		}

		#endregion

		public void Dispose()
		{ }
	}

	*/
}
