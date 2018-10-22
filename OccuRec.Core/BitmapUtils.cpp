/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

#include "stdafx.h"
#include "BitmapUtils.h"
#include <stdlib.h>
#include <math.h>
#include "OccuRec.Core.h"

static float LO_GAMMA = 0.45f;
static float HI_GAMMA = 0.25f;
static BYTE LO_GAMMA_TABLE[256];
static BYTE HI_GAMMA_TABLE[256];
static BYTE HUE_INTENCITY_RED[256];
static BYTE HUE_INTENCITY_GREEN[256];
static BYTE HUE_INTENCITY_BLUE[256];
static bool tablesComputed = false;

void CopyBitmapHeaders(long width, long height, bool flipVertically, BYTE* bitmapPixels)
{
	// define the bitmap information header 
	BITMAPINFOHEADER bih;
	bih.biSize = sizeof(BITMAPINFOHEADER); 
	bih.biPlanes = 1; 
	bih.biBitCount = 24;                          // 24-bit 
	bih.biCompression = BI_RGB;                   // no compression 
	bih.biSizeImage = width * abs(height) * 3;    // width * height * (RGB bytes) 
	bih.biXPelsPerMeter = 0; 
	bih.biYPelsPerMeter = 0; 
	bih.biClrUsed = 0; 
	bih.biClrImportant = 0; 
	bih.biWidth = width;                          // bitmap width 
	bih.biHeight = flipVertically ? -height : height; // bitmap height 

	// and BitmapInfo variable-length UDT
	BYTE memBitmapInfo[40];
	RtlMoveMemory(memBitmapInfo, &bih, sizeof(bih));

	BITMAPFILEHEADER bfh;
	bfh.bfType=19778;    //BM header
	bfh.bfSize=55 + bih.biSizeImage;
	bfh.bfReserved1=0;
	bfh.bfReserved2=0;
	bfh.bfOffBits=sizeof(BITMAPINFOHEADER) + sizeof(BITMAPFILEHEADER); //54

	// Copy the display bitmap including the header
	RtlMoveMemory(bitmapPixels, &bfh, sizeof(bfh));
	RtlMoveMemory(bitmapPixels + sizeof(bfh), &memBitmapInfo, sizeof(memBitmapInfo));
}

HRESULT GetBitmapPixels(long width, long height, long bpp, long flipMode, long* pixels, BYTE* bitmapPixels)
{
	bool flipHorizontally = flipMode == 1 || flipMode == 3;
	bool flipVertically = flipMode == 2 || flipMode == 3;

	CopyBitmapHeaders(width, height, flipVertically, bitmapPixels);

	long x_sp = 3 * width;
	long x_nrc = -6 * width;
	long x_inc = 3;

	if (flipHorizontally)
	{
		x_sp = 0;
		x_nrc = 0;
		x_inc = -3;
	}

	long currLinePos = 0;
	int length = width * height;
	bitmapPixels+=54 + 3 * length + x_sp;

	int shiftVal = bpp == 12 ? 4 : 8;

	int total = width * height;
	while(total--)
	{
		if (currLinePos == 0) 
		{
			currLinePos = width;
			bitmapPixels+= x_nrc;
		};
		
		unsigned int val = *pixels;		
		pixels++;

		unsigned int dblVal;
		if (bpp == 8)
		{
			dblVal = val;
		}
		else
		{
			dblVal = val >> shiftVal;
		}
		 

		BYTE btVal = (BYTE)(dblVal & 0xFF);
		
		*bitmapPixels = btVal;
		*(bitmapPixels + 1) = btVal;
		*(bitmapPixels + 2) = btVal;
		bitmapPixels+=x_inc;

		currLinePos--;
	}

	return S_OK;
}

