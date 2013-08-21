#ifndef UTILS_H
#define UTILS_H

#include <stdio.h>
#include <string.h>
#include <time.h>

void WriteString(FILE* pFile, const char* str);

void DbgPrintBytes(unsigned char *bytes, int maxLen);

enum AavTagType
{
	UInt8 = 0,
	UInt16 = 1,
	UInt32 = 2,
	ULong64 = 3,
	Real = 4, // IEEE/REAL*4
	AnsiString255 = 5,
	List16OfAnsiString255 = 6,
};

enum GetByteMode
{
	Normal = 0,
	KeyFrameBytes = 1,
	DiffCorrBytes = 2
};

enum DiffCorrBaseFrame
{
	DiffCorrKeyFrame = 0,
	DiffCorrPrevFrame = 1
};

enum ImageBytesLayout
{
	FullImageRaw = 0,
	FullImageDiffCorrWithSigns = 1,
	FullImageDiffCorrNoSigns = 2
};

void crc32_init(void);
unsigned int compute_crc32(unsigned char *data, int len);

long long DateTimeToAavTicks(__int64 dayTicks, int hour, int minute, int sec, int tenthMs);
long long SystemTimeToAavTicks(SYSTEMTIME systemTime);
long long WindowsTicksToAavTicks(__int64 windowsTicks);

void DebugViewPrint(const wchar_t* formatText, ...);

#endif // UTILS_H