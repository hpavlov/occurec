/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

#pragma once

#include "psf_fit.h"

struct NativePsfFitInfo
{	
	float XCenter;
	float YCenter;
	float FWHM;
	float IMax;
	float I0;
	float X0;
	float Y0;
	unsigned char MatrixSize;
	unsigned char IsSolved;

	unsigned char IsAsymmetric;
	unsigned char Reserved;
	float R0;
	float R02;	
};

struct NativeTrackedObjectInfo
{
	float CenterXDouble;
	float CenterYDouble;
	float LastKnownGoodPositionXDouble;
	float LastKnownGoodPositionYDouble;
	unsigned char IsLocated;
	unsigned char IsOffScreen;	
	unsigned int TrackingFlags;
};

enum NotMeasuredReasons
{	
	TrackedSuccessfully,
	ObjectCertaintyTooSmall,
	FWHMOutOfRange,
	ObjectTooElongated,
	FitSuspectAsNoGuidingStarsAreLocated,
	FixedObject,
	FullyDisappearingStarMarkedTrackedWithoutBeingFound
};

class TrackedObject
{
public:
	bool IsFixedAperture;
	bool IsOccultedStar;
	double StartingX;
	double StartingY;
	double ApertureInPixels;
	long ObjectId;
	
	long CenterX;
	long CenterY;
	double CenterXDouble;
	double CenterYDouble;
	bool IsLocated;
	double LastKnownGoodPositionXDouble;
	double LastKnownGoodPositionYDouble;
	bool IsOffScreen;
	unsigned int TrackingFlags;
	
	PsfFit* CurrentPsfFit;
	bool UseCurrentPsfFit;
	
	TrackedObject(long objectId, bool isFixedAperture, bool isOccultedStar, double startingX, double startingY, double apertureInPixels);
	~TrackedObject();
	
	void NextFrame();
	
	void InitialiseNewTracking();
	void SetIsTracked(bool isLocated, NotMeasuredReasons reason, double x, double y);	
};

class SimplifiedTracker
{
private:
	long m_Width;
	long m_Height;
	long m_NumTrackedObjects;	
	bool m_IsFullDisappearance;
	
	bool m_IsTrackedSuccessfully;
	TrackedObject** m_TrackedObjects;
	unsigned long* m_AreaPixels;
	
	unsigned long* GetPixelsArea(unsigned long* pixels, long centerX, long centerY, long squareWidth);
	unsigned long* GetPixelsAreaInt8(unsigned char* pixels, long centerX, long centerY, long squareWidth);
	
public:
	SimplifiedTracker(long width, long height, long numTrackedObjects, bool isFullDisappearance);
	~SimplifiedTracker();
	
	void ConfigureObject(long objectId, bool isFixedAperture, bool isOccultedStar, double startingX, double startingY, double apertureInPixels);
	void UpdatePsfFittingMethod();
	void InitialiseNewTracking();
	void NextFrame(int frameNo, unsigned long* pixels);
	void NextFrameInt8(int frameNo, unsigned char* pixels);
	long TrackerGetTargetState(long objectId, NativeTrackedObjectInfo* trackingInfo, NativePsfFitInfo* psfInfo, double* residuals);
	bool IsTrackedSuccessfully();
};

HRESULT TrackerSettings(double maxElongation, double minFWHM, double maxFWHM, double minCertainty, double minGuidingStarCertainty);
HRESULT TrackerNewConfiguration(long width, long height, long numTrackedObjects, bool isFullDisappearance);
HRESULT TrackerConfigureObject(long objectId, bool isFixedAperture, bool isOccultedStar, double startingX, double startingY, double apertureInPixels);
HRESULT TrackerNextFrame(long frameId, unsigned long* pixels);
HRESULT TrackerNextFrame_int8(long frameId, unsigned char* pixels);
HRESULT TrackerGetTargetState(long objectId, NativeTrackedObjectInfo* trackingInfo, NativePsfFitInfo* psfInfo, double* residuals);
HRESULT TrackerInitialiseNewTracking();

float MeasureObjectUsingAperturePhotometry(
	unsigned long* data, float aperture, 
	long nWidth, long nHeight, float x0, float y0, unsigned long saturationValue, float innerRadiusOfBackgroundApertureInSignalApertures, long numberOfPixelsInBackgroundAperture,
	float* totalPixels, bool* hasSaturatedPixels, double* bgOneSigmaVar, double* snr);

float MeasureObjectUsingAperturePhotometry_int8(
	unsigned char* data, float aperture, 
	long nWidth, long nHeight, float x0, float y0, unsigned char saturationValue, float innerRadiusOfBackgroundApertureInSignalApertures, long numberOfPixelsInBackgroundAperture,
	float* totalPixels, bool* hasSaturatedPixels, double* bgOneSigmaVar, double* snr);