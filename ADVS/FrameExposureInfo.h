/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

#ifndef FRAME_EXPOSURE_INFO
#define FRAME_EXPOSURE_INFO

#include "HtccMessage.h"
#include "stdlib.h"
#include <stdio.h>
#include <iostream>
#include "AdvrUtils.h"

class FrameExposureTimeStamp
{

public:
	FrameExposureTimeStamp(HtccMessage* msg)
	{
		UtcYears = msg->TimestampUtcYear;
		UtcMonths = msg->TimestampUtcMonth;
		UtcDays = msg->TimestampUtcDay;
		UtcHours = msg->TimestampUtcHours;
		UtcMinutes = msg->TimestampUtcMinutes;
		UtcSecond = msg->TimestampUtcSecond;
		UtcFractionalSecond10000 = msg->TimestampUtcFractionalSecond10000;	
		
		TrackedSatellites = msg->TrackedSatellites;
		AlmanacStatus = msg->AlmanacStatus;
		GPSFixStatus = msg->GPSFixStatus;
		AlmanacOffset = msg->AlmanacOffset;
		
		if (UtcFractionalSecond10000 > 10000)
		{
			// Sometimes the endtimestamp 1000fractionalSeconds can be higher than 10000. Then we need to make an adjustment
			UtcFractionalSecond10000 -= 10000;
			UtcSecond++;
			
			if (UtcSecond == 60)
			{
				UtcSecond = 0;
				UtcMinutes++;
			}
			
			if (UtcMinutes == 60)
			{
				UtcMinutes = 0;
				UtcHours++;
			}
			
			if (UtcHours == 24)
			{
				UtcHours = 0;
				UtcDays++;
				
				// NOTE: It is possible that this will break the sync and crash ADVR !!!
			}						
		}
		
#ifdef HTCC_DETAILED_LOG		
		strcpy(&RawMessageBytes[0], msg->RawBytes);
#endif
		
	};
	
	
	int UtcYears;
	int UtcMonths;
	int UtcDays;
	
	int UtcHours;
	int UtcMinutes;
	int UtcSecond;
	int UtcFractionalSecond10000;
	
	unsigned char TrackedSatellites;
	unsigned char AlmanacStatus; // 0=uncertain, 1=good
	unsigned char GPSFixStatus; // 00 = no fix, 01 = no fix (confident), 10 = G fix, 11 = P fix	
	//char AlmanacOffset;
	int AlmanacOffset;
		
#ifdef HTCC_DETAILED_LOG		
	char RawMessageBytes[HTCC_DEBUG_MESSAGE_SIZE];
#endif
	
};

class FrameExposureInfo
{
public:
	FrameExposureInfo(HtccMessage* startMsg, HtccMessage* endMsg)
	{
		FrameNo = startMsg->FrameIndex;
		
		// TODO: How do we ensure the date change messages have the correct date assigned??
		//       Send the month day with the message which will be matched against the separately received Date messages?
		
		StartExposure = new FrameExposureTimeStamp(startMsg);
		EndExposure = new FrameExposureTimeStamp(endMsg);
		
		destructorCalled = false;
	};
	
	int FrameNo;
	bool destructorCalled;
	
	FrameExposureTimeStamp* StartExposure;
	FrameExposureTimeStamp* EndExposure;
	
	~FrameExposureInfo()
	{
		destructorCalled = true;
		
		if (StartExposure)
		{
			delete StartExposure;
			StartExposure = NULL;
		}
		
		if (EndExposure)
		{
			delete EndExposure;
			EndExposure = NULL;
		}
	};
	
	void CheckAndAssertExposuresPresent(bool checkStartExposure)
	{
		// NOTE: We crash here simply because of a buffer overrun !!! It is nothing to do with NULL StartExposure/EndExposure
		if (checkStartExposure && StartExposure == NULL)
			std::cerr << "FrameExposureInfo - StartExposure is NULL. ABOUT TO CRASH. " << std::endl << std::endl; 
		
		if (EndExposure == NULL)
			std::cerr << "FrameExposureInfo - EndExposure is NULL. ABOUT TO CRASH. " << std::endl << std::endl; 
		
		#ifdef HTCC_DETAILED_LOG
		if (EndExposure == NULL && StartExposure != NULL)	
			std::cerr << "StartExposure Bytes: " << StartExposure->RawMessageBytes << std::endl << std::endl; 
			
		if (StartExposure == NULL && EndExposure != NULL)	
			std::cerr << "EndExposure Bytes: " << EndExposure->RawMessageBytes << std::endl << std::endl; 
		#endif
	}
	
