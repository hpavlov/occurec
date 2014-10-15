/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using OccuRec.Context;
using OccuRec.Drivers;
using OccuRec.Helpers;
using OccuRec.Properties;

namespace OccuRec.StateManagement
{
    public class CameraStateManager
    {
		private static int MIN_CONSEQUTIVE_FRAMES_TO_LOCK_INTEGRATION = 3;
        private static long MAX_ORC_ERRORS_PER_RUN = 100;

        private CameraState currentState;
        private IVideo driverInstance;
        private List<string> driverInstanceSupportedActions;
        private OverlayManager overlayManager;

        private int ocrErrors;
	    private bool isUsingManualIntegration;
	    private bool providesOcredTimestamps;
        private bool ocrMayBeRunning;

	    private bool isIntegratingCamera;
		private bool isRecordingOcrTestFile;

		internal VideoWrapper VideoObject { get; private set; }

	    public void ProcessFrame(VideoFrameWrapper frame)
        {
            if (currentState != null)
                currentState.ProcessFrame(this, frame);

			if ((Settings.Default.SimulatorRunOCR && Settings.Default.OcrSimulatorNativeCode) ||
			    Settings.Default.AavOcrEnabled)
			{
				if (frame.OcrErrorsSinceReset.HasValue)
				{
					ocrErrors = frame.OcrErrorsSinceReset.Value;
					providesOcredTimestamps = true;
				}
				else
				{
					ocrErrors = 0;
					providesOcredTimestamps = false;
				}
			}

			isUsingManualIntegration = frame.ManualIntegrationRateHint > 0;
        }

        internal void CameraConnected(IVideo driverInstance, VideoWrapper videoObject, OverlayManager overlayManager, int maxOcrErrorsPerRun, bool isIntegrating)
        {
            this.driverInstance = driverInstance;
            this.overlayManager = overlayManager;
	        this.VideoObject = videoObject;

	        isIntegratingCamera = isIntegrating;

            ocrErrors = 0;
	        isUsingManualIntegration = false;
	        providesOcredTimestamps = false;
            MAX_ORC_ERRORS_PER_RUN = maxOcrErrorsPerRun;

            driverInstanceSupportedActions = driverInstance.SupportedActions.Cast<string>().ToList();

            ocrMayBeRunning = driverInstanceSupportedActions.Contains("DisableOcr");

            if (isIntegrating)
				ChangeState(UndeterminedVtiOsdLocationState.Instance);
			else if (driverInstance is Drivers.ASCOMVideo.Video)
				ChangeState(ExternallyManagedCameraState.Instance);
            else
                ChangeState(NoIntegrationSupportedCameraState.Instance);
        }

        public void ChangeState(CameraState newState)
        {
            if (currentState != null)
            {
                currentState.FinaliseState();
                currentState = null;
            }

            Trace.WriteLine(string.Format("CameraState: Changing to state {0}", newState != null ? newState.GetType().ToString() : "NULL"));

            currentState = newState;

            if (currentState != null)
                currentState.InitialiseState(this);
        }

        public void CameraDisconnected()
        {
            currentState = null;
            driverInstance = null;
            overlayManager = null;
        }

		public string StartRecordingOCRTestingFile()
		{
			if (driverInstance != null)
			{
				string fileName = FileNameGenerator.GenerateFileName(OccuRecContext.Current.IsAAV);
				fileName = Path.GetFullPath(string.Format("{0}\\{1}-OCR-TEST.aav", Path.GetDirectoryName(fileName), Path.GetFileNameWithoutExtension(fileName)));
				driverInstance.Action("StartOcrTesting", fileName);

				isRecordingOcrTestFile = true;
				return fileName;
			}

			return null;
		}

		public void StopRecordingOCRTestingFile()
		{
			if (isRecordingOcrTestFile)
			{
				if (driverInstance != null)
					driverInstance.StopRecordingVideoFile();

				isRecordingOcrTestFile = false;
			}
		}

	    public bool IsRecordingOcrTestFile
	    {
			get { return isRecordingOcrTestFile; }
	    }

	    public bool IsValidIntegrationRate
	    {
		    get
		    {
		        if (Settings.Default.ForceIntegrationRatesRestrictions)
		        {
		            // Must be factorizable by 1, 2 and 3 ONLY
		            int factRate = IntegrationRate;
		            do
		            {
		                if (factRate <= 3)
		                    return true;

		                bool isDivisibleBy2 = factRate%2 == 0;
		                bool isDivisibleBy3 = factRate%3 == 0;

		                if (!isDivisibleBy2 && !isDivisibleBy3)
		                    return false;

		                if (isDivisibleBy2) factRate /= 2;
		                if (isDivisibleBy3) factRate /= 3;
		            } while (true);
		        }
		        else
		            return true;
		    }
	    }

        public bool LockIntegration()
        {
            string result = driverInstance.Action("LockIntegration", null);

            bool boolResult;
            bool.TryParse(result, out boolResult);

            if (boolResult)
            {
                ChangeState(LockedIntegrationCameraState.Instance);

                return true;
            }
            else
                return false;
        }

        public bool UnlockIntegration()
        {
            string result = driverInstance.Action("UnlockIntegration", null);

            bool boolResult;
            bool.TryParse(result, out boolResult);

            if (boolResult)
            {
                ChangeState(UndeterminedIntegrationCameraState.Instance);

                return true;
            }
            else
                return false;
        }

