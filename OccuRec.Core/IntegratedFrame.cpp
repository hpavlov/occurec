#include "StdAfx.h"
#include "IntegratedFrame.h"
#include "stdlib.h"


IntegratedFrame::IntegratedFrame(long totalPixelsInFrame)
{
	m_TotalPixelsInFrame = totalPixelsInFrame;
	Pixels = (unsigned char*)malloc(totalPixelsInFrame);
}

IntegratedFrame::~IntegratedFrame(void)
{
	if (Pixels != NULL)
	{
		delete Pixels;
		Pixels = NULL;
	}
}
