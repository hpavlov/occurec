#include "stdafx.h"
#include "SyncLock.h"

namespace SyncLock
{

enum LockTech
{
	Mutex,
	CriticalSection,
	CompExch
};

#define LOCK_TECH LockTech::CriticalSection

CRITICAL_SECTION syncVideo;
CRITICAL_SECTION syncIntDet;
CRITICAL_SECTION syncRawFrame;
HANDLE mutexVideo;
HANDLE mutexRawFrame;
HANDLE mutexIntDet;

void Initialise()
{
	if (LOCK_TECH == LockTech::CriticalSection)
	{
		InitializeCriticalSection(&syncVideo);
		InitializeCriticalSection(&syncIntDet);
		InitializeCriticalSection(&syncRawFrame);
	}
};

void Uninitialise()
{
	if (LOCK_TECH == LockTech::CriticalSection)
	{
		DeleteCriticalSection(&syncVideo);
		DeleteCriticalSection(&syncIntDet);
		DeleteCriticalSection(&syncRawFrame);
	}
};

void LockVideo()
{
	if (LOCK_TECH == LockTech::CriticalSection)
		EnterCriticalSection(&syncVideo);
	else if (LOCK_TECH == LockTech::Mutex)
		WaitForSingleObject(mutexVideo, INFINITE);
};

void UnlockVideo()
{
	if (LOCK_TECH == LockTech::CriticalSection)
		LeaveCriticalSection(&syncVideo);
	else if (LOCK_TECH == LockTech::Mutex)
		ReleaseMutex(mutexVideo);
};

void LockRawFrame()
{
	if (LOCK_TECH == LockTech::CriticalSection)
		EnterCriticalSection(&syncRawFrame);
	else if (LOCK_TECH == LockTech::Mutex)
		WaitForSingleObject(mutexRawFrame, INFINITE);
};

void UnlockRawFrame()
{
	if (LOCK_TECH == LockTech::CriticalSection)
		LeaveCriticalSection(&syncRawFrame);
	else if (LOCK_TECH == LockTech::Mutex)
		ReleaseMutex(mutexRawFrame);
};

void LockIntDet()
{
	if (LOCK_TECH == LockTech::CriticalSection)
		EnterCriticalSection(&syncIntDet);
	else if (LOCK_TECH == LockTech::Mutex)
		WaitForSingleObject(mutexIntDet, INFINITE);
};

void UnlockIntDet()
{
	if (LOCK_TECH == LockTech::CriticalSection)
		LeaveCriticalSection(&syncIntDet);
	else if (LOCK_TECH == LockTech::Mutex)
		ReleaseMutex(mutexIntDet);
};

}