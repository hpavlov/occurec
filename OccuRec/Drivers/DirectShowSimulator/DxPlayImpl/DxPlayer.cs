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
using System.Threading;
using System.Windows.Forms;
using DirectShowLib;
using OccuRec.Helpers;
using OccuRec.Properties;

namespace OccuRec.Drivers.DirectShowSimulator.DxPlayImpl
{
	internal class DxPlayer : ISampleGrabberCB, IDisposable
	{
		enum GraphState
		{
			Stopped,
			Paused,
			Running,
			Exiting
		}

		public int ImageWidth { get; private set; }

		public int ImageHeight { get; private set; }

		public int BitDepth { get; private set; }

		#region Member variables

		// File name we are playing
		private string m_sFileName;

		// graph builder interfaces
		private IFilterGraph2 m_FilterGraph;
		private IMediaControl m_mediaCtrl;
		private IMediaEvent m_mediaEvent;

		// Used to grab current snapshots
		ISampleGrabber m_sampGrabber = null;

		// Grab once.  Used to create bitmap
		private int m_videoWidth;
		private int m_videoHeight;
		private int m_stride;
		private int m_ImageSize; // In bytes

		// Event used by Media Event thread
		private ManualResetEvent m_mre;

		// Current state of the graph (can change async)
		volatile private GraphState m_State = GraphState.Stopped;

		#endregion

		// Release everything.
		public void Dispose()
		{
			CloseInterfaces();
			m_State = GraphState.Exiting;
		}

		~DxPlayer()
		{
			CloseInterfaces();
		}

		// Event that is called when a clip finishs playing
		public event DxPlayEvent StopPlay;
		public delegate void DxPlayEvent(Object sender);

		private float m_FrameRate;
		private bool m_FullAviSimulation;

		internal IVideoCallbacks callbacksObject;

		// Play an avi file into a window.  Allow for snapshots.
		// (Control to show video in, Avi file to play
		public DxPlayer(string FileName, float frameRate, bool fullAviSimulation)
		{
			try
			{
				int hr;
				IntPtr hEvent;

				// Save off the file name
				m_sFileName = FileName;
				m_FrameRate = frameRate;
				m_FullAviSimulation = fullAviSimulation;

				// Set up the graph
				SetupGraph(FileName);

				BitDepth = 8;
				ImageWidth = m_videoWidth;
				ImageHeight = m_videoHeight;

				NativeHelpers.SetupCamera(
					Settings.Default.CameraModel,
					ImageWidth, ImageHeight,
					Settings.Default.HorizontalFlip,
					Settings.Default.VerticalFlip,
					Settings.Default.IsIntegrating,
					(float)Settings.Default.MinSignatureDiffRatio,
					(float)Settings.Default.MinSignatureDiff,
					Settings.Default.GammaDiff,
					OccuRec.Drivers.AVISimulator.Video.DRIVER_DESCRIPTION,
					"N/A",
					frameRate);

				NativeHelpers.SetupAav(Settings.Default.AavImageLayout, Settings.Default.AavCompression);

				// Get the event handle the graph will use to signal
				// when events occur
				hr = m_mediaEvent.GetEventHandle(out hEvent);
				DsError.ThrowExceptionForHR(hr);

				// Wrap the graph event with a ManualResetEvent
				m_mre = new ManualResetEvent(false);
#if USING_NET11
                m_mre.Handle = hEvent;
#else
				m_mre.SafeWaitHandle = new Microsoft.Win32.SafeHandles.SafeWaitHandle(hEvent, true);
#endif

				// Create a new thread to wait for events
				ThreadPool.QueueUserWorkItem(new WaitCallback(this.EventWait));
			}
			catch
			{
				Dispose();
				throw;
			}
		}

