/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

// Code based on https://software.intel.com/en-us/articles/using-cpuid-to-detect-the-presence-of-sse-41-and-sse-42-instruction-sets/
// Intel specs for CPU feaures: https://www.intel.com/content/www/us/en/architecture-and-technology/64-ia-32-architectures-software-developer-vol-2a-manual.html

#pragma once

#include "StdAfx.h"
#include "cpuid.h"

int isRDTSCSupported (void)
{
	CPUIDinfo Info;
    int rVal = 0;
	if (isCPUIDsupported())
    {
		// CPUID.01H:EDX[4]
        get_cpuid_info(&Info, 0x1, 0x0);
		if ((Info.EDX & 0x10) == 0x10)
        {
            rVal = 1;
        }
	}
	return rVal;
}

int isRDTSCPSupported (void)
{
	CPUIDinfo Info;
    int rVal = 0;
	if (isCPUIDsupported())
    {
		// CPUID.80000001H:EDX[27]
        get_cpuid_info(&Info, 0x80000001, 0x0);
		if ((Info.EDX & 0x08000000) == 0x08000000)
        {
            rVal = 1;
        }
	}
	return rVal;
}

int isInvariantTSCAvailable (void)
{
	CPUIDinfo Info;
    int rVal = 0;
	if (isCPUIDsupported())
    {
		// CPUID.80000007H:EDX[8]
        get_cpuid_info(&Info, 0x80000007, 0x0);
		if ((Info.EDX & 0x100) == 0x100)
        {
            rVal = 1;
        }
	}
	return rVal;
}

int cpuid_checked = 0;
int cpuid_supported = 0;

int isCPUIDsupportedInternal (void)
{
    // returns 1 if CPUID instruction supported on this processor, zero otherwise
    // This isn't necessary on 64 bit processors because all 64 bit Intel processors support CPUID
    __asm 
    {
        push ecx ; save ecx
        pushfd ; push original EFLAGS
        pop eax ; get original EFLAGS
        mov ecx, eax ; save original EFLAGS
        xor eax, 200000h ; flip bit 21 in EFLAGS
        push eax ; save new EFLAGS value on stack
        popfd ; replace current EFLAGS value
        pushfd ; get new EFLAGS
        pop eax ; store new EFLAGS in EAX
        xor eax, ecx ; Bit 21 of flags at 200000h will be 1 if CPUID exists
        shr eax, 21  ; Shift bit 21 bit 0 and return it
        push ecx
        popfd ; restore bit 21 in EFLAGS first
        pop ecx ; restore ecx
    }
}

int isCPUIDsupported (void)
{
	if (cpuid_checked == 0)
	{
		cpuid_supported = isCPUIDsupportedInternal();
		cpuid_checked = 1;
	}

	return cpuid_supported;
}

void get_cpuid_info (CPUIDinfo *Info, const unsigned int leaf, const unsigned int subleaf)
{
    // Stores CPUID return Info in the CPUIDinfo structure.
    // leaf and subleaf used as parameters to the CPUID instruction
    // parameters and register usage designed to be safe for both Windows and Linux
    // Use the Intel compiler option -use-msasm when the target is Linux
    __asm 
    {
        mov edx, Info   ; addr of start of output array
        mov eax, leaf  ; leaf
        mov ecx, subleaf  ; subleaf
        push edi
        push ebx
        mov  edi, edx                      ; edi has output addr
        cpuid
        mov DWORD PTR [edi], eax
        mov DWORD PTR [edi+4], ebx
        mov DWORD PTR [edi+8], ecx
        mov DWORD PTR [edi+12], edx
        pop ebx
        pop edi
    }
}

