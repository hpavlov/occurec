/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

#include "stdafx.h"
#include "PtGreyImage.h"
#include "AdvrStateContext.h"
#include "GlobalVars.h"
#include <cmath>

unsigned int intToBigEndianess(const unsigned int x)
{
	return  ( x >> 24 ) |  // Move first byte to the end,
			( ( x << 8 ) & 0x00FF0000 ) | // move 2nd byte to 3rd,
			( ( x >> 8 ) & 0x0000FF00 ) | // move 3rd byte to 2nd,
			( x << 24 ); // move last byte to start.
}
	
float intBitsToFloat(const unsigned int x)
{
    union {
       float f;  // assuming 32-bit IEEE 754 single-precision
       unsigned int i;    // assuming 32-bit 2's complement int
    } u;
        
    u.i = x;
    return u.f;
}

long long previousTimeStamp;

unsigned int PtGreyImage::LastNotTranslatedGainValue = -1;
unsigned int PtGreyImage::LastNotTranslatedShutterValue = -1;

PtGreyImage::PtGreyImage(unsigned short* rawData, FlyCapture2::ImageMetadata* metadata, FlyCapture2::TimeStamp* timestamp, unsigned char bpp)
{
	ImageData = (unsigned short*)malloc(IMAGE_STRIDE * sizeof(unsigned short));
	
	if (NULL != rawData)
		memcpy(const_cast<unsigned short*>(ImageData), rawData, IMAGE_STRIDE * sizeof(unsigned short));
		
	PixelBpp = bpp;

	
	if (metadata != NULL)
	{
		FrameId = metadata->embeddedFrameCounter;
		
		Timestamp = metadata->embeddedTimeStamp;
		Gain = metadata->embeddedGain & 0xFFF;
		Shutter = metadata->embeddedShutter & 0xFFF;
		Brightness = metadata->embeddedBrightness & 0xFFF;
		Exposure = metadata->embeddedExposure & 0xFFF;
		WhiteBalance = metadata->embeddedWhiteBalance;
		StrobePattern = metadata->embeddedStrobePattern;
		GPOIPinState = metadata->embeddedGPIOPinState;
		ROIPosition = metadata->embeddedROIPosition;

		// http://www.ptgrey.com/support/downloads/documents/flycapture/2d0.html
		// ulCycleCount 	The cycle time count. 0-7999. (1/8000ths of a second.)
		// ulCycleOffset 	The cycle offset. 0-3071 (1/3072ths of a cycle count.)
		// ulCycleSeconds 	The cycle time seconds. 0-127.
		double fractCycles = ((double)timestamp->cycleOffset) / 3072.0;
		double fractSec = (fractCycles + (double)timestamp->cycleCount) / 8000.0;
		CameraTicks = (long long)(((double)timestamp->cycleSeconds + fractSec) * 10000.0);
		
		//printf("Frame %d, Timestamp: %d.%d.%d : %lli Exp: %3.1f ms\n", FrameId, timestamp->cycleSeconds, timestamp->cycleCount, timestamp->cycleOffset, CameraTicks, (CameraTicks - previousTimeStamp) / 10.0);
		
		previousTimeStamp = CameraTicks;
		
		TranslateGain();
		TranslateShutter();
		TranslateBrightness();
		TranslateExposure();		
	}
	
	
#ifdef CAMERA_SIMULATOR
	FrameId = s_AdvrState->NumberQueuedFrames;
#endif
	
}

/*
void Convert()
{
    // declare a union of a floating point and unsigned long
    typedef union _AbsValueConversion
    {
       unsigned long ulValue;
       float fValue;
    } AbsValueConversion;

    float fShutter;
    AbsValueConversion regValue;

    // read the 32-bit hex value into the unsigned long member
    flycaptureGetCameraRegister(context, 0x918, & regValue.ulValue );
    fShutter = regValue.fValue; 	
}
*/

