
#include "stdafx.h"

#include "AAVRec.IntegrationChecker.h"
#include "stdlib.h"
#include <stdio.h>
#include "utils.h"
#include <cmath>

namespace AAVRec
{
	IntegrationChecker::IntegrationChecker(float differenceFactor, float minimumDifference)
	{
		signatureDifferenceFactor = differenceFactor;
		minimumSignatureDifference = minimumDifference;

		pastSignaturesAverage = 0;
		pastSignaturesSum = 0;
		pastSignaturesResidualSquareSum = 0;
		pastSignaturesSigma = 0;
		pastSignaturesCount = 0;
		evenLowFrameSignSigma = 0;
		oddLowFrameSignSigma = 0;
		allLowFrameSignSigma = 0;
		evenLowFrameSignAverage = 0;
		oddLowFrameSignAverage = 0;
		allLowFrameSignAverage = 0;
		lowFrameIntegrationMode = 0;
		evenSignMaxResidual = 0;
		oddSignMaxResidual = 0;
		allSignMaxResidual = 0;
	}

	void IntegrationChecker::ControlIntegrationDetectionTuning(bool enabled)
	{
		integrationDetectionTuning = enabled;
	}

	void IntegrationChecker::RecalculateLowIntegrationMetrics()
	{
		float evenSignSum = 0;
		float oddSignSum = 0;
		for (int i = 0; i < LOW_INTEGRATION_CHECK_POOL_SIZE; i++)
		{
			bool isEvenValue = i % 2 == 0;
			if (isEvenValue)
				evenSignSum += signaturesHistory[i];
			else
				oddSignSum += signaturesHistory[i];

		if (integrationDetectionTuning)
			DebugViewPrint(L"LIF:%2d %.3f (EVN:%.3f ODD:%.3f)\r\n", i, signaturesHistory[i], evenSignSum, oddSignSum);
		}

		evenLowFrameSignAverage = evenSignSum * 2 / LOW_INTEGRATION_CHECK_POOL_SIZE;
		oddLowFrameSignAverage = oddSignSum * 2 / LOW_INTEGRATION_CHECK_POOL_SIZE;
		allLowFrameSignAverage = (evenSignSum + oddSignSum) / LOW_INTEGRATION_CHECK_POOL_SIZE;

		float evenSignResidualSquareSum = 0;
		float oddSignResidualSquareSum = 0;
		float allSignResidualSquareSum = 0;
		evenSignMaxResidual = 0;
		oddSignMaxResidual = 0;
		allSignMaxResidual = 0;

		for (int i = 0; i < LOW_INTEGRATION_CHECK_POOL_SIZE; i++)
		{
			float residual = abs(allLowFrameSignAverage - signaturesHistory[i]);
			if (residual > allSignMaxResidual) allSignMaxResidual = residual;

			allSignResidualSquareSum += (allLowFrameSignAverage - signaturesHistory[i]) * (allLowFrameSignAverage - signaturesHistory[i]);
			bool isEvenValue = i % 2 == 0;
			if (isEvenValue)
			{
				residual = abs(evenLowFrameSignAverage - signaturesHistory[i]);
				if (residual > evenSignMaxResidual) evenSignMaxResidual = residual;
				evenSignResidualSquareSum += (evenLowFrameSignAverage - signaturesHistory[i]) * (evenLowFrameSignAverage - signaturesHistory[i]);
			}
			else
			{
				residual = abs(oddLowFrameSignAverage - signaturesHistory[i]);
				if (residual > oddSignMaxResidual) oddSignMaxResidual = residual;
				oddSignResidualSquareSum += (oddLowFrameSignAverage - signaturesHistory[i]) * (oddLowFrameSignAverage - signaturesHistory[i]);
			}
		}
		
		evenLowFrameSignSigma = sqrt(evenSignResidualSquareSum) * 2 / LOW_INTEGRATION_CHECK_POOL_SIZE;
		oddLowFrameSignSigma = sqrt(oddSignResidualSquareSum) * 2  / LOW_INTEGRATION_CHECK_POOL_SIZE;
		allLowFrameSignSigma = sqrt(allSignResidualSquareSum) * 2  / LOW_INTEGRATION_CHECK_POOL_SIZE;

		if (integrationDetectionTuning)
			DebugViewPrint(L"LowIntData Even:%.3f +/- %.3f (MAX: %.3f); Odd:%.3f +/- %.3f(MAX: %.3f); All:%.3f +/- %.3f (MAX: %.3f)\n", 
				evenLowFrameSignAverage, evenLowFrameSignSigma, evenSignMaxResidual, 
				oddLowFrameSignAverage, oddLowFrameSignSigma, oddSignMaxResidual,
				allLowFrameSignAverage, allLowFrameSignSigma, allSignMaxResidual); 
	}

