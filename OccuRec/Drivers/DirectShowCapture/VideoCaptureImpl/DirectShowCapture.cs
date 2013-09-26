using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using OccuRec.Helpers;
using OccuRec.OCR;
using OccuRec.Properties;
using OccuRec.Video.AstroDigitalVideo;
using DirectShowLib;

namespace OccuRec.Drivers.DirectShowCapture.VideoCaptureImpl
{
    internal class DirectShowCapture : ISampleGrabberCB, IDisposable
    {
        [DllImport("Kernel32.dll", EntryPoint = "RtlMoveMemory")]
        private static extern void CopyMemory(IntPtr Destination, IntPtr Source, [MarshalAs(UnmanagedType.U4)] uint Length);

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
        private IBaseFilter deviceFilter = null;
        private IAMCrossbar crossbar;

        private bool isRunning = false;
        private bool iotaVtiOcrTesting = false;

        private int videoWidth;
        private int videoHeight;
        private int stride;
        private long frameCounter;

        Bitmap latestBitmap = null;
        Rectangle fullRect;

        private object syncRoot = new object();

        private ManagedOcrTester ocrTester;

        public DirectShowCapture()
        {
            ocrTester = new ManagedOcrTester();
        }

        // NOTE: If the graph doesn't show up in GraphEdit then see this: http://sourceforge.net/p/directshownet/discussion/460697/thread/67dbf387
        private DsROTEntry rot = null;

        public void SetupFileRecorderGraph(DsDevice dev, SystemCodecEntry compressor, VideoFormatHelper.SupportedVideoFormat selectedFormat, ref float iFrameRate, ref int iWidth, ref int iHeight, string fileName)
        {
            try
            {
                SetupGraphInternal(dev, compressor, selectedFormat, ref iFrameRate, ref iWidth, ref iHeight, fileName);

                latestBitmap = new Bitmap(iWidth, iHeight, PixelFormat.Format24bppRgb);
                fullRect = new Rectangle(0, 0, latestBitmap.Width, latestBitmap.Height);
            }
            catch
            {
                CloseResources();
                throw;
            }
        }

        public void SetupPreviewOnlyGraph(DsDevice dev, VideoFormatHelper.SupportedVideoFormat selectedFormat, ref float iFrameRate, ref int iWidth, ref int iHeight)
        {
            try
            {
                SetupGraphInternal(dev, null, selectedFormat, ref iFrameRate, ref iWidth, ref iHeight, null);

                latestBitmap = new Bitmap(iWidth, iHeight, PixelFormat.Format24bppRgb);
                fullRect = new Rectangle(0, 0, latestBitmap.Width, latestBitmap.Height);
            }
            catch
            {
                CloseResources();
                throw;
            }
        }

        public bool IsRunning
        {
            get { return isRunning; }
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

        public Bitmap GetNextFrame(out long frameId)
        {
            if (latestBitmap == null || isShuttingDown)
            {
                frameId = -1;
                return null;
            }

            Bitmap rv = null;

            NonBlockingLock.Lock(
                NonBlockingLock.LOCK_ID_GetNextFrame,
                () =>
                {
                    rv = (Bitmap)latestBitmap.Clone();
                });

            frameId = frameCounter;
            return rv;
        }

        private IBaseFilter CreateFilter(Guid category, string friendlyname)
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

        private void SetupGraphInternal(DsDevice dev, SystemCodecEntry compressor, VideoFormatHelper.SupportedVideoFormat selectedFormat, ref float iFrameRate, ref int iWidth, ref int iHeight, string fileName)
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

            if (fileName != null)
				deviceFilter = BuildFileCaptureGraph(dev, compressor.Device, selectedFormat, fileName, ref iFrameRate, ref iWidth, ref iHeight);
            else
                deviceFilter = BuildPreviewOnlyCaptureGraph(dev, selectedFormat, ref iFrameRate, ref iWidth, ref iHeight);

            // Now that sizes are fixed/known, store the sizes
            SaveSizeInfo(samplGrabber);

            crossbar = CrossbarHelper.SetupTunerAndCrossbar(capBuilder, deviceFilter);

            NativeHelpers.SetupCamera(
                Settings.Default.CameraModel,
                iWidth, iHeight,
                Settings.Default.HorizontalFlip,
                Settings.Default.VerticalFlip,
                false,
                0, 0, 1,
                string.Empty,
                string.Empty,
                iFrameRate);

            NativeHelpers.SetupAav(Settings.Default.AavImageLayout);
        }