void ColorFromAhsb(int a, float h, float s, float b, int* cr, int* cg, int* cb)
{
	h = 360 * h / 240;
	s = s / 240;
	b = b / 240;

	if (0 == s)
	{
		*cr = (int)(b * 255);
		*cg = (int)(b * 255);
		*cb = (int)(b * 255);
		return;
	}

	float fMax, fMid, fMin;
	int iSextant, iMax, iMid, iMin;

	if (0.5 < b)
	{
		fMax = b - (b * s) + s;
		fMin = b + (b * s) - s;
	}
	else
	{
		fMax = b + (b * s);
		fMin = b - (b * s);
	}

	iSextant = (int)floor(h / 60.0);
	if (300.0 <= h)
	{
		h -= 360.0;
	}
	h /= 60.0;
	h -= 2.0 * (float)floor(((iSextant + 1) % 6) / 2.0);
	if (0 == iSextant % 2)
	{
		fMid = h * (fMax - fMin) + fMin;
	}
	else
	{
		fMid = fMin - h * (fMax - fMin);
	}

	iMax = (int)(fMax * 255);
	iMid = (int)(fMid * 255);
	iMin = (int)(fMin * 255);

	switch (iSextant)
	{
		case 1:
			*cr = iMid; *cg = iMax; *cb = iMin;
			return;
		case 2:
			*cr = iMin; *cg = iMax; *cb = iMid;
			return;
		case 3:
			*cr = iMin; *cg = iMid; *cb = iMax;
			return;
		case 4:
			*cr = iMid; *cg = iMin; *cb = iMax;
			return;
		case 5:
			*cr = iMax; *cg = iMin; *cb = iMid;
			return;
		default:
			*cr = iMax; *cg = iMid; *cb = iMin;
			return;
	}
}

double round_(double value)
{
	return floor(value + 0.5);
};

void ComputeTables()
{
	double lowGammaMax = 255.0 / pow(255, LO_GAMMA);
	double highGammaMax = 255.0 / pow(255, HI_GAMMA);

	int cr = 0;
	int cg = 0;
	int cb = 0;

	for (int i = 0; i <= 255; i++)
	{
		double lowGammaValue = lowGammaMax * pow(i, LO_GAMMA);
		double highGammaValue = highGammaMax * pow(i, HI_GAMMA);

		double rl = round_(lowGammaValue);
		double rh = round_(highGammaValue);
		LO_GAMMA_TABLE[i] = (BYTE)max(0, min(255, rl));
		HI_GAMMA_TABLE[i] = (BYTE)max(0, min(255, rh));

		// HUE Table 3
		if (i <= 180)
		{
			ColorFromAhsb(0, 160 - i, 240, 120, &cr, &cg, &cb);
		}
		else
		{
			ColorFromAhsb(0, 0, 240, 120 - (i - 160), &cr, &cg, &cb);
		}

		HUE_INTENCITY_RED[i] = cr;
		HUE_INTENCITY_GREEN[i] = cg;
		HUE_INTENCITY_BLUE[i] = cb;
	}
}

