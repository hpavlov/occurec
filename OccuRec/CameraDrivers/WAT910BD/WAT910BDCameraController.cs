/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OccuRec.ASCOM.Interfaces.Devices;
using OccuRec.Utilities;

namespace OccuRec.CameraDrivers.WAT910BD
{
    public class DriverErrorEventArgs : EventArgs
    {
        public string ErrorMessage;
        public string CommandId;
    }

	internal class WAT910BDCameraController : IOccuRecCameraController
	{
		internal static string CAMERA_NAME = "WAT-910BD";
		internal static string DRIVER_NAME = "WAT-910BD Nano Driver";

		internal const string PROP_COM_PORT = "COM-PORT";

        public event DriverErrorCallback OnError;

		public IVideoDriverSettings Configuration { get; set; }

		private WAT910BDDriver m_Driver { get; set; }

	    private WAT910BDCameraState m_CurrentState = null;

	    public bool Connected
		{
			get
			{
				return m_Driver != null && m_Driver.IsConnected;
			}
			set
			{
				if (m_Driver == null)
				{
					m_Driver = new WAT910BDDriver();
					m_Driver.OnCommandExecutionCompleted += m_Driver_OnCommandExecutionCompleted;
					m_Driver.OnSerialComms += m_Driver_OnSerialComms;
				}

				if (value && !m_Driver.IsConnected)
				{
				    if (IsConfigured)
				    {
				        m_Driver.Connect(Configuration.GetProperty(PROP_COM_PORT));
                        if (m_Driver.IsConnected)
                            m_Driver.InitialiseCamera();
				    }
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
                Trace.WriteLine(string.Format("WAT-910BD SENT: {0} ({1})", FormatBytesHex(e.Data), e.Message));
			else
				Trace.WriteLine(string.Format("WAT-910BD RCVD: {0} {1}", FormatBytesHex(e.Data), e.Message));
		}

		void m_Driver_OnCommandExecutionCompleted(WAT910DBEventArgs e)
		{
			if (!e.IsSuccessful && !string.IsNullOrEmpty(e.ErrorMessage))
			{
                EventHelper.RaiseEvent(OnError, new DriverErrorEventArgs
                {
                    ErrorMessage = e.ErrorMessage,
                    CommandId = e.CommandId
                });
			}
		}

		public string Description
		{
			get { return DRIVER_NAME; }
		}

		public string DriverName
		{
			get { return DRIVER_NAME; }
		}

        public VideoState GetCurrentState(CameraStateQuery query)
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

                if (m_CurrentState == null)
                {
                    m_CurrentState = new WAT910BDCameraState();
                    query = CameraStateQuery.All;
                }

                WAT910BDCameraState camState = m_Driver.ReadCurrentCameraState(query);
			    if ((query & CameraStateQuery.Gamma) == CameraStateQuery.Gamma)
			    {
                    m_CurrentState.GammaIndex = camState.GammaIndex;
                    m_CurrentState.Gamma = camState.Gamma;
                    m_CurrentState.GammaSuccess = camState.GammaSuccess;
			    }
                if ((query & CameraStateQuery.Gain) == CameraStateQuery.Gain)
                {
                    m_CurrentState.GainIndex = camState.GainIndex;
                    m_CurrentState.Gain = camState.Gain;
                    m_CurrentState.GainSuccess = camState.GainSuccess;
                }
                if ((query & CameraStateQuery.Shutter) == CameraStateQuery.Shutter)
                {
                    m_CurrentState.ExposureIndex = camState.ExposureIndex;
                    m_CurrentState.Exposure = camState.Exposure;
                    m_CurrentState.ExposureSuccess = camState.ExposureSuccess;
                }

			    rv.GainIndex = m_CurrentState.GainIndex;
                rv.GammaIndex = m_CurrentState.GammaIndex;
                rv.ExposureIndex = m_CurrentState.ExposureIndex;

                rv.Gain = m_CurrentState.Gain;
                rv.Gamma = m_CurrentState.Gamma;
                rv.Exposure = m_CurrentState.Exposure;
			}

			return rv;
		}

		public bool RequiresConfiguration 
		{
			get { return true; }
		}

		public bool Supports5ButtonOSD
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

		public void OSDUp()
		{
			if (m_Driver != null && m_Driver.IsConnected)
			{
				m_Driver.OSDCommandUp();
			}
		}

		public void OSDDown()
		{
			if (m_Driver != null && m_Driver.IsConnected)
			{
				m_Driver.OSDCommandDown();
			}
		}

		public void OSDLeft()
		{
			if (m_Driver != null && m_Driver.IsConnected)
			{
				m_Driver.OSDCommandLeft();
			}
		}

		public void OSDRight()
		{
			if (m_Driver != null && m_Driver.IsConnected)
			{
				m_Driver.OSDCommandRight();
			}
		}

		public void OSDSet()
		{
			if (m_Driver != null && m_Driver.IsConnected)
			{
				m_Driver.OSDCommandSet();
			}
		}

		public void GammaUp()
		{
			if (m_Driver != null && m_Driver.IsConnected)
			{
				m_Driver.GammaUp();
			}
		}

		public void GammaDown()
		{
			if (m_Driver != null && m_Driver.IsConnected)
			{
				m_Driver.GammaDown();
			}
		}

		public void GainUp()
		{
			if (m_Driver != null && m_Driver.IsConnected)
			{
				m_Driver.GainUp();
			}
		}

		public void GainDown()
		{
			if (m_Driver != null && m_Driver.IsConnected)
			{
				m_Driver.GainDown();
			}
		}

		public void ExposureUp()
		{
			if (m_Driver != null && m_Driver.IsConnected)
			{
				m_Driver.ExposureUp();
			}
		}

		public void ExposureDown()
		{
			if (m_Driver != null && m_Driver.IsConnected)
			{
				m_Driver.ExposureDown();
			}
		}


		public override string ToString()
		{
			return DRIVER_NAME;
		}

		public void Dispose()
		{
			if (m_Driver != null)
			{
				m_Driver.Dispose();
				m_Driver = null;
			}
		}
	}
}
