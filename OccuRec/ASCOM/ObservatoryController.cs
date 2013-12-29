using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using OccRec.ASCOMWrapper;
using OccRec.ASCOMWrapper.Devices;
using OccRec.ASCOMWrapper.Interfaces;
using OccuRec.ASCOM.Interfaces.Devices;
using OccuRec.Helpers;
using OccuRec.Properties;

namespace OccuRec.ASCOM
{
    public enum ControllerSignals
    {
        TryConnectTelescope,
        TryConnectFocuser,
        GetFocuserState,
        GetTelescopeState,
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

    public class ObservatoryController : IDisposable
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
        

        public void CheckASCOMConnections()
        {
            if (Settings.Default.ASCOMConnectWhenRunning)
            {
                SignalTryConnectTelescope();
                SignalTryConnectFocuser();
            }
        }

        public void DisconnectASCOMDevices()
        {
            Trace.WriteLine(string.Format("OccuRec: ObservatoryController::DisconnectASCOMDevices[telcon:{0},foccon:{1}]", m_ConnectedTelescope != null, m_ConnectedFocuser != null));

            if (m_ConnectedTelescope != null)
            {
                m_MainUIThreadControl.Invoke(new Action(delegate()
                {
                    ASCOMClient.Instance.DisconnectTelescope(m_ConnectedTelescope);
                    m_ConnectedTelescope = null;
                    OnTelescopeDisconnected();
                }));
            }

            if (m_ConnectedFocuser != null)
            {
                m_MainUIThreadControl.Invoke(new Action(delegate()
                {
                    ASCOMClient.Instance.DisconnectFocuser(m_ConnectedFocuser);
                    m_ConnectedFocuser = null;
                    OnFocuserDisconnected();
                }));
            }
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
                        m_ConnectedTelescope = ASCOMClient.Instance.CreateTelescope(Settings.Default.ASCOMProgIdTelescope);

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

        private void OnTelescopeConnecting()
        {
            m_MainUIThreadControl.Invoke(
                new Action(
                    () => m_CallbacksObject.TelescopeConnectionChanged(ASCOMConnectionState.Connecting))
            );
        }

        private void OnFocuserConnecting()
        {
            m_MainUIThreadControl.Invoke(
                new Action(
                    () => m_CallbacksObject.FocuserConnectionChanged(ASCOMConnectionState.Connecting))
            );
        }

        private void OnTelescopeConnected()
        {
            m_MainUIThreadControl.Invoke(
                new Action(
                    () => m_CallbacksObject.TelescopeConnectionChanged(ASCOMConnectionState.Connected))
            );
        }

        private void OnFocuserConnected()
        {
            m_MainUIThreadControl.Invoke(
                new Action(
                    () => m_CallbacksObject.FocuserConnectionChanged(ASCOMConnectionState.Connected))
            );
        }

        private void OnTelescopeDisconnected()
        {
            m_MainUIThreadControl.Invoke(
                new Action(
                    () => m_CallbacksObject.TelescopeConnectionChanged(ASCOMConnectionState.Disconnected))
            );
        }

        private void OnFocuserDisconnected()
        {
            m_MainUIThreadControl.Invoke(
                new Action(
                    () => m_CallbacksObject.FocuserConnectionChanged(ASCOMConnectionState.Disconnected))
            );
        }

        private void OnTelescopeErrored()
        {
            m_MainUIThreadControl.Invoke(
                new Action(
                    () => m_CallbacksObject.TelescopeConnectionChanged(ASCOMConnectionState.Errored))
            );
        }

        private void OnFocuserErrored()
        {
            m_MainUIThreadControl.Invoke(
                new Action(
                    () => m_CallbacksObject.FocuserConnectionChanged(ASCOMConnectionState.Errored))
            );
        }

        private void OnTelescopeState(TelescopeState state)
        {
            m_MainUIThreadControl.Invoke(
                new Action(
                    () => m_CallbacksObject.TelescopeStateUpdate(state))
            );
        }

        private void OnFocuserState(FocuserState state)
        {
            m_MainUIThreadControl.Invoke(
                new Action(
                    () => m_CallbacksObject.FocuserStateUpdated(state))
            );
        }
        
    }
}
