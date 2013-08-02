using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

namespace AAVRecUpdate
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
                MessageBox.Show("The installation cannot continue:\r\n\r\n" + frmUpdate.s_Error.Message, "AAVRec Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 101;
            }
            else if (frmUpdate.s_Error is Exception)
            {
                MessageBox.Show("An unanticipated error has occured:\r\n\r\n" + frmUpdate.s_Error.Message, "AAVRec Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 102;
            }

            return 0;
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            InstallationAbortException exia = e.ExceptionObject as InstallationAbortException;
            Exception ex = e.ExceptionObject as Exception;

            if (exia != null)
                MessageBox.Show("The installation cannot continue:\r\n\r\n" + exia.Message, "AAVRec Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
                MessageBox.Show("An unanticipated error has occured:\r\n\r\n" + ex != null ? ex.Message : "", "AAVRec Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}