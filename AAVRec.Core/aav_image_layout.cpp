#include "StdAfx.h"

#include "aav_image_layout.h"
#include "utils.h"
#include "stdlib.h"
#include "math.h"
#include <stdio.h>
#include <assert.h>

#include "aav_profiling.h"

namespace AavLib
{

bool m_UsesCompression;
	
AavImageLayout::AavImageLayout(unsigned int width, unsigned int height, unsigned char layoutId, const char* layoutType, const char* compression, int keyFrame)
{	
	LayoutId = layoutId;
	Width = width;
	Height = height;
	KeyFrame = keyFrame;
	IsDiffCorrLayout = false;
	
	SIGNS_MASK = new unsigned char(8);
	SIGNS_MASK[0] = 0x01;
	SIGNS_MASK[1] = 0x02;
	SIGNS_MASK[2] = 0x04;
	SIGNS_MASK[3] = 0x08;
	SIGNS_MASK[4] = 0x10;
	SIGNS_MASK[5] = 0x20;
	SIGNS_MASK[6] = 0x40;
	SIGNS_MASK[7] = 0x80;
	
	MaxFrameBufferSize = Width * Height * 4 + 1 + 4 + + 16; //NOTE: The buufer is for 32bit data!! should be Width * Height rather than Width * Height * 4

	AddOrUpdateTag("DATA-LAYOUT", layoutType);
	AddOrUpdateTag("SECTION-DATA-COMPRESSION", compression);

	Compression = new char[strlen(compression) + 1];
	strcpy(const_cast<char*>(Compression), compression);
	m_UsesCompression = 0 != strcmp(compression, "UNCOMPRESSED");
	
	if (keyFrame > 0)
	{
		char keyFrameStr [5];
		snprintf(keyFrameStr, 5, "%d", keyFrame);
		AddOrUpdateTag("DIFFCODE-KEY-FRAME-FREQUENCY", keyFrameStr);		
		AddOrUpdateTag("DIFFCODE-BASE-FRAME", "KEY-FRAME");		
	}
	
	m_MaxSignsBytesCount = (unsigned int)ceil(Width * Height / 8.0) + 1;
	
	m_MaxPixelArrayLengthWithoutSigns = 1 + 4 + 2 * Width * Height;
		
	m_KeyFrameBytesCount = Width * Height * sizeof(unsigned char);
	
	m_PrevFramePixels = NULL;
	m_PrevFramePixelsTemp = NULL;
	m_PixelArrayBuffer = NULL;
	m_CompressedPixels = NULL;	
	m_StateCompress = NULL;
	
	m_PixelArrayBuffer = (unsigned char*)malloc(m_MaxPixelArrayLengthWithoutSigns + m_MaxSignsBytesCount);
	m_PrevFramePixels = (unsigned char*)malloc(m_KeyFrameBytesCount);		
	memset(m_PrevFramePixels, 0, m_KeyFrameBytesCount);
	
	m_SignsBytes = (unsigned char*)malloc(m_MaxSignsBytesCount);
	
	m_PrevFramePixelsTemp = (unsigned char*)malloc(m_KeyFrameBytesCount);	
	m_CompressedPixels = (char*)malloc(m_MaxPixelArrayLengthWithoutSigns + m_MaxSignsBytesCount + 401);
	
	m_StateCompress = (qlz_state_compress *)malloc(sizeof(qlz_state_compress));
}

AavImageLayout::~AavImageLayout()
{
	ResetBuffers();	
}

void AavImageLayout::ResetBuffers()
{
	if (NULL != m_PrevFramePixels)
		delete m_PrevFramePixels;		

	if (NULL != m_PrevFramePixelsTemp)
		delete m_PrevFramePixelsTemp;		

	if (NULL != m_PixelArrayBuffer)
		delete m_PixelArrayBuffer;

	if (NULL != m_CompressedPixels)
		delete m_CompressedPixels;
	
	if (NULL != m_StateCompress)
		delete m_StateCompress;	
	
	if (NULL != m_SignsBytes)
		delete m_SignsBytes;

	m_PrevFramePixels = NULL;
	m_PrevFramePixelsTemp = NULL;
	m_PixelArrayBuffer = NULL;
	m_CompressedPixels = NULL;	
	m_StateCompress = NULL;
	m_SignsBytes = NULL;
}


void AavImageLayout::StartNewDiffCorrSequence()
{
   //TODO: Reset the prev frame buffer (do we need to do anything??)
}

void AavImageLayout::AddOrUpdateTag(const char* tagName, const char* tagValue)
{
	map<string, string>::iterator curr = m_LayoutTags.begin();
	while (curr != m_LayoutTags.end()) 
	{
		const char* existingTagName = curr->first.c_str();	
		
		if (0 == strcmp(existingTagName, tagName))
		{
			m_LayoutTags.erase(curr);
			break;
		}
		
		curr++;
	}
	
	m_LayoutTags.insert(make_pair(string(tagName), string(tagValue)));
	
	if (0 == strcmp("DIFFCODE-BASE-FRAME", tagName))
	{
		if (0 == strcmp("KEY-FRAME", tagValue))
		{
			BaseFrameType = DiffCorrKeyFrame;
		}
		else if (0 == strcmp("PREV-FRAME", tagValue))
		{
			BaseFrameType = DiffCorrPrevFrame;
		}
	}
	
	if (0 == strcmp("DATA-LAYOUT", tagName))
	{
		m_BytesLayout = FullImageRaw;
		if (0 == strcmp("FULL-IMAGE-DIFFERENTIAL-CODING", tagValue)) m_BytesLayout = FullImageDiffCorrWithSigns;
		if (0 == strcmp("FULL-IMAGE-DIFFERENTIAL-CODING-NOSIGNS", tagValue)) m_BytesLayout = FullImageDiffCorrNoSigns;
		IsDiffCorrLayout = m_BytesLayout == FullImageDiffCorrWithSigns || m_BytesLayout == FullImageDiffCorrNoSigns;
	}	
}


void AavImageLayout::WriteHeader(FILE* pFile)
{
	unsigned char buffChar;
	
	buffChar = 1;
	fwrite(&buffChar, 1, 1, pFile); /* Version */

	unsigned char bpp = 8;
	fwrite(&bpp, 1, 1, pFile);	

	
	buffChar = (unsigned char)m_LayoutTags.size();
	fwrite(&buffChar, 1, 1, pFile);
	
	map<string, string>::iterator curr = m_LayoutTags.begin();
	while (curr != m_LayoutTags.end()) 
	{
		char* tagName = const_cast<char*>(curr->first.c_str());	
		WriteString(pFile, tagName);
		
		char* tagValue = const_cast<char*>(curr->second.c_str());	
		WriteString(pFile, tagValue);
		
		curr++;
	}		
}


unsigned int WordSignMask(int bit)
{
	switch (bit + 1)	
	{
		case 1:
			return 0x00000001;
			break;
		case 2:
			return 0x00000002;
			break;
		case 3:
			return 0x00000004;
			break;
		case 4:
			return 0x00000008;
			break;
		case 5:
			return 0x00000010;
			break;
		case 6:
			return 0x00000020;
			break;
		case 7:
			return 0x00000040;
			break;
		case 8:
			return 0x00000080;
			break;
		case 9:
			return 0x00000100;
			break;
		case 10:
			return 0x00000200;
			break;
		case 11:
			return 0x00000400;
			break;
		case 12:
			return 0x00000800;
			break;
		case 13:
			return 0x00001000;
			break;
		case 14:
			return 0x00002000;
			break;
		case 15:
			return 0x00004000;
			break;
		case 16:
			return 0x00008000;
			break;			
		case 17:
			return 0x00010000;
			break;
		case 18:
			return 0x00020000;
			break;
		case 19:
			return 0x00040000;
			break;
		case 20:
			return 0x00080000;
			break;
		case 21:
			return 0x00100000;
			break;
		case 22:
			return 0x00200000;
			break;
		case 23:
			return 0x00400000;
			break;
		case 24:
			return 0x00800000;
			break;
		case 25:
			return 0x01000000;
			break;
		case 26:
			return 0x02000000;
			break;
		case 27:
			return 0x04000000;
			break;
		case 28:
			return 0x08000000;
			break;
		case 29:
			return 0x10000000;
			break;
		case 30:
			return 0x20000000;
			break;
		case 31:
			return 0x40000000;
			break;
		case 32:
			return 0x80000000;
			break;
		default:
			return 0x00000000;
			break;			
	}
}


unsigned char* AavImageLayout::GetDataBytes(unsigned char* currFramePixels, enum GetByteMode mode, unsigned int *bytesCount)
{
	unsigned char* bytesToCompress;
	
	if (m_BytesLayout == FullImageDiffCorrWithSigns)
	{
		bytesToCompress = GetFullImageDiffCorrWithSignsDataBytes(currFramePixels, mode, bytesCount);
	}
	else if (m_BytesLayout == FullImageDiffCorrNoSigns)
	{
		bytesToCompress = GetFullImageDiffCorrNoSignsDataBytes(currFramePixels, mode, bytesCount);
	}
	else if (m_BytesLayout == FullImageRaw)
		bytesToCompress = GetFullImageRawDataBytes(currFramePixels, bytesCount);

	
	if (0 == strcmp(Compression, "QUICKLZ"))
	{
		unsigned int frameSize = 0;

		// compress and write result 
		size_t len2 = qlz_compress(bytesToCompress, m_CompressedPixels, *bytesCount, m_StateCompress); 		

		DebugViewPrint(L"Compressed to %d %%\r\n",  100 * len2 / *bytesCount);

		*bytesCount = len2;
		
		return (unsigned char*)(m_CompressedPixels);
	}
	else if (0 == strcmp(Compression, "UNCOMPRESSED"))
	{
		return bytesToCompress;
	}
	
		
	return NULL;
}

unsigned char* AavImageLayout::GetFullImageRawDataBytes(unsigned char* currFramePixels, unsigned int *bytesCount)
{
	int buffLen = Width * Height;
	
	memcpy(&m_PixelArrayBuffer[0], &currFramePixels[0], buffLen);
	

	*bytesCount = buffLen;
	return m_PixelArrayBuffer;
}

unsigned char* AavImageLayout::GetFullImageDiffCorrNoSignsDataBytes(unsigned char* currFramePixels, enum GetByteMode mode, unsigned int *bytesCount)
{
	bool isKeyFrame = mode == KeyFrameBytes;
	bool diffCorrFromPrevFramePixels = isKeyFrame || this->BaseFrameType == DiffCorrPrevFrame;
	
	if (diffCorrFromPrevFramePixels)
	{
		// STEP1 from maintaining the old pixels for DiffCorr
		if (mode == DiffCorrBytes)
			memcpy(&m_PrevFramePixelsTemp[0], &currFramePixels[0], m_KeyFrameBytesCount);
		else
			memcpy(&m_PrevFramePixels[0], &currFramePixels[0], m_KeyFrameBytesCount);
	}	

    // NOTE: The CRC computation is a huge overhead and is currently disabled
	//unsigned int pixelsCrc = ComputePixelsCRC32(currFramePixels);
	unsigned int pixelsCrc = 0;
	
	if (mode == KeyFrameBytes)
	{
		*bytesCount = 0;
	}
	else if (mode == DiffCorrBytes)
	{					
		*bytesCount = 0;
		
		unsigned char* pCurrFramePixels = (unsigned char*)currFramePixels;
		unsigned char* pPrevFramePixels = (unsigned char*)m_PrevFramePixels;

		memcpy(&m_PixelArrayBuffer[5 + Width*Height], &currFramePixels[0], Width * Height);

		for (int j = 0; j < Height; ++j)
		{
			for (int i = 0; i < Width; ++i)
			{
				unsigned long charCurr = (unsigned char)*pCurrFramePixels;
				unsigned long charOld = (unsigned char)*pPrevFramePixels;
				
				unsigned char charDiff = (unsigned char)((charCurr - charOld) & 0xFF);

				*pCurrFramePixels = charDiff;
				
				pCurrFramePixels++;
				pPrevFramePixels++;
			}
		}
	}

	if (diffCorrFromPrevFramePixels && mode == DiffCorrBytes)
		// STEP2 from maintaining the old pixels for DiffCorr
		memcpy(&m_PrevFramePixels[0], &m_PrevFramePixelsTemp[0], m_KeyFrameBytesCount);
	
	GetDataBytes(currFramePixels, mode, pixelsCrc, NULL, bytesCount);

	*bytesCount+= Width * Height;

	return m_PixelArrayBuffer;
}

unsigned char* AavImageLayout::GetFullImageDiffCorrWithSignsDataBytes(unsigned char* currFramePixels, enum GetByteMode mode, unsigned int *bytesCount)
{
	bool isKeyFrame = mode == KeyFrameBytes;
	bool diffCorrFromPrevFramePixels = isKeyFrame || this->BaseFrameType == DiffCorrPrevFrame;
	
	if (diffCorrFromPrevFramePixels)
	{
		// STEP1 from maintaining the old pixels for DiffCorr
		if (mode == DiffCorrBytes)
			memcpy(&m_PrevFramePixelsTemp[0], &currFramePixels[0], m_KeyFrameBytesCount);
		else
			memcpy(&m_PrevFramePixels[0], &currFramePixels[0], m_KeyFrameBytesCount);
	}	

    // NOTE: The CRC computation is a huge overhead and is currently disabled
	//unsigned int pixelsCrc = ComputePixelsCRC32(currFramePixels);
	unsigned int pixelsCrc = 0;
	
	if (mode == KeyFrameBytes)
	{
		*bytesCount = 0;		
	}
	else if (mode == DiffCorrBytes)
	{					
		*bytesCount = 0;

		memset(m_SignsBytes, 0, m_MaxSignsBytesCount);
		
		unsigned char* pCurrFramePixels = (unsigned char*)currFramePixels;
		unsigned char* pPrevFramePixels = (unsigned char*)m_PrevFramePixels;
		//unsigned char* pOuputPixelsFrameCopy = (unsigned char*)(m_PixelArrayBuffer + 5 + Width*Height);

		memcpy(&m_PixelArrayBuffer[5 + Width*Height + Width * Height / 8], &currFramePixels[0], Width * Height);

		for (int y = 0; y < Height; ++y)
		{
			//char dbg[6400];

			for (int x = 0; x < Width; ++x)
			{
				unsigned long charCurr = (unsigned char)*pCurrFramePixels;
				unsigned long charOld = (unsigned char)*pPrevFramePixels;
				
				unsigned long charDiffLong; 
				int signsBit = (x + y * Width) % 8;
				int signsIndex = (x + y * Width) / 8;
				bool signIsNegative;
				if (charCurr > charOld)
				{
					charDiffLong = charCurr - charOld;
					signIsNegative = false;
				}
				else
				{
					charDiffLong = charOld - charCurr;
					signIsNegative = true;
				}

				unsigned char charDiff = (unsigned char)(charDiffLong & 0xFF);
				
				if (signIsNegative)
					m_SignsBytes[signsIndex] = m_SignsBytes[signsIndex] | SIGNS_MASK[signsBit];

				//sprintf(&dbg[i*4], "%03d ", charDiff);

				*pCurrFramePixels = charDiff;
				//*pOuputPixelsFrameCopy = charCurr;
				
				pCurrFramePixels++;
				pPrevFramePixels++;
				//pOuputPixelsFrameCopy++;
			}
			
			//dbg[Width * 4 + 1] = '\0';
			//std::wstring wc(4096, L'#');
			//mbstowcs(&wc[0], dbg, 4096);
			//DebugViewPrint(&wc[0]);
		}

		*bytesCount = Width * Height / 8;
	}

	if (diffCorrFromPrevFramePixels && mode == DiffCorrBytes)
		// STEP2 from maintaining the old pixels for DiffCorr
		memcpy(&m_PrevFramePixels[0], &m_PrevFramePixelsTemp[0], m_KeyFrameBytesCount);
	
	GetDataBytes(currFramePixels, mode, pixelsCrc, m_SignsBytes, bytesCount);

	//*bytesCount += Width * Height; // The second frame copied at the end 

	*bytesCount+= Width * Height;

	return m_PixelArrayBuffer;
}

void AavImageLayout::GetDataBytes(unsigned char* pixels, enum GetByteMode mode, unsigned int pixelsCRC32, unsigned char* signBytes, unsigned int *bytesCount)
{
	// Flags: 0 - no key frame used, 1 - key frame follows, 2 - diff corr data follows
	bool isKeyFrame = mode == KeyFrameBytes;
	bool noKeyFrameUsed = mode == Normal;
	bool isDiffCorrFrame = mode == DiffCorrBytes;
	
	int signsBytesCnt = *bytesCount;
	
	unsigned int bytesCounter = *bytesCount;
	
	m_PixelArrayBuffer[0] = noKeyFrameUsed ? (unsigned char)0 : (isKeyFrame ? (unsigned char)1 : (unsigned char)2);
	bytesCounter++;

	if (signsBytesCnt > 0)
	{
		memcpy(&m_PixelArrayBuffer[1], &signBytes[0], signsBytesCnt);		
	}	

	for (int y = 0; y < Height; ++y)
	{
		for (int x = 0; x < Width; ++x)
		{
			unsigned char pixel = (unsigned char)(pixels[x + y * Width]);

			m_PixelArrayBuffer[bytesCounter] = pixel;
			bytesCounter++;
		}
	}

	m_PixelArrayBuffer[bytesCounter] = (unsigned char)(pixelsCRC32 & 0xFF);
	m_PixelArrayBuffer[bytesCounter + 1] = (unsigned char)((pixelsCRC32 >> 8) & 0xFF);
	m_PixelArrayBuffer[bytesCounter + 2] = (unsigned char)((pixelsCRC32 >> 16) & 0xFF);
	m_PixelArrayBuffer[bytesCounter + 3] = (unsigned char)((pixelsCRC32 >> 24) & 0xFF);
	*bytesCount = bytesCounter + 4;

}


}
