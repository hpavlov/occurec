using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AAVRec.Drivers.DirectShowCapture.VideoCaptureImpl;
using AAVRec.Helpers;
using AAVRec.Properties;
using DirectShowLib;

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

            tbxOutputLocation.Text = Settings.Default.OutputLocation;
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
            if (string.IsNullOrEmpty(cbxCameraModel.Text))
            {
                MessageBox.Show("Please specify a camera model.");
                cbxCameraModel.Focus();
            }


            Settings.Default.CameraModel = cbxCameraModel.Text;
            Settings.Default.PreferredCaptureDevice = (string)cbxCaptureDevices.SelectedItem;

            if (rbFileAAV.Checked)
                Settings.Default.FileFormat = "AAV";
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
            }

            Settings.Default.OutputLocation = tbxOutputLocation.Text;
            Settings.Default.IsIntegrating = cbxIsIntegrating.Checked;
            Settings.Default.MonochromePixelsType = (LumaConversionMode)cbxMonochromeConversion.SelectedIndex;

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
    }
}
