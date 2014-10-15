/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

#include "stdafx.h"

#include "GlobalVars.h"
#include "GlobalConfig.h"
#include "DisplayConsts.h"
#include "PtGreyImage.h"

#include "AdvrStateContext.h"
#include "AdvrNotReadyState.h"
#include "AdvrShuttingDownState.h"
#include "AdvrRecordingState.h"
#include "AdvrReadyState.h"

#include "SerialHtccLoop.h"
#include "HtccChangingBetweenAutoAndTriggeredState.h"

#include "math.h"
	
	
unsigned char AdvrState::LOW_GAMMA[256];
unsigned char AdvrState::HI_GAMMA[256];
bool AdvrState::GammaArraysInitialized = false;
	
void AdvrState::Initialise(AdvrStateContext *context)
{

}

void AdvrState::Finalise(AdvrStateContext *context)
{
	
}

void AdvrState::SystemInitialised(AdvrStateContext *context)
{
	
}
	
void AdvrState::IncreaseFrameRate(AdvrStateContext *context)
{
	int frRateInt;
	frRateInt = static_cast<int>(context->FrameRate);
	frRateInt--;
	
	if ((context->FrameRate > ExposureAuto60Frames && FRAMERATE_AUTO_60_ENABLED) || 
		(context->FrameRate > ExposureAuto30Frames && FRAMERATE_AUTO_30_ENABLED) ||
		 context->FrameRate > ExposureAuto15Frames)
	{
		CameraFrameRate newFrameRate = static_cast<CameraFrameRate>(frRateInt);
			
		context->FrameRate = s_Camera->ChangeFrameRate(newFrameRate);
		
		/* Exposures above 2 sec are Triggered
		if (newFrameRate == ExposureTriggered2Seconds)
		{
			ChangeBetweenAutoAndTriggeredFrameRate(context, frRateInt);
		}
		else
			context->FrameRate = s_Camera->ChangeFrameRate(newFrameRate);
		*/
	}
}

void AdvrState::DecreaseFrameRate(AdvrStateContext *context)
{
	int frRateInt;
	frRateInt = static_cast<int>(context->FrameRate);
	frRateInt++;
	
	if (context->FrameRate < ExposureTriggered8Seconds) 
	{
		CameraFrameRate newFrameRate = static_cast<CameraFrameRate>(frRateInt);
				
		context->FrameRate = s_Camera->ChangeFrameRate(newFrameRate);
	}
}

void AdvrState::ChangeBetweenAutoAndTriggeredFrameRate(AdvrStateContext *context, int newFrameRate)
{
	// Changing between AutoRunning and Trigger Modes. Need to Sync HTCC & Camera frame numbers again
	context->SetState(AdvrNotReadyState::Instance());

	context->IsReSynchronisingTimestamps = true;
	s_HtccStateMachine->StartingFrameRate = static_cast<CameraFrameRate>(newFrameRate);
	s_HtccStateMachine->SetState(HtccChangingBetweenAutoAndTriggeredState::Instance());

}

bool AdvrState::ReceiveCommand(AdvrStateContext *context, HcCommand* cmd)
{
	if (cmd->UserCommand == UserCommandSystemShutdown)
	{
		context->SetState(AdvrShuttingDownState::Instance());
		return true;
	}
};

int lastFrameExposureId = -1;

