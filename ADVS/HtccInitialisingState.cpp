/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

#include "stdafx.h"
#include <stdlib.h>
#include "HtccMessage.h"
#include "SerialHtccLoop.h"
#include "HtccInitialisingState.h"
#include "HtccInitialisedState.h"
#include "HtccWaitingForFirstFrameState.h"
#include "GlobalVars.h"


HtccInitialisingState* HtccInitialisingState::s_pInstance = NULL;

HtccInitialisingState* HtccInitialisingState::Instance()
{
	if (!s_pInstance)
		s_pInstance = new HtccInitialisingState();
	
	return s_pInstance;
};

void HtccInitialisingState::Initialise(HtccStateMachine *context)
{
#ifdef HTCC_DETAILED_LOG
	printf("HTCC: Transitioned to HtccInitialisingState\n");
#endif
	
	endPacketReceived = false;
	timestampFrameIndex = 0;
	startTimestamp = NULL;
	
	s_Camera->SetTriggerMode(true, false);
	s_Camera->AdjustFreeRunningMode(false);
	
	//Reset the eror and frame counter
	SendHtccCommand(HtccCmdResetErrorCounter);
	SendHtccCommand(HtccCmdZeroFrameCounter);
	
	// Trigger a single exposure of 727 ms
	s_Camera->EnableFrameRate(true, false);
	s_Camera->SetExtendedShutterSpeed(727, false);
	
	if (USE_SOFTWARE_TRIGGER)
	{
		ticksTillFiringSoftwareTrigger = 2;
	}
	else
	{
		// NOTE: HtccCmdTriggerSingleImageExposure doesn't seem to send the START of the frame so using 8 sec trigger instead	
		SendHtccCommand(HtccCmdStartEightSecTriggeredExposures);		
	}
};

void HtccInitialisingState::Finalise(HtccStateMachine *context)
{ };

HtccMessage* HtccInitialisingState::ReceivePacket(HtccStateMachine *context, unsigned char* packet)
{
	if (packet[0] == 0xFE && packet[1] == 's')
	{
		startTimestamp = new HtccMessage(packet);
		
		#ifdef HTCC_DETAILED_LOG
		printf("HTCC: Initializing image (%ld) start ... \n", startTimestamp->FrameIndex);
		#endif
	}
	else if (packet[0] == 0xFE && packet[1] == 'e')
	{
		HtccMessage* endTimestamp = new HtccMessage(packet);

		#ifdef HTCC_DETAILED_LOG
		printf("HTCC: Initializing image (%ld) end... \n", endTimestamp->FrameIndex);
		#endif
		
		if (NULL != startTimestamp)
		{
			long long start10thMs = DateTimeToAdvTicks(startTimestamp->TimestampUtcYear, startTimestamp->TimestampUtcMonth, startTimestamp->TimestampUtcDay, startTimestamp->TimestampUtcHours, startTimestamp->TimestampUtcMinutes, startTimestamp->TimestampUtcSecond, startTimestamp->TimestampUtcFractionalSecond10000);
			long long end10thMs = DateTimeToAdvTicks(endTimestamp->TimestampUtcYear, endTimestamp->TimestampUtcMonth, endTimestamp->TimestampUtcDay, endTimestamp->TimestampUtcHours, endTimestamp->TimestampUtcMinutes, endTimestamp->TimestampUtcSecond, endTimestamp->TimestampUtcFractionalSecond10000);
			
			int exposureInMilliseconds = (int)((end10thMs - start10thMs) / 10);
			
				#ifdef HTCC_DETAILED_LOG
				printf("HTCC: end - start = %d ms... \n", exposureInMilliseconds);
				#endif
			
			if (startTimestamp->FrameIndex == endTimestamp->FrameIndex)
			{
				int exporeDiff = exposureInMilliseconds - 727;
				
				#ifdef HTCC_DETAILED_LOG
				printf("HTCC: Initializing image exposure is %d ms... \n", exposureInMilliseconds);
				#endif
				
				if (exporeDiff > -20 && exporeDiff < 20)
				{
					timestampFrameIndex = endTimestamp->FrameIndex;
					endPacketReceived = true;
					
					// We have identified the image we triggered (so there is nothing else in the buffers)
					// Now we wait for 2 sec before we transition to the next state
					ticksTillTransition = 2;
					//printf("HTCC: Received SYNC End timestamp for FrameId=%d \n", (int)endTimestamp->FrameIndex);
					
					if (!USE_SOFTWARE_TRIGGER)
						SendHtccCommand(HtccCmdStopTriggeredExposures);
				}
				else
				{
					// The image we received is not with the correct duration
					#ifdef HTCC_DETAILED_LOG
					printf("HTCC: Received image (%ld) with exposure of (%d) ms. Re-triggering ...\n", endTimestamp->FrameIndex, exposureInMilliseconds);
					#endif
					
					if (USE_SOFTWARE_TRIGGER)
					{
						s_Camera->EnableFrameRate(true, false);
						s_Camera->SetExtendedShutterSpeed(727, false);
						
						ticksTillFiringSoftwareTrigger = 2;
					}					
				}				
			}
		}

		return NULL;
	}
	
	return NULL;
};

bool HtccInitialisingState::ProcessOneSecondTick(HtccStateMachine *context)
{
	if (endPacketReceived)
	{
		ticksTillTransition--;
		
		if (ticksTillTransition == 0)
		{
			context->SetState(HtccWaitingForFirstFrameState::Instance());
		}
	}

	if (USE_SOFTWARE_TRIGGER)
	{
		ticksTillFiringSoftwareTrigger--;
		
		if (ticksTillFiringSoftwareTrigger == 1)
		{
			if (startTimestamp != NULL)
			{
				delete startTimestamp;
				startTimestamp = NULL;
			}
			
			endPacketReceived = false;			
		}
		
		if (ticksTillFiringSoftwareTrigger == 0)
		{
			s_Camera->PollForTriggerReady();
			s_Camera->FireSoftwareTrigger();
		}
	}	
};