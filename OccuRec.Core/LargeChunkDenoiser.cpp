#include "stdafx.h"
#include "LargeChunkDenoiser.h"
#include <stack>

long* s_CheckedPixels = NULL;
long s_ChunkDenoiseIndex;
long s_ChunkDenoiseWidth;
long s_ChunkDenoiseHeight;
long s_ChunkDenoiseMaxIndex;
long* s_ObjectPixelsIndex = NULL;
long s_ObjectPixelsCount;
long s_ObjectPixelsXFrom;
long s_ObjectPixelsXTo;
long s_ObjectPixelsYFrom;
long s_ObjectPixelsYTo;
long s_MaxLowerBoundNoiseChunkPixels;
long s_MinUpperBoundNoiseChunkPixels;
long s_MinLowerBoundNoiseChunkHeight;
long s_MaxUpperBoundNoiseChunkWidth;
std::stack<long> s_ObjectPixelsPath;

void SetObjectPixelsXFromTo(long pixelIndex)
{
	long width = pixelIndex % s_ChunkDenoiseWidth;
	long height = pixelIndex / s_ChunkDenoiseWidth;
	
	if (s_ObjectPixelsXFrom > width) s_ObjectPixelsXFrom = width;
	if (s_ObjectPixelsXTo < width) s_ObjectPixelsXTo = width;
	if (s_ObjectPixelsYFrom > height) s_ObjectPixelsYFrom = height;
	if (s_ObjectPixelsYTo < height) s_ObjectPixelsYTo = height;	
}

void EnsureChunkDenoiseBuffers(long width, long height)
{
	if (s_ChunkDenoiseWidth != width || s_ChunkDenoiseHeight != height)
	{
		if (NULL == s_CheckedPixels)
		{
			delete s_CheckedPixels;
			s_CheckedPixels = NULL;
		}
		
		if (NULL == s_ObjectPixelsIndex)
		{
			delete s_ObjectPixelsIndex;
			s_ObjectPixelsIndex = NULL;
		}
		
		s_ChunkDenoiseWidth = width;
		s_ChunkDenoiseHeight = height;
		s_ChunkDenoiseMaxIndex = width * height;		
		s_CheckedPixels = (long*)malloc(sizeof(long) * s_ChunkDenoiseMaxIndex);
		s_ObjectPixelsIndex = (long*)malloc(sizeof(long) * s_ChunkDenoiseMaxIndex);
		
		// The max noise chink part to be removed is 50% of the pixels in "1".
		// This value is determined experimentally and varied based on the area hight
       // Block(22x16), AreaHeight(20) -> '1' is 70 pixels (50% = 35 pixels)
		s_MaxLowerBoundNoiseChunkPixels = (long)(0.5 + 35.0 * height / 20);
		// The min noise chunk is 80% of a fully black square
		s_MinUpperBoundNoiseChunkPixels = (long)(0.5 + 0.8 * width * height);
		s_MinLowerBoundNoiseChunkHeight = (long)(0.5 + 0.5 * (height - 4));
		s_MaxUpperBoundNoiseChunkWidth = height + 4;
	}
}

#define UNCHECKED 0
#define WENT_UP 1
#define WENT_LEFT 2
#define WENT_RIGHT 3
#define WENT_DOWN 4
#define CHECKED 5

long FindNextObjectPixel(unsigned long* pixels, unsigned long onColour)
{
	while (s_ChunkDenoiseIndex < s_ChunkDenoiseMaxIndex - 1)
	{
		s_ChunkDenoiseIndex++;
		if (s_CheckedPixels[s_ChunkDenoiseIndex] == UNCHECKED)
		{
			if (pixels[s_ChunkDenoiseIndex] != onColour)
				s_CheckedPixels[s_ChunkDenoiseIndex] = CHECKED;
			else
				return s_ChunkDenoiseIndex;
		}
	}

	return -1;	
}

