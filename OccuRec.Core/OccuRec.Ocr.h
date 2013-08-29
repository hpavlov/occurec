#ifndef OCCURECOCR_H
#define OCCURECOCR_H

#include <vector>
#include <map>
#include <stdio.h>

using namespace std;

#define MAX_ZONE_COUNT 16
#define MAX_PIXELS_IN_ZONE_COUNT 32

namespace OccuOcr
{

// Types of 'values' that a zone can have when matched against a configuration in order to recognize a character
enum ZoneBehaviour
{
  On = 0,
  Off = 1,
  Gray = 2,
  NotOn = 3,
  NotOff = 4,
  OnOff = 5,
  OffOn = 6,
  NotOnOff = 7,
  NotOffOn = 8
};

enum ZoneMode
{
  Standard = 0,
  SplitZones = 1
};

enum OcrErrorCode
{
	Unknown = 0,
	Success = 0,
	TrackedSatellitesInvalid = 0x0001,
	WaitFlagInvalid = 0x0002,
	HoursInvalid = 0x0004,
	MinutesInvalid = 0x0008,
	SecondsInvalid = 0x0010,
	MillisecondsInvalid = 0x0020,
	FieldNumberInvalid = 0x0040,
	FieldNumbersNotSequential = 0x0080,
	FrameDurationNotPALorNTSC = 0x0100
};

// 
class OcrZoneEntry
{
	public:
		long ZoneId;
		long ZoneBehaviour;
		long NumPixels;
};

// 
class OcrZoneValue
{
	public:
		long ZoneId;
		long AccumulatedZoneValue;
};

// Configuration of the zone values that match to a specific character that can be recognized
class OcrCharDefinition 
{
	public:
		char Character;
		long FixedPosition;
		vector<OcrZoneEntry*> ZoneEntries;

		OcrCharDefinition(char character, long fixedPosition);
		~OcrCharDefinition();

		void AddZoneEntry(long zoneId, long zoneBehaviour, long numPixelsInZone);
};

// Holds the pixel values for a single zone of a single character position
class Zone
{
	public:
		unsigned char ZoneMean;
		unsigned char ZoneTopMean;
		unsigned char ZoneBottomMean;
		long ZonePixelsCount;

		Zone(long zonePixelsCount);

		unsigned char ZonePixels[MAX_PIXELS_IN_ZONE_COUNT];
};

// Recognizes a single character
class CharRecognizer
{
	private:
		long m_CharPosition;
		void ComputeFullZones();
		void ComputeSplitZones();
		char OcrSplitZones();
		char OcrStandardZones(long frameMedian);

	public:
		Zone* Zones[MAX_ZONE_COUNT];

		CharRecognizer(long charPosition);

		void NewFrame();
		char Ocr(long medianValue);
};

// Holds the OCR-ed information from a video field
class OcredFieldOsd
{
	public:
		long FieldNumber;
		__int64 FieldTimeStamp;
		long TrackedSatellites;
		long AlmanacUpdateState;
		char GpsFixType;
};

// Processes each video frame and OCRs data from the video fields in the frame
class OcrFrameProcessor
{
	private:
		vector<unsigned char> m_MedianComputationValues;
		map<char, CharRecognizer*> OddFieldChars;
		map<char, CharRecognizer*> EvenFieldChars;
		bool m_IsOddFieldDataFirst;

		char m_OcredCharsOdd[25];
		char m_OcredCharsEven[25];

		OcredFieldOsd OddFieldOcredOsd;
		OcredFieldOsd EvenFieldOcredOsd;

		OcrErrorCode ExtractFieldInfo(char ocredChars[25], __int64 currentUtcDayAsTicks, OcredFieldOsd& fieldInfo);
		void SafeCopyCharsReplaceZeroWithSpace(char* destBuffer, char* source, int len);

	public:
		bool Success();
		OcrErrorCode ErrorCodeOddField;
		OcrErrorCode ErrorCodeEvenField;

		OcrFrameProcessor();
		~OcrFrameProcessor();

		void NewFrame();
		void AddMedianComputationPixel(unsigned char pixelValue);
		void ProcessZonePixel(long packedInfo, long pixX, long pixY, unsigned char pixelValue);
		void Ocr(__int64 currentUtcDayAsTicks);
		bool IsOddFieldDataFirst();
		long GetOcredStartFrameNumber();
		__int64 GetOcredStartFrameTimeStamp(long fieldDurationInTicks);
		long GetOcredEndFrameNumber();
		__int64 GetOcredEndFrameTimeStamp();

		void GetOcredStartFrameTimeStampStr(char* destBuffer);
		void GetOcredEndFrameTimeStampStr(char* destBuffer);

		unsigned char GetOcredTrackedSatellitesCount();
		unsigned char GetOcredAlmanacUpdateState();
		char GetOcredGpsFixType();
		unsigned char GetOcredGpsFixState();
};

class OcrManager
{
	private:
		bool receivingTimestamps;
		bool frameIdOddBeforeEven;
		long prevStartFieldId;
		long long prevStartTimeStamp;

		void VerifyOcredFrameIntegrity(OcrFrameProcessor* ocredFrame);
		void VerifyOcredIntegratedIntervalIntegrity(OcrFrameProcessor* firstOcredFrame, OcrFrameProcessor* lastOcredFrame);

	public:
		long OcrErrorsSinceReset;
		long FieldDurationInTicks;

		OcrManager();

		void Reset();
		void ResetErrorCounter();

		void RegisterFirstSuccessfullyOcredFrame(OcrFrameProcessor* ocredFrame);
		void ProcessedOcredFrame(OcrFrameProcessor* ocredFrame);
		void ProcessedOcredFramesInLockedMode(OcrFrameProcessor* firstOcredFrame, OcrFrameProcessor* lastOcredFrame);
		bool IsReceivingTimeStamps();
};

extern vector<OcrCharDefinition*> OCR_CHAR_DEFS;
extern long OCR_NUMBER_OF_CHAR_POSITIONS;
extern long OCR_NUMBER_OF_ZONES;
extern long OCR_ZONE_MODE;
extern long OCR_ZONE_PIXEL_COUNTS[MAX_ZONE_COUNT];

void UnpackValue(long packed, long* charId, bool* isOddField, long* zoneId, long* zonePixelId);

}

#endif // OCCURECOCR_H