bool AdvrState::ReceiveMessage(AdvrStateContext *context, HtccMessage *msg)
{
	if (msg->MessageType == HtccMsgStartExposureTimestamp)
	{
		pthread_rwlock_wrlock (&s_RWRoot);				
		if (s_StartExposureMessages.size() > MAX_IMAGES_IN_BUFFER)
		{
			HtccMessage* rubbish = s_StartExposureMessages.front();
			delete rubbish;
			s_StartExposureMessages.pop();
		}		
		s_StartExposureMessages.push(msg);
		
		pthread_rwlock_unlock(&s_RWRoot);
		
		return true;
	}
	else if (msg->MessageType == HtccMsgEndExposureTimestamp)
	{
		// NOTE: At the time an EndExposure frame is received, the StartExposure message must have arrived already
		// Messages are received in order (by frame no). We need to match the EndExposure message to a start exposure here
		if (s_StartExposureMessages.size() > 0)
		{
			pthread_rwlock_wrlock (&s_RWRoot);

			HtccMessage* startMsg = NULL;
			do
			{
				HtccMessage* startMsg = s_StartExposureMessages.front();
				s_StartExposureMessages.pop();
				if (startMsg->FrameIndex < msg->FrameIndex)
				{
					// Start exposure message without an end exposure message
					delete startMsg;
				}
				else 
				{
					if (startMsg->FrameIndex == msg->FrameIndex)
					{
						if (s_FrameExposureData.size() < MAX_IMAGES_IN_BUFFER)
						{
							//FrameExposureInfo* rubbish = s_FrameExposureData.front();
							//delete rubbish;
							//s_FrameExposureData.pop();
							
							FrameExposureInfo* frameExposure = new FrameExposureInfo(startMsg, msg);
							if (s_AdvrState->FirstCameraFrameTimestampReceived)
							{
								s_FrameExposureData.push(frameExposure);
							}
							else
								s_AdvrState->NumberDroppedHtccTimestamps++;
							
#ifndef HTCC_SIMULATOR							
							if (s_AdvrState->FrameCountingSynchronised)
							{
								lastFrameExposureId = frameExposure->FrameNo;							
							}
#endif
						}
						
						delete startMsg;
					}
				}
			}
			while (s_StartExposureMessages.size() > 0);
					
			pthread_rwlock_unlock(&s_RWRoot);
			
			delete msg;
			
			return true;
		}		
	}
	else if (msg->MessageType == HtccMsgGremlin)
	{
		context->PostHtccGremlinMessage(msg);
	}
	
	return false;
}

void AdvrState::ProcessCurrentFrame(AdvrStateContext *context, PtGreyImage* image, FrameExposureInfo* exposure)
{
	// Only in Recording state we should be 'processing' the frame
}

