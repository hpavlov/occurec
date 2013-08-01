#include "StdAfx.h"
#include "RawFrame.h"
#include "stdlib.h"


RawFrame::RawFrame(int imageWidth, int imageHeight)
{
	BmpBitsSize = imageWidth * imageHeight * 3;
	BmpBits = (unsigned char*)malloc(BmpBitsSize * sizeof(unsigned char));
}

RawFrame::~RawFrame(void)
{
	if (BmpBits != NULL)
	{
		delete BmpBits;
		BmpBits = NULL;
	}
}
