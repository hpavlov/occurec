/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

#include "stdafx.h"
#include <stdio.h>

#include "CameraLoop.h"
#include "GlobalVars.h"
#include "GlobalConfig.h"
#include "PtGreyImage.h"
#include "PtGreyCamera.h"


void* DoCameraLoop(void* arg)
{
	Uint32 oldTicks = SDL_GetTicks();
	int oldCounter = 0;
	

	s_Camera = new PtGreyCamera();	
	
#ifdef CAMERA_SIMULATOR
	s_AdvrState->IsCameraDetected = true;
	Simulator::LoadFakeFrames();
	s_AdvrState->CameraConnected();
#endif
		
	pthread_cond_t fakeCond = PTHREAD_COND_INITIALIZER;
	pthread_mutex_t fakeMutex = PTHREAD_MUTEX_INITIALIZER;
	struct timespec timeToWait;
	
	int counter = 0;
	int receivedFramesThisInterval = 0;
	FlyCapture2::ImageMetadata metadata;
	FlyCapture2::TimeStamp timestamp;
	bool runCameraCheckThisTime = false;
	
	while (s_Running)
	{
		counter++;
		
		// Profiling Code
		Uint32 newTicks = SDL_GetTicks();
		if (newTicks - oldTicks > 1000)
		{			
			s_AdvrState->CameraLoopsPerSecond = (counter - oldCounter) * 1000.0 / (newTicks - oldTicks);
			s_AdvrState->CameraFramesPerSecond = receivedFramesThisInterval / (newTicks - oldTicks);
			oldTicks = newTicks;
			oldCounter = counter;
			receivedFramesThisInterval = 0;
			runCameraCheckThisTime = true;
		};
		
#ifndef CAMERA_SIMULATOR
		if (runCameraCheckThisTime && !s_Camera->IsInTransition)
		{
			pthread_rwlock_rdlock(&s_SyncCameraReset);
			
			if (!s_Camera->IsInTransition)
			{
				if (!s_Camera->IsCameraAvailable) 
				{
					s_Camera->CheckCamera();
					if (!s_Camera->IsCameraAvailable)
					{
						printf("Camera: No camera available \n");
						s_AdvrState->NoCameraAvailable();
					}
				}
				if (!s_Camera->IsConnected) 
				{
					s_Camera->Connect();
					if (s_Camera->IsConnected)
					{
						printf("Camera: Camera connected \n");
						s_AdvrState->CameraConnected();				
					}
					else
					{
						printf("Camera: Error connecting \n");
						s_AdvrState->NoCameraConnected();
					}
				}
				if (!s_Camera->IsCapturing)
				{
					// NOTE: It is required by both external triggered and free running modes that the camera is put in capturing state
					printf("CameraLoop ln91 - cam startCapture\n");
					s_Camera->StartCapture();			
				}
			}

			pthread_rwlock_unlock(&s_SyncCameraReset);		
		}
#endif		
		if (s_Camera->IsInTransition)
			continue;

#ifndef CAMERA_SIMULATOR
		// Get the next camera image. This is a blocking call
		unsigned short* frame = s_Camera->RetrieveFrameBlocking(&metadata, &timestamp);
		
#else
		// In simulated mode just get the next mocked up image
		unsigned short* frame = Simulator::ReceiveCameraImage(counter, &metadata);
#endif

		if (s_Camera->IsInTransition)
			continue;

		pthread_rwlock_wrlock (&s_SyncCameraData);
		
		s_AdvrState->NumberQueuedFrames++;
		
		if (s_CameraImages.size() > MAX_IMAGES_IN_BUFFER)
		{
			PtGreyImage* rubbish = s_CameraImages.front();
			delete rubbish;
			s_CameraImages.pop();			
			s_AdvrState->NumberDroppedFrames++;
		}
		
		PtGreyImage* img;
		try
		{
			// NOTE: Sometimes this crashes! Anything wrong with the frame bytes ??
			img = new PtGreyImage(frame, &metadata, &timestamp, 16);
		}
		catch(...)
		{
			pthread_rwlock_unlock (&s_SyncCameraData);
			s_AdvrState->NumberDroppedFrames++;
			continue;
		}
			
		if (s_AdvrState->FrameCountingSynchronised &&
			img->FrameId == s_AdvrState->LastQueuedFrameId)
		{
			// NOTE: When transitioning from and to Mode7_Format7 the camera may resend the same image id as already sent. Ignore those images
			continue;
		}
			
		s_CameraImages.push(img);
 
#ifdef HTCC_SIMULATOR				
		Simulator::QueueStartEndFrameExposureHtccPackets(img->FrameId);
#endif
		
		receivedFramesThisInterval++;
										
		if (s_AdvrState->FrameCountingSynchronised &&
		    s_AdvrState->LastQueuedFrameId > 1 && 
			s_AdvrState->LastQueuedFrameId != img->FrameId - 1)
		{
			if (!s_AdvrState->ExpectFirstCameraFrame &&
				!s_AdvrState->ExpectDroppedCameraImages)
			{
				printf("Camera: ERROR Expected frame %d but received frame %d\n", (int)s_AdvrState->LastQueuedFrameId + 1, img->FrameId);
				s_AdvrState->NumberCameraDroppedFrames++;
			}
		}
		
		if (s_AdvrState->ExpectFirstCameraFrame)
		{
			printf("Camera: Received sync frame (%d) ... \n", img->FrameId);
			s_AdvrState->SetSynchronisationFrameImageId(img->FrameId, img->CameraTicks);
			s_AdvrState->ExpectFirstCameraFrame = false;				
		}		

		s_AdvrState->LastQueuedFrameId = img->FrameId;
		pthread_rwlock_unlock (&s_SyncCameraData);
		
		
		
#ifdef CAMERA_SIMULATOR		
		// Wait to simulate camera frame rate
		Simulator::GetTimeToWaitForFrameRate(&timeToWait, s_AdvrState->FrameRate);		
		pthread_cond_timedwait(&fakeCond, &fakeMutex, &timeToWait);
#endif
		
	}  //this is the end of the while s_running
	
#ifndef CAMERA_SIMULATOR
	s_Camera->Disconnect();
#endif

	printf ("Disconnected from Camera\n");
	pthread_exit((void*) 0);	
}