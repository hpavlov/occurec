//tabs=4
// --------------------------------------------------------------------------------
// TODO fill in this information for your driver, then remove this line!
//
// ASCOM Video driver for GenericCCDCamera
//
// Description:	Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam 
//				nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam 
//				erat, sed diam voluptua. At vero eos et accusam et justo duo 
//				dolores et ea rebum. Stet clita kasd gubergren, no sea takimata 
//				sanctus est Lorem ipsum dolor sit amet.
//
// Implements:	ASCOM Video interface version: <To be completed by driver developer>
// Author:		(XXX) Your N. Here <your@email.here>
//
// Edit Log:
//
// Date			Who	Vers	Description
// -----------	---	-----	-------------------------------------------------------
// dd-mmm-yyyy	XXX	6.0.0	Initial edit, created from ASCOM driver template
// --------------------------------------------------------------------------------
//


// This is used to define code in the template that is specific to one class implementation
// unused code canbe deleted and this definition removed.
#define Video

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Runtime.InteropServices;

using ASCOM;
using ASCOM.Astrometry;
using ASCOM.Astrometry.AstroUtils;
using ASCOM.Utilities;
using ASCOM.DeviceInterface;
using System.Globalization;
using System.Collections;
using ASCOM.DriverAccess;

namespace ASCOM.GenericCCDCamera
{
	//
	// Your driver's DeviceID is ASCOM.GenericCCDCamera.Video
	//
	// The Guid attribute sets the CLSID for ASCOM.GenericCCDCamera.Video
	// The ClassInterface/None addribute prevents an empty interface called
	// _GenericCCDCamera from being created and used as the [default] interface
	//

	/// <summary>
	/// ASCOM Video Driver for GenericCCDCamera.
	/// </summary>
	[Guid("e83f34ee-66a6-4fa2-a309-877ba744c658")]
	[ClassInterface(ClassInterfaceType.None)]
	public class Video : IVideo
	{
		/// <summary>
		/// ASCOM DeviceID (COM ProgID) for this driver.
		/// The DeviceID is used by ASCOM applications to load the driver at runtime.
		/// </summary>
		internal static string m_driverID = "ASCOM.GenericCCDCamera.Video";

		/// <summary>
		/// Driver description that displays in the ASCOM Chooser.
		/// </summary>
		private static string m_driverDescription = "Video for CCD camera";

		internal static string m_comPortProfileName = "COM Port"; // Constants used for Profile persistence
		internal static string m_comPortDefault = "COM1";
		internal static string m_traceStateProfileName = "Trace Level";
		internal static string m_traceStateDefault = "false";
		internal static string m_ccdDriverProgIdProfileName = "CCD Driver ProgId";

		internal static string m_comPort; // Variables to hold the currrent device configuration
		internal static bool m_traceState;
		internal static string m_ccdDriverProgId;

		/// <summary>
		/// Private variable to hold the connected state
		/// </summary>
		private bool m_connectedState;

		/// <summary>
		/// Private variable to hold an ASCOM Utilities object
		/// </summary>
		private Util m_utilities;

		/// <summary>
		/// Private variable to hold an ASCOM AstroUtilities object to provide the Range method
		/// </summary>
		private AstroUtils m_astroUtilities;

		/// <summary>
		/// Private variable to hold the trace logger object (creates a diagnostic log file with information that you specify)
		/// </summary>
		private TraceLogger m_tl;

		private CCDVideoImpl m_video;

		/// <summary>
		/// Initializes a new instance of the <see cref="GenericCCDCamera"/> class.
		/// Must be public for COM registration.
		/// </summary>
		public Video()
		{
			ReadProfile(); // Read device configuration from the ASCOM Profile store

			m_tl = new TraceLogger("", "GenericCCDCamera");
			m_tl.Enabled = m_traceState;
			m_tl.LogMessage("Video", "Starting initialisation");

			m_connectedState = false; // Initialise connected to false
			m_utilities = new Util(); //Initialise util object
			m_astroUtilities = new AstroUtils(); // Initialise astro utilities object
			//TODO: Implement your additional construction here

			m_tl.LogMessage("Video", "Completed initialisation");
		}


		//
		// PUBLIC COM INTERFACE IVideo IMPLEMENTATION
		//

		#region Common properties and methods.