void AdvrState::DisplayCurrentFrame(AdvrStateContext *context, unsigned short* pImageData)
{
	if (s_HasPixelData)
	{
		// If the pixels are stored internally in BIG ENDIAN mode the we need to reverse them for display purposes
		bool reverseByteOrderForDisplay = !context->IsLittleEndianByteOrder;
		
		unsigned int* doubleBuffer = (unsigned int*)pImageData; //original Hristo
		
		for(int y = 0; y < IMAGE_HEIGHT; y++) 
		{
			for(int x = 0; x < IMAGE_WIDTH / 2; x++) 
			{
				unsigned int pixel32 = *doubleBuffer;
				unsigned short pixel16_1 = pixel32 & 0xFFFF;
				unsigned short pixel16_2 = pixel32 >> 16;
				
				if (reverseByteOrderForDisplay)
				{
					
					pixel16_1 = ((pixel16_1 & 0xFF) << 8) + ((pixel16_1 & 0xFF00) >> 8);
					pixel16_2 = ((pixel16_2 & 0xFF) << 8) + ((pixel16_2 & 0xFF00) >> 8);
				}				
				
				doubleBuffer++;
				
				unsigned char pixel1 = (unsigned char)(pixel16_1 >> 8);
				unsigned char pixel2 = (unsigned char)(pixel16_2 >> 8);

				Uint8 *p = (Uint8 *)screen->pixels + y * screen->pitch + (2 * x) * 3;
				
				DisplayPixels(context, p, pixel1, pixel2);
			};
		};
		
		ShowErrorMessages(context);
		ShowSystemSettingChange(context);
		
		if (!context->IsHtccDetected)
			return;
			
		if (context->IsVideoOnlyMode)
			return;
		
		SDL_Color timeStampColour = CLR_RED;
		
		if (s_GpsNoFixButConfidentTime || !s_GpsAlmanacNotUpdated)
		{
			if (SYS_GPS_LONGITUDE[0] != 0 && SYS_GPS_LATITUDE[0] != 0 && SYS_GPS_ALTITUDE[0] != 0 && SYS_GPS_WSG84[0] != 0)
				timeStampColour = CLR_GREEN;
			else
				// If Geolocation is not known, still show the timestamp in yellow
				timeStampColour = CLR_YELLOW;
		}
		else if (s_GpsAlmanacNotUpdated)
			timeStampColour = CLR_YELLOW;
		
		SDL_Surface* sText = TTF_RenderText_Solid(s_AdvrTimeStampFont, s_TimeStamp, timeStampColour);			
		SDL_BlitSurface(sText, NULL, screen, &RECT_TIMESTAMP);
		SDL_FreeSurface(sText );	
		
		sText = TTF_RenderText_Solid(s_AdvrTimeStampFont, s_GpsSatellites, CLR_YELLOW);			
		SDL_BlitSurface(sText, NULL, screen, &RECT_GPS_SATELLITES);
		SDL_FreeSurface(sText );
		
		if (s_GpsAlmanacNotUpdated)
		{
			sText = TTF_RenderText_Solid(s_AdvrTimeStampFont, "?", CLR_RED);			
			SDL_BlitSurface(sText, NULL, screen, &RECT_GPS_ALMANAC);
			SDL_FreeSurface(sText );			
		}
		else
		{
			sText = TTF_RenderText_Solid(s_AdvrFrameRateFont, s_AlmanacOffset, CLR_YELLOW);			
			SDL_BlitSurface(sText, NULL, screen, &RECT_GPS_ALMANAC_OFFSET);
			SDL_FreeSurface(sText );				
		}
		
		if (!context->FrameCountingSynchronised)
			return;
			
		char framePos[10];
		snprintf(framePos, 10, "% 8d", (int)(s_CurrentDisplayImageFrameId % 10000000));
		sText = TTF_RenderText_Solid(s_AdvrFrameRateFont, framePos, CLR_WHITE);			
		SDL_BlitSurface(sText, NULL, screen, &RECT_IMAGE_FRAME_NO);
		SDL_FreeSurface(sText );
		
		if (context->NumberDroppedFrames > 0)
		{
			snprintf(framePos, 10, "% 8d*", (int)(context->NumberDroppedFrames));
			sText = TTF_RenderText_Solid(s_AdvrFrameRateFont, framePos, CLR_RED);			
			SDL_BlitSurface(sText, NULL, screen, &RECT_DROPPED_FRAMES);
			SDL_FreeSurface(sText );
		}
		
		if (context->NumberCameraDroppedFrames > 0)
		{
			snprintf(framePos, 10, "% 8d*", (int)(context->NumberCameraDroppedFrames));
			sText = TTF_RenderText_Solid(s_AdvrFrameRateFont, framePos, CLR_ORANGE);			
			SDL_BlitSurface(sText, NULL, screen, &RECT_CAMEERA_DROPPED_FRAMES);
			SDL_FreeSurface(sText );			
		}
		
		if (context->NumberDroppedHtccTimestamps > 0)
		{
			snprintf(framePos, 10, "% 8d*", (int)(context->NumberDroppedHtccTimestamps));
			sText = TTF_RenderText_Solid(s_AdvrFrameRateFont, framePos, CLR_YELLOW);			
			SDL_BlitSurface(sText, NULL, screen, &RECT_HTCC_DROPPED_FRAMES);
			SDL_FreeSurface(sText );			
		}
		
		if (DISPLAY_GEOLOCATION && 
			SYS_GPS_LONGITUDE[0] != 0 && SYS_GPS_LATITUDE[0] != 0 && SYS_GPS_ALTITUDE[0] != 0)
		{
			char geoLocation[50];
			
			snprintf(geoLocation, 50, "%s %3d\xB0%2d\'%2d\", %s %2d\xB0%2d\'%2d\", %0.1f m", 
				&SYS_GPS_LONGITUDE_DIRECTION[0],
				(int)SYS_GPS_LONGITUDE_FLOAT,
				(int)((SYS_GPS_LONGITUDE_FLOAT - (int)SYS_GPS_LONGITUDE_FLOAT) * 60),
				(int)(((SYS_GPS_LONGITUDE_FLOAT - (int)SYS_GPS_LONGITUDE_FLOAT) * 60 - (int)((SYS_GPS_LONGITUDE_FLOAT - (int)SYS_GPS_LONGITUDE_FLOAT) * 60)) * 60),
				&SYS_GPS_LATITUDE_DIRECTION[0],
				(int)SYS_GPS_LATITUDE_FLOAT,
				(int)((SYS_GPS_LATITUDE_FLOAT - (int)SYS_GPS_LATITUDE_FLOAT) * 60),
				(int)(((SYS_GPS_LATITUDE_FLOAT - (int)SYS_GPS_LATITUDE_FLOAT) * 60 - (int)((SYS_GPS_LATITUDE_FLOAT - (int)SYS_GPS_LATITUDE_FLOAT) * 60)) * 60),
				SYS_GPS_ALTITUDE_FLOAT);

			SDL_Surface* sText = TTF_RenderText_Solid(s_AdvrFont, geoLocation, CLR_YELLOW);	
			SDL_BlitSurface(sText, NULL, screen, &RECT_GEOLOCATION_LN);
			SDL_FreeSurface(sText);	
		}
		
		
		if (context->HelpScreenDisplayed)
			ShowHelpInfo(context);
		
		ShowDebugInfo(context);
		
	}
	else
	{
		// TODO: Show the splash screen as bitmap
		SDL_FillRect(screen, &RECT_FULL_SCREEN, 0);
	}
};

