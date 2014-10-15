/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

#include "stdafx.h"

#include "stdlib.h"

#include "GlobalVars.h"
#include "GlobalConfig.h"
#include "DisplayConsts.h"
#include "PtGreyImage.h"

#include "AdvrStateContext.h"
#include "AdvrNotReadyState.h"
#include "AdvrShuttingDownState.h"
#include "AdvrRecordingState.h"
#include "AdvrReadyState.h"

#include "SerialHtccLoop.h"

#define UNINITIALISED_FRAME_ID -1

AdvrStateContext::AdvrStateContext()
{
	IsRecording = false;
	FrameRate = ExposureNotSet;
	
	StartTimeTicks = SDL_GetTicks();
	
	ResetProfiling();
	ProfilingMode = PROFILING_MODE_FRAME_RATE_ONLY;
	
	IsLittleEndianByteOrder = CFG_ADV_CAMERA_BYTE_ORDER == CFG_ADV_BYTE_ORDER_LITTLE_ENDIAN;
	
	RecordingFileName = NULL;
	
	m_CurrentState = NULL;
	FleaSyncFrameId = UNINITIALISED_FRAME_ID;
	HtccSyncFrameId = UNINITIALISED_FRAME_ID;
	FleaSyncFrameCameraTicks = 0;
	HtccSyncFrameAdvTicks = 0;
	
	InitAdvTicksConversion();
	
	IsCameraDetected = false;
	IsHtccDetected = false;
	IsHtccConnected = false;
	IsHcDetected = false;
	IsVideoOnlyMode = false;
	ExpectDroppedCameraImages = false;
	ExpectDroppedHtccTimeStamps= false;
	EnhancedDisplayMode = 0;
	InvertedDisplay = false;
	
	SetState(AdvrNotReadyState::Instance());
	
	Gamma = CAMERA_STARTING_GAMMA;
	Gain = CAMERA_STARTING_GAIN;
	Brightness = CAMERA_STARTING_BRIGHTNESS;
	StartingFrameRateAfterInitialization = CAMERA_FRAME_RATE_ON_STARTUP;
}

void AdvrStateContext::ResetProfiling()
{
	NumberDroppedFrames = 0;
	NumberDroppedHtccTimestamps = 0;
	NumberCameraDroppedFrames = 0;
	NumberProcessedFrames = 0;
	NumberQueuedFrames = 0;
	NumberRecordedFrames = 0;
	NumberProcessedHtccMessages = 0;
	NumberReceivedHtccMessages = 0;
	
	AdvProfiling_ResetPerformanceCounters();
	SendHtccCommand(HtccCmdResetErrorCounter);
}

void AdvrStateContext::ToggleHelpScreen()
{
	HelpScreenDisplayed = !HelpScreenDisplayed;
}

int AdvrStateContext::GetUpTimeMinutes()
{
	Uint32 nowTicks = SDL_GetTicks();
	return (nowTicks - StartTimeTicks) / 60000;
}

void AdvrStateContext::SetState(AdvrState *newState)
{
	if (m_CurrentState != NULL)
		m_CurrentState->Finalise(this);
	
	m_CurrentState = newState;
	newState->Initialise(this);
}
	
bool AdvrStateContext::ReceiveMessage(HtccMessage *msg)
{
	return m_CurrentState->ReceiveMessage(this, msg);
}
	
void AdvrStateContext::CameraConnected()
{
	#ifdef CAMERA_DETAILED_LOG
	printf("Camera: AdvrStateContext::CameraConnected\n");
	#endif	
	IsCameraDetected = true;
	IsCameraConnected = true;
	if (SystemIsReady()) SystemInitialised();
}

void AdvrStateContext::HtccConnected()
{
	IsHtccDetected = true;
	IsHtccConnected = true;	
	if (SystemIsReady()) SystemInitialised();
}

void AdvrStateContext::HcConnected()
{
	IsHcDetected = true;
	if (SystemIsReady()) SystemInitialised();
}

