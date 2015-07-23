/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

// OccuRec.Core.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"

#include "recording_buffer.h"
#include "raw_frame_buffer.h"

#include "OccuRec.Core.h"
#include "RawFrame.h"
#include "stdlib.h"
#include <vector>
#include <stdio.h>
#include "IntegratedFrame.h";
#include "aav_lib.h"
#include <windows.h>

#include "IotaVtiOcr.h"
#include "OccuRec.Ocr.h"
#include "OccuRec.IntegrationChecker.h"

#include "simplified_tracking.h"
#include "OccuRec.Math.h"

using namespace OccuOcr;

#define MEDIAN_CALC_ROWS_FROM 10
#define MEDIAN_CALC_ROWS_TO 11
#define STARTUP_FRAMES_WITH_NO_OUTPUT 2

bool AAV_16 = false;
long AAV16_MAX_BINNED_FRAMES = 0;
long IMAGE_WIDTH;
long IMAGE_HEIGHT;
long IMAGE_STRIDE;
long IMAGE_TOTAL_PIXELS;
long MONOCHROME_CONVERSION_MODE;
long USE_IMAGE_LAYOUT = 4;
long USE_COMPRESSION_ALGORITHM = 0;
bool USE_BUFFERED_FRAME_PROCESSING = true;
bool INTEGRATION_DETECTION_TUNING = false;
bool USE_NTP_TIMESTAMP = false;
bool USE_SECONDARY_TIMESTAMP = false;
bool RECORD_ONLY_STATUS_CHANNEL_WITH_OCRED_TIMESTAMPS = false;
long NTP_DEBUG_REC_FREQ = 1;
float NTP_DEBUG_MAX_IGNORED_SHIFT = 10;


bool OCR_IS_SETUP = false;
bool RUN_TRACKING = false;
long TRACKED_TARGET_ID = -1;
long TRACKED_GUIDING_ID = -1;
long TRACKING_FREQUENCY = 1;
float TRACKED_TARGET_APERTURE = 10;
float TRACKED_GUIDING_APERTURE = 10;
float TRACKING_BG_INNER_RADIUS = 2;
long TRACKING_BG_MIN_NUM_PIXELS = 200;
bool OCR_PRESERVE_VTI;
long OCR_FRAME_TOP_ODD;
long OCR_FRAME_TOP_EVEN;
long OCR_CHAR_WIDTH;
long OCR_CHAR_FIELD_HEIGHT; 
long* OCR_ZONE_MATRIX = NULL; 

bool OCR_FAILED_TEST_RECORDING = false;

long MEDIAN_CALC_INDEX_FROM;
long MEDIAN_CALC_INDEX_TO;

OcrFrameProcessor* firstFrameOcrProcessor = NULL;
OcrFrameProcessor* lastFrameOcrProcessor = NULL;
OcrManager* ocrManager = NULL;

bool FLIP_VERTICALLY;
bool FLIP_HORIZONTALLY;

bool IS_INTEGRATING_CAMERA;
bool FORCE_NEW_FRAME_ON_LOCKED_RATE;
float SIGNATURE_DIFFERENCE_RATIO;
float MINIMUM_SIGNATURE_DIFFERENCE;
bool USES_DIFF_GAMMA;
unsigned char GAMMA[256];

long MANUAL_INTEGRATION_RATE = 0;
long NO_INTEGRATION_STACK_RATE = 0;
bool INTEGRATION_LOCKED;
int inconsistentIntegrations = 0;

#define INTEGRATION_CALIBRATION_CYCLES 10
#define GAMMA_PROBES_COUNT 10
#define MAX_CALIBRATION_SIGNATURES_SIZE 256 * GAMMA_PROBES_COUNT * INTEGRATION_CALIBRATION_CYCLES

float GAMMA_PROBES[GAMMA_PROBES_COUNT];

bool INTEGRATION_CALIBRATION;
long INTEGRATION_CALIBRATION_TOTAL_PASSES;
long INTEGRATION_CALIBRATION_PASSES;

float CALIBRATION_SIGNATURES[MAX_CALIBRATION_SIGNATURES_SIZE];

unsigned char* prtPreviousDiffArea = NULL;
__int64 numberOfDiffSignaturesCalculated; 
long numberOfIntegratedFrames;
bool lastFrameWasNewIntegrationPeriod;
bool trackedThisIntegrationPeriod = false;

double* integratedPixels = NULL;

__int64 idxFrameNumber = 0;
__int64 idxFirstFrameNumber = 0;
__int64 idxLastFrameNumber = 0;
__int64 idxFirstFrameTimestamp = 0;
__int64 idxLastFrameTimestamp = 0;
__int64 idxIntegratedFrameNumber = 0;
char firstFrameTimestampStr[128];
char endFrameTimestampStr[128];
long droppedFramesSinceIntegrationIsLocked = 0;
long lockedIntegrationFrames = 0;
bool ocrFirstFrameProcessed = false;
long ocrErrorsSiceLastReset = 0;
__int64 firstFrameNtpTimestamp = 0;
__int64 lastFrameNtpTimestamp = 0;
__int64 firstFrameSecondaryTimestamp = 0;
__int64 lastFrameSecondaryTimestamp = 0;
__int64 VIDEO_FRAME_IN_WINDOWS_TICKS = 0;

unsigned char* latestIntegratedFrame = NULL;
ImageStatus latestImageStatus;
ImageStatus latestDetectedIntegrationFrameImageStatus;

unsigned char* firstIntegratedFramePixels = NULL;
unsigned char* lastIntegratedFramePixels = NULL;
unsigned char* currTrackedFramePixels = NULL;

HANDLE hRecordingThread = NULL;
bool recording = false;
long long numRecordedFrames = 0;
double averageNtpDebugOffsetMS = 0;
double aggregatedNtpDebug = 0;
char cameraModel[128];
char occuRecVersion[32];
char grabberName[128];
char videoMode[128];
char targetInfo[128];
char telescopeInfo[128];
char observerInfo[128];
char obsLongitude[128];
char obsLatitude[128];
char raObjInfo[128];
char decObjInfo[128];
float videoFrameRate = 0;
int HARDWARE_TIMING_CORRECTION = 0;

unsigned int STATUS_TAG_NUMBER_INTEGRATED_FRAMES;
unsigned int STATUS_TAG_START_FRAME_ID;
unsigned int STATUS_TAG_END_FRAME_ID;
unsigned int STATUS_TAG_START_TIMESTAMP;
unsigned int STATUS_TAG_END_TIMESTAMP;
unsigned int STATUS_TAG_SYSTEM_TIME;
unsigned int STATUS_TAG_NTP_START_TIMESTAMP;
unsigned int STATUS_TAG_NTP_END_TIMESTAMP;
unsigned int STATUS_TAG_SECONDARY_START_TIMESTAMP;
unsigned int STATUS_TAG_SECONDARY_END_TIMESTAMP;
unsigned int STATUS_TAG_GPS_TRACKED_SATELLITES;
unsigned int STATUS_TAG_GPS_ALMANAC;
unsigned int STATUS_TAG_GPS_FIX;
unsigned int STATUS_TAG_OCR_TESTING_ERROR_MESSAGE;
unsigned int STATUS_TAG_NTP_TIME_ERROR;
unsigned int STATUS_TAG_GAMMA;
unsigned int STATUS_TAG_GAIN;
unsigned int STATUS_TAG_TEMPERATURE;
unsigned int STATUS_TAG_EXPOSURE;

OccuRec::IntegrationChecker* integrationChecker;

void ClearResourses()
{
	if (NULL != prtPreviousDiffArea)
	{
		delete prtPreviousDiffArea;
		prtPreviousDiffArea = NULL;
	}

	if (NULL != integratedPixels)
	{
		delete integratedPixels;
		integratedPixels = NULL;
	}
}

HRESULT LockIntegration(bool lock)
{
	INTEGRATION_LOCKED = lock;

	if (INTEGRATION_LOCKED)
		droppedFramesSinceIntegrationIsLocked = 0;

	if (NULL != ocrManager)
	{
		ocrManager->Reset();
		ocrFirstFrameProcessed = false;
	}

	return S_OK;
}

HRESULT ControlIntegrationCalibration(long cameraIntegrationRate)
{
	if (cameraIntegrationRate == 0)
	{
		INTEGRATION_CALIBRATION = false;
	}
	else if (cameraIntegrationRate > 0)
	{
		// NOTE: There is surely a better place to do this
		GAMMA_PROBES[0] = 0.10;
		GAMMA_PROBES[1] = 0.15;
		GAMMA_PROBES[2] = 0.25;
		GAMMA_PROBES[3] = 0.35;
		GAMMA_PROBES[4] = 0.55;
		GAMMA_PROBES[5] = 0.75;
		GAMMA_PROBES[6] = 1.0;
		GAMMA_PROBES[7] = 2.0;
		GAMMA_PROBES[8] = 3.0;
		GAMMA_PROBES[9] = 4.0;

		INTEGRATION_CALIBRATION = true;
		INTEGRATION_CALIBRATION_TOTAL_PASSES = INTEGRATION_CALIBRATION_CYCLES * GAMMA_PROBES_COUNT * cameraIntegrationRate;
		INTEGRATION_CALIBRATION_PASSES = 0;
	}

	return S_OK;
}

HRESULT GetIntegrationCalibrationDataConfig(long* gammasLength, long* signaturesPerCycle)
{
	*gammasLength = GAMMA_PROBES_COUNT;
	*signaturesPerCycle = INTEGRATION_CALIBRATION_TOTAL_PASSES / GAMMA_PROBES_COUNT;

	return S_OK;
}

HRESULT GetIntegrationCalibrationData(float* rawSignatures, float* gammas)
{
	*gammas = 0.10; *gammas++;
	*gammas = 0.15; *gammas++;
	*gammas = 0.25; *gammas++;
	*gammas = 0.35; *gammas++;
	*gammas = 0.55; *gammas++;
	*gammas = 0.75; *gammas++;
	*gammas = 1.0; *gammas++;
	*gammas = 2.0; *gammas++;
	*gammas = 3.0; *gammas++;
	*gammas = 4.0;

	for (int i = 0; i < INTEGRATION_CALIBRATION_TOTAL_PASSES; i++)
	{
		*(rawSignatures + i) = CALIBRATION_SIGNATURES[i];
	}

	return S_OK;
}

OccuRec::IntegrationChecker* testChecker = NULL;

HRESULT InitNewIntegrationPeriodTesting(float differenceRatio, float minimumDifference)
{
	if (NULL != testChecker)
	{
		delete testChecker;
		testChecker = NULL;
	}

	testChecker = new OccuRec::IntegrationChecker(differenceRatio, minimumDifference);
	testChecker->ControlIntegrationDetectionTuning(true);

	return S_OK;
}

HRESULT TestNewIntegrationPeriod(__int64 frameNo, float diffSignature, bool* isNew)
{
	if (NULL != testChecker)
	{
		*isNew = testChecker->IsNewIntegrationPeriod_Automatic(frameNo, diffSignature);
		return S_OK;
	}
	else
		return E_HANDLE;
}

HRESULT SetManualIntegrationHint(long manualRate)
{
	MANUAL_INTEGRATION_RATE = manualRate;
	NO_INTEGRATION_STACK_RATE = 0;

	return S_OK;
}

HRESULT SetNoIntegrationStackRate(long stackRate)
{
	NO_INTEGRATION_STACK_RATE = stackRate;

	return S_OK;
}

bool IsNewIntegrationPeriod(float diffSignature, bool expectedNewPeriod)
{
	if (!IS_INTEGRATING_CAMERA)
		return true;

	if (INTEGRATION_LOCKED)
	{
		// TODO:
		//return IsNewIntegrationPeriodOfLockedIntegration(diffSignature, diffSignature2);
	}

	if (NULL != integrationChecker)
	{
		SyncLock::LockIntDet();

		bool isNewIntegrationPeriod = MANUAL_INTEGRATION_RATE > 0
			? integrationChecker->IsNewIntegrationPeriod_Manual(idxFrameNumber, MANUAL_INTEGRATION_RATE, NO_INTEGRATION_STACK_RATE, diffSignature)
			: integrationChecker->IsNewIntegrationPeriod_Automatic(idxFrameNumber, diffSignature);

		if (FORCE_NEW_FRAME_ON_LOCKED_RATE && INTEGRATION_LOCKED)
		{
			if (!isNewIntegrationPeriod && expectedNewPeriod && inconsistentIntegrations < 3)
			{
				isNewIntegrationPeriod = true;
				inconsistentIntegrations++;
			}
			else if (isNewIntegrationPeriod && !expectedNewPeriod && inconsistentIntegrations < 3)
			{
				isNewIntegrationPeriod = false;
				inconsistentIntegrations++;
			}
			else if (isNewIntegrationPeriod && expectedNewPeriod)
				inconsistentIntegrations = 0;
		}

		SyncLock::UnlockIntDet();

		return isNewIntegrationPeriod;
	}
	else
		return false;
}


