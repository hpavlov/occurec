#include "StdAfx.h"
#include <stdio.h>
#include <iostream>
#include <vector>
#include <stdlib.h>
#include <string.h>

#include "aav_lib.h"
#include "aav_image_layout.h"
#include "aav_profiling.h"


char* g_CurrentAavFile;
AavLib::AavFile* g_AavFile;
bool g_FileStarted = false;

using namespace std;

char* AavGetCurrentFilePath(void)
{
	return g_CurrentAavFile;
}

void AavNewFile(const char* fileName)
{
    if (NULL != g_AavFile)
	{
		delete g_AavFile;
		g_AavFile = NULL;		
	}
	
	if (NULL != g_CurrentAavFile)
	{
		delete g_CurrentAavFile;
		g_CurrentAavFile = NULL;
	}
	
	g_FileStarted = false;
	
	int len = strlen(fileName);	
	if (len > 0)
	{
		g_CurrentAavFile = new char[len + 1];
		strncpy(g_CurrentAavFile, fileName, len + 1);
	
		g_AavFile = new AavLib::AavFile();	
	}
}

void AavEndFile()
{
	if (NULL != g_AavFile)
	{
		g_AavFile->EndFile();
		
		delete g_AavFile;
		g_AavFile = NULL;		
	}
	
	if (NULL != g_CurrentAavFile)
	{
		delete g_CurrentAavFile;
		g_CurrentAavFile = NULL;
	}
	
	g_FileStarted = false;
}

void AavDefineImageSection(unsigned short width, unsigned short height)
{
	AavLib::AavImageSection* imageSection = new AavLib::AavImageSection(width, height, 8);
	g_AavFile->AddImageSection(imageSection);
}

void AavDefineImageLayout(unsigned char layoutId, const char* layoutType, const char* compression, unsigned char bpp, int keyFrame, const char* diffCorrFromBaseFrame)
{	
	AavLib::AavImageLayout* imageLayout = g_AavFile->ImageSection->AddImageLayout(layoutId, layoutType, compression, bpp, keyFrame);
	if (diffCorrFromBaseFrame != NULL)
		imageLayout->AddOrUpdateTag("DIFFCODE-BASE-FRAME", diffCorrFromBaseFrame);
}

unsigned int AavDefineStatusSectionTag(const char* tagName, int tagType)
{
	unsigned int statusTagId = g_AavFile->StatusSection->DefineTag(tagName, (AavTagType)tagType);
	return statusTagId;
}

unsigned int AavAddFileTag(const char* tagName, const char* tagValue)
{
	unsigned int fileTagId = g_AavFile->AddFileTag(tagName, tagValue);
	return fileTagId;
}

void AavAddOrUpdateImageSectionTag(const char* tagName, const char* tagValue)
{
	return g_AavFile->ImageSection->AddOrUpdateTag(tagName, tagValue);
}

bool AavBeginFrame(long long timeStamp, unsigned int elapsedTime, unsigned int exposure)
{
	if (!g_FileStarted)
	{
		bool success = g_AavFile->BeginFile(g_CurrentAavFile);
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
	
	g_AavFile->BeginFrame(timeStamp, elapsedTime, exposure);
	return true;
}

void AavFrameAddImage(unsigned char layoutId,  unsigned char* pixels)
{
	g_AavFile->AddFrameImage(layoutId, pixels, 8);
}

void AavFrameAddStatusTag(unsigned int tagIndex, const char* tagValue)
{
	g_AavFile->AddFrameStatusTag(tagIndex, tagValue);
}

void AddFrameStatusTagMessage(unsigned int tagIndex, const char* tagValue)
{	
	g_AavFile->AddFrameStatusTagMessage(tagIndex, tagValue);
}

void AavFrameAddStatusTagUInt8(unsigned int tagIndex, unsigned char tagValue)
{
	g_AavFile->AddFrameStatusTagUInt8(tagIndex, tagValue);
}

void AavFrameAddStatusTag64(unsigned int tagIndex, long long tagValue)
{
	g_AavFile->AddFrameStatusTagUInt64(tagIndex, tagValue);
}

void AavFrameAddStatusTag16(unsigned int tagIndex, unsigned short tagValue)
{
	g_AavFile->AddFrameStatusTagUInt16(tagIndex, tagValue);	
}

void AddFrameStatusTagReal(unsigned int tagIndex, float tagValue)
{
	g_AavFile->AddFrameStatusTagReal(tagIndex, tagValue);
}

void AavEndFrame()
{
	g_AavFile->EndFrame();
}