HRESULT GetBitmapPixels2(long width, long height, long bpp, long flipMode, long* pixels, BYTE* bitmapPixels, long gamma, bool invert, bool hueIntensity, bool saturationCheck, long saturationWarningValue)
{
	if (!tablesComputed)
	{
		ComputeTables();
		tablesComputed = true;
	}

	bool flipHorizontally = flipMode == 1 || flipMode == 3;
	bool flipVertically = flipMode == 2 || flipMode == 3;

	CopyBitmapHeaders(width, height, flipVertically, bitmapPixels);

	long x_sp = 3 * width;
	long x_nrc = -6 * width;
	long x_inc = 3;

	if (flipHorizontally)
	{
		x_sp = 0;
		x_nrc = 0;
		x_inc = -3;
	}

	long currLinePos = 0;
	int length = width * height;
	bitmapPixels+=54 + 3 * length + x_sp;

	int shiftVal = bpp == 12 ? 4 : 8;

	int total = width * height;

	BYTE minVal = 0;
	double sum = 0;
	int sumCount = 0;
	if (hueIntensity)
	{
		long* p = pixels;

		for (int counter = 0; counter < total; ++counter)
		{
			sum += *p;
			sumCount++;
			p++;
		}

		minVal = (BYTE)(sum / sumCount);
	}

	if (gamma == 1)
		minVal = LO_GAMMA_TABLE[minVal];
	else if (gamma == 2)
		minVal = HI_GAMMA_TABLE[minVal];

	while(total--)
	{
		if (currLinePos == 0) 
		{
			currLinePos = width;
			bitmapPixels+= x_nrc;
		};
		
		unsigned int val = *pixels;		
		pixels++;

		unsigned int dblVal;
		if (bpp == 8)
		{
			dblVal = val;
		}
		else
		{
			dblVal = val >> shiftVal;
		}
		 

		BYTE btVal = (BYTE)(dblVal & 0xFF);
		BYTE btrVal = btVal;
		BYTE btgVal = btVal;
		BYTE btbVal = btVal;

		if (saturationCheck && btVal > saturationWarningValue)
		{
			btrVal = 160; btgVal = 0; btbVal = 0;
		}
		else
		{
			if (gamma == 1)
			{
				btrVal = LO_GAMMA_TABLE[btrVal];
				btgVal = LO_GAMMA_TABLE[btgVal];
				btbVal = LO_GAMMA_TABLE[btbVal];
			}
			else if (gamma == 2)
			{	
				btrVal = HI_GAMMA_TABLE[btrVal];
				btgVal = HI_GAMMA_TABLE[btgVal];
				btbVal = HI_GAMMA_TABLE[btbVal];
			}
		}

		if (invert)
		{
			btbVal = (BYTE)(min(255, 255 - btbVal));
			btgVal = (BYTE)(min(255, 255 - btgVal));
			btrVal = (BYTE)(min(255, 255 - btrVal));
		}

		if (hueIntensity)
		{
			if (invert)
			{
				btbVal = HUE_INTENCITY_RED[min(255, btbVal + minVal)];
				btgVal = HUE_INTENCITY_GREEN[min(255, btgVal + minVal)];
				btrVal = HUE_INTENCITY_BLUE[min(255, btrVal + minVal)];
			}
			else
			{
				btbVal = HUE_INTENCITY_RED[max(0, btbVal - minVal)];
				btgVal = HUE_INTENCITY_GREEN[max(0, btgVal - minVal)];
				btrVal = HUE_INTENCITY_BLUE[max(0, btrVal - minVal)];
			}
		}

		*bitmapPixels = btbVal;
		*(bitmapPixels + 1) = btgVal;
		*(bitmapPixels + 2) = btrVal;
		bitmapPixels+=x_inc;

		currLinePos--;
	}

	return S_OK;
}

HRESULT GetColourBitmapPixels(long width, long height, long bpp, long flipMode, long* pixels, BYTE* bitmapPixels)
{
	bool flipHorizontally = flipMode == 1 || flipMode == 3;
	bool flipVertically = flipMode == 2 || flipMode == 3;

	CopyBitmapHeaders(width, height, flipVertically, bitmapPixels);
	bitmapPixels+= 54;

	long x_sp = 3 * width;
	long x_nrc = -6 * width;
	long x_inc = 3;

	if (flipHorizontally)
	{
		x_sp = 0;
		x_nrc = 0;
		x_inc = -3;
	}

	long currLinePos = 0;
	int length = width * height;
	int twoLengths = 2 * length;
	bitmapPixels+=3 * length + x_sp;

	int shiftVal = bpp == 12 ? 4 : 8;

	int total = width * height;
	while(total--)
	{
		if (currLinePos == 0) 
		{
			currLinePos = width;
			bitmapPixels+=x_nrc;
		};

		unsigned int valR = *pixels;
		unsigned int valG = *(pixels + length);
		unsigned int valB = *(pixels + twoLengths);

		pixels++;

		unsigned int dblValR;
		unsigned int dblValG;
		unsigned int dblValB;

		if (bpp == 8)
		{
			dblValR = valR;
			dblValG = valG;
			dblValB = valB;
		}
		else
		{
			dblValR = valR >> shiftVal;
			dblValG = valG >> shiftVal;
			dblValB = valB >> shiftVal;
		}
		
		*bitmapPixels = (BYTE)(dblValB & 0xFF);
		*(bitmapPixels + 1) = (BYTE)(dblValG & 0xFF);
		*(bitmapPixels + 2) = (BYTE)(dblValR & 0xFF);

		bitmapPixels+=x_inc;

		currLinePos--;
	}

	return S_OK;
}