HRESULT SetupAav(long useImageLayout, long compressionAlgorithm, long bpp, long usesBufferedMode, long integrationDetectionTuning, LPCTSTR szOccuRecVersion, long recordNtpTimestamp, long recordSecondaryTimestamp)
{
	OCR_IS_SETUP = false;
	USE_IMAGE_LAYOUT = useImageLayout;
	USE_COMPRESSION_ALGORITHM = compressionAlgorithm;
	USE_BUFFERED_FRAME_PROCESSING = usesBufferedMode == 1;
	INTEGRATION_DETECTION_TUNING = integrationDetectionTuning == 1;
	USE_NTP_TIMESTAMP = recordNtpTimestamp == 1;
	USE_SECONDARY_TIMESTAMP = recordSecondaryTimestamp == 1;
	RECORD_ONLY_STATUS_CHANNEL_WITH_OCRED_TIMESTAMPS = useImageLayout == 5;

	if (NULL != integrationChecker)
	{
		SyncLock::LockIntDet();
		integrationChecker->ControlIntegrationDetectionTuning(INTEGRATION_DETECTION_TUNING);
		SyncLock::UnlockIntDet();
	}
	
	strcpy(&occuRecVersion[0], (char *)szOccuRecVersion);

	switch(USE_IMAGE_LAYOUT)
	{
		case 1:
			DebugViewPrint(L"AAVSetup: ImageLayout = FULL-IMAGE-RAW::UNCOMPRESSED; BufferedMode = %d; IntegrationTuning: %s\n", USE_BUFFERED_FRAME_PROCESSING ? 1:0, INTEGRATION_DETECTION_TUNING ? L"Y":L"N"); 
			break;
		case 2:
			DebugViewPrint(L"AAVSetup: ImageLayout = FULL-IMAGE-DIFFERENTIAL-CODING-NOSIGNS::QUICKLZ; BufferedMode = %d; IntegrationTuning: %s\n", USE_BUFFERED_FRAME_PROCESSING ? 1:0, INTEGRATION_DETECTION_TUNING ? L"Y":L"N"); 
			break;
		case 3:
			DebugViewPrint(L"AAVSetup: ImageLayout = FULL-IMAGE-DIFFERENTIAL-CODING::QUICKLZ; BufferedMode = %d; IntegrationTuning: %s\n", USE_BUFFERED_FRAME_PROCESSING ? 1:0, INTEGRATION_DETECTION_TUNING ? L"Y":L"N"); 
			break;
		case 4:
			DebugViewPrint(L"AAVSetup: ImageLayout = FULL-IMAGE-RAW::QUICKLZ; BufferedMode = %d; IntegrationTuning: %s\n", USE_BUFFERED_FRAME_PROCESSING ? 1:0, INTEGRATION_DETECTION_TUNING ? L"Y":L"N"); 
			break;
		case 5:
			DebugViewPrint(L"AAVSetup: ImageLayout = STATUS-CHANNEL-ONLY::QUICKLZ; BufferedMode = %d; IntegrationTuning: %s\n", USE_BUFFERED_FRAME_PROCESSING ? 1:0, INTEGRATION_DETECTION_TUNING ? L"Y":L"N"); 
			break;
		default:
			DebugViewPrint(L"AAVSetup: ImageLayout = %d; BufferedMode = %d; IntegrationTuning: %s\n", USE_IMAGE_LAYOUT, USE_BUFFERED_FRAME_PROCESSING ? 1:0, INTEGRATION_DETECTION_TUNING ? L"Y":L"N"); 
			break;
	}

	AAV_16 = bpp == 16;

	return S_OK;
}

HRESULT SetupNtpDebugParams(long debugValue1, float debugValue2)
{
	NTP_DEBUG_REC_FREQ = debugValue1;
	NTP_DEBUG_MAX_IGNORED_SHIFT = debugValue2;

	return S_OK;
}

HRESULT SetupIntegrationPreservationArea(bool preserveVti, int areaTopOdd, int areaTopEven, int areaHeight)
{
	OCR_PRESERVE_VTI = preserveVti;
	OCR_FRAME_TOP_ODD = areaTopOdd;
	OCR_FRAME_TOP_EVEN = areaTopEven;
	OCR_CHAR_FIELD_HEIGHT = areaHeight;

	if (OCR_PRESERVE_VTI)
		DebugViewPrint(L"IntegrationPreservationArea: From = %d; To = %d\n", OCR_FRAME_TOP_ODD, OCR_FRAME_TOP_EVEN + 2 * OCR_CHAR_FIELD_HEIGHT); 
	else
		DebugViewPrint(L"IntegrationPreservationArea is turned OFF\n"); 
			
	return S_OK;
}

HRESULT SetupOcrAlignment(long width, long height, long frameTopOdd, long frameTopEven, long charWidth, long charHeight, long numberOfCharPositions, long numberOfZones, long zoneMode, long* pixelsInZones)
{
	if (IMAGE_WIDTH != width || IMAGE_HEIGHT != height)
	{
		OCR_IS_SETUP = false;
		return E_FAIL;
	}

	OCR_FRAME_TOP_ODD = frameTopOdd;
	OCR_FRAME_TOP_EVEN = frameTopEven;
	OCR_CHAR_WIDTH = charWidth;
	OCR_CHAR_FIELD_HEIGHT = charHeight;
	OCR_NUMBER_OF_ZONES = numberOfZones;
	OCR_ZONE_MODE = zoneMode;
	OCR_NUMBER_OF_CHAR_POSITIONS = numberOfCharPositions;

	if (NULL != OCR_ZONE_MATRIX)
	{
		delete OCR_ZONE_MATRIX;
		OCR_ZONE_MATRIX = NULL;
	}
	OCR_ZONE_MATRIX = (long*)malloc(IMAGE_TOTAL_PIXELS * sizeof(long));

	::ZeroMemory(OCR_ZONE_MATRIX, IMAGE_TOTAL_PIXELS);
	
	MEDIAN_CALC_INDEX_FROM = MEDIAN_CALC_ROWS_FROM * IMAGE_WIDTH;
	MEDIAN_CALC_INDEX_TO = MEDIAN_CALC_ROWS_TO * IMAGE_WIDTH;
	
	for (int i = 0; i < MAX_ZONE_COUNT; i++)
	{
		if (i < numberOfZones)
			OCR_ZONE_PIXEL_COUNTS[i] = pixelsInZones[i];
		else
			OCR_ZONE_PIXEL_COUNTS[i] = 0;
	}
	
	OCR_CHAR_DEFS.clear();

	return S_OK;
}

HRESULT SetupOcrZoneMatrix(long* matrix)
{
	if (NULL == OCR_ZONE_MATRIX)
		OCR_ZONE_MATRIX = (long*)malloc(IMAGE_TOTAL_PIXELS * sizeof(long));
	
	for (long i=0; i < IMAGE_TOTAL_PIXELS; i++)
	{
		OCR_ZONE_MATRIX[i] = matrix[i];

/*		long pixX = i % IMAGE_WIDTH;
		long pixY = i / IMAGE_WIDTH; 
		long packed = OCR_ZONE_MATRIX[pixY * IMAGE_WIDTH + pixX];

		if (packed != 0)
		{
			long charId;
			bool isOddField;
			long zoneId;
			long zonePixelId;

			UnpackValue(packed, &charId, &isOddField, &zoneId, &zonePixelId);

			DebugViewPrint(L"<%d,%d> = %d|(%d,%s,%d,%d)", pixX, pixY, packed, charId, isOddField ? "O" : "E", zoneId, zonePixelId);
		}*/			
	}

	if (NULL != firstFrameOcrProcessor)
	{
		delete firstFrameOcrProcessor;
		firstFrameOcrProcessor = NULL;
	}
	if (NULL != lastFrameOcrProcessor)
	{
		delete lastFrameOcrProcessor;
		lastFrameOcrProcessor = NULL;
	}
	if (NULL != ocrManager)
	{
		delete ocrManager;
		ocrManager = NULL;
	}

	firstFrameOcrProcessor = new OcrFrameProcessor();
	lastFrameOcrProcessor = new OcrFrameProcessor();

	ocrManager = new OcrManager();
	ocrFirstFrameProcessed = false;

	OCR_IS_SETUP = true;

	DebugViewPrint(L"OCR has been setup.");

	return S_OK;
}

HRESULT SetupOcrChar(char character, long fixedPosition)
{
	OcrCharDefinition *charDef = new OcrCharDefinition(character, fixedPosition);

	OCR_CHAR_DEFS.push_back(charDef);

	return S_OK;
}

HRESULT SetupOcrCharDefinitionZone(char character, long zoneId, long zoneValue, long zonePixelsCount)
{
	vector<OcrCharDefinition*>::iterator curr = OCR_CHAR_DEFS.begin();
	while (curr != OCR_CHAR_DEFS.end()) 
	{
		if (character == (*curr)->Character)
		{
			(*curr)->AddZoneEntry(zoneId, zoneValue, zonePixelsCount);
			return S_OK;
		}
		
		curr++;
	}

	return E_FAIL;
}

void SetupDiffGammaMemoryTable(float diffGamma)
{
	if (diffGamma < 0.95 && diffGamma > 1.05)
	{
		USES_DIFF_GAMMA = true;
		float gammaAmpl = 255 / pow(255, diffGamma);
		for (int i = 0; i < 256; i++)
		{
			int gammaVal = (int)(gammaAmpl * pow(i, diffGamma));
			if (gammaVal <= 0)
				GAMMA[i] = 0;
			else if (gammaVal >= 255)
				GAMMA[i] = 255;
			else
				GAMMA[i] = (unsigned char)gammaVal;
		}
	}
	else
	{
		USES_DIFF_GAMMA = false;
	}
}

HRESULT SetupObservationInfo(LPCTSTR szTargetInfo, LPCTSTR szTelescopeInfo, LPCTSTR szObserverInfo, LPCTSTR szLongitude, LPCTSTR szLatitude)
{
	strcpy(&targetInfo[0], (char *)szTargetInfo);
	strcpy(&telescopeInfo[0], (char *)szTelescopeInfo);
	strcpy(&observerInfo[0], (char *)szObserverInfo);
	strcpy(&obsLongitude[0], (char *)szLongitude);
	strcpy(&obsLatitude[0], (char *)szLatitude);

	return S_OK;
}

HRESULT SetupTelescopePosition(LPCTSTR szRaObjInfo, LPCTSTR szDecObjInfo)
{
	strcpy(&raObjInfo[0], (char *)szRaObjInfo);
	strcpy(&decObjInfo[0], (char *)szDecObjInfo);

	return S_OK;
}

HRESULT SetupGrabberInfo(LPCTSTR szGrabberName, LPCTSTR szVideoMode, float frameRate, long hardwareTimingCorrection)
{
	strcpy(&grabberName[0], (char *)szGrabberName);
	strcpy(&videoMode[0], (char *)szVideoMode);
	videoFrameRate = frameRate;
	HARDWARE_TIMING_CORRECTION = hardwareTimingCorrection;

	return S_OK;
}

HRESULT SetupCamera(
	long width, long height, LPCTSTR szCameraModel, long monochromeConversionMode, bool flipHorizontally, bool flipVertically, bool isIntegrating)
{
	IMAGE_WIDTH = width;
	IMAGE_HEIGHT = height;
	IMAGE_TOTAL_PIXELS = width * height;
	IMAGE_STRIDE = width * 3;

	MONOCHROME_CONVERSION_MODE = monochromeConversionMode;

	FLIP_VERTICALLY = flipVertically;
	FLIP_HORIZONTALLY = flipHorizontally;

	IS_INTEGRATING_CAMERA = isIntegrating;

	ClearResourses();

	if (NULL != prtPreviousDiffArea)
	{
		delete prtPreviousDiffArea;
		prtPreviousDiffArea = NULL;
	}
	prtPreviousDiffArea = (unsigned char*)malloc(IMAGE_TOTAL_PIXELS);
	::ZeroMemory(prtPreviousDiffArea, IMAGE_TOTAL_PIXELS);

	if (NULL != integratedPixels)
	{
		delete integratedPixels;
		integratedPixels = NULL;
	}
	integratedPixels = (double*)malloc(IMAGE_TOTAL_PIXELS * sizeof(double));
	::ZeroMemory(integratedPixels, IMAGE_TOTAL_PIXELS * sizeof(double));

	if (NULL != latestIntegratedFrame)
	{
		delete latestIntegratedFrame;
		latestIntegratedFrame = NULL;
	}
	latestIntegratedFrame = (unsigned char*)malloc(IMAGE_TOTAL_PIXELS);
	::ZeroMemory(latestIntegratedFrame, IMAGE_TOTAL_PIXELS);

	if (NULL != firstIntegratedFramePixels)
	{
		delete firstIntegratedFramePixels;
		firstIntegratedFramePixels = NULL;
	}
	firstIntegratedFramePixels = (unsigned char*)malloc(IMAGE_TOTAL_PIXELS);

	if (NULL != lastIntegratedFramePixels)
	{
		delete lastIntegratedFramePixels;
		lastIntegratedFramePixels = NULL;
	}
	lastIntegratedFramePixels = (unsigned char*)malloc(IMAGE_TOTAL_PIXELS);

	if (NULL != currTrackedFramePixels)
	{
		delete currTrackedFramePixels;
		currTrackedFramePixels = NULL;
	}
	currTrackedFramePixels = (unsigned char*)malloc(IMAGE_TOTAL_PIXELS);

	idxFrameNumber = 0;
	numberOfDiffSignaturesCalculated = 0;
	numberOfIntegratedFrames = 0;
	idxIntegratedFrameNumber = 0;
	droppedFramesSinceIntegrationIsLocked = 0;
	INTEGRATION_LOCKED = false;

	latestImageStatus.UniqueFrameNo = 0;

	strcpy(&cameraModel[0], (char *)szCameraModel);

	recording = false;

	return S_OK;
}

