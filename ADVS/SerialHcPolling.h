#ifndef SERIAL_HC_LOOP
#define SERIAL_HC_LOOP

enum HcAdvrCommandType
{
	HcAdvrCmdGetVersion
};

void PollForHcEvents();
void SendHcAdvrCommand(HcAdvrCommandType cmd);

#endif