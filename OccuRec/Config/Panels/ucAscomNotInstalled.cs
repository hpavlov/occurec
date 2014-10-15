/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OccuRec.Config.Panels
{
	public partial class ucAscomNotInstalled : SettingsPanel
	{
		public ucAscomNotInstalled()
		{
			InitializeComponent();
		}

		private void llAscomWebSite_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("http://www.ascom-standards.org/");
		}
	}
}
