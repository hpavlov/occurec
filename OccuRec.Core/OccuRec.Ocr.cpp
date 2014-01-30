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
long OCR_ZONE_MODE;
long OCR_ZONE_PIXEL_COUNTS[MAX_ZONE_COUNT];

OcrCharDefinition::OcrCharDefinition(char character, long fixedPosition)
{
	Character = character;
	FixedPosition = fixedPosition;
}

OcrCharDefinition::~OcrCharDefinition()
{
	ZoneEntries.clear();
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

void CharRecognizer::ComputeFullZones()
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
}

void CharRecognizer::ComputeSplitZones()
{
	for (int i = 0; i < MAX_ZONE_COUNT; i++)
	{
		if (NULL != Zones[i])
		{
			Zone* zoneProc = Zones[i];

			int halfZoneLen = zoneProc->ZonePixelsCount / 2;

			long sumTop = 0;
			long sumBottom = 0;
			for (int j = 0; j < zoneProc->ZonePixelsCount; j++)
			{
				unsigned char zonePixel = zoneProc->ZonePixels[j];

				if (j < halfZoneLen)
					sumTop += zonePixel;
				else
					sumBottom += zonePixel;
			}

			zoneProc->ZoneTopMean = sumTop / halfZoneLen;
			zoneProc->ZoneBottomMean = sumBottom / halfZoneLen;
		}
	}
}

char CharRecognizer::Ocr(long frameMedian)
{
	if (OCR_ZONE_MODE == ZoneMode::Standard)
	{
		ComputeFullZones();
		return OcrStandardZones(frameMedian);
	}
	else if (OCR_ZONE_MODE == ZoneMode::SplitZones)
	{
		ComputeSplitZones();
		return OcrSplitZones();
	}
	else
		throw new exception("Unsupported ZoneMode");
}

char CharRecognizer::OcrSplitZones()
{
	long MIN_ON_VALUE = 200;
	long MAX_OFF_VALUE = 100;

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
				bool isOnOff = zoneById->ZoneTopMean >= MIN_ON_VALUE && zoneById->ZoneBottomMean < MAX_OFF_VALUE;
				bool isOffOn = zoneById->ZoneTopMean < MAX_OFF_VALUE && zoneById->ZoneBottomMean >= MIN_ON_VALUE;

				if (zoneBEhaviour == ZoneBehaviour::OnOff && !isOnOff)
				{
					isMatch = false;
					break;
				}

				if (zoneBEhaviour == ZoneBehaviour::OffOn && !isOffOn)
				{
					isMatch = false;
					break;
				}

				if (zoneBEhaviour == ZoneBehaviour::NotOffOn && isOffOn)
				{
					isMatch = false;
					break;
				}

				if (zoneBEhaviour == ZoneBehaviour::NotOnOff && isOnOff)
				{
					isMatch = false;
					break;
				}
			}
			else
			{
				DebugViewPrint(L"Cannot find zone id %d", (*itZoneConfig)->ZoneId);
				isMatch = false;
				break;
			}

			itZoneConfig++;
		}

		if (isMatch)
			return (*itCharDef)->Character;

		itCharDef++;
	}
	
	return ' ';
}

char CharRecognizer::OcrStandardZones(long frameMedian)
{
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
			else
			{
				DebugViewPrint(L"Cannot find zone id %d", (*itZoneConfig)->ZoneId);
				isMatch = false;
				break;
			}

			itZoneConfig++;
		}

		if (isMatch)
			return (*itCharDef)->Character;

		itCharDef++;
	}
	
	return ' ';
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

	ErrorCodeEvenField = OcrErrorCode::Unknown;
	ErrorCodeOddField = OcrErrorCode::Unknown;
}

OcrFrameProcessor::~OcrFrameProcessor()
{
	OddFieldChars.clear();
	EvenFieldChars.clear();
}

