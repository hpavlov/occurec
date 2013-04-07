using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using AAVRec.Drivers;

namespace AAVRec.Helpers
{
	internal class VideoWrapper
	{
		private IVideo video;
		private short[] gainSlots;

		public VideoWrapper(IVideo video)
		{
			this.video = video;
		}

		public void SetFreeRangeGainIntervals(short numIntervals)
		{
			if (SupporstFreeStyleGain)
			{
				gainSlots = new short[numIntervals];
				gainSlots[0] = video.GainMin;
				gainSlots[numIntervals - 1] = video.GainMax;

				double step = (video.GainMax - video.GainMin) * 1.0 / numIntervals;

				for (int i = 1; i < numIntervals - 1; i++)
				{
					gainSlots[i] = (short)Math.Round(video.GainMin + i * step);
				}
			}
		}

		public void Disconnect()
		{
			if (video != null)
			{
				video.Connected = false;
				video = null;

				cameraXSize = null;
				cameraYSize = null;
				supportsGamma = null;
				supporstFreeStyleGain = null;
				supporstDiscreteGain = null;
				supportsIntegrationRates = null;
				pixelSizeX = null;
				pixelSizeY = null;
				sensorName = null;
			}
		}

		public bool IsConnected
		{
			get
			{
				return
					ShieldedCall(
						() =>
							video != null &&
							video.Connected,
						false);
			}
		}

		public bool Connected
		{
			get { return video.Connected; }
			set { video.Connected = true; }
		}

		public int? cameraXSize;
		public int CameraXSize
		{
			get
			{
				if (cameraXSize == null)
				{
					cameraXSize = 
						ShieldedCall(
							() => video.CameraXSize,
							0);
					
				}

				return cameraXSize.Value;
			}
		}

		public int? cameraYSize;
		public int CameraYSize
		{
			get
			{
				if (cameraYSize == null)
				{
					return
						ShieldedCall(
							() => video.CameraYSize,
							0);
				}

				return cameraYSize.Value;
			}
		}

		public bool CanConfigureImage
		{
			get
			{
				return
					ShieldedCall(
						() =>
							video != null &&
							video.CanConfigureImage,
						false);
			}
		}

		public void ConfigureImage()
		{
			if (video != null)
			{
				ShieldedCall(
					delegate()
					{
						video.ConfigureImage();
						return true;
					},
					false);
			}
		}

		public VideoCameraState State
		{
			get
			{
				if (video != null && video.Connected)
				{
					return video.CameraState;
				}

				return VideoCameraState.videoCameraError;
			}
		}

	    public SensorType SensorType
	    {
	        get
	        {
                if (video != null && video.Connected)
                {
                    return video.SensorType;
                }

	            throw new System.InvalidOperationException();
	        }
	    }

		public bool? supportsGamma = null;
		public bool SupportsGamma
		{
			get
			{
				if (video != null && video.Connected)
				{
					if (supportsGamma == null)
					{
						try
						{
							supportsGamma = video.Gammas != null && video.Gammas.Count > 0;
						}
						catch (PropertyNotImplementedException)
						{
							supportsGamma = false;
						}
					}

					return supportsGamma.Value;
				}
				else
					return false;
			}
		}

		public bool CanDecreaseGamma
		{
			get
			{
				if (SupportsGamma)
					return video != null && video.Connected && video.Gammas != null && video.Gammas.Count > 0 && video.Gamma > 0;

				return false;
			}	
		}

		public bool CanIncreaseGamma
		{
			get
			{
				if (SupportsGamma)
					return video != null && video.Connected && video.Gammas != null && video.Gammas.Count > 0 && video.Gamma < video.Gammas.Count - 1;

				return false;
			}
		}

		public string Gamma
		{
			get
			{
				if (SupportsGamma)
				{
					if (video != null && video.Connected && video.Gammas != null && video.Gammas.Count > 0)
					{
						if (video.Gamma >= 0 && video.Gamma < video.Gammas.Count)
							return (string)video.Gammas[video.Gamma];
						else
							return string.Empty;
					}
				}

				return "N/A";
			}

		}

		public void IncreaseGamma()
		{
			if (CanIncreaseGamma)
				video.Gamma++;
		}

		public void DecreaseGamma()
		{
			if (CanDecreaseGamma)
				video.Gamma--;
		}


