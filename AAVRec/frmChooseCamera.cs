using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using AAVRec.Drivers.DirectShowCapture.VideoCaptureImpl;
using AAVRec.Helpers;
using AAVRec.Properties;
using DirectShowLib;
using System.Diagnostics;

namespace AAVRec
{
    public partial class frmChooseCamera : Form
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

        public frmChooseCamera()
        {
            InitializeComponent();

            if (string.IsNullOrEmpty(Settings.Default.OutputLocation))
            {
                Settings.Default.OutputLocation = Path.GetFullPath(string.Format("{0}\\Videos", AppDomain.CurrentDomain.BaseDirectory));
                if (!Directory.Exists(Settings.Default.OutputLocation))
                    Directory.CreateDirectory(Settings.Default.OutputLocation);
            }

            cbxCameraModel.Text = Settings.Default.CameraModel;

            cbxCaptureDevices.Items.Clear();
            foreach (DsDevice ds in DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice))
            {
                cbxCaptureDevices.Items.Add(ds.Name);
            }

            if (cbxCaptureDevices.Items.Count > 0)
            {
                if (cbxCaptureDevices.Items.Contains(Settings.Default.PreferredCaptureDevice))
                    cbxCaptureDevices.SelectedIndex = cbxCaptureDevices.Items.IndexOf(Settings.Default.PreferredCaptureDevice);
                else
                    cbxCaptureDevices.SelectedIndex = 0;
            }

            cbxMonochromeConversion.SelectedIndex = (int)Settings.Default.MonochromePixelsType;

            RadioButton rbCodec;

            List<SystemCodecEntry> systemCodecs = VideoCodecs.GetSupportedVideoCodecs();
            foreach (SystemCodecEntry codec in systemCodecs)
            {
                rbCodec = gbxCodecs
                    .Controls
                    .Cast<Control>()
                    .SingleOrDefault(x => x is RadioButton && string.Equals(x.Text, codec.DeviceName.ToString())) as RadioButton;

                if (rbCodec != null)
                {
                    rbCodec.Enabled = codec.DeviceName != null && codec.IsInstalled;
                    rbCodec.Checked = codec.DeviceName == Settings.Default.PreferredCompressorDevice;
                    rbCodec.Tag = codec;
                }
            }

            rbCodec = gbxCodecs
                        .Controls
                        .Cast<Control>()
                        .SingleOrDefault(x => x is RadioButton && ((RadioButton)x).Checked) as RadioButton;

            if (rbCodec == null)
                rbCodecUncompressed.Checked = true;

            rbFileAAV.Checked = Settings.Default.FileFormat == "AAV";
            rbFileAVI.Checked = Settings.Default.FileFormat == "AVI";

            cbxIsIntegrating.Checked = Settings.Default.IsIntegrating;
            cbxFlipHorizontally.Checked = Settings.Default.FlipHorizontally;
            cbxFlipVertically.Checked = Settings.Default.FlipVertically;

            tbxOutputLocation.Text = Settings.Default.OutputLocation;