HRESULT SetupIntegrationDetection(float minDiffRatio, float minSignDiff, float diffGamma, bool forceNewFrameOnLockedRate)
{
	SyncLock::LockIntDet();

	SIGNATURE_DIFFERENCE_RATIO = minDiffRatio;
	MINIMUM_SIGNATURE_DIFFERENCE = minSignDiff;
    SetupDiffGammaMemoryTable(diffGamma);

	if (NULL != integrationChecker)
	{
		delete integrationChecker;
		integrationChecker = NULL;
	}

	integrationChecker = new OccuRec::IntegrationChecker(SIGNATURE_DIFFERENCE_RATIO, MINIMUM_SIGNATURE_DIFFERENCE);
	integrationChecker->ControlIntegrationDetectionTuning(INTEGRATION_DETECTION_TUNING);

	FORCE_NEW_FRAME_ON_LOCKED_RATE = forceNewFrameOnLockedRate;

	lockedIntegrationFrames = 0;

	SyncLock::UnlockIntDet();

#if _DEBUG
	DebugViewPrint(L"SetupIntegrationDetection(SIGNATURE_DIFFERENCE_RATIO = %.2f; MINIMUM_SIGNATURE_DIFFERENCE = %.2f; INTEGRATION_LOCKED = %d)\n", SIGNATURE_DIFFERENCE_RATIO, MINIMUM_SIGNATURE_DIFFERENCE, INTEGRATION_LOCKED); 
#endif

	return S_OK;
}

HRESULT GetCurrentImageStatus(ImageStatus* imageStatus)
{
	imageStatus->CountedFrames = latestImageStatus.CountedFrames;
	imageStatus->StartExposureFrameNo = latestImageStatus.StartExposureFrameNo;
	imageStatus->StartExposureTicks = latestImageStatus.StartExposureTicks;
	imageStatus->EndExposureFrameNo = latestImageStatus.EndExposureFrameNo;
	imageStatus->EndExposureTicks = latestImageStatus.EndExposureTicks;
	imageStatus->IntegratedFrameNo = latestImageStatus.IntegratedFrameNo;
	imageStatus->CutOffRatio = latestImageStatus.CutOffRatio;
	imageStatus->UniqueFrameNo = latestImageStatus.UniqueFrameNo;
	imageStatus->PerformedAction = latestImageStatus.PerformedAction;
	imageStatus->PerformedActionProgress = latestImageStatus.PerformedActionProgress;
	imageStatus->DetectedIntegrationRate = latestImageStatus.DetectedIntegrationRate;
	imageStatus->DropedFramesSinceIntegrationLock = latestImageStatus.DropedFramesSinceIntegrationLock;
	imageStatus->OcrWorking = latestImageStatus.OcrWorking;
	imageStatus->OcrErrorsSinceLastReset = latestImageStatus.OcrErrorsSinceLastReset;
	imageStatus->UserIntegratonRateHint = latestImageStatus.UserIntegratonRateHint;

	imageStatus->TrkdTargetXPos = latestImageStatus.TrkdTargetXPos;
	imageStatus->TrkdTargetYPos = latestImageStatus.TrkdTargetYPos;
	imageStatus->TrkdTargetIsTracked = latestImageStatus.TrkdTargetIsTracked;
	imageStatus->TrkdTargetMeasurement = latestImageStatus.TrkdTargetMeasurement;
	imageStatus->TrkdGuidingXPos = latestImageStatus.TrkdGuidingXPos;
	imageStatus->TrkdGuidingYPos = latestImageStatus.TrkdGuidingYPos;
	imageStatus->TrkdGuidingIsTracked = latestImageStatus.TrkdGuidingIsTracked;
	imageStatus->TrkdGuidingMeasurement = latestImageStatus.TrkdGuidingMeasurement;
	imageStatus->TrkdGuidingIsLocated = latestImageStatus.TrkdGuidingIsLocated;
	imageStatus->TrkdTargetIsLocated = latestImageStatus.TrkdTargetIsLocated;
	imageStatus->TrkdTargetHasSaturatedPixels = latestImageStatus.TrkdTargetHasSaturatedPixels;
	imageStatus->TrkdGuidingHasSaturatedPixels = latestImageStatus.TrkdGuidingHasSaturatedPixels;

	if (latestImageStatus.TrkdGuidingIsLocated)
	{
		memcpy(imageStatus->TrkdGuidingResiduals, latestImageStatus.TrkdGuidingResiduals, 289 * sizeof(double));
		memcpy(&imageStatus->TrkdGuidingPsfInfo, &latestImageStatus.TrkdGuidingPsfInfo, sizeof(NativePsfFitInfo));
	}
	if (latestImageStatus.TrkdTargetIsLocated)
	{
		memcpy(imageStatus->TrkdTargetResiduals, &latestImageStatus.TrkdTargetResiduals, 289 * sizeof(double));
		memcpy(&imageStatus->TrkdTargetPsfInfo, &latestImageStatus.TrkdTargetPsfInfo, sizeof(NativePsfFitInfo));
	}

	return S_OK;
}

HRESULT GetCurrentImage(BYTE* bitmapPixels)
{
	unsigned char* prtPixels = latestIntegratedFrame;

	// define the bitmap information header 
	BITMAPINFOHEADER bih;
	bih.biSize = sizeof(BITMAPINFOHEADER); 
	bih.biPlanes = 1; 
	bih.biBitCount = 24;                          // 24-bit 
	bih.biCompression = BI_RGB;                   // no compression 
	bih.biSizeImage = IMAGE_WIDTH * abs(IMAGE_HEIGHT) * 3;    // width * height * (RGB bytes) 
	bih.biXPelsPerMeter = 0; 
	bih.biYPelsPerMeter = 0; 
	bih.biClrUsed = 0; 
	bih.biClrImportant = 0; 
	bih.biWidth = IMAGE_WIDTH;                          // bitmap width 
	if (FLIP_VERTICALLY)
		bih.biHeight = -IMAGE_HEIGHT;                        // bitmap height 
	else
		bih.biHeight = IMAGE_HEIGHT;

	// and BitmapInfo variable-length UDT
	BYTE memBitmapInfo[40];
	RtlMoveMemory(memBitmapInfo, &bih, sizeof(bih));

	BITMAPFILEHEADER bfh;
	bfh.bfType=19778;    //BM header
	bfh.bfSize=55 + bih.biSizeImage;
	bfh.bfReserved1=0;
	bfh.bfReserved2=0;
	bfh.bfOffBits=sizeof(BITMAPINFOHEADER) + sizeof(BITMAPFILEHEADER); //54

	// Copy the display bitmap including the header
	RtlMoveMemory(bitmapPixels, &bfh, sizeof(bfh));
	RtlMoveMemory(bitmapPixels + sizeof(bfh), &memBitmapInfo, sizeof(memBitmapInfo));

	bitmapPixels = bitmapPixels + sizeof(bfh) + sizeof(memBitmapInfo);

	long currLinePos = 0;
	int length = IMAGE_WIDTH * IMAGE_HEIGHT;
	
	bitmapPixels += 3 * (length + IMAGE_WIDTH);

	if (FLIP_HORIZONTALLY)
		bitmapPixels -= 3 * IMAGE_WIDTH + 3;

	int total = IMAGE_WIDTH * IMAGE_HEIGHT;
	while(total--)
	{
		if (currLinePos == 0) 
		{
			currLinePos = IMAGE_WIDTH;

			if (!FLIP_HORIZONTALLY)
				bitmapPixels -= 6 * IMAGE_WIDTH;
		};

		unsigned char val = *prtPixels;
		prtPixels++;

		BYTE btVal = (BYTE)(val & 0xFF);
		
		*bitmapPixels = btVal;
		*(bitmapPixels + 1) = btVal;
		*(bitmapPixels + 2) = btVal;

		if (FLIP_HORIZONTALLY)
			bitmapPixels-=3; 
		else
			bitmapPixels+=3;

		currLinePos--;
	}

	return S_OK;
}

void HandleCalibrationBeforeSignatureCalc()
{
	if (INTEGRATION_CALIBRATION)
	{
		if (INTEGRATION_CALIBRATION_PASSES <= INTEGRATION_CALIBRATION_TOTAL_PASSES)
		{
			if (INTEGRATION_CALIBRATION_PASSES % INTEGRATION_CALIBRATION_CYCLES == 0)
			{
				// GammaDiff should change 
				float newGamma = GAMMA_PROBES[(INTEGRATION_CALIBRATION_PASSES / INTEGRATION_CALIBRATION_CYCLES) - 1];
				SetupDiffGammaMemoryTable(newGamma);
			}

			if (INTEGRATION_CALIBRATION_PASSES == INTEGRATION_CALIBRATION_TOTAL_PASSES)
				SetupDiffGammaMemoryTable(1);			
		}
	}
}

void HandleCalibrationAfterSignatureCalc(float signature)
{
	if (INTEGRATION_CALIBRATION)
	{
		if (INTEGRATION_CALIBRATION_PASSES <= INTEGRATION_CALIBRATION_TOTAL_PASSES)
		{
			CALIBRATION_SIGNATURES[INTEGRATION_CALIBRATION_PASSES] = signature;
			INTEGRATION_CALIBRATION_PASSES++;
		}
	}
}

void CalculateDiffSignature(unsigned char* bmpBits, float* signatureThisPrev)
{
	HandleCalibrationBeforeSignatureCalc();

	numberOfDiffSignaturesCalculated++;

	unsigned char* ptrBuf = bmpBits + IMAGE_STRIDE * (IMAGE_HEIGHT / 2 - 1) + (3 * (IMAGE_WIDTH / 2));

	unsigned char* ptrPrevPixels;
	unsigned char* ptrThisPixels;

	switch(numberOfDiffSignaturesCalculated % 2)
	{
		case 0:
			ptrPrevPixels		  = prtPreviousDiffArea;
			ptrThisPixels		  = prtPreviousDiffArea + 1 * IMAGE_WIDTH * 3;
			break;

		case 1:
			ptrThisPixels		  = prtPreviousDiffArea;
			ptrPrevPixels		  = prtPreviousDiffArea + 1 * IMAGE_WIDTH * 3;
			break;
	}

	*signatureThisPrev = 0;

	for(int y = 0; y < 32; y++)
	{
		for(int x = 0; x < 32; x++)
		{
			*ptrThisPixels = *ptrBuf;

			if (USES_DIFF_GAMMA)
				*signatureThisPrev += abs((float)GAMMA[*ptrThisPixels] - (float)GAMMA[*ptrPrevPixels]) / 2.0;
			else
				*signatureThisPrev += abs((float)*ptrThisPixels - (float)*ptrPrevPixels) / 2.0;

			ptrThisPixels++;
			ptrPrevPixels++;
			ptrBuf+=3;
		}

		ptrBuf+= IMAGE_STRIDE - (32 * 3);
	}

	*signatureThisPrev = *signatureThisPrev / 1024.0;

	HandleCalibrationAfterSignatureCalc(*signatureThisPrev);
}