		private bool? supporstFreeStyleGain = null;
		public bool SupporstFreeStyleGain
		{
			get
			{
				if (!supporstFreeStyleGain.HasValue)
				{
					if (video != null && video.Connected)
					{
						try
						{
							short minGain = video.GainMin;
							short maxGain = video.GainMax;

							supporstFreeStyleGain =  minGain < maxGain;
						}
						catch (PropertyNotImplementedException)
						{
							supporstFreeStyleGain = false;
						}
					}
					else
						return false;
				}

				return supporstFreeStyleGain.Value;
			}
		}

		private bool? supporstDiscreteGain = null;
		public bool SupporstDiscreteGain
		{
			get
			{
				if (video != null && video.Connected)
				{
					if (supporstDiscreteGain == null)
					{
						try
						{
							supporstDiscreteGain = !SupporstFreeStyleGain && video.Gains != null && video.Gains.Count > 0;
						}
						catch (PropertyNotImplementedException)
						{
							supporstDiscreteGain = false;
						}
					}

					return supporstDiscreteGain.Value;
				}
				else
					return false;
			}
		}

		public bool CanDecreaseGain
		{
			get
			{
				if (SupporstFreeStyleGain)
					return gainSlots != null && video.GainMin < video.Gain;
				else if (SupporstDiscreteGain)
					return video.Gain > 0;
				else
					return false;
			}
		}

		public bool CanIncreaseGain
		{
			get
			{
				if (SupporstFreeStyleGain)
					return gainSlots != null && video.GainMax > video.Gain;
				else if (SupporstDiscreteGain)
					return video.Gain < video.Gains.Count - 1;
				else
					return false;
			}
		}

		public string Gain
		{
			get
			{
				if (SupporstFreeStyleGain)
					return video.Gain.ToString(CultureInfo.CurrentCulture);
				else if (SupporstDiscreteGain)
					return (string)video.Gains[video.Gain];

				return "N/A";
			}

		}

		public void IncreaseGain()
		{
			if (CanIncreaseGain)
			{
				if (SupporstFreeStyleGain && gainSlots != null)
				{
					int slotIndex = -1;
					for (int i = 0; i < gainSlots.Length; i++)
					{
						if (gainSlots[i] > video.Gain)
						{
							slotIndex = i;
							break;
						}
					}

					if (slotIndex >= gainSlots.Length)
						video.Gain = video.GainMax;
					else if (slotIndex <= 0)
						video.Gain = video.GainMin;
					else
						video.Gain = gainSlots[slotIndex];
				}
				else if (SupporstDiscreteGain)
					video.Gain++;
			}
		}

		public void DecreaseGain()
		{
			if (SupporstFreeStyleGain && gainSlots != null)
			{
				int slotIndex = gainSlots.Length;
				for (int i = gainSlots.Length - 1; i >= 0; i--)
				{
					if (gainSlots[i] < video.Gain)
					{
						slotIndex = i;
						break;
					}
				}

				if (slotIndex >= gainSlots.Length)
					video.Gain = video.GainMax;
				else if (slotIndex <= 0)
					video.Gain = video.GainMin;
				else
					video.Gain = gainSlots[slotIndex];
			}
			else if (SupporstDiscreteGain)
				video.Gain--;
		}

		public bool CanIncreaseIntegration
		{
			get
			{
				if (SupportsIntegrationRates && 
					video.SupportedIntegrationRates.Count > 1)
					return video.IntegrationRate < video.SupportedIntegrationRates.Count - 1;
				else 
					return false;
			}
		}

		public bool CanDecreaseIntegration
		{
			get
			{
				if (SupportsIntegrationRates && 
					video.SupportedIntegrationRates.Count > 1)
					return video.IntegrationRate > 0;
				else
					return false;
			}
		}

		private bool? supportsIntegrationRates = null;
		private bool SupportsIntegrationRates
		{
			get
			{
				if (supportsIntegrationRates == null)
				{
					try
					{
						supportsIntegrationRates = video.SupportedIntegrationRates.Count > 1;
					}
					catch (PropertyNotImplementedException)
					{
						supportsIntegrationRates = false;
					}
				}

				return supportsIntegrationRates.Value;
			}
		}

		public string Integration
		{
			get
			{
				if (SupportsIntegrationRates && 
					video.SupportedIntegrationRates.Count > 1)
				{
					int integrationRate = video.IntegrationRate;

					if (integrationRate >= 0 && integrationRate <= video.SupportedIntegrationRates.Count - 1)
						return Convert.ToString(video.SupportedIntegrationRates[integrationRate]);
					else
						return string.Empty;
				} 
				else 
					return "N/A";
			}

		}

		public void DecreaseIntegration()
		{
			if (CanDecreaseIntegration)
				video.IntegrationRate--;
		}

		public void IncreaseIntegration()
		{
			if (CanIncreaseIntegration)
				video.IntegrationRate++;
		}

