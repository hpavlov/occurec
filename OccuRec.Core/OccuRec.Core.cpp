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

using namespace OccuOcr;

#define MEDIAN_CALC_ROWS_FROM 10
#define MEDIAN_CALC_ROWS_TO 11

long IMAGE_WIDTH;
long IMAGE_HEIGHT;
long IMAGE_STRIDE;
long IMAGE_TOTAL_PIXELS;
long MONOCHROME_CONVERSION_MODE;
long USE_IMAGE_LAYOUT = 4;
bool USE_BUFFERED_FRAME_PROCESSING = true;
bool INTEGRATION_DETECTION_TUNING = false;


bool OCR_IS_SETUP = false;
long OCR_FRAME_TOP_ODD;
long OCR_FRAME_TOP_EVEN;
long OCR_CHAR_WIDTH;
long OCR_CHAR_FIELD_HEIGHT; 
long* OCR_ZONE_MATRIX = NULL; 
//long OCR_NUMBER_OF_ZONES;
//long OCR_NUMBER_OF_CHAR_POSITIONS;

long MEDIAN_CALC_INDEX_FROM;
long MEDIAN_CALC_INDEX_TO;

OcrFrameProcessor* firstFrameOcrProcessor = NULL;
OcrFrameProcessor* lastFrameOcrProcessor = NULL;
OcrManager* ocrManager = NULL;

bool FLIP_VERTICALLY;
bool FLIP_HORIZONTALLY;

bool IS_INTEGRATING_CAMERA;
float SIGNATURE_DIFFERENCE_RATIO;
float MINIMUM_SIGNATURE_DIFFERENCE;
bool USES_DIFF_GAMMA;
unsigned char GAMMA[256];

bool INTEGRATION_LOCKED;

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

unsigned char* latestIntegratedFrame = NULL;
ImageStatus latestImageStatus;
ImageStatus latestDetectedIntegrationFrameImageStatus;

unsigned char* firstIntegratedFramePixels = NULL;
unsigned char* lastIntegratedFramePixels = NULL;

HANDLE hRecordingThread = NULL;
bool recording = false;
char cameraModel[128];
char occuRecVersion[32];
char grabberName[128];
char videoMode[128];

unsigned int STATUS_TAG_NUMBER_INTEGRATED_FRAMES;
unsigned int STATUS_TAG_START_FRAME_ID;
unsigned int STATUS_TAG_END_FRAME_ID;
unsigned int STATUS_TAG_START_TIMESTAMP;
unsigned int STATUS_TAG_END_TIMESTAMP;
unsigned int STATUS_TAG_GPS_TRACKED_SATELLITES;
unsigned int STATUS_TAG_GPS_ALMANAC;
unsigned int STATUS_TAG_GPS_FIX;

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
		*isNew = testChecker->IsNewIntegrationPeriod(frameNo, diffSignature);
		return S_OK;
	}
	else
		return E_HANDLE;
}

bool IsNewIntegrationPeriod(float diffSignature)
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
		bool isNewIntegrationPeriod = integrationChecker->IsNewIntegrationPeriod(idxFrameNumber, diffSignature);
		SyncLock::UnlockIntDet();		

		return isNewIntegrationPeriod;
	}
	else
		return false;
}


HRESULT SetupAav(long useImageLayout, long usesBufferedMode, long integrationDetectionTuning, LPCTSTR szOccuRecVersion)
{
	OCR_IS_SETUP = false;
	USE_IMAGE_LAYOUT = useImageLayout;
	USE_BUFFERED_FRAME_PROCESSING = usesBufferedMode == 1;
	INTEGRATION_DETECTION_TUNING = integrationDetectionTuning == 1;

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
		default:
			DebugViewPrint(L"AAVSetup: ImageLayout = %s; BufferedMode = %d; IntegrationTuning: %s\n", USE_IMAGE_LAYOUT, USE_BUFFERED_FRAME_PROCESSING ? 1:0, INTEGRATION_DETECTION_TUNING ? L"Y":L"N"); 
			break;
	}

	return S_OK;
}

