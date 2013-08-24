#pragma once

#define MAX_INTEGRATION 256
#define LOW_INTEGRATION_CHECK_POOL_SIZE 12 // 0.5 sec @ PAL
#define LOW_INTEGRATION_CHECK_FULL_CALC_FREQUENCY (MAX_INTEGRATION + 1)
#define MANUAL_INTEGRATION_CHECK_POOL_SIZE 10240

namespace OccuRec
{
	class IntegrationChecker 
	{
		private:
			bool integrationDetectionTuning;
			float minimumSignatureRatio;
			float minimumSignatureDifference;

			float pastSignaturesAverage;
			float pastSignaturesSum;
			float pastSignaturesResidualSquareSum;
			float pastSignaturesSigma;
			int pastSignaturesCount;
			float pastSignatures[MAX_INTEGRATION];
			float signaturesHistory[LOW_INTEGRATION_CHECK_POOL_SIZE];
			float manualSignaturesHistory[MANUAL_INTEGRATION_CHECK_POOL_SIZE];

			float evenLowFrameSignSigma;
			float oddLowFrameSignSigma;
			float allLowFrameSignSigma;
			float evenLowFrameSignAverage;
			float oddLowFrameSignAverage;
			float allLowFrameSignAverage;
			int lowFrameIntegrationMode;
			float evenSignMaxResidual;
			float oddSignMaxResidual;
			float allSignMaxResidual;

			__int64 processedManualRateSignatures;
			long currentManualRate;
			bool manualIntegrationIsCalibrated;
			float manualIntegrationHighAverage;
			float manualIntegrationLowAverage;

			void RecalculateLowIntegrationMetrics();
			int TryToFindManuallySpecifiedIntegrationRate();
			void RecalculateDetectedManualIntegrationRateLowAndHigh();
			void CalculateManualIntegrationForDataset(float* signatures, int signaturesCount, float* lowAverage, float* highAverage, float* lowSigma, float* highSigma);

		public:
			IntegrationChecker(float differenceRatio, float minimumDifference);

			float NewIntegrationPeriodCutOffRatio;
			float CurrentSignatureRatio;

			long PerformedAction;
			float PerformedActionProgress;

			void ControlIntegrationDetectionTuning(bool enabled);
			bool IsNewIntegrationPeriod_Automatic(__int64 idxFrameNumber, float diffSignature);
			bool IsNewIntegrationPeriod_Manual(__int64 idxFrameNumber, long manualRate, float diffSignature);
	};
};