void CalculateDiffSignature2(long* pixels, float* signatureThisPrev)
{
	HandleCalibrationBeforeSignatureCalc();

	numberOfDiffSignaturesCalculated++;

	long* ptrLongBuf = pixels + (IMAGE_WIDTH * (IMAGE_HEIGHT / 2 - 1) + (3 * (IMAGE_WIDTH / 2)));

	unsigned char* ptrPrevPixels;
	unsigned char* ptrThisPixels;

	switch(numberOfDiffSignaturesCalculated % 2)
	{
		case 0:
			ptrPrevPixels		  = prtPreviousDiffArea;
			ptrThisPixels		  = prtPreviousDiffArea + 1 * IMAGE_WIDTH * 3;
			break;

		case 1:
			ptrThisPixels		  = prtPreviousDiffArea;
			ptrPrevPixels		  = prtPreviousDiffArea + 1 * IMAGE_WIDTH * 3;
			break;
	}

	*signatureThisPrev = 0;

	for(int y = 0; y < 32; y++)
	{
		for(int x = 0; x < 32; x++)
		{
			*ptrThisPixels = *ptrLongBuf & 0xFF;

			if (USES_DIFF_GAMMA)
				*signatureThisPrev += abs((float)GAMMA[*ptrThisPixels] - (float)GAMMA[*ptrPrevPixels]) / 2.0;
			else
				*signatureThisPrev += abs((float)*ptrThisPixels - (float)*ptrPrevPixels) / 2.0;

			ptrThisPixels++;
			ptrPrevPixels++;
			ptrLongBuf+=3;
		}
	}

	*signatureThisPrev = *signatureThisPrev / 1024.0;

	HandleCalibrationAfterSignatureCalc(*signatureThisPrev);
}

long detectedIntegrationRate = 0;

long BufferNewIntegratedFrame(bool isNewIntegrationPeriod, __int64 currentUtcDayAsTicks, __int64 currentNtpTimeAsTicks,  __int64 currentSecondaryTimeAsTicks, double ntpBasedTimeError, float cameraGain, float cameraGamma, float temperature, char* cameraExposure)
{
	long numItems = 0;

	if (isNewIntegrationPeriod)
	{
		idxIntegratedFrameNumber++;

		detectedIntegrationRate = numberOfIntegratedFrames;

		if (!INTEGRATION_LOCKED)
			lockedIntegrationFrames = detectedIntegrationRate;
		else if (INTEGRATION_LOCKED && lockedIntegrationFrames > 1)
		{
			if (lockedIntegrationFrames != detectedIntegrationRate)
			{
				droppedFramesSinceIntegrationIsLocked += abs(lockedIntegrationFrames - detectedIntegrationRate);
				DebugViewPrint(L"Detected Dropped Frames= %d (LOCKED %d)", detectedIntegrationRate, lockedIntegrationFrames);
			}			
		}
		else
		{
			// No dropped frames when the locked integration is x1
		}
	}

	if (numberOfIntegratedFrames == 0)
	{
		::ZeroMemory(latestIntegratedFrame, IMAGE_TOTAL_PIXELS);
	}
	else
	{
		unsigned char almanacUpdateSatus = 0;
		unsigned char gpsFixStatus = 0;
		unsigned char trackedSatellitesCount = 0;

		bool integratedFrameCouldBeRecorded = recording || OCR_FAILED_TEST_RECORDING || RECORD_ONLY_STATUS_CHANNEL_WITH_OCRED_TIMESTAMPS;

		IntegratedFrame* frame = integratedFrameCouldBeRecorded 
			? new IntegratedFrame(IMAGE_TOTAL_PIXELS, AAV_16) 
			: NULL;
		
		if (AAV_16 && AAV16_MAX_BINNED_FRAMES < numberOfIntegratedFrames)
			AAV16_MAX_BINNED_FRAMES = numberOfIntegratedFrames;

		double* ptrPixels = integratedPixels;
		unsigned char* singleRawFramePixles = NULL;
		if (OCR_FAILED_TEST_RECORDING) singleRawFramePixles = firstIntegratedFramePixels; // We always run OCR testing in locked x1 integration mode

		unsigned char* ptr8BitPixels = latestIntegratedFrame;

		unsigned char* ptrFramePixels = integratedFrameCouldBeRecorded ? frame->Pixels : NULL;
		unsigned short* ptrFramePixels16 = integratedFrameCouldBeRecorded ? frame->Pixels16 : NULL;

		bool runOCR = false;
		if (OCR_IS_SETUP && OCR_ZONE_MATRIX && NULL != firstFrameOcrProcessor)
		{
			firstFrameOcrProcessor->NewFrame();
			lastFrameOcrProcessor->NewFrame();
			runOCR = true;
		}

		int restoredPixels = 0;

		//DebugViewPrint(L"NEW FRAME");
		for (long i=0; i < IMAGE_TOTAL_PIXELS; i++)
		{
			long pixX = i % IMAGE_WIDTH;
			long pixY = i / IMAGE_WIDTH; 

			if (runOCR && i >= MEDIAN_CALC_INDEX_FROM && i <= MEDIAN_CALC_INDEX_TO)
			{
				firstFrameOcrProcessor->AddMedianComputationPixel(firstIntegratedFramePixels[i]);
				lastFrameOcrProcessor->AddMedianComputationPixel(detectedIntegrationRate > 1 ? lastIntegratedFramePixels[i] : firstIntegratedFramePixels[i]);
			}

			if (runOCR)
			{
				long packedInfo = OCR_ZONE_MATRIX[i];
				if (packedInfo != 0)
				{
					firstFrameOcrProcessor->ProcessZonePixel(packedInfo, pixX, pixY, firstIntegratedFramePixels[i]);
					lastFrameOcrProcessor->ProcessZonePixel(packedInfo,  pixX, pixY, detectedIntegrationRate > 1 ? lastIntegratedFramePixels[i] : firstIntegratedFramePixels[i]);
				}
			}

			// NOTE: Possible way to speed this up (which may not be necessary) would be to build a very large memory table
			//       that will return directly the average value for the given numberOfIntegratedFrames and 
			//       *ptrPixels (which is of type double but the value is integer from 0 to 255 * numberOfIntegratedFrames)
			long averageValue = (long)(*ptrPixels / (INTEGRATION_LOCKED ? numberOfIntegratedFrames : 1));
			unsigned short pixel16 = AAV_16 ? (unsigned short)(*ptrPixels) : 0;

			if (OCR_FAILED_TEST_RECORDING) 
				// In OCR testing mode always read the raw pixel from the actual frame
				averageValue = *singleRawFramePixles;

			if (averageValue <= 0)
			{
				*ptr8BitPixels = 0;
				averageValue = 0;
			}
			else if (averageValue >= 255)
			{
				*ptr8BitPixels = 255;
				averageValue = 255;
			}

			*ptr8BitPixels = averageValue;
			if (integratedFrameCouldBeRecorded)
			{
				if (AAV_16)
					*ptrFramePixels16 = pixel16;
				else
					*ptrFramePixels = averageValue;
			}

			if ((detectedIntegrationRate > 1 || NO_INTEGRATION_STACK_RATE > 0)  && OCR_PRESERVE_VTI)
			{
				// Only preserve timestamps from different frames IF the integration is bigger than 1 frame
				if (pixY >= OCR_FRAME_TOP_ODD && pixY < OCR_FRAME_TOP_EVEN + 2 * OCR_CHAR_FIELD_HEIGHT)
				{
					restoredPixels++;

					// Preserve the timestamp pixels from the first and last integrated frame in the final image
					if (pixY % 2 == 0)
					{
						if (integratedFrameCouldBeRecorded)
						{
							if (AAV_16)
								*ptrFramePixels16 = numberOfIntegratedFrames * firstIntegratedFramePixels[pixY * IMAGE_WIDTH + pixX];
							else
								*ptrFramePixels = firstIntegratedFramePixels[pixY * IMAGE_WIDTH + pixX];
						}

						*ptr8BitPixels = firstIntegratedFramePixels[pixY * IMAGE_WIDTH + pixX];
					}
					else
					{
						if (integratedFrameCouldBeRecorded)
						{
							if (AAV_16)
								*ptrFramePixels16 = numberOfIntegratedFrames * lastIntegratedFramePixels[pixY * IMAGE_WIDTH + pixX];
							else
								*ptrFramePixels = lastIntegratedFramePixels[pixY * IMAGE_WIDTH + pixX];
						}

						*ptr8BitPixels = lastIntegratedFramePixels[pixY * IMAGE_WIDTH + pixX];
					}
				}
			}

			ptr8BitPixels++;

			if (integratedFrameCouldBeRecorded)
			{
				if (AAV_16)
					ptrFramePixels16++;
				else
					ptrFramePixels++;
			}

			ptrPixels++;
			if (OCR_FAILED_TEST_RECORDING) singleRawFramePixles ++;
		}

		bool hasOcrErors = false;
		OcrErrorCode firstErrorCode = OcrErrorCode::Unknown;
		OcrErrorCode secondErrorCode = OcrErrorCode::Unknown;

		if (runOCR)
		{

			firstFrameOcrProcessor->Ocr(currentUtcDayAsTicks);
			lastFrameOcrProcessor->Ocr(currentUtcDayAsTicks);

			// If OCR is enabled but we haven'd had a single successfully OCRed frame then don't count errors
			// Once we have had at least one successfully OCRed frame - start counting errors. Use the first OCRed frame to determine which field is first - even or odd
			// Be mindful that when the integration is not Locked - the first and last frames may not have been picked correctly
			// Implement an OCR timestamp checker and fixer (similar to the managed one) which can also report incorrect timestamps

			if (!ocrFirstFrameProcessed)
			{
				if ((INTEGRATION_LOCKED && firstFrameOcrProcessor->Success() && lastFrameOcrProcessor->Success()))
				{
					ocrManager->RegisterFirstSuccessfullyOcredFrame(firstFrameOcrProcessor);
					ocrFirstFrameProcessed = true;

					if (INTEGRATION_DETECTION_TUNING)
					{
						char tsFrom[128];
						char tsTo[128];
						firstFrameOcrProcessor->GetOcredStartFrameTimeStampStr(&tsFrom[0]);
						firstFrameOcrProcessor->GetOcredEndFrameTimeStampStr(&tsTo[0]);
						std::wstring wcFrom( 256, L'#' );
						mbstowcs( &wcFrom[0], tsFrom, 128 );
						std::wstring wcTo( 256, L'#' );
						mbstowcs( &wcTo[0], tsTo, 128 );
						DebugViewPrint(L"%lld: FieldDurationInTicks=%d (LOCKED*first* frame from %d to %d TS from %s to %s)", idxFrameNumber, ocrManager->FieldDurationInTicks, firstFrameOcrProcessor->GetOcredStartFrameNumber(), firstFrameOcrProcessor->GetOcredEndFrameNumber(), &wcFrom[0], &wcTo[0]);
					}
				}
				else if (!INTEGRATION_LOCKED && lastFrameOcrProcessor->Success())
				{
					ocrManager->RegisterFirstSuccessfullyOcredFrame(lastFrameOcrProcessor);
					ocrFirstFrameProcessed = true;
					if (INTEGRATION_DETECTION_TUNING)
					{
						char tsFrom[128];
						char tsTo[128];
						firstFrameOcrProcessor->GetOcredStartFrameTimeStampStr(&tsFrom[0]);
						firstFrameOcrProcessor->GetOcredEndFrameTimeStampStr(&tsTo[0]);
						std::wstring wcFrom( 256, L'#' );
						mbstowcs( &wcFrom[0], tsFrom, 128 );
						std::wstring wcTo( 256, L'#' );
						mbstowcs( &wcTo[0], tsTo, 128 );
						DebugViewPrint(L"%lld: FieldDurationInTicks=%d (UNLOCKED*last* frame from %d to %d from %s to %s)", idxFrameNumber, ocrManager->FieldDurationInTicks, lastFrameOcrProcessor->GetOcredStartFrameNumber(), lastFrameOcrProcessor->GetOcredEndFrameNumber(), &wcFrom[0], &wcTo[0]);
					}
				}
			}
			
			if (ocrFirstFrameProcessed)
			{
				if (INTEGRATION_LOCKED)
				{
					ocrManager->ProcessedOcredFramesInLockedMode(firstFrameOcrProcessor, lastFrameOcrProcessor);

					if (firstFrameOcrProcessor->Success() && lastFrameOcrProcessor->Success())
					{
						idxFirstFrameNumber = firstFrameOcrProcessor->GetOcredStartFrameNumber();
						idxLastFrameNumber = lastFrameOcrProcessor->GetOcredEndFrameNumber();
						idxFirstFrameTimestamp = firstFrameOcrProcessor->GetOcredStartFrameTimeStamp(ocrManager->FieldDurationInTicks);
						idxLastFrameTimestamp = lastFrameOcrProcessor->GetOcredEndFrameTimeStamp();

						almanacUpdateSatus = lastFrameOcrProcessor->GetOcredAlmanacUpdateState();
						gpsFixStatus = lastFrameOcrProcessor->GetOcredGpsFixState();
						trackedSatellitesCount = lastFrameOcrProcessor->GetOcredTrackedSatellitesCount();
					}
					else
					{
						firstErrorCode = firstFrameOcrProcessor->IsOddFieldDataFirst() ? firstFrameOcrProcessor->ErrorCodeOddField : firstFrameOcrProcessor->ErrorCodeEvenField;
						secondErrorCode = lastFrameOcrProcessor->IsOddFieldDataFirst() ? lastFrameOcrProcessor->ErrorCodeEvenField : lastFrameOcrProcessor->ErrorCodeOddField;
					}

					firstFrameOcrProcessor->GetOcredStartFrameTimeStampStr(&firstFrameTimestampStr[0]);
					lastFrameOcrProcessor->GetOcredEndFrameTimeStampStr(&endFrameTimestampStr[0]);
				}
				else
				{
					OccuOcr::OcrFrameProcessor* timeStampFrameProcessor = lastFrameWasNewIntegrationPeriod ? firstFrameOcrProcessor : lastFrameOcrProcessor;

					ocrManager->ProcessedOcredFrame(timeStampFrameProcessor);

					if (timeStampFrameProcessor->Success())
					{
						idxFirstFrameNumber = timeStampFrameProcessor->GetOcredStartFrameNumber();
						idxLastFrameNumber = timeStampFrameProcessor->GetOcredEndFrameNumber();
						idxFirstFrameTimestamp = timeStampFrameProcessor->GetOcredStartFrameTimeStamp(ocrManager->FieldDurationInTicks);
						idxLastFrameTimestamp = timeStampFrameProcessor->GetOcredEndFrameTimeStamp();

						almanacUpdateSatus = timeStampFrameProcessor->GetOcredAlmanacUpdateState();
						gpsFixStatus = timeStampFrameProcessor->GetOcredGpsFixType();
						trackedSatellitesCount = timeStampFrameProcessor->GetOcredTrackedSatellitesCount();
					}
					else
					{
						firstErrorCode = timeStampFrameProcessor->IsOddFieldDataFirst() ? timeStampFrameProcessor->ErrorCodeOddField : timeStampFrameProcessor->ErrorCodeEvenField;
						secondErrorCode = timeStampFrameProcessor->IsOddFieldDataFirst() ? timeStampFrameProcessor->ErrorCodeEvenField : timeStampFrameProcessor->ErrorCodeOddField;
					}

					timeStampFrameProcessor->GetOcredStartFrameTimeStampStr(&firstFrameTimestampStr[0]);
					timeStampFrameProcessor->GetOcredEndFrameTimeStampStr(&endFrameTimestampStr[0]);
				}
			}

			hasOcrErors = ocrManager->OcrErrorsSinceReset > ocrErrorsSiceLastReset;
			ocrErrorsSiceLastReset = ocrManager->OcrErrorsSinceReset;
		}

		if (integratedFrameCouldBeRecorded)
		{
			if (AAV_16)
			{
				unsigned short maxValue = numberOfIntegratedFrames < 255 ? numberOfIntegratedFrames * 255 : 0xFFFF;
			
				if (INTEGRATION_LOCKED && numberOfIntegratedFrames > 1)
				{
					// Marks for "summed" frame and "mixed timestamp"
					frame->Pixels16[0] = maxValue;
					frame->Pixels16[1] = 0;
					frame->Pixels16[2] = maxValue;
					frame->Pixels16[IMAGE_WIDTH] = 0;
					frame->Pixels16[IMAGE_WIDTH + 2] = 0;
					frame->Pixels16[2 * IMAGE_WIDTH] = maxValue;
					frame->Pixels16[2 * IMAGE_WIDTH + 1] = 0;
					frame->Pixels16[2 * IMAGE_WIDTH + 2] = maxValue;
				}
				if (restoredPixels > 0) 
					frame->Pixels16[IMAGE_WIDTH + 1] = maxValue;
			}
			else
			{
				if (INTEGRATION_LOCKED && numberOfIntegratedFrames > 1)
				{
					// Marks for "summed" frame and "mixed timestamp"
					frame->Pixels[0] = 255;
					frame->Pixels[1] = 0;
					frame->Pixels[2] = 255;
					frame->Pixels[IMAGE_WIDTH] = 0;
					frame->Pixels[IMAGE_WIDTH + 2] = 0;
					frame->Pixels[2 * IMAGE_WIDTH] = 255;
					frame->Pixels[2 * IMAGE_WIDTH + 1] = 0;
					frame->Pixels[2 * IMAGE_WIDTH + 2] = 255;
				}
				if (restoredPixels > 0) 
					frame->Pixels[IMAGE_WIDTH + 1] = 255;
			}
		}

		latestImageStatus.CountedFrames = numberOfIntegratedFrames;
		latestImageStatus.CutOffRatio = 0; // NULL != integrationChecker ? integrationChecker->NewIntegrationPeriodCutOffRatio : 0;
		latestImageStatus.IntegratedFrameNo = idxIntegratedFrameNumber;
		latestImageStatus.StartExposureFrameNo = idxFirstFrameNumber;
		latestImageStatus.StartExposureTicks = idxFirstFrameTimestamp;
		latestImageStatus.EndExposureFrameNo = idxLastFrameNumber;
		latestImageStatus.EndExposureTicks = idxLastFrameTimestamp;
		latestImageStatus.DetectedIntegrationRate = lockedIntegrationFrames == 1 ? 1 : detectedIntegrationRate;
		latestImageStatus.DropedFramesSinceIntegrationLock = droppedFramesSinceIntegrationIsLocked;
		latestImageStatus.OcrWorking = (NULL != ocrManager && ocrManager->IsReceivingTimeStamps()) ? 1 : 0;
		latestImageStatus.OcrErrorsSinceLastReset = ocrErrorsSiceLastReset;
		latestImageStatus.UserIntegratonRateHint = MANUAL_INTEGRATION_RATE > 0 ? MANUAL_INTEGRATION_RATE : 0;

		if (INTEGRATION_CALIBRATION)
		{
			latestImageStatus.PerformedAction = 1;
			if (INTEGRATION_CALIBRATION_PASSES >= INTEGRATION_CALIBRATION_TOTAL_PASSES)
				latestImageStatus.PerformedActionProgress = 1;
			else
				latestImageStatus.PerformedActionProgress = 1.0 * INTEGRATION_CALIBRATION_PASSES / INTEGRATION_CALIBRATION_TOTAL_PASSES;
		}
		else if (NULL != integrationChecker)
		{
			latestImageStatus.PerformedAction = integrationChecker->PerformedAction;
			latestImageStatus.PerformedActionProgress = integrationChecker->PerformedActionProgress;
		}
		else
		{
			latestImageStatus.PerformedAction = 0;
			latestImageStatus.PerformedActionProgress = 0;
		}

		latestImageStatus.UniqueFrameNo++;

		bool recordNtpDebugFrame = false;

		if (RECORD_ONLY_STATUS_CHANNEL_WITH_OCRED_TIMESTAMPS)
		{
			if (NTP_DEBUG_REC_FREQ == 1)
			{
				recordNtpDebugFrame = true;
			}
			else if (numRecordedFrames < 25 * 60)
			{
				// If we haven't recorded 1 min of frames yet (25 * 60) then keep recording
				double deltaTicksMS = abs(idxFirstFrameTimestamp - firstFrameNtpTimestamp) / 100000.0;
				aggregatedNtpDebug += deltaTicksMS;

				recordNtpDebugFrame = true;
			}
			else
			{
				if (averageNtpDebugOffsetMS == 0)
					// If we have recorded 1 min of frames but haven't determined the average NTP shift then do this now
					averageNtpDebugOffsetMS = aggregatedNtpDebug / numRecordedFrames;
				
				double deltaTicksMS = abs(idxFirstFrameTimestamp - firstFrameNtpTimestamp) / 100000.0;

				if (latestImageStatus.UniqueFrameNo % NTP_DEBUG_REC_FREQ == 0)
					// If we have recorded 1 min of frames and this is the NTP_DEBUG_REC_FREQ-th unique frame then record it
					recordNtpDebugFrame = true;

				if (abs(averageNtpDebugOffsetMS - deltaTicksMS) > NTP_DEBUG_MAX_IGNORED_SHIFT)
					// If we have recorded 1 min of frames and the diff between average and current NTP offset is more than NTP_DEBUG_MAX_IGNORED_SHIFT then record the frame
					recordNtpDebugFrame = true;
			}			
		}

		if (recording && integratedFrameCouldBeRecorded && (!OCR_FAILED_TEST_RECORDING || hasOcrErors) && (!RECORD_ONLY_STATUS_CHANNEL_WITH_OCRED_TIMESTAMPS || recordNtpDebugFrame))
		{
			// Record every buffered frame in standard mode or only record frames with OCR errors when running OCR testing
			if (NULL != frame)
			{
				frame->NumberOfIntegratedFrames = numberOfIntegratedFrames;
				frame->StartFrameId = idxFirstFrameNumber;
				frame->EndFrameId = idxLastFrameNumber;
				frame->StartTimeStamp = idxFirstFrameTimestamp;
				frame->EndTimeStamp = idxLastFrameTimestamp;
				frame->FrameNumber = idxIntegratedFrameNumber;
				frame->GpsTrackedSatellites = trackedSatellitesCount;
				frame->GpsAlamancStatus = almanacUpdateSatus;
				frame->GpsFixStatus = gpsFixStatus;
				frame->NTPStartTimestamp = firstFrameNtpTimestamp;
				frame->NTPEndTimestamp = lastFrameNtpTimestamp;
				frame->SecondaryStartTimestamp = firstFrameSecondaryTimestamp;
				frame->SecondaryEndTimestamp = lastFrameSecondaryTimestamp;
				frame->NTPTimestampError = (long)(0.5 + ntpBasedTimeError * 10);
				frame->Gain = cameraGain;
				frame->Gamma = cameraGamma;
				frame->Temperature = temperature;
				if (cameraExposure)
					strcpy(&frame->Exposure[0], cameraExposure);
				else
					frame->Exposure[0] = 0;

				if (OCR_FAILED_TEST_RECORDING && hasOcrErors)
					sprintf(&frame->OcrErrorMessageStr[0], "FirstFieldError: %d; LastFieldError: %d", (long)firstErrorCode, (long)secondErrorCode);

				strcpy(&frame->StartTimeStampStr[0], &firstFrameTimestampStr[0]);
				strcpy(&frame->EndTimeStampStr[0], &endFrameTimestampStr[0]);


				numItems = AddFrameToRecordingBuffer(frame);

				numRecordedFrames++;
			}
		}
		else if (NULL != frame)
		{
			delete frame;
		}

		return numItems;
	}
}

