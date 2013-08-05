using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using AAVRec.Helpers;

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
			System.Reflection.Assembly a = System.Reflection.Assembly.GetExecutingAssembly();
			Version appVersion = a.GetName().Version;
			string appVersionString = appVersion.ToString();

			Trace.WriteLine(string.Format("Starting AAVRec v.{0}", appVersionString));

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

			AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
			Application.ThreadException += Application_ThreadException;

            #region Make sure the settings are not forgotten between application version updates


            if (Properties.Settings.Default.ApplicationVersion != appVersion.ToString())
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.ApplicationVersion = appVersionString;
                Properties.Settings.Default.Save();
            }
            #endregion

            Application.Run(new frmMain());
        }

		static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
		{
			if (e.Exception != null)
				Trace.WriteLine(e.Exception.GetFullErrorDescription("Application.ThreadException"));
		}

	    private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
	    {
		    var exception = unhandledExceptionEventArgs.ExceptionObject as Exception;
		    if (exception != null)
				Trace.WriteLine(exception.GetFullErrorDescription("AppDomain.CurrentDomain.UnhandledException"));
	    }
    }
}
