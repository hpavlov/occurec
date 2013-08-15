// dllmain.cpp : Defines the entry point for the DLL application.
#include "stdafx.h"
#include "SyncLock.h"
#include <process.h>
#include "OccuRec.Core.h"

HANDLE hFrameProcessingThread;

BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
					 )
{
	switch (ul_reason_for_call)
	{
		case DLL_THREAD_ATTACH:
		case DLL_THREAD_DETACH:
			break;

		case DLL_PROCESS_ATTACH:
			hFrameProcessingThread = (HANDLE)_beginthread(FrameProcessingThreadProc, 0, NULL);
			SyncLock::Initialise();
			break;

		
		case DLL_PROCESS_DETACH:
			SyncLock::Uninitialise();
			break;
	}
	return TRUE;
}
