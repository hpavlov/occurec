/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

#ifndef ADVRUTILS_H
#define ADVRUTILS_H

#include <string>
#include <vector>

using namespace std;

size_t strpos(const string &haystack, const string &needle)
{
    int sleng = haystack.length();
    int nleng = needle.length();

    if (sleng==0 || nleng==0)
        return string::npos;

    for(int i=0, j=0; i<sleng; j=0, i++ )
    {
        while (i+j<sleng && j<nleng && haystack[i+j]==needle[j])
            j++;
        if (j==nleng)
            return i;
    }
    return string::npos;	
};

long long DateTimeToAdvTicks(int years, int months, int days, int hours, int minutes, int second, int fractionalSecond10000)
{
	// TODO:
	return 0;
};

#endif