		// Wait for events to happen.  This approach uses waiting on an event handle.
		// The nice thing about doing it this way is that you aren't in the windows message
		// loop, and don't have to worry about re-entrency or taking too long.  Plus, being
		// in a class as we are, we don't have access to the message loop.
		// Alternately, you can receive your events as windows messages.  See
		// IMediaEventEx.SetNotifyWindow.
		private void EventWait(object state)
		{
			// Returned when GetEvent is called but there are no events
			const int E_ABORT = unchecked((int)0x80004004);

			int hr;
			IntPtr p1, p2;
			EventCode ec;

			do
			{
				// Wait for an event
				m_mre.WaitOne(-1, true);

				// Avoid contention for m_State
				lock (this)
				{
					// If we are not shutting down
					if (m_State != GraphState.Exiting)
					{
						// Read the event
						for (
							hr = m_mediaEvent.GetEvent(out ec, out p1, out p2, 0);
							hr >= 0;
							hr = m_mediaEvent.GetEvent(out ec, out p1, out p2, 0)
							)
						{
							// Write the event name to the debug window
							Debug.WriteLine(ec.ToString());

							// If the clip is finished playing
							if (ec == EventCode.Complete)
							{
								// Call Stop() to set state
								Stop();

								// Throw event
								if (StopPlay != null)
								{
									StopPlay(this);
								}
							}

							// Release any resources the message allocated
							hr = m_mediaEvent.FreeEventParams(ec, p1, p2);
							DsError.ThrowExceptionForHR(hr);
						}

						// If the error that exited the loop wasn't due to running out of events
						if (hr != E_ABORT)
						{
							DsError.ThrowExceptionForHR(hr);
						}
					}
					else
					{
						// We are shutting down
						break;
					}
				}
			} while (true);
		}


		public VideoCameraFrameRate GetFrameRate()
		{
			if (Math.Abs(m_FrameRate - 25.00) < 0.01)
				return VideoCameraFrameRate.PAL;
			else if (Math.Abs(m_FrameRate - 29.97) < 0.01)
				return VideoCameraFrameRate.NTSC;

			return VideoCameraFrameRate.Variable;
		}

		// Return the currently playing file name
		public string FileName
		{
			get
			{
				return m_sFileName;
			}
		}

		// start playing
		public void Start()
		{
			// If we aren't already playing (or shutting down)
			if (m_State == GraphState.Stopped || m_State == GraphState.Paused)
			{
				m_FrameCounter = 0;

				int hr = m_mediaCtrl.Run();
				DsError.ThrowExceptionForHR(hr);

				m_State = GraphState.Running;
			}
		}

		public bool IsRunning
		{
			get { return m_State == GraphState.Running; }
		}

		// Pause the capture graph.
		public void Pause()
		{
			// If we are playing
			if (m_State == GraphState.Running)
			{
				int hr = m_mediaCtrl.Pause();
				DsError.ThrowExceptionForHR(hr);

				m_State = GraphState.Paused;
			}
		}
		// Pause the capture graph.
		public void Stop()
		{
			// Can only Stop when playing or paused
			if (m_State == GraphState.Running || m_State == GraphState.Paused)
			{
				int hr = m_mediaCtrl.Stop();
				DsError.ThrowExceptionForHR(hr);

				m_State = GraphState.Stopped;
			}
		}

		// Reset the clip back to the beginning
		public void Rewind()
		{
			int hr;

			IMediaPosition imp = m_FilterGraph as IMediaPosition;
			hr = imp.put_CurrentPosition(0);
		}

		// Grab a snapshot of the most recent image played.
		// Returns A pointer to the raw pixel data. Caller must release this memory with
		// Marshal.FreeCoTaskMem when it is no longer needed.
		public IntPtr SnapShot()
		{
			int hr;
			IntPtr ip = IntPtr.Zero;
			int iBuffSize = 0;

			// Read the buffer size
			hr = m_sampGrabber.GetCurrentBuffer(ref iBuffSize, ip);
			DsError.ThrowExceptionForHR(hr);

			Debug.Assert(iBuffSize == m_ImageSize, "Unexpected buffer size");

			// Allocate the buffer and read it
			ip = Marshal.AllocCoTaskMem(iBuffSize);

			hr = m_sampGrabber.GetCurrentBuffer(ref iBuffSize, ip);
			DsError.ThrowExceptionForHR(hr);

			return ip;
		}

