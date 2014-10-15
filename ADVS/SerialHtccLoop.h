/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

#ifndef SERIAL_HTCC_LOOP
#define SERIAL_HTCC_LOOP

enum HtccCommandType
{
	HtccCmdGetVersion,
	HtccCmdStartOneSecTriggeredExposures,
	HtccCmdStartTwoSecTriggeredExposures,
	HtccCmdStartFourSecTriggeredExposures,
	HtccCmdStartEightSecTriggeredExposures,
	HtccCmdStopTriggeredExposures,
	HtccCmdZeroFrameCounter,
	HtccCmdResetErrorCounter,
	HtccCmdGetGpsPosition,
	HtccCmdGetGpsHeight,
	HtccCmdGetGpsAlmanachUpdateTime,
	HtccCmdStartDateStamps,
	HtccCmdTriggerSingleImageExposure
};

#define LARGE_PORT_BUFFER_SIZE 1024

void* DoSerialHtccLoop(void* arg);
void SendHtccCommand(HtccCommandType cmd);
void ReadHtccPacketBytesIfAvailable();
unsigned char* DeQueueHtccPacket();

#endif