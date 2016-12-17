using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OccuRec.Helpers;

namespace OccuRec.Drivers.QHYVideo
{
    public partial class frmChooseQHYCamera : Form
    {
        public frmChooseQHYCamera()
        {
            InitializeComponent();
        }

        private void frmChooseQHYCamera_Load(object sender, EventArgs e)
        {
            cbxQHYCamera.Items.AddRange(QHYCameraManager.Instance.ListAvailableCameras().ToArray());
            if (cbxQHYCamera.Items.Count > 0)
            {
                cbxQHYCamera.SelectedIndex = 0;
                btnOK.Enabled = true;
            }
            else
            {
                btnOK.Enabled = false;
            }
        }

        private void cbxQHYCamera_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxQHYCamera.SelectedIndex > -1)
            {
                string cameraId = (string) cbxQHYCamera.SelectedItem;
                IntPtr handle = QHYPInvoke.OpenQHYCCD(cameraId);
                if (handle != IntPtr.Zero)
                {
                    try
                    {
                        cbxBPP.Items.Clear();
                        if (QHYPInvoke.IsQHYCCDControlAvailable(handle, CONTROL_ID.CAM_8BITS))
                            cbxBPP.Items.Add("16");
                        if (QHYPInvoke.IsQHYCCDControlAvailable(handle, CONTROL_ID.CAM_16BITS))
                            cbxBPP.Items.Add("8");
                        cbxBPP.Enabled = cbxBPP.Items.Count > 0;
                        if (cbxBPP.Items.Count > 0)
                            cbxBPP.SelectedIndex = 0;

                        cbxTiming.Items.Clear();
                        if (QHYPInvoke.IsQHYCCDControlAvailable(handle, CONTROL_ID.CAM_GPS))
                            cbxTiming.Items.Add("GPS");
                        cbxTiming.Items.Add("NTP");
                        cbxTiming.Enabled = cbxTiming.Items.Count > 0;
                        if (cbxTiming.Items.Count > 0)
                            cbxTiming.SelectedIndex = 0;

                        cbxBinning.Items.Clear();
                        if (QHYPInvoke.IsQHYCCDControlAvailable(handle, CONTROL_ID.CAM_BIN4X4MODE))
                            cbxBinning.Items.Add("4x4");
                        if (QHYPInvoke.IsQHYCCDControlAvailable(handle, CONTROL_ID.CAM_BIN3X3MODE))
                            cbxBinning.Items.Add("3x3");
                        if (QHYPInvoke.IsQHYCCDControlAvailable(handle, CONTROL_ID.CAM_BIN2X2MODE))
                            cbxBinning.Items.Add("2x2");
                        if (QHYPInvoke.IsQHYCCDControlAvailable(handle, CONTROL_ID.CAM_BIN1X1MODE))
                            cbxBinning.Items.Add("1x1");
                        cbxBinning.Enabled = cbxBinning.Items.Count > 0;
                        if (cbxBinning.Items.Count > 0)
                            cbxBinning.SelectedIndex = 0;
                    }
                    finally
                    {
                        QHYPInvoke.CloseQHYCCD(handle);
                    }
                }
            }
        }

        public string CameraId { get; private set; }
        public int BinningMode { get; private set; }
        public int BPP { get; private set; }
        public bool UseGPS { get; private set; }
        public bool UseCooling { get; private set; }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (cbxQHYCamera.SelectedIndex == -1)
            {
                MessageBox.Show("Please connect a camera and select it.", "OccuRec", MessageBoxButtons.OK, MessageBoxIcon.Error);
                cbxQHYCamera.Focus();
            }
            else
                CameraId = (string) cbxQHYCamera.SelectedItem;

            if (cbxBinning.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a binning mode.", "OccuRec", MessageBoxButtons.OK, MessageBoxIcon.Error);
                cbxBinning.Focus();
            }
            else
                BinningMode = int.Parse(((string)cbxBinning.SelectedItem).Substring(0, 1));

            if (cbxBPP.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a Bpp mode.", "OccuRec", MessageBoxButtons.OK, MessageBoxIcon.Error);
                cbxBPP.Focus();
            }
            else
                BPP = int.Parse(((string)cbxBPP.SelectedItem));

            if (cbxTiming.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a Timing mode.", "OccuRec", MessageBoxButtons.OK, MessageBoxIcon.Error);
                cbxTiming.Focus();
            }
            else
                UseGPS = (string)cbxTiming.SelectedItem == "GPS";

            UseCooling = cbxCooling.Checked;

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