	unsigned char GetTrackedGPSSatellites()
	{
		CheckAndAssertExposuresPresent(false);
		
		return StartExposure->TrackedSatellites;
	}
	
	unsigned char GetGPSSatellitesAlmanacStatus()
	{
		CheckAndAssertExposuresPresent(false);
		
		return StartExposure->AlmanacStatus;
	}
	
	char GetGPSSatellitesAlmanacOffset()
	{
		CheckAndAssertExposuresPresent(false);
		
		return StartExposure->AlmanacOffset;
	}	
	
	unsigned char GetGPSSatellitesFixStatus()
	{
		CheckAndAssertExposuresPresent(false);
		
		return StartExposure->GPSFixStatus;
	}
	
	void GetAdvTimeStamp(long long* timeStamp, unsigned int* exposureIn10thMilliseconds, bool applyAlmanacOffset)
	{
		CheckAndAssertExposuresPresent(true);
	
		// Timestamp is the time ellapsed in milliseconds since 1 Jan 2010 00:00:00.0000 UT	
		long long start10thMs = DateTimeToAdvTicks(
			StartExposure->UtcYears,
			StartExposure->UtcMonths, 
			StartExposure->UtcDays, 
			StartExposure->UtcHours, 
			StartExposure->UtcMinutes, 
			StartExposure->UtcSecond, 
			StartExposure->UtcFractionalSecond10000);
			
		if (applyAlmanacOffset)
			start10thMs += 10000 * StartExposure->AlmanacOffset;
			
		long long end10thMs = DateTimeToAdvTicks(
			EndExposure->UtcYears, 
			EndExposure->UtcMonths, 
			EndExposure->UtcDays, 
			EndExposure->UtcHours, 
			EndExposure->UtcMinutes, 
			EndExposure->UtcSecond, 
			EndExposure->UtcFractionalSecond10000);
		
		if (applyAlmanacOffset)
			// To ensure the integrity of the exposure durations only the start exposure almanac offset is used
			// for both the start and the end timestamp
			end10thMs += 10000 * StartExposure->AlmanacOffset;
		
		*exposureIn10thMilliseconds = (unsigned int)(end10thMs - start10thMs);
		*timeStamp = (start10thMs + end10thMs) / 20;
		
		if (((start10thMs + end10thMs) % 20) >= 10)
			*timeStamp++;
	};
	
	void GetAdvEndExposureTimeStamp(long long* endExposureTimeStamp, unsigned int* exposureIn10thMilliseconds, bool applyAlmanacOffset)
	{
		CheckAndAssertExposuresPresent(true);
		
		// Timestamp is the time ellapsed in milliseconds since 1 Jan 2010 00:00:00.0000 UT
		long long start10thMs = DateTimeToAdvTicks(
			StartExposure->UtcYears, 
			StartExposure->UtcMonths, 
			StartExposure->UtcDays, 
			StartExposure->UtcHours, 
			StartExposure->UtcMinutes, 
			StartExposure->UtcSecond,  
			StartExposure->UtcFractionalSecond10000);
			
		if (applyAlmanacOffset)
			start10thMs += 10000 * StartExposure->AlmanacOffset;
			
		long long end10thMs = DateTimeToAdvTicks(
			EndExposure->UtcYears, 
			EndExposure->UtcMonths, 
			EndExposure->UtcDays, 
			EndExposure->UtcHours, 
			EndExposure->UtcMinutes, 
			EndExposure->UtcSecond, 
			EndExposure->UtcFractionalSecond10000);
		
		if (applyAlmanacOffset)
			// To ensure the integrity of the exposure durations only the start exposure almanac offset is used
			// for both the start and the end timestamp			
			end10thMs += 10000 * StartExposure->AlmanacOffset;
		
		*exposureIn10thMilliseconds = (unsigned int)(end10thMs - start10thMs);
		*endExposureTimeStamp = end10thMs / 10;
		
		if  ((end10thMs % 10) >= 10)
			*endExposureTimeStamp++;
	};

#ifdef HTCC_DETAILED_LOG
	char* DebugString()
	{
		char* output = (char*)malloc(2 * HTCC_DEBUG_MESSAGE_SIZE + 2);
		sprintf(&output[0], "%s %s\0", StartExposure->RawMessageBytes, EndExposure->RawMessageBytes);
		return output;
	}		
#endif

};

#endif