void HandleTracking(unsigned char* pixelsChar, long* pixels)
{
	if (RUN_TRACKING && 
		idxFrameNumber % TRACKING_FREQUENCY == 0 &&
		!trackedThisIntegrationPeriod)
	{
		// Run the tracking
		if (NULL != pixelsChar)
			TrackerNextFrame_int8(idxFrameNumber, pixelsChar);
		else
			TrackerNextFrame(idxFrameNumber, (unsigned long*)pixels);

		// Update the status fields

		NativeTrackedObjectInfo trackingInfo;
		double residuals[1024];
		float totalReading;
		float totalPixels;
		bool hasSaturatedPixels;

		if (TRACKED_TARGET_ID > -1)
		{
			TrackerGetTargetState(0, &trackingInfo, &latestImageStatus.TrkdTargetPsfInfo, &latestImageStatus.TrkdTargetResiduals[0]);

			latestImageStatus.TrkdTargetIsLocated = trackingInfo.IsLocated;
			latestImageStatus.TrkdTargetXPos = trackingInfo.CenterXDouble;
			latestImageStatus.TrkdTargetYPos = trackingInfo.CenterYDouble;
			latestImageStatus.TrkdTargetIsTracked = 1;

			if (NULL != pixelsChar)
				totalReading = MeasureObjectUsingAperturePhotometry_int8(pixelsChar, TRACKED_TARGET_APERTURE, IMAGE_WIDTH, IMAGE_HEIGHT, trackingInfo.CenterXDouble, trackingInfo.CenterYDouble, SATURATION_8BIT, TRACKING_BG_INNER_RADIUS, TRACKING_BG_MIN_NUM_PIXELS, &totalPixels, &hasSaturatedPixels);
			else
				totalReading = MeasureObjectUsingAperturePhotometry((unsigned long*)pixels, TRACKED_TARGET_APERTURE, IMAGE_WIDTH, IMAGE_HEIGHT, trackingInfo.CenterXDouble, trackingInfo.CenterYDouble, SATURATION_8BIT, TRACKING_BG_INNER_RADIUS, TRACKING_BG_MIN_NUM_PIXELS, &totalPixels, &hasSaturatedPixels);
			
			latestImageStatus.TrkdTargetMeasurement = totalReading;
			latestImageStatus.TrkdTargetHasSaturatedPixels = hasSaturatedPixels ? 1 : 0;			
		}
		
		if (TRACKED_GUIDING_ID > -1)
		{
			TrackerGetTargetState(TRACKED_GUIDING_ID, &trackingInfo, &latestImageStatus.TrkdGuidingPsfInfo, &latestImageStatus.TrkdGuidingResiduals[0]);

			latestImageStatus.TrkdGuidingIsLocated = trackingInfo.IsLocated;
			latestImageStatus.TrkdGuidingXPos = trackingInfo.CenterXDouble;
			latestImageStatus.TrkdGuidingYPos = trackingInfo.CenterYDouble;
			latestImageStatus.TrkdGuidingIsTracked = 1;

			if (NULL != pixelsChar)
				totalReading = MeasureObjectUsingAperturePhotometry_int8(pixelsChar, TRACKED_GUIDING_APERTURE, IMAGE_WIDTH, IMAGE_HEIGHT, trackingInfo.CenterXDouble, trackingInfo.CenterYDouble, SATURATION_8BIT, TRACKING_BG_INNER_RADIUS, TRACKING_BG_MIN_NUM_PIXELS, &totalPixels, &hasSaturatedPixels);
			else
				totalReading = MeasureObjectUsingAperturePhotometry((unsigned long*)pixels, TRACKED_GUIDING_APERTURE, IMAGE_WIDTH, IMAGE_HEIGHT, trackingInfo.CenterXDouble, trackingInfo.CenterYDouble, SATURATION_8BIT, TRACKING_BG_INNER_RADIUS, TRACKING_BG_MIN_NUM_PIXELS, &totalPixels, &hasSaturatedPixels);
			
			latestImageStatus.TrkdGuidingMeasurement = totalReading;
			latestImageStatus.TrkdGuidingHasSaturatedPixels = hasSaturatedPixels ? 1 : 0;
		}
		
		trackedThisIntegrationPeriod = INTEGRATION_LOCKED;
	}
	else
	{
		// This is how we tell OccuRec that there is no tracking info in the ImageStatus
		latestImageStatus.TrkdTargetIsTracked = 0;
		latestImageStatus.TrkdGuidingIsTracked = 0;
	}
}

