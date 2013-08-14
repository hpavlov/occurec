
#include "stdafx.h"

#include "OccuRec.IntegrationChecker.h"
#include "stdlib.h"
#include <stdio.h>
#include "utils.h"
#include <cmath>

namespace OccuRec
{
	IntegrationChecker::IntegrationChecker(float differenceRatio, float minimumDifference)
	{
		minimumSignatureRatio = differenceRatio;
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
		float diffRatio = 1;
		bool isNewIntegrationPeriod = false;
	
		if (idxFrameNumber % 128 == 0 && lowFrameIntegrationMode != 0)
			RecalculateLowIntegrationMetrics();

		if (lowFrameIntegrationMode == 1)
		{
			float allEvenFrameDiff = abs(diffSignature - evenLowFrameSignAverage);
			float allOddFrameDiff = abs(diffSignature - oddLowFrameSignAverage);

			if (3 * allEvenFrameDiff < minimumSignatureDifference && 3 * allOddFrameDiff < minimumSignatureDifference)
				isNewIntegrationPeriod = true;
			else
				// Looks like the the 1-frame integration has ended. So enter in normal mode
				lowFrameIntegrationMode = 0;

			if (integrationDetectionTuning)
				DebugViewPrint(L"LFM-1: lowFrameIntegrationMode = %d; DIFF-EVEN = %.3f; DIFF-ODD = %.3f; MIN_DIFF/3 = %.3f; NEW = %d\n", lowFrameIntegrationMode, allEvenFrameDiff, allOddFrameDiff, minimumSignatureDifference / 3, isNewIntegrationPeriod);
		}
		else if (lowFrameIntegrationMode == 2)
		{
			float evenDiff = abs(evenLowFrameSignAverage - diffSignature);
			float oddDiff = abs(oddLowFrameSignAverage - diffSignature);

			bool isEvenFrame = evenDiff > minimumSignatureDifference;
			bool isOddFrame = oddDiff > minimumSignatureDifference;

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
			diffRatio = diffSignature / pastSignaturesAverage;

			isNewIntegrationPeriod = 
				pastSignaturesCount >= MAX_INTEGRATION || 
				(pastSignaturesCount > 1 && (diff > minimumSignatureDifference || diffRatio > minimumSignatureRatio));
		}

		long currSignaturesHistoryIndex = (long)((idxFrameNumber % (long long)LOW_INTEGRATION_CHECK_POOL_SIZE) & 0xFFFF);
		signaturesHistory[currSignaturesHistoryIndex] = diffSignature;

		if (!isNewIntegrationPeriod && 
			lowFrameIntegrationMode == 0 && 
			//pastSignaturesCount > 0 && 
			//pastSignaturesCount % LOW_INTEGRATION_CHECK_FULL_CALC_FREQUENCY == 0 &&
			idxFrameNumber % LOW_INTEGRATION_CHECK_FULL_CALC_FREQUENCY == 0)
		{
			if (integrationDetectionTuning)
				DebugViewPrint(L"LowFrameRate Check Triggered. pastSignaturesCount = %d (%d); lowFrameIntegrationMode = %d\n", pastSignaturesCount % LOW_INTEGRATION_CHECK_FULL_CALC_FREQUENCY, pastSignaturesCount, lowFrameIntegrationMode);

			// After having collected history for LOW_INTEGRATION_CHECK_POOL_SIZE frames, without recognizing a new integration period larger than 2-frame integration
			// we can try to recognize a 1-frame and 2-frame signatures in order to enter lowFrameIntegrationMode

			RecalculateLowIntegrationMetrics();

			float allEvenFrameDiff = abs(allLowFrameSignAverage - evenLowFrameSignAverage);
			float allOddFrameDiff = abs(allLowFrameSignAverage - oddLowFrameSignAverage);
			float evenOddFrameDiff = abs(evenLowFrameSignAverage - oddLowFrameSignAverage);

			// NOTE: This code is still 'IN TESTING' and may not work
			if (evenOddFrameDiff > minimumSignatureDifference)
			{
				// 2-Frame integration
				lowFrameIntegrationMode = 2;
				isNewIntegrationPeriod = false; // will be checked on the next frame in the lowFrameIntegrationMode specific code
			}
			else if (3 * allEvenFrameDiff < minimumSignatureDifference && 3 * allOddFrameDiff < minimumSignatureDifference)
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

			if (integrationDetectionTuning)
				DebugViewPrint(L"lowFrameIntegrationMode = %d; ALL-EVEN = %.5f; ALL-ODD = %.5f; ODD-EVEN = %.5f; DIFF_SIGN/3 = %.5f\n", 
				lowFrameIntegrationMode, allEvenFrameDiff, allOddFrameDiff, evenOddFrameDiff, minimumSignatureDifference/3);
		}

		if (integrationDetectionTuning)
			DebugViewPrint(L"FRID:%I64d PSC:%d DF:%.5f D:%.5f %.5f %.5f SM:%.3f AVG:%.5f RSSM:%.5f SGM:%.5f CSI:%d LFIM: %d NEW: %d\n", 
				idxFrameNumber, pastSignaturesCount, diffSignature, diff, minimumSignatureDifference, minimumSignatureRatio, 
				pastSignaturesSum, pastSignaturesAverage, pastSignaturesResidualSquareSum, pastSignaturesSigma, currSignaturesHistoryIndex, lowFrameIntegrationMode, isNewIntegrationPeriod); 

		if (pastSignaturesCount < MAX_INTEGRATION && pastSignaturesCount > 1)
			CurrentSignatureRatio = minimumSignatureRatio;
		else
			CurrentSignatureRatio = 0;

		if (isNewIntegrationPeriod)
		{
			pastSignaturesAverage = 0;
			pastSignaturesCount = 0;
			pastSignaturesSum = 0;
			pastSignaturesResidualSquareSum = 0;
			pastSignaturesSigma = 0;

			NewIntegrationPeriodCutOffRatio = minimumSignatureRatio;

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
