//tabs=4
// --------------------------------------------------------------------------------
//
// ASCOM Video Driver - Simulator
//
// Description:	The IVideo and IVideoFrame interfaces ver 1
//
// Author:		(HDP) Hristo Pavlov <hristo_dpavlov@yahoo.com>
//
// --------------------------------------------------------------------------------
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace AAVRec.Drivers
{
	/// <summary>
	/// Defines the ASCOM <see cref="T:ASCOM.DeviceInterface.IVideo"/> and <see cref="T:ASCOM.DeviceInterface.IVideoFrame"/> interfaces for working with astronomical video cameras
	/// </summary>
	/// <remarks>
	/// The interfaces are designed with the following use cases in mind
	/// <list type="bullet">
	///		<item><description>Instructing the camera to record a video file and save it on the file system</description></item>
	///		<item><description>Occasionally obtaining individual video frames as the camera is running in order to help with focusing, frame rate adjustments, field identification and others.</description></item>
	///	</list>
	/// <p>Video cameras, unlike standard CCD cameras, are used to save video files. It is not practical to try and display in realtime what the camera seas mostly for the following two reasons. Firstly the huge bandwidth 
	/// that will be required to transmit the images (25, 30 or sometimes more images per second) and secondly because of the performance impact on the driver and the hardware. Video is already very resource hungry
	/// and implementing a separate streaming in the driver for realtime viewing by a client will make the system even more demanding for resources (RAM and CPU) that could affect the integrity of the video record (e.g. result in dropped frames) which is not desired and should be avoided.</p>
	/// <p>Still the client will need to see what the camera is pointed to for example to work on the focusing so there is a need for viewing individual images produced by the video camera. This is done is by the client requesting
	/// a single individual frame from the camera and doing this as many times as necessary.</p>
	/// <p>There are broadly two types of video cameras regarding the way the images are captured by a computer - analogue and digital. Analogue cameras will usually require a video capture card and digital cameras will usually be connected to the computer via USB or FireWire port. 
	/// Analogue video cameras output frames with a fixed frame rate that cannot be changed while the digital video cameras can have a variable frame rate. When it comes to exposure duration there are again two types of analogue video cameras - integrating and non integrating. 
	/// The integrating video cameras have the shutter open for the duration of the exposure and after the exposure is completed they output the same video frame (the result from the exposure) with their supported frame rate and for the duration of another exposure. 
	/// Non integrating video cameras have a fixed exposure duration that cannot be changed. Some integrating video cameras also offer exposures shorter than the frame rate interval. In this case the shutter is only open for the corresponding exposure time and the remaining time
	/// for up to the frame rate duration the camera is not collecting light. Digital video cameras usually output a single frame for each exposure that they produce.</p>
	/// <p>The ASCOM video camera interfaces are designed to support all these types of video cameras - analogue, digital, integrating and non integrating.</p>
	/// </remarks>
	[System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
	class NamespaceDoc
	{
	}

	/// <summary>
	/// ASCOM Video Camera supported frame rates.
	/// </summary>
	public enum VideoCameraFrameRate
	{
		/// <summary>
		/// This is a digital camera or system that supports variable frame rates.
		/// </summary>
		Digital = 0,

		/// <summary>
		/// This is a video camera that supports variable frame rates.
		/// </summary>
		Variable = 0,

		/// <summary>
		/// 25 frames per second (fps) corresponding to a <b>PAL</b> (colour) or <b>CCIR</b> (black and white) video standard.
		/// </summary>
		PAL = 1,

		/// <summary>
		/// 29.97  frames per second (fps) corresponding to an <b>NTSC</b> (colour) or <b>EIA</b>b> (black and white) video standard.
		/// </summary>
		NTSC = 2
	}

	/// <summary>
	/// ASCOM Video Camera status values.
	/// </summary>
	public enum VideoCameraState
	{
		/// <summary>
		/// Camera status idle. The video camera is expecting commands.
		/// </summary>
		videoCameraIdle = 0,

		/// <summary>
		/// Camera status running. The video camera is producing images and video frames are available for viewing or recording.
		/// </summary>
		videoCameraRunning = 1,

		/// <summary>
		/// Camera status recording. The video driver is recording video to the file system. Video frames are available for viewing.
		/// </summary>
		videoCameraRecording = 2,

		/// <summary>
		/// Camera status error. The video camera is in a state of an error and cannot continue its operation. Usually a restart will be required to resolve the error condition.
		/// </summary>
		videoCameraError = 3
	}

	/// <summary>
	/// Defines the IVideoFrame Interface.
	/// </summary>
	public interface IVideoFrame
	{
		/// <summary>
		/// Returns a safearray of int of size <see cref="P:ASCOM.DeviceInterface.IVideo.Width"/> * <see cref="P:ASCOM.DeviceInterface.IVideo.Height"/> containing the pixel values from the video frame. 
		/// </summary>
		/// <remarks>
		/// The application must inspect the Safearray parameters to determine the dimensions. 
		/// <para>The value will be only populated when the video frame has been obtained from the <see cref="P:ASCOM.DeviceInterface.IVideo.LastVideoFrame"/> property. When the video frame
		/// is obtained from the <see cref="P:ASCOM.DeviceInterface.IVideo.LastVideoFrameImageArrayVariant"/> property a NULL value must be returned. Do not throw an exception in this case.</para>
		/// <para>Color cameras will produce an array of  <see cref="P:ASCOM.DeviceInterface.IVideo.Width"/> * <see cref="P:ASCOM.DeviceInterface.IVideo.Height"/> * 3.
		/// The three planes are in the following order: R, G and B. If the application cannot handle color images, it should use just the first (R) plane.</para>
		/// <para>The pixels in the array start from the top left part of the image and are listed by horizontal lines/rows. The second pixels in the array is the second pixels from the first horizontal row
		/// and the second last pixel in the array is the second last pixels from the last horizontal row.</para>
		/// </remarks>
		/// <value>The image array.</value>
		object ImageArray { get; }

		/// <summary>
		/// Returns a safearray of Variant of size <see cref="P:ASCOM.DeviceInterface.IVideo.Width"/> * <see cref="P:ASCOM.DeviceInterface.IVideo.Height"/> containing the pixel values from the last video frame. 
		/// </summary>
		/// <remarks>
		/// The application must inspect the Safearray parameters to determine the dimensions. Note: This property should only be used from scripts due to the extremely high memory utilization on
		/// large image arrays (26 bytes per pixel). Pixels values should be in Short, int, or Double format.
		/// <para>The value will be only populated when the video frame has been obtained from the <see cref="P:ASCOM.DeviceInterface.IVideo.LastVideoFrameImageArrayVariant"/> property. When the video frame
		/// is obtained from the <see cref="P:ASCOM.DeviceInterface.IVideo.LastVideoFrame"/> property a NULL value must be returned. Do not throw an exception in this case.</para>
		/// <para>Color cameras will produce an array of  <see cref="P:ASCOM.DeviceInterface.IVideo.Width"/> * <see cref="P:ASCOM.DeviceInterface.IVideo.Height"/> * 3.
		/// The three planes are in the following order: R, G and B. If the application cannot handle color images, it should use just the first (R) plane.</para>
		/// <para>The pixels in the array start from the top left part of the image and are listed by horizontal lines/rows. The second pixels in the array is the second pixels from the first horizontal row
		/// and the second last pixel in the array is the second last pixels from the last horizontal row.</para>
		/// </remarks>
		/// <value>The image array variant.</value>
		object ImageArrayVariant { get; }

		/// <summary>
		/// Returns the frame number.
		/// </summary>
		/// <remarks>
		/// The frame number of the first exposed frame may not be zero and is dependent on the device and/or the driver. The frame number increases with each acquired frame not with each requested frame by the client.
		/// Must return -1 if frame numbering is not supported.
		/// </remarks>
		/// <value>The frame number of the current video frame.</value>
		long FrameNumber { get; }
		
		/// <summary>
		/// Returns the actual exposure duration in seconds (i.e. shutter open time).
		/// </summary>
		/// <remarks>
		/// This may differ from the exposure time corresponding to the requested frame exposure due to shutter latency, camera timing precision, etc.
		/// </remarks>
		/// <value>The duration of the frame exposure.</value>
		/// <exception cref="T:ASCOM.PropertyNotImplementedException">Must throw an exception if not supported</exception>
		double ExposureDuration { get; }

		/// <summary>
		/// Returns the actual exposure start time in the FITS-standard CCYY-MM-DDThh:mm:ss[.sss...] format, if supported.
		/// </summary>
		/// <value>The frame exposure start time.</value>
		/// <exception cref="T:ASCOM.PropertyNotImplementedException">Must throw an exception if not supported</exception>
		string ExposureStartTime { get; }

		/// <summary>
		/// Returns additional information associated with the video frame.
		/// </summary>
		/// <remarks><p style="color:red"><b>Must be implemented</b></p> This property must return an empty string if no additonal video frame information is supported. Please do not throw a 
		/// <see cref="T:ASCOM.PropertyNotImplementedException"/>.
		/// </remarks>
		/// <value>A string in a well known format agreed by interested parties that represents any additional information associated with the video frame. This could include for example
		/// any values embedded in the image by some video cameras. The values could include things like Gamma, Gain and Offset of the individual image. The most accurate timestamp and frame duration
		/// must be always returned by <see cref="P:ASCOM.DeviceInterface.IVideo.ExposureStartTime"/> and <see cref="P:ASCOM.DeviceInterface.IVideo.ExposureDuration"/> properties. Secondary timing reference
		/// can be included here if supported by the driver or camera.</value>
		string ImageInfo { get; }
	}


	/// <summary>
	/// Defines the IVideo Interface.
	/// </summary>
	public interface IVideo
	{
		/// <summary>
		/// Set True to connect to the device. Set False to disconnect from the device.
		/// You can also read the property to check whether it is connected.
		/// </summary>
		/// <value><c>true</c> if connected; otherwise, <c>false</c>.</value>
		/// <exception cref="T:ASCOM.DriverException">Must throw an exception if the call was not successful</exception>
		/// <remarks><p style="color:red"><b>Must be implemented</b></p>Do not use a NotConnectedException here, that exception is for use in other methods that require a connection in order to succeed.</remarks>
		bool Connected { get; set; }


		/// <summary>
		/// Returns a description of the device, such as manufacturer and model number. Any ASCII characters may be used. 
		/// </summary>
		/// <value>The description.</value>
		/// <exception cref="T:ASCOM.NotConnectedException">If the device is not connected and this information is only available when connected.</exception>
		/// <exception cref="T:ASCOM.DriverException">Must throw an exception if the call was not successful</exception>
		/// <remarks><p style="color:red"><b>Must be implemented</b></p>
		/// </remarks>
		string Description { get; }


		/// <summary>
		/// Descriptive and version information about this ASCOM driver.
		/// </summary>
		/// <exception cref="T:ASCOM.DriverException">Must throw an exception if the call was not successful</exception>
		/// <remarks>
		///	<p style="color:red"><b>Must be implemented</b></p> This string may contain line endings and may be hundreds to thousands of characters long.
		/// It is intended to display detailed information on the ASCOM driver, including version and copyright data.
		/// See the <see cref="P:ASCOM.DeviceInterface.IVideo.Description"/> property for information on the device itself.
		/// To get the driver version in a parseable string, use the <see cref="P:ASCOM.DeviceInterface.IVideo.DriverVersion"/> property.
		/// </remarks>
		string DriverInfo { get; }


		/// <summary>
		/// A string containing only the major and minor version of the driver.
		/// </summary>
		/// <exception cref="T:ASCOM.DriverException">Must throw an exception if the call was not successful</exception>
		/// <remarks><p style="color:red"><b>Must be implemented</b></p> This must be in the form "n.n".
		/// It should not to be confused with the <see cref="P:ASCOM.DeviceInterface.IVideo.InterfaceVersion"/> property, which is the version of this specification supported by the 
		/// driver.
		/// </remarks>
		string DriverVersion { get; }


		/// <summary>
		/// The interface version number that this device supports. Should return 2 for this interface version.
		/// </summary>
		/// <exception cref="T:ASCOM.DriverException">Must throw an exception if the call was not successful</exception>
		/// <remarks><p style="color:red"><b>Must be implemented</b></p> Clients can detect legacy V1 drivers by trying to read ths property.
		/// If the driver raises an error, it is a V1 driver. V1 did not specify this property. A driver may also return a value of 1. 
		/// In other words, a raised error or a return value of 1 indicates that the driver is a V1 driver.
		/// </remarks>
		short InterfaceVersion { get; }


		/// <summary>
		/// The short name of the driver, for display purposes.
		/// </summary>		
		/// <exception cref="T:ASCOM.DriverException">Must throw an exception if the call was not successful</exception>
		/// <remarks><p style="color:red"><b>Must be implemented</b></p>
		/// </remarks>
		string Name { get; }

		/// <summary>
		/// The name of the video device or the video capture device, when such a device is used. This is typically the name or model of the camera or video system. It can also be the name 
		/// of the capture card when a separate video capture device is used.
		/// <exception cref="T:ASCOM.NotConnectedException">If the device is not connected and this information is only available when connected.</exception>
		/// </summary>
		string VideoCaptureDeviceName { get; }

		/// <summary>
		/// Launches a configuration dialog box for the driver. The call will not return until the user clicks OK or cancel manually.
		/// </summary>
		/// <exception cref="T:ASCOM.DriverException">Must throw an exception if the call was not successful</exception>
		/// <remarks><p style="color:red"><b>Must be implemented</b></p>
		/// </remarks>
		void SetupDialog();


		/// <summary>
		/// Invokes the specified device-specific action.
		/// </summary>
		/// <param name="ActionName">
		/// A well known name agreed by interested parties that represents the action to be carried out. 
		/// </param>
		/// <param name="ActionParameters">List of required parameters or an <see cref="T:System.String">Empty String</see> if none are required.
		/// </param>
		///	<returns>A string response. The meaning of returned strings is set by the driver author.</returns>
		/// <exception cref="T:ASCOM.MethodNotImplementedException">Throws this exception if no actions are suported.</exception>
		/// <exception cref="T:ASCOM.ActionNotImplementedException">It is intended that the SupportedActions method will inform clients 
		/// of driver capabilities, but the driver must still throw an ASCOM.ActionNotImplemented exception if it is asked to 
		/// perform an action that it does not support.</exception>
		/// <exception cref="T:ASCOM.NotConnectedException">If the driver is not connected.</exception>
		/// <exception cref="T:ASCOM.DriverException">Must throw an exception if the call was not successful</exception>
		/// <example>Suppose a video driver offers an automatic motion detection; new actions could 
		/// be “Video:MotionStartFrameId” and “Video:MotionEndFrameId” returning for example frame numbers corresponding to the first and last frame 
		/// of the last detected motion. Any error information and exact return value formats are defined by the driver vendor.
		/// </example>
		/// <remarks><p style="color:red"><b>Can throw a not implemented exception</b></p>
		/// This method is intended for use in all current and future device types and to avoid name clashes, management of action names 
		/// is important from day 1. A two-part naming convention will be adopted - <b>DeviceType:UniqueActionName</b> where:
		/// <list type="bullet">
		///		<item><description>DeviceType is the same value as would be used by <see cref="P:ASCOM.Utilities.Chooser.DeviceType"/> e.g. Video in the case of a video camera.</description></item>
		///		<item><description>UniqueActionName is a single word, or multiple words joined by underscore characters, that sensibly describes the action to be performed.</description></item>
		///	</list>
		/// <para>
		/// It is recommended that UniqueActionNames should be a maximum of 16 characters for legibility.
		/// Should the same function and UniqueActionName be supported by more than one type of device, the reserved DeviceType of 
		/// “General” will be used. Action names will be case insensitive, so Video:MotionStartFrameId, video:motionstartframeid 
		/// and VIDEO:MOTIONSTARTFRAMEID will all refer to the same action.</para>
		///	<para>The names of all supported actions must be returned in the <see cref="P:ASCOM.DeviceInterface.IVideo.SupportedActions"/> property.</para>
		/// </remarks>
		string Action(string ActionName, string ActionParameters);


		/// <summary>
		/// Returns the list of action names supported by this driver.
		/// </summary>
		///	<value>An ArrayList of strings (SafeArray collection) containing the names of supported actions.</value>
		///	<exception cref="T:ASCOM.DriverException">Must throw an exception if the call was not successful</exception>
		/// <remarks><p style="color:red"><b>Must be implemented</b></p> This method must return an empty arraylist if no actions are supported. Please do not throw a 
		/// <see cref="T:ASCOM.PropertyNotImplementedException"/>.
		/// <para>This is an aid to client authors and testers who would otherwise have to repeatedly poll the driver to determine its capabilities. 
		/// Returned action names may be in mixed case to enhance presentation but  will be recognised case insensitively in 
		/// the <see cref="M:ASCOM.DeviceInterface.IVideo.Action(System.String,System.String)">Action</see> method.</para>
		/// <para>An array list collection has been selected as the vehicle for action names in order to make it easier for clients to
		/// determine whether a particular action is supported. This is easily done through the Contains method. Since the
		/// collection is also enumerable it is easy to use constructs such as For Each ... to operate on members without having to be concerned 
		/// about hom many members are in the collection. </para>
		///	<para>Collections have been used in the Telescope specification for a number of years and are known to be compatible with COM. Within .NET
		/// the ArrayList is the correct implementation to use as the .NET Generic methods are not compatible with COM.</para>
		/// </remarks>
		ArrayList SupportedActions { get; }

		/// <summary>
		/// Dispose the late-bound interface, if needed. Will release it via COM
		/// if it is a COM object, else if native .NET will just dereference it
		/// for GC.
		/// </summary>
		void Dispose();


		/// <summary>
		/// The maximum supported exposure (integration time) in seconds.
		/// </summary>
		/// <remarks>
		/// This value is for information purposes only. The exposure cannot be set directly in seconds, use <see cref="P:ASCOM.DeviceInterface.IVideo.IntegrationRate"/> method to change the exposure. 
		/// </remarks>
		double ExposureMax { get; }

		/// <summary>
		/// The minimum supported exposure (integration time) in seconds.
		/// </summary>
		/// <remarks>
		/// This value is for information purposes only. The exposure cannot be set directly in seconds, use <see cref="P:ASCOM.DeviceInterface.IVideo.IntegrationRate"/> method to change the exposure. 
		/// </remarks>
		double ExposureMin { get; }


		/// <summary>
		/// The frame reate at which the camera is running. 
		/// </summary>
		/// <remarks>
		/// Analogue cameras usually run in one of the two fixes frame rates - 25fps for PAL video and 29.97fps for NTSC video. Digital cameras usually can run on variable framerate. Some analogue cameras may allow Variable framerate.
		/// </remarks>
		VideoCameraFrameRate FrameRate { get; }


		/// <summary>
		/// Returns an ArrayList of strings (SafeArray collection) containing the integration rates supported by the video camera.
		/// </summary>
		/// <remarks>
		/// Digital and integrating analogue video cameras allow the effective exposure of a frame to be changed. If the camera supports setting the exposure directly i.e. 2.153 sec then the driver must only
		/// return a range of useful supported exposures. For many video cameras the supported exposures (integration rates) increase by a factor of 2 from a base exposure e.g. 1, 2, 4, 8, 16 frames or 0.04, 0.08, 0.16, 0.32, 0.64, 1.28, 2.56, 5.12, 10.24 sec.
		/// If the camers supports only one exposure that cannot be changed (such as all non integrating PAL or NTSC video cameras) then this property must throw <see cref="T:ASCOM.PropertyNotImplementedException"/>.
		/// </remarks>
		/// <value>The list of supported integration rates in seconds.</value>
		/// <exception cref="T:ASCOM.NotConnectedException">Must throw exception if data unavailable.</exception>
		/// <exception cref="T:ASCOM.PropertyNotImplementedException">Must throw exception if camera supports only one integration rate (exposure) that cannot be changed.</exception>		
		ArrayList SupportedIntegrationRates { get; }


		/// <summary>
		///	Index into the <see cref="P:ASCOM.DeviceInterface.IVideo.SupportedIntegrationRates"/> array for the selected camera integration rate
		///	</summary>
		///	<value>Integer index for the current camera integration rate in the <see cref="P:ASCOM.DeviceInterface.IVideo.SupportedIntegrationRates"/> string array.</value>
		///	<returns>Index into the SupportedIntegrationRates array for the selected camera integration rate</returns>
		///	<exception cref="T:ASCOM.NotConnectedException">Must throw an exception if the information is not available. (Some drivers may require an 
		///	active <see cref="P:ASCOM.DeviceInterface.IVideo.Connected">connection</see> in order to retrieve necessary information from the camera.)</exception>
		///	<exception cref="T:ASCOM.InvalidValueException">Must throw an exception if not valid.</exception>
		///	<exception cref="T:ASCOM.PropertyNotImplementedException">Must throw an exception if the camera supports only one integration rate (exposure) that cannot be changed.</exception>
		///	<remarks>
		///	<see cref="P:ASCOM.DeviceInterface.IVideo.IntegrationRate"/> can be used to adjust the integration rate (exposure) of the camera, if supported. A 0-based array of strings - <see cref="P:ASCOM.DeviceInterface.IVideo.SupportedIntegrationRates"/>, 
		/// which correspond to different discrete integration rate settings supported by the camera will be returned. <see cref="P:ASCOM.DeviceInterface.IVideo.IntegrationRate"/> must be set to an integer in this range.
		///	<para>The driver must default <see cref="P:ASCOM.DeviceInterface.IVideo.IntegrationRate"/> to a valid value when integration rate is supported by the camera. </para>
		///	</remarks>				
		int IntegrationRate { get; set; }


		/// <summary>
		/// Returns an <see cref="T:ASCOM.DeviceInterface.IVideoFrame"/> with its <see cref="P:ASCOM.DeviceInterface.IVideoFrame.ImageArray"/> property populated. 
		/// </summary>
		/// <remarks>
		/// The <see cref="P:ASCOM.DeviceInterface.IVideoFrame.ImageArrayVariant"/> property of the video frame will not be populated. Use the <see cref="P:ASCOM.DeviceInterface.IVideo.LastVideoFrameImageArrayVariant"/> property
		/// to obtain a video frame that has the <see cref="P:ASCOM.DeviceInterface.IVideoFrame.ImageArrayVariant"/> populated.
		/// </remarks>
		/// <value>The latest video frame.</value>
		/// <exception cref="T:ASCOM.NotConnectedException">Must throw exception if data unavailable.</exception>
		/// <exception cref="T:ASCOM.InvalidOperationException">If called before any video frame has been taken</exception>	
		IVideoFrame LastVideoFrame { get; }

		/// <summary>
		/// Returns an <see cref="T:ASCOM.DeviceInterface.IVideoFrame"/> with its <see cref="P:ASCOM.DeviceInterface.IVideoFrame.ImageArrayVariant"/> property populated. 
		/// </summary>
		/// <remarks>
		/// The <see cref="P:ASCOM.DeviceInterface.IVideoFrame.ImageArray"/> property of the video frame will not be populated. Use the <see cref="P:ASCOM.DeviceInterface.IVideo.LastVideoFrame"/> property
		/// to obtain a video frame that has the <see cref="P:ASCOM.DeviceInterface.IVideoFrame.ImageArray"/> populated.
		/// </remarks>
		/// <value>The latest video frame.</value>
		/// <exception cref="T:ASCOM.NotConnectedException">Must throw exception if data unavailable.</exception>
		/// <exception cref="T:ASCOM.InvalidOperationException">If called before any video frame has been taken</exception>			
		IVideoFrame LastVideoFrameImageArrayVariant { get; }


		/// <summary>
		/// Sensor name
		/// </summary>
		///	<returns>The name of sensor used within the camera</returns>
		///	<exception cref="T:ASCOM.NotConnectedException">Must throw an exception if the information is not available. (Some drivers may require an 
		///	active <see cref="P:ASCOM.DeviceInterface.IVideo.Connected">connection</see> in order to retrieve necessary information from the camera.)</exception>
		///	<remarks>Returns the name (datasheet part number) of the sensor, e.g. ICX285AL.  The format is to be exactly as shown on 
		///	manufacturer data sheet, subject to the following rules. All letter shall be uppercase.  Spaces shall not be included.
		///	<para>Any extra suffixes that define region codes, package types, temperature range, coatings, grading, color/monochrome, 
		///	etc. shall not be included. For color sensors, if a suffix differentiates different Bayer matrix encodings, it shall be 
		///	included.</para>
		///	<para>Examples:</para>
		///	<list type="bullet">
		///		<item><description>ICX285AL-F shall be reported as ICX285</description></item>
		///		<item><description>KAF-8300-AXC-CD-AA shall be reported as KAF-8300</description></item>
		///	</list>
		///	<para><b>Note:</b></para>
		///	<para>The most common usage of this property is to select approximate color balance parameters to be applied to 
		///	the Bayer matrix of one-shot color sensors.  Application authors should assume that an appropriate IR cutoff filter is 
		///	in place for color sensors.</para>
		///	<para>It is recommended that this function be called only after a <see cref="P:ASCOM.DeviceInterface.IVideo.Connected">connection</see> is established with 
		///	the camera hardware, to ensure that the driver is aware of the capabilities of the specific camera model.</para>
		/// Many analogue video cameras or their generic drivers may not know the sensor used in the camera and must throw a <exception cref="T:ASCOM.PropertyNotImplementedException"/> in such a case.
		///	</remarks>
		/// <exception cref="T:ASCOM.PropertyNotImplementedException">Must throw an exception if SensorName is not supported or unknown</exception>
		string SensorName { get; }


		/// <summary>
		///Type of colour information returned by the the camera sensor
		///</summary>
		///   <value></value>
		///   <returns>The <see cref="T:ASCOM.DeviceInterface.SensorType"/> enum value of the camera sensor</returns>
		///   <exception cref="T:ASCOM.NotConnectedException">Must throw an exception if the information is not available. (Some drivers may require an 
		///active <see cref="P:ASCOM.DeviceInterface.ICameraV2.Connected">connection</see> in order to retrieve necessary information from the camera.)</exception>
		///   <remarks>
		///       <para><see cref="P:ASCOM.DeviceInterface.ICameraV2.SensorType"/> returns a value indicating whether the sensor is monochrome, or what Bayer matrix it encodes.  
		///The following values are defined:</para>
		///       <para>
		///           <table style="width:76.24%;" cellspacing="0" width="76.24%">
		///               <col style="width: 11.701%;"></col>
		///               <col style="width: 20.708%;"></col>
		///               <col style="width: 67.591%;"></col>
		///               <tr>
		///                   <td colspan="1" rowspan="1" style="width: 11.701%; padding-right: 10px; padding-left: 10px; &#xA; border-left-color: #000000; border-left-style: Solid; &#xA; border-top-color: #000000; border-top-style: Solid; &#xA; border-right-color: #000000; border-right-style: Solid;&#xA; border-bottom-color: #000000; border-bottom-style: Solid; &#xA; border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; &#xA; background-color: #00ffff;" width="11.701%">
		///                       <b>Value</b></td>
		///                   <td colspan="1" rowspan="1" style="width: 20.708%; padding-right: 10px; padding-left: 10px; &#xA; border-top-color: #000000; border-top-style: Solid; &#xA; border-right-style: Solid; border-right-color: #000000; &#xA; border-bottom-color: #000000; border-bottom-style: Solid; &#xA; border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; &#xA; background-color: #00ffff;" width="20.708%">
		///                       <b>Enumeration</b></td>
		///                   <td colspan="1" rowspan="1" style="width: 67.591%; padding-right: 10px; padding-left: 10px; &#xA; border-top-color: #000000; border-top-style: Solid; &#xA; border-right-style: Solid; border-right-color: #000000; &#xA; border-bottom-color: #000000; border-bottom-style: Solid; &#xA; border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; &#xA; background-color: #00ffff;" width="67.591%">
		///                       <b>Meaning</b></td>
		///               </tr>
		///               <tr>
		///                   <td style="padding-right: 10px; padding-left: 10px; &#xA; border-left-color: #000000; border-left-style: Solid; &#xA; border-right-color: #000000; border-right-style: Solid; &#xA; border-bottom-color: #000000; border-bottom-style: Solid; &#xA; border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
		///0</td>
		///                   <td style="padding-right: 10px; padding-left: 10px; &#xA; border-right-color: #000000; border-right-style: Solid; &#xA; border-bottom-color: #000000; border-bottom-style: Solid; &#xA; border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
		///Monochrome</td>
		///                   <td style="padding-right: 10px; padding-left: 10px; &#xA; border-right-color: #000000; border-right-style: Solid; &#xA; border-bottom-color: #000000; border-bottom-style: Solid; &#xA; border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
		///Camera produces monochrome array with no Bayer encoding</td>
		///               </tr>
		///               <tr>
		///                   <td style="padding-right: 10px; padding-left: 10px; &#xA; border-left-color: #000000; border-left-style: Solid; &#xA; border-right-color: #000000; border-right-style: Solid; &#xA; border-bottom-color: #000000; border-bottom-style: Solid; &#xA; border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
		///1</td>
		///                   <td style="padding-right: 10px; padding-left: 10px; &#xA; border-right-color: #000000; border-right-style: Solid; &#xA; border-bottom-color: #000000; border-bottom-style: Solid; &#xA; border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
		///Colour</td>
		///                   <td style="padding-right: 10px; padding-left: 10px; &#xA; border-right-color: #000000; border-right-style: Solid; &#xA; border-bottom-color: #000000; border-bottom-style: Solid; &#xA; border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
		///Camera produces color image directly, requiring not Bayer decoding</td>
		///               </tr>
		///               <tr>
		///                   <td style="padding-right: 10px; padding-left: 10px; &#xA; border-left-color: #000000; border-left-style: Solid; &#xA; border-right-color: #000000; border-right-style: Solid; &#xA; border-bottom-color: #000000; border-bottom-style: Solid; &#xA; border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
		///2</td>
		///                   <td style="padding-right: 10px; padding-left: 10px; &#xA; border-right-color: #000000; border-right-style: Solid; &#xA; border-bottom-color: #000000; border-bottom-style: Solid; &#xA; border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
		///RGGB</td>
		///                   <td style="padding-right: 10px; padding-left: 10px; &#xA; border-right-color: #000000; border-right-style: Solid; &#xA; border-bottom-color: #000000; border-bottom-style: Solid; &#xA; border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
		///Camera produces RGGB encoded Bayer array images</td>
		///               </tr>
		///               <tr>
		///                   <td style="padding-right: 10px; padding-left: 10px; &#xA; border-left-color: #000000; border-left-style: Solid; &#xA; border-right-color: #000000; border-right-style: Solid; &#xA; border-bottom-color: #000000; border-bottom-style: Solid; &#xA; border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
		///3</td>
		///                   <td style="padding-right: 10px; padding-left: 10px; &#xA; border-right-color: #000000; border-right-style: Solid; &#xA; border-bottom-color: #000000; border-bottom-style: Solid; &#xA; border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
		///CMYG</td>
		///                   <td style="padding-right: 10px; padding-left: 10px; &#xA; border-right-color: #000000; border-right-style: Solid; &#xA; border-bottom-color: #000000; border-bottom-style: Solid; &#xA; border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
		///Camera produces CMYG encoded Bayer array images</td>
		///               </tr>
		///               <tr>
		///                   <td style="padding-right: 10px; padding-left: 10px; &#xA; border-left-color: #000000; border-left-style: Solid; &#xA; border-right-color: #000000; border-right-style: Solid; &#xA; border-bottom-color: #000000; border-bottom-style: Solid; &#xA; border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
		///4</td>
		///                   <td style="padding-right: 10px; padding-left: 10px; &#xA; border-right-color: #000000; border-right-style: Solid; &#xA; border-bottom-color: #000000; border-bottom-style: Solid; &#xA; border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
		///CMYG2</td>
		///                   <td style="padding-right: 10px; padding-left: 10px; &#xA; border-right-color: #000000; border-right-style: Solid; &#xA; border-bottom-color: #000000; border-bottom-style: Solid; &#xA; border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
		///Camera produces CMYG2 encoded Bayer array images</td>
		///               </tr>
		///               <tr>
		///                   <td style="padding-right: 10px; padding-left: 10px; &#xA; border-left-color: #000000; border-left-style: Solid; &#xA; border-right-color: #000000; border-right-style: Solid; &#xA; border-bottom-color: #000000; border-bottom-style: Solid; &#xA; border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
		///5</td>
		///                   <td style="padding-right: 10px; padding-left: 10px; &#xA; border-right-color: #000000; border-right-style: Solid; &#xA; border-bottom-color: #000000; border-bottom-style: Solid; &#xA; border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
		///LRGB</td>
		///                   <td style="padding-right: 10px; padding-left: 10px; &#xA; border-right-color: #000000; border-right-style: Solid; &#xA; border-bottom-color: #000000; border-bottom-style: Solid; &#xA; border-right-width: 1px; border-left-width: 1px; border-top-width: 1px; border-bottom-width: 1px; ">
		///Camera produces Kodak TRUESENSE Bayer LRGB array images</td>
		///               </tr>
		///           </table>
		///       </para>
		///       <para>Please note that additional values may be defined in future updates of the standard, as new Bayer matrices may be created 
		///by sensor manufacturers in the future.  If this occurs, then a new enumeration value shall be defined. The pre-existing enumeration 
		///values shall not change.
		///<para><see cref="P:ASCOM.DeviceInterface.ICameraV2.SensorType"/> can possibly change between exposures, for example if <see cref="P:ASCOM.DeviceInterface.ICameraV2.ReadoutMode">Camera.ReadoutMode</see> is changed, and should always be checked after each exposure.</para>
		///           <para>In the following definitions, R = red, G = green, B = blue, C = cyan, M = magenta, Y = yellow.  The Bayer matrix is 
		///defined with X increasing from left to right, and Y increasing from top to bottom. The pattern repeats every N x M pixels for the 
		///entire pixel array, where N is the height of the Bayer matrix, and M is the width.</para>
		///           <para>RGGB indicates the following matrix:</para>
		///       </para>
		///       <para>
		///           <table style="width:41.254%;" cellspacing="0" width="41.254%">
		///               <col style="width: 10%;"></col>
		///               <col style="width: 10%;"></col>
		///               <col style="width: 10%;"></col>
		///               <tr valign="top" align="center">
		///                   <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; background-color: #ffffff" width="10%">
		///                   </td>
		///                   <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px;&#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; background-color: #00ffff;" width="10%">
		///                       <b>X = 0</b></td>
		///                   <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; border-right-color: #000000; border-right-style: Solid; border-right-width: 1px; &#xA; background-color: #00ffff;" width="10%">
		///                       <b>X = 1</b></td>
		///               </tr>
		///               <tr valign="top" align="center">
		///                   <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; background-color: #00ffff" width="10%">
		///                       <b>Y = 0</b></td>
		///                   <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; " width="10%">
		///R</td>
		///                   <td colspan="1" rowspan="1" style="width:10%; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; border-top-color: #000000; border-top-style: Solid;  border-top-width: 1px; &#xA; border-right-color: #000000; border-right-style: Solid; border-right-width: 1px; &#xA; " width="10%">
		///G</td>
		///               </tr>
		///               <tr valign="top" align="center">
		///                   <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; border-bottom-color: #000000; border-bottom-style: Solid; border-bottom=width: 1px;&#xA; background-color: #00ffff;" width="10%">
		///                       <b>Y = 1</b></td>
		///                   <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; border-bottom-color: #000000; border-bottom-style: Solid; border-bottom=width: 1px;&#xA; " width="10%">
		///G</td>
		///                   <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; border-bottom-color: #000000; border-bottom-style: Solid; border-bottom=width: 1px;&#xA; border-right-color: #000000; border-right-style: Solid; border-right-width: 1px; &#xA; " width="10%">
		///B</td>
		///               </tr>
		///           </table>
		///       </para>
		///       <para>CMYG indicates the following matrix:</para>
		///       <para>
		///           <table style="width:41.254%;" cellspacing="0" width="41.254%">
		///               <col style="width: 10%;"></col>
		///               <col style="width: 10%;"></col>
		///               <col style="width: 10%;"></col>
		///               <tr valign="top" align="center">
		///                   <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; background-color: #ffffff" width="10%">
		///                   </td>
		///                   <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px;&#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; background-color: #00ffff;" width="10%">
		///                       <b>X = 0</b></td>
		///                   <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; border-right-color: #000000; border-right-style: Solid; border-right-width: 1px; &#xA; background-color: #00ffff;" width="10%">
		///                       <b>X = 1</b></td>
		///               </tr>
		///               <tr valign="top" align="center">
		///                   <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; background-color: #00ffff" width="10%">
		///                       <b>Y = 0</b></td>
		///                   <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; " width="10%">
		///Y</td>
		///                   <td colspan="1" rowspan="1" style="width:10%; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; border-top-color: #000000; border-top-style: Solid;  border-top-width: 1px; &#xA; border-right-color: #000000; border-right-style: Solid; border-right-width: 1px; &#xA; " width="10%">
		///C</td>
		///               </tr>
		///               <tr valign="top" align="center">
		///                   <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; border-bottom-color: #000000; border-bottom-style: Solid; border-bottom=width: 1px;&#xA; background-color: #00ffff;" width="10%">
		///                       <b>Y = 1</b></td>
		///                   <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; border-bottom-color: #000000; border-bottom-style: Solid; border-bottom=width: 1px;&#xA; " width="10%">
		///G</td>
		///                   <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; border-bottom-color: #000000; border-bottom-style: Solid; border-bottom=width: 1px;&#xA; border-right-color: #000000; border-right-style: Solid; border-right-width: 1px; &#xA; " width="10%">
		///M</td>
		///               </tr>
		///           </table>
		///       </para>
		///       <para>CMYG2 indicates the following matrix:</para>
		///       <para>
		///           <table style="width:41.254%;" cellspacing="0" width="41.254%">
		///               <col style="width: 10%;"></col>
		///               <col style="width: 10%;"></col>
		///               <col style="width: 10%;"></col>
		///               <tr valign="top" align="center">
		///                   <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; background-color: #ffffff" width="10%">
		///                   </td>
		///                   <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px;&#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; background-color: #00ffff;" width="10%">
		///                       <b>X = 0</b></td>
		///                   <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; border-right-color: #000000; border-right-style: Solid; border-right-width: 1px; &#xA; background-color: #00ffff;" width="10%">
		///                       <b>X = 1</b></td>
		///               </tr>
		///               <tr valign="top" align="center">
		///                   <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; background-color: #00ffff" width="10%">
		///                       <b>Y = 0</b></td>
		///                   <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; " width="10%">
		///C</td>
		///                   <td colspan="1" rowspan="1" style="width:10%; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; border-top-color: #000000; border-top-style: Solid;  border-top-width: 1px; &#xA; border-right-color: #000000; border-right-style: Solid; border-right-width: 1px; &#xA; " width="10%">
		///Y</td>
		///               </tr>
		///               <tr valign="top" align="center">
		///                   <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; background-color: #00ffff;" width="10%">
		///                       <b>Y = 1</b></td>
		///                   <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; " width="10%">
		///M</td>
		///                   <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; border-right-color: #000000; border-right-style: Solid; border-right-width: 1px; &#xA; " width="10%">
		///G</td>
		///               </tr>
		///               <tr valign="top" align="center">
		///                   <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; background-color: #00ffff" width="10%">
		///                       <b>Y = 2</b></td>
		///                   <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; " width="10%">
		///C</td>
		///                   <td colspan="1" rowspan="1" style="width:10%; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; border-top-color: #000000; border-top-style: Solid;  border-top-width: 1px; &#xA; border-right-color: #000000; border-right-style: Solid; border-right-width: 1px; &#xA; " width="10%">
		///Y</td>
		///               </tr>
		///               <tr valign="top" align="center">
		///                   <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; border-bottom-color: #000000; border-bottom-style: Solid; border-bottom=width: 1px;&#xA; background-color: #00ffff;" width="10%">
		///                       <b>Y = 3</b></td>
		///                   <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; border-bottom-color: #000000; border-bottom-style: Solid; border-bottom=width: 1px;&#xA; " width="10%">
		///G</td>
		///                   <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; border-right-color: #000000; border-right-style: Solid; border-right-width: 1px; &#xA; border-bottom-color: #000000; border-bottom-style: Solid; border-bottom=width: 1px;&#xA; " width="10%">
		///M</td>
		///               </tr>
		///           </table>
		///       </para>
		///       <para>LRGB indicates the following matrix (Kodak TRUESENSE):</para>
		///       <para>
		///           <table style="width:68.757%;" cellspacing="0" width="68.757%">
		///               <col style="width: 10%;"></col>
		///               <col style="width: 10%;"></col>
		///               <col style="width: 10%;"></col>
		///               <col style="width: 10%;"></col>
		///               <col style="width: 10%;"></col>
		///               <tr valign="top" align="center">
		///                   <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; background-color: #ffffff" width="10%">
		///                   </td>
		///                   <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px;&#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; background-color: #00ffff;" width="10%">
		///                       <b>X = 0</b></td>
		///                   <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; background-color: #00ffff;" width="10%">
		///                       <b>X = 1</b></td>
		///                   <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; background-color: #00ffff;" width="10%">
		///                       <b>X = 2</b></td>
		///                   <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; border-right-color: #000000; border-right-style: Solid; border-right-width: 1px; &#xA; background-color: #00ffff;" width="10%">
		///                       <b>X = 3</b></td>
		///               </tr>
		///               <tr valign="top" align="center">
		///                   <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; background-color: #00ffff" width="10%">
		///                       <b>Y = 0</b></td>
		///                   <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; " width="10%">
		///L</td>
		///                   <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; " width="10%">
		///R</td>
		///                   <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; " width="10%">
		///L</td>
		///                   <td colspan="1" rowspan="1" style="width:10%; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; border-top-color: #000000; border-top-style: Solid;  border-top-width: 1px; &#xA; border-right-color: #000000; border-right-style: Solid; border-right-width: 1px; &#xA; " width="10%">
		///G</td>
		///               </tr>
		///               <tr valign="top" align="center">
		///                   <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; background-color: #00ffff;" width="10%">
		///                       <b>Y = 1</b></td>
		///                   <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; " width="10%">
		///R</td>
		///                   <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; " width="10%">
		///L</td>
		///                   <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; " width="10%">
		///G</td>
		///                   <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; border-right-color: #000000; border-right-style: Solid; border-right-width: 1px; &#xA; " width="10%">
		///L</td>
		///               </tr>
		///               <tr valign="top" align="center">
		///                   <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; background-color: #00ffff" width="10%">
		///                       <b>Y = 2</b></td>
		///                   <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; " width="10%">
		///L</td>
		///                   <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; " width="10%">
		///G</td>
		///                   <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; " width="10%">
		///L</td>
		///                   <td colspan="1" rowspan="1" style="width:10%; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; border-top-color: #000000; border-top-style: Solid;  border-top-width: 1px; &#xA; border-right-color: #000000; border-right-style: Solid; border-right-width: 1px; &#xA; " width="10%">
		///B</td>
		///               </tr>
		///               <tr valign="top" align="center">
		///                   <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; border-bottom-color: #000000; border-bottom-style: Solid; border-bottom=width: 1px;&#xA; background-color: #00ffff;" width="10%">
		///                       <b>Y = 3</b></td>
		///                   <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; border-bottom-color: #000000; border-bottom-style: Solid; border-bottom=width: 1px;&#xA; " width="10%">
		///G</td>
		///                   <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; border-bottom-color: #000000; border-bottom-style: Solid; border-bottom=width: 1px;&#xA; " width="10%">
		///L</td>
		///                   <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; border-bottom-color: #000000; border-bottom-style: Solid; border-bottom=width: 1px;&#xA; " width="10%">
		///B</td>
		///                   <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; border-right-color: #000000; border-right-style: Solid; border-right-width: 1px; &#xA; border-bottom-color: #000000; border-bottom-style: Solid; border-bottom=width: 1px;&#xA; " width="10%">
		///L</td>
		///               </tr>
		///           </table>
		///       </para>
		///       <para>The alignment of the array may be modified by <see cref="P:ASCOM.DeviceInterface.ICameraV2.BayerOffsetX"/> and <see cref="P:ASCOM.DeviceInterface.ICameraV2.BayerOffsetY"/>. 
		///The offset is measured from the 0,0 position in the sensor array to the upper left corner of the Bayer matrix table. 
		///Please note that the Bayer offset values are not affected by subframe settings.</para>
		///       <para>For example, if a CMYG2 sensor has a Bayer matrix offset as shown below, <see cref="P:ASCOM.DeviceInterface.ICameraV2.BayerOffsetX"/> is 0 and <see cref="P:ASCOM.DeviceInterface.ICameraV2.BayerOffsetY"/> is 1:</para>
		///       <para>
		///           <table style="width:41.254%;" cellspacing="0" width="41.254%">
		///               <col style="width: 10%;"></col>
		///               <col style="width: 10%;"></col>
		///               <col style="width: 10%;"></col>
		///               <tr valign="top" align="center">
		///                   <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; background-color: #ffffff" width="10%">
		///                   </td>
		///                   <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px;&#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; background-color: #00ffff;" width="10%">
		///                       <b>X = 0</b></td>
		///                   <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; border-right-color: #000000; border-right-style: Solid; border-right-width: 1px; &#xA; background-color: #00ffff;" width="10%">
		///                       <b>X = 1</b></td>
		///               </tr>
		///               <tr valign="top" align="center">
		///                   <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; background-color: #00ffff" width="10%">
		///                       <b>Y = 0</b></td>
		///                   <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; " width="10%">
		///G</td>
		///                   <td colspan="1" rowspan="1" style="width:10%; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; border-top-color: #000000; border-top-style: Solid;  border-top-width: 1px; &#xA; border-right-color: #000000; border-right-style: Solid; border-right-width: 1px; &#xA; " width="10%">
		///M</td>
		///               </tr>
		///               <tr valign="top" align="center">
		///                   <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; background-color: #00ffff;" width="10%">
		///                       <b>Y = 1</b></td>
		///                   <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; " width="10%">
		///C</td>
		///                   <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; border-right-color: #000000; border-right-style: Solid; border-right-width: 1px; &#xA; " width="10%">
		///Y</td>
		///               </tr>
		///               <tr valign="top" align="center">
		///                   <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; background-color: #00ffff" width="10%">
		///                       <b>Y = 2</b></td>
		///                   <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; " width="10%">
		///M</td>
		///                   <td colspan="1" rowspan="1" style="width:10%; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; border-top-color: #000000; border-top-style: Solid;  border-top-width: 1px; &#xA; border-right-color: #000000; border-right-style: Solid; border-right-width: 1px; &#xA; " width="10%">
		///G</td>
		///               </tr>
		///               <tr valign="top" align="center">
		///                   <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; border-bottom-color: #000000; border-bottom-style: Solid; border-bottom=width: 1px;&#xA; background-color: #00ffff;" width="10%">
		///                       <b>Y = 3</b></td>
		///                   <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; border-bottom-color: #000000; border-bottom-style: Solid; border-bottom=width: 1px;&#xA; " width="10%">
		///C</td>
		///                   <td colspan="1" rowspan="1" style="width:10%; &#xA; border-top-color: #000000; border-top-style: Solid; border-top-width: 1px; &#xA; border-left-color: #000000; border-left-style: Solid; border-left-width: 1px; &#xA; border-right-color: #000000; border-right-style: Solid; border-right-width: 1px; &#xA; border-bottom-color: #000000; border-bottom-style: Solid; border-bottom=width: 1px;&#xA; " width="10%">
		///Y</td>
		///               </tr>
		///           </table>
		///       </para>
		///       <para>It is recommended that this function be called only after a <see cref="P:ASCOM.DeviceInterface.ICameraV2.Connected">connection</see> is established with the video hardware, to ensure that 
		///the driver is aware of the capabilities of the specific camera model.</para>
		///   </remarks>
		SensorType SensorType { get; }


		/// <summary>
		///	Returns the width of the video camera CCD chip in unbinned pixels.
		///	</summary>
		///	<value>The size of the camera X.</value>
		///	<exception cref="T:ASCOM.NotConnectedException">Must throw exception if the value is not known</exception>
		/// <exception cref="T:ASCOM.PropertyNotImplementedException">Must throw an exception if CameraXSize is not supported or unknown</exception>
		int CameraXSize { get; }

		/// <summary>
		///	Returns the height of the video camera CCD chip in unbinned pixels.
		///	</summary>
		///	<value>The size of the camera Y.</value>
		///	<exception cref="T:ASCOM.NotConnectedException">Must throw exception if the value is not known</exception>
		/// <exception cref="T:ASCOM.PropertyNotImplementedException">Must throw an exception if CameraYSize is not supported or unknown</exception>
		int CameraYSize { get; }

		/// <summary>
		///	Returns the width of the video frame in pixels.
		///	</summary>
		///	<value>The video frame width.</value>
		///	<exception cref="T:ASCOM.NotConnectedException">Must throw exception if the value is not known</exception>
		/// <remarks>
		/// For analogue video cameras working via a frame grabber the dimensions of the video frames may be different than the dimension of the CCD chip.
		/// </remarks>
		int Width { get; }

		/// <summary>
		///	Returns the height of the video frame in pixels.
		///	</summary>
		///	<value>The video frame height.</value>
		///	<exception cref="T:ASCOM.NotConnectedException">Must throw exception if the value is not known</exception>
		/// <remarks>
		/// For analogue video cameras working via a frame grabber the dimensions of the video frames may be different than the dimension of the CCD chip.
		/// </remarks>
		int Height { get; }

		/// <summary>
		///	Returns the width of the CCD chip pixels in microns.
		///	</summary>
		///	<value>The pixel size X.</value>
		///	<exception cref="T:ASCOM.NotConnectedException">Must throw exception if data unavailable.</exception>
		/// <exception cref="T:ASCOM.PropertyNotImplementedException">Must throw an exception if CameraYSize is not supported or unknown.</exception>
		double PixelSizeX { get; }

		/// <summary>
		///	Returns the height of the CCD chip pixels in microns.
		///	</summary>
		///	<value>The pixel size Y.</value>
		///	<exception cref="T:ASCOM.NotConnectedException">Must throw exception if data unavailable.</exception>
		/// <exception cref="T:ASCOM.PropertyNotImplementedException">Must throw an exception if CameraYSize is not supported or unknown.</exception>
		double PixelSizeY { get; }

		/// <summary>
		///	Reports the bit depth the camera can produce.
		///	</summary>
		///	<value>The bit depth per pixel.</value>
		/// <remarks>
		/// The allowed values are between 1 and 16 inclusive, but typically the bit depth is one of: 8, 12, 14 or 16 bpp.
		/// </remarks>
		///	<exception cref="T:ASCOM.NotConnectedException">Must throw exception if data unavailable.</exception>
		int BitDepth { get; }

		/// <summary>
		/// Returns the video codec used to record the video file, e.g. XVID, DVSD, YUY2, HFYU etc. For AVI files this is usually the FourCC identifier of the codec. If no codec is used an empty string must be returned.
		/// when a custom file format is used then this property can be used to specify a format that is not necessarily a video codec.
		/// </summary>
		string VideoCodec { get;  }

		/// <summary>
		/// Returns the file format of the recorded video file, e.g. AVI, MPEG, ADV etc.
		/// </summary>
		string VideoFileFormat { get; }


		/// <summary>
		///	The size of the video frame buffer. 
		///	</summary>
		///	<value>The size of the video frame buffer. </value>
		///	<remarks><p style="color:red"><b>Must be implemented</b></p> When retrieving video frames using the <see cref="P:ASCOM.DeviceInterface.IVideo.LastVideoFrame" /> and 
		/// <see cref="P:ASCOM.DeviceInterface.IVideo.LastVideoFrameImageArrayVariant" /> properties the driver may use a buffer to queue the frames waiting to be read by 
		/// the client. This property returns the size of the buffer in frames or if no buffering is supported then the value of less than 2 should be returned. The size 
		/// of the buffer can be controlled by the end user from the driver setup dialog. 
		///	</remarks>
		int VideoFramesBufferSize { get; }

		/// <summary>
		/// Starts recording a new video file.
		/// </summary>
		/// <param name="PreferredFileName">The file name requested by the client. Some systems may not allow the file name to be controlled directly and they should ignore this parameter.</param>
		/// <returns>The actual file name that is being recorded.</returns>
		///	<exception cref="T:ASCOM.NotConnectedException">Must throw exception if not connected.</exception>
		///	<exception cref="T:ASCOM.InvalidOperationException">Must throw exception if the current camera state doesn't allow to begin recording a file.</exception>
		///	<exception cref="T:ASCOM.DriverException">Must throw exception if there is any other problem as a result of which the recording cannot begin.</exception>
		string StartRecordingVideoFile(string PreferredFileName);


		/// <summary>
		/// Stops the recording of a video file.
		/// </summary>
		///	<exception cref="T:ASCOM.NotConnectedException">Must throw exception if not connected.</exception>
		///	<exception cref="T:ASCOM.InvalidOperationException">Must throw exception if the current camera state doesn't allow to stop recording the file or no file is currently being recorded.</exception>
		///	<exception cref="T:ASCOM.DriverException">Must throw exception if there is any other problem as result of which the recording cannot stop.</exception>
		void StopRecordingVideoFile();


		/// <summary>
		///	Returns the current driver operational state
		///	</summary>
		///	<remarks>
		///	Returns one of the following status information:
		///	<list type="bullet">
		///		<listheader><description>Value  State           Meaning</description></listheader>
		///		<item><description>0      CameraIdle      At idle state, camera is available for commands</description></item>
		///		<item><description>1      CameraRunning	  The camera is running and video frames are available for viewing and recording</description></item>
		///		<item><description>2      CameraRecording The camera is running and recording a video</description></item>
		///		<item><description>3      CameraError     Camera error condition serious enough to prevent further operations (connection fail, etc.).</description></item>
		///	</list>
		///	</remarks>
		///	<value>The state of the camera.</value>
		///	<exception cref="T:ASCOM.NotConnectedException">Must return an exception if the camera status is unavailable.</exception>
		VideoCameraState CameraState { get; }


		/// <summary>
		///	Maximum value of <see cref="P:ASCOM.DeviceInterface.IVideo.Gain"/>
		///	</summary>
		///	<value>Short integer representing the maximum gain value supported by the camera.</value>
		///	<returns>The maximum gain value that this camera supports</returns>
		///	<exception cref="T:ASCOM.NotConnectedException">Must throw an exception if the information is not available. (Some drivers may require an 
		///	active <see cref="P:ASCOM.DeviceInterface.IVideo.Connected">connection</see> in order to retrieve necessary information from the camera.)</exception>
		///	<exception cref="T:ASCOM.PropertyNotImplementedException">Must throw an exception if <see cref="P:ASCOM.DeviceInterface.IVideo.GainMax"/> is not supported</exception>
		///	<remarks>When specifying the gain setting with an integer value, <see cref="P:ASCOM.DeviceInterface.IVideo.GainMax"/> is used in conjunction with <see cref="P:ASCOM.DeviceInterface.IVideo.GainMin"/> to 
		///	specify the range of valid settings.
		///	<para><see cref="P:ASCOM.DeviceInterface.IVideo.GainMax"/> shall be greater than <see cref="P:ASCOM.DeviceInterface.IVideo.GainMin"/>. If either is available, then both must be available.</para>
		///	<para>Please see <see cref="P:ASCOM.DeviceInterface.IVideo.Gain"/> for more information.</para>
		///	<para>It is recommended that this function be called only after a <see cref="P:ASCOM.DeviceInterface.IVideo.Connected">connection</see> is established with the camera hardware, to ensure 
		///	that the driver is aware of the capabilities of the specific camera model.</para>
		///	</remarks>
		short GainMax { get; }

		/// <summary>
		///	Minimum value of <see cref="P:ASCOM.DeviceInterface.IVideo.Gain"/>
		///	</summary>
		///	<returns>The minimum gain value that this camera supports</returns>
		///	<exception cref="T:ASCOM.NotConnectedException">Must throw an exception if the information is not available. (Some drivers may require an 
		///	active <see cref="P:ASCOM.DeviceInterface.IVideo.Connected">connection</see> in order to retrieve necessary information from the camera.)</exception>
		///	<exception cref="T:ASCOM.PropertyNotImplementedException">Must throw an exception if <see cref="P:ASCOM.DeviceInterface.IVideo.GainMin"/> is not supported</exception>
		///	<remarks>When specifying the gain setting with an integer value, <see cref="P:ASCOM.DeviceInterface.IVideo.GainMin"/> is used in conjunction with <see cref="P:ASCOM.DeviceInterface.IVideo.GainMax"/> to 
		///	specify the range of valid settings.
		///	<para><see cref="P:ASCOM.DeviceInterface.IVideo.GainMax"/> shall be greater than <see cref="P:ASCOM.DeviceInterface.IVideo.GainMin"/>. If either is available, then both must be available.</para>
		///	<para>Please see <see cref="P:ASCOM.DeviceInterface.IVideo.Gain"/> for more information.</para>
		///	<para>It is recommended that this function be called only after a <see cref="P:ASCOM.DeviceInterface.IVideo.Connected">connection</see> is established with the camera hardware, to ensure 
		///	that the driver is aware of the capabilities of the specific camera model.</para>
		///	</remarks>
		short GainMin { get; }

		/// <summary>
		///	Index into the <see cref="P:ASCOM.DeviceInterface.IVideo.Gains"/> array for the selected camera gain
		///	</summary>
		///	<value>Short integer index for the current camera gain in the <see cref="P:ASCOM.DeviceInterface.IVideo.Gains"/> string array.</value>
		///	<returns>Index into the Gains array for the selected camera gain</returns>
		///	<exception cref="T:ASCOM.NotConnectedException">Must throw an exception if the information is not available. (Some drivers may require an 
		///	active <see cref="P:ASCOM.DeviceInterface.IVideo.Connected">connection</see> in order to retrieve necessary information from the camera.)</exception>
		///	<exception cref="T:ASCOM.InvalidValueException">Must throw an exception if not valid.</exception>
		///	<exception cref="T:ASCOM.PropertyNotImplementedException">Must throw an exception if <see cref="P:ASCOM.DeviceInterface.IVideo.Gain"/> is not supported</exception>
		///	<remarks>
		///	<see cref="P:ASCOM.DeviceInterface.IVideo.Gain"/> can be used to adjust the gain setting of the camera, if supported. There are two typical usage scenarios:
		///	<ul>
		///		<li>Discrete gain: video cameras will return a 0-based array of strings - <see cref="P:ASCOM.DeviceInterface.IVideo.Gains"/>, which correspond to different discrete gain settings supported by the camera. <see cref="P:ASCOM.DeviceInterface.IVideo.Gain"/> must be set to an integer in this range. <see cref="P:ASCOM.DeviceInterface.IVideo.GainMin"/> and <see cref="P:ASCOM.DeviceInterface.IVideo.GainMax"/> must thrown an exception if 
		///	this mode is used.</li>
		///		<li>Adjustable gain: video cameras return the <see cref="P:ASCOM.DeviceInterface.IVideo.GainMin"/> and <see cref="P:ASCOM.DeviceInterface.IVideo.GainMax"/> values, which specify the valid range for <see cref="P:ASCOM.DeviceInterface.IVideo.GainMin"/> and <see cref="P:ASCOM.DeviceInterface.IVideo.Gain"/>.</li>
		///	</ul>
		///	<para>The driver must default <see cref="P:ASCOM.DeviceInterface.IVideo.Gain"/> to a valid value. </para>
		///	</remarks>
		short Gain { get; set; }



		/// <summary>
		/// Gains supported by the camera
		///	</summary>
		///	<returns>An ArrayList of strings of supported gain names or values</returns>
		///	<exception cref="T:ASCOM.NotConnectedException">Must throw an exception if the information is not available. (Some drivers may require an 
		///	active <see cref="P:ASCOM.DeviceInterface.IVideo.Connected">connection</see> in order to retrieve necessary information from the camera.)</exception>
		///	<exception cref="T:ASCOM.PropertyNotImplementedException">Must throw an exception if <see cref="P:ASCOM.DeviceInterface.IVideo.Gains"/>  is not supported</exception>
		///	<remarks><see cref="P:ASCOM.DeviceInterface.IVideo.Gains"/> provides a 0-based array of available gain settings.  This is often used to specify ISO settings for DSLR cameras.  
		///	Typically the application software will display the available gain settings in a drop list. The application will then supply 
		///	the selected index to the driver via the <see cref="P:ASCOM.DeviceInterface.IVideo.Gain"/> property. 
		///	<para>The <see cref="P:ASCOM.DeviceInterface.IVideo.Gain"/> setting may alternatively be specified using integer values; if this mode is used then <see cref="P:ASCOM.DeviceInterface.IVideo.Gains"/> is invalid 
		///	and must throw an exception. Please see <see cref="P:ASCOM.DeviceInterface.IVideo.GainMax"/> and <see cref="P:ASCOM.DeviceInterface.IVideo.GainMin"/> for more information.</para>
		///	<para>It is recommended that this function be called only after a <see cref="P:ASCOM.DeviceInterface.IVideo.Connected">connection</see> is established with the camera hardware, 
		///	to ensure that the driver is aware of the capabilities of the specific camera model.</para>
		///	</remarks>
		ArrayList Gains { get; }


		/// <summary>
		///	Index into the <see cref="P:ASCOM.DeviceInterface.IVideo.Gammas"/> array for the selected camera gamma
		///	</summary>
		///	<value>Integer index for the current camera gamma in the <see cref="P:ASCOM.DeviceInterface.IVideo.Gammas"/> string array.</value>
		///	<returns>Index into the Gammas array for the selected camera gamma</returns>
		///	<exception cref="T:ASCOM.NotConnectedException">Must throw an exception if the information is not available. (Some drivers may require an 
		///	active <see cref="P:ASCOM.DeviceInterface.IVideo.Connected">connection</see> in order to retrieve necessary information from the camera.)</exception>
		///	<exception cref="T:ASCOM.InvalidValueException">Must throw an exception if not valid.</exception>
		///	<exception cref="T:ASCOM.PropertyNotImplementedException">Must throw an exception if <see cref="P:ASCOM.DeviceInterface.IVideo.Gamma"/> is not supported</exception>
		///	<remarks>
		///	<see cref="P:ASCOM.DeviceInterface.IVideo.Gamma"/> can be used to adjust the gamma setting of the camera, if supported. A 0-based array of strings - <see cref="P:ASCOM.DeviceInterface.IVideo.Gammas"/>, 
		/// which correspond to different discrete gamma settings supported by the camera will be returned. <see cref="P:ASCOM.DeviceInterface.IVideo.Gamma"/> must be set to an integer in this range.
		///	<para>The driver must default <see cref="P:ASCOM.DeviceInterface.IVideo.Gamma"/> to a valid value. </para>
		///	</remarks>
		int Gamma { get; set; }


		/// <summary>
		/// Gamma values supported by the camera
		///	</summary>
		///	<returns>An ArrayList of strings of gamma names or values</returns>
		///	<exception cref="T:ASCOM.NotConnectedException">Must throw an exception if the information is not available. (Some drivers may require an 
		///	active <see cref="P:ASCOM.DeviceInterface.IVideo.Connected">connection</see> in order to retrieve necessary information from the camera.)</exception>
		///	<exception cref="T:ASCOM.PropertyNotImplementedException">Must throw an exception if gainmin is not supported</exception>
		///	<remarks><see cref="P:ASCOM.DeviceInterface.IVideo.Gammas"/> provides a 0-based array of available gamma settings.
		///	Typically the application software will display the available gamma settings in a drop list. The application will then supply 
		///	the selected index to the driver via the <see cref="P:ASCOM.DeviceInterface.IVideo.Gamma"/> property. 
		///	<para>It is recommended that this function be called only after a <see cref="P:ASCOM.DeviceInterface.IVideo.Connected">connection</see> is established with the camera hardware, 
		///	to ensure that the driver is aware of the capabilities of the specific camera model.</para>
		///	</remarks>
		ArrayList Gammas { get; }


		/// <summary>
		/// Returns True if the camera supports custom image configuration via the <see cref="M:ASCOM.DeviceInterface.IVideo.ConfigureImage"/> method.
		/// </summary>
		/// <remarks><p style="color:red"><b>Must be implemented</b></p> 
		/// </remarks>
		bool CanConfigureImage { get; }


		/// <summary>
		/// Displays an image configuration dialog that allows live configuration of specialized image settings such as White or Colour Balance for example. 
		/// </summary>
		///	<exception cref="T:ASCOM.NotConnectedException">Must throw an exception if the camera is not connected.</exception>
		///	<exception cref="T:ASCOM.MethodNotImplementedException">Must throw an exception if ConfigureImage is not supported.</exception>
		/// <remarks>
		/// <para>This dialog is not intended to be used in unattended mode but can give great control over the image quality for some drivers and devices. The dialog may also allow 
		/// chaning settings such as Gamma and Gain that can be also controlled directly via the <see cref="T:ASCOM.DeviceInterface.IVideo"/> interface. If a client software 
		/// displays the current Gamma and Gain it should update the values after this method has been called as those values for Gamma and Gain may have changed.</para>
		/// <para>To support automated and unattended control over the specialized image settings available on this dialog the driver must also alow their control via <see cref="P:ASCOM.DeviceInterface.IVideo.SupportedActions"/></para>
		/// </remarks>
		void ConfigureImage();
	}
}
