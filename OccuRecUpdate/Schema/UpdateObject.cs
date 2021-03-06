/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Reflection;
using System.Diagnostics;
using System.Globalization;

namespace OccuRecUpdate.Schema
{
    internal abstract class UpdateObject
    {
        internal readonly List<Schema.File> AllFiles = new List<File>();

        protected int m_Version = -1;
        protected string m_VersionStr = null;
        protected string m_File = null;
        protected string m_ModuleName = null;
        protected string m_ReleaseDate = null;
        protected bool m_MustExist = true;

        internal int Version
        {
            get { return m_Version; }
        }

        internal string File
        {
            get { return m_File; }
        }

        internal string ModuleName
        {
            get { return m_ModuleName; }
        }

        internal bool MustExist
        {
            get { return m_MustExist; }
        }

        internal string ReleaseDate
        {
            get { return m_ReleaseDate; }
        }

        public string VersionString
        {
            get
            {
                if (m_Version == -1)
                {
                    if (m_VersionStr != null)
                        return m_VersionStr;
                    else
                        return "N/A";
                }
                else
                    return Config.Instance.VersionToVersionString(m_Version);
            }
        }

        protected UpdateObject(XmlElement node)
        {
            foreach (XmlNode fileNode in node.SelectNodes("./File"))
            {
                AllFiles.Add(new File(fileNode as XmlElement));
            }
        }

        protected virtual void OnFileUpdated(Schema.File file, string localFileName)
        { }

		public virtual void Update(Updater updater, string occuRecPath, bool acceptBetaUpdates, IProgressUpdate progress)
        {
            foreach (Schema.File fileToUpdate in AllFiles)
            {
                try
                {
					string localFilePath;

					if (fileToUpdate.Action == "DeleteIfExists")
					{
						localFilePath = updater.GetLocalFileName(fileToUpdate);
						if (System.IO.File.Exists(localFilePath))
						{
							try
							{
								System.IO.File.Delete(localFilePath);
							}
							catch (Exception ex)
							{
								Trace.WriteLine(ex);
							}
						}
						return;
					}

					localFilePath = updater.UpdateFile(fileToUpdate, progress);

                    OnFileUpdated(fileToUpdate, localFilePath);

                    if (!string.IsNullOrEmpty(fileToUpdate.Action))
                        ExecutePostUpdateAction(fileToUpdate, localFilePath);
                }
                catch (Exception ex)
                {                    
                    progress.OnError(ex);
                    return;
                }
            }
        }

        public void ExecutePostUpdateAction(Schema.File updatedFile, string localFilePath)
        {
            if ("ShellExecute".Equals(updatedFile.Action, StringComparison.InvariantCultureIgnoreCase) &&
                System.IO.File.Exists(localFilePath))
            {
                Process.Start(localFilePath);
            }
        }

        public virtual bool NewUpdatesAvailable(string occuRecPath)
        {
            Assembly asm = null;
			string fullLocalFileName = System.IO.Path.GetFullPath(occuRecPath + "\\" + this.File);
            Trace.WriteLine(string.Format("Checking updates for: '{0}'", fullLocalFileName));
            if (System.IO.File.Exists(fullLocalFileName))
            {
                byte[] asmBytes = System.IO.File.ReadAllBytes(fullLocalFileName);

                try
                {
                    asm = Assembly.Load(asmBytes);

                }
                catch(BadImageFormatException)
                {
                    Trace.WriteLine(string.Format("Cannot load assembly: '{0}'", fullLocalFileName));
                    asm = null;
                    try
                    {
                        System.IO.File.Delete(fullLocalFileName);
                    }
                    catch
                    { }

                    return true;
                }
            }
            else
            {
                Trace.WriteLine(string.Format("File not found: '{0}'", fullLocalFileName));

                // The file doesn't have to exist and because it actually doesn't 
                // this is why it must be downloaded i.e. a newer version is available
                if (!m_MustExist)
                {
                    Trace.WriteLine(string.Format("Update required for '{0}': The file is not found locally", File));
                    return true;
                }
            }

            if (asm != null)
            {
                object[] atts = asm.GetCustomAttributes(typeof(AssemblyFileVersionAttribute), true);
                if (atts.Length == 1)
                {
                    string currVersionString = ((AssemblyFileVersionAttribute)atts[0]).Version;
                    int currVersionAsInt = Config.Instance.FileVersionStringToVersion(currVersionString);

                    Trace.WriteLine(string.Format("Module: '{0}', Local Version: {1}, Server Version: {2}", this.File, currVersionAsInt, Version));

                    if (Version > currVersionAsInt)
                    {
                        Trace.WriteLine(string.Format("Update required for '{0}': local version: {1}; server version: {2}", File, currVersionAsInt, Version));
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
