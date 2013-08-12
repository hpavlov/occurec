#include "stdafx.h"
#include "SyncLock.h"
#include "SpinLock.h"

namespace SyncLock
{

enum LockTech
{
	Mutex,
	CriticalSection,
	CompExch
};

#define LOCK_TECH LockTech::Mutex

CRITICAL_SECTION syncVideo;
CRITICAL_SECTION syncIntDet;
CRITICAL_SECTION syncRawFrame;
HANDLE mutexVideo;
HANDLE mutexRawFrame;
HANDLE mutexIntDet;
LockFree::tSpinLock lockVideo;
LockFree::tSpinLock lockIntDet;
LockFree::tSpinLock lockRawFrame;
LockFree::tSpinWait spinWait;

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
	else if (LOCK_TECH == LockTech::CompExch)
		spinWait.Lock(lockVideo);
};

void UnlockVideo()
{
	if (LOCK_TECH == LockTech::CriticalSection)
		LeaveCriticalSection(&syncVideo);
	else if (LOCK_TECH == LockTech::Mutex)
		ReleaseMutex(mutexVideo);
	else if (LOCK_TECH == LockTech::CompExch)
		spinWait.Unlock(lockVideo);
};

void LockRawFrame()
{
	if (LOCK_TECH == LockTech::CriticalSection)
		EnterCriticalSection(&syncRawFrame);
	else if (LOCK_TECH == LockTech::Mutex)
		WaitForSingleObject(mutexRawFrame, INFINITE);
	else if (LOCK_TECH == LockTech::CompExch)
		spinWait.Lock(lockRawFrame);
};

void UnlockRawFrame()
{
	if (LOCK_TECH == LockTech::CriticalSection)
		LeaveCriticalSection(&syncRawFrame);
	else if (LOCK_TECH == LockTech::Mutex)
		ReleaseMutex(mutexRawFrame);
	else if (LOCK_TECH == LockTech::CompExch)
		spinWait.Unlock(lockRawFrame);
};

void LockIntDet()
{
	if (LOCK_TECH == LockTech::CriticalSection)
		EnterCriticalSection(&syncIntDet);
	else if (LOCK_TECH == LockTech::Mutex)
		WaitForSingleObject(mutexIntDet, INFINITE);
	else if (LOCK_TECH == LockTech::CompExch)
		spinWait.Lock(lockIntDet);
};

void UnlockIntDet()
{
	if (LOCK_TECH == LockTech::CriticalSection)
		LeaveCriticalSection(&syncIntDet);
	else if (LOCK_TECH == LockTech::Mutex)
		ReleaseMutex(mutexIntDet);
	else if (LOCK_TECH == LockTech::CompExch)
		spinWait.Unlock(lockIntDet);
};

}