using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using OccuRec.Properties;
using DirectShowLib;

namespace OccuRec.Helpers
{
    public interface ISupportsCrossbar
    {
        void ConnectToCrossbarSource(int inputPinIndex);
        void LoadCrossbarSources(ComboBox comboBox);
    }

    public static class CrossbarHelper
    {
        internal class CrossbarPinEntry
        {
            public string PinName { get; set; }
            public int PinIndex { get; set; }

            public override string ToString()
            {
                return PinName;
            }
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

        private static bool IsVideoPin(PhysicalConnectorType connectorType)
        {
            switch (connectorType)
            {
                case PhysicalConnectorType.Video_Tuner:
                case PhysicalConnectorType.Video_Composite:
                case PhysicalConnectorType.Video_SVideo:
                case PhysicalConnectorType.Video_RGB:
                case PhysicalConnectorType.Video_YRYBY:
                case PhysicalConnectorType.Video_SerialDigital:
                case PhysicalConnectorType.Video_ParallelDigital:
                case PhysicalConnectorType.Video_SCSI:
                case PhysicalConnectorType.Video_AUX:
                case PhysicalConnectorType.Video_1394:
                case PhysicalConnectorType.Video_USB:
                case PhysicalConnectorType.Video_VideoDecoder:
                case PhysicalConnectorType.Video_VideoEncoder:
                    return true;
            }

            return false;
        }

        private static string GetPhysicalPinName(PhysicalConnectorType connectorType)
        {
            switch (connectorType)
            {
                case PhysicalConnectorType.Video_Tuner: return "Video Tuner";
                case PhysicalConnectorType.Video_Composite: return "Video Composite";
                case PhysicalConnectorType.Video_SVideo: return "S-Video";
                case PhysicalConnectorType.Video_RGB: return "Video RGB";
                case PhysicalConnectorType.Video_YRYBY: return "Video YRYBY";
                case PhysicalConnectorType.Video_SerialDigital: return "Video Serial Digital";
                case PhysicalConnectorType.Video_ParallelDigital: return "Video Parallel Digital";
                case PhysicalConnectorType.Video_SCSI: return "Video SCSI";
                case PhysicalConnectorType.Video_AUX: return "Video AUX";
                case PhysicalConnectorType.Video_1394: return "Video 1394";
                case PhysicalConnectorType.Video_USB: return "Video USB";
                case PhysicalConnectorType.Video_VideoDecoder: return "Video Decoder";
                case PhysicalConnectorType.Video_VideoEncoder: return "Video Encoder";

                case PhysicalConnectorType.Audio_Tuner: return "Audio Tuner";
                case PhysicalConnectorType.Audio_Line: return "Audio Line";
                case PhysicalConnectorType.Audio_Mic: return "Audio Microphone";
                case PhysicalConnectorType.Audio_AESDigital: return "Audio AES/EBU Digital";
                case PhysicalConnectorType.Audio_SPDIFDigital: return "Audio S/PDIF";
                case PhysicalConnectorType.Audio_SCSI: return "Audio SCSI";
                case PhysicalConnectorType.Audio_AUX: return "Audio AUX";
                case PhysicalConnectorType.Audio_1394: return "Audio 1394";
                case PhysicalConnectorType.Audio_USB: return "Audio USB";
                case PhysicalConnectorType.Audio_AudioDecoder: return "Audio Decoder";

                default: return "Unknown Type";
            }
        }

        private delegate void CrossbarCallback(IAMCrossbar crossbar);

        private static void DoCrossbarOperation(string deviceName, CrossbarCallback callback)
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

                // Render any preview pin of the device
                hr = captureGraphBuilder.RenderStream(PinCategory.Preview, MediaType.Video, theDevice, null, null);
                DsError.ThrowExceptionForHR(hr);

                IAMCrossbar crossbar = null;
                object o;

                hr = captureGraphBuilder.FindInterface(null, null, theDevice, typeof(IAMCrossbar).GUID, out o);
                if (hr >= 0)
                {
                    crossbar = (IAMCrossbar)o;
                    o = null;

                    if (crossbar != null)
                    {
                        callback(crossbar);
                        return;
                    }
                }

                callback(null);
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

        private static int FindVideoDecoderOutputPin(IAMCrossbar crossbar, out int routedToInputPin)
        {
            int rv = -1;
            routedToInputPin = -1;

            int outputPinsCount;
            int inputPinsCount;
            int hr = crossbar.get_PinCounts(out outputPinsCount, out inputPinsCount);
            DsError.ThrowExceptionForHR(hr);

            for (int i = 0; i < outputPinsCount; i++)
            {
                int relatedIndex;
                PhysicalConnectorType connectorType;
                hr = crossbar.get_CrossbarPinInfo(false, i, out relatedIndex, out connectorType);
                if (hr == 0)
                {
                    int inputPinIndex;
                    hr = crossbar.get_IsRoutedTo(i, out inputPinIndex);
                    DsError.ThrowExceptionForHR(hr);

                    Trace.WriteLine(string.Format("Crossbar Output Pin {0}: '{1}' routed to pin {2}, related index: {3}", i, GetPhysicalPinName(connectorType), inputPinIndex, relatedIndex));
                    if (connectorType == PhysicalConnectorType.Video_VideoDecoder)
                    {
                        rv = i;
                        routedToInputPin = inputPinIndex;
                    }
                }
            }

            return rv;
        }

        private static string GetParentFormName(Control control)
        {
            Control parentControl = control;
            while (parentControl != null && !(parentControl is Form))
            {
                parentControl = parentControl.Parent;
            }

            if (parentControl != null)
                return parentControl.Name;
            else
                return "Unidentified";
        }

