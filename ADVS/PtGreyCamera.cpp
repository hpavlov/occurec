/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

#include "stdafx.h"
#include "stdlib.h"

#include "PtGreyCamera.h"
#include "GlobalConfig.h"
#include "DisplayConsts.h"
#include "GlobalVars.h"
#include "SerialHtccLoop.h"
#include "AdvrUtils.h"
#include "DisplayLoop.h"

using namespace FlyCapture2;

PtGreyCamera::PtGreyCamera()
{
	IsConnected = false;
	IsCapturing = false;
	IsCameraAvailable = false;
	IsInTransition = false;
	Bpp = 16;
	CurrentAdvsFrameRate = ExposureNotSet;
	m_IsBigEndian = true; /* The default byte order of connected camera */

	ConnectionErrorMessageSeqNo = 0;
	OperationErrorMessageSeqNo = 0;

	FC2Version fc2Version;
	Utilities::GetLibraryVersion( &fc2Version );
	SYS_CAMERA_DRIVER_VERSION = (char*)malloc(64);
	sprintf(const_cast<char*>(SYS_CAMERA_DRIVER_VERSION), "%d.%d.%d.%d", fc2Version.major, fc2Version.minor, fc2Version.type, fc2Version.build);
}

void PtGreyCamera::CheckCamera()
{
	BusManager busMgr;
	unsigned int numCameras;
	Error error;

	error = busMgr.GetNumOfCameras(&numCameras);
	if (error != PGRERROR_OK) {
		SetConnectionErrorMessage(error);
		return;
	}

	if (numCameras == 1) {
		error = busMgr.GetCameraFromIndex(0, &m_CameraGuid);
		if (error != PGRERROR_OK) {
			SetConnectionErrorMessage(error);
		} else
			IsCameraAvailable = true;

	} else if (numCameras == 0) {
		// No camera detected
		ConnectionErrorMessage = "No camera detected.";
		ConnectionErrorMessageSeqNo++;
	} else if (numCameras > 1) {
		// Too many cameras
		ConnectionErrorMessage = "Too many cameras detected.";
		ConnectionErrorMessageSeqNo++;
	}

	if (IsCameraAvailable) {
#ifdef CAMERA_DETAILED_LOG
		printf("Camera: Camera found. \n");
#endif
	} else
		printf("Camera: Error Detecting - %s \n", ConnectionErrorMessage);
}


