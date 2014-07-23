#include "stdafx.h"
#include "HtccMessage.h"
#include "stdlib.h"
#include "stdio.h"
#include <string.h>
#include "GlobalVars.h"

int HtccMessage::s_CorruptTimeStampMode = 1;
bool HtccMessage::s_CorruptNextEndTimeStampOnce = false;
bool HtccMessage::s_ToggleTimestampAlmanacOffset = false;

HtccMessage::HtccMessage(unsigned char* rawBytes)
{
	MessageType = HtccMsgInvalid;
	FrameIndex = -1;
	ErrorCode = 0;
	ErrorCounter = 0;
	TimestampUtcYear = -1;
	TimestampUtcMonth = -1;
	TimestampUtcDay = -1;
	TimestampUtcHours = -1;
	TimestampUtcMinutes = -1;
	TimestampUtcSecond = -1;
	TimestampUtcFractionalSecond10000 = -1;
	VersionMajor = -1;
    VersionMinor = -1;
	VersionBuild = -1;
	TrackedSatellites = 0;
	AlmanacOffset = 0;

	if (rawBytes == NULL || rawBytes[0] != 0xFE) 
		return;
	
	strncpy(&PacketType[0], &((char*)rawBytes)[1], 1);
	PacketType[1] = 0;
	
	if (HtccMessage::s_CorruptNextEndTimeStampOnce)
	{
		if (rawBytes[1] == 'e')
		{
			if (HtccMessage::s_CorruptTimeStampMode == 0)
			{
				// Add an hour
				rawBytes[5]++;
				HtccMessage::s_CorruptNextEndTimeStampOnce = false;
			}
			else if (HtccMessage::s_CorruptTimeStampMode == 1)
			{
				// Add a second
				rawBytes[7]++;
				HtccMessage::s_CorruptNextEndTimeStampOnce = false;
			}
			else if (HtccMessage::s_CorruptTimeStampMode == 2)
			{
				// Skip the end timestamp message (making it invalid type)
				rawBytes[1] = 'i';
				HtccMessage::s_CorruptNextEndTimeStampOnce = false;
			}
		}
		else if (rawBytes[1] == 's')
		{
			if (HtccMessage::s_CorruptTimeStampMode == 3)
			{
				// Skip the start timestamp message (making it invalid type)
				rawBytes[1] = 'i';
				HtccMessage::s_CorruptNextEndTimeStampOnce = false;
			}
		}

	}
	if (HtccMessage::s_ToggleTimestampAlmanacOffset)
	{
		if (rawBytes[1] == 's' || rawBytes[1] == 'e')
		{
			// Make the AlmanacUpdate be -10 sec
			rawBytes[14] = 0x4A;
		}
	}
	
#ifdef HTCC_DETAILED_LOG	
	sprintf(&RawBytes[0], "%02X%02X%02X%02X%02X%02X%02X%02X%02X%02X%02X%02X%02X%02X%02X\0",
			rawBytes[0],
			rawBytes[1],
			rawBytes[2],
			rawBytes[3],
			rawBytes[4],
			rawBytes[5],
			rawBytes[6],
			rawBytes[7],
			rawBytes[8],
			rawBytes[9],
			rawBytes[10],
			rawBytes[11],
			rawBytes[12],
			rawBytes[13],
			rawBytes[14]);	
#endif
	
	// Byte 1 = Message Type
	switch(rawBytes[1])
	{
		case 's': //(0x73)
			MessageType = HtccMsgStartExposureTimestamp;
			ParseStartExposureMessage(rawBytes);
			break;

		case 'e': //(0x65)
			MessageType = HtccMsgEndExposureTimestamp;
			ParseEndExposureMessage(rawBytes);
			break;
			
		case 'g': //(0x78)
			MessageType = HtccMsgGremlin;
			ParseGremlinMessage(rawBytes);
			break;
			
		case 'p': //(0x70)
			MessageType = HtccMsgGpsPositionLatitude;
			ParseLongLatMessage(rawBytes);
			break;
			
		case 'm': //(0x6D)
			MessageType = HtccMsgGpsPositionLongitude;
			ParseLongLatMessage(rawBytes);
			break;			

		case 'h': //(0x68)
			MessageType = HtccMsgGpsPositionHeight;
			ParseHeightWsg84Message(rawBytes);
			break;	

		case 'w': //(0x68)
			MessageType = HtccMsgGpsPositionWsg84;
			ParseHeightWsg84Message(rawBytes);
			break;	
			
		case 'd': //(0x100)
			MessageType = HtccMsgDate;
			break;
			
		case 'a': //(0x61)
			MessageType = HtccMsgAlmanachTimestamp;
			break;
			
		case 'v': //(0x76)
			MessageType = HtccMsgVersionInfo;
			ParseVersionInfo(rawBytes);
			break;			
			
		default:
			MessageType = HtccMsgInvalid;
			break;
	}
}

