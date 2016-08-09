//tabs=4
// --------------------------------------------------------------------------------
// TODO fill in this information for your driver, then remove this line!
//
// ASCOM Video driver for QHYCCD
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

namespace ASCOM.QHYCCD
{
    //
    // Your driver's DeviceID is ASCOM.QHYCCD.Video
    //
    // The Guid attribute sets the CLSID for ASCOM.QHYCCD.Video
    // The ClassInterface/None addribute prevents an empty interface called
    // _QHYCCD from being created and used as the [default] interface
    //
    // TODO Replace the not implemented exceptions with code to implement the function or
    // throw the appropriate ASCOM exception.
    //

    /// <summary>
    /// ASCOM Video Driver for QHYCCD.
    /// </summary>
    [Guid("d7c38d6a-3ae0-45f1-90ae-1adf6bf047c0")]
    [ClassInterface(ClassInterfaceType.None)]
    public class Video : IVideo
    {
        /// <summary>
        /// ASCOM DeviceID (COM ProgID) for this driver.
        /// The DeviceID is used by ASCOM applications to load the driver at runtime.
        /// </summary>
        internal static string driverID = "ASCOM.QHYCCD.Video";
        // TODO Change the descriptive string for your driver then remove this line
        /// <summary>
        /// Driver description that displays in the ASCOM Chooser.
        /// </summary>
        private static string driverDescription = "ASCOM Video Driver for QHYCCD.";

        internal static string comPortProfileName = "COM Port"; // Constants used for Profile persistence
        internal static string comPortDefault = "COM1";
        internal static string traceStateProfileName = "Trace Level";
        internal static string traceStateDefault = "false";

        internal static string comPort; // Variables to hold the currrent device configuration
        internal static bool traceState;

        /// <summary>
        /// Private variable to hold the connected state
        /// </summary>
        private bool connectedState;

        /// <summary>
        /// Private variable to hold an ASCOM Utilities object
        /// </summary>
        private Util utilities;

        /// <summary>
        /// Private variable to hold an ASCOM AstroUtilities object to provide the Range method
        /// </summary>
        private AstroUtils astroUtilities;

        /// <summary>
        /// Private variable to hold the trace logger object (creates a diagnostic log file with information that you specify)
        /// </summary>
        private TraceLogger tl;

        /// <summary>
        /// Initializes a new instance of the <see cref="QHYCCD"/> class.
        /// Must be public for COM registration.
        /// </summary>
        public Video()
        {
            ReadProfile(); // Read device configuration from the ASCOM Profile store

            tl = new TraceLogger("", "QHYCCD");
            tl.Enabled = traceState;
            tl.LogMessage("Video", "Starting initialisation");

            connectedState = false; // Initialise connected to false
            utilities = new Util(); //Initialise util object
            astroUtilities = new AstroUtils(); // Initialise astro utilities object
            //TODO: Implement your additional construction here

            tl.LogMessage("Video", "Completed initialisation");
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
                tl.LogMessage("SupportedActions Get", "Returning empty arraylist");
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
            tl.Enabled = false;
            tl.Dispose();
            tl = null;
            utilities.Dispose();
            utilities = null;
            astroUtilities.Dispose();
            astroUtilities = null;
        }

        public bool Connected
        {
            get
            {
                tl.LogMessage("Connected Get", IsConnected.ToString());
                return IsConnected;
            }
            set
            {
                tl.LogMessage("Connected Set", value.ToString());
                if (value == IsConnected)
                    return;

                if (value)
                {
                    connectedState = true;
                    tl.LogMessage("Connected Set", "Connecting to port " + comPort);
                    // TODO connect to the device
                }
                else
                {
                    connectedState = false;
                    tl.LogMessage("Connected Set", "Disconnecting from port " + comPort);
                    // TODO disconnect from the device
                }
            }
        }

        public string Description
        {
            // TODO customise this device description
            get
            {
                tl.LogMessage("Description Get", driverDescription);
                return driverDescription;
            }
        }

        public string DriverInfo
        {
            get
            {
                Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                // TODO customise this driver description
                string driverInfo = "Information about the driver itself. Version: " + String.Format(CultureInfo.InvariantCulture, "{0}.{1}", version.Major, version.Minor);
                tl.LogMessage("DriverInfo Get", driverInfo);
                return driverInfo;
            }
        }