void PtGreyCamera::Connect()
{
	if (IsConnected || !IsCameraAvailable)
		return;
	printf("Using PtGreyCamera Line75 connect statement.\n");
	// Connect to a camera
	Error error = m_Camera.Connect(&m_CameraGuid);
	if (error != PGRERROR_OK) {
		SetConnectionErrorMessage(error);
		return;
	}

	/*FC2Config* pConfig;
	error = m_Camera.GetConfiguration(pConfig);

	pConfig->numBuffers = 1;
	pConfig->numImageNotifications = 1;
	pConfig->minNumImageNotifications = 0;
	pConfig->grabTimeout = 1000;
	pConfig->grabMode = DROP_FRAMES;
	pConfig->isochBusSpeed = BUSSPEED_S800;
	pConfig->asyncBusSpeed = BUSSPEED_ANY;
	pConfig->bandwidthAllocation = BANDWIDTH_ALLOCATION_UNSPECIFIED;
	pConfig->registerTimeoutRetries = 1;
	pConfig->registerTimeout = 100;
	pConfig->highPerformanceRetrieveBuffer = false;

	error = m_Camera.SetConfiguration(pConfig);  */

	// Get the camera information
	error = m_Camera.GetCameraInfo(&m_CameraInfo);
	if (error != PGRERROR_OK) {
		SetConnectionErrorMessage(error);
		return;
	}

	SYS_CAMERA_MODEL = m_CameraInfo.modelName;
	SYS_CAMERA_SERIAL_NO = (char*)malloc(14);
	snprintf(const_cast<char*>(SYS_CAMERA_SERIAL_NO), 14, "%d", m_CameraInfo.serialNumber);

	SYS_CAMERA_VENDOR_NAME = m_CameraInfo.vendorName;
	SYS_CAMERA_SENSOR_INFO = m_CameraInfo.sensorInfo;
	SYS_CAMERA_SENSOR_RESOLUTION = m_CameraInfo.sensorResolution;
	SYS_CAMERA_FIRMWARE_VERSION = m_CameraInfo.firmwareVersion;
	SYS_CAMERA_FIRMWARE_BUILD_TIME = m_CameraInfo.firmwareBuildTime;

	if ((int)strpos(SYS_CAMERA_MODEL, "FL3-FW-03S3M") > -1) {
		// BigFlea3
		IMAGE_WIDTH = 640;
		IMAGE_HEIGHT = 480;
		FRAMERATE_AUTO_30_ENABLED = true;
		CAMERA_FREERUNNNG_MODE = FlyCapture2::VIDEOMODE_640x480Y16;
		SYS_CAMERA_TYPE = BigFlea3;
		strcpy(VideoModeString, "640x480Y16\0");
		MAX_CAMERA_GAIN = 28;
		CFG_ADV_BPP = 12;
	} else if ((int)strpos(SYS_CAMERA_MODEL, "FL3-FW-03S1M") > -1) {
		// SmallFlea3
		IMAGE_WIDTH = 640;
		IMAGE_HEIGHT = 480;
		FRAMERATE_AUTO_30_ENABLED = true;
		CAMERA_FREERUNNNG_MODE = FlyCapture2::VIDEOMODE_640x480Y16;
		SYS_CAMERA_TYPE = SmallFlea3;
		strcpy(VideoModeString, "640x480Y16\0");
		MAX_CAMERA_GAIN = 33;
		CFG_ADV_BPP = 12;
	} else if ((int)strpos(SYS_CAMERA_MODEL, "GX-FW-28S5M") > -1) {
		// GrasshopperExpress
		printf("GEx init values to 800x600 Y16");
		IMAGE_WIDTH = 800;
		IMAGE_HEIGHT = 600;
		FRAMERATE_AUTO_30_ENABLED = true;
		CAMERA_FREERUNNNG_MODE = FlyCapture2::VIDEOMODE_800x600Y16;
		SYS_CAMERA_TYPE = GrasshopperExpress;
		strcpy(VideoModeString, "800x600Y16\0");
		MAX_CAMERA_GAIN = 29;
		CFG_ADV_BPP = 14;

		/*
		IMAGE_WIDTH = 1600;
		IMAGE_HEIGHT = 1200;
		FRAMERATE_AUTO_30_ENABLED = false;
		CAMERA_FREERUNNNG_MODE = FlyCapture2::VIDEOMODE_1600x1200Y16;
		SYS_CAMERA_TYPE = GrasshopperExpress;
		strcpy(VideoModeString, "1600x1200Y16\0");
		MAX_CAMERA_GAIN = 29;
		 */
	} else {
		// Unsupported camera using BigFlea3 settings
		IMAGE_WIDTH = 640;
		IMAGE_HEIGHT = 480;
		FRAMERATE_AUTO_30_ENABLED = true;
		CAMERA_FREERUNNNG_MODE = FlyCapture2::VIDEOMODE_640x480Y16;
		SYS_CAMERA_TYPE = BigFlea3;
		strcpy(VideoModeString, "640x480Y16\0");
	}

	printf("Camera: %s %s, SN %s \n", SYS_CAMERA_VENDOR_NAME, SYS_CAMERA_MODEL, SYS_CAMERA_SERIAL_NO);

	IMAGE_STRIDE = IMAGE_HEIGHT * IMAGE_WIDTH;
	if (NULL != s_PixelInts) {
		delete s_PixelInts;
		s_PixelInts = NULL;
	}

	EnterCriticalSection(&s_SyncDisplayBytes);
	s_PixelInts = (unsigned short*)malloc(IMAGE_STRIDE * sizeof(unsigned short));

	MAX_IMAGES_IN_BUFFER = 0.85 * MAX_BUFFER_MEMORY_IN_MEGABYTES * 1024 * 1024 / (IMAGE_WIDTH * IMAGE_HEIGHT * 2);
	printf("Camera: Maximum %d images will be buffered.\n", MAX_IMAGES_IN_BUFFER);

	ResetWindowScreenMode();

	if (IMAGE_WIDTH == 640)
		InitSdlRects640_480();
	else if (IMAGE_WIDTH == 800)
		InitSdlRects800_600();

	LeaveCriticalSection(&s_SyncDisplayBytes);

	VideoMode videoMode;
	FrameRate frameRate;
	error = m_Camera.GetVideoModeAndFrameRate(&videoMode, &frameRate);
	if (error != PGRERROR_OK) {
		printf("Line216 getVideoMode statement\n");
		SetConnectionErrorMessage(error);
		return;
	}

	if ((int)strpos(SYS_CAMERA_MODEL, "FL3-FW-03S1M") > -1) {
		error = m_Camera.SetVideoModeAndFrameRate(CAMERA_FREERUNNNG_MODE, FRAMERATE_1_875);
	}
	if ((int)strpos(SYS_CAMERA_MODEL, "FL3-FW-03S3M") > -1) {
		error = m_Camera.SetVideoModeAndFrameRate(CAMERA_FREERUNNNG_MODE, FRAMERATE_1_875);
	}
	if ((int)strpos(SYS_CAMERA_MODEL, "GX-FW-28S5M") > -1) {
		error = m_Camera.SetVideoModeAndFrameRate(CAMERA_FREERUNNNG_MODE, FRAMERATE_3_75);
	}

	if (error != PGRERROR_OK) {
		printf("Line223 setVideoModeAndFrameRate - 1.875 statement - did not finish setup on camera.\n");
		SetConnectionErrorMessage(error);
		printf("%s\n", ConnectionErrorMessage);
		return;
	}

	EmbeddedImageInfo pInfo;

	error = m_Camera.GetEmbeddedImageInfo(&pInfo);
	if (error != PGRERROR_OK) {
		SetConnectionErrorMessage(error);
		printf("Error getting embedded info.");
		return;
	}

	pInfo.brightness.onOff = true;
	pInfo.exposure.onOff = true;
	pInfo.frameCounter.onOff = true;
	pInfo.gain.onOff = true;
	pInfo.GPIOPinState.onOff = false;
	pInfo.ROIPosition.onOff = false;
	pInfo.shutter.onOff = true;
	pInfo.strobePattern.onOff = false;
	pInfo.timestamp.onOff = true;
	pInfo.whiteBalance.onOff = false;

	error = m_Camera.SetEmbeddedImageInfo(&pInfo);
	if (error != PGRERROR_OK) {
		SetConnectionErrorMessage(error);
		return;
	}
#ifdef CAMERA_DETAILED_LOG
	printf("Camera: Embedded image info - exposure;frameCounter;gain;shutter;timestamp;\n");
#endif

	if (!ConfigureByteOrder())
		return;

	if (!DisableAutoSettings())
		return;

	if (!ConfigureStrobeSettings())
		return;

	if (!SetTriggerMode(false))
		return;

	s_AdvrState->Gamma = CAMERA_STARTING_GAMMA;
	float gamma = s_AdvrState->GetGammaFromCameraGamma(CAMERA_STARTING_GAMMA);
	SetGamma(gamma);

	s_AdvrState->Gain = CAMERA_STARTING_GAIN;
	SetGain(CAMERA_STARTING_GAIN);

	s_AdvrState->Brightness = CAMERA_STARTING_BRIGHTNESS;
	SetBrightness(CAMERA_STARTING_BRIGHTNESS);

	IsConnected = true;

#ifndef HTCC_SIMULATOR
	// In 'real' mode we don't want the camera to take images unless triggered by HTCC
	// so the initial FrameId synchronisation can be done
	SetTriggerMode(true);
#else
	// Im HTCC simulated mode, we want the camera to start showing images in a free run mode
	s_AdvrState->SetFrameRate(CAMERA_FRAME_RATE_ON_STARTUP);
#endif
}

bool PtGreyCamera::ConfigureStrobeSettings()
{
#ifdef CAMERA_SIMULATOR
	return true;
#endif

	// Set GPIO 1 for output strobe
	StrobeControl mStrobe;
	mStrobe.source = 1;
	mStrobe.onOff = true;
	mStrobe.polarity = 1;
	mStrobe.delay = 0.078f;
	mStrobe.duration = 0.0f;

	Error error = m_Camera.SetStrobe(&mStrobe);
	if (error != PGRERROR_OK) {
		SetOperationErrorMessage(error);
		return false;
	}
#ifdef CAMERA_DETAILED_LOG
	printf("Camera: Strobe control configured to %d;%d;%d;%.3f;%.2f \n", mStrobe.source, mStrobe.onOff , mStrobe.polarity, mStrobe.delay, mStrobe.duration);
#endif

	unsigned int registerValue;
	error = m_Camera.ReadRegister(0x11F8, &registerValue);
	if (error != PGRERROR_OK) {
		SetConnectionErrorMessage(error);
		return false;
	}

	// Set GPIO 0 for input/trigger
	registerValue = registerValue & 0X7FFFFFFF;

	error = m_Camera.WriteRegister(0x11F8, registerValue);
	if (error != PGRERROR_OK) {
		SetConnectionErrorMessage(error);
		return false;
	}

#ifdef CAMERA_DETAILED_LOG
	printf("Camera: GPIO 0 configured for input/trigger \n");
#endif
	return true;
}

