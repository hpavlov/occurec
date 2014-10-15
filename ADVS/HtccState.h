/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

#ifndef HTCC_STATE
#define HTCC_STATE

#include <stdlib.h>
#include "HtccMessage.h"
#include "SerialHtccLoop.h"
#include "HtccStateMachine.h"

class HtccStateMachine;

class HtccState
{
public:
	virtual void Initialise(HtccStateMachine *context);
	virtual void Finalise(HtccStateMachine *context);
	virtual HtccMessage* ReceivePacket(HtccStateMachine *context, unsigned char* packet);
	virtual bool ProcessOneSecondTick(HtccStateMachine *context);
};


#endif