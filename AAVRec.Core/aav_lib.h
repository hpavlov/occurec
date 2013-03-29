#ifndef ADV_LIB
#define ADV_LIB

#include "aav_file.h"

extern char* g_CurrentAavFile;
extern AavLib::AavFile* g_AavFile;
extern bool g_FileStarted;

		
char* AavGetCurrentFilePath(void);
void AavNewFile(const char* fileName);
void AavDefineImageSection(unsigned short width, unsigned short height);
void AavDefineImageLayout(unsigned char layoutId, const char* layoutType, const char* compression, unsigned char layoutBpp, int keyFrame, const char* diffCorrFromBaseFrame);
unsigned int AavDefineStatusSectionTag(const char* tagName, int tagType);
unsigned int AavAddFileTag(const char* tagName, const char* tagValue);
void AavAddOrUpdateImageSectionTag(const char* tagName, const char* tagValue);
void AavEndFile();
bool AavBeginFrame(long long timeStamp, unsigned int elapsedTime, unsigned int exposure);
void AavFrameAddImage(unsigned char layoutId, unsigned char* pixels);
void AavFrameAddStatusTag(unsigned int tagIndex, const char* tagValue);
void AddFrameStatusTagMessage(unsigned int tagIndex, const char* tagValue);
void AavFrameAddStatusTagUInt8(unsigned int tagIndex, unsigned char tagValue);
void AavFrameAddStatusTag16(unsigned int tagIndex, unsigned short tagValue);
void AddFrameStatusTagReal(unsigned int tagIndex, float tagValue);
void AavFrameAddStatusTag64(unsigned int tagIndex, long long tagValue);
void AavEndFrame();

#endif

