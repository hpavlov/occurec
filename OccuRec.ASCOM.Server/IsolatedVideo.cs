/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using ASCOM;
using ASCOM.DeviceInterface;
using OccuRec.ASCOM.Interfaces.Devices;
using OccuRec.Utilities;

namespace OccuRec.ASCOM.Server
{
	[Serializable]
	public class IsolatedVideo : IsolatedDevice, IASCOMVideo
	{
		private global::ASCOM.DriverAccess.Video m_Video;

		internal IsolatedVideo(string progId)
		{
            m_Video = new global::ASCOM.DriverAccess.Video(progId);

			SetIsolatedDevice(m_Video, progId);
		}

		public VideoState GetCurrentState()
		{
			var state = new VideoState();

			// TODO: Populate the state from the m_Video object

			return state;
		}

		[DebuggerStepThrough]
        public void Configure()
        {
			CallAndTranslateExceptions(() => m_Video.SetupDialog());
		}

        public int Width
        {
			[DebuggerStepThrough]
            get { return GetValueAndTranslateExceptions(() => m_Video.Width); }
        }

        public int Height
        {
			[DebuggerStepThrough]
            get { return GetValueAndTranslateExceptions(() => m_Video.Height); }
        }

        public int BitDepth
        {
			[DebuggerStepThrough]
            get { return GetValueAndTranslateExceptions(() => m_Video.BitDepth); }
        }

	    public bool CanConfigureImage
	    {
			[DebuggerStepThrough]
            get { return GetValueAndTranslateExceptions(() => m_Video.CanConfigureDeviceProperties); }
	    }

	    public ArrayList SupportedActions
	    {
			[DebuggerStepThrough]
            get { return GetValueAndTranslateExceptions(() => m_Video.SupportedActions); }
	    }

	    public int CameraState
	    {
			[DebuggerStepThrough]
            get { return GetValueAndTranslateExceptions(() => (int)m_Video.CameraState); }
	    }

        public string VideoFileFormat
        {
			[DebuggerStepThrough]
            get { return GetValueAndTranslateExceptions(() => m_Video.VideoFileFormat); }
        }

        public string VideoCodec
        {
			[DebuggerStepThrough]
            get { return GetValueAndTranslateExceptions(() => m_Video.VideoCodec); }
        }

        public string Name
		{
			[DebuggerStepThrough]
            get { return GetValueAndTranslateExceptions(() => m_Video.Name); }
        }

        public string VideoCaptureDeviceName
        {
			[DebuggerStepThrough]
            get { return GetValueAndTranslateExceptions(() => m_Video.VideoCaptureDeviceName); }
        }

        public IASCOMVideoFrame LastVideoFrame
        {
			[DebuggerStepThrough]
            get { return GetValueAndTranslateExceptions(() => new IsolatedVideoFrame(m_Video.LastVideoFrame)); }
        }

		public short InterfaceVersion
		{
			[DebuggerStepThrough]
			get { return GetValueAndTranslateExceptions(() => m_Video.InterfaceVersion); }
		}


		public double ExposureMax
		{
			[DebuggerStepThrough]
			get { return GetValueAndTranslateExceptions(() => m_Video.ExposureMax); }
		}

		public double ExposureMin
		{
			[DebuggerStepThrough]
			get { return GetValueAndTranslateExceptions(() => m_Video.ExposureMin); }
		}

		public int FrameRate
		{
			[DebuggerStepThrough]
			get { return GetValueAndTranslateExceptions(() => (int)m_Video.FrameRate); }
		}

		public ArrayList SupportedIntegrationRates
		{
			[DebuggerStepThrough]
			get
			{
				try
				{
					return m_Video.SupportedIntegrationRates;
				}
				catch (PropertyNotImplementedException pex)
				{
					throw new OccuRec.Utilities.Exceptions.PropertyNotImplementedException(pex.Message);
				}
				catch (MethodNotImplementedException mex)
				{
					throw new OccuRec.Utilities.Exceptions.MethodNotImplementedException(mex.Message);
				}
				catch (NotConnectedException cex)
				{
					throw new OccuRec.Utilities.Exceptions.NotConnectedException();
				}
				catch (DriverException dex)
				{
					throw new OccuRec.Utilities.Exceptions.DriverException(dex.Message, dex);
				}
			}
		}

