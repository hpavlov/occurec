#include "stdafx.h"
#include "HtccMessage.h"
#include "HtccStateMachine.h"
#include "HtccState.h"
#include "HtccDisconnectedState.h"

bool HtccStateMachine::ProcessOneSecondTick()
{
	m_CurrentState->ProcessOneSecondTick(this);
};
	
bool HtccStateMachine::Initialise(CameraFrameRate startingFrameRate)
{
	StartingFrameRate = startingFrameRate;
	SetState(HtccDisconnectedState::Instance());
};

HtccMessage* HtccStateMachine::ReceivePacket(unsigned char* packet)
{
	return m_CurrentState->ReceivePacket(this, packet);
};

void HtccStateMachine::SetState(HtccState *newState)
{
	if (m_CurrentState != NULL)
		m_CurrentState->Finalise(this);
	
	m_CurrentState = newState;
	newState->Initialise(this);
};


void HtccStateMachine::TriggerTimestampCorruption(int mode)
{
	HtccMessage::s_CorruptTimeStampMode = mode;
	HtccMessage::s_CorruptNextEndTimeStampOnce = true;
}

bool HtccStateMachine::ToggleTimestampAlmanacOffset()
{
	HtccMessage::s_ToggleTimestampAlmanacOffset = !HtccMessage::s_ToggleTimestampAlmanacOffset;
	return HtccMessage::s_ToggleTimestampAlmanacOffset;
}