/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

#include "stdafx.h"
#include <stdlib.h>
#include "HtccMessage.h"
#include "SerialHtccLoop.h"
#include "HtccDisconnectedState.h"
#include "HtccInitialisingState.h"
#include "GlobalVars.h"
#include "GlobalConfig.h"


HtccDisconnectedState* HtccDisconnectedState::s_pInstance = NULL;

HtccDisconnectedState* HtccDisconnectedState::Instance()
{
	if (!s_pInstance)
		s_pInstance = new HtccDisconnectedState();
	
	return s_pInstance;
};


void HtccDisconnectedState::Initialise(HtccStateMachine *context)
{ 
#ifdef HTCC_DETAILED_LOG
	printf("HTCC: Transitioned to HtccDisconnectedState\n");
#endif	

	s_AdvrState->HtccSecondsRemainingToGSPSignal = 999999;
	s_AdvrState->IsCameraDisconnectedFromHtcc = false;
	
	HtccDetected = false;	
};

void HtccDisconnectedState::Finalise(HtccStateMachine *context)
{ };

HtccMessage* HtccDisconnectedState::ReceivePacket(HtccStateMachine *context, unsigned char* packet)
{
	// Only respond to a version response packet
	if (packet[0] == 0xFE && packet[1] == 'v')
	{
		HtccMessage* msg = new HtccMessage(packet);
		
		if (!HtccDetected)
		{
			SYS_HTCC_FIRMWARE_VERSION = const_cast<char*>((char*)malloc(16));
			sprintf(&const_cast<char*>(SYS_HTCC_FIRMWARE_VERSION)[0], "%d.%d.%d", msg->VersionMajor, msg->VersionMinor, msg->VersionBuild);		
			
			printf("HTCC: HTCC v%s detected\n", SYS_HTCC_FIRMWARE_VERSION);
			
			sprintf(&SYS_HTCC_UNIT_NUMBER[0], "%s", &msg->UnitSerialNumber[0]);
			
			HtccDetected = true;
			
			if (!msg->VersionGpsIsReady)
			{
				printf("HTCC: Waiting for HTCC unit to become ready ...\n");
				s_AdvrState->HtccSecondsRemainingToGSPSignal = 7;
			}
		}
		
		
		if (IGNORE_HTCC_CAMERA_FLAG)
			UnitIsReady = msg->VersionGpsIsReady;
		else
		{
			UnitIsReady = msg->VersionGpsIsReady && msg->VersionCameraIsReady;
			
			if (msg->VersionGpsIsReady)
				s_AdvrState->IsCameraDisconnectedFromHtcc = !msg->VersionCameraIsReady;
		}
		
		if (HtccDetected && UnitIsReady)
		{
			printf("HTCC: HTCC unit is ready\n");
			
			s_AdvrState->IsHtccDetected = true;
			s_AdvrState->IsReSynchronisingTimestamps = false;
			context->SetState(HtccInitialisingState::Instance());			
			
			return msg;
		}
		
		return NULL;
	}
	else
		return NULL;
};

bool HtccDisconnectedState::ProcessOneSecondTick(HtccStateMachine *context)
{	
	if (s_Camera != NULL)
	{
		if (s_Camera->IsConnected && !s_Camera->IsManualTriggeringMode)
			s_Camera->SetTriggerMode(true, false); 
			
		SendHtccCommand(HtccCmdGetVersion);
		
		s_AdvrState->HtccSecondsRemainingToGSPSignal--;
		
		#ifdef HTCC_DETAILED_LOG
		printf("HTCC: Version command sent. Waiting for response. \n");
		#endif
	}
};