void AdvrStateContext::SystemInitialised()
{
	if (!FrameCountingSynchronised)
	{
		FrameCountingSynchronised = true;
		printf("HTCC: Initialised Flea3(%ld|%lli) = Htcc(%ld|%lli)\n", FleaSyncFrameId, FleaSyncFrameCameraTicks, HtccSyncFrameId, HtccSyncFrameAdvTicks);
		
		s_AdvrState->NumberDroppedFrames = 0;
		s_AdvrState->NumberCameraDroppedFrames = 0;
		s_AdvrState->CameraClockCorrectionTicks = 0;
		s_AdvrState->TotalCorrectionTicksInLastCycle = 0;
		s_AdvrState->TotalFramesWithCorrectionTicksInLastCycle = 0;
			
		m_CurrentState->SystemInitialised(this);		
	}
}

void AdvrStateContext::NoCameraAvailable()
{
	#ifdef CAMERA_DETAILED_LOG
	printf("Camera: AdvrStateContext::NoCameraAvailable");
	#endif
	IsCameraDetected = false;
}

void AdvrStateContext::NoCameraConnected()
{
	IsCameraConnected = false;
}
	

bool AdvrStateContext::SystemIsReady()
{
	//printf("DEBUG::SystemIsReady() IsCameraDetected:%d IsHtccDetected:%d IsHcDetected:%d FirstCameraFrameTimestampReceived:%d FirstCameraFrameReceived:%d\n", IsCameraDetected, IsHtccDetected, IsHcDetected, FirstCameraFrameTimestampReceived, FirstCameraFrameReceived);
	
	return 
		IsCameraDetected && 
		IsHtccDetected && 
		IsHcDetected &&
		FirstCameraFrameTimestampReceived && 
		FirstCameraFrameReceived;
};


bool AdvrStateContext::ReceiveCommand(HcCommand* cmd)
{
	return m_CurrentState->ReceiveCommand(this, cmd);
}
	
void AdvrStateContext::ProcessCurrentFrame(PtGreyImage* image, FrameExposureInfo* exposure)
{
	m_CurrentState->ProcessCurrentFrame(this, image, exposure);
}
	
void AdvrStateContext::DisplayCurrentFrame(unsigned short* pImageData)
{
	m_CurrentState->DisplayCurrentFrame(this, pImageData);
}	

