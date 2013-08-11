
#ifndef _LOCKFREESPINLOCK__H
#define _LOCKFREESPINLOCK__H

#include "Helpers.h"
	

namespace LockFree
{
	const unsigned int YIELD_ITERATION = 30; // yeild after 30 iterations
	const unsigned int MAX_SLEEP_ITERATION = 40; 
	const int SeedVal = 100;

	// This class acts as a synchronization object similar to a mutex

	struct tSpinLock
	{
		volatile LONG dest;
		LONG exchange;
		LONG compare;

		tSpinLock(){
			dest = 0;
			exchange = SeedVal;
			compare = 0;		
		}
	};
	//
	

	// This class provides wait free locking functionalities 
	class tSpinWait
	{
	public:
		
		tSpinWait(){ m_iterations = 0;}
		inline bool HasThreasholdReached() { return (m_iterations >= YIELD_ITERATION); }
		void Lock(tSpinLock &LockObj);
		void Unlock(tSpinLock &LockObj);

	private:
		tSpinWait(const tSpinWait&);
		tSpinWait& operator=(const tSpinWait&);
	private:
		unsigned int m_iterations;
		
	};
};

#endif // _LOCKFREESPINLOCK__H