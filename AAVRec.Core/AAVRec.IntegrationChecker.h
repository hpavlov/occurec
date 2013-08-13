#pragma once

#define MAX_INTEGRATION 256
#define LOW_INTEGRATION_CHECK_POOL_SIZE 12 // 0.5 sec @ PAL
#define LOW_INTEGRATION_CHECK_FULL_CALC_FREQUENCY (MAX_INTEGRATION + 1)

namespace AAVRec
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

			void RecalculateLowIntegrationMetrics();

		public:
			IntegrationChecker(float differenceRatio, float minimumDifference);

			float NewIntegrationPeriodCutOffRatio;
			float CurrentSignatureRatio;

			void ControlIntegrationDetectionTuning(bool enabled);
			bool IsNewIntegrationPeriod(__int64 idxFrameNumber, float diffSignature);
	};
};