		/// <summary>
		/// Displays the Setup Dialog form.
		/// If the user clicks the OK button to dismiss the form, then
		/// the new settings are saved, otherwise the old values are reloaded.
		/// THIS IS THE ONLY PLACE WHERE SHOWING USER INTERFACE IS ALLOWED!
		/// </summary>
		public void SetupDialog()
		{
			// consider only showing the setup dialog if not connected
			// or call a different dialog if connected
			if (IsConnected)
				System.Windows.Forms.MessageBox.Show("Already connected, just press OK");

			using (SetupDialogForm F = new SetupDialogForm())
			{
				var result = F.ShowDialog();
				if (result == System.Windows.Forms.DialogResult.OK)
				{
					WriteProfile(); // Persist device configuration values to the ASCOM Profile store
				}
			}
		}

		public ArrayList SupportedActions
		{
			get
			{
				return new ArrayList();
			}
		}

		public string Action(string actionName, string actionParameters)
		{
			throw new ASCOM.ActionNotImplementedException("Action " + actionName + " is not implemented by this driver");
		}

		public void CommandBlind(string command, bool raw)
		{
			CheckConnected("CommandBlind");
			// Call CommandString and return as soon as it finishes
			this.CommandString(command, raw);
			// or
			throw new ASCOM.MethodNotImplementedException("CommandBlind");
		}

		public bool CommandBool(string command, bool raw)
		{
			CheckConnected("CommandBool");
			string ret = CommandString(command, raw);
			// TODO decode the return string and return true or false
			// or
			throw new ASCOM.MethodNotImplementedException("CommandBool");
		}

		public string CommandString(string command, bool raw)
		{
			CheckConnected("CommandString");
			// it's a good idea to put all the low level communication with the device here,
			// then all communication calls this function
			// you need something to ensure that only one command is in progress at a time

			throw new ASCOM.MethodNotImplementedException("CommandString");
		}

		public void Dispose()
		{
			// Clean up the tracelogger and util objects
			m_tl.Enabled = false;
			m_tl.Dispose();
			m_tl = null;
			m_utilities.Dispose();
			m_utilities = null;
			m_astroUtilities.Dispose();
			m_astroUtilities = null;
			if (m_video != null)
			{
				m_video.Dispose();
				m_video = null;
			}
		}

		public bool Connected
		{
			get
			{
				m_tl.LogMessage("Connected Get", IsConnected.ToString());
				return m_video != null && m_video.Connected;
			}
			set
			{
				m_tl.LogMessage("Connected Set", value.ToString());
				if (value == IsConnected)
					return;

				if (m_video == null)
				{
					m_video = new CCDVideoImpl(m_ccdDriverProgId);
				}

				if (value)
				{
					m_connectedState = true;
					m_tl.LogMessage("Connected Set", "Connecting to port " + m_comPort);

					m_video.Connected = true;
				}
				else
				{
					m_connectedState = false;
					m_tl.LogMessage("Connected Set", "Disconnecting from port " + m_comPort);

					m_video.Connected = false;
				}
			}
		}

		public string Description
		{
			get
			{
				m_tl.LogMessage("Description Get", m_driverDescription);
				return m_driverDescription;
			}
		}

		public string DriverInfo
		{
			get
			{
				Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

				string driverInfo = "Video driver for generic CCD camera. Version: " + String.Format(CultureInfo.InvariantCulture, "{0}.{1}", version.Major, version.Minor);
				m_tl.LogMessage("DriverInfo Get", driverInfo);
				return driverInfo;
			}
		}

		public string DriverVersion
		{
			get
			{
				Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
				string driverVersion = String.Format(CultureInfo.InvariantCulture, "{0}.{1}", version.Major, version.Minor);
				m_tl.LogMessage("DriverVersion Get", driverVersion);
				return driverVersion;
			}
		}

		public short InterfaceVersion
		{
			// set by the driver wizard
			get
			{
				m_tl.LogMessage("InterfaceVersion Get", "1");
				return Convert.ToInt16("1");
			}
		}

		public string Name
		{
			get
			{
				string name = "VideoForCCDCamera";
				m_tl.LogMessage("Name Get", name);
				return name;
			}
		}

		#endregion

		#region IVideo Implementation

		private const int deviceWidth = 720; // Constants to define the ccd pixel dimenstions
		private const int deviceHeight = 480;

		private const int bitDepth = 16;
		private const string videoFileFormat = "ADV";

		public int BitDepth
		{
			get
			{
				m_tl.LogMessage("BitDepth Get", bitDepth.ToString());
				return bitDepth;
			}
		}

		public VideoCameraState CameraState
		{
			get { return m_video != null ? m_video.CameraState : VideoCameraState.videoCameraError; }
		}

		public bool CanConfigureDeviceProperties
		{
			get
			{
				return true;
			}
		}

		public void ConfigureDeviceProperties()
		{
			throw new ASCOM.PropertyNotImplementedException();
		}

