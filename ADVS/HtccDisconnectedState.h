#ifndef HTCC_DISCONNECTED_STATE
#define HTCC_DISCONNECTED_STATE

#include "HtccState.h"

class HtccDisconnectedState : public HtccState
{
private:
	bool HtccDetected;
	bool UnitIsReady;
	
	// Send a 'V' packet every second until HTCC responds
public:
	static HtccDisconnectedState* s_pInstance;
	static HtccDisconnectedState* Instance();


	virtual void Initialise(HtccStateMachine *context);
	virtual void Finalise(HtccStateMachine *context);
	virtual HtccMessage* ReceivePacket(HtccStateMachine *context, unsigned char* packet);
	virtual bool ProcessOneSecondTick(HtccStateMachine *context);
};

#endif