#include "stdafx.h"
#include <stdlib.h>
#include "HtccMessage.h"
#include "SerialHtccLoop.h"
#include "HtccWaitingForFirstFrameState.h"
#include "HtccInitialisedState.h"
#include "GlobalVars.h"


HtccWaitingForFirstFrameState* HtccWaitingForFirstFrameState::s_pInstance = NULL;

HtccWaitingForFirstFrameState* HtccWaitingForFirstFrameState::Instance()
{
	if (!s_pInstance)
		s_pInstance = new HtccWaitingForFirstFrameState();
	
	return s_pInstance;
};

void HtccWaitingForFirstFrameState::Initialise(HtccStateMachine *context)
{
#ifdef HTCC_DETAILED_LOG
	printf("HTCC: Transitioned to HtccWaitingForFirstFrameState\n");
#endif
		
	s_Camera->CurrentAdvsFrameRate == ExposureNotSet;
	m_SecondFrameStartTimestamp = NULL;
	
	pthread_rwlock_wrlock (&s_RWRoot);
	pthread_rwlock_wrlock (&s_SyncCameraData);

	if (s_StartExposureMessages.size() > 0)
		printf("Clearing Buffers: Removing %d HTCC start exposure messages from the queue.\n", s_StartExposureMessages.size());

	while(s_StartExposureMessages.size() > 0) s_StartExposureMessages.pop();

	if (s_FrameExposureData.size() > 0)
		printf("Clearing Buffers: Removing %d timestamps from the queue.\n", s_FrameExposureData.size());
		
	while(s_FrameExposureData.size() > 0) s_FrameExposureData.pop();
	
	if (s_CameraImages.size() > 0)
		printf("Clearing Buffers: Removing %d images from the queue.\n", s_CameraImages.size());
	
	while(s_CameraImages.size() > 0) s_CameraImages.pop();
	
	pthread_rwlock_unlock(&s_SyncCameraData);
	pthread_rwlock_unlock(&s_RWRoot);
		
	m_FirstFrameArrived = USE_SOFTWARE_TRIGGER ? true : false;
	
	m_SecondFrameStartTimestamp = NULL;
	
	//NOTE: ChangeFrameRate() will also call SetTriggerMode()
    s_AdvrState->FrameRate = s_Camera->ChangeFrameRate(context->StartingFrameRate);
	
	if (m_FirstFrameArrived)
		printf("HTCC: Waiting for timed sync frame (first frame)... \n");
	else
		printf("HTCC: Waiting for timed sync frame (second frame)... \n");
};

void HtccWaitingForFirstFrameState::Finalise(HtccStateMachine *context)
{ };




HtccMessage* HtccWaitingForFirstFrameState::ReceivePacket(HtccStateMachine *context, unsigned char* packet)
{
	if (packet[0] == 0xFE && packet[1] == 's')
	{
		#ifdef HTCC_DETAILED_LOG
		startTimestamp = new HtccMessage(packet);
		printf("HTCC: Waiting state image (%ld) start ... \n", startTimestamp->FrameIndex);
		#endif
		
		if (!m_FirstFrameArrived)
		{
			m_FirstFrameArrived = true;		
			#ifdef HTCC_DETAILED_LOG
			printf("HTCC: m_FirstFrameArrived = true\n");
			#endif			
		}
		else
		{
			// Exposure has started
			if (NULL == m_SecondFrameStartTimestamp)
			{
				m_SecondFrameStartTimestamp = new HtccMessage(packet);
				//printf("FIRST-START: %d.%d\n", m_SecondFrameStartTimestamp->TimestampUtcSecond, m_SecondFrameStartTimestamp->TimestampUtcFractionalSecond10000);
				s_AdvrState->ExpectFirstCameraFrame = true;	
				#ifdef HTCC_DETAILED_LOG
				printf("HTCC: Sync image (%d) start timestamp has arrived ... \n", USE_SOFTWARE_TRIGGER ? 1 : 2);
				#endif					
			}
		}
	}
	else if (packet[0] == 0xFE && packet[1] == 'e')
	{
		HtccMessage* endTimestamp = new HtccMessage(packet);

		#ifdef HTCC_DETAILED_LOG		
		long long start10thMs = DateTimeToAdvTicks(startTimestamp->TimestampUtcYear, startTimestamp->TimestampUtcMonth, startTimestamp->TimestampUtcDay, startTimestamp->TimestampUtcHours, startTimestamp->TimestampUtcMinutes, startTimestamp->TimestampUtcSecond, startTimestamp->TimestampUtcFractionalSecond10000);
		long long end10thMs = DateTimeToAdvTicks(endTimestamp->TimestampUtcYear, endTimestamp->TimestampUtcMonth, endTimestamp->TimestampUtcDay, endTimestamp->TimestampUtcHours, endTimestamp->TimestampUtcMinutes, endTimestamp->TimestampUtcSecond, endTimestamp->TimestampUtcFractionalSecond10000);
		
		int exposureInMilliseconds = (int)((end10thMs - start10thMs) / 10);
		printf("HTCC: Waiting state image (%ld) end. Exposure is %d ms.\n", endTimestamp->FrameIndex, exposureInMilliseconds);
		#endif
		
		if (NULL != m_SecondFrameStartTimestamp &&
		    endTimestamp->FrameIndex == m_SecondFrameStartTimestamp->FrameIndex)
		{
			// NOTE: We are using the end of the second frame as a sync point as the reported frame exposure by Htcc for the first frame looks odd (only few ms)
			
			#ifdef HTCC_DETAILED_LOG
			printf("HTCC: Sync image (%d) end timestamp has arrived ... \n", USE_SOFTWARE_TRIGGER ? 1 : 2);
			#endif
			
			//printf("FIRST-END: %d.%d\n", endTimestamp->TimestampUtcSecond, endTimestamp->TimestampUtcFractionalSecond10000);
		
			long timestampFrameIndex = m_SecondFrameStartTimestamp->FrameIndex;
				
			long long advTicksEnd = DateTimeToAdvTicks(
				endTimestamp->TimestampUtcYear, 
				endTimestamp->TimestampUtcMonth, 
				endTimestamp->TimestampUtcDay, 
				endTimestamp->TimestampUtcHours, 
				endTimestamp->TimestampUtcMinutes, 
				endTimestamp->TimestampUtcSecond, 
				endTimestamp->TimestampUtcFractionalSecond10000);				
			
			printf("HTCC: Received timestamp (%d) for sync frame ... \n", (int)timestampFrameIndex);
			s_AdvrState->SetSynchronisationFrameHtccId(timestampFrameIndex, advTicksEnd);
					
			// Push the frame exposure so the first frame can be timed by the DataProcessingLoop
			FrameExposureInfo* frameExposure = new FrameExposureInfo(m_SecondFrameStartTimestamp, endTimestamp);
			s_FrameExposureData.push(frameExposure);
			
			context->SetState(HtccInitialisedState::Instance());			
		}
   
		return NULL;
	}
	
	return NULL;
};

bool HtccWaitingForFirstFrameState::ProcessOneSecondTick(HtccStateMachine *context)
{ };