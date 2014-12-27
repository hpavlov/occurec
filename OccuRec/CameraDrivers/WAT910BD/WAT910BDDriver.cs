/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using OccuRec.Utilities;

namespace OccuRec.CameraDrivers.WAT910BD
{
	public class WAT910DBEventArgs : EventArgs
	{
		public bool IsSuccessful;
		public bool ReceivedResponseFromCamera;
		public string ErrorMessage;
		public string CommandId;
	}

    public class SerialCommsEventArgs : EventArgs
    {
        public byte[] Data;
        public bool Received;
        public bool Sent;
	    public string Message;
    }

	public class WAT910BDCameraState
	{
		public int GainIndex;
		public int GammaIndex;
		public int ExposureIndex;
		public bool GainSuccess;
		public bool GammaSuccess;
		public bool ExposureSuccess;
		public string Gain;
		public string Gamma;
		public string Exposure;
	}

    [Flags]
    public enum CameraStateQuery
    {
        Shutter = 1,
        Gain = 2,
        Gamma = 4,
        All = Shutter | Gain | Gamma
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
			SendCameraCommandSeries(MultipleCommandSeries.InitCamera, false);
		}

		public void ReadCurrentCameraSettings()
		{
			SendCameraCommandSeries(MultipleCommandSeries.ReadCameraSettings, true);
		}

		public WAT910BDCameraState ReadCurrentCameraState(CameraStateQuery query)
		{
			var rv = new WAT910BDCameraState();

		    List<WAT910BDCommandWithResponse> executedCommands = new List<WAT910BDCommandWithResponse>();

            WAT910BDCommandWithResponse cmd;

            if (query == CameraStateQuery.All)
                executedCommands = SendCameraCommandSeries(MultipleCommandSeries.ReadCameraState, true);
            else if ((query & CameraStateQuery.Gamma) == CameraStateQuery.Gamma)
            {
                cmd = new WAT910BDCommandWithResponse()
                {
                    Command = WAT910BDCommand.ReadGamma,
                    CommandBytes = m_StateMachine.BuildReadCommand(WAT910BDCommand.ReadGamma)
                };
                if (SendReadCommand(cmd.CommandBytes, cmd.Command.ToString()))
                {
                    cmd.Response = m_StateMachine.LastReceivedMessage();
                    executedCommands.Add(cmd);
                }
            }
            else if ((query & CameraStateQuery.Gain) == CameraStateQuery.Gain)
            {
                cmd = new WAT910BDCommandWithResponse()
                {
                    Command = WAT910BDCommand.ReadGain,
                    CommandBytes = m_StateMachine.BuildReadCommand(WAT910BDCommand.ReadGain)
                };
                if (SendReadCommand(cmd.CommandBytes, cmd.Command.ToString()))
                {
                    cmd.Response = m_StateMachine.LastReceivedMessage();
                    executedCommands.Add(cmd);
                }
            }
            else if ((query & CameraStateQuery.Shutter) == CameraStateQuery.Shutter)
            {
                cmd = new WAT910BDCommandWithResponse()
                {
                    Command = WAT910BDCommand.ReadShutter,
                    CommandBytes = m_StateMachine.BuildReadCommand(WAT910BDCommand.ReadShutter)
                };
                if (SendReadCommand(cmd.CommandBytes, cmd.Command.ToString()))
                {
                    cmd.Response = m_StateMachine.LastReceivedMessage();
                    executedCommands.Add(cmd);
                }
            }

            byte gammaByte = 0;
		    byte gainByte = 0;
		    byte shuterByte = 0;

            if ((query & CameraStateQuery.Gamma) == CameraStateQuery.Gamma)
            {
                cmd = executedCommands.SingleOrDefault(x => x.Command == WAT910BDCommand.ReadGamma);
                if (cmd != null && cmd.Response != null)
                {
                    rv.GammaSuccess = cmd.Response != null && cmd.Response.Length == 3;
                    gammaByte = rv.GammaSuccess ? cmd.Response[2] : (byte)0;

                    rv.GammaIndex = m_StateMachine.GammaByteToIndex(gammaByte);
                    rv.Gamma = rv.GammaSuccess ? m_StateMachine.GammaByteToString(gammaByte) : string.Empty;
                    m_StateMachine.SetGammaIndex(rv.GammaIndex);
                }
            }
            if ((query & CameraStateQuery.Gain) == CameraStateQuery.Gain)
		    {
		        cmd = executedCommands.SingleOrDefault(x => x.Command == WAT910BDCommand.ReadGain);
                if (cmd != null && cmd.Response != null)
                {
                    rv.GainSuccess = cmd.Response != null && cmd.Response.Length == 3;
                    gainByte = rv.GainSuccess ? cmd.Response[2] : (byte)0;

                    rv.GainIndex = m_StateMachine.GainByteToIndex(gainByte);
                    rv.Gain = rv.GainSuccess ? m_StateMachine.GainByteToString(gainByte) : string.Empty;
                    m_StateMachine.SetGainIndex(rv.GainIndex);                    
                }
		    }
            if ((query & CameraStateQuery.Shutter) == CameraStateQuery.Shutter)
		    {
		        cmd = executedCommands.SingleOrDefault(x => x.Command == WAT910BDCommand.ReadShutter);
                if (cmd != null && cmd.Response != null)
                {
                    rv.ExposureSuccess = cmd.Response != null && cmd.Response.Length == 3;
                    shuterByte = rv.ExposureSuccess ? cmd.Response[2] : (byte)0;

                    rv.ExposureIndex = m_StateMachine.ExposureByteToIndex(shuterByte);
                    rv.Exposure = rv.ExposureSuccess ? m_StateMachine.ExposureByteToString(shuterByte) : string.Empty;
                    m_StateMachine.SetExposureIndex(rv.ExposureIndex);
                }
		    }

#if DEBUG
            Trace.WriteLine(string.Format("gainByte = {0} gammaByte = {1} shuterByte = {2}", gainByte, gammaByte, shuterByte));
#endif
			
			return rv;
		}

