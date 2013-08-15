#include "StdAfx.h"

#include "OccuRec.Ocr.h"
#include "stdio.h"
#include <algorithm>
#include <functional>
#include <numeric>
#include "utils.h"
#include <time.h>


namespace OccuOcr
{

	
vector<OcrCharDefinition*> OCR_CHAR_DEFS;
long OCR_NUMBER_OF_CHAR_POSITIONS;
long OCR_NUMBER_OF_ZONES;
long OCR_ZONE_PIXEL_COUNTS[MAX_ZONE_COUNT];

OcrCharDefinition::OcrCharDefinition(char character, long fixedPosition)
{
	Character = character;
	FixedPosition = fixedPosition;
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

Zone::Zone(long zonePixelsCount)
{
	ZonePixelsCount = zonePixelsCount;

	if (zonePixelsCount > MAX_PIXELS_IN_ZONE_COUNT)
		throw std::exception("Zone can only have up to MAX_PIXELS_IN_ZONE_COUNT pixels");

	for (int i = 0; i < MAX_PIXELS_IN_ZONE_COUNT; i++)
	{
		ZonePixels[i] = 0;
	}
}

CharRecognizer::CharRecognizer(long charPosition)
{
	for (int i = 0; i < MAX_ZONE_COUNT; i++)
	{
		Zones[i] = NULL;
	}

	for (int i = 0; i < OCR_NUMBER_OF_ZONES; i++)
	{
		long numPixelsInZone = OCR_ZONE_PIXEL_COUNTS[i];
		Zone* zoneProc = new Zone(numPixelsInZone);

		Zones[i] = zoneProc;

		for (int i = 0; i < numPixelsInZone; i++)
		{
			zoneProc->ZonePixels[i] = 0;
		}
	}

	m_CharPosition = charPosition;
}

void CharRecognizer::NewFrame()
{
	for (int i = 0; i < MAX_ZONE_COUNT; i++)
	{
		if (NULL != Zones[i])
		{
			for (int j = 0; j < Zones[i]->ZonePixelsCount; j++)
			{
				Zones[i]->ZonePixels[j] = 0;
			}
		}
	}
}

char CharRecognizer::Ocr(long frameMedian)
{
	for (int i = 0; i < MAX_ZONE_COUNT; i++)
	{
		if (NULL != Zones[i])
		{
			Zone* zoneProc = Zones[i];

			long sum = 0;
			for (int j = 0; j < zoneProc->ZonePixelsCount; j++)
			{
				sum += zoneProc->ZonePixels[j];
			}

			zoneProc->ZoneMean = sum / zoneProc->ZonePixelsCount;
		}
	}

	long MIN_ON_VALUE = 220;
	long MAX_OFF_VALUE = frameMedian + (MIN_ON_VALUE - frameMedian) / 4;

	vector<OcrCharDefinition*>::iterator itCharDef = OCR_CHAR_DEFS.begin();
	while(itCharDef != OCR_CHAR_DEFS.end())
	{
		if ((*itCharDef)->FixedPosition > -1 &&
			(*itCharDef)->FixedPosition != m_CharPosition)
		{
			itCharDef++;
			continue;
		}

		bool isMatch = true;

		vector<OcrZoneEntry*>::iterator itZoneConfig = (*itCharDef)->ZoneEntries.begin();
		while(itZoneConfig != (*itCharDef)->ZoneEntries.end())
		{
			long zoneBEhaviour = (*itZoneConfig)->ZoneBehaviour;

			Zone* zoneById = Zones[(*itZoneConfig)->ZoneId];

			if (NULL != zoneById)
			{
				unsigned char zoneValue = zoneById->ZoneMean;

				if (zoneBEhaviour == ZoneBehaviour::On && zoneValue < MIN_ON_VALUE)
				{
					isMatch = false;
					break;
				}

				if (zoneBEhaviour == ZoneBehaviour::Off && zoneValue >= MAX_OFF_VALUE)
				{
					isMatch = false;
					break;
				}

				if (zoneBEhaviour == ZoneBehaviour::Gray && (zoneValue < MAX_OFF_VALUE || zoneValue > MIN_ON_VALUE))
				{
					isMatch = false;
					break;
				}

				if (zoneBEhaviour == ZoneBehaviour::NotOn && zoneValue > MIN_ON_VALUE)
				{
					isMatch = false;
					break;
				}

				if ((*itZoneConfig)->ZoneBehaviour == ZoneBehaviour::NotOff && zoneValue < MAX_OFF_VALUE)
				{
					isMatch = false;
					break;
				}
			}

			itZoneConfig++;
		}

		if (isMatch)
			return (*itCharDef)->Character;

		itCharDef++;
	}
	
	return 0;
}


OcrFrameProcessor::OcrFrameProcessor()
{
	OddFieldChars.clear();
	EvenFieldChars.clear();

	for (int position = 0; position < OCR_NUMBER_OF_CHAR_POSITIONS; position++)
	{
		CharRecognizer* charProcEven = new CharRecognizer(position);
		CharRecognizer* charProcOdd = new CharRecognizer(position);

		EvenFieldChars[position] = charProcEven;
		OddFieldChars[position] = charProcOdd;
	}

	Success = false;
}

void OcrFrameProcessor::NewFrame()
{
	m_MedianComputationValues.clear();

	map<char,  CharRecognizer*>::iterator itCharProc = OddFieldChars.begin();
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

	if (charId >= 0 && EvenFieldChars.size() > charId)
	{
		// OddFieldChars and EvenFieldChars should be an array ??
		CharRecognizer* ocredChar = isOddField ? OddFieldChars[charId] : EvenFieldChars[charId];

		ocredChar->Zones[zoneId]->ZonePixels[zonePixelId] = pixelValue;
	}
}

void OcrFrameProcessor::Ocr(__int64 currentUtcDayAsTicks)
{
	if (m_MedianComputationValues.size() > 0)
	{
		std::sort(m_MedianComputationValues.begin(), m_MedianComputationValues.end());
		long medianValue = m_MedianComputationValues[m_MedianComputationValues.size() / 2];

		int position = 0;
		map<char,  CharRecognizer*>::iterator itCharProc = OddFieldChars.begin();
		while (itCharProc != OddFieldChars.end()) 
		{
			char chr = itCharProc->second->Ocr(medianValue);
			m_OcredCharsOdd[position] = chr;

			itCharProc++;
			position++;
		}
		m_OcredCharsOdd[position] = 0;

		position = 0;
		itCharProc = EvenFieldChars.begin();
		while (itCharProc != EvenFieldChars.end()) 
		{
			char chr = itCharProc->second->Ocr(medianValue);
			m_OcredCharsEven[position] = chr;

			itCharProc++;
			position++;
		}
		m_OcredCharsEven[position] = 0;

		ExtractFieldInfo(m_OcredCharsOdd, currentUtcDayAsTicks, OddFieldOcredOsd);
		ExtractFieldInfo(m_OcredCharsEven, currentUtcDayAsTicks, EvenFieldOcredOsd);
	}
}

void OcrFrameProcessor::ExtractFieldInfo(char ocredChars[25], __int64 currentUtcDayAsTicks, OcredFieldOsd& fieldInfo)
{
	bool success = true;

	fieldInfo.GpsFixType = ocredChars[0];
	if (ocredChars[1] > 0)
		fieldInfo.TrackedSatellites = ocredChars[1] - 0x30;
	else
	{
		fieldInfo.TrackedSatellites = 0;
		success = false;
	}

	fieldInfo.AlmanacUpdateState = ocredChars[2] != 'W';

	long hh = 0;
	if (ocredChars[3] > 0 && ocredChars[4] > 0)
		hh = 10 * (ocredChars[3] - 0x30) + (ocredChars[4] - 0x30);
	else
		success = false;

	long mm = 0;
	if (ocredChars[5] > 0 && ocredChars[6] > 0)
		mm = 10 * (ocredChars[5] - 0x30) + (ocredChars[6] - 0x30);
	else
		success = false;

	long ss = 0;
	if (ocredChars[7] > 0 && ocredChars[8] > 0)
		ss = 10 * (ocredChars[7] - 0x30) + (ocredChars[8] - 0x30);
	else
		success = false;
        
	long ms1 = 0;
	long ms2 = 0;

	if (ocredChars[9] > 0 && ocredChars[10] > 0 && ocredChars[11] > 0 && ocredChars[12] > 0)
		ms1 = 1000 * (ocredChars[9] - 0x30) + 100 * (ocredChars[10] - 0x30) + 10 * (ocredChars[11] - 0x30) + (ocredChars[12] - 0x30);
	else if (ocredChars[13] > 0 && ocredChars[14] > 0 && ocredChars[15] > 0 && ocredChars[16] > 0)
		ms2 = 1000 * (ocredChars[13] - 0x30) + 100 * (ocredChars[14] - 0x30) + 10 * (ocredChars[15] - 0x30) + (ocredChars[16] - 0x30);
	else
		success = false;

	// NOTE: This will be problematic at date change. 
	// TODO: Need to some something more to ensure we are capturing and processing correctly the case of date change
	// 0.1ms = 1000 ticks
	//   1ms = 10000 ticks
	//  1sec = 10000000 ticks
	//  1min = 600000000 ticks
	// 1hour = 36000000000 ticks
	fieldInfo.FieldTimeStamp = 
		currentUtcDayAsTicks +
		(long long)(36000000000) * (long long) hh + 
		  (long long)(600000000) * (long long) mm + 
		   (long long)(10000000) * (long long) ss +
		       (long long)(1000) * (long long) (ms1 != 0 ? ms1 : ms2);

	char fieldNoStr[7];
	::ZeroMemory(fieldNoStr, 7);
	char* fieldNoPtr = &fieldNoStr[0];
	bool fieldNoBeginingFound = false;
	bool gapFound = false;
	for (int i = 17; i < 23; i++)
	{
		if (ocredChars[i] != 0)
		{
			if (gapFound)
			{
				fieldNoBeginingFound = false;
				break;
			}

			*fieldNoPtr = ocredChars[i];
			fieldNoPtr++;
			fieldNoBeginingFound = true;
		}
		else
		{
			if (fieldNoBeginingFound)
				gapFound = true;
		}
	}

	if (!fieldNoBeginingFound)
		success = false;

	fieldInfo.FieldNumber = atoi(fieldNoStr);

	Success = success;
	m_IsOddFieldDataFirst = 
		Success && 
		OddFieldOcredOsd.FieldNumber < EvenFieldOcredOsd.FieldNumber;
}

bool OcrFrameProcessor::IsOddFieldDataFirst()
{
	return m_IsOddFieldDataFirst;
}

long OcrFrameProcessor::GetOcredStartFrameNumber()
{
	return m_IsOddFieldDataFirst 
		? OddFieldOcredOsd.FieldNumber 
		: EvenFieldOcredOsd.FieldNumber;
}

__int64 OcrFrameProcessor::GetOcredStartFrameTimeStamp(long fieldDurationInTicks)
{
	__int64 endFirstFieldTimestamp = m_IsOddFieldDataFirst 
		? OddFieldOcredOsd.FieldTimeStamp 
		: EvenFieldOcredOsd.FieldTimeStamp;

	// The frame exposure actually starts 1 field before the first field timestamp
	return endFirstFieldTimestamp - fieldDurationInTicks;
}

long OcrFrameProcessor::GetOcredEndFrameNumber()
{
	return m_IsOddFieldDataFirst 
		? EvenFieldOcredOsd.FieldNumber 
		: OddFieldOcredOsd.FieldNumber;
}

__int64 OcrFrameProcessor::GetOcredEndFrameTimeStamp()
{
	return m_IsOddFieldDataFirst 
		? EvenFieldOcredOsd.FieldTimeStamp 
		: OddFieldOcredOsd.FieldTimeStamp;
}

void OcrFrameProcessor::GetOcredStartFrameTimeStampStr(char* destBuffer)
{
	if (m_IsOddFieldDataFirst)
		SafeCopyCharsReplaceZeroWithSpace(destBuffer, &m_OcredCharsOdd[0], 25);
	else
		SafeCopyCharsReplaceZeroWithSpace(destBuffer, &m_OcredCharsEven[0], 25);
}

void OcrFrameProcessor::GetOcredEndFrameTimeStampStr(char* destBuffer)
{
	if (m_IsOddFieldDataFirst)
		SafeCopyCharsReplaceZeroWithSpace(destBuffer, &m_OcredCharsEven[0], 25);
	else
		SafeCopyCharsReplaceZeroWithSpace(destBuffer, &m_OcredCharsOdd[0], 25);
}

void OcrFrameProcessor::SafeCopyCharsReplaceZeroWithSpace(char* destBuffer, char* source, int len)
{
	int j = 0;
	for (int i = 0; i < len; i++, j++)
	{
		if (source[i] != '\0')
			destBuffer[j] = source[i];
		else
			destBuffer[j] = ' ';

		if (i == 1 || i == 2 || i == 8 || i == 16)
		{
			j++; destBuffer[j] = ' ';
		}
		else if (i == 4 || i == 6)
		{
			j++; destBuffer[j] = ':';
		}
	}
	destBuffer[j + 1] = '\0';
}

unsigned char OcrFrameProcessor::GetOcredTrackedSatellitesCount()
{
	return (unsigned char)(OddFieldOcredOsd.TrackedSatellites & 0xFF);
}

unsigned char OcrFrameProcessor::GetOcredAlmanacUpdateState()
{
	
	return (unsigned char)(OddFieldOcredOsd.AlmanacUpdateState & 0xFF);
}

unsigned char OcrFrameProcessor::GetOcredGpsFixState()
{
	switch(OddFieldOcredOsd.GpsFixType)
	{
		case 'G':
			return (unsigned char)1;

		case 'P':
			return (unsigned char)2;

		default:
			return (unsigned char)0;
	}
}

char OcrFrameProcessor::GetOcredGpsFixType()
{
	return OddFieldOcredOsd.GpsFixType;
}

OcrManager::OcrManager()
{
	OcrErrorsSinceReset = 0;
	receivingTimestamps = false;
};

void OcrManager::Reset()
{
	OcrErrorsSinceReset = 0;
	FieldDurationInTicks = 0;
	receivingTimestamps = false;
};

void OcrManager::ResetErrorCounter()
{
	OcrErrorsSinceReset = 0;
};

void OcrManager::RegisterFirstSuccessfullyOcredFrame(OcrFrameProcessor* ocredFrame)
{
	frameIdOddBeforeEven = ocredFrame->IsOddFieldDataFirst();
	prevStartFieldId = ocredFrame->GetOcredStartFrameNumber();
	prevStartTimeStamp = ocredFrame-> GetOcredStartFrameTimeStamp(0);

	FieldDurationInTicks = abs(ocredFrame->GetOcredEndFrameTimeStamp() - ocredFrame->GetOcredStartFrameTimeStamp(0));

	receivingTimestamps = true;
};

void OcrManager::VerifyAndFixOcredFrame(OcrFrameProcessor* ocredFrame)
{
	// TODO:
};

void OcrManager::VerifyAndFixOcredIntegratedInterval(OcrFrameProcessor* firstOcredFrame, OcrFrameProcessor* lastOcredFrame)
{
	// TODO:
};

bool OcrManager::IsReceivingTimeStamps()
{
	return receivingTimestamps;
};

void OcrManager::ProcessedOcredFrame(OcrFrameProcessor* ocredFrame)
{
	if (receivingTimestamps)
	{
		VerifyAndFixOcredFrame(ocredFrame);

		if (!ocredFrame->Success)
			OcrErrorsSinceReset++;
	}
};

void OcrManager::ProcessedOcredFramesInLockedMode(OcrFrameProcessor* firstOcredFrame, OcrFrameProcessor* lastOcredFrame)
{
	if (receivingTimestamps)
	{
		VerifyAndFixOcredIntegratedInterval(firstOcredFrame, lastOcredFrame);

		if (!firstOcredFrame->Success)
			OcrErrorsSinceReset++;

		if (!lastOcredFrame->Success)
			OcrErrorsSinceReset++;
	}
};

}