void PtGreyImage::TranslateBigFlea3Gain()
{
	/*
	GAIN: From 0 to 24 dB in steps of 0.046 db
	 * Values specific for: FL3-FW-03S3M

	dB    0xFFF
	00 -> 200 	00.000 dB
	01 -> 228	00.985 dB
	02 -> 257	02.006 dB
	03 -> 285	02.991 dB
	04 -> 314	04.012 dB
	05 -> 342	04.997 dB
	06 -> 370	05.982 dB
	07 -> 399	07.003 dB
	08 -> 427	07.988 dB
	09 -> 456	09.009 dB
	10 -> 484	09.994 dB
	11 -> 513	11.015 dB
	12 -> 541	12.000 dB
	13 -> 569	12.985 dB
	14 -> 598	14.006 dB
	15 -> 626	14.991 dB
	16 -> 655	16.012 dB
	17 -> 683	16.997 dB
	18 -> 711	17.982 dB
	19 -> 740	19.003 dB
	20 -> 768	19.988 dB
	21 -> 797	21.009 dB
	22 -> 825	21.994 dB
	23 -> 854	23.015 dB
	24 -> 882	24.000 dB
	25 -> 910	24.985 dB
	26 -> 939	26.006 dB
	27 -> 967	26.991 dB
	28 -> 996	28.012 dB
	*/
	
	switch(Gain)
	{
		case 200: GainFloat = 00.000; break;
		case 228: GainFloat = 00.985; break;
		case 257: GainFloat = 02.006; break;
		case 285: GainFloat = 02.991; break;
		case 314: GainFloat = 04.012; break;
		case 342: GainFloat = 04.997; break;
		case 370: GainFloat = 05.982; break;
		case 399: GainFloat = 07.003; break;
		case 427: GainFloat = 07.988; break;
		case 456: GainFloat = 09.009; break;
		case 484: GainFloat = 09.994; break;
		case 513: GainFloat = 11.015; break;
		case 541: GainFloat = 12.000; break;
		case 569: GainFloat = 12.985; break;
		case 598: GainFloat = 14.006; break;
		case 626: GainFloat = 14.991; break;
		case 655: GainFloat = 16.012; break;
		case 683: GainFloat = 16.997; break;
		case 711: GainFloat = 17.982; break;
		case 740: GainFloat = 19.003; break;
		case 768: GainFloat = 19.988; break;
		case 797: GainFloat = 21.009; break;
		case 825: GainFloat = 21.994; break;
		case 854: GainFloat = 23.015; break;
		case 882: GainFloat = 24.000; break;
		case 910: GainFloat = 24.985; break;
		case 939: GainFloat = 26.006; break;
		case 967: GainFloat = 26.991; break;
		case 996: GainFloat = 28.012; break;
		default: 
			if (PtGreyImage::LastNotTranslatedGainValue != Gain)
			{
				LastNotTranslatedGainValue = Gain;
				//printf("ERROR: Cannot translate embedded gain of %d.\n", LastNotTranslatedGainValue);
			}
			GainFloat = -100.000;
	}	
}

void PtGreyImage::TranslateSmallFlea3Gain()
{
	/*
	GAIN: From 0 to 24 dB in steps of 0.046 db
	 * Values specific for: FL3-FW-03S1M

	dB    0xFFF
	00 -> 80 	00.000 dB
	01 -> 108 	00.000 dB
	02 -> 137	00.985 dB
	03 -> 165	02.006 dB
	04 -> 194	02.991 dB
	05 -> 222	04.012 dB
	06 -> 250	04.997 dB
	07 -> 279	05.982 dB
	08 -> 307	07.003 dB
	09 -> 336	07.988 dB
	10 -> 364	09.009 dB
	11 -> 393	09.994 dB
	12 -> 421	11.015 dB
	13 -> 449	12.000 dB
	14 -> 478	12.985 dB
	15 -> 506	14.006 dB
	16 -> 535	14.991 dB
	17 -> 563	16.012 dB
	18 -> 591	17.982 dB
	19 -> 620	19.003 dB
	20 -> 648	19.988 dB
	21 -> 677	21.009 dB
	22 -> 705	21.994 dB
	23 -> 734	23.015 dB
	24 -> 762	24.000 dB
	25 -> 790	24.985 dB
	26 -> 819	26.006 dB
	27 -> 847	26.991 dB
	28 -> 876	28.012 dB
	29 -> 904	28.997 dB
	30 -> 932	29.982 dB
	31 -> 961	31.003 dB
	32 -> 989	31.988 dB
	33 ->1018	33.009 dB
	*/
	
	switch(Gain)
	{
		case  80: GainFloat = 00.000; break;
		case 108: GainFloat = 00.985; break;
		case 137: GainFloat = 02.006; break;
		case 165: GainFloat = 02.991; break;
		case 194: GainFloat = 04.012; break;
		case 222: GainFloat = 04.997; break;
		case 250: GainFloat = 05.982; break;
		case 279: GainFloat = 07.003; break;
		case 307: GainFloat = 07.988; break;
		case 336: GainFloat = 09.009; break;
		case 364: GainFloat = 09.994; break;
		case 393: GainFloat = 11.015; break;
		case 421: GainFloat = 12.000; break;
		case 449: GainFloat = 12.985; break;
		case 478: GainFloat = 14.006; break;
		case 506: GainFloat = 14.991; break;
		case 535: GainFloat = 16.012; break;
		case 563: GainFloat = 16.997; break;
		case 591: GainFloat = 17.982; break;
		case 620: GainFloat = 19.003; break;
		case 648: GainFloat = 19.988; break;
		case 677: GainFloat = 21.009; break;
		case 705: GainFloat = 21.994; break;
		case 734: GainFloat = 23.015; break;
		case 762: GainFloat = 24.000; break;
		case 790: GainFloat = 24.985; break;
		case 819: GainFloat = 26.006; break;
		case 847: GainFloat = 26.991; break;
		case 876: GainFloat = 28.012; break;
		case 904: GainFloat = 28.997; break;
		case 932: GainFloat = 29.982; break;
		case 961: GainFloat = 31.003; break;
		case 989: GainFloat = 31.988; break;
		case 1018: GainFloat = 33.009; break;
		default: 
			if (PtGreyImage::LastNotTranslatedGainValue != Gain)
			{
				LastNotTranslatedGainValue = Gain;
				//printf("ERROR: Cannot translate embedded gain of %d.\n", LastNotTranslatedGainValue);
			}
			GainFloat = -100.000;
	}		
}