		// Convert a point to the raw pixel data to a .NET bitmap
		public Bitmap IPToBmp(IntPtr ip)
		{
			// We know the Bits Per Pixel is 24 (3 bytes) because we forced it 
			// to be with sampGrabber.SetMediaType()
			int iBufSize = m_videoWidth * m_videoHeight * 3;

			return new Bitmap(
				m_videoWidth,
				m_videoHeight,
				-m_stride,
				PixelFormat.Format24bppRgb,
				(IntPtr)(ip.ToInt32() + iBufSize - m_stride)
				);
		}

		// Build the capture graph for grabber and renderer.</summary>
		// (Control to show video in, Filename to play)
		private void SetupGraph(string FileName)
		{
			int hr;

			// Get the graphbuilder object
			m_FilterGraph = new FilterGraph() as IFilterGraph2;

			// Get a ICaptureGraphBuilder2 to help build the graph
			ICaptureGraphBuilder2 icgb2 = new CaptureGraphBuilder2() as ICaptureGraphBuilder2;

			try
			{
				// Link the ICaptureGraphBuilder2 to the IFilterGraph2
				hr = icgb2.SetFiltergraph(m_FilterGraph);
				DsError.ThrowExceptionForHR(hr);

				// Add the filters necessary to render the file.  This function will
				// work with a number of different file types.
				IBaseFilter sourceFilter = null;
				hr = m_FilterGraph.AddSourceFilter(FileName, FileName, out sourceFilter);
				DsError.ThrowExceptionForHR(hr);

				// Get the SampleGrabber interface
				m_sampGrabber = (ISampleGrabber)new SampleGrabber();
				IBaseFilter baseGrabFlt = (IBaseFilter)m_sampGrabber;

				// Configure the Sample Grabber
				ConfigureSampleGrabber(m_sampGrabber);

				// Add it to the filter
				hr = m_FilterGraph.AddFilter(baseGrabFlt, "Ds.NET Grabber");
				DsError.ThrowExceptionForHR(hr);

				// Add the null renderer to the graph
				IBaseFilter nullrenderer = new NullRenderer() as IBaseFilter;
				hr = m_FilterGraph.AddFilter(nullrenderer, "Null renderer");
				DsError.ThrowExceptionForHR(hr);

				// Connect the pieces together, use the default renderer
				hr = icgb2.RenderStream(null, null, sourceFilter, baseGrabFlt, nullrenderer);
				DsError.ThrowExceptionForHR(hr);

				// Now that the graph is built, read the dimensions of the bitmaps we'll be getting
				SaveSizeInfo(m_sampGrabber);

				// Grab some other interfaces
				m_mediaEvent = m_FilterGraph as IMediaEvent;
				m_mediaCtrl = m_FilterGraph as IMediaControl;
			}
			finally
			{
				if (icgb2 != null)
				{
					Marshal.ReleaseComObject(icgb2);
					icgb2 = null;
				}
			}
#if DEBUG
			// Double check to make sure we aren't releasing something
			// important.
			GC.Collect();
			GC.WaitForPendingFinalizers();
#endif
		}

		// Set the options on the sample grabber
		private void ConfigureSampleGrabber(ISampleGrabber sampGrabber)
		{
			AMMediaType media;
			int hr;

			// Set the media type to Video/RBG24
			media = new AMMediaType();
			media.majorType = MediaType.Video;
			media.subType = MediaSubType.RGB24;
			media.formatType = FormatType.VideoInfo;
			hr = sampGrabber.SetMediaType(media);
			DsError.ThrowExceptionForHR(hr);

			DsUtils.FreeAMMediaType(media);
			media = null;

			// Configure the samplegrabber
			hr = sampGrabber.SetBufferSamples(true);
			DsError.ThrowExceptionForHR(hr);

			// Configure the samplegrabber callback
			hr = sampGrabber.SetCallback(this, 1);
			DsError.ThrowExceptionForHR(hr);
		}

