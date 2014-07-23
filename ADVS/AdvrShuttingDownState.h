#ifndef ADVR_SHUTTING_DOWN_STATE
#define ADVR_SHUTTING_DOWN_STATE

#include "HcCommand.h"
#include "AdvrState.h"
#include "AdvrStateContext.h"

class AdvrShuttingDownState : public AdvrState
{
private:
    static AdvrShuttingDownState* s_pInstance;
	AdvrShuttingDownState();

	AdvrShuttingDownState(AdvrShuttingDownState const&){};
    AdvrShuttingDownState& operator=(AdvrShuttingDownState const&){};
  
public:    
	static AdvrShuttingDownState* Instance();
	
	void Initialise(AdvrStateContext *context);
	bool ReceiveCommand(AdvrStateContext *context, HcCommand* cmd);	
	void DisplayCurrentFrame(AdvrStateContext *context, unsigned short* pImageData);	
	void IncreaseFrameRate(AdvrStateContext *context);
	void DecreaseFrameRate(AdvrStateContext *context);	
};

#endif