		public double ExposureMax
		{
			get
			{
				if (!m_video.Connected)
					throw new ASCOM.NotConnectedException();

				return m_video.ExposureMax;
			}
		}

		public double ExposureMin
		{
			get
			{
				if (!m_video.Connected)
					throw new ASCOM.NotConnectedException();

				return m_video.ExposureMin;
			}
		}

		public VideoCameraFrameRate FrameRate
		{
			get { return VideoCameraFrameRate.NTSC; }
		}

		///	<exception cref="T:ASCOM.NotConnectedException">Must throw an exception if the information is not available. (Some drivers may require an 
		///	active <see cref="P:ASCOM.DeviceInterface.IVideo.Connected">connection</see> in order to retrieve necessary information from the camera.)</exception>
		///	<exception cref="T:ASCOM.InvalidValueException">Must throw an exception if not valid.</exception>
		///	<exception cref="T:ASCOM.PropertyNotImplementedException">Must throw an exception if gain is not supported</exception>
		public short Gain
		{
			[DebuggerStepThrough]
			get { throw new PropertyNotImplementedException("Gain", false); }

			[DebuggerStepThrough]
			set { throw new PropertyNotImplementedException("Gain", true); }
		}

		///	<exception cref="T:ASCOM.NotConnectedException">Must throw an exception if the information is not available. (Some drivers may require an 
		///	active <see cref="P:ASCOM.DeviceInterface.IVideo.Connected">connection</see> in order to retrieve necessary information from the camera.)</exception>
		///	<exception cref="T:ASCOM.PropertyNotImplementedException">Must throw an exception if gainmax is not supported</exception>
		public short GainMax
		{
			[DebuggerStepThrough]
			get
			{
				throw new PropertyNotImplementedException("GainMax", false);
			}
		}

		///	<exception cref="T:ASCOM.NotConnectedException">Must throw an exception if the information is not available. (Some drivers may require an 
		///	active <see cref="P:ASCOM.DeviceInterface.IVideo.Connected">connection</see> in order to retrieve necessary information from the camera.)</exception>
		///	<exception cref="T:ASCOM.PropertyNotImplementedException">Must throw an exception if gainmin is not supported</exception>		
		public short GainMin
		{
			[DebuggerStepThrough]
			get
			{
				throw new PropertyNotImplementedException("GainMin", false);
			}
		}

		///	<exception cref="T:ASCOM.NotConnectedException">Must throw an exception if the information is not available. (Some drivers may require an 
		///	active <see cref="P:ASCOM.DeviceInterface.IVideo.Connected">connection</see> in order to retrieve necessary information from the camera.)</exception>
		///	<exception cref="T:ASCOM.PropertyNotImplementedException">Must throw an exception if Gains is not supported</exception>
		public ArrayList Gains
		{
			[DebuggerStepThrough]
			get
			{
				throw new PropertyNotImplementedException("Gains", false);
			}
		}

		///	<exception cref="T:ASCOM.NotConnectedException">Must throw an exception if the information is not available. (Some drivers may require an 
		///	active <see cref="P:ASCOM.DeviceInterface.IVideo.Connected">connection</see> in order to retrieve necessary information from the camera.)</exception>
		///	<exception cref="T:ASCOM.InvalidValueException">Must throw an exception if not valid.</exception>
		///	<exception cref="T:ASCOM.PropertyNotImplementedException">Must throw an exception if gamma is not supported</exception>
		public short Gamma
		{
			[DebuggerStepThrough]
			get
			{
				throw new PropertyNotImplementedException("Gamma", false);
			}

			[DebuggerStepThrough]
			set
			{
				throw new PropertyNotImplementedException("Gamma", true);
			}
		}

		///	<exception cref="T:ASCOM.NotConnectedException">Must throw an exception if the information is not available. (Some drivers may require an 
		///	active <see cref="P:ASCOM.DeviceInterface.IVideo.Connected">connection</see> in order to retrieve necessary information from the camera.)</exception>
		///	<exception cref="T:ASCOM.PropertyNotImplementedException">Must throw an exception if gainmax is not supported</exception>
		public short GammaMax
		{
			[DebuggerStepThrough]
			get
			{
				throw new PropertyNotImplementedException("GainMax", false);
			}
		}

		///	<exception cref="T:ASCOM.NotConnectedException">Must throw an exception if the information is not available. (Some drivers may require an 
		///	active <see cref="P:ASCOM.DeviceInterface.IVideo.Connected">connection</see> in order to retrieve necessary information from the camera.)</exception>
		///	<exception cref="T:ASCOM.PropertyNotImplementedException">Must throw an exception if gainmin is not supported</exception>
		public short GammaMin
		{
			[DebuggerStepThrough]
			get
			{
				throw new PropertyNotImplementedException("GainMin", false);
			}
		}

