/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using DirectShowLib;

namespace OccuRec.Helpers
{
    public static class VideoFormatHelper
    {

        internal class SupportedVideoFormat
        {
            public SupportedVideoFormat()
            { }

			public SupportedVideoFormat(SupportedVideoFormat cloneFrom)
			{
				Width = cloneFrom.Width;
				Height = cloneFrom.Height;
				FrameRate = cloneFrom.FrameRate;
				BitCount = cloneFrom.BitCount;
			}

			private static Regex REGEX_FORMATTER = new Regex("^(\\d+) x (\\d+) @([\\d\\.,]+) fps \\((\\d+) bpp\\)$");

			public static SupportedVideoFormat PAL = new SupportedVideoFormat("720 x 576 @25 fps (24 bpp)");
			public static SupportedVideoFormat NTSC = new SupportedVideoFormat("720 x 480 @29.97 fps (24 bpp)");

			public SupportedVideoFormat(string stringRep)
            {
                if (!string.IsNullOrEmpty(stringRep))
                {
                    Match regexMatch = REGEX_FORMATTER.Match(stringRep);
                    if (regexMatch.Success)
                    {
                        Width = int.Parse(regexMatch.Groups[1].Value);
                        Height = int.Parse(regexMatch.Groups[2].Value);
                        FrameRate = double.Parse(regexMatch.Groups[3].Value.Replace(',', '.'), CultureInfo.InvariantCulture);
						BitCount = int.Parse(regexMatch.Groups[4].Value);
                    }
                }
            }

			public bool IsPal()
			{
                return Width == 720 && Height == 576 && IsPalFrameRate();
			}

			public bool IsNtsc()
			{
                return Width == 720 && Height == 480 && IsNtscFrameRate();
			}

            public bool IsPalFrameRate()
            {
                return Math.Abs(FrameRate - 25.00) < 0.01;
            }

            public bool IsNtscFrameRate()
            {
                return Math.Abs(FrameRate - 29.97) < 0.01;
            }

            public bool Matches(SupportedVideoFormat compareTo)
            {
	            return
		            Width == compareTo.Width &&
		            Height == compareTo.Height &&
		            Math.Abs(FrameRate - compareTo.FrameRate) < 0.01 &&
					BitCount  == compareTo.BitCount;
            }

            public int Width;
            public int Height;
            public int BitCount;
            public double FrameRate;

            public override string ToString()
            {
                return string.Format("{0} x {1} @{2} fps", Width, Height, FrameRate.ToString("0.00", CultureInfo.InvariantCulture));
            }

			public string AsSerialized()
			{
				return string.Format("{0} x {1} @{2} fps ({3} bpp)", Width, Height, FrameRate.ToString("0.00", CultureInfo.InvariantCulture), BitCount);
			}
        }

        public static void LoadSupportedVideoFormats(string deviceName, ComboBox cbxCrossbarInput, out bool palSupported, out bool ntscSupported)
        {
	        palSupported = false;
	        ntscSupported = false;
	        bool palFound = false;
			bool ntscFound = false;

	        var allFormatsDict = new Dictionary<string, SupportedVideoFormat>();
			DoSupportedVideoFormatsOperation(
                deviceName,
                delegate(SupportedVideoFormat format)
                {
					string key = format.ToString();
	                SupportedVideoFormat exusting;
	                allFormatsDict.TryGetValue(key, out exusting);
					if (exusting != null)
					{
						if (exusting.BitCount < format.BitCount)
							allFormatsDict[key] = format;
					}
					else
						allFormatsDict.Add(key, format);

					if (format.IsPal()) palFound = true;
					if (format.IsNtsc()) ntscFound = true;

                });

	        if (ntscFound)
		        ntscSupported = true;
			else	        
				allFormatsDict.Add("NTCS", SupportedVideoFormat.NTSC);

			if (palFound)
				palSupported = true;
			else
				allFormatsDict.Add("PAL", SupportedVideoFormat.PAL);

			List<SupportedVideoFormat> uniqueFormats = allFormatsDict.Values.ToList();
	        uniqueFormats.Sort(
		        delegate(SupportedVideoFormat x, SupportedVideoFormat y)
			    {
				    int compareByFrameRate = x.FrameRate.CompareTo(y.FrameRate);
				    if (compareByFrameRate == 0)
					    return (x.Width + x.Height).CompareTo(y.Width + y.Height);
				    else
					    return compareByFrameRate;
			    });

	        uniqueFormats.ForEach(x => cbxCrossbarInput.Items.Add(x));
        }

        private static IBaseFilter CreateFilter(Guid category, string friendlyname)
        {
            object source = null;
            Guid iid = typeof(IBaseFilter).GUID;
            foreach (DsDevice device in DsDevice.GetDevicesOfCat(category))
            {
                if (device.Name.CompareTo(friendlyname) == 0)
                {
                    device.Mon.BindToObject(null, null, ref iid, out source);
                    break;
                }
            }

            return (IBaseFilter)source;
        }

        internal delegate void SupportedVideoFormatCallback(SupportedVideoFormat format);