bool PtGreyCamera::ConfigureByteOrder()
{
#ifdef CAMERA_SIMULATOR
	return true;
#endif

	// setting the endianness
	unsigned int registerValue;
	Error error = m_Camera.ReadRegister(0x630, &registerValue);
	if (error != PGRERROR_OK) {
		SetConnectionErrorMessage(error);
		return false;
	}

	// 0-7 bit == bit depth; 8 bit = little endian
	bool isLittleEndian = (registerValue & 0X00800000) != 0;
	bool changeEndianness = false;

	if (CFG_ADV_CAMERA_BYTE_ORDER == CFG_ADV_BYTE_ORDER_BIG_ENDIAN) {
		if (isLittleEndian) {
			changeEndianness = true;
			registerValue = registerValue & 0XFF7FFFFF;
		} else
			printf("Camera: Byte order is BIG-ENDIAN. \n");
	} else if (CFG_ADV_CAMERA_BYTE_ORDER == CFG_ADV_BYTE_ORDER_LITTLE_ENDIAN) {
		if (isLittleEndian)
			printf("Camera: Byte order is LITTLE-ENDIAN. \n");
		else {
			changeEndianness = true;
			registerValue = registerValue | 0X00800000;
		}
	}

	if (changeEndianness) {
		error = m_Camera.WriteRegister(0x630, registerValue);
		if (error != PGRERROR_OK) {
			SetConnectionErrorMessage(error);
			return false;
		}

		isLittleEndian = !isLittleEndian;
		printf(isLittleEndian ? "Camera: Byte order set to LITTLE-ENDIAN. \n" : "Camera: Byte order set to BIG-ENDIAN. \n");
	}

	error = m_Camera.ReadRegister(0x630, &registerValue);
	if (error != PGRERROR_OK) {
		SetConnectionErrorMessage(error);
		return false;
	}

	// 0-7 bit == bit depth; 8 bit = little endian
	isLittleEndian = (registerValue & 0X00800000) != 0;

	m_IsBigEndian = !isLittleEndian;

	return true;
}

bool PtGreyCamera::DisableAutoSettings()
{
#ifdef CAMERA_SIMULATOR
	return true;
#endif

	Property prop;
	prop.type = SHARPNESS;

	Error error = m_Camera.GetProperty( &prop );
	if (error != PGRERROR_OK) {
		SetOperationErrorMessage(error);
		return false;
	}

	prop.onOff = false;
	prop.autoManualMode = false;

	error = m_Camera.SetProperty(&prop);
	if (error != PGRERROR_OK) {
		SetOperationErrorMessage(error);
		return false;
	}
#ifdef CAMERA_DETAILED_LOG
	printf("Camera: Auto sharpness disabled. \n");
#endif

	prop.type = BRIGHTNESS;

	error = m_Camera.GetProperty( &prop );
	if (error != PGRERROR_OK) {
		SetOperationErrorMessage(error);
		return false;
	}

	prop.onOff = false;
	prop.autoManualMode = false;

	error = m_Camera.SetProperty(&prop);
	if (error != PGRERROR_OK) {
		SetOperationErrorMessage(error);
		return false;
	}
#ifdef CAMERA_DETAILED_LOG
	printf("Camera: Auto brightness disabled. \n");
#endif

	return true;
}

bool PtGreyCamera::EnableFrameRate(bool enable)
{
	return EnableFrameRate(enable, true);
}

bool PtGreyCamera::EnableFrameRate(bool enable, bool showConsoleMessage)
{
	Property prop;
	prop.type = FRAME_RATE;

	Error error = m_Camera.GetProperty( &prop );
	if (error != PGRERROR_OK) {
		SetOperationErrorMessage(error);
		printf("Error in EnableFrameRate-GetProperty-FRAMERATE\n");
		return false;
	}

	if (enable != prop.onOff) {
		prop.onOff = enable;

		error = m_Camera.SetProperty(&prop);
		if (error != PGRERROR_OK) {
			SetOperationErrorMessage(error);
			printf("Error in EnableFrameRate-SetProperty-FRAMERATE\n");
			return false;
		}

		//if (showConsoleMessage) {

		//}
	}
	if (enable)
		printf("Camera: Auto frame rate enabled. \n");
	else
		printf("Camera: Auto frame rate disabled. \n");

	if (enable)
		DisableAutoSettings();
}

bool PtGreyCamera::AdjustFreeRunningMode(bool isFreeRunningMode)
{
#ifdef CAMERA_SIMULATOR
	return true;
#endif

	// NOTE: In free-running mode we need Auto Exposure and Auto Frame Rate + Turned ON
	//       In manual triggered mode we need the Auto and setting turned OFF

	printf("Using AdjustFreeRunningMode\n");

	Property prop;

	prop.type = AUTO_EXPOSURE;

	Error error = m_Camera.GetProperty( &prop );
	if (error != PGRERROR_OK) {
		SetOperationErrorMessage(error);
		return false;
	}

	prop.onOff = isFreeRunningMode;
	prop.autoManualMode = isFreeRunningMode;

	error = m_Camera.SetProperty(&prop);
	if (error != PGRERROR_OK) {
		SetOperationErrorMessage(error);
		return false;
	}
#ifdef CAMERA_DETAILED_LOG
	if (isFreeRunningMode)
		printf("Camera: Auto exposure enabled. \n");
	else
		printf("Camera: Auto exposure disabled. \n");
#endif

	prop.type = FRAME_RATE;

	error = m_Camera.GetProperty( &prop );
	if (error != PGRERROR_OK) {
		SetOperationErrorMessage(error);
		return false;
	}

	prop.onOff = isFreeRunningMode;
	prop.autoManualMode = isFreeRunningMode;

	error = m_Camera.SetProperty(&prop);
	if (error != PGRERROR_OK) {
		SetOperationErrorMessage(error);
		return false;
	}
#ifdef CAMERA_DETAILED_LOG
	if (isFreeRunningMode)
		printf("Camera: Auto frame rate enabled. \n");
	else
		printf("Camera: Auto frame rate disabled. \n");
#endif

	if (isFreeRunningMode) {
		prop.type = SHUTTER;

		error = m_Camera.GetProperty( &prop );
		if (error != PGRERROR_OK) {
			SetOperationErrorMessage(error);
			return false;
		}

		prop.autoManualMode = true;

		error = m_Camera.SetProperty(&prop);
		if (error != PGRERROR_OK) {
			SetOperationErrorMessage(error);
			return false;
		}
#ifdef CAMERA_DETAILED_LOG
		if (isFreeRunningMode)
			printf("Camera: Auto shutter auto manual mode enabled. \n");
#endif
	}

}

void PtGreyCamera::Disconnect()
{
	if (IsCapturing) {
		// Stop capturing images
		Error error = m_Camera.StopCapture();
		if (error != PGRERROR_OK) {
			SetConnectionErrorMessage(error);
			return;
		}

		IsCapturing = false;
	}

	if (IsConnected) {
		// Power off the camera
		SetCameraPower(false);

		// Disconnect the camera
		Error error = m_Camera.Disconnect();
		if (error != PGRERROR_OK) {
			SetConnectionErrorMessage(error);
			return;
		}

		IsConnected = false;
	}
}

void PtGreyCamera::SetCameraPower(bool on)
{
	const unsigned int powerReg = 0x610;
	unsigned int powerRegVal = 0 ;

	powerRegVal = (on == true) ? 0x80000000 : 0x0;

	Error error = m_Camera.WriteRegister( powerReg, powerRegVal );
	if ( error != PGRERROR_OK ) {
		// Error
		printf("Error writing camera power register. %s", error.GetDescription());
		return;
	} else {
		if (on)
			printf("Camera powered on\n");
		else
			printf("Camera powered off\n");
	}
}

