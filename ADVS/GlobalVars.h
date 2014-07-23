#ifndef GLOBAL_VARS
#define GLOBAL_VARS

#include <queue>
#include "AdvrStateContext.h"
#include "HtccMessage.h"
#include "HcCommand.h"
#include "PtGreyImage.h"
#include "FrameExposureInfo.h"
#include "PtGreyCamera.h"
#include "HtccStateMachine.h"

extern CRITICAL_SECTION s_SyncRoot;
extern CRITICAL_SECTION s_SyncDisplayBytes;
extern CRITICAL_SECTION s_SyncSimulator;
extern CRITICAL_SECTION s_SyncCameraReset;
extern CRITICAL_SECTION s_SyncCameraData;
extern CRITICAL_SECTION s_RWRoot;
extern bool s_Running;

extern PtGreyCamera* s_Camera;

extern unsigned short* s_PixelInts;
extern unsigned short* s_pPixelInts;
extern bool s_HasPixelData;
extern char s_TimeStamp[32];
extern char s_GpsSatellites[2];
extern bool s_GpsAlmanacNotUpdated;
extern bool s_GpsNoFix;
extern bool s_GpsNoFixButConfidentTime;
extern char s_AlmanacOffset[4];
extern long s_CurrentDisplayImageFrameId;

// State Related
extern AdvrStateContext* s_AdvrState;
extern HtccStateMachine* s_HtccStateMachine;

extern char s_GeneralErrorMessage[255];


// TODO: Add buffers with Htcc messages and Hc messages
//       The buffers should be processed in the main thread (also display thread)
extern std::queue<HtccMessage*> s_HtccMessages;
extern std::queue<HcCommand*> s_UserCommands;
extern std::queue<PtGreyImage*> s_CameraImages;

extern std::queue<HtccMessage*> s_StartExposureMessages;
extern std::queue<FrameExposureInfo*> s_FrameExposureData;

extern CameraFrameRate CAMERA_FRAME_RATE_ON_STARTUP;
extern char* CFG_HTCC_DEVICE;

#endif