HRESULT SetupIntegrationPreservationArea(int areaTopOdd, int areaTopEven, int areaHeight)
{
	OCR_FRAME_TOP_ODD = areaTopOdd;
	OCR_FRAME_TOP_EVEN = areaTopEven;
	OCR_CHAR_FIELD_HEIGHT = areaHeight;

	return S_OK;
}

HRESULT SetupOcrAlignment(long width, long height, long frameTopOdd, long frameTopEven, long charWidth, long charHeight, long numberOfCharPositions, long numberOfZones, long* pixelsInZones)
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
	

	return S_OK;
}

HRESULT SetupOcrZoneMatrix(long* matrix)
{
	if (NULL == OCR_ZONE_MATRIX)
		OCR_ZONE_MATRIX = (long*)malloc(IMAGE_TOTAL_PIXELS * sizeof(long));

	// memcpy didn't work??
	// memcpy(&OCR_ZONE_MATRIX[0], &matrix[0], IMAGE_TOTAL_PIXELS);
	
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

HRESULT SetupGrabberInfo(LPCTSTR szGrabberName, LPCTSTR szVideoMode)
{
	strcpy(&grabberName[0], (char *)szGrabberName);
	strcpy(&videoMode[0], (char *)szVideoMode);

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

	INTEGRATION_LOCKED = false;

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

	idxFrameNumber = 0;
	numberOfDiffSignaturesCalculated = 0;
	numberOfIntegratedFrames = 0;
	idxIntegratedFrameNumber = 0;
	droppedFramesSinceIntegrationIsLocked = 0;

	latestImageStatus.UniqueFrameNo = 0;

	strcpy(&cameraModel[0], (char *)szCameraModel);

	recording = false;

	return S_OK;
}

HRESULT SetupIntegrationDetection(float minDiffRatio, float minSignDiff, float diffGamma)
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

long BufferNewIntegratedFrame(bool isNewIntegrationPeriod, __int64 currentUtcDayAsTicks)
{
	long numItems = 0;

	if (isNewIntegrationPeriod)
	{
		idxIntegratedFrameNumber++;

		detectedIntegrationRate = numberOfIntegratedFrames;

		if (!INTEGRATION_LOCKED)
			lockedIntegrationFrames = detectedIntegrationRate;
		else
			droppedFramesSinceIntegrationIsLocked += abs(lockedIntegrationFrames - detectedIntegrationRate);
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

		IntegratedFrame* frame = new IntegratedFrame(IMAGE_TOTAL_PIXELS);

		double* ptrPixels = integratedPixels;
		unsigned char* ptr8BitPixels = latestIntegratedFrame;

		unsigned char* ptrFramePixels = frame->Pixels;

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
				lastFrameOcrProcessor->AddMedianComputationPixel(lastIntegratedFramePixels[i]);
			}

			if (runOCR)
			{
				long packedInfo = OCR_ZONE_MATRIX[i];
				if (packedInfo != 0)
				{
					firstFrameOcrProcessor->ProcessZonePixel(packedInfo, pixX, pixY, firstIntegratedFramePixels[i]);
					lastFrameOcrProcessor->ProcessZonePixel(packedInfo,  pixX, pixY, lastIntegratedFramePixels[i]);
				}
			}

			// NOTE: Possible way to speed this up (which may not be necessary) would be to build a very large memory table
			//       that will return directly the average value for the given numberOfIntegratedFrames and 
			//       *ptrPixels (which is of type double but the value is integer from 0 to 255 * numberOfIntegratedFrames)
			long averageValue = (long)(*ptrPixels / (INTEGRATION_LOCKED ? numberOfIntegratedFrames : 1));

			if (averageValue <= 0)
			{
				*ptr8BitPixels = 0;
				*ptrFramePixels = 0;
			}
			else if (averageValue >= 255)
			{
				*ptr8BitPixels = 255;
				*ptrFramePixels = 255;
			}
			else
			{
				*ptr8BitPixels = averageValue;
				*ptrFramePixels = averageValue;
			}

			if (pixY >= OCR_FRAME_TOP_ODD && pixY < OCR_FRAME_TOP_EVEN + 2 * OCR_CHAR_FIELD_HEIGHT)
			{
				restoredPixels++;

				// Preserve the timestamp pixels from the first and last integrated frame in the final image
				if (pixY % 2 == 0)
				{
					*ptrFramePixels = firstIntegratedFramePixels[pixY * IMAGE_WIDTH + pixX];
					*ptr8BitPixels = *ptrFramePixels;
				}
				else
				{
					*ptrFramePixels = lastIntegratedFramePixels[pixY * IMAGE_WIDTH + pixX];
					*ptr8BitPixels = *ptrFramePixels;
				}
			}

			ptr8BitPixels++;
			ptrFramePixels++;
			ptrPixels++;
		}

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
				if ((INTEGRATION_LOCKED && firstFrameOcrProcessor->Success && lastFrameOcrProcessor->Success) || 
					(!INTEGRATION_LOCKED && lastFrameOcrProcessor->Success))
				{
					ocrManager->RegisterFirstSuccessfullyOcredFrame(lastFrameOcrProcessor);
					ocrFirstFrameProcessed = true;
				}
			}
			
			if (ocrFirstFrameProcessed)
			{
				if (INTEGRATION_LOCKED)
				{
					ocrManager->ProcessedOcredFramesInLockedMode(firstFrameOcrProcessor, lastFrameOcrProcessor);

					if (firstFrameOcrProcessor->Success && lastFrameOcrProcessor->Success)
					{
						idxFirstFrameNumber = firstFrameOcrProcessor->GetOcredStartFrameNumber();
						idxLastFrameNumber = lastFrameOcrProcessor->GetOcredEndFrameNumber();
						idxFirstFrameTimestamp = firstFrameOcrProcessor->GetOcredStartFrameTimeStamp(ocrManager->FieldDurationInTicks);
						idxLastFrameTimestamp = lastFrameOcrProcessor->GetOcredEndFrameTimeStamp();

						firstFrameOcrProcessor->GetOcredStartFrameTimeStampStr(&firstFrameTimestampStr[0]);
						lastFrameOcrProcessor->GetOcredEndFrameTimeStampStr(&endFrameTimestampStr[0]);

						almanacUpdateSatus = lastFrameOcrProcessor->GetOcredAlmanacUpdateState();
						gpsFixStatus = lastFrameOcrProcessor->GetOcredGpsFixState();
						trackedSatellitesCount = lastFrameOcrProcessor->GetOcredTrackedSatellitesCount();
					}
				}
				else
				{
					OccuOcr::OcrFrameProcessor* timeStampFrameProcessor = lastFrameWasNewIntegrationPeriod ? firstFrameOcrProcessor : lastFrameOcrProcessor;

					ocrManager->ProcessedOcredFrame(timeStampFrameProcessor);

					if (timeStampFrameProcessor->Success)
					{
						idxFirstFrameNumber = timeStampFrameProcessor->GetOcredStartFrameNumber();
						idxLastFrameNumber = timeStampFrameProcessor->GetOcredEndFrameNumber();
						idxFirstFrameTimestamp = timeStampFrameProcessor->GetOcredStartFrameTimeStamp(ocrManager->FieldDurationInTicks);
						idxLastFrameTimestamp = timeStampFrameProcessor->GetOcredEndFrameTimeStamp();

						timeStampFrameProcessor->GetOcredStartFrameTimeStampStr(&firstFrameTimestampStr[0]);
						timeStampFrameProcessor->GetOcredEndFrameTimeStampStr(&endFrameTimestampStr[0]);

						almanacUpdateSatus = timeStampFrameProcessor->GetOcredAlmanacUpdateState();
						gpsFixStatus = timeStampFrameProcessor->GetOcredGpsFixType();
						trackedSatellitesCount = timeStampFrameProcessor->GetOcredTrackedSatellitesCount();
					}
				}
			}

			ocrErrorsSiceLastReset = ocrManager->OcrErrorsSinceReset;
			
		}

		latestImageStatus.CountedFrames = numberOfIntegratedFrames;
		latestImageStatus.CutOffRatio = 0; // NULL != integrationChecker ? integrationChecker->NewIntegrationPeriodCutOffRatio : 0;
		latestImageStatus.IntegratedFrameNo = idxIntegratedFrameNumber;
		latestImageStatus.StartExposureFrameNo = idxFirstFrameNumber;
		latestImageStatus.StartExposureTicks = idxFirstFrameTimestamp;
		latestImageStatus.EndExposureFrameNo = idxLastFrameNumber;
		latestImageStatus.EndExposureTicks = idxLastFrameTimestamp;
		latestImageStatus.DetectedIntegrationRate = detectedIntegrationRate;
		latestImageStatus.DropedFramesSinceIntegrationLock = droppedFramesSinceIntegrationIsLocked;
		latestImageStatus.OcrWorking = (NULL != ocrManager && ocrManager->IsReceivingTimeStamps()) ? 1 : 0;
		latestImageStatus.OcrErrorsSinceLastReset = ocrErrorsSiceLastReset;

		if (INTEGRATION_CALIBRATION)
		{
			latestImageStatus.PerformedAction = 1;
			if (INTEGRATION_CALIBRATION_PASSES >= INTEGRATION_CALIBRATION_TOTAL_PASSES)
				latestImageStatus.PerformedActionProgress = 1;
			else
				latestImageStatus.PerformedActionProgress = 1.0 * INTEGRATION_CALIBRATION_PASSES / INTEGRATION_CALIBRATION_TOTAL_PASSES;
		}
		else
		{
			latestImageStatus.PerformedAction = 0;
			latestImageStatus.PerformedActionProgress = 0;
		}

		latestImageStatus.UniqueFrameNo++;

		if (recording)
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

			strcpy(&frame->StartTimeStampStr[0], &firstFrameTimestampStr[0]);
			strcpy(&frame->EndTimeStampStr[0], &endFrameTimestampStr[0]);


			numItems = AddFrameToRecordingBuffer(frame);
		}
		else
		{
			delete frame;
		}

		return numItems;
	}
}

