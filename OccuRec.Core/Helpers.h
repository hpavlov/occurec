/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

#ifndef __UTILITY_HELPER__H

#define __UTILITY_HELPER__H

#include "stdafx.h"

namespace LockFree
{
	// disallow copying objects
	template <class T>
	class NonCopyable
	{
	  protected:

		NonCopyable () {}
		~NonCopyable () {} /// Protected non-virtual destructor

	  private: 

		NonCopyable (const NonCopyable &);
		T & operator = (const T &);
	};
	//

	class tSysInfo
	{
	public:
		tSysInfo()
		{
			GetSystemInfo( &sysinfo );
		}
		 
		unsigned int GetNumberofProcessors()
		{
			return sysinfo.dwNumberOfProcessors;
		}
	private:
		SYSTEM_INFO sysinfo;
	};
	//

	class Helper : private NonCopyable<Helper>
	{
	public:
		// Get the number of CPU on this machine
		static unsigned int GetNumberOfProcessors() 
		{
			static tSysInfo Sysinfo;
			return Sysinfo.GetNumberofProcessors();
		}
	private:
		Helper();
	};
	//
};

#endif // __UTILITY_HELPER__H