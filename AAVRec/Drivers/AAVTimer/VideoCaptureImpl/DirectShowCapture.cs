using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using AAVRec.Helpers;
using AAVRec.OCR;
using AAVRec.Properties;
using DirectShowLib;

namespace AAVRec.Drivers.AAVTimer.VideoCaptureImpl
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
				SetupGraphInternal(dev, selectedFormat, ref iFrameRate, ref iWidth, ref iHeight);

				latestBitmap = new Bitmap(iWidth, iHeight, PixelFormat.Format24bppRgb);
				fullRect = new Rectangle(0, 0, latestBitmap.Width, latestBitmap.Height);

                NativeHelpers.SetupCamera(
                    Settings.Default.CameraModel, 
                    iWidth, iHeight, 
                    Settings.Default.FlipHorizontally, 
                    Settings.Default.FlipVertically,
                    Settings.Default.IsIntegrating,
                    (float)Settings.Default.MinSignatureDiffRatio,
                    (float)Settings.Default.MinSignatureDiff,
					Settings.Default.GammaDiff);

                NativeHelpers.SetupAav(Settings.Default.AavImageLayout);

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
				else
				{
					errorMessage = NativeHelpers.SetupTimestampPreservation(iWidth, iHeight);
					if (errorMessage != null && callbacksObject != null)
						callbacksObject.OnError(-1, errorMessage);

					if (callbacksObject != null)
						callbacksObject.OnError(-1, "IOTA VTI Timestamp OCR is disabled.");
				}
			}
			catch
			{
				CloseResources();
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
			filterGraph = (IFilterGraph2)new FilterGraph();
			mediaCtrl = filterGraph as IMediaControl;

			capBuilder = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();

			samplGrabber = (ISampleGrabber)new SampleGrabber();			
			IBaseFilter nullRendered = null;

			try
			{
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

			    // Add the video device
				hr = filterGraph.AddSourceFilterForMoniker(dev.Mon, null, dev.Name, out capFilter);
				DsError.ThrowExceptionForHR(hr);

                if (capFilter != null)
                    // If any of the default config items are set
                    SetConfigParms(capBuilder, capFilter, selectedFormat, ref iFrameRate, ref iWidth, ref iHeight);

				IBaseFilter baseGrabFlt = (IBaseFilter)samplGrabber;
				ConfigureSampleGrabber(samplGrabber);

				// Add the frame grabber to the graph
				hr = filterGraph.AddFilter(baseGrabFlt, "AAVRec Video Grabber");
				DsError.ThrowExceptionForHR(hr);

				// Add the frame grabber to the graph
				nullRendered = (IBaseFilter)new NullRenderer();
                hr = filterGraph.AddFilter(nullRendered, "AAVRec Video Null Renderer");
				DsError.ThrowExceptionForHR(hr);

				// Connect everything together
				hr = capBuilder.RenderStream(PinCategory.Preview, MediaType.Video, capFilter, baseGrabFlt, nullRendered);
				DsError.ThrowExceptionForHR(hr);

				// Now that sizes are fixed/known, store the sizes
				SaveSizeInfo(samplGrabber);

                crossbar = CrossbarHelper.SetupTunerAndCrossbar(capBuilder, capFilter);

				// Turn off clock so frames are sent as fast as possible
				hr = ((IMediaFilter) filterGraph).SetSyncSource(null);
				DsError.ThrowExceptionForHR(hr);
			}
			finally
			{
				if (nullRendered != null)
					Marshal.ReleaseComObject(nullRendered);
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

                    // If overriding the framerate, set the frame rate
                    if (iFrameRate > 0)
                    {
                        v.AvgTimePerFrame = (int)Math.Round(10000000 / iFrameRate);
                    }
                    else
                        iFrameRate = 10000000 / v.AvgTimePerFrame;

                    // If overriding the width, set the width
                    if (iWidth > 0)
                    {
                        v.BmiHeader.Width = iWidth;
                    }
                    else
                        iWidth = v.BmiHeader.Width;

                    // If overriding the Height, set the Height
                    if (iHeight > 0)
                    {
                        v.BmiHeader.Height = iHeight;
                    }
                    else
                        iHeight = v.BmiHeader.Height;

                    // Copy the media structure back
                    Marshal.StructureToPtr(v, media.formatPtr, false);

                    // Set the new format
                    hr = videoStreamConfig.SetFormat(media);
                    DsError.ThrowExceptionForHR(hr);

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
