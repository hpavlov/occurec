
#include "stdafx.h"

#include "OccuRec.IntegrationChecker.h"
#include "stdlib.h"
#include <stdio.h>
#include "utils.h"
#include <cmath>

#define MANUAL_INT_TOTAL_HISTORY_LOOPS 10
#define MANUAL_INT_RECALC_AFTER_LOOPS 5

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

	float tempLowValues[MANUAL_INTEGRATION_CHECK_POOL_SIZE];
	float tempHighValues[MANUAL_INTEGRATION_CHECK_POOL_SIZE];

	void IntegrationChecker::CalculateManualIntegrationForDataset(float* signatures, int signaturesCount, float* lowAverage, float* highAverage, float* lowSigma, float* highSigma)
	{
		float sumLow = 0;
		float sumHigh = 0;
		long lowFiguresCount = 0;
		long highFiguresCount = 0;
		
		for (int i = 0; i < signaturesCount; i++)
		{
			float currVal = *(signatures + i);
			if (i % currentManualRate == 0)
			{
				sumHigh += currVal;
				tempHighValues[highFiguresCount] = currVal;
				highFiguresCount++;
			}
			else
			{
				sumLow += currVal;
				tempLowValues[lowFiguresCount] = currVal;
				lowFiguresCount++;
			}
		}

		*lowAverage = sumLow / lowFiguresCount;
		float lowVariance = 0;
		for (int i = 0; i < lowFiguresCount; i++)
		{
			lowVariance += (tempLowValues[i] - *lowAverage) * (tempLowValues[i] - *lowAverage);
		}
		*lowSigma = sqrt(lowVariance / (lowFiguresCount - 1));

		*highAverage = sumHigh / highFiguresCount;
		float highVariance = 0;
		for (int i = 0; i < highFiguresCount; i++)
		{
			highVariance += (tempHighValues[i] - *highAverage) * (tempHighValues[i] - *highAverage);
		}
		*highSigma = sqrt(highVariance / (highFiguresCount - 1));
	}

	int IntegrationChecker::TryToFindManuallySpecifiedIntegrationRate()
	{
		float testSignatures[MANUAL_INTEGRATION_CHECK_POOL_SIZE];

		float bestLowAverage = 0;
		float bestHighAverage = 0;
		float bestLowSigma = 1000;
		float bestHighSigma = 1000;

		int signaturesCount = min(processedManualRateSignatures, currentManualRate * 10);

		for (int i = 0; i < currentManualRate; i++)
		{			
			float lowAverage;
			float highAverage;
			float lowSigma;
			float highSigma;

			int counter = 0;
			for (int j = i; j < signaturesCount; j++, counter++)
			{
				testSignatures[counter] = manualSignaturesHistory[j];
			}			
			int backwardsCounter = 1;
			for (int j = i - 1; j >= 0 && counter < signaturesCount; j--, counter++, backwardsCounter++)
			{
				testSignatures[signaturesCount - backwardsCounter] = manualSignaturesHistory[j];
			}

			CalculateManualIntegrationForDataset(&testSignatures[0], signaturesCount, &lowAverage, &highAverage, &lowSigma, &highSigma);

			if (lowSigma < bestLowSigma && lowSigma < highSigma)
			{
				bestHighSigma = highSigma;
				bestLowSigma = lowSigma;
				bestHighAverage = highAverage;
				bestLowAverage = lowAverage;
			}
		}

		// If the 3-sigma intervals around the Low and high values do not overlap, then we have found a solution
		if (bestLowAverage + 3 * bestLowSigma < bestHighAverage - 3 * bestHighSigma)
		{
			manualIntegrationHighAverage = bestHighAverage;
			manualIntegrationLowAverage = bestLowAverage;

			if (integrationDetectionTuning)
				DebugViewPrint(L"ManuallyHintedIntegration: New averages - Low = %.2f +/- %.4f; High: %.2f +/- %.4f", manualIntegrationLowAverage, bestLowSigma, manualIntegrationHighAverage, bestHighSigma);
			
			return 0;
		}
		else
		{
			if (integrationDetectionTuning)
				DebugViewPrint(L"ManuallyHintedIntegration: Failed for find new averaged values. %.2f +/- %.4f; %.2f +/- %.4f", manualIntegrationLowAverage, bestLowSigma, manualIntegrationHighAverage, bestHighSigma);
			
			return 1;
		}
	}

	void IntegrationChecker::RecalculateDetectedManualIntegrationRateLowAndHigh()
	{
		float oldHigh = manualIntegrationHighAverage;
		float oldLow = manualIntegrationLowAverage;

		if (TryToFindManuallySpecifiedIntegrationRate() != 0)
		{
			if (integrationDetectionTuning)
				DebugViewPrint(L"ManuallyHintedIntegration: Old averages - Low = %.2f; High: %.2f", manualIntegrationLowAverage, manualIntegrationHighAverage);
			manualIntegrationHighAverage = oldHigh;
			manualIntegrationLowAverage = oldLow;
		}
	}

	bool IntegrationChecker::IsNewIntegrationPeriod_Manual(__int64 idxFrameNumber, long manualRate, long stackRate, float diffSignature)
	{
		if (currentManualRate != manualRate)
		{
			processedManualRateSignatures = 0;
			currentManualRate = manualRate;

			if (currentManualRate == 1)
			{
				manualIntegrationIsCalibrated = true;
				PerformedAction = 0;
				PerformedActionProgress = 0;
			}
			else
			{
				manualIntegrationIsCalibrated = false;
				PerformedAction = 2;
				PerformedActionProgress = 0;
			}
		}

		if (currentManualRate == 1)
		{
			if (stackRate > 0)
				return idxFrameNumber % stackRate == 0;
			else
				// NOTE: Manual rate of 1 (without stacking) is easy! Don't do anything just always report every frame as new
				return true;
		}

		long currSignaturesHistoryIndex = (long)((processedManualRateSignatures % (long long)(MANUAL_INT_TOTAL_HISTORY_LOOPS * currentManualRate)) & 0xFFFF);
		manualSignaturesHistory[currSignaturesHistoryIndex] = diffSignature;

		processedManualRateSignatures++;

		// otherwise continue to save the diffSignatures and run all calculations (if we have at least 3 x currentManualRate signatures saved)
		bool isNewIntegrationPeriod = false;

		if (manualIntegrationIsCalibrated)
		{
			isNewIntegrationPeriod = abs(diffSignature - manualIntegrationHighAverage) < abs(diffSignature - manualIntegrationLowAverage);

			// TODO: Must deal with dropped frames somehow, otherwise calculations will be wrong

			if (processedManualRateSignatures % (MANUAL_INT_RECALC_AFTER_LOOPS * currentManualRate) == 0)
			{
				RecalculateDetectedManualIntegrationRateLowAndHigh();
			}
		}
		else
		{
			if (processedManualRateSignatures > (3 * currentManualRate + 1))
			{
				// Collection of data completed. Reset the progress values.
				PerformedActionProgress = 0;

				// Need some minimum data before anything can be detected (wait for 3 full periods/integrations)
				manualIntegrationIsCalibrated = TryToFindManuallySpecifiedIntegrationRate();
				if (manualIntegrationIsCalibrated)
					// Recognition completed
					PerformedAction = 0;
			}
			else
			{
				// Set the progress bar so the user sees we are waiting to collect enough samples to run the stats test
				PerformedActionProgress = 1.0 * processedManualRateSignatures / (3 * currentManualRate);
			}
		}		

		return isNewIntegrationPeriod;
	}

	bool IntegrationChecker::IsNewIntegrationPeriod_Automatic(__int64 idxFrameNumber, float diffSignature)
	{
		float diff = 0;
		float diffRatio = 1;
		bool isNewIntegrationPeriod = false;
	
		if (currentManualRate > 0)
		{
			// Reset the manual rate, in case the user changes from Manual X -> to automatic -> back to Manual X, which should be treated as a new manual rate
			currentManualRate = 0;
			PerformedAction = 0;
			PerformedActionProgress = 0;
		}

#if LOW_INTEGRATION_ENABLED
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
			bool newIntegrationStartsOnEven = evenLowFrameSignAverage > oddLowFrameSignAverage;

			float evenDiff = abs(evenLowFrameSignAverage - diffSignature);
			float oddDiff = abs(oddLowFrameSignAverage - diffSignature);

			bool isEvenFrame = evenDiff < oddDiff;

			if (isEvenFrame)
			{
				// Assume as even frame of a x2 integration. NOTE: The check if  we are still in x2 integration mode is done once every LOW_INTEGRATION_CHECK_FULL_CALC_FREQUENCY frames
			
				// This is a new frame if a new 2-Frame period starts on an even frame
				isNewIntegrationPeriod = newIntegrationStartsOnEven; 
			}
			else
			{
				//  Assume as odd frame of a x2 integration. NOTE: The check if  we are still in x2 integration mode is done once every LOW_INTEGRATION_CHECK_FULL_CALC_FREQUENCY frames

				// This is a new frame if a new 2-Frame period starts on an odd frame
				isNewIntegrationPeriod = !newIntegrationStartsOnEven; 
			}

			if (integrationDetectionTuning)
				DebugViewPrint(L"LFM-2: lowFrameIntegrationMode = %d; evenDiff = %.3f; oddDiff = %.3f; 3MaxEven = %.5f; 3MaxOdd = %.5f;NEW = %d\n", 
					lowFrameIntegrationMode, evenDiff, oddDiff, 3 * evenSignMaxResidual, 3 * oddSignMaxResidual, isNewIntegrationPeriod);
		}
