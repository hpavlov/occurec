#include "stdafx.h"
#include <stdlib.h>
#include "HtccMessage.h"
#include "SerialHtccLoop.h"
#include "HtccState.h"
#include "GlobalVars.h"


void HtccState::Initialise(HtccStateMachine *context)
{
	
}

void HtccState::Finalise(HtccStateMachine *context)
{
	
}

HtccMessage* HtccState::ReceivePacket(HtccStateMachine *context, unsigned char* packet)
{
	return NULL;
}

bool HtccState::ProcessOneSecondTick(HtccStateMachine *context)
{
	
}