/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

#pragma once

#include "stdafx.h"

#include <list>
#include "IntegratedFrame.h";

#include <windows.h>
#include <process.h>
#include "SyncLock.h"


using namespace std;

IntegratedFrame* recordingBuffer[1024];
long currentIndex = -1;


void ClearRecordingBuffer()
{
	SyncLock::LockVideo();

	while (currentIndex >= 0)
	{
		IntegratedFrame* frame = recordingBuffer[currentIndex];	
		delete frame;
		
		currentIndex--;
	}

	SyncLock::UnlockVideo();
}

long AddFrameToRecordingBuffer(IntegratedFrame* frameToAdd)
{
	SyncLock::LockVideo();

	currentIndex++;
	recordingBuffer[currentIndex] = frameToAdd;
	long numItems = currentIndex + 1;

	SyncLock::UnlockVideo();

	return numItems;
}

IntegratedFrame* FetchFrameFromRecordingBuffer()
{
	IntegratedFrame* rv = NULL;

	SyncLock::LockVideo();

	if (currentIndex >= 0)
	{
		rv = recordingBuffer[0];
		for (int i = 0; i < currentIndex; i++)
			recordingBuffer[i] = recordingBuffer[i + 1];

		currentIndex--;
	}

	SyncLock::UnlockVideo();

	return rv;
}