            SetSettingsVisibility();
        }

        private void btnBrowseOutputFolder_Click(object sender, EventArgs e)
        {
            folderBrowserDialog.SelectedPath = tbxOutputLocation.Text;

            if (folderBrowserDialog.ShowDialog(this) == DialogResult.OK)
            {
                tbxOutputLocation.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {            
            if (pnlCrossbar.Visible && cbxCrossbarInput.Enabled)
            {
                if (cbxCrossbarInput.SelectedIndex == -1)
                {
                    MessageBox.Show("Please select a crossbar source.");
                    cbxCameraModel.Focus();
                    return;
                }
            }

            Settings.Default.CameraModel = cbxCameraModel.Text;
            Settings.Default.PreferredCaptureDevice = (string)cbxCaptureDevices.SelectedItem;

            if (rbFileAAV.Checked)
            {
                if (string.IsNullOrEmpty(cbxCameraModel.Text))
                {
                    MessageBox.Show("Please specify a camera model.");
                    cbxCameraModel.Focus();
                    return;
                }

                Settings.Default.FileFormat = "AAV";
            }
            else
                Settings.Default.FileFormat = "AVI";

            RadioButton rbCodec = gbxCodecs
                                .Controls
                                .Cast<Control>()
                                .SingleOrDefault(x => x is RadioButton && ((RadioButton)x).Checked) as RadioButton;

            if (rbCodec != null && rbCodec.Tag is SystemCodecEntry)
                Settings.Default.PreferredCompressorDevice = ((SystemCodecEntry)rbCodec.Tag).DeviceName;
            else
                Settings.Default.PreferredCompressorDevice = VideoCodecs.UNCOMPRESSED_VIDEO;

            if (!Directory.Exists(tbxOutputLocation.Text))
            {
                MessageBox.Show("Output location must be an existing directory.");
                tbxOutputLocation.Focus();
                return;
            }

            Settings.Default.OutputLocation = tbxOutputLocation.Text;
            Settings.Default.IsIntegrating = cbxIsIntegrating.Checked;
            Settings.Default.MonochromePixelsType = (LumaConversionMode)cbxMonochromeConversion.SelectedIndex;
            Settings.Default.FlipHorizontally = cbxFlipHorizontally.Checked;
            Settings.Default.FlipVertically = cbxFlipVertically.Checked;

            Settings.Default.Save();

            DialogResult = DialogResult.OK;
            Close();
        }

        private void rbFileAAV_CheckedChanged(object sender, EventArgs e)
        {
            SetSettingsVisibility();
        }

        private void rbFileAVI_CheckedChanged(object sender, EventArgs e)
        {
            SetSettingsVisibility();
        }

        private void SetSettingsVisibility()
        {
            if (rbFileAVI.Checked)
            {
                gbxAAVSettings.Visible = false;
                gbxAAVSettings.SendToBack();
                gbxCodecs.Visible = true;
                gbxCodecs.BringToFront();

                cbxFlipHorizontally.Visible = false;
            }
            else
            {
                gbxCodecs.Visible = false;
                gbxCodecs.SendToBack();
                gbxAAVSettings.Visible = true;
                gbxAAVSettings.BringToFront();

                cbxFlipHorizontally.Visible = true;
            }
        }

        private bool IsVideoPin(PhysicalConnectorType connectorType)
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

        private string GetPhysicalPinName(PhysicalConnectorType connectorType)
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

        private delegate void CrossbarCallback(IAMCrossbar crossbar);

        private void DoCrossbarOperation(string deviceName, CrossbarCallback callback)
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

                IAMTVTuner tuner = null;
                IAMCrossbar crossbar = null;
                object o;

                hr = captureGraphBuilder.FindInterface(null, null, theDevice, typeof(IAMTVTuner).GUID, out o);
                if (hr >= 0)
                {
                    tuner = (IAMTVTuner)o;
                    o = null;

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
                }

                callback(null);
            }
            finally
            {
                Marshal.ReleaseComObject(theDevice);
                Marshal.ReleaseComObject(graphBuilder);
                Marshal.ReleaseComObject(captureGraphBuilder);
            }                
        }

        private int FindVideoDecoderOutputPin(IAMCrossbar crossbar)
        {
            int rv = -1;

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
                    crossbar.get_IsRoutedTo(i, out inputPinIndex);

                    Trace.WriteLine(string.Format("Crossbar Output Pin {0}: '{1}' routed to pin {2}", i, GetPhysicalPinName(connectorType), inputPinIndex));
                    if (connectorType == PhysicalConnectorType.Video_VideoDecoder)
                        rv = i;
                }                
            }

            return rv;
        }

        private void LoadCrossbarSources(string deviceName)
        {
            DoCrossbarOperation(
                deviceName,
                delegate(IAMCrossbar crossbar)
                {
                    pnlCrossbar.Visible = crossbar != null;

                    if (crossbar != null)
                    {
                        Trace.WriteLine("Found Crossbar");

                        Settings.Default.UsesTunerCrossbar = true;
                        Settings.Default.Save();

                        cbxCrossbarInput.Items.Clear();
                        cbxCrossbarInput.SelectedIndexChanged -=new EventHandler(cbxCrossbarInput_SelectedIndexChanged);

                        try
                        {
                            int connectedInputPin = -1;
                            int videoDecoderOutPinIndex = FindVideoDecoderOutputPin(crossbar);
                            cbxCrossbarInput.Enabled = videoDecoderOutPinIndex != -1;
                            if (videoDecoderOutPinIndex != -1)
                                crossbar.get_IsRoutedTo(videoDecoderOutPinIndex, out connectedInputPin);

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
                        }
                        finally
                        {
                            cbxCrossbarInput.SelectedIndexChanged +=new EventHandler(cbxCrossbarInput_SelectedIndexChanged);
                        }
                    }
                    else
                    {
                        Settings.Default.UsesTunerCrossbar = false;
                        Settings.Default.Save();
                    }
                }
            );
        }

        void cbxCrossbarInput_SelectedIndexChanged(object sender, EventArgs e)
        {
            string deviceName = (string)cbxCaptureDevices.SelectedItem;

            if (!string.IsNullOrEmpty(deviceName))
            {
                CrossbarPinEntry selectedItem = (CrossbarPinEntry) cbxCrossbarInput.SelectedItem;
                if (selectedItem != null)
                {
                    Cursor = Cursors.WaitCursor;
                    Update();
                    try
                    {
                        ConnectToCrossbarSource(deviceName, selectedItem.PinIndex);
                    }
                    finally
                    {
                        Cursor = Cursors.Default;
                    }   
                }
            }
        }

        private void ConnectToCrossbarSource(string deviceName, int inputPinIndex)
        {
            DoCrossbarOperation(
                deviceName,
                delegate(IAMCrossbar crossbar)
                {
                    int videoDecoderOutPinIndex = FindVideoDecoderOutputPin(crossbar);
                    int hr = crossbar.Route(videoDecoderOutPinIndex, inputPinIndex);
                    DsError.ThrowExceptionForHR(hr);

                    Trace.WriteLine(string.Format("Crossbar Input Pin {0} routed to Output Pin {1}.", inputPinIndex, videoDecoderOutPinIndex));

                    Settings.Default.UsesTunerCrossbar = true;
                    Settings.Default.CrossbarInputPin = inputPinIndex;
                    Settings.Default.CrossbarOutputPin = videoDecoderOutPinIndex;
                    Settings.Default.Save();
                }
            );
        }

        private void cbxCaptureDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            string deviceName = (string)cbxCaptureDevices.SelectedItem;

            if (!string.IsNullOrEmpty(deviceName))
            {
                Cursor = Cursors.WaitCursor;
                Update();
                try
                {
                    LoadCrossbarSources(deviceName);
                }
                finally
                {
                    Cursor = Cursors.Default;
                }                
            }
        }
    }
}