int AdvrStateContext::CompareFrameIdToTimeStampId(
	int frameId, 
	long long cameraTicks, 
	int timestampId, 
	long long htccAdvTicks, 
	unsigned int exposureIn10thMilliseconds,
	float shutterSeconds,
	FrameExposureInfo* frameExposure)
{	
	printf("FrameID = %d  camTicks = %lli  TimestampID = %d  HTCCticks = %lli \n ", frameId, cameraTicks, timestampId, htccAdvTicks);
	int frameIdAdjustedToTimeStampId = frameId - FleaSyncFrameId + HtccSyncFrameId;
	
	if (frameId == 0 && cameraTicks == 0) 
		// Sometimes camera may return bogus images which have a frameid of '0'. We ignore them
	{
		if (DETAILED_CAMERA_AND_HTCC_MESSAGE_EXPOSURE_OUTPUT)
			printf("Dropping camera image with id = 0. CameraTicks = %lli; ShutterSeconds = %3.6f\n", cameraTicks, shutterSeconds);		
		return -1;
	}

	if (HTCC_TO_CAMERA_TIMESTAMP_MATCHING_MODE == TimestampMatchingMode_UseCameraTimestamps)
	{
		int cycleAdjustment = 0;
		long long cameraExposure;
		if (LastFleaSyncFrameCameraTicks > cameraTicks) 
		{
			cycleAdjustment++;
			cameraExposure = 128 * 10000 + cameraTicks - LastFleaSyncFrameCameraTicks;
			if (DETAILED_CAMERA_AND_HTCC_MESSAGE_EXPOSURE_OUTPUT)
				printf("New Cycle %li at [%lli > %lli]\n", ElapsedFleaSyncFrameCameraCycles, LastFleaSyncFrameCameraTicks, cameraTicks);		
		}
		else
			cameraExposure = cameraTicks - LastFleaSyncFrameCameraTicks;
			
		long long elapsedHtccSyncFrameAdvTicks = (10 * htccAdvTicks) - HtccSyncFrameAdvTicks;
		long long elapsedFleaSyncFrameCameraTicks = (ElapsedFleaSyncFrameCameraCycles + cycleAdjustment) * 128 * 10000 + cameraTicks - FleaSyncFrameCameraTicks;
		double diffMs = (double)(elapsedHtccSyncFrameAdvTicks - elapsedFleaSyncFrameCameraTicks - CameraClockCorrectionTicks) / 10.0;
		if (DETAILED_CAMERA_AND_HTCC_MESSAGE_EXPOSURE_OUTPUT)
		{
#ifdef HTCC_DETAILED_LOG			
			printf("F-%d, H-%d. CE: %3.1fms, TE: %3.1fms. Diff: %3.2f ms. Shutter: %3.1f ms %s\n", 
				   frameId, timestampId,  cameraExposure / 10.0, exposureIn10thMilliseconds / 10.0, 
				   diffMs, shutterSeconds * 1000.0, frameExposure->DebugString());
#else
			printf("F-%d, H-%d. CE: %3.1fms, TE: %3.1fms. Diff: %3.2f ms. Shutter: %3.1f ms\n", 
				   frameId, timestampId,  cameraExposure / 10.0, exposureIn10thMilliseconds / 10.0, diffMs, shutterSeconds * 1000.0);
#endif						
		}
		
		LastFleaSyncFrameCameraTicks = cameraTicks;
		ElapsedFleaSyncFrameCameraCycles += cycleAdjustment;
		
		if (diffMs < HTCC_TO_CAMERA_END_TIMESTAMP_LOWER_ERROR)
		{
			// Must drop Htcc timestamp
			if (DETAILED_CAMERA_AND_HTCC_MESSAGE_EXPOSURE_OUTPUT && 
				!(frameIdAdjustedToTimeStampId < timestampId) && 
				!ExpectDroppedHtccTimeStamps)
				printf("WARNING: About to drop a timestamp when frameIdAdjustedToTimeStampId >= timestampId\n");
			
#ifdef HTCC_DETAILED_LOG			
			printf("Htcc timestamp messages: %s\n", frameExposure->DebugString());
#endif			
			return 1;
		}
		else if (diffMs > HTCC_TO_CAMERA_END_TIMESTAMP_UPPER_ERROR)
		{
			// Must drop Camera image
			if (DETAILED_CAMERA_AND_HTCC_MESSAGE_EXPOSURE_OUTPUT && 
				!(frameIdAdjustedToTimeStampId > timestampId) &&
				!ExpectDroppedCameraImages)
				printf("WARNING: About to drop a camera image when frameIdAdjustedToTimeStampId <= timestampId\n");		
			return -1;
		}
		else
		{
			ElapsedHtccSyncFrameAdvTicks = elapsedHtccSyncFrameAdvTicks;
			ElapsedFleaSyncFrameCameraTicks = elapsedFleaSyncFrameCameraTicks;
			
			if (diffMs < CAMERA_TIMESTAMP_CORRECTION_POINT * HTCC_TO_CAMERA_END_TIMESTAMP_LOWER_ERROR ||
				diffMs > CAMERA_TIMESTAMP_CORRECTION_POINT * HTCC_TO_CAMERA_END_TIMESTAMP_UPPER_ERROR)
			{
				if (TotalFramesWithCorrectionTicksInLastCycle > 0)
					CameraClockCorrectionTicks = TotalCorrectionTicksInLastCycle / TotalFramesWithCorrectionTicksInLastCycle;
				else
					CameraClockCorrectionTicks = elapsedHtccSyncFrameAdvTicks - elapsedFleaSyncFrameCameraTicks;
				
				if (DETAILED_CAMERA_AND_HTCC_MESSAGE_EXPOSURE_OUTPUT)
					printf("New camera correction: %li\n", CameraClockCorrectionTicks);

				TotalFramesWithCorrectionTicksInLastCycle = 0;
				TotalCorrectionTicksInLastCycle = 0;		
			}
			else
			{
				TotalCorrectionTicksInLastCycle += (elapsedHtccSyncFrameAdvTicks - elapsedFleaSyncFrameCameraTicks);
				TotalFramesWithCorrectionTicksInLastCycle++;
			}
		
			if (DETAILED_CAMERA_AND_HTCC_MESSAGE_EXPOSURE_OUTPUT && frameIdAdjustedToTimeStampId != timestampId)
				printf(" WARNING: Matched frame ids not equal: frameIdAdjustedToTimeStampId != timestampId\n");	
			
			return 0;
		}		
	}
	else if (HTCC_TO_CAMERA_TIMESTAMP_MATCHING_MODE == TimestampMatchingMode_UseFrameIds)
	{
		if (frameIdAdjustedToTimeStampId < timestampId)
			return -1;
		else if (frameIdAdjustedToTimeStampId > timestampId)
			return 1;
		else if (frameIdAdjustedToTimeStampId == timestampId)
		{
			long long cameraExposure;
			if (LastFleaSyncFrameCameraTicks > cameraTicks) 			
			{
				ElapsedFleaSyncFrameCameraCycles++;
				cameraExposure = 128 * 10000 + cameraTicks - LastFleaSyncFrameCameraTicks;
				if (DETAILED_CAMERA_AND_HTCC_MESSAGE_EXPOSURE_OUTPUT)
					printf("New Cycle %li at [%lli > %lli]\n", ElapsedFleaSyncFrameCameraCycles, LastFleaSyncFrameCameraTicks, cameraTicks);
			}
			else
				cameraExposure = cameraTicks - LastFleaSyncFrameCameraTicks;
				
			LastFleaSyncFrameCameraTicks = cameraTicks;
			
			ElapsedHtccSyncFrameAdvTicks = (10 * htccAdvTicks) - HtccSyncFrameAdvTicks;
			ElapsedFleaSyncFrameCameraTicks = ElapsedFleaSyncFrameCameraCycles * 128 * 10000 + cameraTicks - FleaSyncFrameCameraTicks;
			
			double diff = (double)(ElapsedHtccSyncFrameAdvTicks - ElapsedFleaSyncFrameCameraTicks);
			double halfExposure = (double)(exposureIn10thMilliseconds / 2);
			
			if (DETAILED_CAMERA_AND_HTCC_MESSAGE_EXPOSURE_OUTPUT)
				printf("F-%d, H-%d. (F: %3.1fms, H: %3.1fms. Diff: %3.2f ms. Shutter: %3.1f ms)\n", frameId, timestampId,  cameraExposure / 10.0, exposureIn10thMilliseconds / 10.0, diff / 10.0, shutterSeconds * 1000.0);
				
			return 0;
		}		
	}
}

