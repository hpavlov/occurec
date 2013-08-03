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
using AAVRec.Helpers;
using AAVRec.OCR;
using AAVRec.Properties;
using AAVRec.Video.AstroDigitalVideo;
using DirectShowLib;

namespace AAVRec.Drivers.DirectShowCapture.VideoCaptureImpl
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

		public void SetupFileRecorderGraph(DsDevice dev, SystemCodecEntry compressor, ref float iFrameRate, ref int iWidth, ref int iHeight, string fileName)
		{
			try
			{
				SetupGraphInternal(dev, compressor, ref iFrameRate, ref iWidth, ref iHeight, fileName);

				latestBitmap = new Bitmap(iWidth, iHeight, PixelFormat.Format24bppRgb);
				fullRect = new Rectangle(0, 0, latestBitmap.Width, latestBitmap.Height);
			}
			catch
			{
				CloseResources();
				throw;
			} 
		}

		public void SetupPreviewOnlyGraph(DsDevice dev, ref float iFrameRate, ref int iWidth, ref int iHeight)
		{
			try
			{
				SetupGraphInternal(dev, null, ref iFrameRate, ref iWidth, ref iHeight, null);

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
                        rv = (Bitmap) latestBitmap.Clone();
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

		private void SetupGraphInternal(DsDevice dev, SystemCodecEntry compressor, ref float iFrameRate, ref int iWidth, ref int iHeight, string fileName)
		{
            NativeHelpers.SetupCamera(
                Settings.Default.CameraModel, 
                iWidth, iHeight, 
                Settings.Default.FlipHorizontally, 
                Settings.Default.FlipVertically,
                false,
                0, 0);

            NativeHelpers.SetupAav(Settings.Default.AavImageLayout);

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
			{
				if (compressor.Codec == SupportedCodec.Uncompressed)
				{
					deviceFilter = BuildFileCaptureGraph_UncompressedOrDV(dev, compressor.Device, fileName);
				}
                else if (compressor.Codec == SupportedCodec.XviD || compressor.Codec == SupportedCodec.HuffYuv211 || compressor.Codec == SupportedCodec.Lagarith)
				{
					deviceFilter = BuildFileCaptureGraph_WithCodec(dev, compressor.Device, fileName);
				}
			}
			else
			{
				deviceFilter = BuildPreviewOnlyCaptureGraph(dev);
			}

			if (deviceFilter != null)
				// If any of the default config items are set
				SetConfigParms(capBuilder, deviceFilter, ref iFrameRate, ref iWidth, ref iHeight);

			// Now that sizes are fixed/known, store the sizes
			SaveSizeInfo(samplGrabber);

            crossbar = CrossbarHelper.SetupTunerAndCrossbar(capBuilder, deviceFilter);
		}

		private IBaseFilter BuildPreviewOnlyCaptureGraph(DsDevice dev)
		{
			IBaseFilter muxFilter = null;

			try
			{
				IBaseFilter capFilter = null;

				// Add the video device
				int hr = filterGraph.AddSourceFilterForMoniker(dev.Mon, null, dev.Name, out capFilter);
				DsError.ThrowExceptionForHR(hr);

				IBaseFilter baseGrabFlt = (IBaseFilter)samplGrabber;
				ConfigureSampleGrabber(samplGrabber);

				// Add the frame grabber to the graph
				hr = filterGraph.AddFilter(baseGrabFlt, "AAVRec Video Grabber");
				DsError.ThrowExceptionForHR(hr);

				// Add the frame grabber to the graph
				muxFilter = (IBaseFilter)new NullRenderer();
                hr = filterGraph.AddFilter(muxFilter, "AAVRec Video Null Renderer");
				DsError.ThrowExceptionForHR(hr);

				// Connect everything together
				hr = capBuilder.RenderStream(PinCategory.Preview, MediaType.Video, capFilter, baseGrabFlt, muxFilter);
				DsError.ThrowExceptionForHR(hr);

				return capFilter;
			}
			finally
			{
				if (muxFilter != null)
					Marshal.ReleaseComObject(muxFilter);
			}
		}


		private IBaseFilter BuildFileCaptureGraph_UncompressedOrDV(DsDevice dev, DsDevice compressor, string fileName)
		{
			IBaseFilter compressorFilter = null;
			IBaseFilter muxFilter = null;
			IFileSinkFilter fileWriterFilter = null;
			IBaseFilter nullRenderer = null;

			try
			{
				IBaseFilter capFilter = CreateFilter(FilterCategory.VideoInputDevice, dev.Name);

				// Add the Video input device to the graph
                int hr = filterGraph.AddFilter(capFilter, "AAVRec Video Source");
				DsError.ThrowExceptionForHR(hr);

				if (compressor != null)
				{
					compressorFilter = CreateFilter(FilterCategory.VideoCompressorCategory, compressor.Name);

					// Add the Video compressor filter to the graph
                    hr = filterGraph.AddFilter(compressorFilter, "AAVRec Video Compressor");
					DsError.ThrowExceptionForHR(hr);
				}

				// Create a filter for the output avi file
				hr = capBuilder.SetOutputFileName(MediaSubType.Avi, fileName, out muxFilter, out fileWriterFilter);
				DsError.ThrowExceptionForHR(hr);

				IBaseFilter baseGrabFlt = (IBaseFilter)samplGrabber;
				ConfigureSampleGrabber(samplGrabber);

				// Add the frame grabber to the graph
                hr = filterGraph.AddFilter(baseGrabFlt, "AAVRec Video Grabber");
				DsError.ThrowExceptionForHR(hr);

				// Add the frame grabber to the graph
				nullRenderer = (IBaseFilter)new NullRenderer();
                hr = filterGraph.AddFilter(nullRenderer, "AAVRec Video Null Renderer");
				DsError.ThrowExceptionForHR(hr);

				// Render any preview pin of the device to the sample grabber
				hr = capBuilder.RenderStream(PinCategory.Preview, MediaType.Video, capFilter, baseGrabFlt, nullRenderer);
				DsError.ThrowExceptionForHR(hr);

				// Connect the device and compressor to the mux to render the capture part of the graph
				hr = capBuilder.RenderStream(PinCategory.Capture, MediaType.Video, capFilter, compressorFilter, muxFilter);
				DsError.ThrowExceptionForHR(hr);

				return capFilter;
			}
			finally
			{

				if (compressorFilter != null)
					Marshal.ReleaseComObject(compressorFilter);

				if (muxFilter != null)
					Marshal.ReleaseComObject(muxFilter);

				if (fileWriterFilter != null)
					Marshal.ReleaseComObject(fileWriterFilter);

				if (nullRenderer != null)
					Marshal.ReleaseComObject(nullRenderer);
			}
			
		}

		private static string SMART_TEE_MONKIER = @"@device:sw:{083863F1-70DE-11D0-BD40-00A0C911CE86}\{CC58E280-8AA1-11D1-B3F1-00AA003761C5}";
		private static string AVI_DECOMPRESSOR_MONKIER = @"@device:sw:{083863F1-70DE-11D0-BD40-00A0C911CE86}\{CF49D4E0-1115-11CE-B03A-0020AF0BA770}";

		private IBaseFilter BuildFileCaptureGraph_WithCodec(DsDevice dev, DsDevice compressor, string fileName)
		{
			// Capture Source (Capture/Video) --> (Input) Smart Tee (Capture) --> (Input) Video Compressor (Output) --> (Input 01/Video/) AVI Mux (Output) --> (In) FileSink
			//                                                      \
			//                                                       (Preview)--> [AVI Decompressor] --> (Input) Sample Grabber (Output) --> (In) Null Renderer
			//
			// NOTE: An AVI Decompressor will be inserted automatically between the [Smart Tee] and [Sample Grabber]


			IBaseFilter muxFilter = null;
			IFileSinkFilter fileWriterFilter = null;
			IBaseFilter nullRenderer = null;
			IBaseFilter smartTeeFilter = null;
			IBaseFilter compressorFilter = null;

			try
			{
				IBaseFilter capFilter;

				// Add the video device
				int hr = filterGraph.AddSourceFilterForMoniker(dev.Mon, null, dev.Name, out capFilter);
				DsError.ThrowExceptionForHR(hr);

				IBaseFilter baseGrabFlt = (IBaseFilter)samplGrabber;
				ConfigureSampleGrabber(samplGrabber);

                hr = filterGraph.AddFilter(baseGrabFlt, "AAVRec Video Grabber");
				DsError.ThrowExceptionForHR(hr);

				smartTeeFilter = Marshal.BindToMoniker(SMART_TEE_MONKIER) as IBaseFilter;
                hr = filterGraph.AddFilter(smartTeeFilter, "AAVRec Video SmartTee");
				DsError.ThrowExceptionForHR(hr);

				// Connect the video device output to the splitter
                IPin videoCaptureOutputPin = FindPin(capFilter, PinDirection.Output, MediaType.Video, PinCategory.Capture, "Capture");
				IPin smartTeeInputPin = DsFindPin.ByDirection(smartTeeFilter, PinDirection.Input, 0);
				hr = filterGraph.Connect(videoCaptureOutputPin, smartTeeInputPin);
				DsError.ThrowExceptionForHR(hr);
				Marshal.ReleaseComObject(videoCaptureOutputPin);
				Marshal.ReleaseComObject(smartTeeInputPin);

				// Connect the splitter Preview pin to the sample grabber
                IPin smartTeePreviewPin = FindPin(smartTeeFilter, PinDirection.Output, MediaType.Video, PinCategory.Preview, "Preview");
				IPin grabberInputPin = DsFindPin.ByDirection(baseGrabFlt, PinDirection.Input, 0);
				hr = filterGraph.Connect(smartTeePreviewPin, grabberInputPin);
				DsError.ThrowExceptionForHR(hr);
				Marshal.ReleaseComObject(smartTeePreviewPin);
				Marshal.ReleaseComObject(grabberInputPin);

				// Add the frame grabber to the graph
				nullRenderer = (IBaseFilter)new NullRenderer();
                hr = filterGraph.AddFilter(nullRenderer, "AAVRec Video Null Renderer");
				DsError.ThrowExceptionForHR(hr);

				// Connect the sample grabber to the null renderer (so frame samples will be coming through)
				IPin grabberOutputPin = DsFindPin.ByDirection(baseGrabFlt, PinDirection.Output, 0);
				IPin renderedInputPin = DsFindPin.ByDirection(nullRenderer, PinDirection.Input, 0);
				hr = filterGraph.Connect(grabberOutputPin, renderedInputPin);
				DsError.ThrowExceptionForHR(hr);
				Marshal.ReleaseComObject(grabberOutputPin);
				Marshal.ReleaseComObject(renderedInputPin);

				// Create the compressor
				compressorFilter = CreateFilter(FilterCategory.VideoCompressorCategory, compressor.Name);
                hr = filterGraph.AddFilter(compressorFilter, "AAVRec Video Compressor");
				DsError.ThrowExceptionForHR(hr);

				// Connect the splitter Capture pin to the compressor
                IPin smartTeeCapturePin = FindPin(smartTeeFilter, PinDirection.Output, MediaType.Video, PinCategory.Capture, "Capture");
				IPin compressorInputPin = DsFindPin.ByDirection(compressorFilter, PinDirection.Input, 0);
				hr = filterGraph.Connect(smartTeeCapturePin, compressorInputPin);
				DsError.ThrowExceptionForHR(hr);
				Marshal.ReleaseComObject(smartTeeCapturePin);
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

				if (nullRenderer != null)
					Marshal.ReleaseComObject(nullRenderer);

				if (smartTeeFilter != null)
					Marshal.ReleaseComObject(smartTeeFilter);
			}
		}

        private IPin PrintInfoAndReturnPin(IBaseFilter filter, IPin pin, PinDirection direction, Guid mediaType, Guid pinCategory, string debugInfo)
        {
            FilterInfo fInfo;
            int hr = filter.QueryFilterInfo(out fInfo);
            DsError.ThrowExceptionForHR(hr);

            string vendorInfo = null;
            hr = filter.QueryVendorInfo(out vendorInfo);
            if ((uint)hr != 0x80004001) /* Not Implemented*/
                DsError.ThrowExceptionForHR(hr);

            string mediaTypeStr = "N/A";
            if (mediaType == MediaType.AnalogVideo)
                mediaTypeStr = "AnalogVideo";
            else if (mediaType == MediaType.Video)
                mediaTypeStr = "Video";
            else if (mediaType == MediaType.Null)
                mediaTypeStr = "Null";

            string categoryStr = "N/A";
            if (pinCategory == PinCategory.Capture)
                categoryStr = "Capture";
            else if (pinCategory == PinCategory.Preview)
                categoryStr = "Preview";
            else if (pinCategory == PinCategory.AnalogVideoIn)
                categoryStr = "AnalogVideoIn";
            else if (pinCategory == PinCategory.CC)
                categoryStr = "CC";
            
            PinInfo pinInfo;
            hr = pin.QueryPinInfo(out pinInfo);
            DsError.ThrowExceptionForHR(hr);

            Trace.WriteLine(string.Format("Using {0} pin '{1}' of media type '{2}' and category '{3}' {4} ({5}::{6})", 
                direction == PinDirection.Input ? "input" : "output", 
                pinInfo.name,
                mediaTypeStr,
                categoryStr, 
                debugInfo,
                vendorInfo, fInfo.achName));

            return pin;
        }

		private IPin FindPin(IBaseFilter filter, PinDirection direction, Guid mediaType, Guid pinCategory, string preferredName)
		{
            if (Guid.Empty != pinCategory)
            {
                int idx = 0;

                do
                {
                    IPin pinByCategory = DsFindPin.ByCategory(filter, pinCategory, idx);

                    if (pinByCategory != null)
                    {
                        if (IsMatchingPin(pinByCategory, direction, mediaType))
                            return PrintInfoAndReturnPin(filter, pinByCategory, direction, mediaType, pinCategory, "found by category");

                        Marshal.ReleaseComObject(pinByCategory);
                    }
                    else
                        break;

                    idx++;
                }
                while (true);
            }

			if (!string.IsNullOrEmpty(preferredName))
			{
				IPin pinByName = DsFindPin.ByName(filter, preferredName);
				if (pinByName != null && IsMatchingPin(pinByName, direction, mediaType))
                    return PrintInfoAndReturnPin(filter, pinByName, direction, mediaType, pinCategory, "found by name");
 
				Marshal.ReleaseComObject(pinByName);
			}

			IEnumPins pinsEnum;
			IPin[] pins = new IPin[1];

			int hr = filter.EnumPins(out pinsEnum);
			DsError.ThrowExceptionForHR(hr);

			while (pinsEnum.Next(1, pins, IntPtr.Zero) == 0)
			{
				IPin pin = pins[0];
				if (pin != null)
				{
					if (IsMatchingPin(pin, direction, mediaType))
                        return PrintInfoAndReturnPin(filter, pin, direction, mediaType, pinCategory, "found by direction and media type");

					Marshal.ReleaseComObject(pin);
				}
			}

			return null;
		}

		private bool IsMatchingPin(IPin pin, PinDirection direction, Guid mediaType)
		{
			PinDirection pinDirection;
			int hr = pin.QueryDirection(out pinDirection);
			DsError.ThrowExceptionForHR(hr);

			if (pinDirection != direction)
				// The pin lacks direction
				return false;

			IPin connectedPin;
			hr = pin.ConnectedTo(out connectedPin);
			if ((uint)hr != 0x80040209 /* Pin is not connected */)
				DsError.ThrowExceptionForHR(hr);

			if (connectedPin != null)
			{
				// The pin is already connected
				Marshal.ReleaseComObject(connectedPin);
				return false;
			}

			IEnumMediaTypes mediaTypesEnum;
			hr = pin.EnumMediaTypes(out mediaTypesEnum);
			DsError.ThrowExceptionForHR(hr);

			AMMediaType[] mediaTypes = new AMMediaType[1];

			while (mediaTypesEnum.Next(1, mediaTypes, IntPtr.Zero) == 0)
			{
				Guid majorType = mediaTypes[0].majorType;
				DsUtils.FreeAMMediaType(mediaTypes[0]);

				if (majorType == mediaType)
				{
					// We have found the pin we were looking for
					return true;
				}
			}

			return false;
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

		private void SetConfigParms(ICaptureGraphBuilder2 capBuilder, IBaseFilter capFilter, ref float iFrameRate, ref int iWidth, ref int iHeight)
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

				// Fix upsidedown video
				if (videoControl != null)
				{
					VideoControlFlags pCapsFlags;

					IPin pPin = DsFindPin.ByCategory(capFilter, PinCategory.Capture, 0);
					hr = videoControl.GetCaps(pPin, out pCapsFlags);
					DsError.ThrowExceptionForHR(hr);

					if ((pCapsFlags & VideoControlFlags.FlipVertical) > 0)
					{
						hr = videoControl.GetMode(pPin, out pCapsFlags);
						DsError.ThrowExceptionForHR(hr);

						hr = videoControl.SetMode(pPin, pCapsFlags & ~VideoControlFlags.FlipVertical);
						DsError.ThrowExceptionForHR(hr);
					}
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
