// OccuTimeCheck.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include <windows.h>

int getSysOpType()
{
	int ret = 0;
	NTSTATUS(WINAPI * RtlGetVersion)(LPOSVERSIONINFOEXW);
	OSVERSIONINFOEXW osInfo;

	*(FARPROC*)&RtlGetVersion = GetProcAddress(GetModuleHandleA("ntdll"), "RtlGetVersion");

	if (NULL != RtlGetVersion)
	{
		osInfo.dwOSVersionInfoSize = sizeof(osInfo);
		RtlGetVersion(&osInfo);
		ret = osInfo.dwMajorVersion;
	}
	return ret;
}

int _tmain(int argc, _TCHAR* argv[])
{
	HANDLE hConsole = GetStdHandle(STD_OUTPUT_HANDLE);
	SetConsoleTextAttribute(hConsole, 15);

	cout << "-----------------------------------------------\r\n";
	cout << "| OccuTimeCheck Utility v1.0 by Hristo Pavlov |\r\n";
	cout << "-----------------------------------------------\r\n\r\n";

	bool systemOK = true;

	if (isInvariantTSCAvailable() == 1)
	{
		SetConsoleTextAttribute(hConsole, 10);
		cout << "Your CPU supports Invariant TSC and can keep accurate time.\r\n";
	}
	else
	{
		systemOK = false;

		SetConsoleTextAttribute(hConsole, 12);
		cout << "Your CPU does not support Invariant TSC and cannot keep accurate time.\r\n";

		SetConsoleTextAttribute(hConsole, 14);
		const int buffSize = 65535;
		static char buffer[buffSize];
		if (GetEnvironmentVariableA("PROCESSOR_IDENTIFIER", buffer, buffSize))
		{
			cout << "Your CPU is: " << buffer << "\r\n";
		}
		if (isRDTSCSupported() == 1)
		{
			cout << "Your CPU supports RDTSCP\r\n";
		}
	}

	SetConsoleTextAttribute(hConsole, 15);

	int winVer = getSysOpType();
	if (winVer < 10)
	{
		systemOK = false;
		SetConsoleTextAttribute(hConsole, 12);
		cout << "You need Windows 10 or later to query the CPU time accurately.\r\n";
	}
	else
	{
		SetConsoleTextAttribute(hConsole, 10);
		cout << "Your Windows version can query the CPU time accurately.\r\n";
	}

	if (systemOK)
	{
		SetConsoleTextAttribute(hConsole, 15);

		cout << "\r\n\r\nPress Enter to continue ...";
		cin.get();

		SetConsoleTextAttribute(hConsole, 14);
		cout << "\r\n\r\nIn order to get accurate Windows timestamps on your system, make sure that:\r\n\r\n";
		cout << "\r\n   * If you use internet NTP synchronisation, then only use Meinberg NTP. If you are using a different way of synchronising to an external timing source then you need to prove that this is accurate.\r\n";
		cout << "\r\n   * Make sure that you are using fast internet connection such as ADSL2+\r\n";
		cout << "\r\n   * Check with the software vendor that your recording software uses the GetSystemTimePreciseAsFileTime() function to obtain Windows time for timestamping.\r\n";
		cout << "\r\n   * You still need to measure the acquisition delay of your system using hardware device like SEXTA or by obseving at least 10 lunar occultations.\r\n";
		cout << "\r\n   * Once you have measured your acquisition delay don't change any component (opitcal, mechanical or software). If you need to make a change then you need to measure the acquisition delay again.\r\n";
		cout << "\r\nIf you require additional help or clarifications, contact me at hristo_dpavlov@yahoo.com\r\n";
	}
	else
	{
		SetConsoleTextAttribute(hConsole, 12);
		cout << "\r\nYour system cannot be used for accurate Windows Time timestamping!";
		SetConsoleTextAttribute(hConsole, 14);
		cout << "\r\n\r\nIf you require additional help or clarifications, contact me at hristo_dpavlov@yahoo.com\r\n";
	}
	
	SetConsoleTextAttribute(hConsole, 15);
	cout << "\r\n\r\nPress Enter to exit";
	cin.get();

	return 0;
}

