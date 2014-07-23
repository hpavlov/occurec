using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace OccuRec.Drivers
{
	[System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
	class NamespaceDoc
	{
	}

	public enum VideoCameraFrameRate
	{
		/// <summary>
		/// This is a digital camera or system that supports variable frame rates.
		/// </summary>
		Digital = 0,

		/// <summary>
		/// This is a video camera that supports variable frame rates.
		/// </summary>
		Variable = 0,

		/// <summary>
		/// 25 frames per second (fps) corresponding to a <b>PAL</b> (colour) or <b>CCIR</b> (black and white) video standard.
		/// </summary>
		PAL = 1,

		/// <summary>
		/// 29.97  frames per second (fps) corresponding to an <b>NTSC</b> (colour) or <b>EIA</b>b> (black and white) video standard.
		/// </summary>
		NTSC = 2
	}

	public enum VideoCameraState
	{
		/// <summary>
		/// Camera status running. The video camera is producing images and video frames are available for viewing or recording.
		/// </summary>
		videoCameraRunning = 0,

		/// <summary>
		/// Camera status recording. The video driver is recording video to the file system. Video frames are available for viewing.
		/// </summary>
		videoCameraRecording = 1,

		/// <summary>
		/// Camera status error. The video camera is in a state of an error and cannot continue its operation. Usually a restart will be required to resolve the error condition.
		/// </summary>
		videoCameraError = 2
	}

	public interface IVideoFrame
	{
		object ImageArray { get; }

		object ImageArrayVariant { get; }

        Bitmap PreviewBitmap { get; }

		long FrameNumber { get; }
		
		double ExposureDuration { get; }

		string ExposureStartTime { get; }

		string ImageInfo { get; }
	}

    public interface IVideoCallbacks
    {
        void OnError(int errorCode, string errorMessage);
        void OnEvent(int eventId, string eventData);
        void OnInfo(string infoMessage);
    }

	public interface IVideo
	{
		bool Connected { get; set; }

		string Description { get; }

		string DriverInfo { get; }

        string DriverVersion { get; }

		short InterfaceVersion { get; }

		string Name { get; }

		string VideoCaptureDeviceName { get; }

		void SetupDialog();

	    void SetCallbacks(IVideoCallbacks callbacksObject);

		string Action(string ActionName, string ActionParameters);

		ArrayList SupportedActions { get; }

        void Dispose();

		double ExposureMax { get; }

		double ExposureMin { get; }

		VideoCameraFrameRate FrameRate { get; }

		ArrayList SupportedIntegrationRates { get; }

		int IntegrationRate { get; set; }

		IVideoFrame LastVideoFrame { get; }

		IVideoFrame LastVideoFrameImageArrayVariant { get; }

		string SensorName { get; }

		SensorType SensorType { get; }

		int CameraXSize { get; }

		int CameraYSize { get; }

        int Width { get; }

		int Height { get; }

		double PixelSizeX { get; }

		double PixelSizeY { get; }

		int BitDepth { get; }

		string VideoCodec { get;  }

		string VideoFileFormat { get; }

		int VideoFramesBufferSize { get; }

		string StartRecordingVideoFile(string PreferredFileName);

		void StopRecordingVideoFile();

		VideoCameraState CameraState { get; }

		short GainMax { get; }

		short GainMin { get; }

		short Gain { get; set; }

		ArrayList Gains { get; }

		int Gamma { get; set; }

		ArrayList Gammas { get; }

		bool CanConfigureImage { get; }

		void ConfigureImage();
	}
}
