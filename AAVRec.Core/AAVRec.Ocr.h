#ifndef AAVRECOCR_H
#define AAVRECOCR_H

#include <vector>
#include <map>
#include <stdio.h>

using namespace std;

namespace AavOcr
{

enum ZoneBehaviour
{
  On = 0,
  Off = 1,
  Gray = 2,
  NotOn = 3,
  NotOff = 4
};

class OcrZoneEntry
{
	public:
		long ZoneId;
		long ZoneBehaviour;
		long NumPixels;
};

class OcrZoneValue
{
	public:
		long ZoneId;
		long AccumulatedZoneValue;
};

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

class OcrZoneProcessor
{
	public:
		unsigned char ZoneMean;
		OcrZoneEntry* ZoneConfig;

		OcrZoneProcessor(OcrZoneEntry* zoneConfig);

		vector<unsigned char> ZonePixels;
};

class OcrCharProcessor
{
	private:
		long m_CharPosition;

	public:
		map<long, OcrZoneProcessor*> Zones;

		OcrCharProcessor(OcrCharDefinition* charDef, long charPosition);

		void NewFrame();
		char Ocr(long medianValue);
};

class OcredFieldOsd
{
	public:
		long FieldNumber;
		__int64 FieldTimeStamp;
		long TrackedSatellites;
		long AlmanacUpdateState;
		char GpsFixType;
};

class OcrFrameProcessor
{
	private:
		vector<unsigned char> m_MedianComputationValues;
		map<char, OcrCharProcessor*> OddFieldChars;
		map<char, OcrCharProcessor*> EvenFieldChars;
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

}

#endif // AAVRECOCR_H
