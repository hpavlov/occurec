/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

#include "stdafx.h"

#include <stdio.h>   /* Standard input/output definitions */
#include <string.h>  /* String function definitions */
#include <fcntl.h>   /* File control definitions */
#include <errno.h>   /* Error number definitions */

#include "SerialHtccLoop.h"
#include "GlobalVars.h"
#include "HtccMessage.h"
#include "HtccState.h"

int open_port(void);
void setup_port(int fd);

std::queue<unsigned char*> queuedHtccPackets;
HANDLE FILE_HTCC_PORT;
HANDLE OpenAndConfigureHttcPort();
BOOL fWaitingOnRead = FALSE;
OVERLAPPED osReader = {0};

void* DoSerialHtccLoop(void* arg)
{	
	int counter = 0;
	DWORD oldTicks = GetTickCount();
	int oldCounter = 0;
	FILE_HTCC_PORT = INVALID_HANDLE_VALUE;

#ifndef HTCC_SIMULATOR
	FILE_HTCC_PORT = OpenAndConfigureHttcPort();
	
	s_HtccStateMachine = new HtccStateMachine();
	s_HtccStateMachine->Initialise(CAMERA_FRAME_RATE_ON_STARTUP);
#else
	// In simulated mode we set the calibration frame Id and pretend we are connected
	s_AdvrState->LastQueuedFrameId = 0;
	s_AdvrState->SetSynchronisationFrameHtccId(0, 0);
	s_AdvrState->HtccConnected();
#endif
	
	while (s_Running)
	{
		counter++;

#ifdef HTCC_SIMULATOR		
		unsigned char* packet = (unsigned char*)Simulator::DeQueueHtccPacket();
#else		
		ReadHtccPacketBytesIfAvailable();
		
		unsigned char* packet = DeQueueHtccPacket();
#endif
		
		if (NULL != packet)
		{
			
#ifndef HTCC_SIMULATOR	

			#ifdef HTCC_DETAILED_LOG
				HtccMessage* msg = new HtccMessage(packet);
				struct timeval tv;
				gettimeofday(&tv,NULL);
				unsigned long time_in_micros = 1000000 * tv.tv_sec + tv.tv_usec;
				printf("HTCC: '%s':Frame %d, Time %lu, %s\n", &msg->PacketType[0], (int)msg->FrameIndex, time_in_micros, msg->RawBytes);
				delete msg;
			#endif	
				
			HtccMessage* message = s_HtccStateMachine->ReceivePacket(packet);
#else
			HtccMessage* message = new HtccMessage(packet);
#endif

			delete packet;
			
			if (message != NULL && message->MessageType != HtccMsgInvalid)
			{
				EnterCriticalSection(&s_SyncRoot);
				
				bool messageProcessed = s_AdvrState->ReceiveMessage(message);
				s_AdvrState->NumberProcessedHtccMessages++;
				
				if (!messageProcessed)
					delete message;
				
				LeaveCriticalSection(&s_SyncRoot);				
			}
		}
		
		
		DWORD newTicks = GetTickCount();
				
		// Profiling Code
		if (newTicks - oldTicks > 1000)
		{			
			//s_AdvrState->HtccLoopsPerSecond = (counter - oldCounter) * 1000.0 / (newTicks - oldTicks);
			oldTicks = newTicks;
			oldCounter = counter;

#ifndef HTCC_SIMULATOR			
			s_HtccStateMachine->ProcessOneSecondTick();
#endif
		}
	}
	
	if (FILE_HTCC_PORT != INVALID_HANDLE_VALUE)
		CloseHandle(FILE_HTCC_PORT);

	if (osReader.hEvent != NULL)
		CloseHandle(osReader.hEvent);
	
	printf ("Disconnected from HTCC\n");
	_endthread();
}


void SendHtccCommand(HtccCommandType cmd)
{
	char commandChar;
	
	switch(cmd)
	{
		case HtccCmdGetVersion:
			commandChar = 'V';
			break;
			
		case HtccCmdStartOneSecTriggeredExposures:
			commandChar = '1';
			break;
			
		case HtccCmdStartTwoSecTriggeredExposures:
			commandChar = '2';
			break;
			
		case HtccCmdStartFourSecTriggeredExposures:
			commandChar = '4';
			break;
			
		case HtccCmdStartEightSecTriggeredExposures:
			commandChar = '8';
			break;
			
		case HtccCmdStopTriggeredExposures:
			commandChar = 'T';
			break;
			
		case HtccCmdZeroFrameCounter:
			commandChar = 'Z';
			break;
			
		case HtccCmdResetErrorCounter:
			commandChar = 'R';
			break;
			
		case HtccCmdGetGpsPosition:
			commandChar = 'P';
			break;
			
		case HtccCmdGetGpsHeight:
			commandChar = 'H';
			break;
			
		case HtccCmdGetGpsAlmanachUpdateTime:
			commandChar = 'A';
			break;
			
		case HtccCmdStartDateStamps:
			commandChar = 'D';
			break;
			
		case HtccCmdTriggerSingleImageExposure:
			commandChar = 'I';
			break;
			
		default:
			commandChar = '\0';
			break;
	}
		
	// Write data
	if (FILE_HTCC_PORT != INVALID_HANDLE_VALUE)
	{
		DWORD byteswritten;
		size_t n = WriteFile(FILE_HTCC_PORT, &commandChar, 1, &byteswritten, NULL);

		#ifdef HTCC_DETAILED_LOG
		struct timeval tv;
		gettimeofday(&tv,NULL);
		unsigned long time_in_micros = 1000000 * tv.tv_sec + tv.tv_usec;
		char commandString[2];
		commandString[0] = commandChar;
		commandString[1] = '\0';
		
		printf("HTCC: Command Sent - '%s' (Time:%lu)\n", commandString, time_in_micros);
		#endif
		
		//commandChar = '\n';
		//n = write(FILE_HTCC_PORT, &commandChar, 1);
	}
}

