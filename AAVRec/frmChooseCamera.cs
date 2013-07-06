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

            rbFileSIM.Enabled = Settings.Default.OcrSimulatorTestMode && File.Exists(Settings.Default.SimulatorFilePath);

            SetSettingsVisibility();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {            
            if (pnlCrossbar.Visible && cbxCrossbarInput.Enabled)
            {
                var selectedItem = (CrossbarHelper.CrossbarPinEntry)cbxCrossbarInput.SelectedItem;

                if (selectedItem == null)
                {
                    MessageBox.Show("Please select a crossbar source.");
                    cbxCameraModel.Focus();
                    return;
                }

                Settings.Default.CrossbarInputPin = selectedItem.PinIndex;
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
            else if (rbFileAVI.Checked)
                Settings.Default.FileFormat = "AVI";
            else if (rbFileSIM.Checked)
                Settings.Default.FileFormat = "SIM";

            RadioButton rbCodec = gbxCodecs
                                .Controls
                                .Cast<Control>()
                                .SingleOrDefault(x => x is RadioButton && ((RadioButton)x).Checked) as RadioButton;

            if (rbCodec != null && rbCodec.Tag is SystemCodecEntry)
                Settings.Default.PreferredCompressorDevice = ((SystemCodecEntry)rbCodec.Tag).DeviceName;
            else
                Settings.Default.PreferredCompressorDevice = VideoCodecs.UNCOMPRESSED_VIDEO;

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
            }
            else
            {
                gbxCodecs.Visible = false;
                gbxCodecs.SendToBack();
                gbxAAVSettings.Visible = true;
                gbxAAVSettings.BringToFront();
            }
        }

        void cbxCrossbarInput_SelectedIndexChanged(object sender, EventArgs e)
        {
            string deviceName = (string)cbxCaptureDevices.SelectedItem;

            if (!string.IsNullOrEmpty(deviceName))
            {
                CrossbarHelper.CrossbarPinEntry selectedItem = (CrossbarHelper.CrossbarPinEntry) cbxCrossbarInput.SelectedItem;
                if (selectedItem != null)
                {
                    Cursor = Cursors.WaitCursor;
                    Update();
                    try
                    {
                        CrossbarHelper.ConnectToCrossbarSource(deviceName, selectedItem.PinIndex);
                    }
                    finally
                    {
                        Cursor = Cursors.Default;
                    }   
                }
            }
        }

        private void cbxCaptureDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            string deviceName = (string)cbxCaptureDevices.SelectedItem;

            if (!string.IsNullOrEmpty(deviceName))
            {
                Cursor = Cursors.WaitCursor;
                Update();

                cbxCrossbarInput.Items.Clear();
                cbxCrossbarInput.SelectedIndexChanged -= new EventHandler(cbxCrossbarInput_SelectedIndexChanged);
                try
                {
                    CrossbarHelper.LoadCrossbarSources(deviceName, cbxCrossbarInput);
                    pnlCrossbar.Visible = cbxCrossbarInput.Items.Count > 0;
                }
                finally
                {
                    cbxCrossbarInput.SelectedIndexChanged += new EventHandler(cbxCrossbarInput_SelectedIndexChanged);
                    Cursor = Cursors.Default;
                }
            }
        }
    }
}