		///	<exception cref="T:ASCOM.NotConnectedException">Must throw an exception if the information is not available. (Some drivers may require an 
		///	active <see cref="P:ASCOM.DeviceInterface.IVideo.Connected">connection</see> in order to retrieve necessary information from the camera.)</exception>
		///	<exception cref="T:ASCOM.PropertyNotImplementedException">Must throw an exception if gainmin is not supported</exception>
		public ArrayList Gammas
		{
			[DebuggerStepThrough]
			get
			{
				throw new PropertyNotImplementedException("Gammas", false);
			}
		}

		public int Height
		{
			get
			{
				m_tl.LogMessage("Height Get", deviceHeight.ToString());
				return deviceHeight;
			}
		}


		private int m_IntegrationRate = -1;

		///	<exception cref="T:ASCOM.NotConnectedException">Must throw an exception if the information is not available. (Some drivers may require an 
		///	active <see cref="P:ASCOM.DeviceInterface.IVideo.Connected">connection</see> in order to retrieve necessary information from the camera.)</exception>
		///	<exception cref="T:ASCOM.InvalidValueException">Must throw an exception if not valid.</exception>
		///	<exception cref="T:ASCOM.PropertyNotImplementedException">Must throw an exception if the camera supports only one integration rate (exposure) that cannot be changed.</exception>
		public int IntegrationRate
		{
			get
			{
				if (!m_video.Connected) throw new NotConnectedException();

				return m_video.IntegrationRate;
			}


			set
			{
				if (!m_video.Connected) throw new NotConnectedException();

				m_video.IntegrationRate = value;
			}
		}

		public IVideoFrame LastVideoFrame
		{
			get
			{
				if (m_video != null)
					return m_video.GetLastVideoFrame();

				return null;
			}
		}

		///	<exception cref="T:ASCOM.NotConnectedException">Must throw exception if data unavailable.</exception>
		/// <exception cref="T:ASCOM.PropertyNotImplementedException">Must throw exception if not implemented.</exception>
		public double PixelSizeX
		{

			get
			{
				throw new PropertyNotImplementedException("PixelSizeX", false);
			}
		}

		///	<exception cref="T:ASCOM.NotConnectedException">Must throw exception if data unavailable.</exception>
		/// <exception cref="T:ASCOM.PropertyNotImplementedException">Must throw exception if not implemented.</exception>
		public double PixelSizeY
		{

			get
			{
				throw new PropertyNotImplementedException("PixelSizeY", false);
			}
		}

		///	<exception cref="T:ASCOM.NotConnectedException">Must throw an exception if the information is not available. (Some drivers may require an 
		///	active <see cref="P:ASCOM.DeviceInterface.IVideo.Connected">connection</see> in order to retrieve necessary information from the camera.)</exception>
		/// <exception cref="T:ASCOM.PropertyNotImplementedException">Must throw exception if not implemented.</exception>
		public string SensorName
		{

			get
			{
				throw new PropertyNotImplementedException("SensorName", false);
			}
		}

		public SensorType SensorType
		{
			get { return SensorType.Monochrome; }
		}

		public string StartRecordingVideoFile(string PreferredFileName)
		{
			m_tl.LogMessage("StartRecordingVideoFile", "Supplied file name: " + PreferredFileName);
			throw new InvalidOperationException("Cannot start recording a video file right now.");
		}

		public void StopRecordingVideoFile()
		{
			throw new InvalidOperationException("Cannot stop recording right now.");
		}

		/// <exception cref="T:ASCOM.NotConnectedException">Must throw exception if data unavailable.</exception>
		/// <exception cref="T:ASCOM.PropertyNotImplementedException">Must throw exception if camera supports only one integration rate (exposure) that cannot be changed.</exception>		
		public ArrayList SupportedIntegrationRates
		{

			get
			{
				if (!m_video.Connected)
					throw new ASCOM.NotConnectedException();

				return m_video.SupportedIntegrationRates;
			}
		}

		public string VideoCaptureDeviceName
		{
			get
			{
				if (m_video != null)
					return m_video.CCDDeviceName;

				return "Unknown";
			}
		}

		public string VideoCodec
		{
			get { return string.Empty; }
		}

		public string VideoFileFormat
		{
			get
			{
				m_tl.LogMessage("VideoFileFormat Get", videoFileFormat);
				return videoFileFormat;
			}
		}

		public int VideoFramesBufferSize
		{
			get { return 0; }
		}

