/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

#ifndef FLEA3_CAMERA
#define FLEA3_CAMERA

#include "FlyCapture2.h"
#include "AdvrStateContext.h"

class PtGreyCamera
{
private:
	FlyCapture2::Error m_LastError;
	FlyCapture2::PGRGuid m_CameraGuid;
	FlyCapture2::CameraInfo m_CameraInfo;
	
	bool m_IsBigEndian;
	const unsigned short* m_BlackFrame;	
	bool m_IsFormat7;
	
	FlyCapture2::Camera m_Camera;
	
	void CheckAndUpdateStatusAfterError();	
	bool SetupFormat7Mode7(CameraFrameRate newFrameRate);
	bool EnsureFormat7Mode7IsOn(CameraFrameRate newFrameRate, bool force);
	bool EnsureFormat7Mode7IsOff();	
	
	bool ConfigureByteOrder();
	bool DisableAutoSettings();
	bool ConfigureStrobeSettings();
	
	void SetIsInTransitionSafe();	
	
	void SetConnectionErrorMessage(FlyCapture2::Error error);
	void SetOperationErrorMessage(FlyCapture2::Error error);
	bool ShouldForceFormat7Setup(CameraFrameRate newFrameRate, CameraFrameRate oldFrameRate);
	
	char VideoModeString[64];
	
public:
	const char* ConnectionErrorMessage;
	const char* OperationErrorMessage;
	int ConnectionErrorMessageSeqNo;
	int OperationErrorMessageSeqNo;
	
	CameraFrameRate CurrentAdvsFrameRate;
	
	bool IsCameraAvailable;
	bool IsConnected;
	bool IsManualTriggeringMode;
	bool IsCapturing;
	bool IsInTransition;
	int Bpp;
	
	PtGreyCamera();
	
	void CheckCamera();
	void Connect();
	void StartCapture();
	unsigned short* RetrieveFrameBlocking(FlyCapture2::ImageMetadata* metadata, FlyCapture2::TimeStamp* timestamp);
	void Disconnect();
	
	CameraFrameRate ChangeFrameRate(CameraFrameRate newFrameRate);
	void PostFrameRateChangeCommandMessage(CameraFrameRate newFrameRate);	
	
	void SetFrameRate(FlyCapture2::FrameRate frameRate);
	FlyCapture2::VideoMode intendedVideoMode(FlyCapture2::FrameRate frameRate);	
	bool SetWhiteBalance(bool enabled, int wbRedChannel, int wbBlueChannel);
	
	bool SetGamma(float gamma);
	bool SetGain(float gain);
	bool SetBrightness(float brightness);
	
	bool AdjustFreeRunningMode(bool isFreeRunningMode);
	bool SetTriggerMode(bool enabled);
	bool SetTriggerMode(bool enabled, bool showConsoleMessage);	
	bool EnableFrameRate(bool enable);
	bool EnableFrameRate(bool enable, bool showConsoleMessage);
	bool SetExtendedShutterSpeed(float milliseconds);
	bool SetExtendedShutterSpeed(float milliseconds, bool showConsoleMessage);
	void SetCameraPower(bool on);
	
	bool PollForTriggerReady();
	bool FireSoftwareTrigger();
	void ClearOperationErrorMessage();
};



#endif