		private List<WAT910BDCommandWithResponse> SendCameraCommandSeries(MultipleCommandSeries commandSeries, bool readCommands)
		{
			if (m_StateMachine.CanSendCommand() && IsConnected)
			{
				m_StateMachine.StartSendingMultipleCommands(commandSeries);

				try
				{
					List<WAT910BDCommandWithResponse> commands = m_StateMachine.GetCommandSeries(commandSeries);

					for (int i = 0; i < commands.Count; i++)
					{
						WAT910BDCommandWithResponse command = commands[i];

						if (readCommands)
						{
                            if (!SendReadCommand(command.CommandBytes, command.Command.ToString()))
								// One of the commands errored. Aborting
								break;

							command.Response = m_StateMachine.LastReceivedMessage();
						}
						else
						{
                            if (!SendWriteCommand(command.CommandBytes, command.Command.ToString()))
								// One of the commands errored. Aborting
								break;

							command.Response = m_StateMachine.LastReceivedMessage();
						}
					}

					return commands;
				}
				finally
				{
					m_StateMachine.FinishedSendingMultipleCommands(commandSeries);
					RaiseOnExecutionCompeted();
				}
			}

			return null;
		}

		public void OSDCommandUp()
		{
            DoCameraOperation(() => SendWriteCommand(m_StateMachine.BuildOsdCommand(OsdOperation.Up), "OSD-Up"));
		}

		public void OSDCommandDown()
		{
            DoCameraOperation(() => SendWriteCommand(m_StateMachine.BuildOsdCommand(OsdOperation.Down), "OSD-Down"));
		}

		public void OSDCommandLeft()
		{
            DoCameraOperation(() => SendWriteCommand(m_StateMachine.BuildOsdCommand(OsdOperation.Left), "OSD-Left"));
		}

		public void OSDCommandRight()
		{
            DoCameraOperation(() => SendWriteCommand(m_StateMachine.BuildOsdCommand(OsdOperation.Right), "OSD-Right"));
		}

		public void OSDCommandSet()
		{
            DoCameraOperation(() => SendWriteCommand(m_StateMachine.BuildOsdCommand(OsdOperation.Set), "OSD-Set"));
		}

        public void GainUp()
        {
            DoCameraOperation(() => m_StateMachine.SetGain(m_SerialPort, m_StateMachine.Gain + 1, (command) => RaiseOnCommsData(false, command, "Gain Up")));
        }

