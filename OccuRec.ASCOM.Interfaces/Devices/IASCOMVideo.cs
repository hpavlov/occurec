using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace OccuRec.ASCOM.Interfaces.Devices
{
	[Serializable]
	public class VideoState
	{
		public int MinGainIndex { get; set; }
		public int MaxGainIndex { get; set; }
		public int MinGammaIndex { get; set; }
		public int MaxGammaIndex { get; set; }
		public int MinExposureIndex { get; set; }
		public int MaxExposureIndex { get; set; }

		public int GainIndex { get; set; }
		public int GammaIndex  { get; set; }
		public int ExposureIndex { get; set; }

		public string Gain { get; set; }
		public string Gamma { get; set; }
		public string Exposure { get; set; }
	}

	public interface IASCOMVideo : IASCOMDevice
	{
		VideoState GetCurrentState();
	    void Configure();

        int Width { get; }
        int Height { get; }
        int BitDepth { get; }

        ArrayList SupportedActions { get; }
        int CameraState { get; }
        bool CanConfigureImage { get; }
        string VideoFileFormat { get; }
        string VideoCodec { get; }
        string Name { get; }
        string VideoCaptureDeviceName { get; }
        IASCOMVideoFrame LastVideoFrame { get; }
		short InterfaceVersion { get; }
		double ExposureMax { get;  }
		double ExposureMin { get; }
		int FrameRate { get; }
		ArrayList SupportedIntegrationRates { get; }
		int IntegrationRate { get; set; }

		string Action(string actionName, string actionParameters);
		string StartRecordingVideoFile(string preferredFileName);
		void StopRecordingVideoFile();
		void ConfigureImage();

		string SensorName { get; }
		int SensorType { get; }
		double PixelSizeX { get; }
		double PixelSizeY { get; }
		int VideoFramesBufferSize { get; }
		short GainMax { get; }
		short GainMin { get; }
		short Gain { get; set; }
		ArrayList Gains { get; }
		short Gamma { get; set; }
		ArrayList Gammas { get; }
	}

    public interface IASCOMVideoFrame : IDisposable
    {
        object ImageArray { get; }
        object ImageArrayVariant { get; }
        byte[] PreviewBitmap { get; }
        long FrameNumber { get; }
        double ExposureDuration { get; }
        string ExposureStartTime { get; }
        string ImageInfo { get; }
    }
}
