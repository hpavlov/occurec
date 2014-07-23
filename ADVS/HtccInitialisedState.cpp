#include "stdafx.h"
#include <stdlib.h>
#include "HtccMessage.h"
#include "SerialHtccLoop.h"
#include "HtccInitialisedState.h"
#include "GlobalVars.h"


HtccInitialisedState* HtccInitialisedState::s_pInstance = NULL;

HtccInitialisedState* HtccInitialisedState::Instance()
{
	if (!s_pInstance)
		s_pInstance = new HtccInitialisedState();
	
	return s_pInstance;
};

long secondsRunning = 1;

void HtccInitialisedState::Initialise(HtccStateMachine *context)
{ 
#ifdef HTCC_DETAILED_LOG
	printf("HTCC: Transitioned to HtccInitialisedState\n");
#endif
	
	s_Camera->ClearOperationErrorMessage();
	s_AdvrState->HtccConnected();
	
	secondsRunning = 0;
};

void HtccInitialisedState::Finalise(HtccStateMachine *context)
{ };

int lastFrameIndexId = -1;
bool endTimeStampReceived = false;

HtccMessage* HtccInitialisedState::ReceivePacket(HtccStateMachine *context, unsigned char* packet)
{
	HtccMessage* msg = new HtccMessage(packet);
	

    /*
	if (packet[0] == 0xFE && packet[1] == 's')
	{
		if (lastFrameIndexId != -1 && lastFrameIndexId != msg->FrameIndex - 1)
		{	
			// Frame timestamps from HTCC should come in order 
			printf("ERROR: HTCC timestamp not received in order. Last Start FrameId = %d; Current Start FrameId = %d\n", lastFrameIndexId, (int)msg->FrameIndex);
		}
		else if (!endTimeStampReceived && lastFrameIndexId != -1)
		{
			printf("ERROR: HTCC no end timestamp received for FrameId = %d\n", lastFrameIndexId);
		}
		lastFrameIndexId = msg->FrameIndex;
		endTimeStampReceived = false;
	}
	else if (packet[0] == 0xFE && packet[1] == 'e')
	{
		if (lastFrameIndexId != -1 && lastFrameIndexId != msg->FrameIndex)
		{	
			// Frame timestamps from HTCC should come in order 
			printf("ERROR: HTCC time stamp not received in order. Last Start FrameId = %d; Current End FrameId = %d\n", lastFrameIndexId, (int)msg->FrameIndex);
		}
		else if (lastFrameIndexId != -1)
			endTimeStampReceived = true;		
	}*/
	
	//if (packet[0] == 0xFE && packet[1] == 'e')
	//{
	//	printf("HTCC time stamp received for Htcc Frame Id : %li\n", msg->FrameIndex);
	//}

	return msg;
};

bool HtccInitialisedState::ProcessOneSecondTick(HtccStateMachine *context)
{ 
	// Once every 15 min resample the Geolocation data from HTTC	
	if (secondsRunning % (15 * 60) == 0)
	{
		SendHtccCommand(HtccCmdGetGpsPosition);
	}
	
	secondsRunning++;
};