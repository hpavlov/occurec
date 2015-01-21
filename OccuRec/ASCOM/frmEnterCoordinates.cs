using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OccuRec.Utilities;

namespace OccuRec.ASCOM
{
    public partial class frmEnterCoordinates : Form
    {
        public frmEnterCoordinates()
        {
            InitializeComponent();
        }

        public double RAHours { get; private set; }
        public double DEDeg { get; private set; }
		public bool IsSyncMode { get; set; }

	    private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                RAHours = AstroConvert.ToRightAcsension(tbxRA.Text);    
            }
            catch
            {
                MessageBox.Show(this, "Please enter valid Right Ascension e.g. 23 19 12", "OccuRec", MessageBoxButtons.OK, MessageBoxIcon.Error );
                tbxRA.Focus();
                return;
            }

            try
            {
                DEDeg = AstroConvert.ToDeclination(tbxDec.Text);
            }
            catch
            {
                MessageBox.Show(this, "Please enter valid Declination e.g. +04 19 54", "OccuRec", MessageBoxButtons.OK, MessageBoxIcon.Error);
                tbxDec.Focus();
                return;
            }

	        Properties.Settings.Default.SlewLastRA = RAHours;
			Properties.Settings.Default.SlewLastDE = DEDeg;
	        Properties.Settings.Default.Save();

            DialogResult = DialogResult.OK;
            Close();
        }

		private void frmEnterCoordinates_Load(object sender, EventArgs e)
		{
			if (!double.IsNaN(Properties.Settings.Default.SlewLastRA))
				tbxRA.Text = AstroConvert.ToStringValue(Properties.Settings.Default.SlewLastRA, "HH MM SS");
			if (!double.IsNaN(Properties.Settings.Default.SlewLastDE))
				tbxDec.Text = AstroConvert.ToStringValue(Properties.Settings.Default.SlewLastDE, "+DD MM SS");

			btnOK.Text = IsSyncMode ? "Sync" : "Slew";
		}
    }
}