        public static void LoadCrossbarSources(IAMCrossbar crossbar, ComboBox cbxCrossbarInput)
        {
            int connectedInputPin = -1;
            int videoDecoderOutPinIndex = FindVideoDecoderOutputPin(crossbar, out connectedInputPin);
            cbxCrossbarInput.Enabled = videoDecoderOutPinIndex != -1;

            Trace.WriteLine(string.Format("Loading crossbar dropdown on {0}\r\nVideo decoder output pin is {1} which is routed to pin {2}.",
                GetParentFormName(cbxCrossbarInput), videoDecoderOutPinIndex, connectedInputPin));

            int outputPinsCount;
            int inputPinsCount;
            int hr = crossbar.get_PinCounts(out outputPinsCount, out inputPinsCount);
            DsError.ThrowExceptionForHR(hr);

            for (int i = 0; i < inputPinsCount; i++)
            {
                int relatedIndex;
                PhysicalConnectorType connectorType;
                hr = crossbar.get_CrossbarPinInfo(true, i, out relatedIndex, out connectorType);
                if (hr == 0)
                {
                    Trace.WriteLine(string.Format("Crossbar Input Pin {0}: {1}", i, GetPhysicalPinName(connectorType)));

                    if (IsVideoPin(connectorType))
                    {
                        int addedIdnex = cbxCrossbarInput.Items.Add(
                            new CrossbarPinEntry()
                            {
                                PinIndex = i,
                                PinName = GetPhysicalPinName(connectorType)
                            }
                        );

                        if (connectedInputPin == i)
                            cbxCrossbarInput.SelectedIndex = addedIdnex;
                    }
                }
            }

            cbxCrossbarInput.Enabled = true;            
        }

        public static void LoadCrossbarSources(string deviceName, ComboBox cbxCrossbarInput)
        {
            DoCrossbarOperation(
                deviceName,
                delegate(IAMCrossbar crossbar)
                {
                    if (crossbar != null)
                    {
                        Trace.WriteLine("Found Crossbar for " + deviceName);

                        Settings.Default.UsesTunerCrossbar = true;
                        Settings.Default.Save();

                        LoadCrossbarSources(crossbar, cbxCrossbarInput);
                    }
                    else
                    {
                        UpdateNoCrossbarSettings(cbxCrossbarInput);

                        Settings.Default.UsesTunerCrossbar = false;
                        Settings.Default.Save();
                    }
                }
            );
        }

        public static void UpdateNoCrossbarSettings(ComboBox cbxCrossbarInput)
        {
            cbxCrossbarInput.Items.Clear();
            cbxCrossbarInput.Enabled = false;
        }

        public static IAMCrossbar SetupTunerAndCrossbar(ICaptureGraphBuilder2 graphBuilder, IBaseFilter deviceFilter)
        {
            if (Settings.Default.UsesTunerCrossbar)
            {
                object o;

                int hr = graphBuilder.FindInterface(null, null, deviceFilter, typeof(IAMCrossbar).GUID, out o);
                if (hr >= 0)
                {
                    IAMCrossbar crossbar = (IAMCrossbar)o;

                    if (crossbar != null)
                    {
                        hr = crossbar.Route(Settings.Default.CrossbarOutputPin, Settings.Default.CrossbarInputPin);
                        DsError.ThrowExceptionForHR(hr);

                        Trace.WriteLine(string.Format("SetupTunerAndCrossbar: Crossbar Input Pin {0} routed to Output Pin {1}.", Settings.Default.CrossbarInputPin, Settings.Default.CrossbarOutputPin));

                        return crossbar;
                    }
                }
            }

            return null;
        }

        public static void ConnectToCrossbarSource(string deviceName, int inputPinIndex)
        {
            DoCrossbarOperation(
                deviceName,
                delegate(IAMCrossbar crossbar)
                {
                    int routedTo;
                    int videoDecoderOutPinIndex = FindVideoDecoderOutputPin(crossbar, out routedTo);
                    int hr = crossbar.Route(videoDecoderOutPinIndex, inputPinIndex);
                    DsError.ThrowExceptionForHR(hr);

                    Trace.WriteLine(string.Format("ConnectToCrossbarSource: Crossbar Input Pin {0} routed to Output Pin {1}.", inputPinIndex, videoDecoderOutPinIndex));

                    Settings.Default.UsesTunerCrossbar = true;
                    Settings.Default.CrossbarInputPin = inputPinIndex;
                    Settings.Default.CrossbarOutputPin = videoDecoderOutPinIndex;
                    Settings.Default.Save();
                }
            );
        }

        public static void ConnectToCrossbarSource(IAMCrossbar crossbar, int inputPinIndex)
        {
            if (crossbar != null)
            {
                int routedTo;
                int videoDecoderOutPinIndex = FindVideoDecoderOutputPin(crossbar, out routedTo);
                int hr = crossbar.Route(videoDecoderOutPinIndex, inputPinIndex);
                DsError.ThrowExceptionForHR(hr);

                Trace.WriteLine(string.Format("ConnectToCrossbarSource: Crossbar Input Pin {0} routed to Output Pin {1}.", inputPinIndex, videoDecoderOutPinIndex));

                Settings.Default.UsesTunerCrossbar = true;
                Settings.Default.CrossbarInputPin = inputPinIndex;
                Settings.Default.CrossbarOutputPin = videoDecoderOutPinIndex;
                Settings.Default.Save();
            }
        }
    }
}
