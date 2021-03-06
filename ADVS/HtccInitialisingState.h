/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */


#ifndef HTCC_INITIALISING_STATE
#define HTCC_INITIALISING_STATE

#include "HtccState.h"

class HtccInitialisingState : public HtccState
{
private:
	bool endPacketReceived;
	long ticksTillTransition;
	long ticksTillFiringSoftwareTrigger;
	long timestampFrameIndex;
	HtccMessage* startTimestamp;

public:
	static HtccInitialisingState* s_pInstance;
	static HtccInitialisingState* Instance();
	
	virtual void Initialise(HtccStateMachine *context);
	virtual void Finalise(HtccStateMachine *context);
	virtual HtccMessage* ReceivePacket(HtccStateMachine *context, unsigned char* packet);
	virtual bool ProcessOneSecondTick(HtccStateMachine *context);
};


#endif