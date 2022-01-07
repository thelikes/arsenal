// SOURCE: https://gist.github.com/benheise/ad7f2adb605a7ec216a506e821705a06
// quick and dirty C++ execution guardrail on executing process file name, inspired by @0xHop av evasion post
// https://0xhop.github.io/evasion/2021/04/19/evasion-pt1/

#include <stdio.h>
#include <psapi.h>


    // Hide the console window
    //ShowWindow (GetConsoleWindow(), SW_HIDE);
    
    // compare current and expected process name, exit if they don't match (ie executing in a sandbox)
    TCHAR szName[MAX_PATH];
    char ourName[] = "evade.exe";
    GetModuleBaseName(GetCurrentProcess(), GetModuleHandle(NULL), szName, MAX_PATH);

    // if they dont match, exit. otherwise get this party started
    if (strcmp(ourName, szName) != 0) {
    exit(STATUS_SUCCESS);
    }