using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WAT910BD.Tester.WAT910BDComms
{
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
		private MultipleCommandSeries m_CurrentMultipleCommandSeries = MultipleCommandSeries.None;

		public bool CanSendCommand { get; private set; }

		public void StartSendingMultipleCommands(MultipleCommandSeries series)
		{
			if (m_CurrentMultipleCommandSeries != MultipleCommandSeries.None)
			{
				CanSendCommand = false;
				m_CurrentMultipleCommandSeries = series;
			}
		}

		public void FinishedSendingMultipleCommands(MultipleCommandSeries series)
		{
			if (m_CurrentMultipleCommandSeries == series)
			{
				CanSendCommand = true;
				m_CurrentMultipleCommandSeries = MultipleCommandSeries.None;
			}
		}

		public List<byte[]> GetCommandSeries(MultipleCommandSeries series)
		{
			var rv = new List<byte[]>();

			// Parameter		 	Value		Address		Bit 	Data
			// 3DNR				    OFF			0x450		1		0
			// ZOOM				    OFF			0x4A8		1		0
			// MOTION			 	OFF			0x460		1		0
			// MOTION VIEW		 	OFF			0x461		6		0
			// WDR MODE			    OFF			0x448		0-1		0b ---- --00
			// SHARPNESS		 	OFF			0x451		0-6		0b --00 0000
			// AGL MODE			    OFF			0x401		2-3		0b ---- 00--
			// BLC MODE			    OFF			0x41B		0-1		0b ---- --00
			// SENS UP			 	OFF			0x406		3		0
			// DIGIT OUT		 	OFF			0x5E2		1		0

			rv.Add(BuildWriteCommand(0x450, 1, 0));
			rv.Add(BuildWriteCommand(0x4A8, 1, 0));
			rv.Add(BuildWriteCommand(0x460, 1, 0));


			return rv;
		}

		public void PushReceivedByte(byte bt)
		{
			
		}

		private byte[] BuildWriteCommand(int address, byte mask, byte value)
		{
			throw new NotImplementedException();
		}

		private byte[] BuildReadCommand(int address, byte mask, byte value)
		{
			throw new NotImplementedException();
		}

		private byte[] BuildOsdCommand(OsdOperation operation)
		{
			throw new NotImplementedException();
		}
	}
}