void PtGreyCamera::StartCapture()
{
	if (IsCapturing)
		return;
	printf("Using PtGreyCamera Line603 startCapture\n");

	// Start capturing images
	Error error = m_Camera.StartCapture(NULL, NULL); //in flyCapture, this requires callback = null, data ptr = null
	if (error != PGRERROR_OK) {
		SetConnectionErrorMessage(error);
	} else
		IsCapturing = true;

	if (IsCapturing)
		printf("Camera: Camera is capturing \n");
	else {
		printf("Camera: Error Capturing - %s \n", ConnectionErrorMessage);

		unsigned int registerValue;
		Error error = m_Camera.ReadRegister(0x0614, &registerValue);
		if (error == PGRERROR_OK) {
			printf("Camera: Register 0x614 value is %d \n", registerValue);
		}

		registerValue = 0X00000000;

		error = m_Camera.WriteRegister(0x0610, registerValue);
		if (error == PGRERROR_OK) {
			printf("Camera: Power down \n", registerValue);

			registerValue = 0X00000001;

			error = m_Camera.WriteRegister(0x0610, registerValue);
			if (error == PGRERROR_OK) {
				printf("Camera: Power up \n", registerValue);

				// Wait for camera to power up
				sleep(1);
			}

			IsConnected = false;
			IsCameraAvailable = false;
		}
	}
}

unsigned short* PtGreyCamera::RetrieveFrameBlocking(ImageMetadata* metadata, FlyCapture2::TimeStamp* timestamp)
{
	if (!IsCapturing || IsInTransition)
		return (unsigned short*)malloc(IMAGE_STRIDE * 2);

	Image rawImage;
	// Retrieve an image
	Error error;
	try {
#ifdef CAMERA_DETAILED_LOG
		printf("Camera: RetrieveBuffer is waiting for image ... \n");
#endif

		error = m_Camera.RetrieveBuffer(&rawImage);
		if (error != PGRERROR_OK) {
			if (!IsInTransition) {
				SetConnectionErrorMessage(error);
				CheckAndUpdateStatusAfterError();
			}
			return (unsigned short*)malloc(IMAGE_STRIDE * 2);
		}
	} catch(...) {
		return (unsigned short*)malloc(IMAGE_STRIDE * 2);
	}

	//unsigned int rows = rawImage.GetRows();
	//unsigned int cols = rawImage.GetCols();
	unsigned int bpp = rawImage.GetBitsPerPixel();

	if (bpp != 16) {
		printf("Camera: Error - Image depth is not set to 16bpp! Terminating ...");
		exit(1);
	}

	unsigned char* data	= rawImage.GetData();

	ImageMetadata md = rawImage.GetMetadata();

	metadata->embeddedTimeStamp = md.embeddedTimeStamp;
	metadata->embeddedGain = md.embeddedGain;
	metadata->embeddedShutter = md.embeddedShutter;
	metadata->embeddedBrightness = md.embeddedBrightness;
	metadata->embeddedExposure = md.embeddedExposure;
	metadata->embeddedWhiteBalance = md.embeddedWhiteBalance;
	metadata->embeddedFrameCounter = md.embeddedFrameCounter;
	metadata->embeddedStrobePattern = md.embeddedStrobePattern;
	metadata->embeddedGPIOPinState = md.embeddedGPIOPinState;
	metadata->embeddedROIPosition = md.embeddedROIPosition;

	FlyCapture2::TimeStamp ts = rawImage.GetTimeStamp();
	timestamp->seconds = ts.seconds;
	timestamp->microSeconds = ts.microSeconds;
	timestamp->cycleSeconds = ts.cycleSeconds;
	timestamp->cycleCount = ts.cycleCount;
	timestamp->cycleOffset = ts.cycleOffset;

	//TODO: Use FlyCap 2.3 Image::Convert to get a display image with the correct resolution.
#ifdef CAMERA_DETAILED_LOG
	printf("Camera: Received image %d with shutter %d \n", metadata->embeddedFrameCounter, metadata->embeddedShutter & 0xFFF);
#endif

	return (unsigned short*)data;
}

void PtGreyCamera::CheckAndUpdateStatusAfterError()
{
	// TODO: Add acctual calls that determine the status
	IsConnected = false;
	IsCameraAvailable = false;
	IsCapturing = false;
}

bool PtGreyCamera::EnsureFormat7Mode7IsOff()
{
	VideoMode currVideoMode;
	FrameRate currFrameRate;

	Error error = m_Camera.GetVideoModeAndFrameRate( &currVideoMode, &currFrameRate);
	if (error != PGRERROR_OK) {
		SetOperationErrorMessage(error);
		printf("Camera: Error calling EnsureFormat7Mode7IsOff; GetVideoModeAndFrameRate() errored \n");
		return false;
	}

	if (currVideoMode == VIDEOMODE_FORMAT7) {
		printf("Camera: TODO - Turn OFF Mode7 Format7 \n");
	}
}

bool PtGreyCamera::EnsureFormat7Mode7IsOn(CameraFrameRate newFrameRate, bool force)
{
	VideoMode currVideoMode;
	FrameRate currFrameRate;

	Error error = m_Camera.GetVideoModeAndFrameRate( &currVideoMode, &currFrameRate);
	if (error != PGRERROR_OK) {
		SetOperationErrorMessage(error);
		printf("Camera: Error calling EnsureFormat7Mode7IsOn; GetVideoModeAndFrameRate() errored \n");
		return false;
	}

	if (currVideoMode != VIDEOMODE_FORMAT7 || force)
		SetupFormat7Mode7(newFrameRate);
}

