using System;
using System.Collections.Generic;
using System.ComponentModel;
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

    public class SerialCommsEventArgs : EventArgs
    {
        public byte[] Data;
        public bool Received;
        public bool Sent;
    }

	public class WAT910BDDriver : IDisposable
	{
		private object m_SyncRoot = new object();
		private SerialPort m_SerialPort = null;
		private WAT910BDStateMachine m_StateMachine;

		public delegate void CommandExecutionCompletedCallback(WAT910DBEventArgs e);
        public delegate void SerialCommsCallback(SerialCommsEventArgs e);

		public event CommandExecutionCompletedCallback OnCommandExecutionCompleted;
        public event SerialCommsCallback OnSerialComms;

		public WAT910BDDriver()
		{
            m_StateMachine = new WAT910BDStateMachine();

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
                        if (!SendWriteCommand(command))
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
			try
			{
			    if (m_StateMachine.CanSendCommand() && IsConnected)
			    {
			        SendWriteCommand(m_StateMachine.BuildOsdCommand(OsdOperation.Up));
			    }
			}
			finally
			{
				RaiseOnExecutionCompeted();
			}
		}

		public void OSDCommandDown()
		{
			if (m_StateMachine.CanSendCommand() && IsConnected)
			{
				try
				{
                    SendWriteCommand(m_StateMachine.BuildOsdCommand(OsdOperation.Down));
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
                    SendWriteCommand(m_StateMachine.BuildOsdCommand(OsdOperation.Left));
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
                    SendWriteCommand(m_StateMachine.BuildOsdCommand(OsdOperation.Right));
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
					SendWriteCommand(m_StateMachine.BuildOsdCommand(OsdOperation.Set));
				}
				finally
				{
					RaiseOnExecutionCompeted();
				}
			}
		}

        public void GainUp()
        {
            if (m_StateMachine.CanSendCommand() && IsConnected)
            {
                try
                {
                    m_StateMachine.SetGain(m_SerialPort, m_StateMachine.Gain + 1, (command) => RaiseOnCommsData(false, command));
                }
                finally
                {
                    RaiseOnExecutionCompeted();
                }
            }
        }

        public void GainDown()
        {
            if (m_StateMachine.CanSendCommand() && IsConnected)
            {
                try
                {
                    m_StateMachine.SetGain(m_SerialPort, m_StateMachine.Gain - 1, (command) => RaiseOnCommsData(false, command));
                }
                finally
                {
                    RaiseOnExecutionCompeted();
                }
            }
        }

        private bool SendWriteCommand(byte[] command)
        {
            return m_StateMachine.SendWriteCommandAndWaitToExecute(m_SerialPort, command, (cmd) => RaiseOnCommsData(false, cmd));
        }

	    public int Gain
	    {
            get { return m_StateMachine.Gain; }
	    }

		private void RaiseOnExecutionCompeted()
		{
			RaiseEvent(OnCommandExecutionCompleted, 
                new WAT910DBEventArgs()
				{
					IsSuccessful = m_StateMachine.WasLastCameraOperationSuccessful(),
					ErrorMessage = m_StateMachine.GetLastCameraErrorMessage()
				});
		}

        private void RaiseOnCommsData(bool received, byte[] data)
        {
            RaiseEvent(OnSerialComms, 
                new SerialCommsEventArgs()
                {
                    Data = data, Received = received, Sent = !received
                });
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

                        if (m_StateMachine.IsReceivedMessageComplete)
                        {
                            RaiseOnCommsData(true, m_StateMachine.LastReceivedMessage());
                        }
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
				    m_SerialPort.DataBits = 8;
					m_SerialPort.PortName = comPort;
				    m_SerialPort.BaudRate = 57600;
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

        /// <summary>Raises the event (on the UI thread if available).</summary>
        /// <param name="multicastDelegate">The event to raise.</param>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An EventArgs that contains the event data.</param>
        /// <returns>The return value of the event invocation or null if none.</returns>
        public static object RaiseEvent<T>(MulticastDelegate multicastDelegate, T eventArg)
        {
            object retVal = null;

            MulticastDelegate threadSafeMulticastDelegate = multicastDelegate;
            if (threadSafeMulticastDelegate != null)
            {
                foreach (Delegate d in threadSafeMulticastDelegate.GetInvocationList())
                {
                    var synchronizeInvoke = d.Target as ISynchronizeInvoke;
                    if ((synchronizeInvoke != null) && synchronizeInvoke.InvokeRequired)
                    {
                        retVal = synchronizeInvoke.EndInvoke(synchronizeInvoke.BeginInvoke(d, new object[] { eventArg }));
                    }
                    else
                    {
                        retVal = d.DynamicInvoke(new object[] { eventArg });
                    }
                }
            }

            return retVal;
        }
	}
}
