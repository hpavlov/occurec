#ifndef ADVR_COMMAND
#define ADVR_COMMAND

enum HcUserCommand
{
	UserCommandStartRecording,
	UserCommandStopRecording,
	UserCommandToggleShowInfo,
	UserCommandChangeVideoSettings,
	UserCommandSystemShutdown,
	UserMessageVersionInfo
};

class HcCommand
{
public:
	// TODO: Add a timestamp?SDLK_r
	
	HcCommand(HcUserCommand command, double argument)
	{
		UserCommand = command;
		UserCommandArgument = argument;
	};
	
	HcUserCommand UserCommand;
	double UserCommandArgument;
};

#endif