void OcrFrameProcessor::NewFrame()
{
	m_MedianComputationValues.clear();

	int position = 0;
	map<char,  CharRecognizer*>::iterator itCharProc = OddFieldChars.begin();
	while (itCharProc != OddFieldChars.end()) 
	{
		itCharProc->second->NewFrame();
		m_OcredCharsOdd[position] = ' ';

		itCharProc++;
		position++;
	}

	OddFieldOcredOsd.FieldNumber = 0;
	OddFieldOcredOsd.FieldTimeStamp = 0;
	OddFieldOcredOsd.TrackedSatellites = 0;
	OddFieldOcredOsd.GpsFixType = 0;
	OddFieldOcredOsd.AlmanacUpdateState = 0;

	position = 0;
	itCharProc = EvenFieldChars.begin();
	while (itCharProc != EvenFieldChars.end()) 
	{
		itCharProc->second->NewFrame();
		m_OcredCharsEven[position] = ' ';

		itCharProc++;
		position++;
	}
	
	EvenFieldOcredOsd.FieldNumber = 0;
	EvenFieldOcredOsd.FieldTimeStamp = 0;
	EvenFieldOcredOsd.TrackedSatellites = 0;
	EvenFieldOcredOsd.GpsFixType = 0;
	EvenFieldOcredOsd.AlmanacUpdateState = 0;

	ErrorCodeEvenField = OcrErrorCode::Unknown;
	ErrorCodeOddField = OcrErrorCode::Unknown;
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
		size_t n = m_MedianComputationValues.size() / 2;
		std::nth_element(m_MedianComputationValues.begin(), m_MedianComputationValues.begin()+n, m_MedianComputationValues.end());
		long medianValue = m_MedianComputationValues[n];

		int position = 0;
		map<char,  CharRecognizer*>::iterator itCharProc = OddFieldChars.begin();
		while (itCharProc != OddFieldChars.end()) 
		{
			char chr = itCharProc->second->Ocr(medianValue);
			m_OcredCharsOdd[position] = chr;

			itCharProc++;
			position++;
		}
		m_OcredCharsOdd[position] = ' ';

		position = 0;
		itCharProc = EvenFieldChars.begin();
		while (itCharProc != EvenFieldChars.end()) 
		{
			char chr = itCharProc->second->Ocr(medianValue);
			m_OcredCharsEven[position] = chr;

			itCharProc++;
			position++;
		}
		m_OcredCharsEven[position] = ' ';

		ErrorCodeOddField = ExtractFieldInfo(m_OcredCharsOdd, currentUtcDayAsTicks, OddFieldOcredOsd);
		ErrorCodeEvenField = ExtractFieldInfo(m_OcredCharsEven, currentUtcDayAsTicks, EvenFieldOcredOsd);

		m_IsOddFieldDataFirst = 
			OddFieldOcredOsd.FieldNumber < EvenFieldOcredOsd.FieldNumber;

		//char tsFrom[128];
		//char tsTo[128];
		//GetOcredStartFrameTimeStampStr(&tsFrom[0]);
		//GetOcredEndFrameTimeStampStr(&tsTo[0]);
		//std::wstring wcFrom( 256, L'#' );
		//mbstowcs( &wcFrom[0], tsFrom, 128 );
		//std::wstring wcTo( 256, L'#' );
		//mbstowcs( &wcTo[0], tsTo, 128 );
		//DebugViewPrint(L"%s to %s)", &wcFrom[0], &wcTo[0]);
	}
}