HRESULT ProcessVideoFrame2(long* pixels, __int64 currentUtcDayAsTicks, FrameProcessingStatus* frameInfo)
{
	frameInfo->FrameDiffSignature = 0;

	float diffSignature;

	CalculateDiffSignature2(pixels, &diffSignature);

	idxFrameNumber++;

	bool isNewIntegrationPeriod = IsNewIntegrationPeriod(diffSignature);

	// After the integration has been 'locked' we only output a frame when a new integration period has been detected
	// When the integration hasn't been 'locked' we output every frame received from the camera
	bool showOutputFrame = isNewIntegrationPeriod || !INTEGRATION_LOCKED;

	if (showOutputFrame)
	{
		BufferNewIntegratedFrame(isNewIntegrationPeriod, currentUtcDayAsTicks);
		::ZeroMemory(integratedPixels, IMAGE_TOTAL_PIXELS * sizeof(double));

		if (isNewIntegrationPeriod)
		{
			numberOfIntegratedFrames = 0;

			idxFirstFrameNumber = idxFrameNumber;
			idxLastFrameNumber = 0;
		}
	}

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
#if _DEBUG
		DebugViewPrint(L"Copying pixels of the %d-th frame to FirstIntegratedFramePixels", idxFrameNumber);
#endif
	}
	else
	{
		ptrFirstOrLastFrameCopy = lastIntegratedFramePixels;
		lastFrameWasNewIntegrationPeriod = false;
#if _DEBUG
		DebugViewPrint(L"Copying pixels of the %d-th frame to LastIntegratedFramePixels", idxFrameNumber);
#endif
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

	return S_OK;
}