        public string DriverVersion
        {
            get
            {
                Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                string driverVersion = String.Format(CultureInfo.InvariantCulture, "{0}.{1}", version.Major, version.Minor);
                tl.LogMessage("DriverVersion Get", driverVersion);
                return driverVersion;
            }
        }

        public short InterfaceVersion
        {
            // set by the driver wizard
            get
            {
                tl.LogMessage("InterfaceVersion Get", "1");
                return Convert.ToInt16("1");
            }
        }

        public string Name
        {
            get
            {
                string name = "Short driver name - please customise";
                tl.LogMessage("Name Get", name);
                return name;
            }
        }

        #endregion

        #region IVideo Implementation

        private const int deviceWidth = 720; // Constants to define the ccd pixel dimenstions
        private const int deviceHeight = 480;
        private const int bitDepth = 8;
        private const string videoFileFormat = "AVI";

        public int BitDepth
        {
            get
            {
                tl.LogMessage("BitDepth Get", bitDepth.ToString());
                return bitDepth;
            }
        }

        public VideoCameraState CameraState
        {
            get { return VideoCameraState.videoCameraError; }
        }

        public bool CanConfigureDeviceProperties
        {
            get
            {
                return false;
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
                // Standard NTSC frame duration
                return 0.03337;
            }
        }

        public double ExposureMin
        {
            get
            {
                // Standard NTSC frame duration
                return 0.03337;
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

            get
            {
                throw new PropertyNotImplementedException("Gain", false);
            }


            set
            {
                throw new PropertyNotImplementedException("Gain", true);
            }
        }

        ///	<exception cref="T:ASCOM.NotConnectedException">Must throw an exception if the information is not available. (Some drivers may require an 
        ///	active <see cref="P:ASCOM.DeviceInterface.IVideo.Connected">connection</see> in order to retrieve necessary information from the camera.)</exception>
        ///	<exception cref="T:ASCOM.PropertyNotImplementedException">Must throw an exception if gainmax is not supported</exception>
        public short GainMax
        {

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

            get
            {
                throw new PropertyNotImplementedException("Gamma", false);
            }


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

            get
            {
                throw new PropertyNotImplementedException("Gammas", false);
            }
        }

        public int Height
        {
            get
            {
                tl.LogMessage("Height Get", deviceHeight.ToString());
                return deviceHeight;
            }
        }

        ///	<exception cref="T:ASCOM.NotConnectedException">Must throw an exception if the information is not available. (Some drivers may require an 
        ///	active <see cref="P:ASCOM.DeviceInterface.IVideo.Connected">connection</see> in order to retrieve necessary information from the camera.)</exception>
        ///	<exception cref="T:ASCOM.InvalidValueException">Must throw an exception if not valid.</exception>
        ///	<exception cref="T:ASCOM.PropertyNotImplementedException">Must throw an exception if the camera supports only one integration rate (exposure) that cannot be changed.</exception>
        public int IntegrationRate
        {

            get
            {
                throw new PropertyNotImplementedException("IntegrationRate", false);
            }


            set
            {
                throw new PropertyNotImplementedException("IntegrationRate", true);
            }
        }

        public IVideoFrame LastVideoFrame
        {
            get { throw new ASCOM.InvalidOperationException("There are no video frames available."); }
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
            tl.LogMessage("StartRecordingVideoFile", "Supplied file name: " + PreferredFileName);
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
                throw new PropertyNotImplementedException("SupportedIntegrationRates", false);
            }
        }

        public string VideoCaptureDeviceName
        {
            get { return string.Empty; }
        }

        public string VideoCodec
        {
            get { return string.Empty; }
        }

        public string VideoFileFormat
        {
            get
            {
                tl.LogMessage("VideoFileFormat Get", videoFileFormat);
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
                tl.LogMessage("Height Width", deviceWidth.ToString());
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
                    P.Register(driverID, driverDescription);
                }
                else
                {
                    P.Unregister(driverID);
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
                return connectedState;
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
                traceState = Convert.ToBoolean(driverProfile.GetValue(driverID, traceStateProfileName, string.Empty, traceStateDefault));
                comPort = driverProfile.GetValue(driverID, comPortProfileName, string.Empty, comPortDefault);
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
                driverProfile.WriteValue(driverID, traceStateProfileName, traceState.ToString());
                driverProfile.WriteValue(driverID, comPortProfileName, comPort.ToString());
            }
        }

        #endregion

    }
}
