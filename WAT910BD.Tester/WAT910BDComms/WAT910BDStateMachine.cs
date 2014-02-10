using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace WAT910BD.Tester.WAT910BDComms
{
	#region Arduino Nano Protocol Specs
	//
	//an Arduino Nano (328) program to interface between a PC and a Watec 910BD camera.
	//
	//--------------------------------------------------------------------------
	//
	//This code is copyright (c) 2014 Tony Barry and TACOS. All rights reserved.
	//This code is disseminated under the Berkeley standard 3-clause licence
	//http://en.wikipedia.org/wiki/BSD_licenses
	//which means there is NO WARRANTY - ever - under any circumstances.
	//For enquiries contact tonybarry@mac.com
	//
	//--------------------------------------------------------------------------
	//
	//the 910BD SPI interface employs a variant of traditional SPI configuration 
	//as the data line is bidirectional (i.e. MISO and MOSI rolled into one line)
	//and can be driven from either Nano or cam.  It is also 3V3 logic
	//and the interface must have level shifted ability to work.
	//
	//Protocol for the BD cam transmission
	//we have eight bytes to send from the PC
	//Index  1    2    3    4    5    6    7    8
	//Name SOH  HBR  MSK  ADH  ADL  DAT  CSM  STA
	//Hex   FE   XX   XX   0X   XX   XX   XX   AA
	//
	//where SOH = 0xFE always (frame start byte)
	//            This is the only byte with the high bit set.
	//      HBR = the high bit7s from byte 3 to 8, and the READ-ONLY bit
	//            bit 6 of HB7 is the READ-ONLY bit.  
	//            When set to 1, the Arduino does a read of the register without writing.
	//            When cleared to 0, (normal ops) the register is written.
	//            Bit 3 = MSB of byte 5 (ADL), bit 2 = MSB of byte 6 (DAT) etc.
	//      MSK = the binary mask for the bits you want to actually set in the address
	//            (MSB is delivered via HB7 bit 4)
	//      ADH = The code page register address 0x00 or 0x01 
	//            (0x00 = addr 0x400 to 0x4FF; 0x01 = addr 0x500 to 0x5FF)
	//      ADL = The low byte register address 0x00 to 0xFF 
	//            (the MSB of ADL is delivered via HB7 bit 3, so ADR is sent as 0x00 to 0x7F)
	//      DAT = The data for the register 0x00 to 0xFF 
	//            (with MSB delivered via HB7 bit 2)
	//      CSM = The checksum being the low byte of ADH + ADL + DAT; 0x00 to 0xFF 
	//            (with MSB delivered via HB7 bit 1)
	//      STA = The status byte; 0xAA 
	//            (with MSB delivered via HB7 bit 0)
	//
	//The Nano responds with a two byte response.
	//
	//First the inProcess byte, sent when a valid packet has been decoded by the Nano
	//executeInProcess = 0x45
	//
	//Then one of the following:-
	//
	//executeSuccess = 0x55
	//executeCamTimeout = 0x54
	//executeFail = 0x46
	//executeCamDisconnected = 0x44
	//executeCommsFail = 0x43
	//
	//Re. MSK:-
	//Watec advise that changing unspecified bits of cam memory can cause 
	//unpredictable operation of the camera.
	//The MSK byte assists the user by reading a given address, ANDing it with the MSK, 
	//and then writing the result back to the cam. 
	//
	//Thus if the user desires to set the shutter speed to 1/500 sec, 
	//it is necessary to write 0x0C to address 0x402, while respecting bits 5-7.
	//In this case the user would write the following eight bytes:
	//
	//SOH  HBR  MSK  ADH  ADL  DAT  CSM  STA
	// FE   01   1F   00   02   0C   0E   2A
	//
	//Re. DAT:-
	//Watec present data in their document as local to the function;  
	//e.g. AGC HI MODE is described as 0x03, 
	//even though in the byte at 0x0401 this should be 0b ---- 11-- 
	//Watec also describe the byte in its entirety, in the Parameter Table, 
	//using the - symbol to describe bits that are extraneous to the function.
	//I have adopted the entire description of the DAT byte to avoid ambiguity, 
	//thus to send :- 
	//AGC HI    MSK 0b 0000 1100    DAT 0b 0000 1100
	//AGC MID   MSK 0b 0000 1100    DAT 0b 0000 1000
	// 
	//Re. Serial comms:-
	//The serial over USB port is configured at 57600 baud, 8N1. No carriage returns after a send.
	//
	//Read Cam Reg Value:-
	//If the user wishes to simply read a given cam register, then the bytes sent are the same as for a write, 
	//with the addition that the READ_ONLY bit is set in HBR.
	//The cam returns the register value (one byte), ANDed with the MSK, after the executeSuccess byte.
	//Thus, in a read of the AGC gain register (0x0401), 
	//the cam would return executeInProgress (0x45), followed by executeSuccess (0x55), 
	//followed by one byte e.g. 0b 0000 1100 signifying the value of AGC in the cam ( = AGC HI). 
	//
	//OSD control:-
	//The OSD commands are implemented in a non-standard way by Watec 
	// i.e. the CS byte is not a computed checksum but is used as a kind of data byte in its own right  
	//The implementation of these commands is therefore a bit of a kludge.  Oh well.
	//To use the OSD commands, the packet are:-
	//LEFT
	//SOH  HBR  MSK  ADH  ADL  DAT  CSM  STA
	// FE   41   00   02   00   01   03   2A
	//RIGHT
	//SOH  HBR  MSK  ADH  ADL  DAT  CSM  STA
	// FE   41   00   02   00   02   04   2A
	//UP
	//SOH  HBR  MSK  ADH  ADL  DAT  CSM  STA
	// FE   41   00   02   00   03   05   2A
	//DOWN
	//SOH  HBR  MSK  ADH  ADL  DAT  CSM  STA
	// FE   41   00   02   00   04   06   2A
	//SET
	//SOH  HBR  MSK  ADH  ADL  DAT  CSM  STA
	// FE   41   00   02   00   05   07   2A
	//
	//
	//Limitations:-
	//I have not implemented the Area Display On-Off or Camera Parameter Save.
	#endregion

	public enum MultipleCommandSeries
	{
		None,
		InitCamera
	}

	public enum OsdOperation
	{
		Up,
		Down,
		Left,
		Right,
		Set
	}

	public class WAT910BDStateMachine
	{
		private const byte EXECUTE_IN_PROCESS = 0x45;
		private const byte EXECUTE_SUCCESS = 0x55;
		private const byte EXECUTE_CAM_TIMEOUT = 0x54;
		private const byte EXECUTE_FAIL = 0x46;
		private const byte EXECUTE_CAM_DISCONNECTED = 0x44;
		private const byte EXECUTE_COMMS_FAIL = 0x43;

		private static byte SHUTTER_SPEED_256 = 0x00;
		private static byte SHUTTER_SPEED_128 = 0x01;
		private static byte SHUTTER_SPEED_64 = 0x02;
		private static byte SHUTTER_SPEED_32 = 0x03;
		private static byte SHUTTER_SPEED_16 = 0x04;
		private static byte SHUTTER_SPEED_8 = 0x05;
		private static byte SHUTTER_SPEED_4 = 0x06;
		private static byte SHUTTER_SPEED_2 = 0x07;
		private static byte SHUTTER_SPEED_EI = 0x08;
		private static byte SHUTTER_SPEED_OFF = 0x09;
		private static byte SHUTTER_SPEED_FL = 0x0A;
		private static byte SHUTTER_SPEED_250th = 0x0B;
		private static byte SHUTTER_SPEED_500th = 0x0C;
		private static byte SHUTTER_SPEED_1000th = 0x0D;
		private static byte SHUTTER_SPEED_2000th = 0x0E;
		private static byte SHUTTER_SPEED_5000th = 0x0F;
		private static byte SHUTTER_SPEED_10000th = 0x10;
		private static byte SHUTTER_SPEED_100000th = 0x11;

		private static byte GAIN_DB_MIN = 6;
		private static byte GAIN_DB_MAX = 41;

		private static byte GAMMA_OFF = 0x14; // 1.00
		private static byte GAMMA_LO = 0x09; // 0.45
		private static byte GAMMA_HI = 0x07; // 0.35
		private static byte GAMMA_MAX = 0x01; // 0.05

		private MultipleCommandSeries m_CurrentMultipleCommandSeries = MultipleCommandSeries.None;
		private bool m_WaitingReceivedAck = false;
		private bool m_WaitingResult = false;
		private bool m_CommandSuccessful = false;
		private string m_ErrorMessage = null;

		private bool m_CanSendCommand;

		public bool CanSendCommand()
		{
			return m_CanSendCommand && m_CurrentMultipleCommandSeries == MultipleCommandSeries.None;
		}

		public void StartSendingMultipleCommands(MultipleCommandSeries series)
		{
			if (m_CurrentMultipleCommandSeries != MultipleCommandSeries.None)
			{
				m_CanSendCommand = false;
				m_CurrentMultipleCommandSeries = series;
			}
		}

		public void FinishedSendingMultipleCommands(MultipleCommandSeries series)
		{
			if (m_CurrentMultipleCommandSeries == series)
			{
				m_CanSendCommand = true;
				m_CurrentMultipleCommandSeries = MultipleCommandSeries.None;
			}
		}

		public List<byte[]> GetCommandSeries(MultipleCommandSeries series)
		{
			var rv = new List<byte[]>();

			// Parameter		 	Value		Address		Bit 	Data
			// 3DNR				    OFF			0x450		0		0
			// ZOOM				    OFF			0x4A8		0		0
			// MOTION			 	OFF			0x460		0		0
			// MOTION VIEW		 	OFF			0x461		6		0
			// WDR MODE			    OFF			0x448		0-1		0b ---- --xx
			// SHARPNESS		 	OFF			0x451		0-6		0b --xx xxxx
			// AGC MODE			    OFF			0x401		2-3		0b ---- xx--
			// BLC MODE			    OFF			0x41B		0-1		0b ---- --xx
			// SENS UP			 	OFF			0x406		3		0
			// DIGIT OUT		 	OFF			0x5E2		0		0

			rv.Add(BuildWriteCommand(0x450, "0000 0001", 0)); // 3DNR = OFF
			rv.Add(BuildWriteCommand(0x4A8, "0000 0001", 0)); // ZOOM = OFF
			rv.Add(BuildWriteCommand(0x460, "0000 0001", 0)); // MOTION = OFF
			rv.Add(BuildWriteCommand(0x461, "0100 0000", 0)); // MOTION VIEW = OFF
			rv.Add(BuildWriteCommand(0x448, "0000 0011", 0)); // WDR MODE = OFF
			rv.Add(BuildWriteCommand(0x451, "0011 1111", 0)); // SHARPNESS = OFF
			rv.Add(BuildWriteCommand(0x401, "0000 1100", 0)); // AGC MODE = OFF
			rv.Add(BuildWriteCommand(0x41B, "0000 0011", 0)); // BLC MODE = OFF
			rv.Add(BuildWriteCommand(0x406, "0000 1000", 0)); // SENS UP = OFF
			rv.Add(BuildWriteCommand(0x5E2, "0000 0001", 0)); // DIGIT OUT = OFF

			return rv;
		}

		public string GetLastCameraErrorMessage()
		{
			return m_ErrorMessage;
		}

		public bool WasLastCameraOperationSuccessful()
		{
			return m_CommandSuccessful;
		}

		public bool SendWriteCommandAndWaitToExecute(SerialPort port, byte[] command)
		{
			m_CanSendCommand = false;
			try
			{
				m_WaitingResult = false;
				m_WaitingReceivedAck = true;
				m_CommandSuccessful = false;
				port.Write(command, 0, command.Length);

				int ticks = 0;
				while (ticks < 100 && (m_WaitingReceivedAck || m_WaitingResult))
				{
					Thread.Sleep(10);
					ticks++;
				}

				if (!m_WaitingReceivedAck && !m_WaitingResult && m_CommandSuccessful)
				{
					// We received the command response
					return true;
				}

				if (ticks >= 100 && m_ErrorMessage == null)
				{
					// Timeout occured
					SetCommandError("Camera did not respond within 1 sec.");
				}

				return false;
			}
			catch (Exception ex)
			{
				SetCommandError(ex.GetType().ToString() + " : " + ex.Message);
				return false;
			}
			finally
			{
				m_CanSendCommand = true;
			}
		}

		private void SetCommandError(string errorMessage)
		{
			m_ErrorMessage = errorMessage;
			m_CommandSuccessful = false;
			m_WaitingReceivedAck = false;
			m_WaitingResult = false;
		}

		public void PushReceivedByte(byte bt)
		{
			if (m_WaitingReceivedAck)
			{
				if (bt == EXECUTE_IN_PROCESS)
				{
					m_WaitingResult = true;
					m_WaitingReceivedAck = false;
					return;
				}

				m_CommandSuccessful = false;
				m_WaitingResult = false;
				switch (bt)
				{
					case EXECUTE_CAM_DISCONNECTED:
						m_ErrorMessage = "Camera is disconnected.";
						return;

					case EXECUTE_CAM_TIMEOUT:
					case EXECUTE_COMMS_FAIL:
					case EXECUTE_FAIL:
						m_ErrorMessage = "Error communicating with the Camera.";
						return;
				}
			}
			else if (m_WaitingResult)
			{
				if (bt == EXECUTE_SUCCESS)
				{
					m_ErrorMessage = null;
					m_CommandSuccessful = true;
					m_WaitingResult = false;
					return;
				}

				m_CommandSuccessful = false;
				m_WaitingResult = false;

				switch (bt)
				{
					case EXECUTE_CAM_DISCONNECTED:
						m_ErrorMessage = "Camera is disconnected.";
						return;

					case EXECUTE_CAM_TIMEOUT:
					case EXECUTE_COMMS_FAIL:
					case EXECUTE_FAIL:
						m_ErrorMessage = "Error communicating with the Camera.";
						return;
				}				
			}

			m_ErrorMessage = string.Format("The received byte 0x{0} was unexpected in the current camera state.", Convert.ToString(bt, 16));
		}

		private byte[] BuildWriteCommand(int address, string mask, byte value)
		{
			return BuildReadWriteCommand(address, mask, value, false);
		}


		private byte[] BuildReadCommand(int address, string mask, byte value)
		{
			return BuildReadWriteCommand(address, mask, value, true);
		}

		private byte[] BuildReadWriteCommand(int address, string mask, byte value, bool isRead)
		{
			byte btHBR = 0;

			//Index  1    2    3    4    5    6    7    8
			//Name SOH  HBR  MSK  ADH  ADL  DAT  CSM  STA
			//Hex   FE   XX   XX   0X   XX   XX   XX   AA
			//
			//where SOH = 0xFE always (frame start byte)
			//            This is the only byte with the high bit set.
			//      HBR = the high bit7s from byte 3 to 8, and the READ-ONLY bit
			//            bit 6 of HB7 is the READ-ONLY bit.  
			//            When set to 1, the Arduino does a read of the register without writing.
			//            When cleared to 0, (normal ops) the register is written.
			//            Bit 3 = MSB of byte 5 (ADL), bit 2 = MSB of byte 6 (DAT) etc.
			//      MSK = the binary mask for the bits you want to actually set in the address
			//            (MSB is delivered via HB7 bit 4)
			//      ADH = The code page register address 0x00 or 0x01 
			//            (0x00 = addr 0x400 to 0x4FF; 0x01 = addr 0x500 to 0x5FF)
			//      ADL = The low byte register address 0x00 to 0xFF 
			//            (the MSB of ADL is delivered via HB7 bit 3, so ADR is sent as 0x00 to 0x7F)
			//      DAT = The data for the register 0x00 to 0xFF 
			//            (with MSB delivered via HB7 bit 2)
			//      CSM = The checksum being the low byte of ADH + ADL + DAT; 0x00 to 0xFF 
			//            (with MSB delivered via HB7 bit 1)
			//      STA = The status byte; 0xAA 
			//            (with MSB delivered via HB7 bit 0)

			byte btMask = GetMaskAsByte(mask);
			HB7Operation(ref btMask, ref btHBR, 4);
			
			byte btADH = address < 0x500 ? (byte)0 : (byte)1;
			
			byte btADL = (byte) (address & 0xFF);
			HB7Operation(ref btADL, ref btHBR, 3);

			HB7Operation(ref value, ref btHBR, 2);
			
			int checkSum = (int) btADH + (int) btADL + (int) value;
			byte btCheckSum = (byte)(checkSum & 0xFF);
			HB7Operation(ref value, ref btHBR, 1);

			byte btSTA = 0xAA;
			HB7Operation(ref btSTA, ref btHBR, 0);

			if (isRead)
				btHBR = (byte)(btHBR | 0x40);
			else
				btHBR = (byte)(btHBR & 0xBF);

			return new byte[] { 0xFE, btHBR, btMask, btADH, value, btCheckSum, btSTA };
		}

		public byte[] BuildOsdCommand(OsdOperation operation)
		{
			//OSD control:-
			//The OSD commands are implemented in a non-standard way by Watec 
			// i.e. the CS byte is not a computed checksum but is used as a kind of data byte in its own right  
			//The implementation of these commands is therefore a bit of a kludge.  Oh well.
			//To use the OSD commands, the packet are:-
			//LEFT
			//SOH  HBR  MSK  ADH  ADL  DAT  CSM  STA
			// FE   41   00   02   00   01   03   2A
			//RIGHT
			//SOH  HBR  MSK  ADH  ADL  DAT  CSM  STA
			// FE   41   00   02   00   02   04   2A
			//UP
			//SOH  HBR  MSK  ADH  ADL  DAT  CSM  STA
			// FE   41   00   02   00   03   05   2A
			//DOWN
			//SOH  HBR  MSK  ADH  ADL  DAT  CSM  STA
			// FE   41   00   02   00   04   06   2A
			//SET
			//SOH  HBR  MSK  ADH  ADL  DAT  CSM  STA
			// FE   41   00   02   00   05   07   2A

			switch (operation)
			{
				case OsdOperation.Left:
					return new byte[] { 0xFE, 0x41, 0x00, 0x02, 0x00, 0x01, 0x03, 0x2A };
					break;

				case OsdOperation.Right:
					return new byte[] { 0xFE, 0x41, 0x00, 0x02, 0x00, 0x02, 0x04, 0x2A };
					break;

				case OsdOperation.Up:
					return new byte[] { 0xFE, 0x41, 0x00, 0x02, 0x00, 0x03, 0x05, 0x2A };
					break;

				case OsdOperation.Down:
					return new byte[] { 0xFE, 0x41, 0x00, 0x02, 0x00, 0x04, 0x06, 0x2A };
					break;

				case OsdOperation. Set:
					return new byte[] { 0xFE, 0x41, 0x00, 0x02, 0x00, 0x05, 0x07, 0x2A };
					break;

				default:
					throw new ArgumentOutOfRangeException("Invalid OsdOperation.");
			}
		}

		private static Regex BITMASKREGEX = new Regex("^[01]{4}\\s[01]{4}$");

		private byte GetMaskAsByte(string binaryMark)
		{
			if (BITMASKREGEX.IsMatch(binaryMark))
			{
				// 0123456789
				// 0000 0000
				int bit0 = binaryMark[8] == '1' ? 1 : 0;
				int bit1 = binaryMark[7] == '1' ? 1 : 0;
				int bit2 = binaryMark[6] == '1' ? 1 : 0;
				int bit3 = binaryMark[5] == '1' ? 1 : 0;
				int bit4 = binaryMark[3] == '1' ? 1 : 0;
				int bit5 = binaryMark[2] == '1' ? 1 : 0;
				int bit6 = binaryMark[1] == '1' ? 1 : 0;
				int bit7 = binaryMark[0] == '1' ? 1 : 0;

				int maskInt = bit0 + bit1 * 2 + bit2 * 4 + bit3 * 8 + bit4 * 16 + bit5 * 32 + bit6 * 64 + bit7 * 128;
				if (maskInt >= 0 && maskInt <= 255) return (byte) maskInt;
			}
			
			throw new FormatException("Not a valid binary mask.");
		}

		private void HB7Operation(ref byte data, ref byte hbr, int hbrMsbPosition)
		{
			byte msbLessData = (byte)(data & 0x7F);
			bool msbSet = msbLessData != data;
			data = msbLessData;

			switch (hbrMsbPosition)
			{
				case 0:
					hbr = (byte)(msbSet ? hbr | 0x1 : hbr & 0xFE);
					break;

				case 1:
					hbr = (byte)(msbSet ? hbr | 0x2 : hbr & 0xFD);
					break;

				case 2:
					hbr = (byte)(msbSet ? hbr | 0x4 : hbr & 0xFB);
					break;

				case 3:
					hbr = (byte)(msbSet ? hbr | 0x8 : hbr & 0xF7);
					break;

				case 4:
					hbr = (byte)(msbSet ? hbr | 0x10 : hbr & 0xEF);
					break;

				case 5:
					hbr = (byte)(msbSet ? hbr | 0x20 : hbr & 0xDF);
					break;

				case 6:
					hbr = (byte)(msbSet ? hbr | 0x40 : hbr & 0xBF);
					break;

				case 7:
					hbr = (byte)(msbSet ? hbr | 0x80 : hbr & 0x7F);
					break;

				default:
					throw new ArgumentOutOfRangeException("Invalid bit location.");
			}
		}
	}
}
