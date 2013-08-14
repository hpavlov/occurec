using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Globalization;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using System.Net;

namespace OccuRecUpdate.Schema
{
    class OccuRecUpdate : UpdateObject 
    {
		// <OccuRecUpdate Version="16000" Path="OccuRecUpdate.exe" ArchivedPath="OccuRecUpdate.zip"/>

        internal readonly string Path = null;
        internal readonly string ArchivedPath = null;

        public OccuRecUpdate(XmlElement node)
            : base(node)
        {
            m_Version = int.Parse(node.Attributes["Version"].Value, CultureInfo.InvariantCulture); ;
            Path = node.Attributes["Path"].Value;
            if (node.Attributes["ArchivedPath"] != null)
                ArchivedPath = node.Attributes["ArchivedPath"].Value;
        }

        public override bool NewUpdatesAvailable(string occuRecPath)
        {
            Assembly asm = GetLocalOccuRecUpdateAssembly();
            if (asm != null)
            {
                object[] atts = asm.GetCustomAttributes(typeof(AssemblyFileVersionAttribute), true);
                if (atts.Length == 1)
                {
                    string currVersionString = ((AssemblyFileVersionAttribute)atts[0]).Version;
                    int currVersionAsInt = Config.Instance.OccuRecUpdateVersionStringToVersion(currVersionString);

                    if (base.Version > currVersionAsInt)
                    {
                        Trace.WriteLine(string.Format("Update required for 'OccuRecUpdate.exe': local version: {0}; server version: {1}", currVersionAsInt, Version));
                        return true;
                    }
                }
            }

            return false;
        }
        
        private Assembly GetLocalOccuRecUpdateAssembly()
        {
            if (Assembly.GetExecutingAssembly().GetName().Name == "OccuRecUpdate")
                return Assembly.GetExecutingAssembly();

            string probeFile = System.IO.Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + "\\OccuRecUpdate.exe");
            if (System.IO.File.Exists(probeFile))
                return Assembly.ReflectionOnlyLoadFrom(probeFile);            

            return null;
        }

        public override void Update(Updater updater, string occuRecPath, bool acceptBetaUpdates, IProgressUpdate progress)
        {
            string newVerFileLocalFileName = System.IO.Path.GetFullPath(System.IO.Path.GetTempPath() + "\\OccuRecUpdate.exe");

            if (System.IO.File.Exists(newVerFileLocalFileName))
                System.IO.File.Delete(newVerFileLocalFileName);

            // Download the new OccuRecUpdate version under newVerFileLocalFileName
            string fileLocation = null;
            if (string.IsNullOrEmpty(ArchivedPath))
                fileLocation = Path;
            else
                fileLocation = ArchivedPath;

            try
            {
                progress.RefreshMainForm();
                updater.UpdateFile(fileLocation, newVerFileLocalFileName, !string.IsNullOrEmpty(ArchivedPath), progress);
            }
            catch (WebException wex)
            {
                progress.OnError(wex);

                return;
            }

            string selfUpdaterExecutable = Config.Instance.PrepareOccuRecSelfUpdateTempFile(occuRecPath, acceptBetaUpdates, newVerFileLocalFileName);

            using (Stream selfUpdater = Shared.AssemblyHelper.GetEmbededResourceStreamThatClientMustDispose("OccuRecUpdate.SelfUpdate", "OccuRecSelfUpdate.bin"))
            {
                byte[] buffer = new byte[selfUpdater.Length];
                selfUpdater.Read(buffer, 0, buffer.Length);
                System.IO.File.WriteAllBytes(selfUpdaterExecutable, buffer);
            }

            // Run the copyFileName passing as argument current process ID and then terminate the current process fast!
            Process currProcess = Process.GetCurrentProcess();
            ProcessStartInfo pi = new ProcessStartInfo(selfUpdaterExecutable, currProcess.Id.ToString());
            
            if (System.Environment.OSVersion.Version.Major > 5)
                // UAC Elevate as Administrator for Windows Vista, Win7 and later
                pi.Verb = "runas";

            pi.WindowStyle = ProcessWindowStyle.Hidden;
            Process.Start(pi);

            currProcess.Kill();
        }
    }
}
