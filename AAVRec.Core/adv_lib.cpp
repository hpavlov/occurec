#include "StdAfx.h"
#include <stdio.h>
#include <iostream>
#include <vector>
#include <stdlib.h>
#include <string.h>

#include "adv_lib.h"
#include "adv_image_layout.h"
#include "adv_profiling.h"


char* g_CurrentAdvFile;
AdvLib::AdvFile* g_AdvFile;
bool g_FileStarted = false;

using namespace std;

char* AdvGetCurrentFilePath(void)
{
	return g_CurrentAdvFile;
}

void AdvNewFile(const char* fileName)
{
    if (NULL != g_AdvFile)
	{
		delete g_AdvFile;
		g_AdvFile = NULL;		
	}
	
	if (NULL != g_CurrentAdvFile)
	{
		delete g_CurrentAdvFile;
		g_CurrentAdvFile = NULL;
	}
	
	g_FileStarted = false;
	
	int len = strlen(fileName);	
	if (len > 0)
	{
		g_CurrentAdvFile = new char[len + 1];
		strncpy(g_CurrentAdvFile, fileName, len + 1);
	
		g_AdvFile = new AdvLib::AdvFile();	
	}
}

void AdvEndFile()
{
	if (NULL != g_AdvFile)
	{
		g_AdvFile->EndFile();
		
		delete g_AdvFile;
		g_AdvFile = NULL;		
	}
	
	if (NULL != g_CurrentAdvFile)
	{
		delete g_CurrentAdvFile;
		g_CurrentAdvFile = NULL;
	}
	
	g_FileStarted = false;
}

void AdvDefineImageSection(unsigned short width, unsigned short height, unsigned char dataBpp)
{
	AdvLib::AdvImageSection* imageSection = new AdvLib::AdvImageSection(width, height, dataBpp);
	g_AdvFile->AddImageSection(imageSection);
}

void AdvDefineImageLayout(unsigned char layoutId, const char* layoutType, const char* compression, unsigned char bpp, int keyFrame, const char* diffCorrFromBaseFrame)
{	
	AdvLib::AdvImageLayout* imageLayout = g_AdvFile->ImageSection->AddImageLayout(layoutId, layoutType, compression, bpp, keyFrame);
	if (diffCorrFromBaseFrame != NULL)
		imageLayout->AddOrUpdateTag("DIFFCODE-BASE-FRAME", diffCorrFromBaseFrame);
}

unsigned int AdvDefineStatusSectionTag(const char* tagName, int tagType)
{
	unsigned int statusTagId = g_AdvFile->StatusSection->DefineTag(tagName, (AdvTagType)tagType);
	return statusTagId;
}

unsigned int AdvAddFileTag(const char* tagName, const char* tagValue)
{
	unsigned int fileTagId = g_AdvFile->AddFileTag(tagName, tagValue);
	return fileTagId;
}

void AdvAddOrUpdateImageSectionTag(const char* tagName, const char* tagValue)
{
	return g_AdvFile->ImageSection->AddOrUpdateTag(tagName, tagValue);
}

bool AdvBeginFrame(long long timeStamp, unsigned int elapsedTime, unsigned int exposure)
{
	if (!g_FileStarted)
	{
		bool success = g_AdvFile->BeginFile(g_CurrentAdvFile);
		if (success)
		{
			g_FileStarted = true;	
		}
		else
		{
			g_FileStarted = false;
			return false;
		}		
	}
	
	g_AdvFile->BeginFrame(timeStamp, elapsedTime, exposure);
	return true;
}

void AdvFrameAddImage(unsigned char layoutId,  unsigned short* pixels, unsigned char pixelsBpp)
{
	g_AdvFile->AddFrameImage(layoutId, pixels, pixelsBpp);
}

void AdvFrameAddStatusTag(unsigned int tagIndex, const char* tagValue)
{
	g_AdvFile->AddFrameStatusTag(tagIndex, tagValue);
}

void AddFrameStatusTagMessage(unsigned int tagIndex, const char* tagValue)
{	
	g_AdvFile->AddFrameStatusTagMessage(tagIndex, tagValue);
}

void AdvFrameAddStatusTagUInt8(unsigned int tagIndex, unsigned char tagValue)
{
	g_AdvFile->AddFrameStatusTagUInt8(tagIndex, tagValue);
}

void AdvFrameAddStatusTag64(unsigned int tagIndex, long long tagValue)
{
	g_AdvFile->AddFrameStatusTagUInt64(tagIndex, tagValue);
}

void AdvFrameAddStatusTag16(unsigned int tagIndex, unsigned short tagValue)
{
	g_AdvFile->AddFrameStatusTagUInt16(tagIndex, tagValue);	
}

void AddFrameStatusTagReal(unsigned int tagIndex, float tagValue)
{
	g_AdvFile->AddFrameStatusTagReal(tagIndex, tagValue);
}

void AdvEndFrame()
{
	g_AdvFile->EndFrame();
}