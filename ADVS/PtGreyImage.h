#ifndef FLEA3_IAMEG
#define FLEA3_IAMEG

#include "FlyCapture2.h"

class PtGreyImage
{
public:
	PtGreyImage(unsigned short* rawData, FlyCapture2::ImageMetadata* metadata, FlyCapture2::TimeStamp* timestamp, unsigned char bpp);
	~PtGreyImage();
		
	const unsigned short* ImageData;
	unsigned char PixelBpp;
	
private:
	void TranslateGain();
	void TranslateShutter();
	void TranslateBrightness();
	void TranslateExposure();
	
	void TranslateBigFlea3Gain();
	void TranslateSmallFlea3Gain();
	void TranslateGrasshopperExpressGain();
	
	void TranslateBigFlea3Shutter();
	void TranslateSmallFlea3Shutter();
	void TranslateGrasshopperExpressShutter();
	
	static unsigned int LastNotTranslatedGainValue;
	static unsigned int LastNotTranslatedShutterValue;
	
public:
	// Embedded Image Information
	unsigned int Timestamp;
	unsigned int Gain;
	float GainFloat;
	unsigned int Shutter;
	float ShutterSeconds;
	unsigned int Brightness;
	float BrightnessFloat;
	unsigned int Exposure;
	float ExposureFloat;
	unsigned int WhiteBalance;
	unsigned int FrameId;
	unsigned int StrobePattern;
	unsigned int GPOIPinState;
	unsigned int ROIPosition;
	
	long long CameraTicks;
};

#endif