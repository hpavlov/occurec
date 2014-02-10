using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;

namespace WAT910BD.Tester.WAT910BDComms
{
	public class WAT910DBEventArgs : EventArgs
	{
		public bool IsSuccessful;
		public string ErrorMessage;
		public string CommandId;
	}

	public class WAT910BDDriver : IDisposable
	{
		private object m_SyncRoot = new object();
		private SerialPort m_SerialPort = null;
		private WAT910BDStateMachine m_StateMachine;

		public delegate void CommandExecutionCompletedCallback(WAT910DBEventArgs e);

		public event CommandExecutionCompletedCallback OnCommandExecutionCompleted;

		public WAT910BDDriver()
		{
			m_SerialPort = new SerialPort();
			m_SerialPort.ReadTimeout = 10;
			m_SerialPort.DataReceived +=m_SerialPort_DataReceived;
			m_SerialPort.ErrorReceived += m_SerialPort_ErrorReceived;
		}

		public bool Connect(string comPort)
		{
			return OpenPort(comPort);
		}

		public bool Disconnect()
		{
			return ClosePort();
		}

		public bool IsConnected
		{
			get
			{
				lock (m_SyncRoot)
				{
					return m_SerialPort != null && m_SerialPort.IsOpen;
				}
			}
		}

		public void InitialiseCamera()
		{
			if (m_StateMachine.CanSendCommand() && IsConnected)
			{
				m_StateMachine.StartSendingMultipleCommands(MultipleCommandSeries.InitCamera);

				try
				{
					List<byte[]> commands = m_StateMachine.GetCommandSeries(MultipleCommandSeries.InitCamera);

					foreach (byte[] command in commands)
					{
						if (!m_StateMachine.SendWriteCommandAndWaitToExecute(m_SerialPort, command))
						{
							// One of the commands errored. Aborting
							break;
						}
					}
				}
				finally
				{
					m_StateMachine.FinishedSendingMultipleCommands(MultipleCommandSeries.InitCamera);
					RaiseOnExecutionCompeted();
				}
			}
		}

		public void OSDCommandUp()
		{
			if (m_StateMachine.CanSendCommand() && IsConnected)
			{
				try
				{
					m_StateMachine.SendWriteCommandAndWaitToExecute(m_SerialPort, m_StateMachine.BuildOsdCommand(OsdOperation.Up));
				}
				finally
				{
					RaiseOnExecutionCompeted();
				}
			}			
		}

		public void OSDCommandDown()
		{
			if (m_StateMachine.CanSendCommand() && IsConnected)
			{
				try
				{
					m_StateMachine.SendWriteCommandAndWaitToExecute(m_SerialPort, m_StateMachine.BuildOsdCommand(OsdOperation.Down));
				}
				finally
				{
					RaiseOnExecutionCompeted();
				}
			}
		}

		public void OSDCommandLeft()
		{
			if (m_StateMachine.CanSendCommand() && IsConnected)
			{
				try
				{
					m_StateMachine.SendWriteCommandAndWaitToExecute(m_SerialPort, m_StateMachine.BuildOsdCommand(OsdOperation.Left));
				}
				finally
				{
					RaiseOnExecutionCompeted();
				}
			}
		}

		public void OSDCommandRight()
		{
			if (m_StateMachine.CanSendCommand() && IsConnected)
			{
				try
				{
					m_StateMachine.SendWriteCommandAndWaitToExecute(m_SerialPort, m_StateMachine.BuildOsdCommand(OsdOperation.Right));
				}
				finally
				{
					RaiseOnExecutionCompeted();
				}
			}
		}

		public void OSDCommandSet()
		{
			if (m_StateMachine.CanSendCommand() && IsConnected)
			{
				try
				{
					m_StateMachine.SendWriteCommandAndWaitToExecute(m_SerialPort, m_StateMachine.BuildOsdCommand(OsdOperation.Set));
				}
				finally
				{
					RaiseOnExecutionCompeted();
				}
			}
		}

		private void RaiseOnExecutionCompeted()
		{
			CommandExecutionCompletedCallback copy = OnCommandExecutionCompleted;

			if (copy != null)
			{
				copy.DynamicInvoke(new WAT910DBEventArgs()
				{
					IsSuccessful = m_StateMachine.WasLastCameraOperationSuccessful(),
					ErrorMessage = m_StateMachine.GetLastCameraErrorMessage()
				});				
			}
		}

		private void SendWriteCommand(byte[] commandBytes)
		{
			
		}

		void m_SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
		{
			lock (m_SyncRoot)
			{
				while (true)
				{
					try
					{
						int bt = m_SerialPort.ReadByte();
						m_StateMachine.PushReceivedByte((byte)bt);

					}
					catch (TimeoutException)
					{
						break;
					}
					catch (Exception ex)
					{
						// TODO: Raise on error?
						Trace.WriteLine(ex);
					}
				}
			}
		}


		void m_SerialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
		{
			throw new NotImplementedException();
		}

		private bool OpenPort(string comPort)
		{
			if (m_SerialPort != null && !m_SerialPort.IsOpen)
			{
				try
				{
					m_SerialPort.StopBits = StopBits.One;
					m_SerialPort.Parity = Parity.None;
					m_SerialPort.PortName = comPort;
					m_SerialPort.Open();

					return true;
				}
				catch (Exception ex)
				{
					Trace.WriteLine(ex);
				}
			}

			return false;
		}

		private bool ClosePort()
		{
			if (m_SerialPort != null && m_SerialPort.IsOpen)
			{
				try
				{
					m_SerialPort.Close();
					return true;
				}
				catch (Exception ex)
				{
					Trace.WriteLine(ex);
				}				
			}

			return false;
		}

		public void Dispose()
		{
			ClosePort();

			if (m_SerialPort != null)
				m_SerialPort.Dispose();

			m_SerialPort = null;
		}
	}
}
