#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <windows.h>  // For the SIZE_T and PBYTE types

// Function declaration
VOID XorByInputKey(IN PBYTE pShellcode, IN SIZE_T sShellcodeSize, IN PBYTE bKey, IN SIZE_T sKeySize);

// Function to read file into a buffer
PBYTE ReadFileToBuffer(const char *filePath, SIZE_T *fileSize) {
    FILE *file = fopen(filePath, "rb");
    if (!file) {
        perror("Failed to open file");
        return NULL;
    }

    fseek(file, 0, SEEK_END);
    *fileSize = ftell(file);
    fseek(file, 0, SEEK_SET);

    PBYTE buffer = (PBYTE)malloc(*fileSize);
    if (!buffer) {
        perror("Failed to allocate memory");
        fclose(file);
        return NULL;
    }

    fread(buffer, 1, *fileSize, file);
    fclose(file);

    return buffer;
}

int main(int argc, char *argv[]) {
    if (argc != 3) {
        fprintf(stderr, "Usage: %s <file_path> <key>\n", argv[0]);
        return EXIT_FAILURE;
    }

    SIZE_T fileSize;
    PBYTE payload = ReadFileToBuffer(argv[1], &fileSize);
    if (!payload) {
        return EXIT_FAILURE;
    }

    // Get the key from the command line argument
    PBYTE key = (PBYTE)argv[2];
    SIZE_T keySize = strlen(argv[2]);

    // Decode the payload (since XOR is symmetric, the same function is used)
    XorByInputKey(payload, fileSize, key, keySize);

    // Print decoded payload
    printf("Decoded Data:\n");
    for (SIZE_T i = 0; i < fileSize; i++) {
        printf("%c", payload[i]);
    }
    printf("\n");

    free(payload);
    return EXIT_SUCCESS;
}

// Function definition
VOID XorByInputKey(IN PBYTE pShellcode, IN SIZE_T sShellcodeSize, IN PBYTE bKey, IN SIZE_T sKeySize) {
    for (size_t i = 0, j = 0; i < sShellcodeSize; i++, j++) {
        if (j >= sKeySize) {
            j = 0;
        }
        pShellcode[i] = pShellcode[i] ^ bKey[j];
    }
}
