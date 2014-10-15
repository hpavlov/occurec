/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

namespace OccuRecUpdate
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static int Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            frmUpdate frmMain = new frmUpdate();
            //Debugger.Launch();

            Application.Run(frmMain);

            if (frmUpdate.s_Error is InstallationAbortException)
            {
                MessageBox.Show("The installation cannot continue:\r\n\r\n" + frmUpdate.s_Error.Message, "OccuRec Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 101;
            }
            else if (frmUpdate.s_Error is Exception)
            {
				MessageBox.Show("An unanticipated error has occured:\r\n\r\n" + frmUpdate.s_Error.Message, "OccuRec Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 102;
            }

            return 0;
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            InstallationAbortException exia = e.ExceptionObject as InstallationAbortException;
            Exception ex = e.ExceptionObject as Exception;

            if (exia != null)
				MessageBox.Show("The installation cannot continue:\r\n\r\n" + exia.Message, "OccuRec Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
				MessageBox.Show("An unanticipated error has occured:\r\n\r\n" + ex != null ? ex.Message : "", "OccuRec Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}