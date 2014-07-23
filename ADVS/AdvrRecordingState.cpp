#include "stdafx.h"
#include "stdlib.h"

#include "GlobalVars.h"
#include "GlobalConfig.h"
#include "DisplayConsts.h"

#include "AdvrRecordingState.h"
#include "AdvrReadyState.h"
#include "PtGreyImage.h"
#include "HtccMessage.h"
	
AdvrRecordingState* AdvrRecordingState::s_pInstance = NULL;


AdvrRecordingState::AdvrRecordingState()
{ };

AdvrRecordingState* AdvrRecordingState::Instance()
{
	if (!s_pInstance)
		s_pInstance = new AdvrRecordingState();
	
	return s_pInstance;
};

void AdvrRecordingState::IncreaseFrameRate(AdvrStateContext *context)
{
	/*
	if (context->FrameRate == ExposureTriggered1Second)
	{
		// Decreasing frame rate from 1 spf will require changing the camera mode 
		// to free running. We don't do this during recording. Show error message to user
		pthread_rwlock_wrlock(&s_RWRoot);
		sprintf(s_GeneralErrorMessage, "%s", "Cannot switch exposure modes while recording");
		pthread_rwlock_unlock(&s_RWRoot);
	}
	else
	{
		AdvrState::IncreaseFrameRate(context);
	}*/
	
	AdvrState::IncreaseFrameRate(context);
}

void AdvrRecordingState::DecreaseFrameRate(AdvrStateContext *context)
{
	/*
	if (context->FrameRate == ExposureAuto1point875Frames)
	{
		// Increasing frame rate from 1.875fps will require changing the camera mode 
		// to manually triggered. We don't do this during recording. Show error message to user
		pthread_rwlock_wrlock(&s_RWRoot);
		sprintf(s_GeneralErrorMessage, "%s", "Cannot switch exposure modes while recording");
		pthread_rwlock_unlock(&s_RWRoot);		
	}
	else
	{
		AdvrState::DecreaseFrameRate(context);
	}*/
	
	AdvrState::DecreaseFrameRate(context);
}

bool AdvrRecordingState::ReceiveCommand(AdvrStateContext *context, HcCommand* cmd)
{
	if (cmd->UserCommand == UserCommandStopRecording)
	{
		// The user has requested to stop the recording. See which is the last added frame in the buffer
		// than will need to be saved and enter in "Stopping Mode" (May be use a separate State for this??)
		pthread_rwlock_rdlock(&s_RWRoot);
		m_LastExpectedFrameId = s_AdvrState->LastQueuedFrameId;
		pthread_rwlock_unlock(&s_RWRoot);
		
		context->IsRecordingStopping = true;
		
		return true;
	}
	else
		return AdvrState::ReceiveCommand(context, cmd);	
};

void AdvrRecordingState::DisplayCurrentFrame(AdvrStateContext *context, unsigned short* pImageData)
{
	AdvrState::DisplayCurrentFrame(context, pImageData);
	
	if (context->IsVideoOnlyMode)
		return;
		
	SDL_Surface* sText = TTF_RenderText_Solid(s_AdvrFont, context->IsRecordingStopping ? "STOPPING" : (context->IsRecording ? "RECORDING" : ""), CLR_RED);	
	SDL_BlitSurface(sText, NULL, screen, &RECT_STATUS_LN1);
	SDL_FreeSurface(sText);	
	
	if (context->RecordingFileName != NULL)
	{
		sText = TTF_RenderText_Solid( s_AdvrFont, context->RecordingFileName, CLR_RED);	
		SDL_BlitSurface(sText, NULL, screen, &RECT_STATUS_LN2);
		SDL_FreeSurface(sText);

		char numImages [30];
		snprintf(numImages, 29, "%6d Mb  %6d Frames", (int)(ADVRPF_HDDWRITE_BYTES_WRITTEN / (1024.0  * 1024.0)), context->NumberRecordedFrames);
		sText = TTF_RenderText_Solid( s_AdvrFont, numImages, CLR_RED);	
		SDL_BlitSurface(sText, NULL, screen, &RECT_STATUS_LN3);
		SDL_FreeSurface(sText);
	}
};