bool PtGreyCamera::SetupFormat7Mode7(CameraFrameRate newFrameRate)
{
#ifdef CAMERA_SIMULATOR
	return true;
#endif

	IsInTransition = true;
	SetIsInTransitionSafe();

	VideoMode currVideoMode;
	FrameRate currFrameRate;
	Format7ImageSettings imageSettings;
	unsigned int packetSize;
	unsigned int maxPacketSize;

	float percentage;

	printf("Using SetupFormat7Mode7\n");

	Error error = m_Camera.GetVideoModeAndFrameRate( &currVideoMode, &currFrameRate);
	if (error != PGRERROR_OK) {
		SetOperationErrorMessage(error);
		IsInTransition = false;
		printf("Camera: Error setting Format_7; GetVideoModeAndFrameRate() errored \n");
		return false;
	}

	if (currVideoMode == VIDEOMODE_FORMAT7) {
		error = m_Camera.GetFormat7Configuration(&imageSettings, &packetSize, &percentage);
		if (error != PGRERROR_OK) {
			SetOperationErrorMessage(error);
			IsInTransition = false;
			printf("Camera: Error setting Format_7; GetFormat7Configuration() errored \n");
			return false;
		}
	}

	m_Camera.StopCapture();

	s_AdvrState->ExpectDroppedCameraImages = true;
	s_AdvrState->ExpectDroppedHtccTimeStamps= true;

	IsCapturing = false;
	bool useTransitionFrameRate = false;

	if (SYS_CAMERA_TYPE == GrasshopperExpress) {
		// TODO: If GX Firmware is version 1.25.3.8 then MODE_7 is supported

		if (newFrameRate >= ExposureTriggered3Seconds) {
			imageSettings.mode = MODE_7;
			imageSettings.offsetX = 560;
			imageSettings.offsetY = 420;
			imageSettings.pixelFormat = PIXEL_FORMAT_MONO16;
			imageSettings.width = IMAGE_WIDTH;
			imageSettings.height = IMAGE_HEIGHT;

			packetSize = 16; // The packet size should be the smallest for the longest exposures
			maxPacketSize = 3776;
			useTransitionFrameRate = newFrameRate < ExposureTriggered5Seconds;
		} else {
			printf("GEx Mode 1\n");
			imageSettings.mode = MODE_1;
			imageSettings.offsetX = 80;
			imageSettings.offsetY = 60;
			imageSettings.pixelFormat = PIXEL_FORMAT_MONO16;
			imageSettings.width = 800;
			imageSettings.height = 600;

			packetSize = 124; // The packet size should be the smallest for the longest exposures
			maxPacketSize = 4092;
			useTransitionFrameRate = newFrameRate < ExposureTriggered5Seconds;
			//useTransitionFrameRate = false;
		}
	} else {
		imageSettings.mode = MODE_7;
		imageSettings.offsetX = 0;
		imageSettings.offsetY = 0;
		imageSettings.pixelFormat = PIXEL_FORMAT_MONO16;
		imageSettings.width = IMAGE_WIDTH;
		imageSettings.height = IMAGE_HEIGHT;

		packetSize = 64; // The packet size should be the smallest for the longest exposures
		maxPacketSize = 3240;
		useTransitionFrameRate = newFrameRate < ExposureTriggered3Seconds;
	}

	if (useTransitionFrameRate) {
		printf("Camera: Using transition frame rate \n");

		error = m_Camera.SetVideoModeAndFrameRate(CAMERA_FREERUNNNG_MODE, FRAMERATE_30);
		if (error != PGRERROR_OK) {
			SetOperationErrorMessage(error);
		}

		error = m_Camera.SetFormat7Configuration(&imageSettings, maxPacketSize);
		if (error != PGRERROR_OK) {
			SetOperationErrorMessage(error);
		}
	} else {
		printf("Camera: Not using transition frame rate \n");

		error = m_Camera.SetVideoModeAndFrameRate(CAMERA_FREERUNNNG_MODE, FRAMERATE_1_875);
		if (error != PGRERROR_OK) {
			SetOperationErrorMessage(error);
		}

		error = m_Camera.SetFormat7Configuration(&imageSettings, packetSize);
		if (error != PGRERROR_OK) {
			SetOperationErrorMessage(error);
		}
	}

	EnableFrameRate(false);

	error = m_Camera.StartCapture();
	if (error != PGRERROR_OK) {
		SetOperationErrorMessage(error);

		if (currVideoMode != VIDEOMODE_FORMAT7) {
			// Trying to restore the non Format7 Mode
			m_Camera.SetVideoModeAndFrameRate(currVideoMode, currFrameRate);
		}

		IsInTransition = false;
		printf("Camera: Error setting Format_7; StartCapture() errored \n");
		return false;
	}

	if (useTransitionFrameRate) {
		error = m_Camera.SetFormat7Configuration(&imageSettings, packetSize);
		if (error != PGRERROR_OK) {
			// NOTE: When transitioning into FORMAT_7 using intermediate FrameRate we usually get a PGRERROR_ISOCH_ALREADY_STARTED error that should be ignored.
			if (error.GetType() != PGRERROR_ISOCH_ALREADY_STARTED) {
				SetOperationErrorMessage(error);
			}
		}
	}

	printf("Camera: Video mode set to VIDEOMODE_FORMAT7 MODE_7 \n");

	m_IsFormat7 = true;
	IsConnected = true;
	IsCapturing = true;
	IsInTransition = false;

	return true;
}

FlyCapture2::VideoMode PtGreyCamera::intendedVideoMode(FrameRate frameRate)
{
	VideoMode intVideoMode;

	if (frameRate == FRAMERATE_30) {
		intVideoMode = CAMERA_FREERUNNNG_MODE;
		return intVideoMode;
	}
	if (frameRate == FRAMERATE_15) {
		intVideoMode = CAMERA_FREERUNNNG_MODE;
		return intVideoMode;
	}
	if (frameRate == FRAMERATE_7_5) {
		intVideoMode = CAMERA_FREERUNNNG_MODE;
		return intVideoMode;
	}
	if (frameRate == FRAMERATE_3_75) {
		intVideoMode = CAMERA_FREERUNNNG_MODE;
		return intVideoMode;
	}
	intVideoMode = VIDEOMODE_FORMAT7;
	return intVideoMode;
}

//-------------------------
void PtGreyCamera::SetFrameRate(FrameRate frameRate)
{
#ifdef CAMERA_SIMULATOR
	return;
#endif

//Version 1

	VideoMode currVideoMode;
	FrameRate currFrameRate;
	printf("Using SetFrameRate Line 903\n");

	Error error = m_Camera.GetVideoModeAndFrameRate( &currVideoMode, &currFrameRate);
	if (error != PGRERROR_OK) {
		SetOperationErrorMessage(error);
	}

	SetIsInTransitionSafe();

	if (currVideoMode == VIDEOMODE_FORMAT7) {
		error = m_Camera.StopCapture();
		if (error != PGRERROR_OK) {
			SetOperationErrorMessage(error);
		}
		s_AdvrState->ExpectDroppedCameraImages = true;
		s_AdvrState->ExpectDroppedHtccTimeStamps= true;
		IsCapturing = false;

		//putting bracket X here causes video scrambling but allows frame rates > 3.75

		error = m_Camera.SetVideoModeAndFrameRate(CAMERA_FREERUNNNG_MODE, frameRate);
		if (error != PGRERROR_OK) {
			SetOperationErrorMessage(error);
			return;
		}

	}//putting bracket X here prevents frame rates > 3.75 but gives good video

	error = m_Camera.StartCapture(NULL, NULL);
	if (error != PGRERROR_OK) {
		SetOperationErrorMessage(error);
	}

	//this bit just reports on frame rates
	switch(frameRate) {
	case FRAMERATE_60:
		printf("Camera: Video mode set to FRAMERATE_60\n");
		break;

	case FRAMERATE_30:
		printf("Camera: Video mode set to FRAMERATE_30\n");
		break;

	case FRAMERATE_15:
		printf("Camera: Video mode set to FRAMERATE_15\n");
		break;

	case FRAMERATE_7_5:
		printf("Camera: Video mode set to FRAMERATE_7_5\n");
		break;

	case FRAMERATE_3_75:
		printf("Camera: Video mode set to FRAMERATE_3_75\n");
		break;

	case FRAMERATE_1_875:
		printf("Camera: Video mode set to FRAMERATE_1_875\n");
		break;

	default:
		printf("Camera: Video mode set to FRAMERATE_1_875\n");
		break;
	}

	EnableFrameRate(true);

	printf("Camera: Finished transition frameRate\n");

	DisableAutoSettings();
	
	pthread_rwlock_wrlock(&s_SyncCameraReset);
	s_Camera->IsInTransition = false;
	pthread_rwlock_unlock(&s_SyncCameraReset);
	
	IsConnected = true;
	IsCapturing = true;
}

