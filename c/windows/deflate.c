#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <zlib.h>

#define CHUNK 16384

typedef struct {
    unsigned char *data;
    size_t size;
} DecompressedData;

DecompressedData decompress(const unsigned char *source, size_t sourceSize, size_t bufferSize) {
    int ret;
    unsigned have;
    z_stream strm;
    unsigned char *out = malloc(bufferSize);
    size_t totalSize = 0;
    size_t allocatedSize = bufferSize;
    unsigned char *result = malloc(allocatedSize);

    if (out == NULL || result == NULL) {
        fprintf(stderr, "Memory allocation error\n");
        exit(1);
    }

    // Allocate inflate state
    strm.zalloc = Z_NULL;
    strm.zfree = Z_NULL;
    strm.opaque = Z_NULL;
    strm.avail_in = 0;
    strm.next_in = Z_NULL;
    ret = inflateInit2(&strm, 16 + MAX_WBITS); // 16 + MAX_WBITS for gzip decoding
    if (ret != Z_OK) {
        fprintf(stderr, "inflateInit error: %d\n", ret);
        exit(1);
    }

    // Decompress until deflate stream ends or end of buffer
    strm.avail_in = sourceSize;
    strm.next_in = (unsigned char *)source;

    do {
        strm.avail_out = bufferSize;
        strm.next_out = out;
        ret = inflate(&strm, Z_NO_FLUSH);
        switch (ret) {
            case Z_NEED_DICT:
                ret = Z_DATA_ERROR;     // and fall through
            case Z_DATA_ERROR:
            case Z_MEM_ERROR:
                (void)inflateEnd(&strm);
                fprintf(stderr, "inflate error: %d\n", ret);
                exit(1);
        }
        have = bufferSize - strm.avail_out;
        totalSize += have;
        if (totalSize > allocatedSize) {
            allocatedSize *= 2;
            result = realloc(result, allocatedSize);
            if (result == NULL) {
                fprintf(stderr, "Memory allocation error\n");
                exit(1);
            }
        }
        memcpy(result + totalSize - have, out, have);

    } while (ret != Z_STREAM_END);

    // Clean up
    (void)inflateEnd(&strm);
    free(out);

    if (ret != Z_STREAM_END) {
        fprintf(stderr, "File decompression failed\n");
        exit(1);
    }

    DecompressedData decompressedData = {result, totalSize};
    return decompressedData;
}

int main(int argc, char **argv) {
    if (argc != 3) {
        fprintf(stderr, "Usage: %s <input file> <output file>\n", argv[0]);
        return 1;
    }

    FILE *source = fopen(argv[1], "rb");
    if (source == NULL) {
        perror("Failed to open input file");
        return 1;
    }

    fseek(source, 0, SEEK_END);
    size_t sourceSize = ftell(source);
    fseek(source, 0, SEEK_SET);
    unsigned char *sourceBuffer = malloc(sourceSize);
    if (sourceBuffer == NULL) {
        perror("Memory allocation error");
        fclose(source);
        return 1;
    }
    fread(sourceBuffer, 1, sourceSize, source);
    fclose(source);

    DecompressedData decompressedData = decompress(sourceBuffer, sourceSize, CHUNK);
    free(sourceBuffer);

    FILE *dest = fopen(argv[2], "wb");
    if (dest == NULL) {
        perror("Failed to open output file");
        free(decompressedData.data);
        return 1;
    }
    fwrite(decompressedData.data, 1, decompressedData.size, dest);
    fclose(dest);
    free(decompressedData.data);

    printf("File successfully decompressed.\n");
    return 0;
}
