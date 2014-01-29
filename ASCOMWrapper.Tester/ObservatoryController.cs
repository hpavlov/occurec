using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ASCOMWrapper.Tester
{
	public enum ControllerSignals
	{
		TryConnectFocuser,
		GetFocuserState,
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

	public enum ASCOMConnectionState
	{
		Connecting,
		Connected,
		Disconnecting,
		Disconnected,
		NotResponding,
		Errored
	}

	public interface IASCOMDeviceCallbacks
	{
		void FocuserConnectionChanged(ASCOMConnectionState state);
	}

	public class ObservatoryController : IDisposable
	{
		private bool m_Active = false;
		private readonly Control m_MainUIThreadControl = null;
		private readonly IASCOMDeviceCallbacks m_CallbacksObject = null;

		private string m_ASCOMProgIdFocuser;

		private global::ASCOM.DriverAccess.Focuser m_Focuser;
		private global::ASCOM.DriverAccess.Video m_Video;
		private ConcurrentQueue<Signal> m_QueuedSignals = new ConcurrentQueue<Signal>();

		public ObservatoryController(Control mainForm, IASCOMDeviceCallbacks callbacks, string focuserProgId)
		{
			m_MainUIThreadControl = mainForm;
			m_CallbacksObject = callbacks;

			m_Active = true;
			m_ASCOMProgIdFocuser = focuserProgId;

			ThreadPool.QueueUserWorkItem(WorkerThread);
		}

		private void WorkerThread(object state)
		{
			//DeviceChangeNotifier notifier = new DeviceChangeNotifier();

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

				Thread.Sleep(1);
			}
		}

		public void TryConnectFocuser()
		{
			SignalTryConnectFocuser();
		}

		private void ProcessSignal(Signal signal)
		{
			if (signal.Command == ControllerSignals.TryConnectFocuser)
			{
				try
				{
					if (m_Focuser == null && !string.IsNullOrEmpty(m_ASCOMProgIdFocuser))
					{
						 //m_Focuser = new global::ASCOM.DriverAccess.Focuser(m_ASCOMProgIdFocuser);
						 m_Video = new global::ASCOM.DriverAccess.Video(m_ASCOMProgIdFocuser);
					}

					if (m_Focuser != null)
					{
						if (!m_Focuser.Connected)
						{
							OnFocuserConnecting();
							m_Focuser.Connected = true;
							OnFocuserConnected();
						}
					}

					if (m_Video != null)
					{
						if (!m_Video.Connected)
						{
							OnFocuserConnecting();
							m_Video.Connected = true;
							OnFocuserConnected();

							m_Video.ConfigureDeviceProperties();
						}
					}
				}
				catch (Exception ex)
				{
					OnFocuserErrored();
					Trace.WriteLine(ex);
				}
			}
		}

		public void Dispose()
		{
			m_Active = false;
			Thread.Sleep(100);
		}

		private void SignalTryConnectFocuser()
		{
			m_QueuedSignals.Enqueue(new Signal() { Command = ControllerSignals.TryConnectFocuser });
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

		private void OnFocuserConnected()
		{
			ShieldedInvoke(() =>
				m_MainUIThreadControl.Invoke(
					new Action(
						() => m_CallbacksObject.FocuserConnectionChanged(ASCOMConnectionState.Connected))
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

	public class DeviceChangeNotifier : NativeWindow
	{
		public DeviceChangeNotifier()
		{
			Thread t = new Thread(StartListening);
			t.SetApartmentState(ApartmentState.STA);
			t.IsBackground = true;
			t.Start();
		}

		internal void StartListening()
		{
			ApplicationContext ctx = new ApplicationContext();
			CreateParams cp = new CreateParams();
			cp.Parent = (IntPtr)(-3); //Without this, WndProc will also intercept messages delivered to regular windows forms
			this.CreateHandle(cp);
			Application.Run(ctx);
		}

		private IntPtr thisHandle;
		protected override void OnHandleChange()
		{
			thisHandle = this.Handle;
			//RegisterForUsbEvents(this.Handle);
		}

		protected override void WndProc(ref Message m)
		{
			if (m.Msg == 0x219)
			{
				//m_Video.ConfigureDeviceProperties();
			}

			base.WndProc(ref m);
		}
	}
}
