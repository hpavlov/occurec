using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OccuRec.ASCOM.Interfaces.Devices;

namespace OccuRec.CameraDrivers.WAT910BD
{
	internal class WAT910BDCameraController : IOccuRecCameraController
	{
		internal static string CAMERA_NAME = "WAT-910BD";
		internal static string DRIVER_NAME = "WAT-910BD Nano Driver";

		internal const string PROP_COM_PORT = "COM-PORT";

		public IVideoDriverSettings Configuration { get; set; }

		private WAT910BDDriver m_Driver { get; set; }

		public bool Connected
		{
			get
			{
				return m_Driver != null && m_Driver.IsConnected;
			}
			set
			{
				if (m_Driver != null)
				{
					m_Driver = new WAT910BDDriver();
					m_Driver.OnCommandExecutionCompleted += m_Driver_OnCommandExecutionCompleted;
					m_Driver.OnSerialComms += m_Driver_OnSerialComms;
				}

				if (value && !m_Driver.IsConnected)
				{
					if (IsConfigured)
						m_Driver.Connect(Configuration.GetProperty(PROP_COM_PORT));
					else
						throw new InvalidOperationException("The driver hasn't been configured.");
				}
				else if (!value && m_Driver.IsConnected)
				{
					m_Driver.Disconnect();
				}
			}
		}

		private string FormatBytesHex(byte[] data)
		{
			var output = new StringBuilder();
			for (int i = 0; i < data.Length; i++)
			{
				output.AppendFormat("{0} ", data[i].ToString("x2").ToUpper());
			}
			return output.ToString();
		}

		void m_Driver_OnSerialComms(SerialCommsEventArgs e)
		{
			if (e.Sent)
				Trace.WriteLine(string.Format("WAT-910BD SENT: {0}", FormatBytesHex(e.Data)));
			else
				Trace.WriteLine(string.Format("WAT-910BD RCVD: {0} {1}", FormatBytesHex(e.Data), e.Message));
		}

		void m_Driver_OnCommandExecutionCompleted(WAT910DBEventArgs e)
		{
			
		}

		public string Description
		{
			get { return DRIVER_NAME; }
		}

		public string DriverName
		{
			get { return DRIVER_NAME; }
		}

		public VideoState GetCurrentState()
		{
			var rv = new VideoState();

			if (m_Driver != null && m_Driver.IsConnected)
			{
				rv.MinGainIndex = m_Driver.MinGainIndex;
				rv.MaxGainIndex = m_Driver.MaxGainIndex;
				rv.MinGammaIndex = m_Driver.MinGammaIndex;
				rv.MaxGammaIndex = m_Driver.MaxGammaIndex;
				rv.MinExposureIndex = m_Driver.MinExposureIndex;
				rv.MaxExposureIndex = m_Driver.MaxExposureIndex;

				WAT910BDCameraState camState = m_Driver.ReadCurrentCameraState();

				rv.GainIndex = camState.GainIndex;
				rv.GammaIndex = camState.GammaIndex;
				rv.ExposureIndex = camState.ExposureIndex;
			}

			return rv;
		}

		public bool RequiresConfiguration 
		{
			get { return true; }
		}

		public bool IsConfigured
		{
			get
			{
				return
					Configuration != null &&
					!string.IsNullOrEmpty(Configuration.GetProperty(PROP_COM_PORT));
			}
		}

		public bool ConfigureConnectionSettings(IWin32Window parent)
		{
			var frm = new frmWAT910BDConnectionSettings();
			frm.DefaultComPort = Configuration.GetProperty(PROP_COM_PORT);
			frm.StartPosition = FormStartPosition.CenterParent;
			if (frm.ShowDialog(parent) == DialogResult.OK)
			{
				Configuration.SetProperty(PROP_COM_PORT, frm.DefaultComPort);
				return true;
			}

			return false;
		}

		public override string ToString()
		{
			return DRIVER_NAME;
		}

		public void Dispose()
		{
			// TODO
		}
	}
}