void AdvrRecordingState::Initialise(AdvrStateContext *context)
{
	context->NumberRecordedFrames = 0;
	context->NumberDroppedFrames = 0;
	context->NumberDroppedHtccTimestamps = 0;
	context->NumberCameraDroppedFrames = 0;
	m_LastExpectedFrameId = 0;
	m_FirstRecordedFrameTimestamp = 0;	
	
	const char* fileName = GetNextAdvFileName();
	
	boost::filesystem::path filePath (fileName);
	const char* fileNameOnly = filePath.filename().c_str();	
	context->RecordingFileName = (char*)malloc(strlen(fileNameOnly) + 1);	
	strcpy(context->RecordingFileName, fileNameOnly);
	
	AdvNewFile(fileName);		
	
	AdvAddFileTag("ADVR-SOFTWARE-VERSION", SYS_ADVR_SOFTWARE_VERSION);
	AdvAddFileTag("HTCC-FIRMWARE-VERSION", SYS_HTCC_FIRMWARE_VERSION);
	AdvAddFileTag("HTCC-UNIT-CODE", SYS_HTCC_UNIT_NUMBER);
	
	AdvAddFileTag("RECORDER", "ASTRO DIGITAL VIDEO SYSTEM");
	AdvAddFileTag("FSTF-TYPE", "ADV");
	AdvAddFileTag("ADV-VERSION", "1");
	AdvAddFileTag("LONGITUDE-WGS84", &SYS_GPS_LONGITUDE[0]);
	AdvAddFileTag("LATITUDE-WGS84", &SYS_GPS_LATITUDE[0]);
	AdvAddFileTag("ALTITUDE-MSL", &SYS_GPS_ALTITUDE[0]);
	AdvAddFileTag("MSL-WGS84-OFFSET", &SYS_GPS_WSG84[0]);
	AdvAddFileTag("GPS-HDOP", &SYS_GPS_HDOP[0]);
	
	AdvAddFileTag("CAMERA-MODEL", SYS_CAMERA_MODEL);
	AdvAddFileTag("CAMERA-SERIAL-NO", SYS_CAMERA_SERIAL_NO);
	AdvAddFileTag("CAMERA-VENDOR-NAME", SYS_CAMERA_VENDOR_NAME);
	AdvAddFileTag("CAMERA-SENSOR-INFO", SYS_CAMERA_SENSOR_INFO);
	AdvAddFileTag("CAMERA-SENSOR-RESOLUTION)", SYS_CAMERA_SENSOR_RESOLUTION);
	AdvAddFileTag("CAMERA-FIRMWARE-VERSION", SYS_CAMERA_FIRMWARE_VERSION);
	AdvAddFileTag("CAMERA-FIRMWARE-BUILD-TIME", SYS_CAMERA_FIRMWARE_BUILD_TIME);
	AdvAddFileTag("CAMERA-DRIVER-VERSION", SYS_CAMERA_DRIVER_VERSION);
			
	AdvDefineImageSection(IMAGE_WIDTH, IMAGE_HEIGHT, CFG_ADV_BPP);
	AdvAddOrUpdateImageSectionTag("IMAGE-BYTE-ORDER", CFG_ADV_CAMERA_BYTE_ORDER);	
	
	AdvDefineImageLayout(CFG_ADV_LAYOUT_1_UNCOMPRESSED, CFG_ADV_LAYOUT_1_DATA_LAYOUT, CFG_ADV_LAYOUT_1_COMPRESSION, CFG_ADV_LAYOUT_1_BPP, CFG_ADV_LAYOUT_1_KEYFRAME, CFG_ADV_LAYOUT_1_DIFFCORR_BASE_FRAME);
	AdvDefineImageLayout(CFG_ADV_LAYOUT_2_COMPRESSED, CFG_ADV_LAYOUT_2_DATA_LAYOUT, CFG_ADV_LAYOUT_2_COMPRESSION, CFG_ADV_LAYOUT_2_BPP, CFG_ADV_LAYOUT_2_KEYFRAME, CFG_ADV_LAYOUT_2_DIFFCORR_BASE_FRAME);
	AdvDefineImageLayout(CFG_ADV_LAYOUT_3_COMPRESSED, CFG_ADV_LAYOUT_3_DATA_LAYOUT, CFG_ADV_LAYOUT_3_COMPRESSION, CFG_ADV_LAYOUT_3_BPP, CFG_ADV_LAYOUT_3_KEYFRAME, CFG_ADV_LAYOUT_3_DIFFCORR_BASE_FRAME);

	// There are always present
	TAGID_SystemTime = AdvDefineStatusSectionTag("SystemTime", ULong64);
	TAGID_TrackedGPSSatellites = AdvDefineStatusSectionTag("GPSTrackedSatellites", UInt8);
	TAGID_GPSAlmanacStatus = AdvDefineStatusSectionTag("GPSAlmanacStatus", UInt8);
	TAGID_GPSAlmanacOffset = AdvDefineStatusSectionTag("GPSAlmanacOffset", UInt8);
	TAGID_GPSFixStatus = AdvDefineStatusSectionTag("GPSFixStatus", UInt8);
	
	// These are coming though the embedded image data
	TAGID_Gain = AdvDefineStatusSectionTag("Gain", Real);
	TAGID_Shutter = AdvDefineStatusSectionTag("Shutter", Real);
	TAGID_Offset = AdvDefineStatusSectionTag("Offset", Real);
	TAGID_VideoCameraFrameId = AdvDefineStatusSectionTag("VideoCameraFrameId", ULong64);
			
	 // These are not coming through the embedded image data
	TAGID_Gamma = AdvDefineStatusSectionTag("Gamma", Real);
	TAGID_GPSFix = AdvDefineStatusSectionTag("GPSFix", List16OfAnsiString255);
	TAGID_UserCommand = AdvDefineStatusSectionTag("UserCommand", List16OfAnsiString255);
	TAGID_SystemError = AdvDefineStatusSectionTag("SystemError", List16OfAnsiString255);
	TAGID_HardwareTimerFrameId = AdvDefineStatusSectionTag("HardwareTimerFrameId", ULong64);
	
	context->ClearStatusMessagesBuffers();
	s_Camera->PostFrameRateChangeCommandMessage(s_Camera->CurrentAdvsFrameRate);
	
	context->IsRecording = true;	
	context->IsRecordingStopping = false;
};

