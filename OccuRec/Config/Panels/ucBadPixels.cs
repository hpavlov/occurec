using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OccuRec.Helpers;
using OccuRec.Properties;
using OccuRec.Utilities;

namespace OccuRec.Config.Panels
{
    public partial class ucBadPixels : SettingsPanel
    {
        public ucBadPixels()
        {
            InitializeComponent();
        }

        public override void LoadSettings()
        {
            cbxEnableBadPixelsControl.Checked = Settings.Default.EnableBadPixelsControl;
            tbxBadPixelsFile.Text = Settings.Default.BadPixelsFileName;
            rbPlus.Checked = Settings.Default.BadPixelsMarkerShapePlus;
            rbCross.Checked = Settings.Default.BadPixelsMarkerShapeCross;
            rbCircle.Checked = Settings.Default.BadPixelsMarkerShapeCircle;
            nupSize.Value = Settings.Default.BadPixelsMarkerSize;
            cbxBlinking.Checked = Settings.Default.BadPixelsMarkerBlinking;

            gbxBadPixels.Enabled = cbxEnableBadPixelsControl.Checked;
        }

        public override void SaveSettings()
        {
            Settings.Default.EnableBadPixelsControl = cbxEnableBadPixelsControl.Checked;
            Settings.Default.BadPixelsFileName = tbxBadPixelsFile.Text;
            Settings.Default.BadPixelsMarkerShapePlus = rbPlus.Checked;
            Settings.Default.BadPixelsMarkerShapeCross = rbCross.Checked;
            Settings.Default.BadPixelsMarkerShapeCircle = rbCircle.Checked;
            Settings.Default.BadPixelsMarkerSize = nupSize.Value;
            Settings.Default.BadPixelsMarkerBlinking = cbxBlinking.Checked;

        }

        public override bool ValidateSettings()
        {
            if (cbxEnableBadPixelsControl.Checked)
            {
                if (!File.Exists(tbxBadPixelsFile.Text))
                {
                    MessageBox.Show("The 'Bad Pixels File' must be an existing file.");
                    tbxBadPixelsFile.Focus();
                    return false;
                }
            }

            return true;
        }

        private void cbxUseBadPixelsAids_CheckedChanged(object sender, EventArgs e)
        {
            gbxBadPixels.Enabled = cbxEnableBadPixelsControl.Checked;
        }

        private void btnBrowseBadPixelsFile_Click(object sender, EventArgs e)
        {
            if (ofdBrowseBadPixelsFile.ShowDialog() == DialogResult.OK)
            {
                tbxBadPixelsFile.Text = ofdBrowseBadPixelsFile.FileName;
            }
        }
    }
}
