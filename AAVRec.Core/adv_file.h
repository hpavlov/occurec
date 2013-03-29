#ifndef ADVFILE_H
#define ADVFILE_H

#include "adv_image_layout.h"
#include "adv_image_section.h"
#include "adv_status_section.h"
#include "adv_frames_index.h"

#include <map>
#include <string>
#include <stdio.h>
#include "quicklz.h"

using namespace std;
using std::string;

namespace AavLib
{

	class AavFile {
		public:
			AavLib::AavImageSection* ImageSection;
			AavLib::AavStatusSection* StatusSection;
			
		protected:
			AavLib::AavFramesIndex* m_Index;
			map<const char*, string> m_FileTags;			
			
		private:
			AavLib::AavImageLayout* m_CurrentImageLayout;
			__int64 m_NewFrameOffset;
			unsigned int m_FrameNo;

			unsigned char *m_FrameBytes;
			unsigned int m_FrameBufferIndex; 
			unsigned int m_ElapedTime;
						
			void InitFileState();
		public:
			AavFile();
			~AavFile();
			
			bool BeginFile(const char* fileName);
			void EndFile();
			
			void AddImageSection(AavLib::AavImageSection* section);
			
			int AddFileTag(const char* tagName, const char* tagValue);
			
			void BeginFrame(long long timeStamp, unsigned int elapsedTime, unsigned int exposure);
			void AddFrameStatusTag(unsigned int tagIndex, const char* tagValue);
			void AddFrameStatusTagMessage(unsigned int tagIndex, const char* tagValue);
			void AddFrameStatusTagUInt8(unsigned int tagIndex, unsigned char tagValue);
			void AddFrameStatusTagUInt16(unsigned int tagIndex, unsigned short tagValue);			
			void AddFrameStatusTagUInt64(unsigned int tagIndex, long long tagValue);
			void AddFrameStatusTagReal(unsigned int tagIndex, float tagValue);
			void AddFrameImage(unsigned char layoutId, unsigned char* pixels, unsigned char pixelsBpp);
			void EndFrame();
		};

}

#endif // ADVFILE_H
