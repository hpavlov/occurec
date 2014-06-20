#ifndef ADV_IMAGE_LAYOUT
#define ADV_IMAGE_LAYOUT

#include <stdio.h>
#include "utils.h"
#include "quicklz.h"

#include <map>
#include <string>

#include "Compressor.h"

using namespace std;
using std::string;

namespace AavLib
{
	class AavImageLayout 
	{

	private:
		unsigned char *SIGNS_MASK;
		map<string, string> m_LayoutTags;
		ImageBytesLayout m_BytesLayout;	
		unsigned char m_BitPix;

		int m_KeyFrameBytesCount;
		unsigned char *m_PrevFramePixels;
		unsigned char *m_PrevFramePixelsTemp;
		unsigned char *m_PixelArrayBuffer;
		unsigned char *m_SignsBytes;
		unsigned int m_MaxSignsBytesCount;
		unsigned int m_MaxPixelArrayLengthWithoutSigns;
		char* m_CompressedPixels;
		qlz_state_compress* m_StateCompress;
		Compressor* m_Lagarith16Compressor;
		
	public:
		unsigned char LayoutId;
		unsigned int Width;
		unsigned int Height;
	
		const char* Compression;
		bool IsDiffCorrLayout;
		bool IsNoImageLayout;
		int KeyFrame;
		
		int MaxFrameBufferSize;
		enum DiffCorrBaseFrame BaseFrameType;
	
	private:
		void GetDataBytes(unsigned char* currFramePixels, enum GetByteMode mode, unsigned int pixelsCRC32, unsigned char* signBytes, unsigned int *bytesCount);
		
		unsigned char* GetFullImageDiffCorrWithSignsDataBytes(unsigned char* currFramePixels, enum GetByteMode mode, unsigned int *bytesCount);
		unsigned char* GetFullImageDiffCorrNoSignsDataBytes(unsigned char* currFramePixels, enum GetByteMode mode, unsigned int *bytesCount);
		unsigned char* GetFullImageRawDataBytes(unsigned char* currFramePixels, unsigned int *bytesCount);
		unsigned char* GetFullImageRawDataBytes16(unsigned short* currFramePixels, unsigned int *bytesCount);
		
		void ResetBuffers();
		
	public:
		AavImageLayout(unsigned int width, unsigned int height, unsigned char bitPix, unsigned char layoutId, const char* layoutType, const char* compression, int keyFrame);
		~AavImageLayout();
		
		void AddOrUpdateTag(const char* tagName, const char* tagValue);
		unsigned char* GetDataBytes(unsigned char* currFramePixels, enum GetByteMode mode, unsigned int *bytesCount);
		unsigned char* GetDataBytes16(unsigned short* currFramePixels, enum GetByteMode mode, unsigned int *bytesCount);
		void WriteHeader(FILE* pfile);
		void StartNewDiffCorrSequence();
	};

};
#endif