void HtccMessage::ParseStartExposureMessage(unsigned char* rawData)
{
	TimestampUtcYear = 2000 + rawData[2];
	TimestampUtcMonth = rawData[3];
	TimestampUtcDay = rawData[4];	
	TimestampUtcHours = rawData[5];
	TimestampUtcMinutes = rawData[6];
	TimestampUtcSecond = rawData[7];
	TimestampUtcFractionalSecond10000 = rawData[8] * 100 + rawData[9];
	
	long frameNo = rawData[10];
	frameNo <<= 7;
	frameNo += rawData[11];
	frameNo <<= 7;
	frameNo += rawData[12];
	
	FrameIndex = frameNo;
	
	TrackedSatellites = rawData[13] & 0xF; // bits 0-3
	AlmanacStatus = (unsigned char)((rawData[13] >> 4) & 0x1); // bits 4
	GPSFixStatus = (unsigned char)((rawData[13] >> 5) & 0x3); // bits 5-6	
	
	AlmanacOffset = rawData[14] & 0x3F; // bits 0-5 - the offset
	if ((int)(rawData[14] & 0x40) == (int)0x40) // bit 6 - sign
		AlmanacOffset = -AlmanacOffset;
}

void HtccMessage::ParseEndExposureMessage(unsigned char* rawData)
{
	TimestampUtcYear = 2000 + rawData[2];
	TimestampUtcMonth = rawData[3];
	TimestampUtcDay = rawData[4];	
	TimestampUtcHours = rawData[5];
	TimestampUtcMinutes = rawData[6];
	TimestampUtcSecond = rawData[7];
	TimestampUtcFractionalSecond10000 = rawData[8] * 100 + rawData[9];
	
	long frameNo = rawData[10];
	frameNo <<= 7;
	frameNo += rawData[11];
	frameNo <<= 7;
	frameNo += rawData[12];
	
	FrameIndex = frameNo;
	
	TrackedSatellites = rawData[13] & 0xF; // bits 0-3
	AlmanacStatus = (unsigned char)((rawData[13] >> 4) & 0x1); // bits 4
	GPSFixStatus = (unsigned char)((rawData[13] >> 5) & 0x3); // bits 5-6	
	
	AlmanacOffset = rawData[14] & 0x3F; // bits 0-5 - the offset
	if ((int)(rawData[14] & 0x40) == (int)0x40) // bit 6 - sign
		AlmanacOffset = -AlmanacOffset;	
}

void HtccMessage::ParseGremlinMessage(unsigned char* rawData)
{
	TimestampUtcYear = 2000 + rawData[2];
	TimestampUtcMonth = rawData[3];
	TimestampUtcDay = rawData[4];	
	TimestampUtcHours = rawData[5];
	TimestampUtcMinutes = rawData[6];
	TimestampUtcSecond = rawData[7];

	ErrorCode = rawData[8];
	AlmanacOffset = rawData[9] & 0x3F; // bits 0-5 - the offset
	if ((int)(rawData[9] & 0x40) == (int)0x40) // bit 6 - sign
		AlmanacOffset = -AlmanacOffset;		

	ErrorCounter = rawData[10];
}

