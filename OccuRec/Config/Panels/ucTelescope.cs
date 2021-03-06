﻿/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

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
    public partial class ucTelescope : SettingsPanel
    {
        public ucTelescope()
        {
            InitializeComponent();
        }

        public override void LoadSettings()
        {
            nudPulseDuration.SetNUDValue(Settings.Default.TelPulseDuration);
            nudPulseGuideFast.SetNUDValue(Settings.Default.TelPulseFasterRate);
            nudPulseSlowRate.SetNUDValue(Settings.Default.TelPulseSlowRate);
            nudPulseSlowestRate.SetNUDValue(Settings.Default.TelPulseSlowestRate);
        }

        public override void SaveSettings()
        {            
            Settings.Default.TelPulseDuration = (int)nudPulseDuration.Value;
            Settings.Default.TelPulseFasterRate = (float)nudPulseGuideFast.Value;
            Settings.Default.TelPulseSlowRate = (float)nudPulseSlowRate.Value;
            Settings.Default.TelPulseSlowestRate = (float)nudPulseSlowestRate.Value;
        }
    }
}
