#include "stdafx.h"
#include "SyncLock.h"
#include "SpinLock.h"
#include "utils.h"

namespace SyncLock
{

enum LockTech
{
	Mutex,
	CriticalSection,
	CompExch,
	Event
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
HANDLE eventVideo;
HANDLE eventRawFrame; 
HANDLE eventIntDet; 


void DebugPrintLastError(const char* message)
{
	DWORD error = GetLastError();
	DebugViewPrint(L"%s: %d", message, error);
}

void Initialise()
{
	if (LOCK_TECH == LockTech::CriticalSection)
	{
		if (0 == InitializeCriticalSectionAndSpinCount(&syncVideo, 4000))
			DebugPrintLastError("Error calling InitializeCriticalSectionAndSpinCount(&syncVideo, 4000)");

		if (0 == InitializeCriticalSectionAndSpinCount(&syncIntDet, 4000))
			DebugPrintLastError("Error calling InitializeCriticalSectionAndSpinCount(&syncVideo, 4000)");

		if (0 == InitializeCriticalSectionAndSpinCount(&syncRawFrame, 4000))
			DebugPrintLastError("Error calling InitializeCriticalSectionAndSpinCount(&syncVideo, 4000)");
	}
	else if (LOCK_TECH == LockTech::Event)
	{
		eventVideo = CreateEvent( NULL, TRUE, TRUE,  L"SyncLockEventVideo"); 
		eventRawFrame = CreateEvent( NULL, TRUE, TRUE,  L"SyncLockEventRawFrame"); 
		eventIntDet = CreateEvent( NULL, TRUE, TRUE,  L"SyncLockEventIntDet"); 
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
	else if (LOCK_TECH == LockTech::Event)
	{
		WaitForSingleObject(eventVideo, INFINITE);
		ResetEvent(eventVideo);
	}
};

void UnlockVideo()
{
	if (LOCK_TECH == LockTech::CriticalSection)
		LeaveCriticalSection(&syncVideo);
	else if (LOCK_TECH == LockTech::Mutex)
		ReleaseMutex(mutexVideo);
	else if (LOCK_TECH == LockTech::CompExch)
		spinWait.Unlock(lockVideo);
	else if (LOCK_TECH == LockTech::Event)
		SetEvent(eventVideo);
};

void LockRawFrame()
{
	if (LOCK_TECH == LockTech::CriticalSection)
		EnterCriticalSection(&syncRawFrame);
	else if (LOCK_TECH == LockTech::Mutex)
		WaitForSingleObject(mutexRawFrame, INFINITE);
	else if (LOCK_TECH == LockTech::CompExch)
		spinWait.Lock(lockRawFrame);
	else if (LOCK_TECH == LockTech::Event)
	{
		WaitForSingleObject(eventRawFrame, INFINITE);
		ResetEvent(eventRawFrame);
	}
};

void UnlockRawFrame()
{
	if (LOCK_TECH == LockTech::CriticalSection)
		LeaveCriticalSection(&syncRawFrame);
	else if (LOCK_TECH == LockTech::Mutex)
		ReleaseMutex(mutexRawFrame);
	else if (LOCK_TECH == LockTech::CompExch)
		spinWait.Unlock(lockRawFrame);
	else if (LOCK_TECH == LockTech::Event)
		SetEvent(eventRawFrame);
};

void LockIntDet()
{
	if (LOCK_TECH == LockTech::CriticalSection)
		EnterCriticalSection(&syncIntDet);
	else if (LOCK_TECH == LockTech::Mutex)
		WaitForSingleObject(mutexIntDet, INFINITE);
	else if (LOCK_TECH == LockTech::CompExch)
		spinWait.Lock(lockIntDet);
	else if (LOCK_TECH == LockTech::Event)
	{
		WaitForSingleObject(eventIntDet, INFINITE);
		ResetEvent(eventIntDet);
	}
};

void UnlockIntDet()
{
	if (LOCK_TECH == LockTech::CriticalSection)
		LeaveCriticalSection(&syncIntDet);
	else if (LOCK_TECH == LockTech::Mutex)
		ReleaseMutex(mutexIntDet);
	else if (LOCK_TECH == LockTech::CompExch)
		spinWait.Unlock(lockIntDet);
	else if (LOCK_TECH == LockTech::Event)
		SetEvent(eventIntDet);
};

}