/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Net;
using OccuRecUpdate.Schema;

namespace OccuRecUpdate
{
    public class InstallationAbortException : Exception
    {
        public InstallationAbortException(string message)
            : base(message)
        { }
    }

    public interface IProgressUpdate
    {
        void UpdateProgress(string message, int value);
        void OnError(Exception error);
        void RefreshMainForm();
    }

    internal class Updater
    {
	    private string occuRecPath;
	    private bool acceptBetaUpdates;

		public Updater(string occuRecPath, bool acceptBetaUpdates)
		{
			this.occuRecPath = occuRecPath;
			this.acceptBetaUpdates = acceptBetaUpdates;
		}

        public UpdateSchema NewUpdatesAvailable()
        {
            Uri updateUri = new Uri(Config.Instance.UpdateLocation + Config.Instance.UpdatesXmlFileName);

            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(updateUri);
            httpRequest.Method = "GET";
            httpRequest.Timeout = 30000; //30 sec

            HttpWebResponse response = null;

            try
            {
                response = (HttpWebResponse)httpRequest.GetResponse();

                string updateXml = null;

                Stream streamResponse = response.GetResponseStream();

                try
                {
                    using (TextReader reader = new StreamReader(streamResponse))
                    {
                        updateXml = reader.ReadToEnd();
                    }
                }
                finally
                {
                    streamResponse.Close();
                }

                if (updateXml != null)
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(updateXml);

                    UpdateSchema updateSchema = new UpdateSchema(xmlDoc);

                    if (updateSchema.NewUpdatesAvailable(occuRecPath))
                        return updateSchema;
                }
            }
            finally
            {
                // Close response
                if (response != null)
                    response.Close();
            }

            return null;
        }

        public void PrepareToUpdate()
        {
            List<int> pids = null;
            do
            {
                pids = SingleInstance.SingleApplication.GetCurrentInstanceWindowHandle("OccuRec.exe");

                if (pids != null)
                {
                    foreach (int pid in pids)
                    {
                        using (Process processToKill = Process.GetProcessById(pid))
                        {
                            processToKill.Kill();
                        }
                    }
                }
            }
            while (pids != null && pids.Count > 0);

            string updaterPath = Path.GetFullPath(occuRecPath + @"/OccuRecUpdate.exe");
            if (!System.IO.File.Exists(updaterPath))
            {
                try
                {
                    System.IO.File.Copy(Process.GetCurrentProcess().MainModule.FileName, updaterPath);
                }
                catch
                { }
            }
        }

        public string UpdateFile(Schema.File fileNode, IProgressUpdate progress)
        {
            return UpdateFile(fileNode, null, progress);
        }

		public string GetLocalFileName(Schema.File fileNode)
		{
			return Path.GetFullPath(occuRecPath + "//" + (string.IsNullOrEmpty(fileNode.LocalPath) ? Path.GetFileName(fileNode.Path) : fileNode.LocalPath));
		}

        public string UpdateFile(Schema.File fileNode, string localFileNameExplicit, IProgressUpdate progress)
        {
            string localFile =
                localFileNameExplicit == null
				? GetLocalFileName(fileNode)
                : localFileNameExplicit;

            return UpdateFile(fileNode.Path, localFile, fileNode.Archived, progress);
        }

        public string UpdateFile(string location, string localFile, bool shouldUnzip, IProgressUpdate progress)
        {
            Uri fileUri = new Uri(Config.Instance.UpdateLocation + "/" + location.TrimStart(new char[] { '/' }));

            progress.UpdateProgress(string.Format("Downloading {0} ...", Path.GetFileName(localFile)), -1);

            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(fileUri);
            httpRequest.Method = "GET";
            httpRequest.Timeout = 600000; //600 sec = 10 min

            HttpWebResponse response = null;

            try
            {
                response = (HttpWebResponse)httpRequest.GetResponse();

                Stream streamResponse = response.GetResponseStream();
                long totalBytes = response.ContentLength;

                try
                {
                    bool allGood = false;
                    int attempts = 0;
                    Exception fileDelException = null;
                    do
                    {
                        try
                        {
                            if (System.IO.File.Exists(localFile))
                                // throws access denied: -2147024891
                                System.IO.File.Delete(localFile);

                            allGood = true;
                            fileDelException = null;
                        }
                        catch (Exception ex)
                        {
                            fileDelException = ex;
                            System.Threading.Thread.Sleep(500);
                        }

                        attempts++;
                    }
                    while (!allGood && attempts < 10);

                    if (fileDelException != null)
                    {
                        System.Windows.Forms.MessageBox.Show(
                            "There was a problem upgrading OccuRec\r\n\r\n" + fileDelException.GetType().ToString() + " : " + fileDelException.Message +
                            "\r\n\r\nPlease note that the update process needs to be run by an administrator. Stop all running copies of OccuRec and then manually run:\r\n\r\n" +
                            Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + "\\OccuRecUpdate.exe") +
                            "\r\n\r\nYou can also use Right-Click and then use 'Run as ...' and specify a user with administrative previlegies.",
                            "Error Upgrading",
                            System.Windows.Forms.MessageBoxButtons.OK,
                            System.Windows.Forms.MessageBoxIcon.Error);

                        Process.GetCurrentProcess().Kill();
                    }

                    if (!Directory.Exists(Path.GetDirectoryName(localFile)))
                        Directory.CreateDirectory(Path.GetDirectoryName(localFile));

                    using (BinaryReader reader = new BinaryReader(streamResponse))
                    using (BinaryWriter writer = new BinaryWriter(new FileStream(localFile, FileMode.Create)))
                    {
                        byte[] chunk = null;
                        do
                        {
                            chunk = reader.ReadBytes(1024);
                            //TODO: Send back info on the download progress with the bytes read and total bytes
                            writer.Write(chunk);
                        }
                        while (chunk != null && chunk.Length == 1024);

                        writer.Flush();
                    }

                    //TODO: Set the full content downloaded, hide the byte download progress label

                    if (shouldUnzip)
                    {
                        string tempOutputDir = Path.ChangeExtension(Path.GetTempFileName(), "");
                        Directory.CreateDirectory(tempOutputDir);
                        try
                        {
							ZipUnzip.UnZip(localFile, tempOutputDir, true);
                            string[] files = Directory.GetFiles(tempOutputDir);
                            System.IO.File.Copy(files[0], localFile, true);
                            System.IO.File.Delete(files[0]);
                        }
                        finally
                        {
                            Directory.Delete(tempOutputDir, true);
                        }
                    }
                }
                finally
                {
                    streamResponse.Close();
                }

                return localFile;

            }
            finally
            {
                // Close response
                if (response != null)
                    response.Close();
            }
        }
    }
}