//-------------------------
bool PtGreyCamera::SetWhiteBalance(bool enabled, int wbRedChannel, int wbBlueChannel)
{
#ifdef CAMERA_SIMULATOR
	return true;
#endif

	Property prop;
	prop.type = WHITE_BALANCE;

	Error error = m_Camera.GetProperty( &prop );
	if (error != PGRERROR_OK) {
		SetOperationErrorMessage(error);
		return false;
	}

	prop.onOff = enabled;
	prop.autoManualMode = false;

	prop.valueA = wbRedChannel;
	prop.valueB = wbBlueChannel;

	error = m_Camera.SetProperty(&prop);
	if (error != PGRERROR_OK) {
		SetOperationErrorMessage(error);
		return false;
	}
#ifdef CAMERA_DETAILED_LOG
	printf("Camera: White balance set to %d;%d;%d\n", enabled,wbRedChannel,wbBlueChannel);
#endif

	return true;
}

bool PtGreyCamera::SetTriggerMode(bool enabled)
{
	return SetTriggerMode(enabled, true);
}

bool PtGreyCamera::SetTriggerMode(bool enabled, bool showConsoleMessage)
{
#ifdef CAMERA_SIMULATOR
	return true;
#endif

	TriggerMode triggerMode;

	Error error = m_Camera.GetTriggerMode( &triggerMode );
	if (error != PGRERROR_OK) {
		SetOperationErrorMessage(error);
		return false;
	}

	if (triggerMode.onOff != enabled) {
		triggerMode.onOff = enabled;
		triggerMode.polarity = 0; // active low

		if (USE_SOFTWARE_TRIGGER) {
			triggerMode.mode = 0; // Set camera to trigger mode 0
			triggerMode.parameter = 0;
			triggerMode.source = 7; // software trigger
			triggerMode.polarity = SOFTWARE_TRIGGER_POLARITY;
		} else {
			triggerMode.mode = 14; // Trigger Mode 14
			triggerMode.parameter = 0; // Use External Trigger
			triggerMode.source = 0; // Use GPIO 0
		}

		error = m_Camera.SetTriggerMode(&triggerMode);
		if (error != PGRERROR_OK) {
			SetOperationErrorMessage(error);
			return false;
		}

		/*
		if (USE_SOFTWARE_TRIGGER && enabled)
		{
			TriggerDelay triggerDelay;
			error = m_Camera.GetTriggerDelay(&triggerDelay);
			if (error != PGRERROR_OK)
			{
				SetOperationErrorMessage(error);
				return false;
			}

			triggerDelay.absValue = 0.2; // 200 ms delay

			error = m_Camera.SetTriggerDelay(&triggerDelay);
			if (error != PGRERROR_OK)
			{
				SetOperationErrorMessage(error);
				return false;
			}

			//PollForTriggerReady();
		} */

		if (showConsoleMessage) {
			if (enabled)
				printf("Camera: Set to manual trigger mode. \n");
			else
				printf("Camera: Set to auto running mode. \n");
		}
	}

	IsManualTriggeringMode = enabled;

	return true;
}

bool PtGreyCamera::SetExtendedShutterSpeed(float milliseconds)
{
	return SetExtendedShutterSpeed(milliseconds, true);
}

bool PtGreyCamera::SetExtendedShutterSpeed(float milliseconds, bool showConsoleMessage)
{
#ifdef CAMERA_SIMULATOR
	return true;
#endif

	Property prop;
	prop.type = SHUTTER;

	Error error = m_Camera.GetProperty( &prop );
	if (error != PGRERROR_OK) {
		SetOperationErrorMessage(error);
		return false;
	}

	//prop.onOff = true;
	//prop.autoManualMode = false;

	prop.autoManualMode = false;
	prop.absControl = true;

	prop.absValue = milliseconds;

	error = m_Camera.SetProperty(&prop);
	if (error != PGRERROR_OK) {
		SetOperationErrorMessage(error);
		return false;
	}

	unsigned int regVal = 0;

	error = m_Camera.ReadRegister(0x918, &regVal);
	if (error == PGRERROR_OK) {
		typedef union _AbsValueConversion {
			unsigned long ulValue;
			float fValue;
		} AbsValueConversion;

		AbsValueConversion conv;
		conv.ulValue = regVal;
		if (showConsoleMessage)
			printf("Camera: Shutter speed set to %.3f sec\n", conv.fValue);
	}
	return true;
}

bool PtGreyCamera::SetGamma(float gamma)
{
#ifdef CAMERA_SIMULATOR
	return true;
#endif

	// from 0.5 to 4

	Property prop;
	prop.type = GAMMA;

	Error error = m_Camera.GetProperty( &prop );
	if (error != PGRERROR_OK) {
		SetOperationErrorMessage(error);
		return false;
	}

	prop.onOff = true;
	prop.absControl = true;

	prop.absValue = gamma;

	error = m_Camera.SetProperty(&prop);
	if (error != PGRERROR_OK) {
		SetOperationErrorMessage(error);
		return false;
	}
	//#ifdef CAMERA_DETAILED_LOG
	printf("Camera: Gamma set to %.3f\n", gamma);
	//#endif

	s_AdvrState->StatusGamma = gamma;

	return true;
}

bool PtGreyCamera::SetGain(float gain)
{
#ifdef CAMERA_SIMULATOR
	return true;
#endif

	// from 0 to 24 dB in steps of 0.046 db.

	Property prop;
	prop.type = GAIN;

	Error error = m_Camera.GetProperty( &prop );
	if (error != PGRERROR_OK) {
		SetOperationErrorMessage(error);
		return false;
	}

	prop.onOff = true;
	prop.absControl = 	true;
	prop.autoManualMode = false;

	prop.absValue = gain;

	error = m_Camera.SetProperty(&prop);
	if (error != PGRERROR_OK) {
		SetOperationErrorMessage(error);
		return false;
	}
	//#ifdef CAMERA_DETAILED_LOG
	error = m_Camera.GetProperty(&prop);
	printf("Camera: Gain set to %.3f\n", prop.absValue);
	//#endif

	return true;
}

