#ifndef HTCC_INITIALISED_STATE
#define HTCC_INITIALISED_STATE

#include "HtccState.h"

class HtccInitialisedState: public HtccState
{

public:
	static HtccInitialisedState* s_pInstance;
	static HtccInitialisedState* Instance();


	virtual void Initialise(HtccStateMachine *context);
	virtual void Finalise(HtccStateMachine *context);
	virtual HtccMessage* ReceivePacket(HtccStateMachine *context, unsigned char* packet);
	virtual bool ProcessOneSecondTick(HtccStateMachine *context);
};

#endif