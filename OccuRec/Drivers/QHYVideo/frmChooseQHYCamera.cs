using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

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
    }
}
