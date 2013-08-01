#pragma once

#include "stdafx.h"

#include <list>
#include "RawFrame.h";

#include <windows.h>
#include <process.h>


using namespace std;

list<RawFrame*> rawFrameBuffer;
HANDLE ghRawFrameMutex;


void ClearRawFrameBuffer()
{
	WaitForSingleObject(ghRawFrameMutex, INFINITE);

	list<RawFrame*>::iterator currFrame = rawFrameBuffer.begin();
	while (currFrame != rawFrameBuffer.end()) 
	{
		RawFrame* frame = *currFrame;	
		delete frame;
		
		currFrame++;
	}
	rawFrameBuffer.empty();

	ReleaseMutex(ghRawFrameMutex);
}

long AddFrameToRawFrameBuffer(RawFrame* frameToAdd)
{
	WaitForSingleObject(ghRawFrameMutex, INFINITE);

	rawFrameBuffer.push_back(frameToAdd);
	long numItems = rawFrameBuffer.size();

	ReleaseMutex(ghRawFrameMutex);

	return numItems;
}

RawFrame* FetchFrameFromRawFrameBuffer()
{
	RawFrame* rv = NULL;

	WaitForSingleObject(ghRawFrameMutex, INFINITE);

	if (rawFrameBuffer.size() > 0)
	{
		rv = rawFrameBuffer.front();
		rawFrameBuffer.pop_front();
	}

	ReleaseMutex(ghRawFrameMutex);

	return rv;
}