HRESULT ProcessVideoFrame2(long* pixels, __int64 currentUtcDayAsTicks, __int64 currentNtpTimeAsTicks, double ntpBasedTimeError,  __int64 currentSecondaryTimeAsTicks, float cameraGain, float cameraGamma, float temperature, char* cameraExposure, FrameProcessingStatus* frameInfo)
{
	frameInfo->FrameDiffSignature = 0;

	float diffSignature;

	CalculateDiffSignature2(pixels, &diffSignature);

	idxFrameNumber++;

	bool isNewIntegrationPeriod = IsNewIntegrationPeriod(diffSignature, INTEGRATION_LOCKED && numberOfIntegratedFrames == lockedIntegrationFrames);

	// After the integration has been 'locked' we only output a frame when a new integration period has been detected
	// When the integration hasn't been 'locked' we output every frame received from the camera
	bool showOutputFrame = idxFrameNumber > STARTUP_FRAMES_WITH_NO_OUTPUT && (isNewIntegrationPeriod || !INTEGRATION_LOCKED);

	if (showOutputFrame)
	{
		BufferNewIntegratedFrame(isNewIntegrationPeriod, currentUtcDayAsTicks, currentNtpTimeAsTicks, currentSecondaryTimeAsTicks, ntpBasedTimeError, cameraGain, cameraGamma, temperature, cameraExposure);
		::ZeroMemory(integratedPixels, IMAGE_TOTAL_PIXELS * sizeof(double));

		if (isNewIntegrationPeriod)
		{
			numberOfIntegratedFrames = 0;

			idxFirstFrameNumber = idxFrameNumber;
			idxLastFrameNumber = 0;
			firstFrameNtpTimestamp = currentNtpTimeAsTicks - VIDEO_FRAME_IN_WINDOWS_TICKS; // The frame started one frame ago
			firstFrameSecondaryTimestamp = currentSecondaryTimeAsTicks - VIDEO_FRAME_IN_WINDOWS_TICKS; // The frame started one frame ago
		}
	}

	lastFrameNtpTimestamp = currentNtpTimeAsTicks;
	lastFrameSecondaryTimestamp = currentSecondaryTimeAsTicks;

	frameInfo->FrameDiffSignature  = diffSignature;
	//frameInfo->CurrentSignatureRatio  =  NULL != integrationChecker ? integrationChecker->CurrentSignatureRatio : 0;

	long stride = 3 * IMAGE_WIDTH;
	long* ptrPixelItt = pixels;

	double* ptrPixels = integratedPixels;

	unsigned char* ptrFirstOrLastFrameCopy = NULL;

	if (isNewIntegrationPeriod)
	{
		ptrFirstOrLastFrameCopy = firstIntegratedFramePixels;
		lastFrameWasNewIntegrationPeriod = true;
	}
	else
	{
		ptrFirstOrLastFrameCopy = lastIntegratedFramePixels;
		lastFrameWasNewIntegrationPeriod = false;
	}	

	for (int y = 0; y < IMAGE_HEIGHT; y++)
	{
		for (int x = 0; x < IMAGE_WIDTH; x++)
		{
			unsigned char thisPixel = *ptrPixelItt & 0xFF;

			// Saving the first/last frame raw pixels for OCR-ing
			*ptrFirstOrLastFrameCopy = thisPixel;
		    *ptrPixels += thisPixel;

			ptrPixels++;
			ptrFirstOrLastFrameCopy++;
			ptrPixelItt++;
		}
	}

	numberOfIntegratedFrames++;

	idxLastFrameNumber = idxFrameNumber;
	//idxLastFrameTimestamp = currentUtcDayAsTicks;

	frameInfo->CameraFrameNo = idxFrameNumber;
	frameInfo->IntegratedFrameNo = idxIntegratedFrameNumber;
	frameInfo->IntegratedFramesSoFar = numberOfIntegratedFrames;

	HandleTracking(NULL, pixels);

	if (lastFrameWasNewIntegrationPeriod)
		trackedThisIntegrationPeriod = false;
	return S_OK;
}

void ProcessRawFrame(RawFrame* rawFrame)
{
	float diffSignature;

	CalculateDiffSignature(rawFrame->BmpBits, &diffSignature);

	idxFrameNumber++;

	bool isNewIntegrationPeriod = IsNewIntegrationPeriod(diffSignature, INTEGRATION_LOCKED && numberOfIntegratedFrames == lockedIntegrationFrames);

	// After the integration has been 'locked' we only output a frame when a new integration period has been detected
	// When the integration hasn't been 'locked' we output every frame received from the camera
	bool showOutputFrame = idxFrameNumber > STARTUP_FRAMES_WITH_NO_OUTPUT && (isNewIntegrationPeriod || !INTEGRATION_LOCKED);

	if (showOutputFrame)
	{
		BufferNewIntegratedFrame(isNewIntegrationPeriod, rawFrame->CurrentUtcDayAsTicks, rawFrame->CurrentNtpTimeAsTicks, rawFrame->CurrentSecondaryTimeAsTicks, rawFrame->NtpBasedTimeError, rawFrame->CameraGain, rawFrame->CameraGamma, rawFrame->Temperature, rawFrame->CameraExposure);
		::ZeroMemory(integratedPixels, IMAGE_TOTAL_PIXELS * sizeof(double));

		if (isNewIntegrationPeriod)
		{
			numberOfIntegratedFrames = 0;

			idxFirstFrameNumber = idxFrameNumber;
			firstFrameNtpTimestamp = rawFrame->CurrentNtpTimeAsTicks - VIDEO_FRAME_IN_WINDOWS_TICKS; // The frame started one frame ago
			firstFrameSecondaryTimestamp = rawFrame->CurrentSecondaryTimeAsTicks - VIDEO_FRAME_IN_WINDOWS_TICKS; // The frame started one frame ago
			idxLastFrameNumber = 0;
		}
	}

	lastFrameNtpTimestamp = rawFrame->CurrentNtpTimeAsTicks;
	lastFrameSecondaryTimestamp = rawFrame->CurrentSecondaryTimeAsTicks;

	long stride = 3 * IMAGE_WIDTH;
	unsigned char* ptrPixelItt = rawFrame->BmpBits + (IMAGE_HEIGHT - 1) * IMAGE_STRIDE;

	double* ptrPixels = integratedPixels;

	unsigned char* ptrFirstOrLastFrameCopy = NULL;
	unsigned char* ptrCurrTrackedFramePixels = currTrackedFramePixels;

	if (isNewIntegrationPeriod)
	{
		ptrFirstOrLastFrameCopy = firstIntegratedFramePixels;
		lastFrameWasNewIntegrationPeriod = true;
	}
	else
	{
		ptrFirstOrLastFrameCopy = lastIntegratedFramePixels;
		lastFrameWasNewIntegrationPeriod = false;
	}

	for (int y = 0; y < IMAGE_HEIGHT; y++)
	{
		for (int x = 0; x < IMAGE_WIDTH; x++)
		{
			unsigned char thisPixel;

			if (MONOCHROME_CONVERSION_MODE == 0)
				thisPixel= *(ptrPixelItt + 2); //R
			else if (MONOCHROME_CONVERSION_MODE == 1)
				thisPixel= *(ptrPixelItt + 1); //G
			else if (MONOCHROME_CONVERSION_MODE == 2)
				thisPixel = *(ptrPixelItt); //B
			else if (MONOCHROME_CONVERSION_MODE == 3)
			{
				// YUV Conversion (PAL & NTSC)
				// Luma = 0.299 R + 0.587 G + 0.114 B
				double luma = 0.299* *(ptrPixelItt) + 0.587* *(ptrPixelItt + 1) + 0.114* *(ptrPixelItt + 2);

				if (luma < 0)
					thisPixel = 0;
				else if (luma > 255)
					thisPixel = 255;
				else
					thisPixel = (unsigned char)luma;
			}

			// Saving the first/last frame raw pixels for OCR-ing
			*ptrFirstOrLastFrameCopy = thisPixel;
			*ptrCurrTrackedFramePixels = thisPixel;

		    *ptrPixels += thisPixel;

			ptrPixels++;
			ptrFirstOrLastFrameCopy++;
			ptrCurrTrackedFramePixels++;
			ptrPixelItt+=3;
		}

		ptrPixelItt = ptrPixelItt - 2 * IMAGE_STRIDE;
	}

	numberOfIntegratedFrames++;

	idxLastFrameNumber = idxFrameNumber;
	//idxLastFrameTimestamp = currentUtcDayAsTicks

	HandleTracking(currTrackedFramePixels, NULL);

	if (lastFrameWasNewIntegrationPeriod)
		trackedThisIntegrationPeriod = false;
}

void ProcessBufferedVideoFrame()
{
	RawFrame* nextFrame = NULL;

	do
	{
		nextFrame = FetchFrameFromRawFrameBuffer();

		if (nextFrame != NULL)
		{
			unsigned char* dataToSave;

			ProcessRawFrame(nextFrame);

			delete nextFrame;
		}
	}
	while(nextFrame != NULL);
}


void FrameProcessingThreadProc( void* pContext )
{
	while(true)
	{
		ProcessBufferedVideoFrame();

		Sleep(1);
	};
}

HRESULT ProcessVideoFrameBuffered(LPVOID bmpBits, __int64 currentUtcDayAsTicks, __int64 currentNtpTimeAsTicks, double ntpBasedTimeError, __int64 currentSecondaryTimeAsTicks, float cameraGain, float cameraGamma, float temperature, char* cameraExposure, FrameProcessingStatus* frameInfo)
{
	frameInfo->FrameDiffSignature = 0;

	unsigned char* buf = reinterpret_cast<unsigned char*>(bmpBits);

	if (NULL != buf)
	{
		RawFrame* frame = new RawFrame(IMAGE_WIDTH, IMAGE_HEIGHT);
		frame->CurrentUtcDayAsTicks = currentUtcDayAsTicks;
		frame->CurrentNtpTimeAsTicks = currentNtpTimeAsTicks;
		frame->NtpBasedTimeError = ntpBasedTimeError;
		frame->CurrentSecondaryTimeAsTicks = currentSecondaryTimeAsTicks;
		frame->CameraGain = cameraGain;
		frame->CameraGamma = cameraGamma;
		frame->Temperature = temperature;
		if (cameraExposure)
			strcpy(&frame->CameraExposure[0], (char *)cameraExposure);
		else
			frame->CameraExposure[0] = 0;

		memcpy(&frame->BmpBits[0], &buf[0], frame->BmpBitsSize);

		AddFrameToRawFrameBuffer(frame);

		return S_OK;
	}
	else
		return E_FAIL;
}

