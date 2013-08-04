using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace AAVRec
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            #region Make sure the settings are not forgotten between application version updates
            System.Reflection.Assembly a = System.Reflection.Assembly.GetExecutingAssembly();

            Version appVersion = a.GetName().Version;
            string appVersionString = appVersion.ToString();

            if (Properties.Settings.Default.ApplicationVersion != appVersion.ToString())
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.ApplicationVersion = appVersionString;
                Properties.Settings.Default.Save();
            }
            #endregion

            Application.Run(new frmMain());
        }
    }
}