		public int IntegrationRate
		{
			[DebuggerStepThrough]
			get { return GetValueAndTranslateExceptions(() => m_Video.IntegrationRate); }

			[DebuggerStepThrough]
			set { CallAndTranslateExceptions(() => m_Video.IntegrationRate = value); }
		}

		[DebuggerStepThrough]
		public string Action(string actionName, string actionParameters)
		{
			return GetValueAndTranslateExceptions(() => m_Video.Action(actionName, actionParameters));
		}

		[DebuggerStepThrough]
		public string StartRecordingVideoFile(string preferredFileName)
		{
			return GetValueAndTranslateExceptions(() => m_Video.StartRecordingVideoFile(preferredFileName));
		}

		[DebuggerStepThrough]
		public void StopRecordingVideoFile()
		{
			CallAndTranslateExceptions(() => m_Video.StopRecordingVideoFile());
		}

		[DebuggerStepThrough]
		public void ConfigureImage()
		{
			CallAndTranslateExceptions(() => m_Video.ConfigureDeviceProperties());
		}

		public string SensorName
		{
			[DebuggerStepThrough]
			get { return GetValueAndTranslateExceptions(() => m_Video.SensorName); }
		}

		public int SensorType
		{
			[DebuggerStepThrough]
			get { return GetValueAndTranslateExceptions(() => (int)m_Video.SensorType); }
		}

		public double PixelSizeX
		{
			[DebuggerStepThrough]
			get { return GetValueAndTranslateExceptions(() => m_Video.PixelSizeX); }
		}

		public double PixelSizeY
		{
			[DebuggerStepThrough]
			get { return GetValueAndTranslateExceptions(() => m_Video.PixelSizeY); }
		}

		public int VideoFramesBufferSize
		{
			[DebuggerStepThrough]
			get { return GetValueAndTranslateExceptions(() => m_Video.VideoFramesBufferSize); }
		}

		public short GainMax
		{
			[DebuggerStepThrough] 
			get
			{
				try
				{
					return m_Video.GainMax;
				}
				catch (PropertyNotImplementedException pex)
				{
					throw new OccuRec.Utilities.Exceptions.PropertyNotImplementedException(pex.Message);
				}
				catch (MethodNotImplementedException mex)
				{
					throw new OccuRec.Utilities.Exceptions.MethodNotImplementedException(mex.Message);
				}
				catch (NotConnectedException cex)
				{
					throw new OccuRec.Utilities.Exceptions.NotConnectedException();
				}
				catch (DriverException dex)
				{
					throw new OccuRec.Utilities.Exceptions.DriverException(dex.Message, dex);
				}
			}
		}

		public short GainMin
		{
			[DebuggerStepThrough] 
			get
			{
				try
				{
					return m_Video.GainMin;
				}
				catch (PropertyNotImplementedException pex)
				{
					throw new OccuRec.Utilities.Exceptions.PropertyNotImplementedException(pex.Message);
				}
				catch (MethodNotImplementedException mex)
				{
					throw new OccuRec.Utilities.Exceptions.MethodNotImplementedException(mex.Message);
				}
				catch (NotConnectedException cex)
				{
					throw new OccuRec.Utilities.Exceptions.NotConnectedException();
				}
				catch (DriverException dex)
				{
					throw new OccuRec.Utilities.Exceptions.DriverException(dex.Message, dex);
				}
			}
		}

		public short Gain
		{
			[DebuggerStepThrough] 
			get
			{
				try
				{
					return m_Video.Gain;
				}
				catch (PropertyNotImplementedException pex)
				{
					throw new OccuRec.Utilities.Exceptions.PropertyNotImplementedException(pex.Message);
				}
				catch (MethodNotImplementedException mex)
				{
					throw new OccuRec.Utilities.Exceptions.MethodNotImplementedException(mex.Message);
				}
				catch (NotConnectedException cex)
				{
					throw new OccuRec.Utilities.Exceptions.NotConnectedException();
				}
				catch (DriverException dex)
				{
					throw new OccuRec.Utilities.Exceptions.DriverException(dex.Message, dex);
				}
			}

			[DebuggerStepThrough]
			set { CallAndTranslateExceptions(() => m_Video.Gain = value); }
		}

