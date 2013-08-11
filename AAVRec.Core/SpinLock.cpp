
#include "stdafx.h"
#include "SpinLock.h"
#include <iostream>

using namespace LockFree;
using namespace std;

void tSpinWait::Lock(tSpinLock &LockObj)
{
	m_iterations = 0;
	while(true)
	{
		// A thread alreading owning the lock shouldn't be allowed to wait to acquire the lock - reentrant safe
		if(LockObj.dest == GetCurrentThreadId())
			break;
		/*
		  Spinning in a loop of interlockedxxx calls can reduce the available memory bandwidth and slow
		  down the rest of the system. Interlocked calls are expensive in their use of the system memory
		  bus. It is better to see if the 'dest' value is what it is expected and then retry interlockedxx.
		*/
		if(InterlockedCompareExchange(&LockObj.dest, LockObj.exchange, LockObj.compare) == 0)
		{
			//assign CurrentThreadId to dest to make it re-entrant safe
			LockObj.dest = GetCurrentThreadId();
			// lock acquired 
			break;			
		}
			
		// spin wait to acquire 
		while(LockObj.dest != LockObj.compare)
		{
			if(HasThreasholdReached())
			{
				if(m_iterations + YIELD_ITERATION >= MAX_SLEEP_ITERATION)
					Sleep(0);
				
				if(m_iterations >= YIELD_ITERATION && m_iterations < MAX_SLEEP_ITERATION)
				{
					m_iterations = 0;
					SwitchToThread();
				}
			}
			// Yield processor on multi-processor but if on single processor then give other thread the CPU
			m_iterations++;
			if(Helper::GetNumberOfProcessors() > 1) { YieldProcessor(/*no op*/); }
			else { SwitchToThread(); }				
		}				
	}
}
//

void tSpinWait::Unlock(tSpinLock &LockObj)
{
	if(LockObj.dest != GetCurrentThreadId())
		throw std::runtime_error("Unexpected thread-id in release");
	// lock released
	InterlockedCompareExchange(&LockObj.dest, LockObj.compare, GetCurrentThreadId());	
}
//

