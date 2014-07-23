#ifndef SERIAL_HTCC_LOOP
#define SERIAL_HTCC_LOOP

enum HtccCommandType
{
	HtccCmdGetVersion,
	HtccCmdStartOneSecTriggeredExposures,
	HtccCmdStartTwoSecTriggeredExposures,
	HtccCmdStartFourSecTriggeredExposures,
	HtccCmdStartEightSecTriggeredExposures,
	HtccCmdStopTriggeredExposures,
	HtccCmdZeroFrameCounter,
	HtccCmdResetErrorCounter,
	HtccCmdGetGpsPosition,
	HtccCmdGetGpsHeight,
	HtccCmdGetGpsAlmanachUpdateTime,
	HtccCmdStartDateStamps,
	HtccCmdTriggerSingleImageExposure
};

#define LARGE_PORT_BUFFER_SIZE 1024

void* DoSerialHtccLoop(void* arg);
void SendHtccCommand(HtccCommandType cmd);
void ReadHtccPacketBytesIfAvailable();
unsigned char* DeQueueHtccPacket();

#endif