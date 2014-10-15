/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

#include "stdafx.h"
#include "stdlib.h"

#include "GlobalVars.h"
#include "DisplayConsts.h"

#include "AdvrReadyState.h"
#include "AdvrRecordingState.h"

AdvrReadyState* AdvrReadyState::s_pInstance = NULL;


AdvrReadyState::AdvrReadyState()
{ };

AdvrReadyState* AdvrReadyState::Instance()
{
	if (!s_pInstance)
		s_pInstance = new AdvrReadyState();
	
	return s_pInstance;
};

void AdvrReadyState::Initialise(AdvrStateContext *context)
{ }

void AdvrReadyState::IncreaseFrameRate(AdvrStateContext *context)
{
	AdvrState::IncreaseFrameRate(context);
}

void AdvrReadyState::DecreaseFrameRate(AdvrStateContext *context)
{
	AdvrState::DecreaseFrameRate(context);
}

bool AdvrReadyState::ReceiveCommand(AdvrStateContext *context, HcCommand* cmd)
{
	if (cmd->UserCommand == UserCommandStartRecording)
	{
		context->SetState(AdvrRecordingState::Instance());
	}
	else
		AdvrState::ReceiveCommand(context, cmd);
};

void AdvrReadyState::DisplayCurrentFrame(AdvrStateContext *context, unsigned short* pImageData)
{
	AdvrState::DisplayCurrentFrame(context, pImageData);
	
	if (context->IsVideoOnlyMode)
		return;
		
	SDL_Color clrFg = {0,255,0,0};
	SDL_Surface *sText = TTF_RenderText_Solid( s_AdvrFont, "READY (Press '?' for help)", clrFg);
	SDL_Rect rcDest = {10, 10, 0, 0};
	SDL_BlitSurface(sText, NULL, screen, &rcDest);
	SDL_FreeSurface(sText );
};