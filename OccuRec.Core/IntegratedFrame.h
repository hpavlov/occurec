/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

#pragma once
class IntegratedFrame
{
private:
	long m_TotalPixelsInFrame;

public:
	unsigned char* Pixels;
	unsigned short* Pixels16;
	long NumberOfIntegratedFrames;
	__int64 StartFrameId;
	__int64 EndFrameId;
	__int64 StartTimeStamp;
	__int64 EndTimeStamp;
	char StartTimeStampStr[128];
	char EndTimeStampStr[128];
	__int64 FrameNumber;
	unsigned char GpsTrackedSatellites;
	unsigned char GpsAlamancStatus;
	unsigned char GpsFixStatus;
	__int64 NTPStartTimestamp;
	__int64 NTPEndTimestamp;
	long NTPTimestampError;
	__int64 SecondaryStartTimestamp;
	__int64 SecondaryEndTimestamp;
	char OcrErrorMessageStr[255];

	IntegratedFrame(long totalPixelsInFrame, bool is16Bit);
	~IntegratedFrame(void);
};

