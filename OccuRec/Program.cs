/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using OccuRec.Helpers;
using OccuRec.Utilities;

namespace OccuRec
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            OccuRecSelfTraceListener.Instance.Initialize();

			System.Reflection.Assembly a = System.Reflection.Assembly.GetExecutingAssembly();
			Version appVersion = a.GetName().Version;
			string appVersionString = appVersion.ToString();

			Trace.WriteLine(string.Format("Starting OccuRec v{0}", appVersionString));

			string ascomPlatform = ASCOM.ObservatoryController.GetInstalledASCOMPlatformVersion();
			if (ascomPlatform == null)
				Trace.WriteLine("ASCOM Platform is not installed");
			else
				Trace.WriteLine(string.Format("ASCOM Platform v{0} installed.", ascomPlatform));

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

			AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
			Application.ThreadException += Application_ThreadException;

            #region Make sure the settings are not forgotten between application version updates

            try
            {
                if (Properties.Settings.Default.ApplicationVersion != appVersion.ToString())
                {
                    Properties.Settings.Default.Upgrade();
                    Properties.Settings.Default.ApplicationVersion = appVersionString;
                    Properties.Settings.Default.Save();
                }
            }
            catch (Exception ex)
            {
                // NOTE: It is better to start the application than to fail upgrading the settings.
            }
            #endregion

            Application.Run(new frmMain());
        }

		static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
		{
			if (e.Exception != null)
				Trace.WriteLine(e.Exception.GetFullStackTrace("Application.ThreadException"));
		}

	    private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
	    {
		    var exception = unhandledExceptionEventArgs.ExceptionObject as Exception;
		    if (exception != null)
				Trace.WriteLine(exception.GetFullStackTrace("AppDomain.CurrentDomain.UnhandledException"));
	    }
    }
}