void AdvrState::DisplayPixels(AdvrStateContext *context, Uint8 *p, unsigned char pixel1, unsigned char pixel2)
{
	if (!GammaArraysInitialized)
	{
		double loGammaFactor = 255 / pow(255, 0.45);
		double hiGammaFactor = 255 / pow(255, 0.35);
		
		for(int i = 0; i< 256; i++)
		{
			double loGamma = loGammaFactor * pow(i, 0.45);
			double hiGamma = hiGammaFactor * pow(i, 0.35);
			
			if (loGamma < 0)
				LOW_GAMMA[i] = 0;
			else if (loGamma > 255)
				LOW_GAMMA[i] = 255;
			else
				LOW_GAMMA[i] = (int)loGamma;
				
			if (hiGamma < 0)
				HI_GAMMA[i] = 0;
			else if (hiGamma > 255)
				HI_GAMMA[i] = 255;
			else
				HI_GAMMA[i] = (int)hiGamma;			
		}
		
		GammaArraysInitialized = true;
	}
	
	Uint8 B1, G1, R1, B2, G2, R2;
	
	switch(context->EnhancedDisplayMode % 4)
	{
		case 0:
			// Linear no saturation flags
			G1 = R1 = B1 = pixel1;
			G2 = R2 = B2 = pixel2;
			break;
		
		case 1:
			// Linear with saturation flags
			if (pixel1 < 168)
			{
				G1 = R1 = B1 = pixel1;
			}
			else if (pixel1 < 250)
			{
				G1 = 0x00;
				B1 = 0x00;
				R1 = 0x33;
			}
			else
			{
				G1 = 0x00;
				B1 = 0x00;
				R1 = 0xCC;	
			}
			
			if (pixel2 < 168)
			{
				G2 = B2 = R2 = pixel2;
			}
			else if (pixel1 < 250)
			{
				G2 = 0x00;
				B2 = 0x00;
				R2 = 0x33;
			}
			else
			{
				G2 = 0x00;
				B2 = 0x00;
				R2 = 0xCC;	
			}			
			break;
			
		case 2:
			// Low gamma
			G1 = R1 = B1 = LOW_GAMMA[pixel1];
			G2 = R2 = B2 = LOW_GAMMA[pixel2];
			break;
			
		case 3:
			// Hi gamma
			G1 = R1 = B1 = HI_GAMMA[pixel1];
			G2 = R2 = B2 = HI_GAMMA[pixel2];		
			break;
	}
	
	if (context->InvertedDisplay)
	{
		B1 = 255 - B1;
		G1 = 255 - G1;
		R1 = 255 - R1;
		B2 = 255 - B2;
		G2 = 255 - G2;
		R2 = 255 - R2;
	}
	
	p[0] = B1;
	p[1] = G1;
	p[2] = R1;	
	p[3] = B2;
	p[4] = G2;
	p[5] = R2;
}

void AdvrState::DisplayColourFrame(unsigned char* pImageData)
{
	unsigned char* buffer = (unsigned char*)pImageData;
	
	for(int x = 0; x < 640; x++) 
	{
		for(int y = 0; y < 480; y++) 
		{
			Uint8 *p = (Uint8 *)screen->pixels + y * screen->pitch + x * 3;
		
			p[2] = (unsigned char)(*buffer);
			buffer++;
			p[1] = (unsigned char)(*buffer);
			buffer++;
			p[0] = (unsigned char)(*buffer);
			buffer++;
		};
	};	
}