HRESULT ProcessVideoFrameSynchronous(LPVOID bmpBits, __int64 currentUtcDayAsTicks, __int64 currentNtpTimeAsTicks, double ntpBasedTimeError,  __int64 currentSecondaryTimeAsTicks, float cameraGain, float cameraGamma, float temperature, char* cameraExposure, FrameProcessingStatus* frameInfo)
{
	frameInfo->FrameDiffSignature = 0;

	unsigned char* buf = reinterpret_cast<unsigned char*>(bmpBits);

	float diffSignature;

	CalculateDiffSignature(buf, &diffSignature);

	idxFrameNumber++;
	
	bool isNewIntegrationPeriod = IsNewIntegrationPeriod(diffSignature, INTEGRATION_LOCKED && numberOfIntegratedFrames == lockedIntegrationFrames);	

	// After the integration has been 'locked' we only output a frame when a new integration period has been detected
	// When the integration hasn't been 'locked' we output every frame received from the camera
	bool showOutputFrame = idxFrameNumber > STARTUP_FRAMES_WITH_NO_OUTPUT && (isNewIntegrationPeriod || !INTEGRATION_LOCKED);

	if (showOutputFrame)
	{
		BufferNewIntegratedFrame(isNewIntegrationPeriod, currentUtcDayAsTicks, currentNtpTimeAsTicks, currentSecondaryTimeAsTicks, ntpBasedTimeError, cameraGain, cameraGamma, temperature, (char*)cameraExposure);
		::ZeroMemory(integratedPixels, IMAGE_TOTAL_PIXELS * sizeof(double));

		if (isNewIntegrationPeriod)
		{
			numberOfIntegratedFrames = 0;

			idxFirstFrameNumber = idxFrameNumber;
			idxLastFrameNumber = 0;
			firstFrameNtpTimestamp = currentNtpTimeAsTicks - VIDEO_FRAME_IN_WINDOWS_TICKS; // The frame started one frame ago
			firstFrameSecondaryTimestamp = currentSecondaryTimeAsTicks - VIDEO_FRAME_IN_WINDOWS_TICKS; // The frame started one frame ago
		}
	}

	lastFrameNtpTimestamp = currentNtpTimeAsTicks;
	lastFrameSecondaryTimestamp = currentSecondaryTimeAsTicks;

	frameInfo->FrameDiffSignature  = diffSignature;
	//frameInfo->CurrentSignatureRatio  = NULL != integrationChecker ? integrationChecker->CurrentSignatureRatio : 0;

	long stride = 3 * IMAGE_WIDTH;
	unsigned char* ptrPixelItt = buf + (IMAGE_HEIGHT - 1) * IMAGE_STRIDE;

	double* ptrPixels = integratedPixels;

	unsigned char* ptrFirstOrLastFrameCopy = NULL;
	unsigned char* ptrCurrTrackedFramePixels = currTrackedFramePixels;

	if (isNewIntegrationPeriod)
	{
		ptrFirstOrLastFrameCopy = firstIntegratedFramePixels;
		lastFrameWasNewIntegrationPeriod = true;
	}
	else
	{
		ptrFirstOrLastFrameCopy = lastIntegratedFramePixels;
		lastFrameWasNewIntegrationPeriod = false;
	}
	
	for (int y = 0; y < IMAGE_HEIGHT; y++)
	{
		for (int x = 0; x < IMAGE_WIDTH; x++)
		{
			unsigned char thisPixel;

			if (MONOCHROME_CONVERSION_MODE == 0)
				thisPixel= *(ptrPixelItt + 2); //R
			else if (MONOCHROME_CONVERSION_MODE == 1)
				thisPixel= *(ptrPixelItt + 1); //G
			else if (MONOCHROME_CONVERSION_MODE == 2)
				thisPixel = *(ptrPixelItt); //B
			else if (MONOCHROME_CONVERSION_MODE == 3)
			{
				// YUV Conversion (PAL & NTSC)
				// Luma = 0.299 R + 0.587 G + 0.114 B
				double luma = 0.299* *(ptrPixelItt) + 0.587* *(ptrPixelItt + 1) + 0.114* *(ptrPixelItt + 2);

				if (luma < 0)
					thisPixel = 0;
				else if (luma > 255)
					thisPixel = 255;
				else
					thisPixel = (unsigned char)luma;
			}

			// Saving the first/last frame raw pixels for OCR-ing
			*ptrFirstOrLastFrameCopy = thisPixel;
			*ptrCurrTrackedFramePixels = thisPixel;
		    *ptrPixels += thisPixel;

			ptrPixels++;
			ptrFirstOrLastFrameCopy++;
			ptrCurrTrackedFramePixels++;
			ptrPixelItt+=3;
		}

		ptrPixelItt = ptrPixelItt - 2 * IMAGE_STRIDE;
	}

	numberOfIntegratedFrames++;

	idxLastFrameNumber = idxFrameNumber;
	//idxLastFrameTimestamp = currentUtcDayAsTicks;

	frameInfo->CameraFrameNo = idxFrameNumber;
	frameInfo->IntegratedFrameNo = idxIntegratedFrameNumber;
	frameInfo->IntegratedFramesSoFar = numberOfIntegratedFrames;

	HandleTracking(currTrackedFramePixels, NULL);

	if (lastFrameWasNewIntegrationPeriod)
		trackedThisIntegrationPeriod = false;

	return S_OK;
}

HRESULT ProcessVideoFrame(LPVOID bmpBits, __int64 currentUtcDayAsTicks, __int64 currentNtpTimeAsTicks, double ntpBasedTimeError, __int64 currentSecondaryTimeAsTicks, float cameraGain, float cameraGamma, float temperature, char* cameraExposure, FrameProcessingStatus* frameInfo)
{
	if (USE_BUFFERED_FRAME_PROCESSING)
		return ProcessVideoFrameBuffered(bmpBits, currentUtcDayAsTicks, currentNtpTimeAsTicks, ntpBasedTimeError, currentSecondaryTimeAsTicks, cameraGain, cameraGamma, temperature, cameraExposure, frameInfo);
	else
		return ProcessVideoFrameSynchronous(bmpBits, currentUtcDayAsTicks, currentNtpTimeAsTicks, ntpBasedTimeError, currentSecondaryTimeAsTicks, cameraGain, cameraGamma, temperature, cameraExposure, frameInfo);
}

long long firstRecordedFrameTimestamp = 0;

void RecordCurrentFrame(IntegratedFrame* nextFrame)
{
	long long timeStamp = WindowsTicksToAavTicks((nextFrame->StartTimeStamp + nextFrame->EndTimeStamp) / 2);
	unsigned int exposureIn10thMilliseconds = (nextFrame->EndTimeStamp - nextFrame->StartTimeStamp) / 1000;

	if (firstRecordedFrameTimestamp == 0)
		firstRecordedFrameTimestamp = timeStamp;

	// since the first recorded frame was taken
	unsigned int elapsedTimeMilliseconds = (unsigned int)((timeStamp - firstRecordedFrameTimestamp) * 0xFFFFFFFF);

	bool frameStartedOk = AavBeginFrame(timeStamp, elapsedTimeMilliseconds, exposureIn10thMilliseconds);

	SYSTEMTIME sysTime;
	GetSystemTime(&sysTime);

	if (!RECORD_ONLY_STATUS_CHANNEL_WITH_OCRED_TIMESTAMPS)
	{
		AavFrameAddStatusTag16(STATUS_TAG_NUMBER_INTEGRATED_FRAMES, nextFrame->NumberOfIntegratedFrames);
		AavFrameAddStatusTag64(STATUS_TAG_START_FRAME_ID, nextFrame->StartFrameId);
		AavFrameAddStatusTag64(STATUS_TAG_END_FRAME_ID, nextFrame->EndFrameId);
		AavFrameAddStatusTag64(STATUS_TAG_SYSTEM_TIME, SystemTimeToAavTicks(sysTime));

		if (nextFrame->Gamma > 0)
			AavFrameAddStatusTagReal(STATUS_TAG_GAMMA, nextFrame->Gamma);

		if (nextFrame->Gain > 0)
			AavFrameAddStatusTagReal(STATUS_TAG_GAIN, nextFrame->Gain);

		if (nextFrame->Exposure)
			AavFrameAddStatusTag(STATUS_TAG_EXPOSURE, &nextFrame->Exposure[0]);

		if (nextFrame->Temperature > -99 && nextFrame->Temperature < 99)
			AavFrameAddStatusTagReal(STATUS_TAG_TEMPERATURE, nextFrame->Temperature);

	}

	if (OCR_IS_SETUP)
	{
		if (!RECORD_ONLY_STATUS_CHANNEL_WITH_OCRED_TIMESTAMPS)
		{
			AavFrameAddStatusTag(STATUS_TAG_START_TIMESTAMP, &nextFrame->StartTimeStampStr[0]);
			AavFrameAddStatusTag(STATUS_TAG_END_TIMESTAMP, &nextFrame->EndTimeStampStr[0]);
		}

		AavFrameAddStatusTagUInt8(STATUS_TAG_GPS_TRACKED_SATELLITES, nextFrame->GpsTrackedSatellites);
		AavFrameAddStatusTagUInt8(STATUS_TAG_GPS_ALMANAC, nextFrame->GpsAlamancStatus);
		AavFrameAddStatusTagUInt8(STATUS_TAG_GPS_FIX, nextFrame->GpsFixStatus);

		if (OCR_FAILED_TEST_RECORDING)
			AavFrameAddStatusTag(STATUS_TAG_OCR_TESTING_ERROR_MESSAGE, &nextFrame->OcrErrorMessageStr[0]);
	}

	if (USE_NTP_TIMESTAMP)
	{
		long long ntpStartTimeStamp = WindowsTicksToAavTicks(nextFrame->NTPStartTimestamp);
		long long ntpEndTimeStamp = WindowsTicksToAavTicks(nextFrame->NTPEndTimestamp);
		AavFrameAddStatusTag64(STATUS_TAG_NTP_START_TIMESTAMP, ntpStartTimeStamp);
		AavFrameAddStatusTag64(STATUS_TAG_NTP_END_TIMESTAMP, ntpEndTimeStamp);
		AavFrameAddStatusTag16(STATUS_TAG_NTP_TIME_ERROR, (short)nextFrame->NTPTimestampError);
	}

	if (USE_SECONDARY_TIMESTAMP)
	{
		long long secondaryStartTimeStamp = WindowsTicksToAavTicks(nextFrame->SecondaryStartTimestamp);
		long long secondaryEndTimeStamp = WindowsTicksToAavTicks(nextFrame->SecondaryEndTimestamp);
		AavFrameAddStatusTag64(STATUS_TAG_SECONDARY_START_TIMESTAMP, secondaryStartTimeStamp);
		AavFrameAddStatusTag64(STATUS_TAG_SECONDARY_END_TIMESTAMP, secondaryEndTimeStamp);
	}

	if (AAV_16)
		AavFrameAddImage16(USE_IMAGE_LAYOUT, nextFrame->Pixels16);
	else
		AavFrameAddImage(USE_IMAGE_LAYOUT, nextFrame->Pixels);

	AavEndFrame();
}

void RecordAllbufferedFrames()
{
	IntegratedFrame* nextFrame = NULL;

	do
	{
		nextFrame = FetchFrameFromRecordingBuffer();

		if (nextFrame != NULL)
		{
			unsigned char* dataToSave;

			RecordCurrentFrame(nextFrame);

			delete nextFrame;
		}
	}
	while(nextFrame != NULL);
}

void RecorderThreadProc( void* pContext )
{
	while(recording)
	{
		RecordAllbufferedFrames();

		Sleep(1);
	};

	// Record all remaining frames, after 'recording' has been set to false
	RecordAllbufferedFrames();

    _endthread();
}

void CopyBuffer(IntegratedFrame* frame, unsigned char* rawPixels)
{
	if (AAV_16)
	{
		for (int i = 0; i < IMAGE_TOTAL_PIXELS; i++)
			frame->Pixels16[i] = *(rawPixels + i);
	}
	else
		memcpy(frame->Pixels, rawPixels, IMAGE_TOTAL_PIXELS);
}