void HtccMessage::ParseVersionInfo(unsigned char* rawData)
{
	VersionMajor = rawData[2];
	VersionMinor = rawData[3];
	VersionBuild = 100 * (rawData[4]) + 10 * (rawData[5]) + (rawData[6]);
	
	VersionCameraIsReady = (int)(rawData[7] & 0x01) == (int)0x01;
	VersionGpsIsReady = (int)(rawData[7] & 0x02) == (int)0x02;
	
	sprintf(&UnitSerialNumber[0], "%d-%d-%d-%d-%d\0", rawData[12], rawData[11], rawData[10], rawData[9], rawData[8]);
}

void HtccMessage::ParseLongLatMessage(unsigned char* rawData)
{
	bool deg100Present = rawData[7] == '.';
	bool deg10Present = rawData[6] == '.';
	
	bool invalidData = !deg100Present && !deg10Present;
	
	unsigned char deg100 = deg100Present ? (rawData[2] - 0x30) : 0;
	unsigned char deg10 = deg100Present ? (rawData[3] - 0x30) : (rawData[2] - 0x30);
	unsigned char deg1 = deg100Present ? (rawData[4] - 0x30) : (rawData[3] - 0x30);
	unsigned char min10 = deg100Present ? (rawData[5] - 0x30) : (rawData[4] - 0x30);
	unsigned char min1 = deg100Present ? (rawData[6] - 0x30) : (rawData[5] - 0x30);
	unsigned char minf10 = deg100Present ? (rawData[8] - 0x30) : (rawData[7] - 0x30);
	unsigned char minf100 = deg100Present ? (rawData[9] - 0x30) : (rawData[9] - 0x30);
	unsigned char minf1000 = deg100Present ? (rawData[10] - 0x30) : (rawData[9] - 0x30);
	unsigned char minf10000 = deg100Present ? (rawData[11] - 0x30) : (rawData[10] - 0x30);
	
	strncpy(&PositionDirection[0], &((char*)rawData)[12], 1);
	PositionDirection[1] = 0;

	PositionHDOP = 0;
	PositionValue = 
		deg100 * 100 + deg10 * 10 + deg1 +
		(min10 * 10 + min1 + minf10 / 10.0 + minf100 / 100.0 + minf1000 / 1000.0 + minf1000 / 10000.0) / 60.0;
		
	strncpy(&PositionStr[0], &((char*)rawData)[2], 11);
	PositionStr[11] = 0;
	
	PositionHDOPStr[0] = 0;
		
	if (invalidData)
	{
		printf("HTCC: Cannot parse position data: %s\n", &PositionStr[0]);		 
		return;		
	}
		
	printf("%s: %f%s (%s;%s)\n", MessageType == HtccMsgGpsPositionLongitude ? "Longitude" : "Latitude", PositionValue, &PositionDirection[0], &PositionStr[0], &PositionHDOPStr[0]);
	
	if (MessageType == HtccMsgGpsPositionLatitude)
	{
		SYS_GPS_LATITUDE_FLOAT = PositionValue;
		SYS_GPS_LATITUDE_DIRECTION[0] = PositionDirection[0];
		SYS_GPS_LATITUDE_DIRECTION[1] = 0;
		
		int valueSign = PositionDirection[0] == 'S' ? -1 : 1;
		
		snprintf(&SYS_GPS_LATITUDE[0], 16, "%3d*%2d\'%2.1f\"", 
			(int)(valueSign * SYS_GPS_LATITUDE_FLOAT),
			(int)((SYS_GPS_LATITUDE_FLOAT - (int)SYS_GPS_LATITUDE_FLOAT) * 60),
			((SYS_GPS_LATITUDE_FLOAT - (int)SYS_GPS_LATITUDE_FLOAT) * 60 - (int)((SYS_GPS_LATITUDE_FLOAT - (int)SYS_GPS_LATITUDE_FLOAT) * 60)) * 60);
	}
	else if (MessageType == HtccMsgGpsPositionLongitude)
	{
		SYS_GPS_LONGITUDE_FLOAT = PositionValue;
		SYS_GPS_LONGITUDE_DIRECTION[0] = PositionDirection[0];
		SYS_GPS_LONGITUDE_DIRECTION[1] = 0;
		
		int valueSign = PositionDirection[0] == 'W' ? -1 : 1;
		
		snprintf(&SYS_GPS_LONGITUDE[0], 16, "%3d*%2d\'%2.1f\"", 
			(int)(valueSign * SYS_GPS_LONGITUDE_FLOAT),
			(int)((SYS_GPS_LONGITUDE_FLOAT - (int)SYS_GPS_LONGITUDE_FLOAT) * 60),
			((SYS_GPS_LONGITUDE_FLOAT - (int)SYS_GPS_LONGITUDE_FLOAT) * 60 - (int)((SYS_GPS_LONGITUDE_FLOAT - (int)SYS_GPS_LONGITUDE_FLOAT) * 60)) * 60);		
	}	
}