		public bool HasSupportedActions
		{
			get
			{
				return 
					ShieldedCall(
						() =>
				             video != null &&
				             video.SupportedActions.Count > 0, 
						false);
			}
		}

		public string[] SupportedActions
		{
			get
			{
				if (video != null)
				{
					return
						ShieldedCall(
							() => video.SupportedActions.Cast<string>().ToArray(),
							new string[0]);	
				}
				else
					return new string[0];
			}
		}

		public string ExecuteAction(string actionName, string actionParameters)
		{
			return video.Action(actionName, actionParameters);
		}

		public IVideoFrame LastVideoFrame
		{
			get
			{
				if (video != null)
				{
					return
						ShieldedCall(
							() => video.LastVideoFrame,
							null);
				}
				else
					return null;
			}
		}

		public IVideoFrame LastVideoFrameVariant
		{
			get
			{
				if (video != null)
				{
					return
						ShieldedCall(
							() => video.LastVideoFrameImageArrayVariant,
							null);
				}
				else
					return null;
			}
		}

		public string CameraFrameRate
		{
			get
			{
				if (video != null)
					return video.FrameRate.ToString();

				return string.Empty;
			}
		}

		public string CameraBitDepth
		{
			get
			{
				if (video != null)
					return video.BitDepth.ToString(CultureInfo.CurrentCulture);

				return string.Empty;
			}
		}

		private string sensorName = null;
		private string SensorName
		{
			get
			{
				if (sensorName == null)
				{
					try
					{
						sensorName = video.SensorName;
					}
					catch (PropertyNotImplementedException)
					{
						sensorName = "Unknown";
					}
				}

				return sensorName;
			}
		}


		public string CameraSensorInfo
		{
			get
			{
				if (video != null)
				{
					string sensorType = ShieldedCall(() => video.SensorType.ToString(), null, false);

					if (sensorName != null)
						return string.Format("{0}{1}\r\n", SensorName ?? "Unknown", sensorType != null ? " " + sensorType : string.Empty);
					else
						return "Unknown";
				}

				return "N/A";
			}
		}

		public string CameraCCDCellSize
		{
			get
			{
				if (video != null)
				{
					string pixelSizeX = PixelSizeX.ToString(CultureInfo.CurrentCulture);
					string pixelSizeY = PixelSizeY.ToString(CultureInfo.CurrentCulture);

					if (pixelSizeX != null && pixelSizeY != null)
						return string.Format("{0}um x {1}um", pixelSizeX, pixelSizeY);
					else
						return "Unknown";
				}

				return "N/A";
			}
		}

		private int? pixelSizeX = null;
		private int PixelSizeX
		{
			get
			{
				if (pixelSizeX == null)
				{
					try
					{
						pixelSizeX = video.CameraXSize;	
					}
					catch (PropertyNotImplementedException)
					{
						pixelSizeX = 0;
					}					
				}

				return pixelSizeX.Value;
			}
		}

		private int? pixelSizeY = null;
		private int PixelSizeY
		{
			get
			{
				if (pixelSizeY == null)
				{
					try
					{
						pixelSizeY = video.CameraYSize;
					}
					catch (PropertyNotImplementedException)
					{
						pixelSizeY = 0;
					}
				}

				return pixelSizeY.Value;
			}
		}

		public string CameraCCDDimentionInPixels
		{
			get
			{
				if (video != null)
				{
					string pixelsX = PixelSizeX.ToString(CultureInfo.CurrentCulture);
					string pixelsY = PixelSizeY.ToString(CultureInfo.CurrentCulture);

					if (pixelsX != null && pixelsY != null)
						return string.Format("{0} x {1}", pixelsX, pixelsY);
					else
						return "Unknown";
				}

				return "N/A";
			}
		}

		public int Width
		{
			get
			{
				if (video != null)
				{
					return ShieldedCall(() => video.Width, 0, false);
				}
				else
					return 0;
			}
		}

		public int Height
		{
			get
			{
				if (video != null)
				{
					return ShieldedCall(() => video.Height, 0, false);
				}
				else
					return 0;
			}
		}

		public string ExposureRangeInfo
		{
			get
			{
				if (video != null)
				{
					string expMin = ShieldedCall(() => video.ExposureMin.ToString("0.000", CultureInfo.CurrentCulture), null, false);
					string expMax = ShieldedCall(() => video.ExposureMax.ToString("0.000", CultureInfo.CurrentCulture), null, false);

					if (expMin != null && expMax != null)
					{
						if (expMin == expMax)
							return string.Format("Fixed {0} sec ({1})", expMin, video.FrameRate);
						else
							return string.Format("{0} sec - {1} sec ({2})", expMin, expMax, video.FrameRate);
					}						
					else
						return "Unknown";
				}
				else
					return "N/A";
			}
		}

