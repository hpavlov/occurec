#ifndef HTCC_CHANGING_BETWEEN_AUTO_AND_TRIGGERED_STATE
#define HTCC_CHANGING_BETWEEN_AUTO_AND_TRIGGERED_STATE

#include "HtccState.h"

class HtccChangingBetweenAutoAndTriggeredState: public HtccState
{
public:
	static HtccChangingBetweenAutoAndTriggeredState* s_pInstance;
	static HtccChangingBetweenAutoAndTriggeredState* Instance();


	virtual void Initialise(HtccStateMachine *context);
	virtual void Finalise(HtccStateMachine *context);
	virtual HtccMessage* ReceivePacket(HtccStateMachine *context, unsigned char* packet);
	virtual bool ProcessOneSecondTick(HtccStateMachine *context);
};

#endif