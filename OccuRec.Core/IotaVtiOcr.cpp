/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

#include "StdAfx.h"
#include "IotaVtiOcr.h"

long AREA1_TOP = 0;
long AREA1_LEFT = 0;
long AREA1_WIDTH = 0;
long AREA1_HEIGHT = 0;
long AREA2_TOP = 0;
long AREA2_LEFT = 0;
long AREA2_WIDTH = 0;
long AREA2_HEIGHT = 0;

HRESULT SetTimeStampArea1(long top, long left, long width, long height)
{
	AREA1_TOP = top;
	AREA1_LEFT = left;
	AREA1_WIDTH = width;
	AREA1_HEIGHT = height;

	return S_OK;
}

HRESULT SetTimeStampArea2(long top, long left, long width, long height)
{
	AREA2_TOP = top;
	AREA2_LEFT = left;
	AREA2_WIDTH = width;
	AREA2_HEIGHT = height;

	return S_OK;
}

void MarkTimeStampAreasOld(unsigned char* pixels)
{
	unsigned char* ptrTopLeft = pixels + (AREA1_TOP - 1) * IMAGE_WIDTH + (IMAGE_WIDTH - AREA1_LEFT);

	unsigned char* ptrVertLines = ptrTopLeft;
	for(int i = 0; i <= AREA1_HEIGHT; i++)
	{
		*ptrVertLines = 255;
		*(ptrVertLines - AREA1_WIDTH) = 255;
		ptrVertLines+=IMAGE_WIDTH;
	}

	unsigned char* ptrHorizLines = ptrTopLeft;
	for(int i = 0; i <= AREA1_WIDTH; i++)
	{
		*ptrHorizLines = 255;
		*(ptrHorizLines + AREA1_HEIGHT * IMAGE_WIDTH) = 255;
		ptrHorizLines--;
	}
}

void MarkTimeStampAreasNew(unsigned char* pixels)
{
	for(int x = AREA1_LEFT; x <= AREA1_LEFT + AREA1_WIDTH; x++)
	{
		MarkPixel(pixels, AREA1_TOP, x, 128);
		MarkPixel(pixels, AREA1_TOP + AREA1_HEIGHT, x, 128);
	}

	for(int y = AREA1_TOP; y <= AREA1_TOP + AREA1_HEIGHT; y++)
	{
		MarkPixel(pixels, y, AREA1_LEFT, 128);
		MarkPixel(pixels, y, AREA1_LEFT + AREA1_WIDTH, 128);
	}
}

void MarkTimeStampAreas(unsigned char* pixels)
{
	MarkTimeStampAreasNew(pixels);
}

void MarkPixel(unsigned char* pixels, long top, long left, unsigned char colour)
{
	*(pixels + (top - 1) * IMAGE_WIDTH + (IMAGE_WIDTH - left)) = colour;
}