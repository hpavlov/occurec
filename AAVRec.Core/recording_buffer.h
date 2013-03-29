#pragma once

#include "stdafx.h"

#include <list>
#include "IntegratedFrame.h";

#include <windows.h>
#include <process.h>


using namespace std;

list<IntegratedFrame*> recordingBuffer;
HANDLE ghMutex;


void ClearRecordingBuffer()
{
	WaitForSingleObject(ghMutex, INFINITE);

	list<IntegratedFrame*>::iterator currFrame = recordingBuffer.begin();
	while (currFrame != recordingBuffer.end()) 
	{
		IntegratedFrame* frame = *currFrame;	
		delete frame;
		
		currFrame++;
	}
	recordingBuffer.empty();

	ReleaseMutex(ghMutex);
}

void AddFrameToRecordingBuffer(IntegratedFrame* frameToAdd)
{
	WaitForSingleObject(ghMutex, INFINITE);

	recordingBuffer.push_back(frameToAdd);

	ReleaseMutex(ghMutex);
}

IntegratedFrame* FetchFrameFromRecordingBuffer()
{
	IntegratedFrame* rv = NULL;

	WaitForSingleObject(ghMutex, INFINITE);

	if (recordingBuffer.size() > 0)
	{
		rv = recordingBuffer.front();
		recordingBuffer.pop_front();
	}

	ReleaseMutex(ghMutex);

	return rv;
}