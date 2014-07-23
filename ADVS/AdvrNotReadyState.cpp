#include "stdafx.h"
#include "stdlib.h"

#include "GlobalVars.h"
#include "GlobalConfig.h"
#include "DisplayConsts.h"

#include "AdvrNotReadyState.h"
#include "AdvrReadyState.h"
#include "SerialHtccLoop.h"
#include "SerialHcPolling.h"
#include "HcCommand.h"


AdvrNotReadyState* AdvrNotReadyState::s_pInstance = NULL;
unsigned char* AdvrNotReadyState::s_LogoPixels = NULL;

AdvrNotReadyState::AdvrNotReadyState()
{
	s_LogoPixels = (unsigned char*)malloc(640 * 480 * 3);
	FILE* f = fopen("res/splash.v4.rgb", "r");
	fread(&s_LogoPixels[0], 1, 640 * 480 * 3, f);			
	fclose(f);
};

AdvrNotReadyState* AdvrNotReadyState::Instance()
{
	if (!s_pInstance)
		s_pInstance = new AdvrNotReadyState();
	
	return s_pInstance;
};

void AdvrNotReadyState::Initialise(AdvrStateContext* context)
{
	s_HasPixelData = false;
	
	context->FrameCountingSynchronised = false;
	context->FirstCameraFrameReceived = false;
	context->FirstCameraFrameTimestampReceived = false;
	context->ExpectFirstCameraFrame = false;
	
	snprintf(s_TimeStamp, 30, "                    ");
}

void AdvrNotReadyState::SystemInitialised(AdvrStateContext *context)
{
	context->SetState(AdvrReadyState::Instance());
}

void AdvrNotReadyState::IncreaseFrameRate(AdvrStateContext *context)
{
	
}

void AdvrNotReadyState::DecreaseFrameRate(AdvrStateContext *context)
{
	
}

bool AdvrNotReadyState::ReceiveCommand(AdvrStateContext *context, HcCommand *cmd)
{
	if (cmd->UserCommand == UserMessageVersionInfo)
	{
		// Hc is alive. Record its version and mark it as connected
	}
	else if (cmd->UserCommand == UserCommandSystemShutdown)
	{
		AdvrState::ReceiveCommand(context, cmd);
	}
};

bool AdvrNotReadyState::ReceiveMessage(AdvrStateContext *context, HtccMessage *msg)
{
	if (msg->MessageType == HtccMsgVersionInfo)
	{
		// NOTE: Htcc is alive. Record its version and mark it as connected
	};
	
	AdvrState::ReceiveMessage(context, msg);
};
	
void AdvrNotReadyState::ProcessCurrentFrame(AdvrStateContext *context, PtGreyImage* image, FrameExposureInfo* exposure)
{
	// We are not ready until we receive this image!
	// And we are not ready for at least 5 sec (??) to show the splash screen	
}