unsigned char* DeQueueHtccPacket()
{
	if (queuedHtccPackets.empty())
		return NULL;
	
	EnterCriticalSection(&s_SyncRoot);
	
	unsigned char* pckt = queuedHtccPackets.front();
	queuedHtccPackets.pop();
	
	LeaveCriticalSection(&s_SyncRoot);
	
	return pckt;
}

unsigned char largePortBuffer[LARGE_PORT_BUFFER_SIZE];
unsigned char currentHtccPacket[HTCC_PACKET_SIZE];
int currentHtccPacketPos = 0;
bool currentPacketStartByteIdentified = false;

void ReadHtccPacketBytesIfAvailable()
{
	if (FILE_HTCC_PORT != INVALID_HANDLE_VALUE)
	{
		DWORD nbytes;

		// MSDN Serial Communications:
		// http://msdn.microsoft.com/en-us/library/ff802693.aspx

		if (!fWaitingOnRead) {
		   // Issue read operation.
		   if (!ReadFile(FILE_HTCC_PORT, &largePortBuffer[0], LARGE_PORT_BUFFER_SIZE, &nbytes, &osReader)) 
		   {
			  if (GetLastError() != ERROR_IO_PENDING)     // read not delayed?
				 // Error in communications; report it.
				 perror("ReadFile: Unable to read from HTCC serial port");
			  else
				 fWaitingOnRead = TRUE;
		   }
		   else 
		   {    
				// read completed immediately

				//#ifdef HTCC_DETAILED_LOG
				//printf("HTCC: %d|%d bytes in COMM buffer: ", bytes, nbytes);
				//for(int kk = 0; kk < nbytes; kk++) printf("%02X ", largePortBuffer[kk]);
				//printf("\n");
				//#endif
				
				int idx = 0;
				while (idx < nbytes)
				{
					if (!currentPacketStartByteIdentified)
					{
						currentPacketStartByteIdentified = idx >=0 && idx < LARGE_PORT_BUFFER_SIZE && largePortBuffer[idx] == HTCC_START_PACKET_BYTE;
						if (currentPacketStartByteIdentified)
						{
							currentHtccPacket[0] = largePortBuffer[idx];
							currentHtccPacketPos = 1;
						} 
					}
					else
					{
						currentHtccPacket[currentHtccPacketPos] = largePortBuffer[idx];
						currentHtccPacketPos++;
						
						if (currentHtccPacketPos == HTCC_PACKET_SIZE)
						{							
							unsigned char* packet = (unsigned char*)malloc(HTCC_PACKET_SIZE);
							memcpy(packet, currentHtccPacket, HTCC_PACKET_SIZE);
							
							EnterCriticalSection(&s_SyncRoot);
							queuedHtccPackets.push(packet);
							s_AdvrState->NumberReceivedHtccMessages++;
							LeaveCriticalSection(&s_SyncRoot);
							
							currentPacketStartByteIdentified = false;
						}
					}					
					
					idx ++;
				}	
			}
		}
	}
}

HANDLE OpenAndConfigureHttcPort()
{
  HANDLE hPort;  // File descriptor for the port
  DCB dcb;

  hPort = CreateFile(&CFG_HTCC_DEVICE[0], GENERIC_READ | GENERIC_WRITE, 0, NULL, OPEN_EXISTING, FILE_FLAG_OVERLAPPED, NULL);
  if (hPort == INVALID_HANDLE_VALUE)
  {
	perror("open_port: Unable to open HTCC serial port - ");
	return INVALID_HANDLE_VALUE;
  }
  else
  {	    
	if (!GetCommState(hPort, &dcb))
		return INVALID_HANDLE_VALUE;
	
	dcb.BaudRate = CBR_115200; //9600 Baud
	dcb.ByteSize = 8; //8 data bits
	dcb.Parity = NOPARITY; //no parity
	dcb.StopBits = ONESTOPBIT; //1 stop

	if (!SetCommState(hPort,&dcb))
		return INVALID_HANDLE_VALUE;

	// Create the overlapped event. Must be closed before exiting
	// to avoid a handle leak.
	osReader.hEvent = CreateEvent(NULL, TRUE, FALSE, NULL);

	if (osReader.hEvent == NULL)
	   // Error creating overlapped event; abort.
		return INVALID_HANDLE_VALUE;

	return hPort;
  }
}