void AdvrStateContext::SetSynchronisationFrameHtccId(long frameId, long long advEndTimeStampTicks)
{
	HtccSyncFrameId = frameId;
	HtccSyncFrameAdvTicks = advEndTimeStampTicks;
	ElapsedHtccSyncFrameAdvTicks = 0;
	FirstCameraFrameTimestampReceived = true;
	
	if (SystemIsReady()) SystemInitialised();
}

void AdvrStateContext::SetSynchronisationFrameImageId(long frameId, long long cameraTicks)
{
	FleaSyncFrameId = frameId;
	FleaSyncFrameCameraTicks = cameraTicks;
	LastFleaSyncFrameCameraTicks = cameraTicks;
	ElapsedFleaSyncFrameCameraTicks = 0;
	ElapsedFleaSyncFrameCameraCycles = 0;
	FirstCameraFrameReceived = true;
	
	if (SystemIsReady()) SystemInitialised();
}

void AdvrStateContext::IncreaseFrameRate()
{
	CameraFrameRate oldFrameRate = FrameRate;
	m_CurrentState->IncreaseFrameRate(this);
	
	if (oldFrameRate != FrameRate)
		SetFrameRateChangedSystemMessage(FrameRate);
}

void AdvrStateContext::DecreaseFrameRate()
{
	CameraFrameRate oldFrameRate = FrameRate;
	m_CurrentState->DecreaseFrameRate(this);
	
	if (oldFrameRate != FrameRate)
		SetFrameRateChangedSystemMessage(FrameRate);
}
/*
void AdvrStateContext::SetFrameRate(CameraFrameRate newFrameRate)
{		
	FrameRate = s_Camera->ChangeFrameRate(newFrameRate);
}*/

