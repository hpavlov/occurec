using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using OccuRec.Properties;

namespace OccuRec.Helpers
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

		public bool Calibrate()
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

				//          New		Same	Ratio
				// x4		0.65	0.19	4.32	
				// x8		1.11	0.27	4.11
				// x16		1.08	0.30	3.6
				// x32		1.33	0.15	8.86
				// x64		1.75	0.3		5.83
				// x128		2.9		0.3		9.73

                float absoluteMinDiffRatio = bestCycle.HighAverageSignature / bestCycle.LowAverageSignature;

				Trace.WriteLine(string.Format("{0}|{1}|{2}", bestCycle.GammaRate, absoluteMinSignDiff, absoluteMinDiffRatio));

				NativeHelpers.InitIntegrationDetectionTesting(absoluteMinDiffRatio, absoluteMinSignDiff);
			    int lastDetectedPeriodRate = -1;
			    List<float> testData = data[bestCycle.GammaRate];
			    bool calibrationIsSuccessul = true;
                for (int i = 0; i < testData.Count; i++)
                {
                    if (NativeHelpers.IntegrationDetectionTestNextFrame(i, testData[i]))
                    {
                        if (lastDetectedPeriodRate != -1)
                        {
                            if (lastDetectedPeriodRate != Settings.Default.CalibrationIntegrationRate)
                            {
                                calibrationIsSuccessul = false;
                                break;
                            }
                            lastDetectedPeriodRate = 1;
                        }
                        else
                            lastDetectedPeriodRate = 1;
                    }
                    else
                    {
                        if (lastDetectedPeriodRate != -1)
                            lastDetectedPeriodRate++;
                    }

                }

                if (calibrationIsSuccessul)
                {
                    Settings.Default.MinSignatureDiff = absoluteMinSignDiff;
					Settings.Default.MinSignatureDiffRatio = absoluteMinDiffRatio;
                    Settings.Default.GammaDiff = bestCycle.GammaRate;
                    Settings.Default.Save();

					Trace.WriteLine(string.Format("Successful calibration DiffGamma={0:0.00}; MinDiff={1:0.00}; Ratio={2:0.00}", Settings.Default.GammaDiff, Settings.Default.MinSignatureDiff, Settings.Default.MinSignatureDiffRatio));

                    NativeHelpers.ReconfigureIntegrationDetection((float)Settings.Default.MinSignatureDiffRatio, (float)Settings.Default.MinSignatureDiff, (float)Settings.Default.GammaDiff);
					NativeHelpers.SetManualIntegrationRateHint(0); // Move from Manual to Automatic integration if required

					return true;
                }
			}

		    return false;
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
