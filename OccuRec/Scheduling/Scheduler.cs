using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OccuRec.Properties;

namespace OccuRec.Scheduling
{
    public enum ScheduledAction
    {
        None,
        StartRecording,
        StopRecording
    }

    public class ScheduleEntry
    {
        public Guid OperaionId { get; set; }
        public ScheduledAction Action { get; set; }
        public DateTime ActionTime { get; set; }
		public bool AutoFocusing { get; set; }
		public bool AutoPulseGuiding { get; set; }

        public override string ToString()
        {
            string timeStr;
            if (Settings.Default.DisplayTimeInUT)
                timeStr = ActionTime.ToUniversalTime().ToString("HH:mm:ss") + " UT";
            else
                timeStr = ActionTime.ToString("HH:mm:ss");

            if (Action == ScheduledAction.StartRecording)
                return string.Format("Rec at {0}", timeStr);
            else if (Action == ScheduledAction.StopRecording)
                return string.Format("Stop at {0}", timeStr);
            else
                return "Invalid";
        }

        public string GetRemainingTime(out double remainingSeconds)
        {
            var leftTimeSpan = new TimeSpan(ActionTime.Ticks - DateTime.Now.Ticks);
	        remainingSeconds = leftTimeSpan.TotalSeconds;

            if (Action == ScheduledAction.StartRecording)
                return string.Format("Rec in {0}", DateTime.Today.AddTicks(leftTimeSpan.Ticks).ToString("HH:mm:ss"));
            else if (Action == ScheduledAction.StopRecording)
                return string.Format("Stop in {0}", DateTime.Today.AddTicks(leftTimeSpan.Ticks).ToString("HH:mm:ss"));
            else
                return "Invalid";
        }
    }

    public static class Scheduler
    {
        private static List<ScheduleEntry> schedules = new List<ScheduleEntry>();

        public static void ClearSchedules()
        {
            schedules.Clear();
        }

		public static void ScheduleRecording(DateTime startRecording, int totalSeconds, bool autoFocusing, bool autoPulseGuiding)
        {
            Guid operationId = Guid.NewGuid();
            schedules.Add(
                new ScheduleEntry()
                {
                    OperaionId = operationId,
                    Action = ScheduledAction.StartRecording,
                    ActionTime = startRecording,
					AutoFocusing = autoFocusing, 
					AutoPulseGuiding = autoPulseGuiding
                });
            schedules.Add(
                new ScheduleEntry()
                {
                    OperaionId = operationId,
                    Action = ScheduledAction.StopRecording,
                    ActionTime = startRecording.AddSeconds(totalSeconds),
					AutoFocusing = false,
					AutoPulseGuiding = false
                });
        }

        public static int RemoveOperation(Guid operationId)
        {
            return schedules.RemoveAll(x => x.OperaionId == operationId);
        }

        public static List<ScheduleEntry> GetAllSchedules()
        {
            var rv = new List<ScheduleEntry>();
            rv.AddRange(schedules);

            rv.Sort((x, y) => x.ActionTime.CompareTo(y.ActionTime));

            return rv;
        }

        public static ScheduledAction CheckSchedules()
        {
            ScheduleEntry next = GetNextEntry();

            if (next != null && next.ActionTime <= DateTime.Now)
            {
                schedules.Remove(next);

                return next.Action;
            }
            return ScheduledAction.None;
        }

        public static ScheduleEntry GetNextEntry()
        {
            if (schedules.Count > 0)
            {
                schedules.Sort((x, y) => x.ActionTime.CompareTo(y.ActionTime));
                return schedules[0];
            }

            return null;
        }
    }
}
