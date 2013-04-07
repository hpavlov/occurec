using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AAVRec.Properties;

namespace AAVRec.Scheduling
{
    public partial class frmAddScheduleEntry : Form
    {
        public frmAddScheduleEntry()
        {
            InitializeComponent();

            cbxOperations.SelectedIndex = 0;
            lblUT.Visible = Settings.Default.DisplayTimeInUT;

            if (Settings.Default.DisplayTimeInUT)
                dtpStartTime.Value = DateTime.UtcNow;
            else
                dtpStartTime.Value = DateTime.Now;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if ((Settings.Default.DisplayTimeInUT && dtpStartTime.Value < DateTime.UtcNow) ||
                (!Settings.Default.DisplayTimeInUT && dtpStartTime.Value < DateTime.Now))
            {
                MessageBox.Show("Start time must be in future");
                dtpStartTime.Focus();
                return;
            }

            if (Settings.Default.DisplayTimeInUT)
                Scheduler.ScheduleRecording(dtpStartTime.Value.ToLocalTime(), (int)nudDuration.Value);
            else
                Scheduler.ScheduleRecording(dtpStartTime.Value, (int)nudDuration.Value);

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
