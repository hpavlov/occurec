/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using OccuRec.Helpers;
using OccuRec.OCR;
using OccuRec.Properties;
using DirectShowLib;
using OccuRec.Utilities;

namespace OccuRec.Drivers.AAVTimer.VideoCaptureImpl
{
	internal class DirectShowCapture : ISampleGrabberCB, IDisposable
	{
        //A (modified) definition of OleCreatePropertyFrame found here: http://groups.google.no/group/microsoft.public.dotnet.languages.csharp/browse_thread/thread/db794e9779144a46/55dbed2bab4cd772?lnk=st&q=[DllImport(%22olepro32.dll%22)]&rnum=1&hl=no#55dbed2bab4cd772
        [DllImport("oleaut32.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        public static extern int OleCreatePropertyFrame(
            IntPtr hwndOwner,
            int x,
            int y,
            [MarshalAs(UnmanagedType.LPWStr)] string lpszCaption,
            int cObjects,
            [MarshalAs(UnmanagedType.Interface, ArraySubType = UnmanagedType.IUnknown)] 
			ref object ppUnk,
            int cPages,
            IntPtr lpPageClsID,
            int lcid,
            int dwReserved,
            IntPtr lpvReserved);

		private IFilterGraph2 filterGraph;
		private IMediaControl mediaCtrl;
		private ISampleGrabber samplGrabber;
		private ICaptureGraphBuilder2 capBuilder;
	    private IAMCrossbar crossbar;

		private bool isRunning = false;

		private int videoWidth;
		private int videoHeight;
		private int stride;
		private long frameCounter;

		Bitmap latestBitmap = null;
		Rectangle fullRect;

        IBaseFilter capFilter = null;

		private object syncRoot = new object();
		
		// NOTE: If the graph doesn't show up in GraphEdit then see this: http://sourceforge.net/p/directshownet/discussion/460697/thread/67dbf387
		private DsROTEntry rot = null;
	    private bool ocrEnabled = false;

	    internal IVideoCallbacks callbacksObject;

        public void SetupGraph(DsDevice dev, bool runOCR, VideoFormatHelper.SupportedVideoFormat selectedFormat, ref float iFrameRate, ref int iWidth, ref int iHeight)
		{
			try
			{
                filterGraph = (IFilterGraph2)new FilterGraph();
                mediaCtrl = filterGraph as IMediaControl;

                capBuilder = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();

                samplGrabber = (ISampleGrabber)new SampleGrabber();

                int hr = capBuilder.SetFiltergraph(filterGraph);
                DsError.ThrowExceptionForHR(hr);

                if (Settings.Default.VideoGraphDebugMode)
                {
                    if (rot != null)
                    {
                        rot.Dispose();
                        rot = null;
                    }
                    rot = new DsROTEntry(filterGraph);
                }

				SetupGraphInternal(dev, selectedFormat, ref iFrameRate, ref iWidth, ref iHeight);

				// Now that sizes are fixed/known, store the sizes
				SaveSizeInfo(samplGrabber);

                crossbar = CrossbarHelper.SetupTunerCrossbarAndAnalogueStandard(capBuilder, capFilter, iFrameRate);

				latestBitmap = new Bitmap(iWidth, iHeight, PixelFormat.Format24bppRgb);
				fullRect = new Rectangle(0, 0, latestBitmap.Width, latestBitmap.Height);

                NativeHelpers.SetupCamera(
                    Settings.Default.CameraModel, 
                    iWidth, iHeight, 
                    Settings.Default.HorizontalFlip, 
                    Settings.Default.VerticalFlip,
                    Settings.Default.IsIntegrating,
                    (float)Settings.Default.MinSignatureDiffRatio,
                    (float)Settings.Default.MinSignatureDiff,
					Settings.Default.GammaDiff,
                    Settings.Default.ForceNewFrameOnLockedIntRate,
					dev.Name,
					selectedFormat.AsSerialized(),
                    selectedFormat.FrameRate);

				NativeHelpers.SetupAav(Settings.Default.RecordStatusSectionOnly ? AavImageLayout.StatusSectionOnly : Settings.Default.AavImageLayout, Settings.Default.AavCompression);

			    ocrEnabled = false;
				string errorMessage;

				if (runOCR)
				{
					OcrConfiguration ocrConfig = OcrSettings.Instance[Settings.Default.SelectedOcrConfiguration];

					errorMessage = NativeHelpers.SetupBasicOcrMetrix(ocrConfig);
					if (errorMessage != null && callbacksObject != null)
						callbacksObject.OnError(-1, errorMessage);
					else
					{
						NativeHelpers.SetupOcr(ocrConfig);
						ocrEnabled = true;
					}
				}
			}
			catch
			{
				CloseResources();

				if (callbacksObject != null)
					callbacksObject.OnError(-1, "Error initialising the camera. The selected video mode may not be supported by the camera.");

				throw;
			}
		}

		public bool IsRunning
		{
			get { return isRunning;}
		}

		public void Start()
		{
			if (!isRunning)
			{
				frameCounter = 0;

				int hr = mediaCtrl.Run();
				DsError.ThrowExceptionForHR(hr);

				isRunning = true;
			}
		}

		public void Pause()
		{
			if (isRunning)
			{
				int hr = mediaCtrl.Pause();
				DsError.ThrowExceptionForHR(hr);

				isRunning = false;
			}
		}

        public Bitmap GetNextFrame(out ImageStatus status)
		{
            status = null;

			if (latestBitmap == null)
			{
				return null;
			}

            try
            {
                Bitmap bmp = NativeHelpers.GetCurrentImage(out status);
                if (bmp != null)
                {
                    return (Bitmap)bmp.Clone();
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }

            return (Bitmap)latestBitmap.Clone();
		}

		private void SetupGraphInternal(DsDevice dev, VideoFormatHelper.SupportedVideoFormat selectedFormat, ref float iFrameRate, ref int iWidth, ref int iHeight)
		{
			// Capture Source (Capture/Video) --> (Input) Sample Grabber (Output) --> (In) Null Renderer
			
            IBaseFilter nullRenderer = null;

			try
			{
				// Add the video device
				int hr = filterGraph.AddSourceFilterForMoniker(dev.Mon, null, dev.Name, out capFilter);
				DsError.ThrowExceptionForHR(hr);

				if (capFilter != null)
					// If any of the default config items are set
					SetConfigParms(capBuilder, capFilter, selectedFormat, ref iFrameRate, ref iWidth, ref iHeight);

				IBaseFilter baseGrabFlt = (IBaseFilter)samplGrabber;
				ConfigureSampleGrabber(samplGrabber);

				hr = filterGraph.AddFilter(baseGrabFlt, "OccuRec AVI Video Grabber");
				DsError.ThrowExceptionForHR(hr);

				// Connect the video device output to the sample grabber
				IPin videoCaptureOutputPin = DsHelper.FindPin(capFilter, PinDirection.Output, MediaType.Video, PinCategory.Capture, "Capture");
				IPin grabberInputPin = DsFindPin.ByDirection(baseGrabFlt, PinDirection.Input, 0);
				hr = filterGraph.Connect(videoCaptureOutputPin, grabberInputPin);
				DsError.ThrowExceptionForHR(hr);
				Marshal.ReleaseComObject(videoCaptureOutputPin);
				Marshal.ReleaseComObject(grabberInputPin);

				// Add the frame grabber to the graph
				nullRenderer = (IBaseFilter)new NullRenderer();
				hr = filterGraph.AddFilter(nullRenderer, "OccuRec AVI Video Null Renderer");
				DsError.ThrowExceptionForHR(hr);

				// Connect the sample grabber to the null renderer (so frame samples will be coming through)
				IPin grabberOutputPin = DsFindPin.ByDirection(baseGrabFlt, PinDirection.Output, 0);
				IPin renderedInputPin = DsFindPin.ByDirection(nullRenderer, PinDirection.Input, 0);
				hr = filterGraph.Connect(grabberOutputPin, renderedInputPin);
				DsError.ThrowExceptionForHR(hr);
				Marshal.ReleaseComObject(grabberOutputPin);
				Marshal.ReleaseComObject(renderedInputPin);
			}
			finally
			{
				if (nullRenderer != null)
					Marshal.ReleaseComObject(nullRenderer);
			}
		}

		private void SaveSizeInfo(ISampleGrabber sampGrabber)
		{
			AMMediaType media = new AMMediaType();
			int hr = sampGrabber.GetConnectedMediaType(media);
			DsError.ThrowExceptionForHR(hr);

			if ((media.formatType != FormatType.VideoInfo) || (media.formatPtr == IntPtr.Zero))
			{
				throw new NotSupportedException("Unknown Grabber Media Format");
			}

			VideoInfoHeader videoInfoHeader = (VideoInfoHeader)Marshal.PtrToStructure(media.formatPtr, typeof(VideoInfoHeader));
			videoWidth = videoInfoHeader.BmiHeader.Width;
			videoHeight = videoInfoHeader.BmiHeader.Height;
			stride = videoWidth * (videoInfoHeader.BmiHeader.BitCount / 8);

			DsUtils.FreeAMMediaType(media);
		}

        private void SetConfigParms(ICaptureGraphBuilder2 capBuilder, IBaseFilter capFilter, VideoFormatHelper.SupportedVideoFormat selectedFormat, ref float iFrameRate, ref int iWidth, ref int iHeight)
		{
			object o;
			AMMediaType media;
			IAMStreamConfig videoStreamConfig;
			IAMVideoControl videoControl = capFilter as IAMVideoControl;

			int hr = capBuilder.FindInterface(PinCategory.Capture, MediaType.Video, capFilter, typeof(IAMStreamConfig).GUID, out o);

			videoStreamConfig = o as IAMStreamConfig;
			try
			{
				if (videoStreamConfig == null)
				{
					throw new Exception("Failed to get IAMStreamConfig");
				}

                int iCount = 0, iSize = 0;
                hr = videoStreamConfig.GetNumberOfCapabilities(out iCount, out iSize);
                DsError.ThrowExceptionForHR(hr);

                VideoInfoHeader vMatching = null;
                VideoFormatHelper.SupportedVideoFormat entry = null;

                IntPtr taskMemPointer = Marshal.AllocCoTaskMem(iSize);

                AMMediaType pmtConfig = null;
                for (int iFormat = 0; iFormat < iCount; iFormat++)
                {
                    IntPtr ptr = IntPtr.Zero;

                    hr = videoStreamConfig.GetStreamCaps(iFormat, out pmtConfig, taskMemPointer);
                    DsError.ThrowExceptionForHR(hr);

                    vMatching = (VideoInfoHeader)Marshal.PtrToStructure(pmtConfig.formatPtr, typeof(VideoInfoHeader));

                    if (vMatching.BmiHeader.BitCount > 0)
                    {
                        entry = new VideoFormatHelper.SupportedVideoFormat()
                        {
                            Width = vMatching.BmiHeader.Width,
                            Height = vMatching.BmiHeader.Height,
                            BitCount = vMatching.BmiHeader.BitCount,
                            FrameRate = 10000000.0 / vMatching.AvgTimePerFrame
                        };

                        if (entry.Matches(selectedFormat))
                        {
                            // WE FOUND IT !!!
                            break;
                        }
                    }

                    vMatching = null;
                }

                if (vMatching != null)
                {
                    hr = videoStreamConfig.SetFormat(pmtConfig);
                    DsError.ThrowExceptionForHR(hr);

                    iFrameRate = 10000000/vMatching.AvgTimePerFrame;
                    iWidth = vMatching.BmiHeader.Width;
                    iHeight = vMatching.BmiHeader.Height;
                }
                else
                {
                    hr = videoStreamConfig.GetFormat(out media);
                    DsError.ThrowExceptionForHR(hr);

                    // Copy out the videoinfoheader
                    VideoInfoHeader v = new VideoInfoHeader();
                    Marshal.PtrToStructure(media.formatPtr, v);

					if (selectedFormat != null && iWidth == 0 && iHeight == 0)
					{
						// Use the config from the selected format
						iWidth = selectedFormat.Width;
						iHeight = selectedFormat.Height;
						iFrameRate = (float) selectedFormat.FrameRate;
					}

                    // If overriding the framerate, set the frame rate
                    if (iFrameRate > 0)
                    {
                        int newAvgTimePerFrame = (int)Math.Round(10000000 / iFrameRate);
                        Trace.WriteLine(string.Format("Overwriting VideoInfoHeader.AvgTimePerFrame from {0} to {1}", v.AvgTimePerFrame, newAvgTimePerFrame));
                        v.AvgTimePerFrame = newAvgTimePerFrame;
                    }
                    else
                        iFrameRate = 10000000 / v.AvgTimePerFrame;

                    // If overriding the width, set the width
                    if (iWidth > 0)
                    {
                        Trace.WriteLine(string.Format("Overwriting VideoInfoHeader.BmiHeader.Width from {0} to {1}", v.BmiHeader.Width, iWidth));
                        v.BmiHeader.Width = iWidth;
                    }
                    else
                        iWidth = v.BmiHeader.Width;

                    // If overriding the Height, set the Height
                    if (iHeight > 0)
                    {
                        Trace.WriteLine(string.Format("Overwriting VideoInfoHeader.BmiHeader.Height from {0} to {1}", v.BmiHeader.Height, iHeight));
                        v.BmiHeader.Height = iHeight;
                    }
                    else
                        iHeight = v.BmiHeader.Height;

                    // Copy the media structure back
                    Marshal.StructureToPtr(v, media.formatPtr, false);

                    // Set the new format
                    hr = videoStreamConfig.SetFormat(media);
                    try
                    {
                        DsError.ThrowExceptionForHR(hr);
                    }
                    catch (Exception ex)
                    {
                        // If setting the format failed then log the error but try to continue
                        Trace.WriteLine(ex.GetFullStackTrace());
                    }

                    DsUtils.FreeAMMediaType(media);
                    media = null;
                }

                Marshal.FreeCoTaskMem(taskMemPointer);
                DsUtils.FreeAMMediaType(pmtConfig);
                pmtConfig = null;

				// Fix upsidedown video
				if (videoControl != null)
				{
                    // NOTE: Flipping detection and fixing doesn't seem to work!

                    //IPin pPin = DsFindPin.ByCategory(capFilter, PinCategory.Capture, 0);
                    //VideoFormatHelper.FixFlippedVideo(videoControl, pPin);

                    //pPin = DsFindPin.ByCategory(capFilter, PinCategory.Preview, 0);
                    //VideoFormatHelper.FixFlippedVideo(videoControl, pPin);
				}
			}
			finally
			{
				Marshal.ReleaseComObject(videoStreamConfig);
			}
		}

		private void ConfigureSampleGrabber(ISampleGrabber sampGrabber)
		{
			AMMediaType media = new AMMediaType();

			// Set the media type to Video/RBG24
			media.majorType = MediaType.Video;
			media.subType = MediaSubType.RGB24;
			media.formatType = FormatType.VideoInfo;
			int hr = sampGrabber.SetMediaType(media);
			DsError.ThrowExceptionForHR(hr);

			DsUtils.FreeAMMediaType(media);
			media = null;

			// Configure the samplegrabber callback
			hr = sampGrabber.SetCallback(this, 1);
			DsError.ThrowExceptionForHR(hr);
		}

        public void CloseResources()
        {
            CloseInterfaces();

	        lock (this)
	        {
		        if (latestBitmap != null)
		        {
					latestBitmap.Dispose();
			        latestBitmap = null;
		        }

				if (samplGrabber != null)
				{
					Marshal.ReleaseComObject(samplGrabber);
					samplGrabber = null;
				}

				if (capBuilder != null)
				{
					Marshal.ReleaseComObject(capBuilder);
					capBuilder = null;
				}

                if (capFilter != null)
                {
                    Marshal.ReleaseComObject(capFilter);
                    capFilter = null;
                }

                if (Settings.Default.VideoGraphDebugMode)
                {
                    if (rot != null)
                    {
                        rot.Dispose();
                        rot = null;
                    }
                }

                crossbar = null;
	        }
        }

		public void Dispose()
		{
			CloseResources();
		}

		~DirectShowCapture()
        {
            CloseInterfaces();

			GC.SuppressFinalize(this);
        }

		private void CloseInterfaces()
		{
			try
			{
				if (mediaCtrl != null)
				{
					// Stop the graph
					int hr = mediaCtrl.Stop();
					mediaCtrl = null;
					isRunning = false;
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}

			if (filterGraph != null)
			{
				Marshal.ReleaseComObject(filterGraph);
				filterGraph = null;
			}
		}

		int ISampleGrabberCB.SampleCB(double SampleTime, IMediaSample pSample)
		{
			Marshal.ReleaseComObject(pSample);

			return 0;
		}



		/// <summary> buffer callback, COULD BE FROM FOREIGN THREAD. </summary>
		int ISampleGrabberCB.BufferCB(double SampleTime, IntPtr pBuffer, int BufferLen)
		{
			// NOTE: How to replace the Sample Grabber:
 			//
			// You need to write your own filter based 
			// on CBaseFilter + CBaseInputPin + CBaseOutputPin.
			//
			// You filter spawns a worker thread (that can optionally be 
			// based on CAMThread or CAMMsgThread). The thread waits for a 
			// sample to arrive (you can optionally use CAMEvent or 
			// CAMMsgEvent), then processes it and delivers it downstream 
			// through the output pin. 

            NativeHelpers.ProcessVideoFrame(pBuffer);

            frameCounter++;

			return 0;
		}

        /// <summary>
        /// Displays a property page for a filter
        /// </summary>
        /// <param name="dev">The filter for which to display a property page</param>
        public static void DisplayPropertyPage(IBaseFilter dev, IntPtr hwndOwner)
        {
            //Get the ISpecifyPropertyPages for the filter
            ISpecifyPropertyPages pProp = dev as ISpecifyPropertyPages;
            int hr = 0;

            if (pProp == null)
            {
                //If the filter doesn't implement ISpecifyPropertyPages, try displaying IAMVfwCompressDialogs instead!
                IAMVfwCompressDialogs compressDialog = dev as IAMVfwCompressDialogs;
                if (compressDialog != null)
                {
                    hr = compressDialog.ShowDialog(VfwCompressDialogs.Config, IntPtr.Zero);
                    DsError.ThrowExceptionForHR(hr);
                }
                return;
            }

            //Get the name of the filter from the FilterInfo struct
            FilterInfo filterInfo;
            hr = dev.QueryFilterInfo(out filterInfo);
            DsError.ThrowExceptionForHR(hr);

            // Get the propertypages from the property bag
            DsCAUUID caGUID;
            hr = pProp.GetPages(out caGUID);
            DsError.ThrowExceptionForHR(hr);

            // Create and display the OlePropertyFrame
            object oDevice = (object)dev;
            hr = OleCreatePropertyFrame(hwndOwner, 0, 0, filterInfo.achName, 1, ref oDevice, caGUID.cElems, caGUID.pElems, 0, 0, IntPtr.Zero);
            DsError.ThrowExceptionForHR(hr);

            // Release COM objects
            Marshal.FreeCoTaskMem(caGUID.pElems);
            Marshal.ReleaseComObject(pProp);
            if (filterInfo.pGraph != null)
            {
                Marshal.ReleaseComObject(filterInfo.pGraph);
            }
        }

        public void ShowDeviceProperties()
        {
            if (capFilter != null)
                DisplayPropertyPage(capFilter, IntPtr.Zero);
        }

        public void ConnectToCrossbarSource(int inputPinIndex)
        {
            if (crossbar != null)
                CrossbarHelper.ConnectToCrossbarSource(crossbar, inputPinIndex);
        }

        public void LoadCrossbarSources(ComboBox comboBox)
        {
            if (crossbar != null)
                CrossbarHelper.LoadCrossbarSources(crossbar, comboBox);
        }

        public bool DisableOcr()
        {
            if (ocrEnabled)
            {
                NativeHelpers.DisableOcr();
                ocrEnabled = false;
                return true;    
            }
            else
                return false;
        }
	}
}
