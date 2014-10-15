/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

#ifndef HTCC_CHANGING_BETWEEN_AUTO_AND_TRIGGERED_STATE
#define HTCC_CHANGING_BETWEEN_AUTO_AND_TRIGGERED_STATE

#include "HtccState.h"

class HtccChangingBetweenAutoAndTriggeredState: public HtccState
{
public:
	static HtccChangingBetweenAutoAndTriggeredState* s_pInstance;
	static HtccChangingBetweenAutoAndTriggeredState* Instance();


	virtual void Initialise(HtccStateMachine *context);
	virtual void Finalise(HtccStateMachine *context);
	virtual HtccMessage* ReceivePacket(HtccStateMachine *context, unsigned char* packet);
	virtual bool ProcessOneSecondTick(HtccStateMachine *context);
};

#endif