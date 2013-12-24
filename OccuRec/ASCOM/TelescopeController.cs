using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using OccRec.ASCOMWrapper;
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
		MoveFocuserAndGetState
    }

	public struct Signal
	{
		public ControllerSignals Command;
		public object Argument;
	}

    public class TelescopeController : IDisposable
    {
        private bool m_Active = false;
        private readonly Control m_MainUIThreadControl = null;
        private readonly IASCOMDeviceCallbacks m_CallbacksObject = null;

        private IASCOMTelescope m_ConnectedTelescope = null;
        private IASCOMFocuser m_ConnectedFocuser = null;
		private ConcurrentQueue<Signal> m_QueuedSignals = new ConcurrentQueue<Signal>();

        public TelescopeController(Control mainForm, IASCOMDeviceCallbacks callbacks)
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

		public void FocuserMove(int step, Action<FocuserState> callback)
		{
			SignalMoveFocuser(step, callback);
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

                    if (nextOneMinCheckUTC <= DateTime.UtcNow)
                    {
                        PerformOneMinuteActions();
                        nextOneMinCheckUTC = DateTime.UtcNow.AddMinutes(1);
                    }
                }

                Thread.Sleep(1);
            }
        }

        private void PerformOneMinuteActions()
        {
            if (m_ConnectedTelescope != null)
            {
                if (m_ConnectedTelescope.Connected)
                {
                    TelescopeState state = m_ConnectedTelescope.GetCurrentState();
                    OnTelescopeState(state);
                }
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

                    if (m_ConnectedTelescope != null && !m_ConnectedTelescope.Connected)
                    {
						OnTelescopeConnecting();
                        m_ConnectedTelescope.Connected = true;
                        OnTelescopeConnected();

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
                        m_ConnectedFocuser = ASCOMClient.Instance.CreateFocuser(Settings.Default.ASCOMProgIdFocuser);

					if (m_ConnectedFocuser != null && !m_ConnectedFocuser.Connected)
                    {
						OnFocuserConnecting();
                        m_ConnectedFocuser.Connected = true;
                        OnFocuserConnected();
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
			else if (signal.Command == ControllerSignals.MoveFocuserAndGetState)
			{
				FocuserCommands.MoveFocuserAndGetState(signal, m_ConnectedFocuser);
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

		private void SignalMoveFocuser(int step, Action<FocuserState> callback)
		{
			m_QueuedSignals.Enqueue(new Signal() { Command = ControllerSignals.MoveFocuserAndGetState, Argument = new Tuple<int, Action<FocuserState>>(step, callback)});
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
    }
}