void AdvrStateContext::SetFrameRateChangedSystemMessage(CameraFrameRate newFrameRate)
{			
	SystemSettingMessageSeqNo++;
	switch(FrameRate)
	{		
		case ExposureAuto60Frames:
			sprintf(&SystemSettingMessage[0], "Frame rate: 60 fps");	
			break;
			
		case ExposureAuto30Frames:
			sprintf(&SystemSettingMessage[0], "Frame rate: 30 fps");
			break;
				
		case ExposureAuto15Frames:
			sprintf(&SystemSettingMessage[0], "Frame rate: 15 fps");
			break;
				
		case ExposureAuto7point5Frames:
			sprintf(&SystemSettingMessage[0], "Frame rate: 7.5 fps");
			break;
				
		case ExposureAuto3point75Frames:
			sprintf(&SystemSettingMessage[0], "Frame rate: 3.75 fps");
			break;
				
		case ExposureAuto1point875Frames:
			sprintf(&SystemSettingMessage[0], "Frame rate: 1.875 fps");
			break;
				
		case ExposureTriggered1Second:
			sprintf(&SystemSettingMessage[0], "Exposure: %1.3f sec", (float)(TRIGGERED_EXPOSURE_1_DURATION / 1000.0));
			break;
				
		case ExposureTriggered2Seconds:
			sprintf(&SystemSettingMessage[0], "Exposure: %1.3f sec", (float)(TRIGGERED_EXPOSURE_2_DURATION / 1000.0));
			break;
				
		case ExposureTriggered3Seconds:
			sprintf(&SystemSettingMessage[0], "Exposure: %1.3f sec", (float)(TRIGGERED_EXPOSURE_3_DURATION / 1000.0));
			break;
			
		case ExposureTriggered4Seconds:
			sprintf(&SystemSettingMessage[0], "Exposure: %1.3f sec", (float)(TRIGGERED_EXPOSURE_4_DURATION / 1000.0));
			break;
			
		case ExposureTriggered5Seconds:
			sprintf(&SystemSettingMessage[0], "Exposure: %1.3f sec", (float)(TRIGGERED_EXPOSURE_5_DURATION / 1000.0));
			break;
			
		case ExposureTriggered6Seconds:
			sprintf(&SystemSettingMessage[0], "Exposure: %1.3f sec", (float)(TRIGGERED_EXPOSURE_6_DURATION / 1000.0));
			break;
				
		case ExposureTriggered8Seconds:
			sprintf(&SystemSettingMessage[0], "Exposure: %1.3f sec", (float)(TRIGGERED_EXPOSURE_7_DURATION / 1000.0));
			break;				
	}
}

void AdvrStateContext::SetGammaChangedSystemMessage(CameraGamma newGamma)
{			
	SystemSettingMessageSeqNo++;
	switch(newGamma)
	{
		case Gamma_2_FleaGamma_0_5:
			sprintf(&SystemSettingMessage[0], "Gamma: 0.500 (MIN)");	
			break;
		case Gamma_1_FleaGamma_1_0:
			sprintf(&SystemSettingMessage[0], "Gamma: 1.000 (OFF)");
			break;
		case Gamma_0_75_FleaGamma_1_333:				
			sprintf(&SystemSettingMessage[0], "Gamma: 1.333");
			break;
		case Gamma_0_45_FleaGamma__2_222:
			sprintf(&SystemSettingMessage[0], "Gamma: 2.222 (LO)");
			break;
		case Gamma_0_35_FleaGamma__2_857:
			sprintf(&SystemSettingMessage[0], "Gamma: 2.857 (HI)");
			break;
		case Gamma_0_25_FleaGamma__4_0:
			sprintf(&SystemSettingMessage[0], "Gamma: 4.000 (MAX)");
			break;
	}	
}

void AdvrStateContext::SetEnhancedModeMessage()
{			
	SystemSettingMessageSeqNo++;
	switch(EnhancedDisplayMode % 4)
	{
		case 0:
			sprintf(&SystemSettingMessage[0], "Standard display mode");	
			break;
			
		case 1:
			sprintf(&SystemSettingMessage[0], "Saturation warning display mode");	
			break;

		case 2:
			sprintf(&SystemSettingMessage[0], "Low gama enhanced display mode");	
			break;
			
		case 3:
			sprintf(&SystemSettingMessage[0], "High gama enhanced display mode");	
			break;			
	}
}

void AdvrStateContext::IncreaseGamma()
{
	int gammaInt;
	gammaInt = static_cast<int>(Gamma);
	gammaInt++;
	
	if (Gamma < Gamma_0_25_FleaGamma__4_0)
	{
		CameraGamma newGamma = static_cast<CameraGamma>(gammaInt);
		float gamma = GetGammaFromCameraGamma(newGamma);	
		
		if (s_Camera->SetGamma(gamma))
		{
			Gamma = newGamma;
			
			SetGammaChangedSystemMessage(newGamma);
		}
	}
}

