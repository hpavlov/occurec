using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using AAVRec.Helpers;

namespace AAVRec.StateManagement
{
	public class IntegrationCalibrationState : CameraState
	{
        internal enum CalibrationStatus
        {
            Unknown,
            CollectingData,
            CalculatingBestSettings,
            Finished,
        }

		public static IntegrationCalibrationState Instance = new IntegrationCalibrationState();

        private CalibrationStatus status = CalibrationStatus.Unknown;
	    private bool calibrationSuccessful = false;

		private IntegrationCalibrationState()
        { }

        public override void InitialiseState()
        {
            status = CalibrationStatus.CollectingData;
            calibrationSuccessful = false;
        }

		public override void ProcessFrame(CameraStateManager stateManager, Helpers.VideoFrameWrapper frame)
		{
			base.ProcessFrame(stateManager, frame);

            if (status == CalibrationStatus.CollectingData)
            {
                if (frame.PerformedAction.HasValue && frame.PerformedAction.Value == 1)
                {
                    if (frame.PerformedActionProgress.HasValue && (Math.Abs(frame.PerformedActionProgress.Value - 1) < 0.001))
                    {
                        // Calibration data collection has finished. Need to get the data and complete the calibration
                        Dictionary<float, List<float>> calibrationData = NativeHelpers.GetIntegrationCalibrationData();
                        status = CalibrationStatus.CalculatingBestSettings;
                        ThreadPool.QueueUserWorkItem(FindBestIntegrationDetectionSettings, calibrationData);
                    }
                }
            }
            else if (status == CalibrationStatus.Finished)
            {
                stateManager.CancelIntegrationCalibration(calibrationSuccessful);
            }
		}

        private void FindBestIntegrationDetectionSettings(object input)
        {
            var calibrationData = input as Dictionary<float, List<float>>;
            if (calibrationData != null)
            {
                Trace.WriteLine("Running integration detection calibration ... ");
                foreach (float gamma in calibrationData.Keys)
                {
                    Trace.WriteLine(string.Format("{0}: {1}", gamma, string.Join(",", calibrationData[gamma].Select(x => x.ToString(CultureInfo.InvariantCulture)))));
                }

                var calibrator = new IntegrationDetectionCalibrator(calibrationData);
                calibrationSuccessful = calibrator.Calibrate();
            }

            status = CalibrationStatus.Finished;
        }
	}
}
