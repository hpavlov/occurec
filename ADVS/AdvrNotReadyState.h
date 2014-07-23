#ifndef ADVR_NOT_READY_STATE
#define ADVR_NOT_READY_STATE

#include "HcCommand.h"
#include "HtccMessage.h"
#include "AdvrState.h"
#include "AdvrStateContext.h"

class AdvrNotReadyState : public AdvrState
{
private:
    static AdvrNotReadyState* s_pInstance;
	static unsigned char* s_LogoPixels;
	
	AdvrNotReadyState();

	AdvrNotReadyState(AdvrNotReadyState const&){};
    AdvrNotReadyState& operator=(AdvrNotReadyState const&){};
  
public:    
	static AdvrNotReadyState* Instance();
	
	void Initialise(AdvrStateContext* context);
	bool ReceiveCommand(AdvrStateContext *context, HcCommand* cmd);	
	bool ReceiveMessage(AdvrStateContext *context, HtccMessage *msg);
	void DisplayCurrentFrame(AdvrStateContext *context, unsigned short* pImageData);
	void ProcessCurrentFrame(AdvrStateContext *context, PtGreyImage* image, FrameExposureInfo* exposure);
	void SystemInitialised(AdvrStateContext *context);	
	void IncreaseFrameRate(AdvrStateContext *context);
	void DecreaseFrameRate(AdvrStateContext *context);	
};

#endif