	bool IntegrationChecker::IsNewIntegrationPeriod(__int64 idxFrameNumber, float diffSignature)
	{
		float diff = 0;
		bool isNewIntegrationPeriod = false;
	
		if (idxFrameNumber % 128 == 0)
			RecalculateLowIntegrationMetrics();

		if (lowFrameIntegrationMode == 1)
		{
			// NOTE: This code is still 'IN TESTING' and may not work
			diff = abs(allLowFrameSignAverage - diffSignature);

			// TODO: Consider using "10 times sigma" rather than "3-times max value". Use is as a configuration parameter
			// "10-times sigma" will be more sensitive to integration changess and less tolerant to slewing and field movement
			// "3-times max value" will be less sensitive to integration changess and more tolerant to slewing and field movement

			if (diff <= 3 * allSignMaxResidual)
				isNewIntegrationPeriod = true;
			else
				// Looks like the the 1-frame integration has ended. So enter in normal mode
				lowFrameIntegrationMode = 0;

			if (integrationDetectionTuning)
				DebugViewPrint(L"LFM-1: lowFrameIntegrationMode = %d; diff = %.3f; 10-Sigma = %.3f; 3-MaxVal = %.3f; NEW = %d\n", lowFrameIntegrationMode, diff, 10 * allLowFrameSignSigma, 3 * allSignMaxResidual, isNewIntegrationPeriod);
		}
		else if (lowFrameIntegrationMode == 2)
		{
			float evenDiff = abs(evenLowFrameSignAverage - diffSignature);
			float oddDiff = abs(oddLowFrameSignAverage - diffSignature);

			bool isEvenFrame = evenDiff <= 5 * evenSignMaxResidual;
			bool isOddFrame = oddDiff <= 5 * oddSignMaxResidual;

			if (isEvenFrame && !isOddFrame)
			{
				// Correctly recognized even frame of a x2 integration 
			
				// This is a new frame if a new 2-Frame period starts on an even frame
				isNewIntegrationPeriod = evenLowFrameSignAverage > oddLowFrameSignAverage; 
			}
			else if (isOddFrame && !isEvenFrame)
			{
				// Correctly recognized odd frame of a x2 integration 

				// This is a new frame if a new 2-Frame period starts on an odd frame
				isNewIntegrationPeriod = oddLowFrameSignAverage > evenLowFrameSignAverage; 
			}
			else
			{
				// Looks like the the 2-frame integration cannot be recognized further, so enter normal mode
				lowFrameIntegrationMode = 0;
			}

			if (integrationDetectionTuning)
				DebugViewPrint(L"LFM-2: lowFrameIntegrationMode = %d; evenDiff = %.3f; oddDiff = %.3f; 10-sigmaEven = %.3f; 10-sigmaOdd = %.3f; 3-MaxValEven = %.3f; 3-MaxValOdd = %.3f; NEW = %d\n", 
					lowFrameIntegrationMode, evenDiff, oddDiff, 10 * evenLowFrameSignSigma, 10 * oddLowFrameSignSigma, 3 * evenSignMaxResidual, 3 * oddSignMaxResidual, isNewIntegrationPeriod);
		}

		if (lowFrameIntegrationMode == 0)
		{
			diff = abs(pastSignaturesAverage - diffSignature);
	
			isNewIntegrationPeriod = 
				pastSignaturesCount >= MAX_INTEGRATION || 
				(pastSignaturesCount > 1 && diff > minimumSignatureDifference /*&& diff > signatureDifferenceFactor * pastSignaturesSigma*/);
		}

		long currSignaturesHistoryIndex = (long)((idxFrameNumber % (long long)LOW_INTEGRATION_CHECK_POOL_SIZE) & 0xFFFF);
		signaturesHistory[currSignaturesHistoryIndex] = diffSignature;

		if (!isNewIntegrationPeriod && lowFrameIntegrationMode == 0 && pastSignaturesCount > 0 && pastSignaturesCount % LOW_INTEGRATION_CHECK_FULL_CALC_FREQUENCY == 0)
		{
			// After having collected history for LOW_INTEGRATION_CHECK_POOL_SIZE frames, without recognizing a new integration period larger than 2-frame integration
			// we can try to recognize a 1-frame and 2-frame signatures in order to enter lowFrameIntegrationMode

			RecalculateLowIntegrationMetrics();

			float allLowFrameDiff = abs(allLowFrameSignAverage - diffSignature);
			float MAX_LOW_INT_SAMEFRAME_SIGMA = 3 * minimumSignatureDifference / signatureDifferenceFactor;

			// NOTE: This code is still 'IN TESTING' and may not work
			if (evenLowFrameSignSigma <= MAX_LOW_INT_SAMEFRAME_SIGMA && oddLowFrameSignSigma <= MAX_LOW_INT_SAMEFRAME_SIGMA && allLowFrameSignSigma > MAX_LOW_INT_SAMEFRAME_SIGMA)
			{
				// 2-Frame integration
				lowFrameIntegrationMode = 2;
				isNewIntegrationPeriod = false; // will be checked on the next frame in the lowFrameIntegrationMode specific code
			}
			else if (allLowFrameSignSigma <= MAX_LOW_INT_SAMEFRAME_SIGMA)
			{
				// 1-Frame integration (No integration)
				lowFrameIntegrationMode = 1;
				isNewIntegrationPeriod = false; // will be checked on the next frame in the lowFrameIntegrationMode specific code
			}
			else
			{
				// Low frame integration not found. Could be more than 64-frame integration
				lowFrameIntegrationMode = 0;
			}

	#if _DEBUG
			DebugViewPrint(L"lowFrameIntegrationMode = %d; MAX_LOW_INT_SAMEFRAME_SIGMA = %.5f\n", lowFrameIntegrationMode, MAX_LOW_INT_SAMEFRAME_SIGMA);
	#endif

		}

		if (integrationDetectionTuning)
			DebugViewPrint(L"FRID:%I64d PSC:%d DF:%.5f D:%.5f %.5f %.5f SM:%.3f AVG:%.5f RSSM:%.5f SGM:%.5f CSI:%d LFIM: %d NEW: %d\n", 
				idxFrameNumber, pastSignaturesCount, diffSignature, diff, minimumSignatureDifference, signatureDifferenceFactor * pastSignaturesSigma, 
				pastSignaturesSum, pastSignaturesAverage, pastSignaturesResidualSquareSum, pastSignaturesSigma, currSignaturesHistoryIndex, lowFrameIntegrationMode, isNewIntegrationPeriod); 

		if (pastSignaturesCount < MAX_INTEGRATION && pastSignaturesCount > 1)
			CurrentSignatureRatio = diff / (signatureDifferenceFactor * pastSignaturesSigma);
		else
			CurrentSignatureRatio = 0;

		if (isNewIntegrationPeriod)
		{
			pastSignaturesAverage = 0;
			pastSignaturesCount = 0;
			pastSignaturesSum = 0;
			pastSignaturesResidualSquareSum = 0;
			pastSignaturesSigma = 0;

			NewIntegrationPeriodCutOffRatio = diff / (signatureDifferenceFactor * pastSignaturesSigma);

			return true;
		}
		else
		{
			pastSignaturesSum+=diffSignature;
			pastSignaturesCount++;
			pastSignatures[pastSignaturesCount - 1] = diffSignature;
			pastSignaturesAverage = pastSignaturesSum / pastSignaturesCount;

			pastSignaturesResidualSquareSum = 0;

			for (int i=0; i<pastSignaturesCount; i++)
			{
				pastSignaturesResidualSquareSum += (pastSignaturesAverage - pastSignatures[i]) * (pastSignaturesAverage - pastSignatures[i]);
			}
		
			pastSignaturesSigma = sqrt(pastSignaturesResidualSquareSum) / pastSignaturesCount;

			return false;
		}
	}
};
