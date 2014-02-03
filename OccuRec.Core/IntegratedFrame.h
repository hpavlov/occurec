#pragma once
class IntegratedFrame
{
private:
	long m_TotalPixelsInFrame;

public:
	unsigned char* Pixels;
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
	char OcrErrorMessageStr[255];

	IntegratedFrame(long totalPixelsInFrame);
	~IntegratedFrame(void);
};