        public bool IsIntegrationLocked
        {
			get { return currentState is LockedIntegrationCameraState; }
        }

        public bool CanLockIntegrationNow
        {
            get
            {
                return
                    currentState is UndeterminedIntegrationCameraState &&
                    currentState.NumberConsequtiveSameIntegrationIntegratedFrames >= MIN_CONSEQUTIVE_FRAMES_TO_LOCK_INTEGRATION;
            }
        }

		public bool VtiOsdPositionUnknown
		{
			get
			{
				return
					currentState is UndeterminedVtiOsdLocationState;
			}
		}

		public bool VtiOsdAutomaticDetectionFailed
		{
			get
			{
				UndeterminedVtiOsdLocationState state = currentState as UndeterminedVtiOsdLocationState;
				return state != null && state.VtiOsdAutomaticDetectionFailed;
			}
		}

	    public int PercentDoneDetectingIntegration
	    {
			get
			{
				if (currentState is UndeterminedIntegrationCameraState && currentState.NumberConsequtiveSameIntegrationIntegratedFrames < MIN_CONSEQUTIVE_FRAMES_TO_LOCK_INTEGRATION)
					return Math.Min(99, Math.Max(1, (int)(100 * (currentState.NumberConsequtiveSameIntegrationIntegratedFrames * 1.0 / MIN_CONSEQUTIVE_FRAMES_TO_LOCK_INTEGRATION))));
				else
					return 100;
			}
	    }

        public bool CanStartRecording
        {
            get
            {
                return
                    currentState is LockedIntegrationCameraState ||
                    currentState is NoIntegrationSupportedCameraState ||
					currentState is ExternallyManagedCameraState;
            }
        }

        public int IntegrationRate
        {
            get
            {
                if (currentState is IKnowsIntegrationRate)
                    return ((IKnowsIntegrationRate)currentState).CurrentIntegrationRate;
                else
                    return 0;
            }
        }

		//public bool ToggleIotaVtiOcrTesting()
		//{
		//	if (!IsTestingIotaVtiOcr && currentState is UndeterminedIntegrationCameraState)
		//	{
		//		return StartIotaVtiOcrTesting();
		//	}
		//	else if (IsTestingIotaVtiOcr && currentState is IotaVtiOcrTestingState)
		//	{
		//		return StopIotaVtiOcrTesting();
		//	}

		//	return false;
		//}

		//private bool StartIotaVtiOcrTesting()
		//{
		//	string result = driverInstance.Action("StartOcrTesting", null);

		//	bool boolResult;
		//	bool.TryParse(result, out boolResult);

		//	if (boolResult)
		//	{
		//		ChangeState(IotaVtiOcrTestingState.Instance);

		//		return true;
		//	}
		//	else
		//		return false;            
		//}

		//private bool StopIotaVtiOcrTesting()
		//{
		//	string result = driverInstance.Action("StopOcrTesting", null);

		//	bool boolResult;
		//	bool.TryParse(result, out boolResult);

		//	if (boolResult)
		//	{
		//		ChangeState(UndeterminedIntegrationCameraState.Instance);

		//		return true;
		//	}
		//	else
		//		return false;            
		//}

		//public bool IsTestingIotaVtiOcr
		//{
		//	get { return currentState is IotaVtiOcrTestingState; }
		//}

		public bool IsCalibratingIntegration
		{
			get { return currentState is IntegrationCalibrationState; }
		}

		public bool IsUsingManualIntegration
		{
			get { return isUsingManualIntegration; }
		}

        public int OcrErrors
        {
            get { return ocrErrors; }
        }
		
	    public bool ProvidesOcredTimestamps
	    {
			get { return providesOcredTimestamps; }
	    }

        public int DroppedFrames
        {
			get
			{
				return currentState != null 
					? currentState.GetNumberOfDroppedFrames() 
					: 0;
			}
        }

		public void BeginIntegrationCalibration(int cameraIntegration)
		{
			if (isIntegratingCamera && !IsIntegrationLocked && !IsCalibratingIntegration)
			{
				string resultStr = driverInstance.Action("IntegrationCalibration", cameraIntegration.ToString());
				bool result;
				if (bool.TryParse(resultStr, out result) &&
					result)
				{
					ChangeState(IntegrationCalibrationState.Instance);
				}
			}
		}

        public void CancelIntegrationCalibration(bool calibrationSuccessful)
		{
			if (IsCalibratingIntegration)
			{
				string resultStr = driverInstance.Action("CancelIntegrationCalibration", string.Empty);
				bool result;
				if (bool.TryParse(resultStr, out result) &&
				    result)
				{
					ChangeState(UndeterminedIntegrationCameraState.Instance);
				}

                if (!calibrationSuccessful)
                    overlayManager.OnError(200, "Calibration was not successful.");
			}
		}

        public void RegisterOcrError()
        {
            ocrErrors++;

			//if (ocrErrors > MAX_ORC_ERRORS_PER_RUN)
			//{
			//	if (IsTestingIotaVtiOcr)
			//	{
			//		if (currentState is IotaVtiOcrTestingState)
			//			StopIotaVtiOcrTesting();
			//	}
			//	else
			//	{
			//		if (ocrMayBeRunning)
			//		{
			//			driverInstance.Action("DisableOcr", null);
			//			ocrMayBeRunning = false;
			//		}
			//	}
			//}
        }
    }
}
