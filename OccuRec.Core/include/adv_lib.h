/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

#ifndef ADV_LIB2
#define ADV_LIB2

#ifdef __cplusplus
extern "C"
{
#endif 
#define ADVRESULT HRESULT

namespace AdvVer2
{
extern ADVRESULT AdvVer2_NewFile(const char* fileName, bool overwriteExisting);
extern ADVRESULT AdvVer2_SetTicksTimingPrecision(int mainStreamAccuracy, int calibrationStreamAccuracy);
extern ADVRESULT AdvVer2_DefineExternalClockForMainStream(__int64 clockFrequency, int ticksTimingAccuracy);
extern ADVRESULT AdvVer2_DefineExternalClockForCalibrationStream(__int64 clockFrequency, int ticksTimingAccuracy);
extern ADVRESULT AdvVer2_AddMainStreamTag(const char* tagName, const char* tagValue);
extern ADVRESULT AdvVer2_AddCalibrationStreamTag(const char* tagName, const char* tagValue);
extern ADVRESULT AdvVer2_DefineImageSection(unsigned short width, unsigned short height, unsigned char dataBpp);
extern ADVRESULT AdvVer2_DefineImageLayout(unsigned char layoutId, const char* layoutType, const char* compression, unsigned char layoutBpp);
extern ADVRESULT AdvVer2_DefineStatusSection(__int64 utcTimestampAccuracyInNanoseconds);
extern ADVRESULT AdvVer2_DefineStatusSectionTag(const char* tagName, int tagType, unsigned int* addedTagId);
extern ADVRESULT AdvVer2_AddFileTag(const char* tagName, const char* tagValue);
extern ADVRESULT AdvVer2_AddUserTag(const char* tagName, const char* tagValue);
extern ADVRESULT AdvVer2_AddOrUpdateImageSectionTag(const char* tagName, const char* tagValue);
extern ADVRESULT AdvVer2_EndFile();
extern ADVRESULT AdvVer2_BeginFrameWithTicks(unsigned int streamId, __int64 startFrameTicks, __int64 endFrameTicks, __int64 elapsedTicksSinceFirstFrame, __int64 utcStartTimeNanosecondsSinceAdvZeroEpoch, unsigned int utcExposureNanoseconds);
extern ADVRESULT AdvVer2_BeginFrame(unsigned int streamId, __int64 utcStartTimeNanosecondsSinceAdvZeroEpoch, unsigned int utcExposureNanoseconds);
extern ADVRESULT AdvVer2_FrameAddImage(unsigned char layoutId, unsigned short* pixels, unsigned char pixelsBpp);
extern ADVRESULT AdvVer2_FrameAddImageBytes(unsigned char layoutId, unsigned char* pixels, unsigned char pixelsBpp);
extern ADVRESULT AdvVer2_FrameAddStatusTagUTF8String(unsigned int tagIndex, const char* tagValue);
extern ADVRESULT AdvVer2_FrameAddStatusTagUInt8(unsigned int tagIndex, unsigned char tagValue);
extern ADVRESULT AdvVer2_FrameAddStatusTag16(unsigned int tagIndex, unsigned short tagValue);
extern ADVRESULT AdvVer2_FrameAddStatusTagReal(unsigned int tagIndex, float tagValue);
extern ADVRESULT AdvVer2_FrameAddStatusTag32(unsigned int tagIndex, unsigned int tagValue);
extern ADVRESULT AdvVer2_FrameAddStatusTag64(unsigned int tagIndex, __int64 tagValue);
extern ADVRESULT AdvVer2_EndFrame();

extern int AdvVer2_GetLastSystemSpecificFileError();

extern void GetLibraryVersion(char* version);
extern void GetLibraryPlatformId( char* platform);
extern int GetLibraryBitness();
}



#ifdef __cplusplus
}
#endif
 
#endif