bool PtGreyCamera::SetBrightness(float brightness)
{
#ifdef CAMERA_SIMULATOR
	return true;
#endif

	// The absolute value is set in %  from 0.0% to 6.24%.
	// TODO: Check from the FlyCap app what is the max value for brightness

	Property prop;

	prop.type = BRIGHTNESS;

	Error error = m_Camera.GetProperty( &prop );
	if (error != PGRERROR_OK) {
		SetOperationErrorMessage(error);
		return false;
	}

	prop.absValue = brightness;

	prop.onOff = true;
	prop.absControl = true;
	prop.autoManualMode = false;

	error = m_Camera.SetProperty( &prop );
	if (error != PGRERROR_OK) {
		SetOperationErrorMessage(error);
		return false;
	}

	//#ifdef CAMERA_DETAILED_LOG
	printf("Camera: Brightness set to %.2f %%\n", brightness);
	//#endif
	return true;
}

bool IsHtccTriggertedFrameRate(CameraFrameRate frameRate)
{
	return frameRate == ExposureTriggeredManually;
}

bool IsStandardFrameRate(CameraFrameRate frameRate)
{
	return frameRate != ExposureTriggeredManually;
}

bool PtGreyCamera::PollForTriggerReady()
{
	const unsigned int k_softwareTrigger = 0x62C;
	Error error;
	unsigned int regVal = 0;

	int waitingMessagesDisplayed = 0;
	bool cameraIsNotReadyForTrigger = true;

	do {
		error = m_Camera.ReadRegister(k_softwareTrigger, &regVal);
		if (error != PGRERROR_OK) {
			SetOperationErrorMessage(error);

			return false;
		}

		cameraIsNotReadyForTrigger = (regVal >> 31) != 0;

#ifdef CAMERA_DETAILED_LOG
		if (cameraIsNotReadyForTrigger &&
		    waitingMessagesDisplayed % 25 == 0) {
			waitingMessagesDisplayed++;
			printf("Camera: Waiting for trigger to become ready (%d) ... \n", waitingMessagesDisplayed);
		}

		if (!cameraIsNotReadyForTrigger)
			printf("Camera: Camera is now ready for trigger.\n");
#endif
	} while (cameraIsNotReadyForTrigger);

	return true;
}

bool PtGreyCamera::FireSoftwareTrigger()
{
	const unsigned int k_softwareTrigger = 0x62C;
	const unsigned int k_fireVal = 0x80000000;

	Error error = m_Camera.WriteRegister(k_softwareTrigger, k_fireVal);
	if (error != PGRERROR_OK) {
		SetOperationErrorMessage(error);
		return false;
	}

#ifdef CAMERA_DETAILED_LOG
	printf("Camera: Software trigger fired.\n");
#endif

	return true;
}

void PtGreyCamera::SetIsInTransitionSafe()
{
	pthread_rwlock_wrlock(&s_SyncCameraReset);
	IsInTransition = true;
	pthread_rwlock_unlock(&s_SyncCameraReset);
}


CameraFrameRate PtGreyCamera::ChangeFrameRate(CameraFrameRate newFrameRate)
{
	static int doneTenTimes = 0;
	doneTenTimes ++;
	if (doneTenTimes > 12) {
		doneTenTimes = 12;
	}
	
	if (CurrentAdvsFrameRate != newFrameRate) {

		if (IsHtccTriggertedFrameRate(newFrameRate)) {
			printf("Changing FrameRate using Trigger Modes\n");
			SendHtccCommand(HtccCmdStopTriggeredExposures);

			bool format7Enabled = false;

			if (CurrentAdvsFrameRate == ExposureNotSet || IsStandardFrameRate(CurrentAdvsFrameRate)) {
				AdjustFreeRunningMode(false);
				SetTriggerMode(true);

				format7Enabled = SetupFormat7Mode7(newFrameRate);
			} else
				format7Enabled = true;

			if (format7Enabled) {
				switch(newFrameRate) {
					// NOTE: Triggered 1 sec frame rate is not used, auto running mode is used for 1 sec exposures instead
				case ExposureTriggered1Second:
					SetExtendedShutterSpeed(TRIGGERED_EXPOSURE_1_DURATION);
					SendHtccCommand(HtccCmdStartOneSecTriggeredExposures);
					printf("HTCC: Triggered exposure set to 1 sec\n");
					break;

					// NOTE: Triggered 2 sec frame rate is not used, auto running mode is used for 1 sec exposures instead
				case ExposureTriggered2Seconds:
					SetExtendedShutterSpeed(TRIGGERED_EXPOSURE_2_DURATION);
					SendHtccCommand(HtccCmdStartTwoSecTriggeredExposures);
					printf("HTCC: Triggered exposure set to 2 sec\n");
					break;

				case ExposureTriggered4Seconds:
					SetExtendedShutterSpeed(TRIGGERED_EXPOSURE_4_DURATION);
					SendHtccCommand(HtccCmdStartFourSecTriggeredExposures);
					printf("HTCC: Triggered exposure set to 4 sec\n");
					break;

				case ExposureTriggered8Seconds:
					SetExtendedShutterSpeed(TRIGGERED_EXPOSURE_7_DURATION);
					SendHtccCommand(HtccCmdStartEightSecTriggeredExposures);
					printf("HTCC: Triggered exposure set to 8 sec\n");
					break;

				case ExposureTriggeredManually:
					break;

				default:
					SetExtendedShutterSpeed(TRIGGERED_EXPOSURE_1_DURATION);
					SendHtccCommand(HtccCmdStartOneSecTriggeredExposures);
					printf("HTCC: Triggered exposure set to 1 sec\n");
					newFrameRate = ExposureTriggered1Second;
					break;
				}
			} else
				// Could not change frame rate
				newFrameRate = CurrentAdvsFrameRate;

		} else if (IsStandardFrameRate(newFrameRate)) {
			printf("Changing FrameRate using Auto Modes\n");
			if (CurrentAdvsFrameRate == ExposureNotSet || IsHtccTriggertedFrameRate(CurrentAdvsFrameRate)) {
				SendHtccCommand(HtccCmdStopTriggeredExposures);
				SetTriggerMode(false);
			}

			bool forceFormat7Setup = ShouldForceFormat7Setup(newFrameRate, CurrentAdvsFrameRate);

			switch(newFrameRate) {
			case ExposureAuto60Frames:
				SetFrameRate(FRAMERATE_60);
				break;

			case ExposureAuto30Frames:
			if (SYS_CAMERA_TYPE == GrasshopperExpress) {
					printf("30 fps for GEx\n");
					EnsureFormat7Mode7IsOn(ExposureAuto30Frames, true);
					SetExtendedShutterSpeed(1000/30);
				} else
				SetFrameRate(FRAMERATE_30);
				break;

			case ExposureAuto15Frames:
			if (SYS_CAMERA_TYPE == GrasshopperExpress) {
					printf("15 fps for GEx\n");
					EnsureFormat7Mode7IsOn(ExposureAuto15Frames, true);
					SetExtendedShutterSpeed(1000/15);
				} else
				SetFrameRate(FRAMERATE_15);
				break;

			case ExposureAuto7point5Frames:
			if (SYS_CAMERA_TYPE == GrasshopperExpress) {
					printf("7.5 fps for GEx\n");
					EnsureFormat7Mode7IsOn(ExposureAuto7point5Frames, true);
					SetExtendedShutterSpeed(1000/7.5);
				} else
				SetFrameRate(FRAMERATE_7_5);
				break;

			case ExposureAuto3point75Frames:
			if (SYS_CAMERA_TYPE == GrasshopperExpress & doneTenTimes > 3) {
					printf("3.75 fps for GEx\n");
					EnsureFormat7Mode7IsOn(ExposureAuto3point75Frames, true);
					SetExtendedShutterSpeed(1000/3.75);
				} else
				SetFrameRate(FRAMERATE_3_75);
				break;

			case ExposureAuto1point875Frames:
				// GX Doesn't support 1.875 auto running, so we need to use extended trigger
				if (SYS_CAMERA_TYPE == GrasshopperExpress) {
					printf("1.875 fps for GEx\n");
					EnsureFormat7Mode7IsOn(ExposureAuto1point875Frames, CurrentAdvsFrameRate == ExposureAuto3point75Frames);
					SetExtendedShutterSpeed(1000/1.875);
				} else
					SetFrameRate(FRAMERATE_1_875);
				break;

			case ExposureTriggered1Second:
				EnsureFormat7Mode7IsOn(ExposureTriggered1Second, forceFormat7Setup);
				SetExtendedShutterSpeed(TRIGGERED_EXPOSURE_1_DURATION);
				break;

			case ExposureTriggered2Seconds:

				if (CurrentAdvsFrameRate == ExposureTriggered3Seconds && SYS_CAMERA_TYPE != GrasshopperExpress) {
					// Dirty Hack!
					SetFrameRate(FRAMERATE_30);
					EnsureFormat7Mode7IsOn(ExposureTriggered2Seconds, true);
					SetExtendedShutterSpeed(TRIGGERED_EXPOSURE_2_DURATION);
				} else {
					EnsureFormat7Mode7IsOn(ExposureTriggered2Seconds, false);
					SetExtendedShutterSpeed(TRIGGERED_EXPOSURE_2_DURATION);
				}

				break;

			case ExposureTriggered3Seconds:
				EnsureFormat7Mode7IsOn(ExposureTriggered3Seconds, forceFormat7Setup);
				SetExtendedShutterSpeed(TRIGGERED_EXPOSURE_3_DURATION);
				break;

			case ExposureTriggered4Seconds:
				EnsureFormat7Mode7IsOn(ExposureTriggered4Seconds, forceFormat7Setup);
				SetExtendedShutterSpeed(TRIGGERED_EXPOSURE_4_DURATION);
				break;

			case ExposureTriggered5Seconds:
				EnsureFormat7Mode7IsOn(ExposureTriggered5Seconds, forceFormat7Setup);
				SetExtendedShutterSpeed(TRIGGERED_EXPOSURE_5_DURATION);
				break;

			case ExposureTriggered6Seconds:
				EnsureFormat7Mode7IsOn(ExposureTriggered6Seconds, forceFormat7Setup);
				SetExtendedShutterSpeed(TRIGGERED_EXPOSURE_6_DURATION);
				break;

			case ExposureTriggered8Seconds:
				EnsureFormat7Mode7IsOn(ExposureTriggered8Seconds, forceFormat7Setup);
				SetExtendedShutterSpeed(TRIGGERED_EXPOSURE_7_DURATION);
				break;

			default:
				SetFrameRate(FRAMERATE_3_75);
				newFrameRate = ExposureAuto1point875Frames;
				break;
			}

			if (CurrentAdvsFrameRate == ExposureNotSet || IsHtccTriggertedFrameRate(CurrentAdvsFrameRate)) {
				AdjustFreeRunningMode(true);
			}
		}

		PostFrameRateChangeCommandMessage(newFrameRate);
	}

	CurrentAdvsFrameRate = newFrameRate;

	return newFrameRate;
}

