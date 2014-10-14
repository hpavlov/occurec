/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

#include "stdafx.h"
#include "SyncLock.h"
#include "SpinLock.h"
#include <Windows.h>
#include <Dbghelp.h>
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


void make_minidump(EXCEPTION_POINTERS* e)
{
	DebugViewPrint(L"OccuRec: CREATING ERROR DUMP\r\n");

    auto hDbgHelp = LoadLibraryA("dbghelp");
    if(hDbgHelp == nullptr)
        return;
    auto pMiniDumpWriteDump = (decltype(&MiniDumpWriteDump))GetProcAddress(hDbgHelp, "MiniDumpWriteDump");
    if(pMiniDumpWriteDump == nullptr)
        return;

    char name[MAX_PATH];
    {
        auto nameEnd = name + GetModuleFileNameA(GetModuleHandleA(0), name, MAX_PATH);
        SYSTEMTIME t;
        GetSystemTime(&t);
        wsprintfA(nameEnd - strlen(".exe"),
            "_%4d%02d%02d_%02d%02d%02d.dmp",
            t.wYear, t.wMonth, t.wDay, t.wHour, t.wMinute, t.wSecond);
    }

    auto hFile = CreateFileA(name, GENERIC_WRITE, FILE_SHARE_READ, 0, CREATE_ALWAYS, FILE_ATTRIBUTE_NORMAL, 0);
    if(hFile == INVALID_HANDLE_VALUE)
        return;

    MINIDUMP_EXCEPTION_INFORMATION exceptionInfo;
    exceptionInfo.ThreadId = GetCurrentThreadId();
    exceptionInfo.ExceptionPointers = e;
    exceptionInfo.ClientPointers = FALSE;

    auto dumped = pMiniDumpWriteDump(
        GetCurrentProcess(),
        GetCurrentProcessId(),
        hFile,
        MINIDUMP_TYPE(MiniDumpWithIndirectlyReferencedMemory | MiniDumpScanMemory),
        e ? &exceptionInfo : nullptr,
        nullptr,
        nullptr);

    CloseHandle(hFile);

    return;
}

LONG CALLBACK unhandled_handler(EXCEPTION_POINTERS* e)
{
    make_minidump(e);
    return EXCEPTION_CONTINUE_SEARCH;
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

	SetUnhandledExceptionFilter(unhandled_handler);
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