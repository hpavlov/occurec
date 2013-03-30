#pragma once

extern long IMAGE_WIDTH;
extern long IMAGE_HEIGHT;
extern long IMAGE_STRIDE;
extern long IMAGE_TOTAL_PIXELS;

extern long AREA1_TOP;
extern long AREA1_LEFT;
extern long AREA1_WIDTH;
extern long AREA1_HEIGHT;
extern long AREA2_TOP;
extern long AREA2_LEFT;
extern long AREA2_WIDTH;
extern long AREA2_HEIGHT;

HRESULT SetTimeStampArea1(long top, long left, long width, long height);
HRESULT SetTimeStampArea2(long top, long left, long width, long height);

void MarkTimeStampAreas(unsigned char* pixels);
void MarkPixel(unsigned char* pixels, long top, long left, unsigned char colour);