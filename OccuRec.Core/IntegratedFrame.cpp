/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

#include "StdAfx.h"
#include "IntegratedFrame.h"
#include "stdlib.h"


IntegratedFrame::IntegratedFrame(long totalPixelsInFrame, bool is16Bit)
{
	m_TotalPixelsInFrame = totalPixelsInFrame;
	if (is16Bit)
	{
		Pixels = NULL;
		Pixels16 = (unsigned short*)malloc(totalPixelsInFrame * sizeof(unsigned short));
	}
	else
	{
		Pixels = (unsigned char*)malloc(totalPixelsInFrame);
		Pixels16 = NULL;
	}
}

IntegratedFrame::~IntegratedFrame(void)
{
	if (Pixels != NULL)
	{
		delete Pixels;
		Pixels = NULL;
	}

	if (Pixels16 != NULL)
	{
		delete Pixels16;
		Pixels16 = NULL;
	}
}