HRESULT GetRGGBBayerBitmapPixels(long width, long height, long bpp, long* pixels, BYTE* bitmapPixels)
{
	// RGGB Format:
	// 
	//           X = 1      X = 2  
	// Y = 1       R          G
	// Y = 2       G          B

	// Possible conversion from the BIAS open source library
	http://www.mip.informatik.uni-kiel.de/~wwwadmin/Software/Doc/BIAS/html/d5/d41/ImageConvert_8cpp_source.html

	return E_NOTIMPL;
}

HRESULT GetMonochromePixelsFromBitmap(long width, long height, long bpp, long flipMode, HBITMAP* bitmap, long* pixels, int mode)
{
	BITMAP bmp;
	GetObject(bitmap, sizeof(bmp), &bmp);

	unsigned char* buf = reinterpret_cast<unsigned char*>(bmp.bmBits);

	unsigned char* ptrBuf = buf + ((width * height) - 1) * 4;

	for (int y=0; y < height; y++)
    {
		for (int x=0; x < width; x++)
		{
			long pixVal = 0;
			if (mode == 0)
				pixVal = *(ptrBuf + 2); //R
			else if (mode == 1)
				pixVal = *(ptrBuf + 1); //G
			else if (mode == 2)
				pixVal = *(ptrBuf); //B
			else if (mode == 3)
			{
				// YUV Conversion (PAL & NTSC)
				// Luma = 0.299 R + 0.587 G + 0.114 B
				double luma = 0.299* *(ptrBuf) + 0.587* *(ptrBuf + 1) + 0.114* *(ptrBuf + 2);
				pixVal = (long)luma;
			}

			if (flipMode == 0)
			{
				*(pixels + (width - 1 - x) + width * y) = pixVal;
			}
			else if (flipMode == 1) /* Flip Horizontally */
			{
				*(pixels + x + width * y) = pixVal;
			}
			else if (flipMode == 2) /* Flip Vertically */
			{
				*(pixels + (width - 1 - x) + width * (height - 1 - y)) = pixVal;
			}
			else if (flipMode == 3) /* Flip Horizontally & Vertically */
			{
				*(pixels + x + width * (height - 1 - y)) = pixVal;
			}

			ptrBuf-=4;
		}
	}

	return S_OK;
}

HRESULT GetColourPixelsFromBitmap(long width, long height, long bpp, long flipMode, HBITMAP* bitmap, long* pixels)
{
	BITMAP bmp;
	GetObject(bitmap, sizeof(bmp), &bmp);

	long* ptrPixelsR = pixels;
	long* ptrPixelsG = pixels + (width * height);
	long* ptrPixelsB = pixels + 2 * (width * height);
	unsigned char* buf = reinterpret_cast<unsigned char*>(bmp.bmBits);

	unsigned char* ptrBuf = buf + ((width * height) - 1) * 4;

	for (int y=0; y < height; y++)
    {
		for (int x=0; x < width; x++)
		{
			BYTE* currBitmapPixel;

			if (flipMode == 0)
			{
				*(ptrPixelsR + (width - 1 - x) + width * y ) = *(ptrBuf + 2);
				*(ptrPixelsG + (width - 1 - x) + width * y ) = *(ptrBuf + 1);
				*(ptrPixelsB + (width - 1 - x) + width * y ) = *(ptrBuf);
			}
			else if (flipMode == 1) /* Flip Horizontally */
			{

				*(ptrPixelsR + x + width * y ) = *(ptrBuf + 2);
				*(ptrPixelsG + x + width * y ) = *(ptrBuf + 1);
				*(ptrPixelsB + x + width * y ) = *(ptrBuf);
			}
			else if (flipMode == 2) /* Flip Vertically */
			{
				*(ptrPixelsR + (width - 1 - x) + width * (height - 1 - y) ) = *(ptrBuf + 2);
				*(ptrPixelsG + (width - 1 - x) + width * (height - 1 - y) ) = *(ptrBuf + 1);
				*(ptrPixelsB + (width - 1 - x) + width * (height - 1 - y) ) = *(ptrBuf);
			}
			else if (flipMode == 3) /* Flip Horizontally & Vertically */
			{
				*(ptrPixelsR + x + width * (height - 1 - y) ) = *(ptrBuf + 2);
				*(ptrPixelsG + x + width * (height - 1 - y) ) = *(ptrBuf + 1);
				*(ptrPixelsB + x + width * (height - 1 - y) ) = *(ptrBuf);
			}

			ptrBuf-=4;
		}
	}

	return S_OK;

}