float AdvrStateContext::GetGammaFromCameraGamma(CameraGamma cmrGamma)
{
	switch(cmrGamma)
	{
		case Gamma_2_FleaGamma_0_5:
			return 0.5;
			break;
		case Gamma_1_FleaGamma_1_0:
			return 1;
			break;
		case Gamma_0_75_FleaGamma_1_333:
			return 1.333;
			break;
		case Gamma_0_45_FleaGamma__2_222:
			return 2.222;
			break;
		case Gamma_0_35_FleaGamma__2_857:
			return 2.857;
			break;
		case Gamma_0_25_FleaGamma__4_0:
			return 4.0;
			break;
	}
	
	return 1;
}

void AdvrStateContext::DecreaseGamma()
{
	int gammaInt;
	gammaInt = static_cast<int>(Gamma);
	gammaInt--;
	
	if (Gamma > Gamma_2_FleaGamma_0_5)
	{
		CameraGamma newGamma = static_cast<CameraGamma>(gammaInt);
		float gamma = GetGammaFromCameraGamma(newGamma);
		
		if (s_Camera->SetGamma(gamma))
		{
			Gamma = newGamma;
			
			SetGammaChangedSystemMessage(newGamma);
		}
	}	
}

void AdvrStateContext::IncreaseGain()
{
	if (Gain < MAX_CAMERA_GAIN)
	{
		if (s_Camera->SetGain(Gain + 1))
		{
			Gain++;
			
			SystemSettingMessageSeqNo++;
			sprintf(&SystemSettingMessage[0], "Gain: %2.0f dB", Gain);
		}
	}
}

void AdvrStateContext::DecreaseGain()
{
	if (Gain >= 1)
	{
		if (s_Camera->SetGain(Gain - 1))
		{
			Gain--;
			
			SystemSettingMessageSeqNo++;
			sprintf(&SystemSettingMessage[0], "Gain: %2.0f dB", Gain);
		}
	}	
}

void AdvrStateContext::IncreaseBrightness()
{
	float newBrightnessValue = Brightness + BRIGHTNESS_STEP;
	
	if (newBrightnessValue < MAX_BRIGHTNESS)
	{
		if (s_Camera->SetBrightness(newBrightnessValue))
		{
			Brightness = newBrightnessValue;
			
			SystemSettingMessageSeqNo++;
			sprintf(&SystemSettingMessage[0], "Offset: %1.3f %%", Brightness);
		}
	}
}

void AdvrStateContext::DecreaseBrightness()
{
	float newBrightnessValue = Brightness - BRIGHTNESS_STEP;
	
	if (newBrightnessValue >=0 )
	{
		if (s_Camera->SetBrightness(newBrightnessValue))
		{
			Brightness = newBrightnessValue;
			
			SystemSettingMessageSeqNo++;
			sprintf(&SystemSettingMessage[0], "Offset: %1.3f %%", Brightness);		
		}
	}	
}


void AdvrStateContext::AddStatusCommand(char* commandText)
{
	pthread_rwlock_wrlock(&s_RWRoot);
	
	HasStatusUserCommandData = true;
	StatusUserCommands.push_back(string(commandText));
	
	pthread_rwlock_unlock(&s_RWRoot);	
}

	
void AdvrStateContext::AddStatusGPSFix(unsigned char gpsFixStatus, unsigned char almanacStatus)
{
	pthread_rwlock_wrlock(&s_RWRoot);
	
	HasStatusGPSFixData = true;
	
	// TODO: Check 'gpsFixStatus' and add status message (lost fix, acquired fix, almanac updated, almanac old
	char gpsFixMessage[255];
	sprintf(gpsFixMessage, "");
	
	StatusGPSFixMessages.push_back(string(gpsFixMessage));
	
	pthread_rwlock_unlock(&s_RWRoot);	
}

