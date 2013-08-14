#pragma once

#include "stdafx.h"

#include <list>
#include "IntegratedFrame.h";

#include <windows.h>
#include <process.h>
#include "SyncLock.h"


using namespace std;

list<IntegratedFrame*> recordingBuffer;


void ClearRecordingBuffer()
{
	SyncLock::LockVideo();

	list<IntegratedFrame*>::iterator currFrame = recordingBuffer.begin();
	while (currFrame != recordingBuffer.end()) 
	{
		IntegratedFrame* frame = *currFrame;	
		delete frame;
		
		currFrame++;
	}
	recordingBuffer.empty();

	SyncLock::UnlockVideo();
}

long AddFrameToRecordingBuffer(IntegratedFrame* frameToAdd)
{
	SyncLock::LockVideo();

	recordingBuffer.push_back(frameToAdd);
	long numItems = recordingBuffer.size();

	SyncLock::UnlockVideo();

	return numItems;
}

IntegratedFrame* FetchFrameFromRecordingBuffer()
{
	IntegratedFrame* rv = NULL;

	SyncLock::LockVideo();

	if (recordingBuffer.size() > 0)
	{
		rv = recordingBuffer.front();
		recordingBuffer.pop_front();
	}

	SyncLock::UnlockVideo();

	return rv;
}

