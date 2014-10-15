/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

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