// The following ifdef block is the standard way of creating macros which make exporting 
// from a DLL simpler. All files within this DLL are compiled with the OCCURECCORE_EXPORTS
// symbol defined on the command line. This symbol should not be defined on any project
// that uses this DLL. This way any other project whose source files include this file see 
// OCCURECCORE_API functions as being imported from a DLL, whereas this DLL sees symbols
// defined with this macro as being exported.
#ifdef OCCURECCORE_EXPORTS
#define OCCURECCORE_API __declspec(dllexport)
#else
#define OCCURECCORE_API __declspec(dllimport)
#endif

#include "OccuRec.Ocr.h"

using namespace OccuOcr;

struct ImageStatus
{
	__int64 StartExposureTicks;
	__int64 EndExposureTicks;
	__int64 StartExposureFrameNo;
	__int64 EndExposureFrameNo;	
	long CountedFrames;
	float CutOffRatio;
	__int64 IntegratedFrameNo;
	__int64 UniqueFrameNo;
	long PerformedAction;
	float PerformedActionProgress;
	long DetectedIntegrationRate;
	long DropedFramesSinceIntegrationLock;
	long OcrWorking;
	long OcrErrorsSinceLastReset;
	long UserIntegratonRateHint;
};

struct FrameProcessingStatus
{
	__int64 CameraFrameNo;
	__int64 IntegratedFrameNo;
	long IntegratedFramesSoFar;
	float FrameDiffSignature;
	float CurrentSignatureRatio;
};

extern long IMAGE_WIDTH;
extern long IMAGE_HEIGHT;
extern long IMAGE_STRIDE;
extern long IMAGE_TOTAL_PIXELS;
extern long MONOCHROME_CONVERSION_MODE;
extern long USE_IMAGE_LAYOUT;

extern bool FLIP_VERTICALLY;
extern bool FLIP_HORIZONTALLY;

extern bool IS_INTEGRATING_CAMERA;
extern float SIGNATURE_DIFFERENCE_RATIO;

extern bool OCR_IS_SETUP;
extern long OCR_FRAME_TOP_ODD;
extern long OCR_FRAME_TOP_EVEN;
extern long OCR_CHAR_WIDTH;
extern long OCR_CHAR_FIELD_HEIGHT;
extern long* OCR_ZONE_MATRIX;	

extern long MEDIAN_CALC_INDEX_FROM;
extern long MEDIAN_CALC_INDEX_TO;

extern OcrFrameProcessor* firstFrameOcrProcessor;
extern OcrFrameProcessor* lastFrameOcrProcessor;

void FrameProcessingThreadProc(void* pContext);

HRESULT SetupCamera(long width, long height, LPCTSTR szCameraModel, long monochromeConversionMode, bool flipHorizontally, bool flipVertically, bool isIntegrating);
HRESULT SetupGrabberInfo(LPCTSTR szGrabberName, LPCTSTR szVideoMode);
HRESULT SetupIntegrationDetection(float minDiffRatio, float minSignDiff, float diffGamma);
HRESULT SetupIntegrationPreservationArea(int areaTopOdd, int areaTopEven, int areaHeight);
HRESULT SetupOcrAlignment(long width, long height, long frameTopOdd, long frameTopEven, long charWidth, long charHeight, long numberOfCharPositions, long numberOfZones, long* pixelsInZones);
HRESULT SetupOcrZoneMatrix(long* matrix);
HRESULT SetupOcrChar(char character, long fixedPosition);
HRESULT SetupOcrCharDefinitionZone(char character, long zoneId, long zoneValue);
HRESULT DisableOcrProcessing();
HRESULT SetupAav(long useImageLayout, long usesBufferedMode, long integrationDetectionTuning, LPCTSTR szOccuRecVersion);
HRESULT GetCurrentImage(BYTE* bitmapPixels);
HRESULT GetCurrentImageStatus(ImageStatus* ImageStatus);
HRESULT ProcessVideoFrame(LPVOID bmpBits, __int64 currentUtcDayAsTicks, FrameProcessingStatus* frameInfo);
HRESULT ProcessVideoFrame2(long* pixels, __int64 currentUtcDayAsTicks, FrameProcessingStatus* frameInfo);
HRESULT StartRecording(LPCTSTR szFileName);
HRESULT StopRecording(long* pixels);
HRESULT LockIntegration(bool lock);
HRESULT SetManualIntegrationHint(long manualRate);
HRESULT ControlIntegrationCalibration(long operation);
HRESULT GetIntegrationCalibrationDataConfig(long* gammasLength, long* signaturesPerCycle);
HRESULT GetIntegrationCalibrationData(float* rawSignatures, float* gammas);
HRESULT InitNewIntegrationPeriodTesting(float differenceRatio, float minimumDifference);
HRESULT TestNewIntegrationPeriod(__int64 frameNo, float diffSignature, bool* isNew);
