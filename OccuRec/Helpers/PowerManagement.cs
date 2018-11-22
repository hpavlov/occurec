using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using OccuRec.Properties;
using OccuRec.Utilities;

namespace OccuRec.Helpers
{
    public class PowerManagement
    {
        private static Guid GUID_SLEEP_SUBGROUP = new Guid("238c9fa8-0aad-41ed-83f4-97be242c8f20");
        private static Guid GUID_HIBERNATEIDLE = new Guid("9d7815a6-7ee4-497e-8888-515a05f02364");
        private static Guid GUID_SLEEPIDLE = new Guid("29f6c1db-86da-48c5-9fdb-f2b67b1f44da");

        [DllImport("powrprof.dll")]
        static extern uint PowerGetActiveScheme(
            IntPtr UserRootPowerKey,
            ref IntPtr ActivePolicyGuid);

        [DllImport("powrprof.dll")]
        static extern uint PowerReadACValue(
            IntPtr RootPowerKey,
            ref Guid SchemeGuid,
            ref Guid SubGroupOfPowerSettingGuid,
            ref Guid PowerSettingGuid,
            ref int Type,
            ref int Buffer,
            ref uint BufferSize);

        public static void CheckPowerSettings(IWin32Window parentWindow)
        {
            try
            {
                var activePolicyGuidPTR = IntPtr.Zero;
                PowerGetActiveScheme(IntPtr.Zero, ref activePolicyGuidPTR);

                var activePolicyGuid = (Guid)Marshal.PtrToStructure(activePolicyGuidPTR, typeof(Guid));
                var type = 0;
                int hybernateAfter = 0;
                int sleepAfter = 0;
                var valueSize = 4u;
                PowerReadACValue(IntPtr.Zero, ref activePolicyGuid,
                    ref GUID_SLEEP_SUBGROUP, ref GUID_HIBERNATEIDLE,
                    ref type, ref hybernateAfter, ref valueSize);

                PowerReadACValue(IntPtr.Zero, ref activePolicyGuid,
                    ref GUID_SLEEP_SUBGROUP, ref GUID_SLEEPIDLE,
                    ref type, ref sleepAfter, ref valueSize);

                if (hybernateAfter > 0)
                {
                    MessageBox.Show(
                        parentWindow,
                        string.Format("Your computer has been configured to hibernate after {0} while on main power. This may affect unattended scheduled recording!", SecondsToHumanReadable(hybernateAfter)),
                        "OccuRec Power Settings Warning",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
                if (sleepAfter > 0)
                {
                    MessageBox.Show(
                        parentWindow,
                        string.Format("Your computer has been configured to sleep after {0} while on main power. This may affect unattended scheduled recording!", SecondsToHumanReadable(sleepAfter)),
                        "OccuRec Power Settings Warning",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }

                if (Settings.Default.WarnIfRunningOnBattery && 
                    SystemInformation.PowerStatus.PowerLineStatus == PowerLineStatus.Offline)
                {
                    MessageBox.Show(
                        parentWindow,
                        string.Format("Your computer is running on battery which has {0}% remaining", Math.Round(SystemInformation.PowerStatus.BatteryLifePercent * 100)),
                        "OccuRec Power Settings Warning",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.GetFullStackTrace());
            }
        }

        private static string SecondsToHumanReadable(int seconds)
        {
            if (seconds <= 90)
            {
                return string.Format("{0} seconds", seconds);
            }
            double mins = seconds/60.0;
            if (mins < 60)
            {
                return string.Format("{0} minutes", Math.Round(mins));
            }
            double hrs = seconds / 3600.0;
            return string.Format("{0} hours", Math.Round(hrs));
        }
    }
}
