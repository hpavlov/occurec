// The following ifdef block is the standard way of creating macros which make exporting 
// from a DLL simpler. All files within this DLL are compiled with the KOYASHVIDEOUTILS_EXPORTS
// symbol defined on the command line. This symbol should not be defined on any project
//tabs=4
// --------------------------------------------------------------------------------
//
// Koyash.VideoUtilities - Unmanaged implementation
//
// Description:	Header file 
//
// Author:		(HDP) Hristo Pavlov <hristo_dpavlov@yahoo.com>
//
// --------------------------------------------------------------------------------
//

// that uses this DLL. This way any other project whose source files include this file see 
// KOYASHVIDEOUTILS_API functions as being imported from a DLL, whereas this DLL sees symbols
// defined with this macro as being exported.
#ifdef KOYASHVIDEOUTILS_EXPORTS
#define KOYASHVIDEOUTILS_API __declspec(dllexport)
#else
#define KOYASHVIDEOUTILS_API __declspec(dllimport)
#endif

#include <windows.h>

HRESULT GetBitmapPixels(long width, long height, long bpp, long* pixels, BYTE* bitmapPixels);
HRESULT GetColourBitmapPixels(long width, long height, long bpp, long* pixels, BYTE* bitmapPixels);
HRESULT GetRGGBBayerBitmapPixels(long width, long height, long bpp, long* pixels, BYTE* bitmapPixels);
HRESULT GetMonochromePixelsFromBitmap(long width, long height, long bpp, HBITMAP* bitmap, long* pixels, int mode);
HRESULT GetColourPixelsFromBitmap(long width, long height, long bpp, HBITMAP* bitmap, long* pixels);
HRESULT GetRGGBBayerPixelsFromBitmap(long width, long height, long bpp, HBITMAP* bitmap, long* pixels);