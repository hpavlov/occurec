using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OccuRec.ASCOM;
using OccuRec.FrameAnalysis;
using OccuRec.Properties;

namespace OccuRec.Scheduling
{
    public partial class frmAddScheduleEntry : Form
    {
	    private IObservatoryController m_ObservatoryController;
	    private FrameAnalysisManager m_AnalysisManager;

	    public frmAddScheduleEntry()
	    {
		    InitializeComponent();
	    }

	    public frmAddScheduleEntry(IObservatoryController observatoryController, FrameAnalysisManager analysisManager)
        {
            InitializeComponent();

	        m_ObservatoryController = observatoryController;
		    m_AnalysisManager = analysisManager;

            cbxOperations.SelectedIndex = 0;
            lblUT.Visible = Settings.Default.DisplayTimeInUT;
			lblUT2.Visible = Settings.Default.DisplayTimeInUT;

			cbxAutoFocusing.Enabled = m_ObservatoryController.IsConnectedToFocuser();
			cbxAutoPulseGuiding.Enabled = m_ObservatoryController.IsConnectedToTelescope() && m_AnalysisManager.IsPulseGuidingCalibrated();

		    cbxAutoFocusing.Checked = false;
			cbxAutoPulseGuiding.Checked = false;

			SetTime(Settings.Default.DisplayTimeInUT ? DateTime.UtcNow : DateTime.Now);
        }

		private void SetTime(DateTime datetime)
		{
			nudSchHours.Value = datetime.Hour;
			nudSchMinutes.Value = datetime.Minute;
			nudSchSeconds.Value = datetime.Second;

			nudMidHours.Value = datetime.Hour;
			nudMidMins.Value = datetime.Minute;
			nudMidSecs.Value = datetime.Second;
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

		private DateTime GetTimeFromMidTime()
		{
			DateTime dateTimeNow = Settings.Default.DisplayTimeInUT ? DateTime.UtcNow : DateTime.Now;
			DateTime dateTime = dateTimeNow.Date;
			dateTime = dateTime.AddHours((int)nudMidHours.Value);
			dateTime = dateTime.AddMinutes((int)nudMidMins.Value);
			dateTime = dateTime.AddSeconds((int)nudMidSecs.Value);

			if (dateTimeNow > dateTime &&
				new TimeSpan(dateTimeNow.Ticks - dateTime.Ticks).TotalHours > 1)
			{
				// Make sure operations scheduled 'after midnight' work correctly
				// as long as they are not more than 23 hours in future
				dateTime = dateTime.AddDays(1);
			}

			int wingInSecs = (int)nudWingsMinutes.Value * 60 + (int)nudWingsSeconds.Value;

			return dateTime.AddSeconds(-1 * wingInSecs);
		}

        private void button1_Click(object sender, EventArgs e)
        {
			if (tcEntryType.SelectedTab == tabStartDuration)
			{
				ScheduleByStartDuration();
			}
			else if (tcEntryType.SelectedTab == tabMidTimeWings)
			{
				ScheduleByMidTimeWings();
			}
        }

		private void ScheduleByStartDuration()
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
				Scheduler.ScheduleRecording(scheduleTime.ToLocalTime(), duration, cbxAutoFocusing.Checked, cbxAutoPulseGuiding.Checked);
			else
				Scheduler.ScheduleRecording(scheduleTime, duration, cbxAutoFocusing.Checked, cbxAutoPulseGuiding.Checked);

			DialogResult = DialogResult.OK;
			Close();			
		}

		private void ScheduleByMidTimeWings()
		{
			DateTime scheduleTime = GetTimeFromMidTime();

			if ((Settings.Default.DisplayTimeInUT && scheduleTime < DateTime.UtcNow) ||
				(!Settings.Default.DisplayTimeInUT && scheduleTime < DateTime.Now))
			{
				MessageBox.Show("Start time must be in future");
				nudSchSeconds.Focus();
				return;
			}

			int duration = 2 * ((int)nudWingsMinutes.Value * 60 + (int)nudWingsSeconds.Value);

			if (Settings.Default.DisplayTimeInUT)
				Scheduler.ScheduleRecording(scheduleTime.ToLocalTime(), duration, cbxAutoFocusing.Checked, cbxAutoPulseGuiding.Checked);
			else
				Scheduler.ScheduleRecording(scheduleTime, duration, cbxAutoFocusing.Checked, cbxAutoPulseGuiding.Checked);

			DialogResult = DialogResult.OK;
			Close();

		}
    }
}