        private static void DoSupportedVideoFormatsOperation(string deviceName, SupportedVideoFormatCallback callback)
        {
            IGraphBuilder graphBuilder = null;
            ICaptureGraphBuilder2 captureGraphBuilder = null;
            IBaseFilter theDevice = null;

            try
            {
                graphBuilder = (IGraphBuilder)new FilterGraph();
                captureGraphBuilder = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
                theDevice = CreateFilter(FilterCategory.VideoInputDevice, deviceName);

                // Attach the filter graph to the capture graph
                int hr = captureGraphBuilder.SetFiltergraph(graphBuilder);
                DsError.ThrowExceptionForHR(hr);

                // Add the Video input device to the graph
                hr = graphBuilder.AddFilter(theDevice, "source filter");
                DsError.ThrowExceptionForHR(hr);

                object o;
                AMMediaType media;
                IAMStreamConfig videoStreamConfig;
                IAMVideoControl videoControl = theDevice as IAMVideoControl;

                hr = captureGraphBuilder.FindInterface(PinCategory.Capture, MediaType.Video, theDevice, typeof(IAMStreamConfig).GUID, out o);
                DsError.ThrowExceptionForHR(hr);

                videoStreamConfig = o as IAMStreamConfig;
                try
                {
                    if (videoStreamConfig == null)
                    {
                        throw new Exception("Failed to get IAMStreamConfig");
                    }

                    int iCount = 0, iSize = 0;
                    videoStreamConfig.GetNumberOfCapabilities(out iCount, out iSize);

                    IntPtr taskMemPointer = Marshal.AllocCoTaskMem(iSize);

                    AMMediaType pmtConfig = null;
                    Trace.WriteLine(string.Format("Video formats supported by {0}:", deviceName));
                    for (int iFormat = 0; iFormat < iCount; iFormat++)
                    {
                        IntPtr ptr = IntPtr.Zero;

                        videoStreamConfig.GetStreamCaps(iFormat, out pmtConfig, taskMemPointer);

                        var v2 = (VideoInfoHeader)Marshal.PtrToStructure(pmtConfig.formatPtr, typeof(VideoInfoHeader));

                        if (v2.BmiHeader.BitCount > 0)
                        {
                            var entry = new SupportedVideoFormat()
                            {
                                Width = v2.BmiHeader.Width,
                                Height = v2.BmiHeader.Height,
                                BitCount = v2.BmiHeader.BitCount,
                                FrameRate = 10000000.0 / v2.AvgTimePerFrame
                            };

							if (Math.Abs(entry.FrameRate - 0) < 0.01 || double.IsNaN(entry.FrameRate) || double.IsInfinity(entry.FrameRate))
							{
								// Some drivers don't report a frame rate or report incorrect frame rate. In this caasea dd both PAL and NTSC frame rates
								entry.FrameRate = 25.0;
								callback(entry);

								var entry2 = new SupportedVideoFormat(entry);
								entry2.FrameRate = 29.97;
								callback(entry2);
							}
							else
								callback(entry);

                            Trace.WriteLine(entry.AsSerialized());
                        }
                    }

                    Marshal.FreeCoTaskMem(taskMemPointer);
                    DsUtils.FreeAMMediaType(pmtConfig);
                }
                finally
                {
                    Marshal.ReleaseComObject(videoStreamConfig);
                }
            }
            finally
            {
                if (theDevice != null)
                    Marshal.ReleaseComObject(theDevice);

                if (graphBuilder != null)
                    Marshal.ReleaseComObject(graphBuilder);

                if (captureGraphBuilder != null)
                    Marshal.ReleaseComObject(captureGraphBuilder);
            }
        }

        public static void FixFlippedVideo(IAMVideoControl videoControl, IPin pPin)
        {
            VideoControlFlags pCapsFlags;

            int hr = videoControl.GetCaps(pPin, out pCapsFlags);
            DsError.ThrowExceptionForHR(hr);

            if ((pCapsFlags & VideoControlFlags.FlipVertical) > 0)
            {
                hr = videoControl.GetMode(pPin, out pCapsFlags);
                DsError.ThrowExceptionForHR(hr);

                hr = videoControl.SetMode(pPin, pCapsFlags & ~VideoControlFlags.FlipVertical);
                DsError.ThrowExceptionForHR(hr);

                PinInfo pinInfo;
                hr = pPin.QueryPinInfo(out pinInfo);
                DsError.ThrowExceptionForHR(hr);

                Trace.WriteLine("Fixing 'FlipVertical' video for pin " + pinInfo.name);
            }

            if ((pCapsFlags & VideoControlFlags.FlipHorizontal) > 0)
            {
                hr = videoControl.GetMode(pPin, out pCapsFlags);
                DsError.ThrowExceptionForHR(hr);

                hr = videoControl.SetMode(pPin, pCapsFlags | VideoControlFlags.FlipHorizontal);
                DsError.ThrowExceptionForHR(hr);

                PinInfo pinInfo;
                hr = pPin.QueryPinInfo(out pinInfo);
                DsError.ThrowExceptionForHR(hr);

                Trace.WriteLine("Fixing 'FlipHorizontal' video for pin " + pinInfo.name);
            }
        }
    }
}
