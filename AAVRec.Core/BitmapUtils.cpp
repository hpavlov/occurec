//tabs=4
// --------------------------------------------------------------------------------
//
// Koyash.VideoUtilities - Unmanaged implementation
//
// Description:	Unmanaged methods to create bitmaps from various ImageArray data
//
// Author:		(HDP) Hristo Pavlov <hristo_dpavlov@yahoo.com>
//
// --------------------------------------------------------------------------------
//

// BitmapUtils.cpp : Defines the exported functions for the DLL application.
//

#include "stdafx.h"
#include "BitmapUtils.h"
#include <stdlib.h>
#include <math.h>


HRESULT GetBitmapPixels(long width, long height, long bpp, long* pixels, BYTE* bitmapPixels)
{
	BYTE* pp = bitmapPixels;

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
	bih.biHeight = height;                        // bitmap height 

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

	bitmapPixels = bitmapPixels + sizeof(bfh) + sizeof(memBitmapInfo);

	long currLinePos = 0;
	int length = width * height;
	bitmapPixels+=3 * (length + width);

	int shiftVal = bpp == 12 ? 4 : 8;

	int total = width * height;
	while(total--)
	{
		if (currLinePos == 0) 
		{
			currLinePos = width;
			bitmapPixels-=6*width;
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
		bitmapPixels+=3;

		currLinePos--;
	}

	return S_OK;
}

HRESULT GetColourBitmapPixels(long width, long height, long bpp, long* pixels, BYTE* bitmapPixels)
{
	BYTE* pp = bitmapPixels;

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
	bih.biHeight = height;                        // bitmap height 

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

	bitmapPixels = bitmapPixels + sizeof(bfh) + sizeof(memBitmapInfo);

	long currLinePos = 0;
	int length = width * height;
	int twoLengths = 2 * length;
	bitmapPixels+=3 * (length + width);

	int shiftVal = bpp == 12 ? 4 : 8;

	int total = width * height;
	while(total--)
	{
		if (currLinePos == 0) 
		{
			currLinePos = width;
			bitmapPixels-=6*width;
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

		bitmapPixels+=3;

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

HRESULT GetMonochromePixelsFromBitmap(long width, long height, long bpp, HBITMAP* bitmap, long* pixels, int mode)
{
	BITMAP bmp;
	GetObject(bitmap, sizeof(bmp), &bmp);

	long* ptrPixels = pixels;

	unsigned char* buf = reinterpret_cast<unsigned char*>(bmp.bmBits);

	unsigned char* ptrBuf = buf + ((width * height) - 1) * 4;

    for (int i=0; i < width * height; i++)
    {
		if (mode == 0)
			*ptrPixels = *(ptrBuf + 2); //R
		else if (mode == 1)
			*ptrPixels = *(ptrBuf + 1); //G
		else if (mode == 2)
			*ptrPixels = *(ptrBuf); //B
		else if (mode == 3)
		{
			// YUV Conversion (PAL & NTSC)
			// Luma = 0.299 R + 0.587 G + 0.114 B
			double luma = 0.299* *(ptrBuf) + 0.587* *(ptrBuf + 1) + 0.114* *(ptrBuf + 2);

			if (luma < 0)
				*ptrPixels = 0;
			else if (luma > 255)
				*ptrPixels = 255;
			else
				*ptrPixels = (long)luma;
		}

		ptrPixels++;
		ptrBuf-=4;
    }

	return S_OK;
}

HRESULT GetColourPixelsFromBitmap(long width, long height, long bpp, HBITMAP* bitmap, long* pixels)
{
	BITMAP bmp;
	GetObject(bitmap, sizeof(bmp), &bmp);

	long* ptrPixelsR = pixels;
	long* ptrPixelsG = pixels + (width * height);
	long* ptrPixelsB = pixels + 2 * (width * height);

	unsigned char* buf = reinterpret_cast<unsigned char*>(bmp.bmBits);

	unsigned char* ptrBuf = buf + ((width * height) - 1) * 4;

    for (int i=0; i < width * height; i++)
    {
		*ptrPixelsR = *(ptrBuf + 2);
		*ptrPixelsG = *(ptrBuf + 1);
		*ptrPixelsB = *(ptrBuf);

		ptrPixelsR++;
		ptrPixelsG++;
		ptrPixelsB++;
		ptrBuf-=4;
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


