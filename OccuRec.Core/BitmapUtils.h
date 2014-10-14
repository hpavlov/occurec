/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

#ifdef KOYASHVIDEOUTILS_EXPORTS
#define KOYASHVIDEOUTILS_API __declspec(dllexport)
#else
#define KOYASHVIDEOUTILS_API __declspec(dllimport)
#endif

#include <windows.h>

HRESULT GetBitmapPixels(long width, long height, long bpp, long* pixels, BYTE* bitmapPixels);
HRESULT GetColourBitmapPixels(long width, long height, long bpp, long* pixels, BYTE* bitmapPixels);
HRESULT GetRGGBBayerBitmapPixels(long width, long height, long bpp, long* pixels, BYTE* bitmapPixels);
HRESULT GetMonochromePixelsFromBitmap(long width, long height, long bpp, long flipMode, HBITMAP* bitmap, long* pixels, int mode);
HRESULT GetColourPixelsFromBitmap(long width, long height, long bpp, long flipMode, HBITMAP* bitmap, long* pixels);
HRESULT GetRGGBBayerPixelsFromBitmap(long width, long height, long bpp, HBITMAP* bitmap, long* pixels);