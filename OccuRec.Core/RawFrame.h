#pragma once
class RawFrame
{
public:
	unsigned char* BmpBits;
	long BmpBitsSize;
	__int64 CurrentUtcDayAsTicks;
	__int64 CurrentNtpTimeAsTicks;
	double NtpBasedTimeError;
	__int64 CurrentSecondaryTimeAsTicks;

	RawFrame(int imageWidth, int imageHeight);
	~RawFrame(void);
};

