#ifndef ADVIMAGESECTION_H
#define ADVIMAGESECTION_H

#include "aav_image_layout.h"
#include <stdio.h>
#include "utils.h"

#include <map>
#include <string>

using namespace std;
using std::string;

namespace AavLib
{

class AavImageSection {

	private:
		map<string, string> m_ImageTags;
		map<unsigned char, AavImageLayout*> m_ImageLayouts;
		unsigned char m_BitPix;
		
	private:
		unsigned int ComputePixelsCRC32(unsigned char* pixels);
		
	public:
		unsigned int Width;
		unsigned int Height;
		
	public:

		AavImageSection(unsigned int width, unsigned int height, unsigned char bitPix);
		~AavImageSection();
		AavImageLayout* AddImageLayout(unsigned char bitPix, unsigned char layoutId, const char* layoutType, const char* compression, int keyFrame);
		void AddOrUpdateTag(const char* tagName, const char* tagValue);
		void WriteHeader(FILE* pfile);

		unsigned char* GetDataBytes(unsigned char layoutId, unsigned char* currFramePixels, unsigned int *bytesCount, char* byteMode);		
		unsigned char* GetDataBytes16(unsigned char layoutId, unsigned short* currFramePixels, unsigned int *bytesCount, char* byteMode);

		void BeginFrame();
		int MaxFrameBufferSize();
		AavImageLayout* GetImageLayoutById(unsigned char layoutId);
};

}

#endif // ADVIMAGESECTION_H