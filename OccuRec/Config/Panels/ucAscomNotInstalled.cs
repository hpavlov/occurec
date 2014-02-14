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
