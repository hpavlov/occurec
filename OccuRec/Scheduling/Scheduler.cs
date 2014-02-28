using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OccuRec.FrameAnalysis;
using OccuRec.Properties;

namespace OccuRec.Scheduling
{
    public enum ScheduledAction
    {
        None,
        StartRecording,
        StopRecording,
		AutoFocus,
		EnablePulseGuiding,
		DisablePulseGuiding
    }

    public class ScheduleEntry
    {
        public Guid OperaionId { get; set; }
        public ScheduledAction Action { get; set; }
        public DateTime ActionTime { get; set; }

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
			
			if (autoPulseGuiding)
			{
				schedules.Add(
					new ScheduleEntry()
					{
						OperaionId = operationId,
						Action = ScheduledAction.EnablePulseGuiding,
						ActionTime = DateTime.UtcNow
					});
			}

			if (autoFocusing)
			{
				List<DateTime> autoFocusingActions = CreateAutoFocusingPlan(startRecording);
				foreach (DateTime scheduledFocusingTime in autoFocusingActions)
				{
					schedules.Add(
						new ScheduleEntry()
						{
							OperaionId = operationId,
							Action = ScheduledAction.AutoFocus,
							ActionTime = scheduledFocusingTime
						});					
				}
			}

            schedules.Add(
                new ScheduleEntry()
                {
                    OperaionId = operationId,
                    Action = ScheduledAction.StartRecording,
                    ActionTime = startRecording,
                });

            schedules.Add(
                new ScheduleEntry()
                {
                    OperaionId = operationId,
                    Action = ScheduledAction.StopRecording,
                    ActionTime = startRecording.AddSeconds(totalSeconds),
                });

			if (autoPulseGuiding)
			{
				schedules.Add(
					new ScheduleEntry()
					{
						OperaionId = operationId,
						Action = ScheduledAction.DisablePulseGuiding,
						ActionTime = startRecording.AddSeconds(totalSeconds + 10),
					});				
			}
        }

		private static List<DateTime> CreateAutoFocusingPlan(DateTime nextOperationTimeUT)
		{
			//  x) Between 1 and 5 min before the event
			//  x) Every 15 to 19 min before the event

			var rv = new List<DateTime>();

			DateTime now = DateTime.UtcNow;
			double secondsUntilEvent = new TimeSpan(nextOperationTimeUT.Ticks - DateTime.UtcNow.Ticks).TotalSeconds;

			if (secondsUntilEvent > 60 && secondsUntilEvent < 5 * 60)
				rv.Add(now);
			else
				rv.Add(nextOperationTimeUT.AddMinutes(-5));

			secondsUntilEvent -= 20 * 60;

			while (secondsUntilEvent > 0)
			{
				rv.Add(nextOperationTimeUT.AddSeconds(-1 * secondsUntilEvent));
				secondsUntilEvent -= 20 * 60;
			}

			return rv;
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

	    public static bool HasScheduledTasks()
	    {
		    return schedules.Count > 0;
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