void AdvrStateContext::PostHtccGremlinMessage(HtccMessage* message)
{
	char error[255];
	bool displayMessageOnScreen = false;
	
	if (message->ErrorCode == HtccErrorAlmanacUpdate)
	{
		sprintf(error, "The GPS almanac has been updated.");
		AddStatusCommand(error);
		return;
	}
	
	switch(message->ErrorCode)
	{
		case HtccErrorUTCDidNotChangeCorrectly:
			displayMessageOnScreen = true;
			sprintf(error, "Htcc advisory HtccErrorUTCDidNotChangeCorrectly (%d).", message->ErrorCounter);
			break;
			
		case HtccErrorSlowPPS:
			sprintf(error, "Htcc advisory HtccErrorSlowPPS (%d).", message->ErrorCounter);
			break;	
			
		case HtccErrorAbsentPPS:
			sprintf(error, "Htcc advisory HtccErrorAbsentPPS (%d).", message->ErrorCounter);
			break;				
		
		case HtccErrorNoNMEATagMsg:
			displayMessageOnScreen = true;
			sprintf(error, "Htcc advisory HtccErrorNoNMEATagMsg (%d).", message->ErrorCounter);
			break;

		case HtccErrorFailedBoardTest:
			displayMessageOnScreen = true;
			sprintf(error, "Htcc advisory HtccErrorFailedBoardTest (%d).", message->ErrorCounter);
			break;

		case HtccErrorPPSTooQuick:
			displayMessageOnScreen = true;
			sprintf(error, "Htcc advisory HtccErrorPPSTooQuick (%d).", message->ErrorCounter);
			break;

		case HtccErrorUnhandledEvent:
			displayMessageOnScreen = true;
			sprintf(error, "Htcc advisory HtccErrorUnhandledEvent (%d).", message->ErrorCounter);
			break;

		case HtccErrorExposurePacketOutOfOrder:
			displayMessageOnScreen = true;
			sprintf(error, "Htcc advisory HtccErrorExposurePacketOutOfOrder (%d).", message->ErrorCounter);
			break;

		case HtccErrorPPSNotServiced:
			displayMessageOnScreen = true;
			sprintf(error, "Htcc advisory HtccErrorPPSNotServiced (%d).", message->ErrorCounter);
			break;

		case HtccErrorStartTriggerNotServiced:
			displayMessageOnScreen = true;
			sprintf(error, "Htcc advisory HtccErrorStartTriggerNotServiced (%d).", message->ErrorCounter);
			break;
		
		case HtccErrorEndTriggerNotServiced:
			displayMessageOnScreen = true;
			sprintf(error, "Htcc advisory HtccErrorEndTriggerNotServiced (%d).", message->ErrorCounter);
			break;
		
		case HtccErrorExtraTriggerNotServiced:
			displayMessageOnScreen = true;
			sprintf(error, "Htcc advisory HtccErrorExtraTriggerNotServiced (%d).", message->ErrorCounter);
			break;
			
		case HtccErrorCameraDisconnected:
			displayMessageOnScreen = true;
			sprintf(error, "Htcc advisory HtccErrorCameraDisconnected (%d).", message->ErrorCounter);
			break;

		case HtccErrorSerialError:
			displayMessageOnScreen = true;
			sprintf(error, "Htcc advisory HtccErrorSerialError (%d).", message->ErrorCounter);
			break;
		
		default:
			sprintf(error, "Htcc advisory No %d (%d).", message->ErrorCode, message->ErrorCounter);
			break;
	}	

	AddStatusSystemError(error);
	
	if (displayMessageOnScreen)
	{
		pthread_rwlock_wrlock(&s_RWRoot);
		sprintf(s_GeneralErrorMessage, "%s", error);
		pthread_rwlock_unlock(&s_RWRoot);
	}
}

void AdvrStateContext::AddStatusSystemError(char* systemError)
{
	pthread_rwlock_wrlock(&s_RWRoot);
	
	HasStatusSystemErrorData = true;
	StatusSystemErrorMessages.push_back(string(systemError));
	
	pthread_rwlock_unlock(&s_RWRoot);	
}

void AdvrStateContext::ClearStatusMessagesBuffers()
{
	pthread_rwlock_wrlock(&s_RWRoot);
	
	HasStatusSystemErrorData = false;
	StatusSystemErrorMessages.clear();
	
	HasStatusGPSFixData = true;
	StatusGPSFixMessages.clear();
	
	HasStatusUserCommandData = true;
	StatusUserCommands.clear();
	
	pthread_rwlock_unlock(&s_RWRoot);
}