bool PtGreyCamera::ShouldForceFormat7Setup(CameraFrameRate newFrameRate, CameraFrameRate oldFrameRate)
{
	// We sometimes need to roce the Format7 setup in order to change the dead-time for the
	// extended shutter speed we are going to use. The logic of when to do this is defined below

	if (SYS_CAMERA_TYPE != GrasshopperExpress) {
		// Flea3

		switch(newFrameRate) {
		case ExposureTriggered1Second:
			return oldFrameRate == ExposureAuto1point875Frames;

		case ExposureTriggered2Seconds:
			return oldFrameRate == ExposureTriggered3Seconds;

		case ExposureTriggered3Seconds:
			return oldFrameRate == ExposureTriggered2Seconds;
		}
	} else {
		// Grasshopper Express

		// NOT required for GX camera
		switch(newFrameRate) {
		case ExposureTriggered2Seconds:
			return oldFrameRate == ExposureTriggered3Seconds;

		case ExposureTriggered3Seconds:
			return oldFrameRate == ExposureTriggered2Seconds;
		}
	}

	return false;
}

void PtGreyCamera::PostFrameRateChangeCommandMessage(CameraFrameRate newFrameRate)
{
	char command[255];
	switch(newFrameRate) {
	case ExposureAuto60Frames:
		sprintf(command, "Frame rate changed to 60 frames per sec");
		break;

	case ExposureAuto30Frames:
		sprintf(command, "Frame rate changed to 30 frames per sec");
		break;

	case ExposureAuto15Frames:
		sprintf(command, "Frame rate changed to 15 frames per sec");
		break;

	case ExposureAuto7point5Frames:
		sprintf(command, "Frame rate changed to 7.5 frames per sec");
		break;

	case ExposureAuto3point75Frames:
		sprintf(command, "Frame rate changed to 3.75 frames per sec");
		break;

	case ExposureAuto1point875Frames:
		sprintf(command, "Frame rate changed to 1.875 frames per sec");
		break;

	case ExposureTriggered1Second:
		sprintf(command, "Frame rate changed to 1 sec per frame");
		break;

	case ExposureTriggered2Seconds:
		sprintf(command, "Frame rate changed to 2 sec per frame");
		break;

	case ExposureTriggered4Seconds:
		sprintf(command, "Frame rate changed to 4 sec per frame");
		break;

	case ExposureTriggered8Seconds:
		sprintf(command, "Frame rate changed to 8 sec per frame");
		break;

	default:
		sprintf(command, "Frame rate changed");
		break;
	}

	s_AdvrState->AddStatusCommand(command);

}

void PtGreyCamera::SetConnectionErrorMessage(FlyCapture2::Error error)
{
	m_LastError = error;
	ConnectionErrorMessage = error.GetDescription();
	ConnectionErrorMessageSeqNo++;
}

void PtGreyCamera::SetOperationErrorMessage(FlyCapture2::Error error)
{
	m_LastError = error;
	OperationErrorMessage = error.GetDescription();
	OperationErrorMessageSeqNo++;
}

void PtGreyCamera::ClearOperationErrorMessage()
{
	OperationErrorMessage = NULL;
	OperationErrorMessageSeqNo = 0;
}
