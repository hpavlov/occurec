/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

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

