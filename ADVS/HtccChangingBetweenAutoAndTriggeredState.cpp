/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

#include "stdafx.h"
#include <stdlib.h>
#include "HtccMessage.h"
#include "SerialHtccLoop.h"
#include "HtccChangingBetweenAutoAndTriggeredState.h"
#include "HtccInitialisingState.h"
#include "GlobalVars.h"


HtccChangingBetweenAutoAndTriggeredState* HtccChangingBetweenAutoAndTriggeredState::s_pInstance = NULL;

HtccChangingBetweenAutoAndTriggeredState* HtccChangingBetweenAutoAndTriggeredState::Instance()
{
	if (!s_pInstance)
		s_pInstance = new HtccChangingBetweenAutoAndTriggeredState();
	
	return s_pInstance;
};

int ticksToWait = 2;

void HtccChangingBetweenAutoAndTriggeredState::Initialise(HtccStateMachine *context)
{
#ifdef HTCC_DETAILED_LOG
	printf("HTCC: Transitioned to HtccChangingBetweenAutoAndTriggeredState\n");
#endif	
	
	s_Camera->SetTriggerMode(true, false);
	s_Camera->AdjustFreeRunningMode(false);
	
	SendHtccCommand(HtccCmdStopTriggeredExposures);
	
	printf("HTCC: Waiting for final frame ... \n");
	ticksToWait = 2;
};

void HtccChangingBetweenAutoAndTriggeredState::Finalise(HtccStateMachine *context)
{ };

HtccMessage* HtccChangingBetweenAutoAndTriggeredState::ReceivePacket(HtccStateMachine *context, unsigned char* packet)
{
	if (packet[0] == 0xFE && packet[1] == 'e')
	{
		#ifdef HTCC_DETAILED_LOG
		printf("HTCC: Received End timestamp ... \n");
		#endif
		
		ticksToWait = 2;
   
		return NULL;
	}
	
	return NULL;
};

bool HtccChangingBetweenAutoAndTriggeredState::ProcessOneSecondTick(HtccStateMachine *context)
{	
	ticksToWait--;
	
	if (ticksToWait <= 0)
	{
		context->SetState(HtccInitialisingState::Instance());
	}
};