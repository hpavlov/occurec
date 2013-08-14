#ifndef OCCURECOCR_H
#define OCCURECOCR_H

#include <vector>
#include <map>
#include <stdio.h>

using namespace std;

#define MAX_ZONE_COUNT 16
#define MAX_PIXELS_IN_ZONE_COUNT 32

namespace AavOcr
{

// Types of 'values' that a zone can have when matched against a configuration in order to recognize a character
enum ZoneBehaviour
{
  On = 0,
  Off = 1,
  Gray = 2,
  NotOn = 3,
  NotOff = 4
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
		long ZonePixelsCount;

		Zone(long zonePixelsCount);

		unsigned char ZonePixels[MAX_PIXELS_IN_ZONE_COUNT];
};

// Recognizes a single character
class CharRecognizer
{
	private:
		long m_CharPosition;

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

		void ExtractFieldInfo(char ocredChars[25], __int64 currentUtcDayAsTicks, OcredFieldOsd& fieldInfo);

	public:
		bool Success;

		OcrFrameProcessor();

		void NewFrame();
		void AddMedianComputationPixel(unsigned char pixelValue);
		void ProcessZonePixel(long packedInfo, long pixX, long pixY, unsigned char pixelValue);
		void Ocr(__int64 currentUtcDayAsTicks);
		bool IsStartTimeStampFirst();
		long GetOcredStartFrameNumber();
		__int64 GetOcredStartFrameTimeStamp();
		long GetOcredEndFrameNumber();
		__int64 GetOcredEndFrameTimeStamp();
		long GetOcredTrackedSatellitesCount();
		long GetOcredAlmanacUpdateState();
		char GetOcredGpsFixType();
};

extern vector<OcrCharDefinition*> OCR_CHAR_DEFS;
extern long OCR_NUMBER_OF_CHAR_POSITIONS;
extern long OCR_NUMBER_OF_ZONES;
extern long OCR_ZONE_PIXEL_COUNTS[MAX_ZONE_COUNT];

void UnpackValue(long packed, long* charId, bool* isOddField, long* zoneId, long* zonePixelId);

}

#endif // OCCURECOCR_H