HRESULT StartRecordingInternal(LPCTSTR szFileName)
{
	AavNewFile((const char*)szFileName);
	
	AavAddFileTag("AAVR-SOFTWARE-VERSION", "1.0");
	AavAddFileTag("RECORDER", occuRecVersion);
	AavAddFileTag("FSTF-TYPE", "AAV");
	AavAddFileTag("AAV-VERSION", "1");
	
	AavAddFileTag("GRABBER", grabberName);
	AavAddFileTag("VIDEO-MODE", videoMode);
	AavAddFileTag("CAMERA-MODEL", cameraModel);
	AavAddFileTag("CAMERA-BITPIX", "8");

	AavAddFileTag("OBSERVER", observerInfo);
	AavAddFileTag("TELESCOP", telescopeInfo);
	AavAddFileTag("OBJECT", targetInfo);
	AavAddFileTag("RA_OBJ", raObjInfo);
	AavAddFileTag("DEC_OBJ", decObjInfo);
	AavAddFileTag("LONGITUDE", obsLongitude);
	AavAddFileTag("LATITUDE", obsLatitude);

	if (OCR_IS_SETUP)
		AavAddFileTag("OCR-ENGINE", "IOTA-VTI OccuRec OCR v1.1");

	if (AAV_16)
		AavAddFileTag("FRAME-COMBINING", "Binning");
	else
		AavAddFileTag("FRAME-COMBINING", "Averaging");

	char buffer[128];

	if (OCR_PRESERVE_VTI)
	{
		sprintf(&buffer[0], "%d", OCR_FRAME_TOP_ODD);
		AavAddFileTag("OSD-FIRST-LINE", &buffer[0]);

		sprintf(&buffer[0], "%d", OCR_FRAME_TOP_EVEN + 2 * OCR_CHAR_FIELD_HEIGHT);
		AavAddFileTag("OSD-LAST-LINE", &buffer[0]);
	}

	if (NO_INTEGRATION_STACK_RATE > 0)
	{
		sprintf(&buffer[0], "%d", NO_INTEGRATION_STACK_RATE);
		AavAddFileTag("FRAME-STACKING-RATE", &buffer[0]);
	}

	float effectiveIntegrationRate = videoFrameRate / detectedIntegrationRate;
	sprintf(&buffer[0], "%.5f", effectiveIntegrationRate);
	AavAddFileTag("EFFECTIVE-FRAME-RATE", &buffer[0]);

	sprintf(&buffer[0], "%.2f", videoFrameRate);
	AavAddFileTag("NATIVE-FRAME-RATE", &buffer[0]);

	if (abs(videoFrameRate - 25.0) < 0.05)
	{
		AavAddFileTag("NATIVE-VIDEO-STANDARD", "PAL");
		VIDEO_FRAME_IN_WINDOWS_TICKS = 400000;
	}
	else if (abs(videoFrameRate - 29.97) < 0.05)
	{
		AavAddFileTag("NATIVE-VIDEO-STANDARD", "NTSC");
		VIDEO_FRAME_IN_WINDOWS_TICKS = 333667;
	}
	else
	{
		AavAddFileTag("NATIVE-VIDEO-STANDARD", "");
		VIDEO_FRAME_IN_WINDOWS_TICKS = (__int64)(10000000.0 / videoFrameRate);
	}

	if (USE_NTP_TIMESTAMP)
	{
		sprintf(&buffer[0], "%d", HARDWARE_TIMING_CORRECTION);
		AavAddFileTag("CAPHNTP-TIMING-CORRECTION", &buffer[0]);
	}

	AavDefineImageSection(IMAGE_WIDTH, IMAGE_HEIGHT, AAV_16 ? 16 : 8);
	AavAddOrUpdateImageSectionTag("IMAGE-BYTE-ORDER", "LITTLE-ENDIAN");
	
	AavDefineImageLayout(1, AAV_16 ? 16 : 8, "FULL-IMAGE-RAW", "UNCOMPRESSED", 0, NULL);
	
	AavDefineImageLayout(2, AAV_16 ? 16 : 8, "FULL-IMAGE-DIFFERENTIAL-CODING-NOSIGNS", USE_COMPRESSION_ALGORITHM == 1 ? "LAGARITH16" : "QUICKLZ", 32, "PREV-FRAME");
	AavDefineImageLayout(3, AAV_16 ? 16 : 8, "FULL-IMAGE-DIFFERENTIAL-CODING", USE_COMPRESSION_ALGORITHM == 1 ? "LAGARITH16" : "QUICKLZ", 32, "PREV-FRAME");
	AavDefineImageLayout(4, AAV_16 ? 16 : 8, "FULL-IMAGE-RAW", USE_COMPRESSION_ALGORITHM == 1 ? "LAGARITH16" : "QUICKLZ", 0, NULL);

	if (RECORD_ONLY_STATUS_CHANNEL_WITH_OCRED_TIMESTAMPS)
	{
		AavDefineImageLayout(5, AAV_16 ? 16 : 8, "STATUS-CHANNEL-ONLY", "UNCOMPRESSED", 0, NULL);
	}
	
	if (!RECORD_ONLY_STATUS_CHANNEL_WITH_OCRED_TIMESTAMPS)
	{
		STATUS_TAG_SYSTEM_TIME= AavDefineStatusSectionTag("SystemTime", AavTagType::ULong64);
		STATUS_TAG_NUMBER_INTEGRATED_FRAMES = AavDefineStatusSectionTag("IntegratedFrames", AavTagType::UInt16);
		STATUS_TAG_START_FRAME_ID = AavDefineStatusSectionTag("StartFrame", AavTagType::ULong64);
		STATUS_TAG_END_FRAME_ID = AavDefineStatusSectionTag("EndFrame", AavTagType::ULong64);
		STATUS_TAG_START_TIMESTAMP = AavDefineStatusSectionTag("StartFrameTimestamp", AavTagType::AnsiString255);
		STATUS_TAG_END_TIMESTAMP = AavDefineStatusSectionTag("EndFrameTimestamp", AavTagType::AnsiString255);
		STATUS_TAG_GAIN = AavDefineStatusSectionTag("Gain", AavTagType::Real);
		STATUS_TAG_GAMMA = AavDefineStatusSectionTag("Gamma", AavTagType::Real);
		STATUS_TAG_EXPOSURE = AavDefineStatusSectionTag("CameraExposure", AavTagType::AnsiString255);
		STATUS_TAG_TEMPERATURE = AavDefineStatusSectionTag("Temperature", AavTagType::Real);
	}

	STATUS_TAG_GPS_TRACKED_SATELLITES = AavDefineStatusSectionTag("GPSTrackedSatellites", AavTagType::UInt8);
	STATUS_TAG_GPS_ALMANAC = AavDefineStatusSectionTag("GPSAlmanacStatus", AavTagType::UInt8);
	STATUS_TAG_GPS_FIX = AavDefineStatusSectionTag("GPSFixStatus", AavTagType::UInt8);

	if (USE_NTP_TIMESTAMP)
	{
		STATUS_TAG_NTP_START_TIMESTAMP = AavDefineStatusSectionTag("NTPStartTimestamp", AavTagType::ULong64);
		STATUS_TAG_NTP_END_TIMESTAMP = AavDefineStatusSectionTag("NTPEndTimestamp", AavTagType::ULong64);
		STATUS_TAG_NTP_TIME_ERROR = AavDefineStatusSectionTag("NTPTimestampError", AavTagType::UInt16);
	}

	if (USE_SECONDARY_TIMESTAMP)
	{
		STATUS_TAG_SECONDARY_START_TIMESTAMP = AavDefineStatusSectionTag("StartTimestampSecondary", AavTagType::ULong64);
		STATUS_TAG_SECONDARY_END_TIMESTAMP = AavDefineStatusSectionTag("EndTimestampSecondary", AavTagType::ULong64);
	}

	if (OCR_FAILED_TEST_RECORDING)
		STATUS_TAG_OCR_TESTING_ERROR_MESSAGE = AavDefineStatusSectionTag("OcrTestingErrorMessage", AavTagType::AnsiString255);

	ClearRecordingBuffer();

	firstRecordedFrameTimestamp = 0;

	if (NULL != ocrManager)
	{
		ocrManager->ResetErrorCounter();
	}

	// As a first frame add a non-integrated frame (to be able to tell the star-end timestamp order)
	IntegratedFrame* frame = new IntegratedFrame(IMAGE_TOTAL_PIXELS, AAV_16);
	CopyBuffer(frame, firstIntegratedFramePixels);

	frame->NumberOfIntegratedFrames = 0;
	frame->StartFrameId = -1;
	frame->EndFrameId = -1;
	frame->StartTimeStamp = 0;
	frame->EndTimeStamp = 0;
	frame->FrameNumber = -1;
	frame->GpsTrackedSatellites = 0;
	frame->GpsAlamancStatus = 0;
	frame->GpsFixStatus = 0;
	frame->StartTimeStampStr[0] = 0;
	frame->EndTimeStampStr[0] = 0;
	frame->OcrErrorMessageStr[0] = 0;
	frame->NTPStartTimestamp = 0;
	frame->NTPEndTimestamp = 0;
	frame->NTPTimestampError = 0;
	frame->SecondaryStartTimestamp = 0;
	frame->SecondaryEndTimestamp = 0;
	frame->Gain = -1;
	frame->Gamma = -1;
	frame->Exposure[0] = 0; 

	RecordCurrentFrame(frame);

	recording = true;
	numRecordedFrames = 0;
	averageNtpDebugOffsetMS = 0;
	aggregatedNtpDebug = 0;

	AAV16_MAX_BINNED_FRAMES = 0;

	// Create a new thread
	hRecordingThread = (HANDLE)_beginthread(RecorderThreadProc, 0, NULL);

	return S_OK;
}

HRESULT StartRecording(LPCTSTR szFileName)
{
	OCR_FAILED_TEST_RECORDING = false;
	return StartRecordingInternal(szFileName);
}

HRESULT StopRecording(long* pixels)
{
	recording = false;

	WaitForSingleObject(hRecordingThread, INFINITE); // wait for thread to exit

	if (NULL == ocrManager || !ocrManager->IsReceivingTimeStamps())
	{
		// As a last frame add a non-integrated frame (to be able to tell the star-end timestamp order)
		IntegratedFrame* frame = new IntegratedFrame(IMAGE_TOTAL_PIXELS, AAV_16);
		CopyBuffer(frame, firstIntegratedFramePixels);

		frame->NumberOfIntegratedFrames = 0;
		frame->StartFrameId = -1;
		frame->EndFrameId = -1;
		frame->StartTimeStamp = 0;
		frame->EndTimeStamp = 0;
		frame->FrameNumber = -1;
		frame->GpsTrackedSatellites = 0;
		frame->GpsAlamancStatus = 0;
		frame->GpsFixStatus = 0;
		frame->StartTimeStampStr[0] = 0;
		frame->EndTimeStampStr[0] = 0;
		frame->OcrErrorMessageStr[0] = 0;
		frame->NTPStartTimestamp = 0;
		frame->NTPEndTimestamp = 0;
		frame->NTPTimestampError = 0;
		frame->SecondaryStartTimestamp = 0;
		frame->SecondaryEndTimestamp = 0;
		frame->Gain = -1;
		frame->Gamma = -1;
		frame->Exposure[0] = 0; 

		RecordCurrentFrame(frame);
	}

	if (AAV_16)
	{
		// Add the max pixel value for normalisation in AAV-16 mode
		char buffer[128];
		sprintf(&buffer[0], "%d", AAV16_MAX_BINNED_FRAMES * 255);
		AavAddUserTag("AAV16-NORMVAL", &buffer[0]);
	}

	AavEndFile();

	OCR_FAILED_TEST_RECORDING = false;

	return S_OK;
}

HRESULT StartOcrTesting(LPCTSTR szFileName)
{
	if (!OCR_IS_SETUP)
		throw new exception("OCR hasn't been setup. Cannot start testing.");

	OCR_FAILED_TEST_RECORDING = true;

	return StartRecordingInternal(szFileName);
}

HRESULT DisableOcrProcessing()
{
	OCR_IS_SETUP = false;

	return S_OK;
}

HRESULT EnableTracking(long targetObjectId, long guidingObjectId, long frequency, float targetAperture, float guidingAperture, float innerRadiusOfBackgroundApertureInSignalApertures, long numberOfPixelsInBackgroundAperture)
{
	RUN_TRACKING = true;
	TRACKED_TARGET_ID = targetObjectId;
	TRACKED_GUIDING_ID = guidingObjectId;
	TRACKING_FREQUENCY = frequency;
	TRACKED_TARGET_APERTURE = targetAperture;
	TRACKED_GUIDING_APERTURE = guidingAperture;

	TRACKING_BG_INNER_RADIUS = innerRadiusOfBackgroundApertureInSignalApertures;
	TRACKING_BG_MIN_NUM_PIXELS = numberOfPixelsInBackgroundAperture;

	return S_OK;
}

HRESULT DisableTracking()
{
	RUN_TRACKING = false;

	return S_OK;
}