bool ProcessNoiseObjectPixel(unsigned long* pixels, long* pixelRef, unsigned long onColour)
{
	long pixel = *pixelRef;
	long x = pixel % s_ChunkDenoiseWidth;
	long y = pixel / s_ChunkDenoiseWidth;
	long width = s_ChunkDenoiseWidth;
	long nextPixel;

	if (s_CheckedPixels[pixel] == UNCHECKED)
	{
		nextPixel = (y - 1) * width + x;

		s_CheckedPixels[pixel] = WENT_UP;

		if (y > 0)
		{
			if (s_CheckedPixels[nextPixel] == UNCHECKED)
			{
				if (pixels[nextPixel] == onColour)
				{
					s_ObjectPixelsPath.push(nextPixel);
					*pixelRef = nextPixel;
					s_ObjectPixelsIndex[s_ObjectPixelsCount] = nextPixel;
					s_ObjectPixelsCount++;
					SetObjectPixelsXFromTo(nextPixel);
				}
				else
					s_CheckedPixels[nextPixel] = CHECKED;
			}
		}
		
		return true;
	}
	else if (s_CheckedPixels[pixel] == WENT_UP)
	{
		nextPixel = y * width + (x - 1);

		s_CheckedPixels[pixel] = WENT_LEFT;

		if (x > 0)
		{
			if (s_CheckedPixels[nextPixel] == UNCHECKED)
			{
				if (pixels[nextPixel] == onColour)
				{
					s_ObjectPixelsPath.push(nextPixel);
					*pixelRef = nextPixel;
					s_ObjectPixelsIndex[s_ObjectPixelsCount] = nextPixel;
					s_ObjectPixelsCount++;
					SetObjectPixelsXFromTo(nextPixel);
				}
				else
					s_CheckedPixels[nextPixel] = CHECKED;
			}
		}
		
		return true;
	}
	else if (s_CheckedPixels[pixel] == WENT_LEFT)
	{
		nextPixel = y * width + (x + 1);

		s_CheckedPixels[pixel] = WENT_RIGHT;

		if (x < width - 1)
		{
			if (s_CheckedPixels[nextPixel] == UNCHECKED)
			{
				if (pixels[nextPixel] == onColour)
				{
					s_ObjectPixelsPath.push(nextPixel);
					*pixelRef = nextPixel;
					s_ObjectPixelsIndex[s_ObjectPixelsCount] = nextPixel;
					s_ObjectPixelsCount++;
					SetObjectPixelsXFromTo(nextPixel);
				}
				else
					s_CheckedPixels[nextPixel] = CHECKED;
			}
		}
	
		return true;
	}
	else if (s_CheckedPixels[pixel] == WENT_RIGHT)
	{
		nextPixel = (y + 1) * width + x;

		s_CheckedPixels[pixel] = WENT_DOWN;

		if (y < s_ChunkDenoiseHeight - 1)
		{
			if (s_CheckedPixels[nextPixel] == UNCHECKED)
			{
				if (pixels[nextPixel] == onColour)
				{
					s_ObjectPixelsPath.push(nextPixel);
					*pixelRef = nextPixel;
					s_ObjectPixelsIndex[s_ObjectPixelsCount] = nextPixel;
					s_ObjectPixelsCount++;
					SetObjectPixelsXFromTo(nextPixel);
				}
				else
					s_CheckedPixels[nextPixel] = CHECKED;
			}
		}

		return true;
	}
	else if (s_CheckedPixels[pixel] >= WENT_DOWN)
	{
		if (s_ObjectPixelsPath.empty())
			return false;

		s_CheckedPixels[pixel] = CHECKED;

		nextPixel = s_ObjectPixelsPath.top();
		s_ObjectPixelsPath.pop();

		if (pixel == nextPixel && !s_ObjectPixelsPath.empty())
		{
			nextPixel = s_ObjectPixelsPath.top();
			s_ObjectPixelsPath.pop();
		}

		*pixelRef = nextPixel;
		return true;
	}
	else
		return false;	
}

bool CurrentObjectChunkIsNoise()
{
	return 
		s_ObjectPixelsCount < s_MaxLowerBoundNoiseChunkPixels ||
		s_ObjectPixelsCount > s_MinUpperBoundNoiseChunkPixels ||
		(s_ObjectPixelsYTo - s_ObjectPixelsYFrom) < s_MinLowerBoundNoiseChunkHeight ||
		(s_ObjectPixelsXTo - s_ObjectPixelsXFrom) > s_MaxUpperBoundNoiseChunkWidth;
}

void CheckAndRemoveNoiseObjectAsNecessary(unsigned long* pixels, long firstPixel, unsigned long onColour, unsigned long offColour)
{
	s_ObjectPixelsCount = 0;
	s_ObjectPixelsXFrom = 0xFFFF;
	s_ObjectPixelsXTo = 0;
	s_ObjectPixelsYFrom = 0xFFFF;
	s_ObjectPixelsYTo = 0;
	while(!s_ObjectPixelsPath.empty()) s_ObjectPixelsPath.pop();
	s_ObjectPixelsPath.push(firstPixel);

	long currPixel = firstPixel;

	s_ObjectPixelsIndex[s_ObjectPixelsCount] = firstPixel;
	s_ObjectPixelsCount++;
	SetObjectPixelsXFromTo(firstPixel);

	while (ProcessNoiseObjectPixel(pixels, &currPixel, onColour))
	{ }

	if (CurrentObjectChunkIsNoise())
	{
		for (int i = 0; i < s_ObjectPixelsCount; i++)
		{
			pixels[s_ObjectPixelsIndex[i]] = offColour;
		}
	}	
}

HRESULT LargeChunkDenoise(unsigned long* pixels, long width, long height, unsigned long onColour, unsigned long offColour)
{
	EnsureChunkDenoiseBuffers(width, height);
	
	s_ObjectPixelsCount = -1;
	s_ChunkDenoiseIndex = -1;
	
	memset(s_CheckedPixels, 0, s_ChunkDenoiseMaxIndex * sizeof(long));
	
	do
	{
		long nextObjectPixelId = FindNextObjectPixel(pixels, onColour);

		if (nextObjectPixelId != -1)
			CheckAndRemoveNoiseObjectAsNecessary(pixels, nextObjectPixelId, onColour, offColour);
		else
			break;

	} 
	while (true);	

	return S_OK;
}