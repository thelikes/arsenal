#include <Windows.h>
#include <unistd.h>
#include <stdio.h>
#include <stdbool.h>

/*
 * Guardrail to determine if a system is domain joined.
 * @thelikes
 */
 
bool isDomainJoined() {
    DWORD bufSize = MAX_PATH;
    TCHAR domainNameBuf[ MAX_PATH ];

    GetComputerNameEx( ComputerNameDnsDomain, domainNameBuf, &bufSize );

    //printf("name: %s", domainNameBuf);

    if (domainNameBuf[0] != '\0')
    {
        return true;
    }

    return false;
}

int main() {
    if (isDomainJoined()) {
        printf("true");
    } else {
        printf("false");
    }

    return 0;
}