using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OccuRec.Properties;

namespace OccuRec.Config.Panels
{
	public partial class ucNTPTime : SettingsPanel
	{
		public ucNTPTime()
		{
			InitializeComponent();
		}

		public override void LoadSettings()
		{
			tbxNTPServer.Text = Settings.Default.NTPServer;
		}

		public override void SaveSettings()
		{
			Settings.Default.NTPServer = tbxNTPServer.Text;
		}

		private void llblFindNTP_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start("http://www.pool.ntp.org/en/");
		}
	}
}