		private IBaseFilter BuildPreviewOnlyCaptureGraph(DsDevice dev, VideoFormatHelper.SupportedVideoFormat selectedFormat, ref float iFrameRate, ref int iWidth, ref int iHeight)
		{
			// Capture Source (Capture/Video) --> (Input) Sample Grabber (Output) --> (In) Null Renderer

			IBaseFilter nullRenderer = null;

			try
			{
				IBaseFilter capFilter;

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

				return capFilter;
			}
			finally
			{
				if (nullRenderer != null)
					Marshal.ReleaseComObject(nullRenderer);
			}
		}

		private IBaseFilter BuildFileCaptureGraph(DsDevice dev, DsDevice compressor, VideoFormatHelper.SupportedVideoFormat selectedFormat, string fileName, ref float iFrameRate, ref int iWidth, ref int iHeight)
		{
			// Capture Source (Capture/Video) --> (Input) Sample Grabber (Output) --> (Input) Video Compressor (Output) --> (Input 01/Video/) AVI Mux (Output) --> (In) FileSink


			IBaseFilter muxFilter = null;
			IFileSinkFilter fileWriterFilter = null;
			IBaseFilter compressorFilter = null;

			try
			{
				IBaseFilter capFilter;

				// Add the video device
				int hr = filterGraph.AddSourceFilterForMoniker(dev.Mon, null, dev.Name, out capFilter);
				DsError.ThrowExceptionForHR(hr);

				if (capFilter != null)
					SetConfigParms(capBuilder, capFilter, selectedFormat, ref iFrameRate, ref iWidth, ref iHeight);

				IBaseFilter baseGrabFlt = (IBaseFilter)samplGrabber;
				ConfigureSampleGrabber(samplGrabber);

				hr = filterGraph.AddFilter(baseGrabFlt, "OccuRec AVI Video Grabber");
				DsError.ThrowExceptionForHR(hr);

				// Connect the video device output to the sample grabber
				IPin videoCaptureOutputPin = DsHelper.FindPin(capFilter, PinDirection.Output, MediaType.Video, Guid.Empty, "Capture");
				IPin smartTeeInputPin = DsFindPin.ByDirection(baseGrabFlt, PinDirection.Input, 0);
				hr = filterGraph.Connect(videoCaptureOutputPin, smartTeeInputPin);
				DsError.ThrowExceptionForHR(hr);
				Marshal.ReleaseComObject(videoCaptureOutputPin);
				Marshal.ReleaseComObject(smartTeeInputPin);

				if (compressor != null)
					// Create the compressor
					compressorFilter = CreateFilter(FilterCategory.VideoCompressorCategory, compressor.Name);

				if (compressorFilter != null)
				{
					hr = filterGraph.AddFilter(compressorFilter, "OccuRec AVI Video Compressor");
					DsError.ThrowExceptionForHR(hr);

					// Connect the sample grabber Output pin to the compressor
					IPin grabberOutputPin = DsFindPin.ByDirection(baseGrabFlt, PinDirection.Output, 0);
					IPin compressorInputPin = DsFindPin.ByDirection(compressorFilter, PinDirection.Input, 0);
					hr = filterGraph.Connect(grabberOutputPin, compressorInputPin);
					DsError.ThrowExceptionForHR(hr);
					Marshal.ReleaseComObject(grabberOutputPin);
					Marshal.ReleaseComObject(compressorInputPin);

					// Create the file writer and AVI Mux (already connected to each other)
					hr = capBuilder.SetOutputFileName(MediaSubType.Avi, fileName, out muxFilter, out fileWriterFilter);
					DsError.ThrowExceptionForHR(hr);

					// Connect the compressor output to the AVI Mux
					IPin compressorOutputPin = DsFindPin.ByDirection(compressorFilter, PinDirection.Output, 0);
					IPin aviMuxVideoInputPin = DsFindPin.ByDirection(muxFilter, PinDirection.Input, 0);
					hr = filterGraph.Connect(compressorOutputPin, aviMuxVideoInputPin);
					DsError.ThrowExceptionForHR(hr);
					Marshal.ReleaseComObject(compressorOutputPin);
					Marshal.ReleaseComObject(aviMuxVideoInputPin);
				}
				else
				{
					// Create the file writer and AVI Mux (already connected to each other)
					hr = capBuilder.SetOutputFileName(MediaSubType.Avi, fileName, out muxFilter, out fileWriterFilter);
					DsError.ThrowExceptionForHR(hr);

					// Connect the sample grabber Output pin to the AVI Mux
					IPin grabberOutputPin = DsFindPin.ByDirection(baseGrabFlt, PinDirection.Output, 0);
					IPin aviMuxVideoInputPin = DsFindPin.ByDirection(muxFilter, PinDirection.Input, 0);
					hr = filterGraph.Connect(grabberOutputPin, aviMuxVideoInputPin);
					DsError.ThrowExceptionForHR(hr);
					Marshal.ReleaseComObject(grabberOutputPin);
					Marshal.ReleaseComObject(aviMuxVideoInputPin);					
				}

				return capFilter;
			}
			finally
			{
				if (fileWriterFilter != null)
					Marshal.ReleaseComObject(fileWriterFilter);

				if (muxFilter != null)
					Marshal.ReleaseComObject(muxFilter);

				if (compressorFilter != null)
					Marshal.ReleaseComObject(compressorFilter);
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

        private List<KeyValuePair<int, int>> GetSupportedResolutions(IBaseFilter filter)
        {
            int max = 0;
            int bitCount = 0;

            IPin pin = DsFindPin.ByCategory(filter, PinCategory.Capture, 0);

            VideoInfoHeader v = new VideoInfoHeader();
            IEnumMediaTypes mediaTypeEnum;
            int hr = pin.EnumMediaTypes(out mediaTypeEnum);
            DsError.ThrowExceptionForHR(hr);

            var availableResolutions = new List<KeyValuePair<int, int>>();

            AMMediaType[] mediaTypes = new AMMediaType[1];
            IntPtr fetched = IntPtr.Zero;
            hr = mediaTypeEnum.Next(1, mediaTypes, fetched);
            DsError.ThrowExceptionForHR(hr);

            while (fetched != null && mediaTypes[0] != null)
            {
                Marshal.PtrToStructure(mediaTypes[0].formatPtr, v);
                if (v.BmiHeader.Size != 0 && v.BmiHeader.BitCount != 0)
                {
                    if (v.BmiHeader.BitCount > bitCount)
                    {
                        availableResolutions.Clear();
                        max = 0;
                        bitCount = v.BmiHeader.BitCount;
                    }
                    availableResolutions.Add(new KeyValuePair<int, int>(v.BmiHeader.Width, v.BmiHeader.Height));
                    if (v.BmiHeader.Width > max || v.BmiHeader.Height > max)
                        max = (Math.Max(v.BmiHeader.Width, v.BmiHeader.Height));
                }
                hr = mediaTypeEnum.Next(1, mediaTypes, fetched);
                DsError.ThrowExceptionForHR(hr);
            }

            return availableResolutions;
        }

        private void SetConfigParms(ICaptureGraphBuilder2 capBuilder, IBaseFilter capFilter, VideoFormatHelper.SupportedVideoFormat selectedFormat, ref float iFrameRate, ref int iWidth, ref int iHeight)
        {
            object o;
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

                    iFrameRate = 10000000 / vMatching.AvgTimePerFrame;
                    iWidth = vMatching.BmiHeader.Width;
                    iHeight = vMatching.BmiHeader.Height;
                }
                else
                {
                    AMMediaType media;
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
            isShuttingDown = true;
            try
            {
                Thread.Sleep(250);

                if (mediaCtrl != null)
                {
                    NonBlockingLock.ExclusiveLock(
                        NonBlockingLock.LOCK_ID_CloseInterfaces,
                        () =>
                        {
                            Application.DoEvents();

                            // Stop the graph
                            int hr = mediaCtrl.Stop();
                        });

                    mediaCtrl = null;
                    isRunning = false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                isShuttingDown = false;
            }

            if (filterGraph != null)
            {
                Marshal.ReleaseComObject(filterGraph);
                filterGraph = null;
            }

            if (deviceFilter != null)
            {
                Marshal.ReleaseComObject(deviceFilter);
                deviceFilter = null;
            }
        }

        int ISampleGrabberCB.SampleCB(double SampleTime, IMediaSample pSample)
        {
            Marshal.ReleaseComObject(pSample);

            return 0;
        }

        private int lockedStatus = 0;
        private bool isShuttingDown = false;

        /// <summary> buffer callback, COULD BE FROM FOREIGN THREAD. </summary>
        int ISampleGrabberCB.BufferCB(double SampleTime, IntPtr pBuffer, int BufferLen)
        {
            if (isShuttingDown)
                return 0;

            NonBlockingLock.Lock(
                NonBlockingLock.LOCK_ID_BufferCB,
                () =>
                {
                    CopyBitmap(pBuffer);

                    frameCounter++;

                    if (iotaVtiOcrTesting)
                    {
                        if (latestBitmap != null)
                        {
                            int[,] pixels = ImageUtils.GetPixelArray(latestBitmap, AdvImageSection.GetPixelMode.Raw8Bit);
                            ocrTester.ProcessFrame(pixels, frameCounter);
                        }
                    }
                });

	        Thread.Sleep(1);

            return 0;
        }

        private void CopyBitmap(IntPtr pBuffer)
        {
            if (latestBitmap != null)
            {
                BitmapData bmd = latestBitmap.LockBits(fullRect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                try
                {
                    IntPtr ipSource = (IntPtr)(pBuffer.ToInt32() + stride * (videoHeight - 1));
                    IntPtr ipDest = bmd.Scan0;

                    for (int x = 0; x < videoHeight; x++)
                    {
                        CopyMemory(ipDest, ipSource, (uint)stride);
                        ipDest = (IntPtr)(ipDest.ToInt32() + bmd.Stride);
                        ipSource = (IntPtr)(ipSource.ToInt32() - stride);
                    }
                }
                finally
                {
                    latestBitmap.UnlockBits(bmd);
                }
            }
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
            if (deviceFilter != null)
                DisplayPropertyPage(deviceFilter, IntPtr.Zero);
        }

        public void ConnectToCrossbarSource(int inputPinIndex)
        {
            if (crossbar != null)
                CrossbarHelper.ConnectToCrossbarSource(crossbar, inputPinIndex);
        }

        public void LoadCrossbarSources(ComboBox comboBox)
        {
            Trace.WriteLine(string.Format("LoadCrossbarSources() called. crossbar is {0}", crossbar == null ? "NULL" : "NOT NULL"));

            if (crossbar != null)
                CrossbarHelper.LoadCrossbarSources(crossbar, comboBox);
        }

        public void ToggleIotaVtiOcrTesting()
        {
            ocrTester.Reset();
            iotaVtiOcrTesting = !iotaVtiOcrTesting;
        }
    }
}