		// Save the size parameters for use in SnapShot
		private void SaveSizeInfo(ISampleGrabber sampGrabber)
		{
			int hr;

			// Get the media type from the SampleGrabber
			AMMediaType media = new AMMediaType();

			hr = sampGrabber.GetConnectedMediaType(media);
			DsError.ThrowExceptionForHR(hr);

			try
			{

				if ((media.formatType != FormatType.VideoInfo) || (media.formatPtr == IntPtr.Zero))
				{
					throw new NotSupportedException("Unknown Grabber Media Format");
				}

				// Get the struct
				VideoInfoHeader videoInfoHeader = new VideoInfoHeader();
				Marshal.PtrToStructure(media.formatPtr, videoInfoHeader);

				// Grab the size info
				m_videoWidth = videoInfoHeader.BmiHeader.Width;
				m_videoHeight = videoInfoHeader.BmiHeader.Height;
				m_stride = videoInfoHeader.BmiHeader.ImageSize / m_videoHeight;
				m_ImageSize = videoInfoHeader.BmiHeader.ImageSize;
			}
			finally
			{
				DsUtils.FreeAMMediaType(media);
				media = null;
			}
		}

		// Shut down capture
		private void CloseInterfaces()
		{
			int hr;

			lock (this)
			{
				if (m_State != GraphState.Exiting)
				{
					m_State = GraphState.Exiting;

					// Release the thread (if the thread was started)
					if (m_mre != null)
					{
						m_mre.Set();
					}
				}

				if (m_mediaCtrl != null)
				{
					// Stop the graph
					hr = m_mediaCtrl.Stop();
					m_mediaCtrl = null;
				}

				if (m_sampGrabber != null)
				{
					Marshal.ReleaseComObject(m_sampGrabber);
					m_sampGrabber = null;
				}

				if (m_FilterGraph != null)
				{
					Marshal.ReleaseComObject(m_FilterGraph);
					m_FilterGraph = null;
				}
			}
			GC.Collect();
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

			m_FrameCounter++;

			return 0;
		}

		private int m_FrameCounter = 0;

		public bool GetCurrentFrame(out Bitmap cameraFrame, out int frameNumber, out FrameProcessingStatus status)
		{
			if (!IsRunning)
			{
				cameraFrame = null;
				frameNumber = -1;
				status = FrameProcessingStatus.Empty;

				return false;
			}

			var sts = new FrameProcessingStatus();
			cameraFrame = null;

			frameNumber = m_FrameCounter;
			status = sts;

			if (m_FrameCounter > 0)
			{
				ImageStatus imgStatus;
				cameraFrame = NativeHelpers.GetCurrentImage(out imgStatus);
				status = new FrameProcessingStatus(imgStatus);				
			}

			return true;
		}

		public bool DisableOcr()
		{
			return false;
		}

		public bool LockIntegration()
		{
			if (m_FullAviSimulation)
			{
				NativeHelpers.LockIntegration();
				return true;
			}
			else
				return false;
		}

		public bool UnlockIntegration()
		{
			if (m_FullAviSimulation)
			{
				NativeHelpers.UnlockIntegration();
				return true;
			}
			else
				return false;
		}

		public bool StartIntegrationCalibration(int cameraIntegration)
		{
			if (m_FullAviSimulation)
			{
				return NativeHelpers.StartIntegrationCalibration(cameraIntegration);
			}
			else
				return false;
		}

		public bool StopIntegrationCalibration()
		{
			if (m_FullAviSimulation)
			{
				return NativeHelpers.StopIntegrationCalibration();
			}
			else
				return false;
		}
	}
}
