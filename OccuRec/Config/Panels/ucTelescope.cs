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

namespace OccuRec.Config.Panels
{
    public partial class ucTelescope : SettingsPanel
    {
        public ucTelescope()
        {
            InitializeComponent();
        }

        public override void LoadSettings()
        {
            nudTelescopePingRate.SetNUDValue(Settings.Default.TelescopePingRateSeconds);
        }

        public override void SaveSettings()
        {
            Settings.Default.TelescopePingRateSeconds = (int)nudTelescopePingRate.Value;
        }
    }
}
