#pragma once

#include "stdafx.h"

#include <list>
#include "RawFrame.h";

#include <windows.h>
#include "SyncLock.h"

using namespace std;

list<RawFrame*> rawFrameBuffer;


void ClearRawFrameBuffer()
{
	SyncLock::LockRawFrame();

	list<RawFrame*>::iterator currFrame = rawFrameBuffer.begin();
	while (currFrame != rawFrameBuffer.end()) 
	{
		RawFrame* frame = *currFrame;	
		delete frame;
		
		currFrame++;
	}
	rawFrameBuffer.empty();

	SyncLock::UnlockRawFrame();
}

long AddFrameToRawFrameBuffer(RawFrame* frameToAdd)
{
	SyncLock::LockRawFrame();

	rawFrameBuffer.push_back(frameToAdd);
	long numItems = rawFrameBuffer.size();

	SyncLock::UnlockRawFrame();

	return numItems;
}

RawFrame* FetchFrameFromRawFrameBuffer()
{
	RawFrame* rv = NULL;

	SyncLock::LockRawFrame();

	if (rawFrameBuffer.size() > 0)
	{
		rv = rawFrameBuffer.front();
		rawFrameBuffer.pop_front();
	}

	SyncLock::UnlockRawFrame();

	return rv;
}

