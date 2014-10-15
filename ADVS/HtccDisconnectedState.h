/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

#ifndef HTCC_DISCONNECTED_STATE
#define HTCC_DISCONNECTED_STATE

#include "HtccState.h"

class HtccDisconnectedState : public HtccState
{
private:
	bool HtccDetected;
	bool UnitIsReady;
	
	// Send a 'V' packet every second until HTCC responds
public:
	static HtccDisconnectedState* s_pInstance;
	static HtccDisconnectedState* Instance();


	virtual void Initialise(HtccStateMachine *context);
	virtual void Finalise(HtccStateMachine *context);
	virtual HtccMessage* ReceivePacket(HtccStateMachine *context, unsigned char* packet);
	virtual bool ProcessOneSecondTick(HtccStateMachine *context);
};

#endif