HRESULT GetBitmapBytes(long width, long height, HBITMAP* bitmap, BYTE* bitmapPixels)
{
	BITMAP bmp;
	GetObject(bitmap, sizeof(bmp), &bmp);

	unsigned char* buf = reinterpret_cast<unsigned char*>(bmp.bmBits);

	CopyBitmapHeaders(width, height, false, bitmapPixels);
	memcpy(bitmapPixels + 53, buf, width * height * 3);

	unsigned char* ptrBuf = buf + ((width * height) - 1) * 4;
	bitmapPixels += 54 + ((width * height) - 1) * 3;

	for (int y=0; y < height; y++)
    {
		for (int x=0; x < width; x++)
		{
			*(bitmapPixels) = *(ptrBuf);
			*(bitmapPixels + 1) = *(ptrBuf + 1);
			*(bitmapPixels + 2) = *(ptrBuf + 2);

			bitmapPixels-=3;
			ptrBuf-=4;
		}
	}
	return S_OK;
}

HRESULT GetRGGBBayerPixelsFromBitmap(long width, long height, long bpp, HBITMAP* bitmap, long* pixels)
{
	// RGGB Format:
	// 
	//           X = 1      X = 2  
	// Y = 1       R          G
	// Y = 2       G          B

	// 

	return E_NOTIMPL;
}

HRESULT GetMonochromePixelsFromBitmapEx(long width, long height, long bpp, long flipMode, HBITMAP* bitmap, long* pixels, BYTE* bitmapPixels, int mode)
{
	BITMAP bmp;
	GetObject(bitmap, sizeof(bmp), &bmp);

	unsigned char* buf = reinterpret_cast<unsigned char*>(bmp.bmBits);

	CopyBitmapHeaders(width, height, false, bitmapPixels);
	
	unsigned char* ptrBuf = buf + ((width * height) - 1) * 4;
	bitmapPixels += 54 + ((width * height) - 1) * 3;

	for (int y=0; y < height; y++)
    {
		for (int x=0; x < width; x++)
		{
			long pixVal = 0;
			if (mode == 0)
				pixVal = *(ptrBuf + 2); //R
			else if (mode == 1)
				pixVal = *(ptrBuf + 1); //G
			else if (mode == 2)
				pixVal = *(ptrBuf); //B
			else if (mode == 3)
			{
				// YUV Conversion (PAL & NTSC)
				// Luma = 0.299 R + 0.587 G + 0.114 B
				double luma = 0.299* *(ptrBuf) + 0.587* *(ptrBuf + 1) + 0.114* *(ptrBuf + 2);
				pixVal = (long)luma;
			}

			BYTE* currBitmapPixel;
			if (flipMode == 0)
			{
				*(pixels + (width - 1 - x) + width * y) = pixVal;
				currBitmapPixel = bitmapPixels - 3 * (x + width * y);
			}
			else if (flipMode == 1) /* Flip Horizontally */
			{
				*(pixels + x + width * y) = pixVal;
				currBitmapPixel = bitmapPixels - 3 * ((width - 1 - x) + width * y);
			}
			else if (flipMode == 2) /* Flip Vertically */
			{
				*(pixels + (width - 1 - x) + width * (height - 1 - y)) = pixVal;
				currBitmapPixel = bitmapPixels - 3 * (x +  width * (height - 1 - y));
			}
			else if (flipMode == 3) /* Flip Horizontally & Vertically */
			{
				*(pixels + x + width * (height - 1 - y)) = pixVal;
				currBitmapPixel = bitmapPixels - 3 * ((width - 1 - x) + width * (height - 1 - y));
			}

			*(currBitmapPixel) = (BYTE) pixVal;
			*(currBitmapPixel + 1) = (BYTE) pixVal;
			*(currBitmapPixel + 2) = (BYTE) pixVal;

			ptrBuf-=4;
		}
	}

	return S_OK;
}

