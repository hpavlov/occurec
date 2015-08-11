using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OccuRec.Helpers;
using OccuRec.Helpers.CalSpec;
using OccuRec.Properties;
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

	        LastSlewPositions instance = LastSlewPositions.Load();
            instance.RegisterLatest(RAHours, DEDeg);
            instance.Save();

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

            stmiCalSpec.Visible = Settings.Default.SpectraUseAid && Math.Abs(Settings.Default.AavObsLongitude) > 0 && Math.Abs(Settings.Default.AavObsLatitude) > 0;
		}

        private void stmiCalSpec_DropDownOpening(object sender, EventArgs e)
        {
            double alt, az;

            stmiCalSpec.DropDownItems.Clear();

            foreach (CalSpecStar star in CalSpecDatabase.Instance.Stars)
            {
                xephem.AltAzCoords(star.RA_J2000_Hours * 15, star.DE_J2000_Deg, Settings.Default.AavObsLatitude, Settings.Default.AavObsLongitude, DateTime.UtcNow, out alt, out az);

                if (alt < 20) continue;

                ToolStripItem fs = new ToolStripMenuItem();
                fs.Text = string.Format("{0} (Alt:{1} Az:{2})", star.AbsFluxStarId, (int)alt, (int)az);
                fs.Tag = star;
                fs.Click += fs_Click;
                stmiCalSpec.DropDownItems.Add(fs);
            }
        }

        void fs_Click(object sender, EventArgs e)
        {
            var tsi = sender as ToolStripMenuItem;
            if (tsi != null)
            {
                var star = (CalSpecStar) tsi.Tag;
                
                double ra_rad = star.RA_J2000_Hours * 15 * Math.PI / 180;
                double de_rad = star.DE_J2000_Deg * Math.PI / 180;

                xephem.Precession(2000, xephem.JD_from_Date(DateTime.Today), ref ra_rad, ref de_rad, 0, 0);

                tbxRA.Text = AstroConvert.ToStringValue(ra_rad * 180 / (15 * Math.PI), "HH MM SS");
                tbxDec.Text = AstroConvert.ToStringValue(de_rad * 180 / Math.PI, "+DD MM SS");
            }
        }

        private void stmiPreviousPositions_DropDownOpening(object sender, EventArgs e)
        {
            stmiPreviousPositions.DropDownItems.Clear();
            LastSlewPositions instance = LastSlewPositions.Load();
            foreach (SlewPosition pos in instance.Positions)
            {
                ToolStripItem ts = new ToolStripMenuItem();
                ts.Text = string.Format("RA={0} DEC={1}", AstroConvert.ToStringValue(pos.RA, "HH MM SS"), AstroConvert.ToStringValue(pos.DEC, "+DD MM SS"));
                ts.Tag = pos;
                ts.Click += ts_Click;
                stmiPreviousPositions.DropDownItems.Add(ts);
            }
        }

        void ts_Click(object sender, EventArgs e)
        {
            var tsi = sender as ToolStripMenuItem;
            if (tsi != null)
            {
                var pos = (SlewPosition)tsi.Tag;

                tbxRA.Text = AstroConvert.ToStringValue(pos.RA, "HH MM SS");
                tbxDec.Text = AstroConvert.ToStringValue(pos.DEC, "+DD MM SS");
            }
        }
    }
}
