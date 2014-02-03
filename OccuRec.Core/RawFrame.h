#pragma once
class RawFrame
{
public:
	unsigned char* BmpBits;
	long BmpBitsSize;
	__int64 CurrentUtcDayAsTicks;
	__int64 CurrentNtpTimeAsTicks;
	double NtpBasedTimeError;

	RawFrame(int imageWidth, int imageHeight);
	~RawFrame(void);
};

