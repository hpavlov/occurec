#pragma once
class RawFrame
{
public:
	unsigned char* BmpBits;
	long BmpBitsSize;
	__int64 CurrentUtcDayAsTicks;

	RawFrame(int imageWidth, int imageHeight);
	~RawFrame(void);
};