        public void GainDown()
        {
            DoCameraOperation(() => m_StateMachine.SetGain(m_SerialPort, m_StateMachine.Gain - 1, (command) => RaiseOnCommsData(false, command, "Gain Down")));
        }

		public void ExposureDown()
		{
            DoCameraOperation(() => m_StateMachine.SetExposure(m_SerialPort, m_StateMachine.ExposureIndex - 1, (command) => RaiseOnCommsData(false, command, "Exposure Down")));
		}

		public void ExposureUp()
		{
            DoCameraOperation(() => m_StateMachine.SetExposure(m_SerialPort, m_StateMachine.ExposureIndex + 1, (command) => RaiseOnCommsData(false, command, "Exposure Up")));
		}

		public void GammaDown()
		{
            DoCameraOperation(() => m_StateMachine.SetGamma(m_SerialPort, m_StateMachine.GammaIndex - 1, (command) => RaiseOnCommsData(false, command, "Gamma Down")));
		}

		public void GammaUp()
		{
            DoCameraOperation(() => m_StateMachine.SetGamma(m_SerialPort, m_StateMachine.GammaIndex + 1, (command) => RaiseOnCommsData(false, command, "Gamma Up")));
		}

		private void DoCameraOperation(Action operation)
		{
			if (m_StateMachine.CanSendCommand() && IsConnected)
			{
				try
				{
					operation();
				}
				finally
				{
					RaiseOnExecutionCompeted();
				}
			}			
		}

        private bool SendWriteCommand(byte[] command, string commandId)
        {
            return m_StateMachine.SendWriteCommandAndWaitToExecute(m_SerialPort, command, commandId, (cmd) => RaiseOnCommsData(false, cmd, commandId));
        }

        private bool SendReadCommand(byte[] command, string commandId)
        {
            return m_StateMachine.SendReadCommandAndWaitToExecute(m_SerialPort, command, commandId, (cmd) => RaiseOnCommsData(false, cmd, commandId));
        }

	    public string Gain
	    {
            get { return string.Format("{0} dB",  m_StateMachine.Gain); }
	    }

		public int GainIndex
		{
			get { return m_StateMachine.Gain; }
		}

		public int MinGainIndex
		{
			get { return WAT910BDStateMachine.MIN_GAIN; }
		}

		public int MaxGainIndex
		{
			get { return WAT910BDStateMachine.MAX_GAIN; }
		}

		public string Exposure
		{
			get { return m_StateMachine.Exposure; }
		}

		public int ExposureIndex
		{
			get { return m_StateMachine.ExposureIndex; }
		}

		public int MinExposureIndex
		{
			get { return WAT910BDStateMachine.MIN_EXPOSURE; }
		}

		public int MaxExposureIndex
		{
			get { return WAT910BDStateMachine.MAX_EXPOSURE; }
		}

		public string Gamma
		{
			get { return m_StateMachine.Gamma; }
		}

		public int GammaIndex
		{
			get { return m_StateMachine.GammaIndex; }
		}

		public int MinGammaIndex
		{
			get { return WAT910BDStateMachine.MIN_GAMMA; }
		}

		public int MaxGammaIndex
		{
			get { return WAT910BDStateMachine.MAX_GAMMA; }
		}

		private void RaiseOnExecutionCompeted()
		{
            EventHelper.RaiseEvent(OnCommandExecutionCompleted, 
                new WAT910DBEventArgs()
				{
					IsSuccessful = m_StateMachine.WasLastCameraOperationSuccessful(),
					ReceivedResponseFromCamera = m_StateMachine.DidLastCameraOperationReceivedResponse(),
                    CommandId = m_StateMachine.GetLastAttemptedCameraCommand(),
					ErrorMessage = m_StateMachine.GetLastCameraErrorMessage()
				});
		}

        private void RaiseOnCommsData(bool received, byte[] data, string sentCommand = null)
        {
            EventHelper.RaiseEvent(OnSerialComms, 
                new SerialCommsEventArgs()
                {
					Data = data,
					Received = received,
					Sent = !received,
                    Message = received ? m_StateMachine.GetLastCameraErrorMessage() : sentCommand
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
					    m_StateMachine.PushReceivedByte((byte) bt);

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
	}
}