#endif

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

#if LOW_INTEGRATION_ENABLED
		if (!isNewIntegrationPeriod && 
			lowFrameIntegrationMode == 0 && 
			idxFrameNumber % LOW_INTEGRATION_CHECK_FULL_CALC_FREQUENCY == 0)
		{
			if (integrationDetectionTuning)
				DebugViewPrint(L"LowFrameRate Check Triggered. pastSignaturesCount = %d (%d); lowFrameIntegrationMode = %d\n", pastSignaturesCount % LOW_INTEGRATION_CHECK_FULL_CALC_FREQUENCY, pastSignaturesCount, lowFrameIntegrationMode);

			// After having collected history for LOW_INTEGRATION_CHECK_POOL_SIZE frames, without recognizing a new integration period larger than 2-frame integration
			// we can try to recognize a 1-frame and 2-frame signatures in order to enter lowFrameIntegrationMode

			RecalculateLowIntegrationMetrics();

			float allEvenFrameDiff = abs(diffSignature - evenLowFrameSignAverage);
			float allOddFrameDiff = abs(diffSignature - oddLowFrameSignAverage);
			float evenOddFrameDiff = abs(evenLowFrameSignAverage - oddLowFrameSignAverage);

			float evenToOddRatio = allEvenFrameDiff / allOddFrameDiff;
			float oddToEvenRatio = allOddFrameDiff / allEvenFrameDiff;

			float minRatioEven = evenOddFrameDiff / (3 * evenSignMaxResidual);
			float minRatioOdd = evenOddFrameDiff / (3 * oddSignMaxResidual);

			// NOTE: This code is still 'IN TESTING' and may not work
			if (evenToOddRatio > minRatioOdd || oddToEvenRatio > minRatioEven)
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
				DebugViewPrint(L"lowFrameIntegrationMode = %d; DIFF-EVEN = %.5f; DIFF-ODD = %.5f; ODD-EVEN = %.5f; DIFF_SIGN/3 = %.5f ODD/EVEN = %.5f EVEN/ODD = %.5f MIN-RATIO-EVEN = %.5f MIN-RATIO-ODD = %.5f\n", 
				lowFrameIntegrationMode, allEvenFrameDiff, allOddFrameDiff, evenOddFrameDiff, minimumSignatureDifference/3, evenToOddRatio, oddToEvenRatio, minRatioEven, minRatioOdd);
		}
#endif
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
