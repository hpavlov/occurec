#include "StdAfx.h"

#include "AAVRec.Ocr.h"
#include "stdio.h"
#include <algorithm>

namespace AavOcr
{

	
vector<OcrCharDefinition*> OCR_CHAR_DEFS;

OcrCharDefinition::OcrCharDefinition(char character, long fixedPosition)
{
	Character = character;
	m_FixedPosition = fixedPosition;
}

OcrCharDefinition::~OcrCharDefinition()
{

}

void OcrCharDefinition::AddZoneEntry(long zoneId, long zoneBehaviour, long numPixelsInZone)
{
	OcrZoneEntry *entry = new OcrZoneEntry();
	entry->ZoneId = zoneId;
	entry->ZoneBehaviour = zoneBehaviour;
	entry->NumPixels = numPixelsInZone;
	
	ZoneEntries.push_back(entry);
}

bool OcrCharDefinition::IsMatch(long position)
{
	vector<OcrZoneEntry*>::iterator curr = ZoneEntries.begin();
	while (curr != ZoneEntries.end()) 
	{
		long zoneId = (*curr)->ZoneId;
		long zoneValue = (*curr)->ZoneBehaviour;
		
		// TODO: Check if this character is a match

		curr++;
	}

	return false;
}

OcrCharProcessor::OcrCharProcessor(OcrCharDefinition* charDef)
{
	vector<OcrZoneEntry*>::iterator itZones = charDef->ZoneEntries.begin();
	while(itZones != charDef->ZoneEntries.end())
	{
		OcrZoneProcessor* zoneProc = new OcrZoneProcessor();
		Zones.insert(make_pair((*itZones)->ZoneId, zoneProc));

		for (int i = 0; i < (*itZones)->NumPixels; i++)
		{
			zoneProc->ZonePixels.push_back(0);
		}

		itZones++;
	}
}

void OcrCharProcessor::NewFrame()
{
	map<long, OcrZoneProcessor*>::iterator itZoneProc = Zones.begin();
	while (itZoneProc != Zones.end()) 
	{
		OcrZoneProcessor* zoneProc = itZoneProc->second;

		for (int i = 0; i < zoneProc->ZonePixels.size(); i++)
		{
			zoneProc->ZonePixels[i] = 0;
		};

		itZoneProc++;
	}
}


void OcrCharProcessor::Ocr()
{
	map<long, OcrZoneProcessor*>::iterator itZoneProc = Zones.begin();
	while (itZoneProc != Zones.end()) 
	{
		OcrZoneProcessor* zoneProc = itZoneProc->second;

		// TODO: Compute the zone status for each zone

		itZoneProc++;
	}

	
	// TODO: Run the OCR logic against the OCR_CHAR_DEFS to recognize the char in the position
}


OcrFrameProcessor::OcrFrameProcessor(long ocrLinesFrom)
{
	m_OcrLinesFrom = ocrLinesFrom;

	OddFieldChars.clear();
	EvenFieldChars.clear();

	vector<OcrCharDefinition*>::iterator itCharDefs = OCR_CHAR_DEFS.begin();
	while (itCharDefs != OCR_CHAR_DEFS.end())
	{
		OcrCharProcessor* charProcEven = new OcrCharProcessor(*itCharDefs);
		OcrCharProcessor* charProcOdd = new OcrCharProcessor(*itCharDefs);

		EvenFieldChars.insert(make_pair((*itCharDefs)->Character, charProcEven));
		OddFieldChars.insert(make_pair((*itCharDefs)->Character, charProcOdd));

		itCharDefs++;
	}

	Success = false;
}

void OcrFrameProcessor::NewFrame()
{
	m_MedianComputationValues.clear();

	map<char,  OcrCharProcessor*>::iterator itCharProc = OddFieldChars.begin();
	while (itCharProc != OddFieldChars.end()) 
	{
		itCharProc->second->NewFrame();
		itCharProc++;
	}

	itCharProc = EvenFieldChars.begin();
	while (itCharProc != EvenFieldChars.end()) 
	{
		itCharProc->second->NewFrame();
		itCharProc++;
	}

	Success = false;
}

void UnpackValue(long packed, long* charId, bool* isOddField, long* zoneId, long* zonePixelId)
{
    *isOddField = (packed & 0x01000000) != 0;
    *charId = (packed >> 16) & 0xFF;
    *zoneId = (packed >> 8) & 0xFF;
    *zonePixelId = packed & 0xFF;
}

void OcrFrameProcessor::AddMedianComputationPixel(unsigned char pixelValue)
{
	m_MedianComputationValues.push_back(pixelValue);
}

void OcrFrameProcessor::ProcessZonePixel(long packedInfo, long pixX, long pixY, unsigned char pixelValue)
{
	long charId;
	bool isOddField;
	long zoneId;
	long zonePixelId;
	UnpackValue(packedInfo, &charId, &isOddField, &zoneId, &zonePixelId);

    OcrCharProcessor* ocredChar = isOddField ? OddFieldChars[charId] : EvenFieldChars[charId];

    ocredChar->Zones[zoneId]->ZonePixels[zonePixelId] = pixelValue;
}

void OcrFrameProcessor::Ocr()
{
	if (m_MedianComputationValues.size() > 0)
	{
		std::sort(m_MedianComputationValues.begin(), m_MedianComputationValues.end());
		unsigned char medianValue = m_MedianComputationValues[m_MedianComputationValues.size() / 2];

		map<char,  OcrCharProcessor*>::iterator itCharProc = OddFieldChars.begin();
		while (itCharProc != OddFieldChars.end()) 
		{
			itCharProc->second->Ocr();
			itCharProc++;
		}

		itCharProc = EvenFieldChars.begin();
		while (itCharProc != EvenFieldChars.end()) 
		{
			itCharProc->second->Ocr();
			itCharProc++;
		}
	}
}

bool OcrFrameProcessor::IsStartTimeStampFirst()
{
	return false;
}

long OcrFrameProcessor::GetOcredStartFrameNumber()
{
	return 0;
}

long long OcrFrameProcessor::GetOcredStartFrameTimeStamp()
{
	return 0;
}

long OcrFrameProcessor::GetOcredEndFrameNumber()
{
	return 0;
}

long long OcrFrameProcessor::GetOcredEndFrameTimeStamp()
{
	return 0;
}

long OcrFrameProcessor::GetOcredTrackedSatellitesCount()
{
	return 0;
}

long OcrFrameProcessor::GetOcredAlmanacUpdateState()
{
	return 0;
}

long OcrFrameProcessor::GetOcredGpsFixType()
{
	return 0;
}


}