void PtGreyImage::TranslateGrasshopperExpressGain()
{
	/*
	GAIN: From 0 to 24 dB in steps of 0.046 db
	 * Values specific for: GX-FW-28S5M

	dB    0xFFF
	00 -> 175 	00.000 dB
	01 -> 203	00.985 dB
	02 -> 232	02.006 dB
	03 -> 260	02.991 dB
	04 -> 289	04.012 dB
	05 -> 317	04.997 dB
	06 -> 345	05.982 dB
	07 -> 374	07.003 dB
	08 -> 402	07.988 dB
	09 -> 431	09.009 dB
	10 -> 459	09.994 dB
	11 -> 488	11.015 dB
	12 -> 516	12.000 dB
	13 -> 544	12.985 dB
	14 -> 573	14.006 dB
	15 -> 601	14.991 dB
	16 -> 630	16.012 dB
	17 -> 658	16.997 dB
	18 -> 686	17.982 dB
	19 -> 715	19.003 dB
	20 -> 743	19.988 dB
	21 -> 772	21.009 dB
	22 -> 800	21.994 dB
	23 -> 829	23.015 dB
	24 -> 857	24.000 dB
	25 -> 885	24.985 dB
	26 -> 914	26.006 dB
	27 -> 942	26.991 dB
	28 -> 971	28.012 dB
	29 -> 999	28.997 dB
	*/
	
	switch(Gain)
	{
		case 175: GainFloat = 00.000; break;
		case 203: GainFloat = 00.985; break;
		case 232: GainFloat = 02.006; break;
		case 260: GainFloat = 02.991; break;
		case 289: GainFloat = 04.012; break;
		case 317: GainFloat = 04.997; break;
		case 345: GainFloat = 05.982; break;
		case 374: GainFloat = 07.003; break;
		case 402: GainFloat = 07.988; break;
		case 431: GainFloat = 09.009; break;
		case 459: GainFloat = 09.994; break;
		case 488: GainFloat = 11.015; break;
		case 516: GainFloat = 12.000; break;
		case 544: GainFloat = 12.985; break;
		case 573: GainFloat = 14.006; break;
		case 601: GainFloat = 14.991; break;
		case 630: GainFloat = 16.012; break;
		case 658: GainFloat = 16.997; break;
		case 686: GainFloat = 17.982; break;
		case 715: GainFloat = 19.003; break;
		case 743: GainFloat = 19.988; break;
		case 772: GainFloat = 21.009; break;
		case 800: GainFloat = 21.994; break;
		case 829: GainFloat = 23.015; break;
		case 857: GainFloat = 24.000; break;
		case 885: GainFloat = 24.985; break;
		case 914: GainFloat = 26.006; break;
		case 942: GainFloat = 26.991; break;
		case 971: GainFloat = 28.012; break;
		case 999: GainFloat = 28.997; break;
		default: 
			if (PtGreyImage::LastNotTranslatedGainValue != Gain)
			{
				LastNotTranslatedGainValue = Gain;
				//printf("ERROR: Cannot translate embedded gain of %d.\n", LastNotTranslatedGainValue);
			}
			GainFloat = -100.000;
	}	
}
	
