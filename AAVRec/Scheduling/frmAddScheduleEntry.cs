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

			SetTime(Settings.Default.DisplayTimeInUT ? DateTime.UtcNow : DateTime.Now);
        }

		private void SetTime(DateTime datetime)
		{
			nudSchHours.Value = datetime.Hour;
			nudSchMinutes.Value = datetime.Minute;
			nudSchSeconds.Value = datetime.Second;
		}

		private DateTime GetTime()
		{
			DateTime dateTimeNow = Settings.Default.DisplayTimeInUT ? DateTime.UtcNow : DateTime.Now;
			DateTime dateTime = dateTimeNow.Date;
			dateTime = dateTime.AddHours((int)nudSchHours.Value);
			dateTime = dateTime.AddMinutes((int)nudSchMinutes.Value);
			dateTime = dateTime.AddSeconds((int)nudSchSeconds.Value);

			if (dateTimeNow > dateTime &&
			    new TimeSpan(dateTimeNow.Ticks - dateTime.Ticks).TotalHours > 1)
			{
				// Make sure operations scheduled 'after midnight' work correctly
				// as long as they are not more than 23 hours in future
				dateTime = dateTime.AddDays(1);
			}

			return dateTime;
		}

        private void button1_Click(object sender, EventArgs e)
        {
	        DateTime scheduleTime = GetTime();

			if ((Settings.Default.DisplayTimeInUT && scheduleTime < DateTime.UtcNow) ||
				(!Settings.Default.DisplayTimeInUT && scheduleTime < DateTime.Now))
            {
                MessageBox.Show("Start time must be in future");
                nudSchSeconds.Focus();
                return;
            }

			int duration = (int)nudDurMinutes.Value * 60 + (int)nudDurSeconds.Value;

            if (Settings.Default.DisplayTimeInUT)
				Scheduler.ScheduleRecording(scheduleTime.ToLocalTime(), duration);
            else
				Scheduler.ScheduleRecording(scheduleTime, duration);

            DialogResult = DialogResult.OK;
            Close();
        }

		private void frmAddScheduleEntry_Load(object sender, EventArgs e)
		{

		}
    }
}