void HtccMessage::ParseHeightWsg84Message(unsigned char* rawData)
{
	bool met1000Format = rawData[6] == '.';
	bool met100Format = rawData[5] == '.';
	bool met10Format = rawData[4] == '.';
	
	// NOTE: These are not ready yet as it is uncertain how to parse values smaller than 1 meter and larger than 100 hdop
	unsigned char met1000 = 0;
	unsigned char met100 = 0;
	unsigned char met10 = 0;
	unsigned char met1 = 0;
	unsigned char metf10 = 0;
	bool invalidData = false;
	
	if (met1000Format)
	{
		met1000 = (rawData[2] - 0x30);
		met100 = (rawData[3] - 0x30);
		met10 = (rawData[4] - 0x30);
		met1 = (rawData[5] - 0x30);
		metf10 = (rawData[7] - 0x30);
	}
	else if (met100Format)
	{
		met1000 = 0;
		met100 = (rawData[2] - 0x30);
		met10 = (rawData[3] - 0x30);
		met1 = (rawData[4] - 0x30);
		metf10 = (rawData[6] - 0x30);
	}
	else if (met10Format)
	{
		met1000 = 0;
		met100 = 0;
		met10 = (rawData[2] - 0x30);
		met1 = (rawData[3] - 0x30);
		metf10 = (rawData[5] - 0x30);
	}
	else
	{
		invalidData = true;
	}

	bool hdop10Format = rawData[11] == '.';
	bool hdop1Format = rawData[10] == '.';
	
	unsigned char hdop10 = 0;
	unsigned char hdop1 = 0;
	unsigned char hdopf10 = 0;
	
	if (hdop10Format)
	{
		hdop10 = rawData[9] - 0x30;
		hdop1 = rawData[10] - 0x30;
		hdopf10 = rawData[12] - 0x30;
	}
	else if (hdop1Format)
	{
		hdop10 = 0;
		hdop1 = rawData[9] - 0x30;
		hdopf10 = rawData[11] - 0x30;
	}
	else
	{
		invalidData = true;
	}
	
	PositionDirection[0] = 0;	
	PositionHDOP = hdop10 * 10 + hdop1 + hdopf10 / 10.0;
	PositionValue = 
		met1000 * 1000 + met100 * 100 + met10 * 10 + met1 +
		metf10 / 10.0;
		
	strncpy(&PositionStr[0], &((char*)rawData)[2], 6);
	PositionStr[6] = 0;
	
	strncpy(&PositionHDOPStr[0], &((char*)rawData)[9], 4);
	PositionHDOPStr[4] = 0;
	
	if (invalidData)
	{
		printf("HTCC: Cannot parse position data: (%s;%s)\n", &PositionStr[0], &PositionHDOPStr[0]);		 
		return;		
	}
		
	printf("%s: %f HDOP:%f (%s;%s)\n", MessageType == HtccMsgGpsPositionHeight ? "Height" : "WSG84", PositionValue, PositionHDOP, &PositionStr[0], &PositionHDOPStr[0]);	
	
	if (MessageType == HtccMsgGpsPositionHeight)
	{
		SYS_GPS_ALTITUDE_FLOAT = PositionValue;
		sprintf(&SYS_GPS_ALTITUDE[0], "%0.1fM", SYS_GPS_ALTITUDE_FLOAT);
	}
	else if (MessageType == HtccMsgGpsPositionWsg84)
	{
		SYS_GPS_WSG84_FLOAT = PositionValue;
		
		sprintf(&SYS_GPS_WSG84[0], "%0.1fM", SYS_GPS_WSG84_FLOAT);		
		sprintf(&SYS_GPS_HDOP[0], "%0.1f", PositionHDOP);
	}
}
