using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Microsoft.Win32;
using System.Threading;
using System.IO;

namespace AAVRecSelfUpdate
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\AAVRec", RegistryKeyPermissionCheck.ReadWriteSubTree);

                if (key != null)
                {
                    try
                    {
                        string copyFromFullFileName = Convert.ToString(key.GetValue("CopySelfAAVRecUpdateFrom", null));
                        string copyToDirectoryName = Convert.ToString(key.GetValue("CopySelfAAVRecUpdateTo", null));

                        Thread.Sleep(2000);

                        try
                        {
                            Process prcToKill = Process.GetProcessById(int.Parse(args[0]));
                            if (prcToKill != null)
                                prcToKill.Kill();
                        }
                        catch { }

                        if (!Directory.Exists(copyToDirectoryName))
                            Directory.CreateDirectory(copyToDirectoryName);

                        string copyToFullName = Path.GetFullPath(copyToDirectoryName + "\\" + Path.GetFileName(copyFromFullFileName));
                        if (File.Exists(copyToFullName))
                            File.Delete(copyToFullName);

                        if (File.Exists(copyFromFullFileName))
                            File.Copy(copyFromFullFileName, copyToFullName);

                        var pi = new ProcessStartInfo(copyToFullName);

                        if (System.Environment.OSVersion.Version.Major > 5)
                            // UAC Elevate as Administrator for Windows Vista, Win7 and later
                            pi.Verb = "runas";

                        Process.Start(pi);
                    }
                    catch (Exception exin)
                    {
                        Trace.WriteLine(exin.ToString());
                    }
                    finally
                    {
                        key.Close();
                    }
                }

            }
            catch(Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }
        }
    }
}