bool OcrFrameProcessor::Success()
{
	return
		ErrorCodeOddField == OcrErrorCode::Success && 
		ErrorCodeEvenField == OcrErrorCode::Success;
}
OcrErrorCode OcrFrameProcessor::ExtractFieldInfo(char ocredChars[25], __int64 currentUtcDayAsTicks, OcredFieldOsd& fieldInfo)
{
	bool success = true;
	long errorCode = OcrErrorCode::Unknown;


	fieldInfo.GpsFixType = ocredChars[0];
	if (ocredChars[1] >= '0' && ocredChars[1] <= '9')
		fieldInfo.TrackedSatellites = ocredChars[1] - 0x30;
	else
	{
		fieldInfo.TrackedSatellites = 0;
		errorCode += OcrErrorCode::TrackedSatellitesInvalid;
		success = false;
	}

	if (ocredChars[2] == 'W' || ocredChars[2] == ' ' || ocredChars[2] == 0) 
		fieldInfo.AlmanacUpdateState = ocredChars[2] != 'W';
	else
	{
		errorCode += OcrErrorCode::WaitFlagInvalid;
		success = false;
	}

	long hh = 0;
	if (ocredChars[3] >= '0' && ocredChars[4] >= '0' && ocredChars[3] <= '9' && ocredChars[4] <= '9')
		hh = 10 * (ocredChars[3] - 0x30) + (ocredChars[4] - 0x30);
	else
	{
		errorCode += OcrErrorCode::HoursInvalid;
		success = false;
	}

	long mm = 0;
	if (ocredChars[5] >= '0' && ocredChars[6] >= '0' && ocredChars[5] <= '9' && ocredChars[6] <= '9')
		mm = 10 * (ocredChars[5] - 0x30) + (ocredChars[6] - 0x30);
	else
	{
		errorCode += OcrErrorCode::MinutesInvalid;
		success = false;
	}

	long ss = 0;
	if (ocredChars[7] >= '0' && ocredChars[8] >= '0' && ocredChars[7] <= '9' && ocredChars[8] <= '9')
		ss = 10 * (ocredChars[7] - 0x30) + (ocredChars[8] - 0x30);
	else
		success = false;
        
	long ms1 = 0;
	long ms2 = 0;

	if (ocredChars[9] >= '0' && ocredChars[10] >= '0' && ocredChars[11] >= '0' && ocredChars[12] >= '0' && 
		ocredChars[9] <= '9' && ocredChars[10] <= '9' && ocredChars[11] <= '9' && ocredChars[12] <= '9')
		ms1 = 1000 * (ocredChars[9] - 0x30) + 100 * (ocredChars[10] - 0x30) + 10 * (ocredChars[11] - 0x30) + (ocredChars[12] - 0x30);
	else if (
		ocredChars[13] >= '0' && ocredChars[14] >= '0' && ocredChars[15] >= '0' && ocredChars[16] >= '0' && 
		ocredChars[13] <= '9' && ocredChars[14] <= '9' && ocredChars[15] <= '9' && ocredChars[16] <= '9')
		ms2 = 1000 * (ocredChars[13] - 0x30) + 100 * (ocredChars[14] - 0x30) + 10 * (ocredChars[15] - 0x30) + (ocredChars[16] - 0x30);
	else
	{
		errorCode += OcrErrorCode::MillisecondsInvalid;
		success = false;
	}

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

	char fieldNoStr[9];
	::ZeroMemory(fieldNoStr, 9);
	char* fieldNoPtr = &fieldNoStr[0];
	bool fieldNoBeginingFound = false;
	bool gapFound = false;
	for (int i = 17; i <= 24; i++)
	{
		char ch = ocredChars[i];
		if (ch != '\0' && ch != ' ')
		{
			if (gapFound)
			{
				// Error: a character had been found after a gap
				fieldNoBeginingFound = false;
				break;
			}

			if (ch < '0' || ch > '9')
			{
				// Error: field no contains a character that is not a digit
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
	{
		errorCode = OcrErrorCode::FieldNumberInvalid;
		success = false;
	}

	fieldInfo.FieldNumber = atoi(fieldNoStr);

	if (success)
		return OcrErrorCode::Success;
	else
		return (OcrErrorCode)errorCode;
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
	if (GetOcredAlmanacUpdateState() == 0 && (OddFieldOcredOsd.GpsFixType == 'P' || OddFieldOcredOsd.GpsFixType == 'G'))
		// Internal Time-Keeping
		return 1;
	else if (GetOcredAlmanacUpdateState() == 1)
	{
		switch(OddFieldOcredOsd.GpsFixType)
		{
			case 'G':
				return (unsigned char)2;

			case 'P':
				return (unsigned char)3;

			default:
				return (unsigned char)0;
		}
	}
	else
		return (unsigned char)0;
}

char OcrFrameProcessor::GetOcredGpsFixType()
{
	return OddFieldOcredOsd.GpsFixType;
}

OcrManager::OcrManager()
{
	Reset();
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

void OcrManager::VerifyOcredFrameIntegrity(OcrFrameProcessor* ocredFrame)
{
	if (ocredFrame->ErrorCodeEvenField != OcrErrorCode::Success || ocredFrame->ErrorCodeEvenField != OcrErrorCode::Success)
		// Cannot try and identify further problem if not all characters have been recognized correctly 
		return;

	// Field numbers must be consequtive
	if (ocredFrame->GetOcredStartFrameNumber() + 1 != ocredFrame->GetOcredEndFrameNumber())
	{
		ocredFrame->ErrorCodeEvenField = (OcrErrorCode)((long)ocredFrame->ErrorCodeEvenField + OcrErrorCode::FieldNumbersNotSequential);
		ocredFrame->ErrorCodeOddField = (OcrErrorCode)((long)ocredFrame->ErrorCodeOddField + OcrErrorCode::FieldNumbersNotSequential);
	}

	// Frame exposure duration must be PAL or NTSC
	long long fieldTimestampDifference = ocredFrame->GetOcredEndFrameTimeStamp() - ocredFrame->GetOcredStartFrameTimeStamp(0);
	bool isPAL = abs(fieldTimestampDifference - 20 * 10000) < 10000;
	bool isNTSC = abs(fieldTimestampDifference - (33.3667 / 2) * 10000) < 10000;
	if (!isPAL && !isNTSC)
	{
		ocredFrame->ErrorCodeEvenField = (OcrErrorCode)((long)ocredFrame->ErrorCodeEvenField + OcrErrorCode::FrameDurationNotPALorNTSC);
		ocredFrame->ErrorCodeOddField = (OcrErrorCode)((long)ocredFrame->ErrorCodeOddField + OcrErrorCode::FrameDurationNotPALorNTSC);		
	}
};

void OcrManager::VerifyOcredIntegratedIntervalIntegrity(OcrFrameProcessor* firstOcredFrame, OcrFrameProcessor* lastOcredFrame)
{
	VerifyOcredFrameIntegrity(firstOcredFrame);
	VerifyOcredFrameIntegrity(lastOcredFrame);

	if (firstOcredFrame->ErrorCodeEvenField != OcrErrorCode::Success || lastOcredFrame->ErrorCodeEvenField != OcrErrorCode::Success)
		// Cannot try and identify further problem if not all characters have been recognized correctly 
		return;

	// Frame integration duration must correspond to the integration period (NOTE: How to handle dropped frames ??)

	// Field numbers must correspond to the integration period (NOTE: How to handle dropped frames ??)
};

bool OcrManager::IsReceivingTimeStamps()
{
	return receivingTimestamps;
};

void OcrManager::ProcessedOcredFrame(OcrFrameProcessor* ocredFrame)
{
	if (receivingTimestamps)
	{
		VerifyOcredFrameIntegrity(ocredFrame);

		if (!ocredFrame->Success())
			OcrErrorsSinceReset++;
	}
};

void OcrManager::ProcessedOcredFramesInLockedMode(OcrFrameProcessor* firstOcredFrame, OcrFrameProcessor* lastOcredFrame)
{
	if (receivingTimestamps)
	{
		VerifyOcredIntegratedIntervalIntegrity(firstOcredFrame, lastOcredFrame);

		if (!firstOcredFrame->Success())
			OcrErrorsSinceReset++;

		if (!lastOcredFrame->Success())
			OcrErrorsSinceReset++;
	}
};

}

