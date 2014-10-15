/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

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