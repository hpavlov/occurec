#ifndef ADVR_READY_STATE
#define ADVR_READY_STATE

#include "HcCommand.h"
#include "AdvrState.h"
#include "AdvrStateContext.h"


class AdvrReadyState : public AdvrState
{
private:
    static AdvrReadyState* s_pInstance;
	AdvrReadyState();

	AdvrReadyState(AdvrReadyState const&){};
    AdvrReadyState& operator=(AdvrReadyState const&){};
  
public:    
	static AdvrReadyState* Instance();
	
	void Initialise(AdvrStateContext *context);
	bool ReceiveCommand(AdvrStateContext *context, HcCommand* cmd);	
	void DisplayCurrentFrame(AdvrStateContext *context, unsigned short* pImageData);
	void IncreaseFrameRate(AdvrStateContext *context);
	void DecreaseFrameRate(AdvrStateContext *context);
};

#endif