/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

// dllmain.cpp : Defines the entry point for the DLL application.
#include "stdafx.h"
#include "SyncLock.h"
#include <process.h>
#include "OccuRec.Core.h"
#include "utils.h"

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
			DebugViewPrint(L"OccuRec: DLL_PROCESS_ATTACH\r\n");
			hFrameProcessingThread = (HANDLE)_beginthread(FrameProcessingThreadProc, 0, NULL);
			SyncLock::Initialise();
			break;
		
		case DLL_PROCESS_DETACH:
			DebugViewPrint(L"OccuRec: DLL_PROCESS_DETACH\r\n");
			SyncLock::Uninitialise();
			break;
	}
	return TRUE;
}