		public int Width
		{
			get
			{
				m_tl.LogMessage("Height Width", deviceWidth.ToString());
				return deviceWidth;
			}
		}

		#endregion

		#region Private properties and methods
		// here are some useful properties and methods that can be used as required
		// to help with driver development

		#region ASCOM Registration

		// Register or unregister driver for ASCOM. This is harmless if already
		// registered or unregistered. 
		//
		/// <summary>
		/// Register or unregister the driver with the ASCOM Platform.
		/// This is harmless if the driver is already registered/unregistered.
		/// </summary>
		/// <param name="bRegister">If <c>true</c>, registers the driver, otherwise unregisters it.</param>
		private static void RegUnregASCOM(bool bRegister)
		{
			using (var P = new ASCOM.Utilities.Profile())
			{
				P.DeviceType = "Video";
				if (bRegister)
				{
					P.Register(m_driverID, m_driverDescription);
				}
				else
				{
					P.Unregister(m_driverID);
				}
			}
		}

		/// <summary>
		/// This function registers the driver with the ASCOM Chooser and
		/// is called automatically whenever this class is registered for COM Interop.
		/// </summary>
		/// <param name="t">Type of the class being registered, not used.</param>
		/// <remarks>
		/// This method typically runs in two distinct situations:
		/// <list type="numbered">
		/// <item>
		/// In Visual Studio, when the project is successfully built.
		/// For this to work correctly, the option <c>Register for COM Interop</c>
		/// must be enabled in the project settings.
		/// </item>
		/// <item>During setup, when the installer registers the assembly for COM Interop.</item>
		/// </list>
		/// This technique should mean that it is never necessary to manually register a driver with ASCOM.
		/// </remarks>
		[ComRegisterFunction]
		public static void RegisterASCOM(Type t)
		{
			RegUnregASCOM(true);
		}

		/// <summary>
		/// This function unregisters the driver from the ASCOM Chooser and
		/// is called automatically whenever this class is unregistered from COM Interop.
		/// </summary>
		/// <param name="t">Type of the class being registered, not used.</param>
		/// <remarks>
		/// This method typically runs in two distinct situations:
		/// <list type="numbered">
		/// <item>
		/// In Visual Studio, when the project is cleaned or prior to rebuilding.
		/// For this to work correctly, the option <c>Register for COM Interop</c>
		/// must be enabled in the project settings.
		/// </item>
		/// <item>During uninstall, when the installer unregisters the assembly from COM Interop.</item>
		/// </list>
		/// This technique should mean that it is never necessary to manually unregister a driver from ASCOM.
		/// </remarks>
		[ComUnregisterFunction]
		public static void UnregisterASCOM(Type t)
		{
			RegUnregASCOM(false);
		}

		#endregion

		/// <summary>
		/// Returns true if there is a valid connection to the driver hardware
		/// </summary>
		private bool IsConnected
		{
			get
			{
				// TODO check that the driver hardware connection exists and is connected to the hardware
				return m_connectedState;
			}
		}

		/// <summary>
		/// Use this function to throw an exception if we aren't connected to the hardware
		/// </summary>
		/// <param name="message"></param>
		private void CheckConnected(string message)
		{
			if (!IsConnected)
			{
				throw new ASCOM.NotConnectedException(message);
			}
		}

		/// <summary>
		/// Read the device configuration from the ASCOM Profile store
		/// </summary>
		internal void ReadProfile()
		{
			using (Profile driverProfile = new Profile())
			{
				driverProfile.DeviceType = "Video";
				m_traceState = Convert.ToBoolean(driverProfile.GetValue(m_driverID, m_traceStateProfileName, string.Empty, m_traceStateDefault));
				m_comPort = driverProfile.GetValue(m_driverID, m_comPortProfileName, string.Empty, m_comPortDefault);
				m_ccdDriverProgId = driverProfile.GetValue(m_driverID, m_ccdDriverProgIdProfileName, string.Empty, string.Empty);
			}
		}

		/// <summary>
		/// Write the device configuration to the  ASCOM  Profile store
		/// </summary>
		internal void WriteProfile()
		{
			using (Profile driverProfile = new Profile())
			{
				driverProfile.DeviceType = "Video";
				driverProfile.WriteValue(m_driverID, m_traceStateProfileName, m_traceState.ToString());
				driverProfile.WriteValue(m_driverID, m_comPortProfileName, m_comPort.ToString());
				driverProfile.WriteValue(m_driverID, m_ccdDriverProgIdProfileName, m_ccdDriverProgId.ToString());
			}
		}

		#endregion

	}
}