		public string GainRangeInfo
		{
			get
			{
				if (video != null)
				{
					string gainMin = ShieldedCall(() => video.GainMin.ToString(CultureInfo.CurrentCulture), null, false);
					string gainMax = ShieldedCall(() => video.GainMax.ToString(CultureInfo.CurrentCulture), null, false);
					int numFixedGains = ShieldedCall(() => video.Gains.Count, -1, false);

					if (gainMin != null && gainMax != null)
						return string.Format("{0} - {1}", gainMin, gainMax);
					else if (numFixedGains > 0)
					{
						return string.Format("{0} - {1}", video.Gains[0], video.Gains[numFixedGains - 1]);
					}
					else
						return "Unknown";
				}
				else
					return "N/A";
			}
		}

		public string GammaRangeInfo
		{
			get
			{
				if (video != null)
				{
					int numGammas = ShieldedCall(() => video.Gammas.Count, -1, false);

					if (numGammas > 0)
					{
						return string.Format("{0} - {1}", video.Gammas[0], video.Gammas[numGammas - 1]);
					}
					else
						return "Unknown";
				}
				else
					return "N/A";
			}
		}

		public string BufferingInfo
		{
			get
			{
				if (video != null)
				{
					int bufferSize = ShieldedCall(() => video.VideoFramesBufferSize, -1, false);

					if (bufferSize > 1)
					{
						return string.Format("{0} frames", bufferSize);
					}
					else
						return "Not Supported";
				}
				else
					return "N/A";
			}
		}

		public string CameraVideoFormat
		{
			get
			{
				if (video != null)
				{
					string fileFormat = ShieldedCall(() => video.VideoFileFormat, string.Empty);
					string codec = ShieldedCall(() => video.VideoCodec, null);
					return string.Format("{0}{1}", fileFormat, codec != null ? ", " + codec : string.Empty);
				}

				return string.Empty;
			}
		}

		public string InterfaceVersion
		{
			get
			{
				if (video != null)
				{
					return ShieldedCall(() => video.InterfaceVersion.ToString(CultureInfo.CurrentCulture), string.Empty, false);
				}

				return string.Empty;
			}
		}

		public string DriverInfo
		{
			get
			{
				if (video != null)
				{
					return ShieldedCall(() => video.DriverInfo, string.Empty, false);
				}

				return string.Empty;
			}
		}

		public string DriverVersion
		{
			get
			{
				if (video != null)
				{
					return ShieldedCall(() => video.DriverVersion, string.Empty, false);
				}

				return string.Empty;
			}
		}

		public string DeviceName
		{
			get
			{
				if (video != null)
				{
					return ShieldedCall(() => video.Name, string.Empty);
				}

				return string.Empty;
			}
		}

		public string DeviceDescription
		{
			get
			{
				if (video != null)
				{
					return ShieldedCall(() => video.Description, string.Empty, false);
				}

				return string.Empty;
			}
		}

		public string VideoCaptureDeviceName
		{
			get
			{
				if (video != null)
				{
					return ShieldedCall(() => video.VideoCaptureDeviceName, string.Empty, false);
				}

				return string.Empty;
			}
		}

	    public bool SupportsCrossbar
	    {
	        get { return video is ISupportsCrossbar; }
	    }

        public void LoadCrossbarSources(ComboBox cbxCrossbarInput)
        {
            if (video is ISupportsCrossbar)
                ((ISupportsCrossbar)video).LoadCrossbarSources(cbxCrossbarInput);    
        }
        

		private T ShieldedCall<T>(Func<T> method, T errorValue)
		{
			return ShieldedCall(method, errorValue, true);
		}

		private T ShieldedCall<T>(Func<T> method, T errorValue, bool showError)
		{
			try
			{
				return method();
			}
			catch(DriverException dex)
			{
				if (showError)
					frmUnexpectedError.ShowErrorMessage(dex);

				return errorValue;
			}
		}

		internal string StartRecording(string fileName)
		{
			if (video != null)
			{
				return ShieldedCall(() => video.StartRecordingVideoFile(fileName), null);
			}

			return null;
		}

		internal void StopRecording()
		{
			if (video != null)
			{
				ShieldedCall<string>(
					() =>
						{
							video.StopRecordingVideoFile();
							return null;
						}, 
					null);
			}
		}
	}
}
