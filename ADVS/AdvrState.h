#ifndef ADVR_STATE
#define ADVR_STATE

#include "HcCommand.h"
#include "AdvrStateContext.h"
#include "PtGreyImage.h"
#include "FrameExposureInfo.h"
#include "stdlib.h"

class AdvrStateContext;

class AdvrState
{
private:
	static unsigned char LOW_GAMMA[256];
	static unsigned char HI_GAMMA[256];
	static bool GammaArraysInitialized;
	
	void ChangeBetweenAutoAndTriggeredFrameRate(AdvrStateContext *context, int newFrameRate);
	
public:
	virtual void Initialise(AdvrStateContext *context);
	virtual void Finalise(AdvrStateContext *context);
	virtual bool ReceiveCommand(AdvrStateContext *context, HcCommand *cmd);	
	virtual bool ReceiveMessage(AdvrStateContext *context, HtccMessage *msg);
	virtual void DisplayCurrentFrame(AdvrStateContext *context, unsigned short* pImageData);
	virtual void ProcessCurrentFrame(AdvrStateContext *context, PtGreyImage* image, FrameExposureInfo* exposure);
	virtual void SystemInitialised(AdvrStateContext *context);
	virtual void IncreaseFrameRate(AdvrStateContext *context);
	virtual void DecreaseFrameRate(AdvrStateContext *context);

	void DisplayColourFrame(unsigned char* pImageData);
};

#endif