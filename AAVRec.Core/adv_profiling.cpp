#include "StdAfx.h"
#include "adv_profiling.h"
#include <stdio.h>
#include <stdlib.h>
#include <cstring>
#include "windows.h"

int advfclose(FILE* file)
{
	return fclose(file);
}

FILE* advfopen(const char* fileName, const char* modes)
{
	FILE* file = fopen(fileName, modes);
	return file;
}

size_t advfwrite(const void* pData, size_t size, size_t count, FILE* file)
{
	size_t written = fwrite(pData, size, count, file);
	return written;
}

void advfgetpos64(FILE* file, __int64* pos)
{
	*pos = _ftelli64(file);
}

int advfsetpos64(FILE* file, const __int64* pos)
{
	int rv = _fseeki64(file, *pos, SEEK_SET);
	return rv;
}

int advfseek(FILE* file, long int off, int whence)
{
	int rv = fseek(file, off, whence);
	return rv;
}

int advfflush(FILE* file)
{
	int rv = fflush(file);
	return rv;
}