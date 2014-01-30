using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OccuRec.Helpers;
using OccuRec.Properties;
using OccuRec.Utilities;

namespace OccuRec.Config.Panels
{
    public partial class ucFocusing : SettingsPanel
    {
        public ucFocusing()
        {
            InitializeComponent();
        }

        public override void LoadSettings()
        {
            nudFocuserSmallestStep.SetNUDValue(Settings.Default.FocuserSmallestStep);
            nudFocuserSmallStep.SetNUDValue(Settings.Default.FocuserSmallStep);
            nudFocuserLargeStep.SetNUDValue(Settings.Default.FocuserLargeStep);
            cbxTempIn.SelectedIndex = cbxTempIn.Items.IndexOf(Settings.Default.FocuserTemperatureIn);
        }

        public override void SaveSettings()
        {
            Settings.Default.FocuserSmallestStep = (int)nudFocuserSmallestStep.Value;
            Settings.Default.FocuserSmallStep = (int)nudFocuserSmallStep.Value;
            Settings.Default.FocuserLargeStep = (int)nudFocuserLargeStep.Value;
            Settings.Default.FocuserTemperatureIn = (string)cbxTempIn.SelectedItem;
        }
    }
}
