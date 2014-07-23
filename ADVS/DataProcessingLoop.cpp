#include "stdafx.h"

#include "DataProcessingLoop.h"
#include "HtccMessage.h"
#include "GlobalVars.h"
#include "PtGreyImage.h"

void* DoDataProcessingLoop(void* arg)
{
	bool enabled = false;
	
	int counter = 0;
	Uint32 oldTicks = SDL_GetTicks();
	int oldCounter = 0;
	s_AdvrState->NumberProcessedFrames = 0;
	AdvProfiling_ResetPerformanceCounters();
	
	printf ("Data Processing: Start\n");
	
	HtccMessage DUMMY_HTCC_MESSAGE;
		
	while (s_Running)
	{
		counter++;
		
		// Profiling Code
		Uint32 newTicks = SDL_GetTicks();
		if (newTicks - oldTicks > 1000)
		{			
			s_AdvrState->ProcessingLoopsPerSecond = (counter - oldCounter) * 1000.0 / (newTicks - oldTicks);
			oldTicks = newTicks;
			oldCounter = counter;
		}
		
		if (s_CameraImages.size() > 0)
		{
			pthread_rwlock_wrlock(&s_SyncCameraData);
			PtGreyImage* currentImage = s_CameraImages.front();			
			s_CameraImages.pop();			
			s_AdvrState->NumberProcessedFrames++;
			pthread_rwlock_unlock(&s_SyncCameraData);
						
			pthread_rwlock_wrlock(&s_RWRoot);
			s_AdvrState->CurrentlyProcessedFrameId = currentImage->FrameId;
			pthread_rwlock_unlock(&s_RWRoot);
			
			FrameExposureInfo* frameExposure;
			frameExposure = NULL;
			bool continueLooking = true;
			
			ptime t1 = microsec_clock::local_time();
			float millisecondsWaiting = 0;
			
			if (s_AdvrState->FrameCountingSynchronised)
			{
				do
				{
					// Wait a configured interval of time to receive the Htcc Start/End messages and then drop the frame
					if (s_FrameExposureData.size() > 0)
					{
						pthread_rwlock_rdlock(&s_RWRoot);
						frameExposure = s_FrameExposureData.front();			
						pthread_rwlock_unlock(&s_RWRoot);
						
						long long htccTimeStampAdvTicks;
						unsigned int exposureIn10thMilliseconds = 0;	
						frameExposure->GetAdvEndExposureTimeStamp(&htccTimeStampAdvTicks, &exposureIn10thMilliseconds, false);
						
						int compResult = s_AdvrState->CompareFrameIdToTimeStampId(currentImage->FrameId, currentImage->CameraTicks, frameExposure->FrameNo, htccTimeStampAdvTicks, exposureIn10thMilliseconds, currentImage->ShutterSeconds, frameExposure);
						printf("CompResult = %d \n", compResult);
						
						if (compResult < 0)
						{
							// FrameId is smaller - drop the frame and continue
							frameExposure = new FrameExposureInfo(&DUMMY_HTCC_MESSAGE, &DUMMY_HTCC_MESSAGE);
							continueLooking = false;
							
							if (!s_AdvrState->ExpectDroppedCameraImages)
							{
								// V2 TODO: Instead of dropping the image - display/record the image with an INVALID timestamp
								printf("Data Processing: Camera image %d will be recorded with a blank timestamp.\n", currentImage->FrameId);
								s_AdvrState->NumberCameraDroppedFrames++;
							}
						}
						else if (compResult > 0)
						{
							// Timestamp is smaller - drop the timestamp and check for a new timestamp
							pthread_rwlock_wrlock(&s_RWRoot);
							s_FrameExposureData.pop();
							
							if (!s_AdvrState->ExpectDroppedHtccTimeStamps)
							{
								printf("Data Processing: Dropping Htcc timestamp %d. Couldn't match to camera image.\n", frameExposure->FrameNo);
								s_AdvrState->NumberDroppedHtccTimestamps++;
							}
							
							pthread_rwlock_unlock(&s_RWRoot);
							
							delete frameExposure;					
							frameExposure = NULL;
							
							continueLooking = true;
						}
						else if (compResult == 0)
						{
							// Found the timestamp for the frame. Remove it from the queue.
							pthread_rwlock_wrlock(&s_RWRoot);
							s_FrameExposureData.pop();
							pthread_rwlock_unlock(&s_RWRoot);
							
							if (s_AdvrState->ExpectDroppedCameraImages)
								s_AdvrState->ExpectDroppedCameraImages = false;
								
							if (s_AdvrState->ExpectDroppedHtccTimeStamps)
								s_AdvrState->ExpectDroppedHtccTimeStamps = false;
								
							printf("Data Processing: Matched Frame (%d) to Timestamp (%d)\n", currentImage->FrameId, frameExposure->FrameNo);				
							continueLooking = false;
						}
					}
					
					ptime t2 = microsec_clock::local_time();
					time_period tp(t1, t2);
					millisecondsWaiting = tp.length().total_microseconds() / 1000.0;
				}
				
				while(continueLooking && millisecondsWaiting < MILLISECONDS_WAIT_TO_RECEIVE_TIMESTAMP_FOR_CAMERA_IMAGE);
			}
		
			// Make a copy for the display thread to show it on the screen
			EnterCriticalSection(&s_SyncDisplayBytes);
			if (NULL != s_PixelInts)
				
				//make a copy for display - pixelInts is unsigned short, currentImage is unsigned short
				memcpy(&s_PixelInts[0], currentImage->ImageData, IMAGE_STRIDE * sizeof(unsigned short));
				
			s_CurrentDisplayImageFrameId = currentImage->FrameId;
			if (frameExposure != NULL)
			{
				if (frameExposure->EndExposure != NULL)
				{
					s_GpsAlmanacNotUpdated = frameExposure->EndExposure->AlmanacStatus == 0; // 0=uncertain, 1=good
					s_GpsNoFix = frameExposure->EndExposure->GPSFixStatus == 0; // 00 = no fix, 01 = no fix (confident), 10 = G fix, 11 = P fix
					s_GpsNoFixButConfidentTime = frameExposure->EndExposure->GPSFixStatus == 1; // 00 = no fix, 01 = no fix (confident), 10 = G fix, 11 = P fix
					
					snprintf(s_GpsSatellites, 2, "%d", frameExposure->EndExposure->TrackedSatellites);	
					if (frameExposure->StartExposure->AlmanacOffset != 0) 
						snprintf(s_AlmanacOffset, 4, "%s%d", frameExposure->StartExposure->AlmanacOffset < 0 ? "-" : "+", abs(frameExposure->StartExposure->AlmanacOffset));
					
					else 
						snprintf(s_AlmanacOffset, 4, "%s", "");
					
					
					if (frameExposure->EndExposure->UtcDays != 0)
					{
						long long timeStamp;
						unsigned int exposureIn10thMilliseconds = 0;
						
						frameExposure->GetAdvEndExposureTimeStamp(&timeStamp, &exposureIn10thMilliseconds, true);
						
						int year;
						int month;
						int day;
						int hours;
						int mins;
						int secs;
						int fracSecs;
						
						AdvTicksToDateTime(10 * timeStamp, &year, &month, &day, &hours, &mins, &secs, &fracSecs);

						if (DISPLAY_DATE_PART_OF_DATETIME)
						{
							snprintf(s_TimeStamp, 30, "%04d/%02d/%02d %02d:%02d:%02d:%03d", 						
									year,
									month,
									day,
									hours, 
									mins, 
									secs, 
									fracSecs / 10);									
						}
						else
						{
							snprintf(s_TimeStamp, 30, "     %02d:%02d:%02d:%03d", 						
									hours, 
									mins, 
									secs, 
									fracSecs / 10);		
						}
				
					}
					else
					{
						snprintf(s_TimeStamp, 30, "NO TIME-STAMP AVAILABLE");
					}
					
				}
			}

			s_HasPixelData = true;	
			LeaveCriticalSection(&s_SyncDisplayBytes);
			
			s_AdvrState->ProcessCurrentFrame(currentImage, frameExposure);
			
			delete currentImage;
			if (frameExposure != NULL) 
			{
				//delete frameExposure;
				frameExposure = NULL;
			}
		}
	}
	
	printf ("Data Processing: Stop\n");
	
#ifndef CAMERA_SIMULATOR
	s_Camera->Disconnect();
#endif	
	
	pthread_exit(NULL);

}