		public ArrayList Gains
		{
			[DebuggerStepThrough] 
			get
			{
				try
				{
					return m_Video.Gains;
				}
				catch (PropertyNotImplementedException pex)
				{
					throw new OccuRec.Utilities.Exceptions.PropertyNotImplementedException(pex.Message);
				}
				catch (MethodNotImplementedException mex)
				{
					throw new OccuRec.Utilities.Exceptions.MethodNotImplementedException(mex.Message);
				}
				catch (NotConnectedException cex)
				{
					throw new OccuRec.Utilities.Exceptions.NotConnectedException();
				}
				catch (DriverException dex)
				{
					throw new OccuRec.Utilities.Exceptions.DriverException(dex.Message, dex);
				}
			}
		}

		public short Gamma
		{
			[DebuggerStepThrough]
			get
			{
				try
				{
					return m_Video.Gamma;
				}
				catch (PropertyNotImplementedException pex)
				{
					throw new OccuRec.Utilities.Exceptions.PropertyNotImplementedException(pex.Message);
				}
				catch (MethodNotImplementedException mex)
				{
					throw new OccuRec.Utilities.Exceptions.MethodNotImplementedException(mex.Message);
				}
				catch (NotConnectedException cex)
				{
					throw new OccuRec.Utilities.Exceptions.NotConnectedException();
				}
				catch (DriverException dex)
				{
					throw new OccuRec.Utilities.Exceptions.DriverException(dex.Message, dex);
				}
			}

			[DebuggerStepThrough]
			set { CallAndTranslateExceptions(() => m_Video.Gamma = value); }
		}

		
		public ArrayList Gammas
		{
			[DebuggerStepThrough]
			get
			{
				try
				{
					return m_Video.Gammas;
				}
				catch (PropertyNotImplementedException pex)
				{
					throw new OccuRec.Utilities.Exceptions.PropertyNotImplementedException(pex.Message);
				}
				catch (MethodNotImplementedException mex)
				{
					throw new OccuRec.Utilities.Exceptions.MethodNotImplementedException(mex.Message);
				}
				catch (NotConnectedException cex)
				{
					throw new OccuRec.Utilities.Exceptions.NotConnectedException();
				}
				catch (DriverException dex)
				{
					throw new OccuRec.Utilities.Exceptions.DriverException(dex.Message, dex);
				}
			}
		}

		[DebuggerStepThrough]
		private T GetValueAndTranslateExceptions<T>(Func<T> getter)
		{
			try
			{
				return getter();
			}
			catch (PropertyNotImplementedException pex)
			{
				throw new OccuRec.Utilities.Exceptions.PropertyNotImplementedException(pex.Message);
			}
			catch (MethodNotImplementedException mex)
			{
				throw new OccuRec.Utilities.Exceptions.MethodNotImplementedException(mex.Message);
			}
			catch (NotConnectedException cex)
			{
				throw new OccuRec.Utilities.Exceptions.NotConnectedException();
			}
			catch (DriverException dex)
			{
				throw new OccuRec.Utilities.Exceptions.DriverException(dex.Message, dex);
			}
		}

		[DebuggerStepThrough]
		private void CallAndTranslateExceptions(Action action)
		{
			try
			{
				action();
			}
			catch (PropertyNotImplementedException pex)
			{
				throw new OccuRec.Utilities.Exceptions.PropertyNotImplementedException(pex.Message);
			}
			catch (MethodNotImplementedException mex)
			{
				throw new OccuRec.Utilities.Exceptions.MethodNotImplementedException(mex.Message);
			}
			catch (NotConnectedException cex)
			{
				throw new OccuRec.Utilities.Exceptions.NotConnectedException();
			}
			catch (DriverException dex)
			{
				throw new OccuRec.Utilities.Exceptions.DriverException(dex.Message, dex);
			}
		}
    }
}
