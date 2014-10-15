/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

#ifndef ADVR_RECORDING_STATE
#define ADVR_RECORDING_STATE

#include "HcCommand.h"
#include "AdvrState.h"
#include "AdvrStateContext.h"
#include "PtGreyImage.h"

void* DoRecorderLoop(void* arg);

class AdvrRecordingState : public AdvrState
{
private:
    static AdvrRecordingState* s_pInstance;
	AdvrRecordingState();
	
	int m_LastExpectedFrameId;
	long m_FirstRecordedFrameTimestamp;

	AdvrRecordingState(AdvrRecordingState const&){};
    AdvrRecordingState& operator=(AdvrRecordingState const&){};
	
	unsigned int TAGID_SystemTime;
	unsigned int TAGID_TrackedGPSSatellites;
	unsigned int TAGID_GPSAlmanacStatus;
	unsigned int TAGID_GPSAlmanacOffset;
	unsigned int TAGID_GPSFixStatus;
	unsigned int TAGID_Gain;
	unsigned int TAGID_Shutter;
	unsigned int TAGID_Offset;
	unsigned int TAGID_Gamma;
	unsigned int TAGID_GPSFix;
	unsigned int TAGID_UserCommand;
	unsigned int TAGID_SystemError;
	unsigned int TAGID_VideoCameraFrameId;
	unsigned int TAGID_HardwareTimerFrameId;
	
public:    
	static AdvrRecordingState* Instance();
	
	void Initialise(AdvrStateContext *context);
	void Finalise(AdvrStateContext *context);
	bool ReceiveCommand(AdvrStateContext *context, HcCommand* cmd);	
	void DisplayCurrentFrame(AdvrStateContext *context, unsigned short* pImageData);
	void ProcessCurrentFrame(AdvrStateContext *context, PtGreyImage* image, FrameExposureInfo* exposure);
	void IncreaseFrameRate(AdvrStateContext *context);
	void DecreaseFrameRate(AdvrStateContext *context);
};

#endif