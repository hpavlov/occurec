/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

#include "stdafx.h"
#include "stdlib.h"
#include "AdvrShuttingDownState.h"
#include "GlobalVars.h"

AdvrShuttingDownState* AdvrShuttingDownState::s_pInstance = NULL;


AdvrShuttingDownState::AdvrShuttingDownState()
{ };

AdvrShuttingDownState* AdvrShuttingDownState::Instance()
{
	if (!s_pInstance)
		s_pInstance = new AdvrShuttingDownState();
	
	return s_pInstance;
};

void AdvrShuttingDownState::IncreaseFrameRate(AdvrStateContext *context)
{
	
}

void AdvrShuttingDownState::DecreaseFrameRate(AdvrStateContext *context)
{
	
}

bool AdvrShuttingDownState::ReceiveCommand(AdvrStateContext *context, HcCommand* cmd)
{
	AdvrState::ReceiveCommand(context, cmd);
};

void AdvrShuttingDownState::DisplayCurrentFrame(AdvrStateContext *context, unsigned short* pImageData)
{
	AdvrState::DisplayCurrentFrame(context, pImageData);
};

void AdvrShuttingDownState::Initialise(AdvrStateContext *context)
{
	// TODO: Stop file recording and everything else
	// then shutdown the system
	s_Running = false;
}