void AdvrRecordingState::ProcessCurrentFrame(AdvrStateContext *context, PtGreyImage* image, FrameExposureInfo* exposure)
{
	if (!context->IsRecording) return;
	
	if (context->IsRecordingStopping) 
	{
		int currentlyProcessedFrameId;
		pthread_rwlock_rdlock(&s_RWRoot);
		currentlyProcessedFrameId = s_AdvrState->CurrentlyProcessedFrameId;
		pthread_rwlock_unlock(&s_RWRoot);
		
		if (m_LastExpectedFrameId < currentlyProcessedFrameId)
		{
			// Stop recording
			AdvEndFile();
			
			context->IsRecording = false;
			
			// We have already saved the last frame. It is safe to stop the recording now.
			context->SetState(AdvrReadyState::Instance());
			
			return;
		}
	};
	
	unsigned char layoutIdForCurrentFramerate = CFG_ADV_LAYOUT_1_UNCOMPRESSED;
	switch(s_Camera->CurrentAdvsFrameRate)
	{
		case ExposureNotSet:
			break;
			
		case ExposureAuto60Frames:
			layoutIdForCurrentFramerate = IMAGE_LAYOUT_ID_FOR_AUTO_60;
			break;		
		case ExposureAuto30Frames:
			layoutIdForCurrentFramerate = IMAGE_LAYOUT_ID_FOR_AUTO_30;
			break;		
		case ExposureAuto15Frames:
			layoutIdForCurrentFramerate = IMAGE_LAYOUT_ID_FOR_AUTO_15;
			break;			
		case ExposureAuto7point5Frames:
			layoutIdForCurrentFramerate = IMAGE_LAYOUT_ID_FOR_AUTO_7_5;
			break;		
		case ExposureAuto3point75Frames:
			layoutIdForCurrentFramerate = IMAGE_LAYOUT_ID_FOR_AUTO_3_75;
			break;
		case ExposureAuto1point875Frames:
			layoutIdForCurrentFramerate = IMAGE_LAYOUT_ID_FOR_AUTO_1_875;
			break;
		case ExposureTriggered1Second:
			layoutIdForCurrentFramerate = IMAGE_LAYOUT_ID_FOR_TRIGGERED_1;
			break;
		case ExposureTriggered2Seconds:
			layoutIdForCurrentFramerate = IMAGE_LAYOUT_ID_FOR_TRIGGERED_2;
			break;
		case ExposureTriggered3Seconds:
			layoutIdForCurrentFramerate = IMAGE_LAYOUT_ID_FOR_TRIGGERED_3;
			break;		
		case ExposureTriggered4Seconds:
			layoutIdForCurrentFramerate = IMAGE_LAYOUT_ID_FOR_TRIGGERED_4;
			break;		
		case ExposureTriggered5Seconds:
			layoutIdForCurrentFramerate = IMAGE_LAYOUT_ID_FOR_TRIGGERED_5;
			break;		
		case ExposureTriggered6Seconds:
			layoutIdForCurrentFramerate = IMAGE_LAYOUT_ID_FOR_TRIGGERED_6;
			break;		
		case ExposureTriggered8Seconds:
			layoutIdForCurrentFramerate = IMAGE_LAYOUT_ID_FOR_TRIGGERED_8;
			break;
	}
	
	long long timeStamp;
	unsigned int exposureIn10thMilliseconds = 0;
	
	// NOTE: Exposure may be NULL when no timestamp has been matched. We still want to record the image
	//      without a timestamp
	if (exposure != NULL)
		exposure->GetAdvTimeStamp(&timeStamp, &exposureIn10thMilliseconds, true);
		
	unsigned int elapsedTimeMilliseconds = 0; // since the first recorded frame was taken
	if (context->NumberRecordedFrames > 0 && m_FirstRecordedFrameTimestamp != 0)
	{
		elapsedTimeMilliseconds = timeStamp  - m_FirstRecordedFrameTimestamp;
	}
	else if (context->NumberRecordedFrames == 0)
	{
		m_FirstRecordedFrameTimestamp = timeStamp;
	}
	
	bool frameStartedOk = AdvBeginFrame(timeStamp, elapsedTimeMilliseconds, exposureIn10thMilliseconds);
	if (!frameStartedOk)
	{
		// If we can't add the first frame, this may be a file creation issue; otherwise increase the dropped frames counter
		if (context->NumberRecordedFrames > 0) 
			context->NumberDroppedFrames++;
		return;
	}

	// NOTE: If we don't copy the data into a separate buffer there are some racing conditions when running 15fps and above
	unsigned short* dataToSave[IMAGE_STRIDE * sizeof(short)];
	memcpy(&dataToSave[0], &image->ImageData[0], IMAGE_STRIDE * sizeof(short));	
	
	// Add the always present status tags
	struct timeval now;
	gettimeofday(&now, NULL);	
	long long currentSystemTime = UnixHiResTimeToAdvTicks(now) / 10;
	AdvFrameAddStatusTag64(TAGID_SystemTime, currentSystemTime);
	
	unsigned char trackedSatellites = exposure != NULL ? exposure->GetTrackedGPSSatellites() : 0;
	AdvFrameAddStatusTagUInt8(TAGID_TrackedGPSSatellites, trackedSatellites);
	unsigned char almanacStatus = exposure != NULL ? exposure->GetGPSSatellitesAlmanacStatus() : 'E';
	AdvFrameAddStatusTagUInt8(TAGID_GPSAlmanacStatus, almanacStatus);
	char almanacOffset = exposure != NULL ? exposure->GetGPSSatellitesAlmanacOffset() : 0;
	AdvFrameAddStatusTagUInt8(TAGID_GPSAlmanacOffset, (unsigned char)almanacOffset);	
	unsigned char fixStatus = exposure != NULL ? exposure->GetGPSSatellitesFixStatus() : 'E';
	AdvFrameAddStatusTagUInt8(TAGID_GPSFixStatus, fixStatus);

	// Add the status tag values from the image embedded data
	char m_FmtNumberString[16];
	
	AddFrameStatusTagReal(TAGID_Gain, image->GainFloat);
	AddFrameStatusTagReal(TAGID_Gamma, context->StatusGamma);
	AddFrameStatusTagReal(TAGID_Shutter, image->ShutterSeconds);
	AddFrameStatusTagReal(TAGID_Offset, image->BrightnessFloat);
	AdvFrameAddStatusTag64(TAGID_VideoCameraFrameId, (long long)image->FrameId);
	AdvFrameAddStatusTag64(TAGID_HardwareTimerFrameId, exposure != NULL ? (long long)exposure->FrameNo : -1);
	
	pthread_rwlock_rdlock(&s_RWRoot);	
	if (context->HasStatusGPSFixData)
	{		
		vector<string>::iterator curr = context->StatusGPSFixMessages.begin();
		while (curr != context->StatusGPSFixMessages.end()) 
		{			
			AddFrameStatusTagMessage(TAGID_GPSFix, curr->c_str());
			curr++;
		}			
		context->HasStatusGPSFixData = false;
		context->StatusGPSFixMessages.clear();
	}
	if (context->HasStatusUserCommandData)
	{
		vector<string>::iterator curr = context->StatusUserCommands.begin();
		while (curr != context->StatusUserCommands.end()) 
		{			
			AddFrameStatusTagMessage(TAGID_UserCommand, curr->c_str());
			curr++;
		}	
		context->HasStatusUserCommandData = false;
		context->StatusUserCommands.clear();
	}
	if (context->HasStatusSystemErrorData)
	{
		vector<string>::iterator curr = context->StatusSystemErrorMessages.begin();
		while (curr != context->StatusSystemErrorMessages.end()) 
		{			
			AddFrameStatusTagMessage(TAGID_SystemError, curr->c_str());
			curr++;
		}			
		context->HasStatusSystemErrorData = false;
		context->StatusSystemErrorMessages.clear();
	}	

	pthread_rwlock_unlock(&s_RWRoot);
			
	AdvFrameAddImage(layoutIdForCurrentFramerate, (unsigned short*)&dataToSave[0], image->PixelBpp);	
	
	AdvEndFrame();
	
	context->NumberRecordedFrames++;
}

void AdvrRecordingState::Finalise(AdvrStateContext *context)
{
	if (!context->IsRecording) return;
	
	if (context->RecordingFileName != NULL) delete context->RecordingFileName;
	context->RecordingFileName = NULL;
};