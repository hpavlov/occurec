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
	__int64 FrameNumber;
	unsigned char GpsTrackedSatellites;
	unsigned char GpsAlamancStatus;
	unsigned char GpsFixStatus;

	IntegratedFrame(long totalPixelsInFrame);
	~IntegratedFrame(void);
};

