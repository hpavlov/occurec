// The following ifdef block is the standard way of creating macros which make exporting 
// from a DLL simpler. All files within this DLL are compiled with the AAVRECCORE_EXPORTS
// symbol defined on the command line. This symbol should not be defined on any project
// that uses this DLL. This way any other project whose source files include this file see 
// AAVRECCORE_API functions as being imported from a DLL, whereas this DLL sees symbols
// defined with this macro as being exported.
#ifdef AAVRECCORE_EXPORTS
#define AAVRECCORE_API __declspec(dllexport)
#else
#define AAVRECCORE_API __declspec(dllimport)
#endif

struct ImageStatus
{
	__int64 StartExposureTicks;
	__int64 EndExposureTicks;
	__int64 StartExposureFrameNo;
	__int64 EndExposureFrameNo;	
	long CountedFrames;
	float CutOffRatio;
	__int64 IntegratedFrameNo;
};

struct FrameProcessingStatus
{
	__int64 CameraFrameNo;
	__int64 IntegratedFrameNo;
	long IntegratedFramesSoFar;
	float FrameDiffSignature;
	float CurrentSignatureRatio;
};

extern long IMAGE_WIDTH;
extern long IMAGE_HEIGHT;
extern long IMAGE_STRIDE;
extern long IMAGE_TOTAL_PIXELS;
extern long MONOCHROME_CONVERSION_MODE;

extern bool FLIP_VERTICALLY;
extern bool FLIP_HORIZONTALLY;

extern bool IS_INTEGRATING_CAMERA;
extern float SIGNATURE_DIFFERENCE_FACTOR;


HRESULT SetupCamera(long width, long height, LPCTSTR szCameraModel, long monochromeConversionMode, bool flipHorizontally, bool flipVertically, bool isIntegrating, float signDiffFactor);
HRESULT GetCurrentImage(BYTE* bitmapPixels);
HRESULT GetCurrentImageStatus(ImageStatus* imageStatus);
HRESULT ProcessVideoFrame(LPVOID bmpBits, __int64 currentUtcDayAsTicks, FrameProcessingStatus* frameInfo);
HRESULT StartRecording(LPCTSTR szFileName);
HRESULT StopRecording(long* pixels);