void PtGreyImage::TranslateGain()
{
	if (SYS_CAMERA_TYPE == BigFlea3)
		TranslateBigFlea3Gain();
	else if (SYS_CAMERA_TYPE == SmallFlea3)
		TranslateSmallFlea3Gain();
	else if (SYS_CAMERA_TYPE == GrasshopperExpress)
		TranslateGrasshopperExpressGain();
}

void PtGreyImage::TranslateShutter()
{
	if (SYS_CAMERA_TYPE == BigFlea3)
		TranslateBigFlea3Shutter();
	else if (SYS_CAMERA_TYPE == SmallFlea3)
		TranslateSmallFlea3Shutter();
	else if (SYS_CAMERA_TYPE == GrasshopperExpress)
		TranslateGrasshopperExpressShutter();
}

void PtGreyImage::TranslateBigFlea3Shutter()
{
	switch(Shutter)
	{
		case 1841: ShutterSeconds = 0.03333; break;
		case 2361: ShutterSeconds = 0.06667; break;
		case 2877: ShutterSeconds = 0.13333; break;
		case 3391: ShutterSeconds = 0.26667; break;
		case 3904: ShutterSeconds = 0.5333; break;
		case 425: ShutterSeconds = 1.0; break;
		case 487: ShutterSeconds = 1.28; break;
		case 781: ShutterSeconds = 2.0; break;
		case 1097: ShutterSeconds = 3.0; break;
		case 1293: ShutterSeconds = 4.0; break;
		case 1488: ShutterSeconds = 5.0; break;
		case 1461: ShutterSeconds = 6.0; break;
		case 1805: ShutterSeconds = 8.0; break;
		default: ShutterSeconds = 0.000;
	}	
}

void PtGreyImage::TranslateSmallFlea3Shutter()
{
	switch(Shutter)
	{
		case 1841: ShutterSeconds = 0.03333; break;
		case 2361: ShutterSeconds = 0.06667; break;
		case 2877: ShutterSeconds = 0.13333; break;
		case 3391: ShutterSeconds = 0.26667; break;
		case 3904: ShutterSeconds = 0.5333; break;
		case 425: ShutterSeconds = 1.0; break;
		case 487: ShutterSeconds = 1.28; break;
		case 781: ShutterSeconds = 2.0; break;
		case 1097: ShutterSeconds = 3.0; break;
		case 1293: ShutterSeconds = 4.0; break;
		case 1805: ShutterSeconds = 8.0; break;
		default: ShutterSeconds = 0.000;
	}
}

void PtGreyImage::TranslateGrasshopperExpressShutter()
{
	switch(Shutter)
	{
		case 1841: ShutterSeconds = 0.03333; break;
		case 2361: ShutterSeconds = 0.06667; break;
		case 2877: ShutterSeconds = 0.13333; break;
		case 3391: ShutterSeconds = 0.26667; break;
		case 3905: ShutterSeconds = 0.5333; break;  //was 3904
		case 269: ShutterSeconds = 1.0; break;      //was 425
		case 487: ShutterSeconds = 1.28; break;
		case 781: ShutterSeconds = 2.0; break;
		case 1097: ShutterSeconds = 3.0; break;
		case 1293: ShutterSeconds = 4.0; break;
		case 1488: ShutterSeconds = 5.0; break;
		case 1461: ShutterSeconds = 6.0; break;
		case 1805: ShutterSeconds = 8.0; break;
		default: ShutterSeconds = 0.000;
	}	
}

	
void PtGreyImage::TranslateBrightness()
{
	// TODO: Use fixed table to translate the value
	BrightnessFloat = Brightness / 10.0;
}

void PtGreyImage::TranslateExposure()
{
	// TODO: Use fixed table to translate the value
	ExposureFloat = intBitsToFloat(Exposure);
}

PtGreyImage::~PtGreyImage()
{	
	if (NULL != ImageData)
	{
		delete ImageData;
		ImageData = NULL;
	}
}