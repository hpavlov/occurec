/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

#pragma once
class RawFrame
{
public:
	unsigned char* BmpBits;
	long BmpBitsSize;
	__int64 CurrentUtcDayAsTicks;
	__int64 CurrentNtpTimeAsTicks;
	double NtpBasedTimeError;
	__int64 CurrentSecondaryTimeAsTicks;

	RawFrame(int imageWidth, int imageHeight);
	~RawFrame(void);
};

