/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

#ifndef HTCC_MESSAGE
#define HTCC_MESSAGE

#include "GlobalDefines.h"

#define HTCC_PACKET_SIZE 15
#define HTCC_DEBUG_MESSAGE_SIZE 31
#define HTCC_START_PACKET_BYTE 0xFE

enum HtccMessageType
{
	HtccMsgInvalid,
	HtccMsgStartExposureTimestamp,
	HtccMsgEndExposureTimestamp,
	HtccMsgAlmanachTimestamp,
	HtccMsgVersionInfo,
	HtccMsgGpsPositionLongitude,
	HtccMsgGpsPositionLatitude,
	HtccMsgGpsPositionHeight,
	HtccMsgGpsPositionWsg84,
	HtccMsgGpsAltitude,
	HtccMsgDate,
	HtccMsgGremlin
};

enum HtccGoodness
{
	HtccGoodnessInvalid,
	HtccGoodnessGood,
	HtccGoodnessFailed
};

// According to HTCC Spec v3.7
enum HtccErrorCode
{
	// 0 - Occurs within 15 minutes of P fix - 1PPS was detected, 
	// and ZDA information described it, but the description was not what was expected.
	HtccErrorAlmanacUpdate,

	// 1 - Occurs if the almanac is considered good - 1PPS was detected, 
	// and ZDA information described it, but the description was not what was expected.
	HtccErrorUTCDidNotChangeCorrectly,
	
	// 2 - The slowPPS error occurs if the subsecond timer thinks a 1PPS sequence takes more than 1.000 099 seconds.
	HtccErrorSlowPPS,

	// 3 - The NMEA sentence did not contain the characters "ZDA" or "GSA" where they should have been.
	HtccErrorNoNMEATagMsg,
	
	HtccError4Reserved,
	
	// 5 - The memory check routine detected memory contents were not what they should have been.
	HtccErrorFailedBoardTest,
	
	// 6 - A 1PPS was detected within 0.999 900 sec of a previous 1PPS.
	HtccErrorPPSTooQuick,
	
	HtccError7Reserved,
	
	// 8 - An event occurred but was not handled before another event of the same kind occurred again.
	HtccErrorUnhandledEvent,
	
	// 9 - A start or end event was detected without its opposite being detected before.
	HtccErrorExposurePacketOutOfOrder,
	
	// 10 - A particular unhandled event
	HtccErrorPPSNotServiced,
	
	// 11 - A particular unhandled event
	HtccErrorStartTriggerNotServiced,
	
	// 12 - A particular unhandled event
	HtccErrorEndTriggerNotServiced,
	
	// 13 - A particular unhandled event
	HtccErrorExtraTriggerNotServiced,
	
	// 14 - Camera unplugged from the Hirose connector
	HtccErrorCameraDisconnected,
	
	HtccError15Reserved,
	
	// 16 - ZDA NMEA checksum does not match computed value
	HtccErrorSerialError,
	
	// 17 - The absentPPS error occurs if the subsecond timer has not detected a 1PPS in 1.001 seconds.
	HtccErrorAbsentPPS
};

class HtccMessage
{
private:
	void ParseVersionInfo(unsigned char* rawData);
	void ParseStartExposureMessage(unsigned char* rawData);
	void ParseEndExposureMessage(unsigned char* rawData);
	void ParseGremlinMessage(unsigned char* rawData);
	void ParseLongLatMessage(unsigned char* rawData);
	void ParseHeightWsg84Message(unsigned char* rawData);
	
public:
	HtccMessage() { };
	HtccMessage(unsigned char* rawBytes);
	
	HtccMessageType MessageType;
	HtccGoodness GoodnessIndicator;
	
	char PacketType[2];
		
#ifdef HTCC_DETAILED_LOG		
	char RawBytes[HTCC_DEBUG_MESSAGE_SIZE];
#endif

	long FrameIndex;
	
	int TimestampUtcYear;
	int TimestampUtcMonth;
	int TimestampUtcDay;	
	int TimestampUtcHours;
	int TimestampUtcMinutes;
	int TimestampUtcSecond;
	int TimestampUtcFractionalSecond10000;
	
	int ErrorCode;
	int ErrorCounter;
	
	bool ChecksumError;
	
	int VersionMajor;
	int VersionMinor;
	int VersionBuild;
	bool VersionCameraIsReady;
	bool VersionGpsIsReady;
	
	unsigned char TrackedSatellites;
	unsigned char AlmanacStatus; // 0=uncertain, 1=good
	unsigned char GPSFixStatus; // 00 = no fix, 01 = G fix, 10 = P fix
	char AlmanacOffset;
	
	double PositionValue;
	char PositionDirection[2];
	float PositionHDOP;
	char PositionStr[13];
	char PositionHDOPStr[5];
	
	char UnitSerialNumber[21];
	
public:
	static bool s_CorruptNextEndTimeStampOnce;
	static int s_CorruptTimeStampMode;
	static bool s_ToggleTimestampAlmanacOffset;
};


#endif