void AdvrNotReadyState::DisplayCurrentFrame(AdvrStateContext *context, unsigned short* pImageData)
{
	// Don't want to display camera images while booting up and displaying the splash screen
	// AdvrState::DisplayCurrentFrame(context, pImageData);
	
	s_HasPixelData = true;
	AdvrState::DisplayColourFrame(s_LogoPixels);
	
	SDL_Surface *sText;
	/*
	SDL_FillRect(screen, &RECT_FULL_SCREEN, 0x202020);
	
	sText = TTF_RenderText_Solid(s_AdvrLogoFont, "ADVR", CLR_GRAY);
	SDL_Rect rcDest = {150, 150, 450, 350};
	SDL_BlitSurface(sText, NULL, screen, &rcDest);
	SDL_FreeSurface(sText); */

	char* displayVersion = new char[250];
	snprintf(displayVersion, 50, "ADVR version %s", SYS_ADVR_SOFTWARE_VERSION);
	sText = TTF_RenderText_Solid(s_AdvrFont, displayVersion, CLR_WHITE);
	SDL_Rect rcDest2 = {40, 400, 280, 440};
	SDL_BlitSurface(sText, NULL, screen, &rcDest2);
	SDL_FreeSurface(sText); 
	
	snprintf(displayVersion, 240, "Output location: %s", CFG_SAVED_FILE_LOCATION);
	sText = TTF_RenderText_Solid(s_AdvrFont, displayVersion, CLR_WHITE);
	SDL_Rect rcDest4 = {40, 460, 430, 480};
	SDL_BlitSurface(sText, NULL, screen, &rcDest4);
	SDL_FreeSurface(sText); 
	
	snprintf(displayVersion, 240, "HTCC Port: %s", CFG_HTCC_DEVICE);
	sText = TTF_RenderText_Solid(s_AdvrFont, displayVersion, CLR_WHITE);
	SDL_Rect rcDest5 = {440, 460, 630, 480};
	SDL_BlitSurface(sText, NULL, screen, &rcDest5);
	SDL_FreeSurface(sText); 	
	
	if (SYS_HTCC_FIRMWARE_VERSION[0] != '?')
	{
		snprintf(displayVersion, 50, "HTCC version %s", SYS_HTCC_FIRMWARE_VERSION);
		sText = TTF_RenderText_Solid(s_AdvrFont, displayVersion, CLR_WHITE);
		SDL_Rect rcDest3 = {40, 420, 280, 460};
		SDL_BlitSurface(sText, NULL, screen, &rcDest3);
		SDL_FreeSurface(sText); 		
	}

    if (!context->IsCameraDetected)
	{
		sText = TTF_RenderText_Solid(s_AdvrFont, "CAMERA NOT DETECTED", CLR_RED);
		SDL_Rect rcDest2 = {10, 10, 0, 0};
		SDL_BlitSurface(sText, NULL, screen, &rcDest2);
		SDL_FreeSurface(sText);
		
		if (s_Camera != NULL && s_Camera->ConnectionErrorMessage != NULL)
		{
			sText = TTF_RenderText_Solid( s_AdvrFont, s_Camera->ConnectionErrorMessage, CLR_RED);
			SDL_Rect rcDest22= {200, 10, 0, 0};
			SDL_BlitSurface(sText, NULL, screen, &rcDest22);
			SDL_FreeSurface(sText);
		}
	}
	else if (!context->IsCameraConnected)
	{
		sText = TTF_RenderText_Solid(s_AdvrFont, "WAITING FOR CAMERA ...", CLR_RED);
		SDL_Rect rcDest3 = {10, 10, 0, 0};
		SDL_BlitSurface(sText, NULL, screen, &rcDest3);
		SDL_FreeSurface(sText);
		
		if (s_Camera != NULL && s_Camera->OperationErrorMessage != NULL)
		{
			sText = TTF_RenderText_Solid( s_AdvrFont, s_Camera->OperationErrorMessage, CLR_RED);
			SDL_Rect rcDest32= {200, 10, 0, 0};
			SDL_BlitSurface(sText, NULL, screen, &rcDest32);
			SDL_FreeSurface(sText);
		}		
	}
	else
	{
		sText = TTF_RenderText_Solid(s_AdvrFont, "CAMERA CONNECTED", CLR_GREEN);
		SDL_Rect rcDest2 = {10, 10, 0, 0};
		SDL_BlitSurface(sText, NULL, screen, &rcDest2);
		SDL_FreeSurface(sText);
	}
	
	if (context->FrameCountingSynchronised)
	{
		sText = TTF_RenderText_Solid(s_AdvrFont, "HTCC+GPS CONNECTED", CLR_GREEN);
		SDL_Rect rcDest2 = {10, 40, 0, 0};
		SDL_BlitSurface(sText, NULL, screen, &rcDest2);
		SDL_FreeSurface(sText);		
	}
	else if (context->IsHtccDetected)
	{
		if (context->IsReSynchronisingTimestamps)
			sText = TTF_RenderText_Solid(s_AdvrFont, "RE-SYNCHRONISING HTCC AND CAMERA DUE TO MODE CHANGE ...", CLR_YELLOW);
		else
			sText = TTF_RenderText_Solid(s_AdvrFont, "INITIALISING ...", CLR_YELLOW);
		SDL_Rect rcDest2 = {10, 40, 0, 0};
		SDL_BlitSurface(sText, NULL, screen, &rcDest2);
		SDL_FreeSurface(sText);					
	}
	else
	{
		if (context->IsCameraDisconnectedFromHtcc)
			sText = TTF_RenderText_Solid(s_AdvrFont, "CAMERA NOT CONNECTED TO HTCC. CHECK CABLE.", CLR_RED);
		else if (context->HtccSecondsRemainingToGSPSignal > 100)
			sText = TTF_RenderText_Solid(s_AdvrFont, "CONNECTING TO HTCC ...", CLR_RED);
		else if (context->HtccSecondsRemainingToGSPSignal > 0)
		{
			snprintf(displayVersion, 50, "WAITING FOR GPS SIGNAL (%d) ...", context->HtccSecondsRemainingToGSPSignal);
			sText = TTF_RenderText_Solid(s_AdvrFont, displayVersion, CLR_RED);
		}
		else
			sText = TTF_RenderText_Solid(s_AdvrFont, "WAITING FOR GPS SIGNAL ...", CLR_RED);
		SDL_Rect rcDest2 = {10, 40, 0, 0};
		SDL_BlitSurface(sText, NULL, screen, &rcDest2);
		SDL_FreeSurface(sText);			
	}
};