void ProcessRawFrame(RawFrame* rawFrame)
{
	float diffSignature;

	CalculateDiffSignature(rawFrame->BmpBits, &diffSignature);

	idxFrameNumber++;

	bool isNewIntegrationPeriod = IsNewIntegrationPeriod(diffSignature);

	// After the integration has been 'locked' we only output a frame when a new integration period has been detected
	// When the integration hasn't been 'locked' we output every frame received from the camera
	bool showOutputFrame = isNewIntegrationPeriod || !INTEGRATION_LOCKED;

	if (showOutputFrame)
	{
		BufferNewIntegratedFrame(isNewIntegrationPeriod, rawFrame->CurrentUtcDayAsTicks);
		::ZeroMemory(integratedPixels, IMAGE_TOTAL_PIXELS * sizeof(double));

		if (isNewIntegrationPeriod)
		{
			numberOfIntegratedFrames = 0;

			idxFirstFrameNumber = idxFrameNumber;
			idxLastFrameNumber = 0;
		}
	}

	long stride = 3 * IMAGE_WIDTH;
	unsigned char* ptrPixelItt = rawFrame->BmpBits + (IMAGE_HEIGHT - 1) * IMAGE_STRIDE;

	double* ptrPixels = integratedPixels;

	unsigned char* ptrFirstOrLastFrameCopy = NULL;

	if (isNewIntegrationPeriod)
	{
		ptrFirstOrLastFrameCopy = firstIntegratedFramePixels;
		lastFrameWasNewIntegrationPeriod = true;
#if _DEBUG
		DebugViewPrint(L"Copying pixels of the %d-th frame to FirstIntegratedFramePixels", idxFrameNumber);
#endif
	}
	else
	{
		ptrFirstOrLastFrameCopy = lastIntegratedFramePixels;
		lastFrameWasNewIntegrationPeriod = false;
#if _DEBUG
		DebugViewPrint(L"Copying pixels of the %d-th frame to LastIntegratedFramePixels", idxFrameNumber);
#endif
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
		    *ptrPixels += thisPixel;

			ptrPixels++;
			ptrFirstOrLastFrameCopy++;
			ptrPixelItt+=3;
		}

		ptrPixelItt = ptrPixelItt - 2 * IMAGE_STRIDE;
	}

	numberOfIntegratedFrames++;

	idxLastFrameNumber = idxFrameNumber;
	//idxLastFrameTimestamp = currentUtcDayAsTicks
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

