/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

#ifndef HTCC_INITIALISED_STATE
#define HTCC_INITIALISED_STATE

#include "HtccState.h"

class HtccInitialisedState: public HtccState
{

public:
	static HtccInitialisedState* s_pInstance;
	static HtccInitialisedState* Instance();


	virtual void Initialise(HtccStateMachine *context);
	virtual void Finalise(HtccStateMachine *context);
	virtual HtccMessage* ReceivePacket(HtccStateMachine *context, unsigned char* packet);
	virtual bool ProcessOneSecondTick(HtccStateMachine *context);
};

#endif