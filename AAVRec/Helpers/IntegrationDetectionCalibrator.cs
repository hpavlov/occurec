using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using AAVRec.Properties;

namespace AAVRec.Helpers
{
    public class IntegrationDetectionCalibrator
    {
	    private Dictionary<float, List<float>> data;

		public IntegrationDetectionCalibrator(Dictionary<float, List<float>> data)
		{
			this.data = data;
		}

	    private int cyclesWithDropFrames = 0;

		internal struct SignatureCycleEntry
		{
			public float GammaRate;
			public float LowAverageSignature;
			public float HighAverageSignature;
		    public int[] NewFrameIndices;
		}

		public void Calibrate()
		{
			cyclesWithDropFrames = 0;
			var signaturesRatio = new Dictionary<float, SignatureCycleEntry>();

			foreach (float gammaRate in data.Keys)
			{
				float lowAverageSignature;
				float highAverageSignature;
				float lowSignatureSigma;
				float highAverageSigma;
			    var newFrameIndices = new List<int>();

				if (FindLowAndHightSignatures(data[gammaRate], out lowAverageSignature, out highAverageSignature))
				{

                    if (MatchesUsedCameraIntegration(data[gammaRate], lowAverageSignature, highAverageSignature, newFrameIndices))
					{
						signaturesRatio[(highAverageSignature - lowAverageSignature) / highAverageSignature] = 
							new SignatureCycleEntry()
							{
								GammaRate = gammaRate,
								LowAverageSignature = lowAverageSignature,
								HighAverageSignature = highAverageSignature,
                                NewFrameIndices = newFrameIndices.ToArray()
							};
					}
					else
					{
						// There are dropped frames detected for this run 
						cyclesWithDropFrames++;
					}
				}
				else
				{
					// TODO: Assuming that there are no dropped frames, try to use the Settings.Default.CalibrationIntegrationRate and see which on 
				}
			}

			if (signaturesRatio.Count > 0)
			{
                // diff = abs(pastSignaturesAverage - diffSignature);
				// isNewIntegrationPeriod = 
				//			pastSignaturesCount >= MAX_INTEGRATION || 
				//			(pastSignaturesCount > 1 && diff > MINIMUM_SIGNATURE_DIFFERENCE && diff > SIGNATURE_DIFFERENCE_FACTOR * pastSignaturesSigma)


				float maxRatio = signaturesRatio.Keys.Max();
				SignatureCycleEntry bestCycle = signaturesRatio[maxRatio];

				float pastSignaturesAverage = data[bestCycle.GammaRate].Average();
				float pastSignaturesSigma = AverageOneSigmaForIntegrationPeriods(data[bestCycle.GammaRate], bestCycle.NewFrameIndices);

				float absoluteNewFrameMinSignDiff = data[bestCycle.GammaRate]
                    .Where(x => Math.Abs(x - bestCycle.HighAverageSignature) < Math.Abs(x - bestCycle.LowAverageSignature)) // Test all "new frame" signatures only
					.Select(x => Math.Abs(pastSignaturesAverage - x))
                    .Min(); // Find the minimum difference from HI value

				float absoluteSameFrameMaxSignDiff = data[bestCycle.GammaRate]
                    .Where(x => Math.Abs(x - bestCycle.HighAverageSignature) > Math.Abs(x - bestCycle.LowAverageSignature)) // Test all "same frame" signatures only
                    .Select(x => Math.Abs(pastSignaturesAverage - x))
                    .Max(); // Find the maxium difference from LOW value

				float absoluteMinSignDiff = (absoluteSameFrameMaxSignDiff + absoluteNewFrameMinSignDiff) / 2.0f;
                float absoluteMaxDiffFact = absoluteMinSignDiff / pastSignaturesSigma;

                Trace.WriteLine(string.Format("{0}|{1}|{2}", bestCycle.GammaRate, absoluteMinSignDiff, absoluteMaxDiffFact));

			    NativeHelpers.InitIntegrationDetectionTesting(absoluteMaxDiffFact, absoluteMinSignDiff);

			    List<float> testData = data[bestCycle.GammaRate];
                for (int i = 0; i < testData.Count; i++)
                {
                    NativeHelpers.IntegrationDetectionTestNextFrame(i, testData[i]);
                }
				//Settings.Default.MinSignatureDiff
				//Settings.Default.SignatureDiffFactorEx2
			}
		}

		private bool MatchesUsedCameraIntegration(List<float> signatures, float lowSignature, float highSignature, List<int> newFrameIndices)
		{
			int lastIntegrationPeriod = -1;
			int detectedIntegrationPeriods = 0;
		    newFrameIndices.Clear();

			for (int i = 0; i < signatures.Count; i++)
			{
				bool isHighSignature = Math.Abs(signatures[i] - highSignature) < Math.Abs(signatures[i] - lowSignature);
				if (isHighSignature)
				{
					if (lastIntegrationPeriod != -1)
					{
					    if (Settings.Default.CalibrationIntegrationRate != lastIntegrationPeriod)
					        return false;
					    else
					    {
					        detectedIntegrationPeriods++;
					        newFrameIndices.Add(i);
					    }
					}

					lastIntegrationPeriod = 0;
				}

				if (lastIntegrationPeriod != -1)
					lastIntegrationPeriod++;
			}

			return detectedIntegrationPeriods >= (signatures.Count/Settings.Default.CalibrationIntegrationRate) - 1;
		}

		private bool FindLowAndHightSignatures(List<float> signatures, out float lowAverageSignature, out float highAverageSignature)
		{
			float maxValue = signatures.Max();
			float minValue = signatures.Min();

			var smallList = new List<float>();
			var bigList = new List<float>();

			foreach (float val in signatures)
			{
				if (Math.Abs(val - minValue) < Math.Abs(val - maxValue))
					smallList.Add(val);
				else
					bigList.Add(val);
			}

			lowAverageSignature = smallList.Average();
			highAverageSignature = bigList.Average();

			float lowSignatureSigma = OneSigmaFromAverage(smallList);
			float highAverageSigma = OneSigmaFromAverage(bigList);

			// NOTE: The success test is that the 3 sigma from average regions for each of the lists must not overlap
			return lowAverageSignature + 3 * lowSignatureSigma < highAverageSignature - 3 * highAverageSigma; 
		}

		private float OneSigmaFromAverage(IList<float> data)
		{
			float average = data.Average();
			float sumSquares = 0;
			foreach (float val in data)
			{
				sumSquares += (average - val)*(average - val);
			}
			return data.Count > 1 ? (float) (Math.Sqrt(sumSquares)/(data.Count - 1)) : float.NaN;
		}

        private float AverageOneSigmaForIntegrationPeriods(List<float> data, int[] newFrameIndices)
        {
            var sigmas = new List<float>();

            foreach (int newFrameIndex in newFrameIndices)
            {
                if (newFrameIndex >= Settings.Default.CalibrationIntegrationRate - 1)
                {
                    List<float> nextPeriodDataWithOutHi = data
                        .Skip(newFrameIndex + 1 - Settings.Default.CalibrationIntegrationRate)
                        .Take(Settings.Default.CalibrationIntegrationRate - 1)
                        .ToList();

                    float nextPeriodSigma = OneSigmaFromAverage(nextPeriodDataWithOutHi);
                    sigmas.Add(nextPeriodSigma);
                }
            }

            return sigmas.Average();
        }
    }
}