HRESULT GetColourPixelsFromBitmapEx(long width, long height, long bpp, long flipMode, HBITMAP* bitmap, long* pixels, BYTE* bitmapPixels)
{
	BITMAP bmp;
	GetObject(bitmap, sizeof(bmp), &bmp);

	long* ptrPixelsR = pixels;
	long* ptrPixelsG = pixels + (width * height);
	long* ptrPixelsB = pixels + 2 * (width * height);
	unsigned char* buf = reinterpret_cast<unsigned char*>(bmp.bmBits);

	CopyBitmapHeaders(width, height, false, bitmapPixels);

	unsigned char* ptrBuf = buf + ((width * height) - 1) * 4;

	bitmapPixels += 54 + ((width * height) - 1) * 3;

	for (int y=0; y < height; y++)
    {
		for (int x=0; x < width; x++)
		{
			BYTE* currBitmapPixel;

			if (flipMode == 0)
			{
				*(ptrPixelsR + (width - 1 - x) + width * y ) = *(ptrBuf + 2);
				*(ptrPixelsG + (width - 1 - x) + width * y ) = *(ptrBuf + 1);
				*(ptrPixelsB + (width - 1 - x) + width * y ) = *(ptrBuf);

				currBitmapPixel = bitmapPixels - 3 * (x + width * y);
			}
			else if (flipMode == 1) /* Flip Horizontally */
			{

				*(ptrPixelsR + x + width * y ) = *(ptrBuf + 2);
				*(ptrPixelsG + x + width * y ) = *(ptrBuf + 1);
				*(ptrPixelsB + x + width * y ) = *(ptrBuf);

				currBitmapPixel = bitmapPixels - 3 * ((width - 1 - x) + width * y);
			}
			else if (flipMode == 2) /* Flip Vertically */
			{
				*(ptrPixelsR + (width - 1 - x) + width * (height - 1 - y) ) = *(ptrBuf + 2);
				*(ptrPixelsG + (width - 1 - x) + width * (height - 1 - y) ) = *(ptrBuf + 1);
				*(ptrPixelsB + (width - 1 - x) + width * (height - 1 - y) ) = *(ptrBuf);

				currBitmapPixel = bitmapPixels - 3 * (x +  width * (height - 1 - y));
			}
			else if (flipMode == 3) /* Flip Horizontally & Vertically */
			{
				*(ptrPixelsR + x + width * (height - 1 - y) ) = *(ptrBuf + 2);
				*(ptrPixelsG + x + width * (height - 1 - y) ) = *(ptrBuf + 1);
				*(ptrPixelsB + x + width * (height - 1 - y) ) = *(ptrBuf);

				currBitmapPixel = bitmapPixels - 3 * ((width - 1 - x) + width * (height - 1 - y));
			}

			*(currBitmapPixel) = *(ptrBuf);
			*(currBitmapPixel + 1) = *(ptrBuf + 1);
			*(currBitmapPixel + 2) = *(ptrBuf + 2);

			ptrBuf-=4;
		}
	}

	return S_OK;

}
