/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

// Based on https://software.intel.com/en-us/articles/using-cpuid-to-detect-the-presence-of-sse-41-and-sse-42-instruction-sets/

#pragma once

typedef struct
{
    unsigned __int32 EAX,EBX,ECX,EDX;
} CPUIDinfo;
 
void get_cpuid_info (CPUIDinfo *, const unsigned int, const unsigned int);
int isCPUIDsupported (void);
int isRDTSCSupported (void);
int isRDTSCPSupported (void);
int isInvariantTSCAvailable (void);
