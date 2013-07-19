#ifndef AAVRECOCR_H
#define AAVRECOCR_H

#include <vector>
#include <map>
#include <stdio.h>

using namespace std;

namespace AavOcr
{

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
	private:		
		long m_FixedPosition;

	public:
		char Character;
		vector<OcrZoneEntry*> ZoneEntries;

		OcrCharDefinition(char character, long fixedPosition);
		~OcrCharDefinition();

		void AddZoneEntry(long zoneId, long zoneBehaviour, long numPixelsInZone);
		bool IsMatch(long position);
};

class OcrZoneProcessor
{
	public:
		vector<unsigned char> ZonePixels;
};

class OcrCharProcessor
{
	public:
		map<long, OcrZoneProcessor*> Zones;

		OcrCharProcessor(OcrCharDefinition* charDef);

		void NewFrame();
		void Ocr();
};

class OcrFrameProcessor
{
	private:
		vector<unsigned char> m_MedianComputationValues;
		map<char, OcrCharProcessor*> OddFieldChars;
		map<char, OcrCharProcessor*> EvenFieldChars;
		long m_OcrLinesFrom;

	public:
		bool Success;

		OcrFrameProcessor(long ocrLinesFrom);

		void NewFrame();
		void AddMedianComputationPixel(unsigned char pixelValue);
		void ProcessZonePixel(long packedInfo, long pixX, long pixY, unsigned char pixelValue);
		void Ocr();		
		bool IsStartTimeStampFirst();
		long GetOcredStartFrameNumber();
		long long GetOcredStartFrameTimeStamp();
		long GetOcredEndFrameNumber();
		long long GetOcredEndFrameTimeStamp();
		long GetOcredTrackedSatellitesCount();
		long GetOcredAlmanacUpdateState();
		long GetOcredGpsFixType();
};

extern vector<OcrCharDefinition*> OCR_CHAR_DEFS;

}

#endif // AAVRECOCR_H
