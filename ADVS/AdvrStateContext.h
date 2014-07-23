#ifndef ADVR_STATE_CONTEXT
#define ADVR_STATE_CONTEXT

#include "HcCommand.h"
#include "HtccMessage.h"
#include "AdvrState.h"
#include "PtGreyImage.h"
#include "FrameExposureInfo.h"

#include <vector>
#include <string>
using namespace std;

enum CameraFrameRate
{
	ExposureNotSet,
	ExposureAuto60Frames,
	ExposureAuto30Frames,
	ExposureAuto15Frames,
	ExposureAuto7point5Frames,
	ExposureAuto3point75Frames,
	ExposureAuto1point875Frames,
	ExposureTriggered1Second,
	ExposureTriggered2Seconds,
	ExposureTriggered3Seconds,
	ExposureTriggered4Seconds,
	ExposureTriggered5Seconds,
	ExposureTriggered6Seconds,
	ExposureTriggered8Seconds,
	ExposureTriggeredManually
};

enum CameraGamma
{
	Gamma_2_FleaGamma_0_5,
	Gamma_1_FleaGamma_1_0,
	Gamma_0_75_FleaGamma_1_333,
	Gamma_0_45_FleaGamma__2_222,
	Gamma_0_35_FleaGamma__2_857,
	Gamma_0_25_FleaGamma__4_0
};

enum TimestampMatchingMode
{
	TimestampMatchingMode_UseCameraTimestamps,
	TimestampMatchingMode_UseFrameIds
};

#define MAX_BRIGHTNESS 6.24
#define BRIGHTNESS_STEP 0.624

class AdvrState;

class AdvrStateContext
{
  AdvrState *m_CurrentState;
  
public:
	long FleaSyncFrameId;
	long HtccSyncFrameId;
	long long FleaSyncFrameCameraTicks;
	long long HtccSyncFrameAdvTicks;
	
public:
	void SetSynchronisationFrameHtccId(long frameId, long long advEndTimeStampTicks);
	void SetSynchronisationFrameImageId(long frameId, long long cameraTicks);
	float GetGammaFromCameraGamma(CameraGamma cmrGamma);
	void SystemInitialised();
	
private:
	long long LastFleaSyncFrameCameraTicks;
	long long ElapsedFleaSyncFrameCameraTicks;
	long long ElapsedHtccSyncFrameAdvTicks;
	long ElapsedFleaSyncFrameCameraCycles;
	long CameraClockCorrectionTicks;
	long long TotalCorrectionTicksInLastCycle;
	long TotalFramesWithCorrectionTicksInLastCycle;
	
	void SetGammaChangedSystemMessage(CameraGamma newGamma);
	void SetFrameRateChangedSystemMessage(CameraFrameRate newFrameRate);

public:
	// Profiling Data
	int ProfilingMode;
	double DisplayedFramesPerSecond;
	double ProcessedFramesPerSecond;
	double HtccLoopsPerSecond;
	double ProcessingLoopsPerSecond;
	double CameraLoopsPerSecond;
	double CameraFramesPerSecond;
	double VirtualMemoryUsed;
	double CPUUsage;
	double PhysicalMemoryUsed;
	int NumberFramesInBuffer;
	int NumberHtccMessagesInBuffer;
	int NumberQueuedFrames;
	int NumberProcessedFrames;
	int NumberRecordedFrames;
	int NumberDroppedFrames;
	int NumberDroppedHtccTimestamps;
	int NumberCameraDroppedFrames;
	int NumberProcessedHtccMessages;
	int NumberReceivedHtccMessages;
	
	bool IsLittleEndianByteOrder;
	unsigned int StartTimeTicks;
	int GetUpTimeMinutes();
	
	int SystemSettingMessageSeqNo;	
	char SystemSettingMessage[255];

public:
	// Status parameters saved in the ADV file STATUS section
	float StatusGamma;
	vector<string> StatusGPSFixMessages;
	bool HasStatusGPSFixData;
	vector<string> StatusUserCommands;
	bool HasStatusUserCommandData;
	vector<string> StatusSystemErrorMessages;
	bool HasStatusSystemErrorData;
	
public:
	// Initialization Data
	bool IsCameraDetected;
	bool IsHtccDetected;
	bool IsHcDetected;
	int HtccSecondsRemainingToGSPSignal;
	bool IsCameraDisconnectedFromHtcc;
	
	bool IsCameraConnected;
	bool IsHtccConnected;
	bool IsReSynchronisingTimestamps;
	
	bool IsVideoOnlyMode;
	int EnhancedDisplayMode;
	bool InvertedDisplay;
	
	bool FrameCountingSynchronised;
	bool FirstCameraFrameReceived;
	bool FirstCameraFrameTimestampReceived;
	bool ExpectFirstCameraFrame;
	bool ExpectDroppedCameraImages;
	bool ExpectDroppedHtccTimeStamps;
	
	CameraFrameRate StartingFrameRateAfterInitialization;
	CameraFrameRate FrameRate;
	CameraGamma Gamma;
	float Gain;
	float Brightness;
	
	bool SystemIsReady();
	
public:
    bool IsRecording;
	bool IsRecordingStopping;
	char* RecordingFileName;
	int CurrentlyProcessedFrameId;
	int LastQueuedFrameId;
	bool HelpScreenDisplayed;
	
    AdvrStateContext();	
	
	void SetState(AdvrState *newState);	
	bool ReceiveCommand(HcCommand *cmd);
	bool ReceiveMessage(HtccMessage *msg);
	void DisplayCurrentFrame(unsigned short* pImageData);    	
	void ProcessCurrentFrame(PtGreyImage* image, FrameExposureInfo* exposure);
	
	void CameraConnected();
	void HtccConnected();
	void HcConnected();
	void NoCameraAvailable();
	void NoCameraConnected();
	
	int CompareFrameIdToTimeStampId(
		int frameId, long long cameraTicks, int timestampId, long long htccAdvTicks, 
		unsigned int exposureIn10thMilliseconds, float shutterSeconds, FrameExposureInfo* frameExposure);
	void ResetProfiling();
	
	void IncreaseFrameRate();
	void DecreaseFrameRate();
	void SetFrameRate(CameraFrameRate newFrameRate);
	
	void IncreaseGamma();
	void DecreaseGamma();
	void IncreaseGain();
	void DecreaseGain();
	void IncreaseBrightness();
	void DecreaseBrightness();
	
	void ToggleHelpScreen();
	
	void AddStatusCommand(char* commandText);
	void AddStatusGPSFix(unsigned char gpsFixStatus, unsigned char almanacStatus);
	void AddStatusSystemError(char* systemError);	
	void PostHtccGremlinMessage(HtccMessage* message);
	void ClearStatusMessagesBuffers();
	
	void SetEnhancedModeMessage();
};



#endif