HRESULT ProcessVideoFrameBuffered(LPVOID bmpBits, __int64 currentUtcDayAsTicks, FrameProcessingStatus* frameInfo)
{
	frameInfo->FrameDiffSignature = 0;

	unsigned char* buf = reinterpret_cast<unsigned char*>(bmpBits);

	RawFrame* frame = new RawFrame(IMAGE_WIDTH, IMAGE_HEIGHT);
	frame->CurrentUtcDayAsTicks = currentUtcDayAsTicks;
	memcpy(&frame->BmpBits[0], &buf[0], frame->BmpBitsSize);

	AddFrameToRawFrameBuffer(frame);

	return S_OK;
}

HRESULT ProcessVideoFrameSynchronous(LPVOID bmpBits, __int64 currentUtcDayAsTicks, FrameProcessingStatus* frameInfo)
{
	frameInfo->FrameDiffSignature = 0;

	unsigned char* buf = reinterpret_cast<unsigned char*>(bmpBits);

	float diffSignature;

	CalculateDiffSignature(buf, &diffSignature);

	idxFrameNumber++;
	
	bool isNewIntegrationPeriod = IsNewIntegrationPeriod(diffSignature);

	// After the integration has been 'locked' we only output a frame when a new integration period has been detected
	// When the integration hasn't been 'locked' we output every frame received from the camera
	bool showOutputFrame = isNewIntegrationPeriod || !INTEGRATION_LOCKED;

	if (showOutputFrame)
	{
		BufferNewIntegratedFrame(isNewIntegrationPeriod, currentUtcDayAsTicks);
		::ZeroMemory(integratedPixels, IMAGE_TOTAL_PIXELS * sizeof(double));

		if (isNewIntegrationPeriod)
		{
			numberOfIntegratedFrames = 0;

			idxFirstFrameNumber = idxFrameNumber;
			idxLastFrameNumber = 0;
		}
	}

	frameInfo->FrameDiffSignature  = diffSignature;
	//frameInfo->CurrentSignatureRatio  = NULL != integrationChecker ? integrationChecker->CurrentSignatureRatio : 0;

	long stride = 3 * IMAGE_WIDTH;
	unsigned char* ptrPixelItt = buf + (IMAGE_HEIGHT - 1) * IMAGE_STRIDE;

	double* ptrPixels = integratedPixels;

	unsigned char* ptrFirstOrLastFrameCopy = NULL;

	if (isNewIntegrationPeriod)
	{
		ptrFirstOrLastFrameCopy = firstIntegratedFramePixels;
		lastFrameWasNewIntegrationPeriod = true;
#if _DEBUG
		DebugViewPrint(L"Copying pixels of the %d-th frame to FirstIntegratedFramePixels", idxFrameNumber);
#endif
	}
	else
	{
		ptrFirstOrLastFrameCopy = lastIntegratedFramePixels;
		lastFrameWasNewIntegrationPeriod = false;
#if _DEBUG
		DebugViewPrint(L"Copying pixels of the %d-th frame to LastIntegratedFramePixels", idxFrameNumber);
#endif
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
		    *ptrPixels += thisPixel;

			ptrPixels++;
			ptrFirstOrLastFrameCopy++;
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

	return S_OK;
}

HRESULT ProcessVideoFrame(LPVOID bmpBits, __int64 currentUtcDayAsTicks, FrameProcessingStatus* frameInfo)
{
	if (USE_BUFFERED_FRAME_PROCESSING)
		return ProcessVideoFrameBuffered(bmpBits, currentUtcDayAsTicks, frameInfo);
	else
		return ProcessVideoFrameSynchronous(bmpBits, currentUtcDayAsTicks, frameInfo);
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

	AavFrameAddStatusTag16(STATUS_TAG_NUMBER_INTEGRATED_FRAMES, nextFrame->NumberOfIntegratedFrames);
	AavFrameAddStatusTag64(STATUS_TAG_START_FRAME_ID, nextFrame->StartFrameId);
	AavFrameAddStatusTag64(STATUS_TAG_END_FRAME_ID, nextFrame->EndFrameId);

	if (OCR_IS_SETUP)
	{
		AavFrameAddStatusTag(STATUS_TAG_START_TIMESTAMP, &nextFrame->StartTimeStampStr[0]);
		AavFrameAddStatusTag(STATUS_TAG_END_TIMESTAMP, &nextFrame->EndTimeStampStr[0]);
		AavFrameAddStatusTagUInt8(STATUS_TAG_GPS_TRACKED_SATELLITES, nextFrame->GpsTrackedSatellites);
		AavFrameAddStatusTagUInt8(STATUS_TAG_GPS_ALMANAC, nextFrame->GpsAlamancStatus);
		AavFrameAddStatusTagUInt8(STATUS_TAG_GPS_FIX, nextFrame->GpsFixStatus);
	}

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


HRESULT StartRecording(LPCTSTR szFileName)
{
	AavNewFile((const char*)szFileName);		
	
	AavAddFileTag("AAVR-SOFTWARE-VERSION", "1.0");
	AavAddFileTag("RECORDER", occuRecVersion);
	AavAddFileTag("FSTF-TYPE", "AAV");
	AavAddFileTag("AAV-VERSION", "1");
	
	AavAddFileTag("GRABBER", grabberName);
	AavAddFileTag("VIDEO-MODE", videoMode);
	AavAddFileTag("CAMERA-MODEL", cameraModel);

	if (OCR_IS_SETUP)
		AavAddFileTag("OCR-ENGINE", "IOTA-VTI OccuRec OCR v1.0");

	AavDefineImageSection(IMAGE_WIDTH, IMAGE_HEIGHT);
	
	AavDefineImageLayout(1, "FULL-IMAGE-RAW", "UNCOMPRESSED", 0, NULL);
	AavDefineImageLayout(2, "FULL-IMAGE-DIFFERENTIAL-CODING-NOSIGNS", "QUICKLZ", 32, "PREV-FRAME");
	AavDefineImageLayout(3, "FULL-IMAGE-DIFFERENTIAL-CODING", "QUICKLZ", 32, "PREV-FRAME");
	AavDefineImageLayout(4, "FULL-IMAGE-RAW", "QUICKLZ", 0, NULL);
	
	STATUS_TAG_NUMBER_INTEGRATED_FRAMES = AavDefineStatusSectionTag("IntegratedFrames", AavTagType::UInt16);
	STATUS_TAG_START_FRAME_ID = AavDefineStatusSectionTag("StartFrame", AavTagType::ULong64);
	STATUS_TAG_END_FRAME_ID = AavDefineStatusSectionTag("EndFrame", AavTagType::ULong64);
	STATUS_TAG_START_TIMESTAMP = AavDefineStatusSectionTag("StartFrameTimestamp", AavTagType::AnsiString255);
	STATUS_TAG_END_TIMESTAMP = AavDefineStatusSectionTag("EndFrameTimestamp", AavTagType::AnsiString255);
	STATUS_TAG_GPS_TRACKED_SATELLITES = AavDefineStatusSectionTag("GPSTrackedSatellites", AavTagType::UInt8);
	STATUS_TAG_GPS_ALMANAC = AavDefineStatusSectionTag("GPSAlmanacStatus", AavTagType::UInt8);
	STATUS_TAG_GPS_FIX = AavDefineStatusSectionTag("GPSFixStatus", AavTagType::UInt8);

	ClearRecordingBuffer();

	firstRecordedFrameTimestamp = 0;

	if (NULL != ocrManager)
	{
		ocrManager->ResetErrorCounter();
	}

	if (NULL == ocrManager || !ocrManager->IsReceivingTimeStamps())
	{
		// As a first frame add a non-integrated frame (to be able to tell the star-end timestamp order)

		IntegratedFrame* frame = new IntegratedFrame(IMAGE_TOTAL_PIXELS);
		memcpy(frame->Pixels, firstIntegratedFramePixels, IMAGE_TOTAL_PIXELS);

		frame->NumberOfIntegratedFrames = 0;
		frame->StartFrameId = -1;
		frame->EndFrameId = -1;
		frame->StartTimeStamp = 0;
		frame->EndTimeStamp = 0;
		frame->FrameNumber = -1;

		RecordCurrentFrame(frame);
	}

	recording = true;

	// Create a new thread
	hRecordingThread = (HANDLE)_beginthread(RecorderThreadProc, 0, NULL);

	return S_OK;
}

HRESULT StopRecording(long* pixels)
{
	recording = false;

	WaitForSingleObject(hRecordingThread, INFINITE); // wait for thread to exit

	if (NULL == ocrManager || !ocrManager->IsReceivingTimeStamps())
	{
		// As a last frame add a non-integrated frame (to be able to tell the star-end timestamp order)
		IntegratedFrame* frame = new IntegratedFrame(IMAGE_TOTAL_PIXELS);
		memcpy(frame->Pixels, firstIntegratedFramePixels, IMAGE_TOTAL_PIXELS);

		frame->NumberOfIntegratedFrames = 0;
		frame->StartFrameId = -1;
		frame->EndFrameId = -1;
		frame->StartTimeStamp = 0;
		frame->EndTimeStamp = 0;
		frame->FrameNumber = -1;
	
		RecordCurrentFrame(frame);
	}

	AavEndFile();

	return S_OK;
}

HRESULT DisableOcrProcessing()
{
	OCR_IS_SETUP = false;

	return S_OK;
}