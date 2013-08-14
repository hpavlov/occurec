#ifndef SYNC_LOCK_H
#define SYNC_LOCK_H

#include "stdafx.h"
#include "windows.h"


namespace SyncLock
{
	void Initialise();
	void Uninitialise();

	void LockVideo();
	void UnlockVideo();

	void LockRawFrame();
	void UnlockRawFrame();

	void LockIntDet();
	void UnlockIntDet();
}

#endif

