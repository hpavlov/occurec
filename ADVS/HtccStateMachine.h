#ifndef HTCC_STATE_MACHINE
#define HTCC_STATE_MACHINE

#include "HtccMessage.h"
#include "HtccState.h"
#include "AdvrStateContext.h"

class HtccState;

class HtccStateMachine
{
private:
	HtccState* m_CurrentState;
		
public:
	CameraFrameRate StartingFrameRate;

	bool ProcessOneSecondTick();
	bool Initialise(CameraFrameRate startingFrameRate);
	HtccMessage* ReceivePacket(unsigned char* packet);
	void SetState(HtccState *newState);
	
	void TriggerTimestampCorruption(int mode);
	bool ToggleTimestampAlmanacOffset();
};

#endif