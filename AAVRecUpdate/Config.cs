using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;
using System.Globalization;
using System.Security.Cryptography;
using Microsoft.Win32;
using System.Security;
using System.Xml;

namespace OccuRecUpdate
{
    internal class Config
    {
        private Config()
        { }

        private readonly static Config m_Instance = new Config();

        public static Config Instance
        {
            get { return m_Instance; }
        }

        public string UpdateLocation;
        public string UpdatesXmlFileName;
        public string SelfUpdateFileNameToDelete;

        public bool IsFirtsTimeRun = false;

		public string DEFAULT_INSTALL_PATH = Path.GetFullPath(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\OccuRec\\");

        public void Load(string occuRecPath, bool acceptBetaUpdates)
        {
			UpdateLocation = "http://www.hristopavlov.net/OccuRec/";
            UpdatesXmlFileName = acceptBetaUpdates ? "/Beta.xml" : "/Updates.xml";

            RegistryKey key = null;

            try
            {
				key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\OccuRec", RegistryKeyPermissionCheck.ReadSubTree);
                if (key == null)
					key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\OccuRec", RegistryKeyPermissionCheck.ReadWriteSubTree);
            }
            catch (UnauthorizedAccessException) { }
            catch (SecurityException) { }

            if (key != null)
            {
                try
                {
					UpdateLocation = Convert.ToString(key.GetValue("UpdateLocation", "http://www.hristopavlov.net/OccuRec/"), CultureInfo.InvariantCulture);
                    UpdateLocation = UpdateLocation.TrimEnd(new char[] { '/' });
					SelfUpdateFileNameToDelete = Convert.ToString(key.GetValue("SelfOccuRecUpdateTempFile", null));
                }
                finally
                {
                    key.Close();
                }
            }

			IsFirtsTimeRun = !File.Exists(OccuRecExePath(occuRecPath));

            try
            {
                if (File.Exists("C:\\UpdateConfig.xml"))
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load("C:\\UpdateConfig.xml");
                    XmlNode node = xmlDoc.SelectSingleNode("/configuration/updateLocation");
                    if (node != null)
                        UpdateLocation = node.InnerText;
                }
            }
            catch { }
        }

        public string OccuRecExePath(string occuRecLocation)
        {
			return Path.GetFullPath(occuRecLocation + @"/OccuRec.exe");
        }

        public string PrepareOccuRecSelfUpdateTempFile(string occuRecLocation, bool acceptBetaUpdates, string newVersionLocation)
        {
            RegistryKey key = null;

            try
            {
				key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\OccuRec", RegistryKeyPermissionCheck.ReadWriteSubTree);
                if (key == null)
					key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\OccuRec", RegistryKeyPermissionCheck.ReadWriteSubTree);
            }
            catch (UnauthorizedAccessException) { }
            catch (SecurityException) { }

            string tempFileName = Path.ChangeExtension(Path.GetTempFileName(), ".exe");
            if (key != null)
            {
                try
                {
                    key.SetValue("SelfOccuRecUpdateTempFile", tempFileName);
					key.SetValue("CopySelfOccuRecUpdateFrom", newVersionLocation);
					key.SetValue("CopySelfOccuRecUpdateTo", AppDomain.CurrentDomain.BaseDirectory);
					key.SetValue("OccuRecUpdateOccuRecLocation", occuRecLocation);
					key.SetValue("OccuRecUpdateAcceptBeta", acceptBetaUpdates);
                }
                catch { }
                finally
                {
                    key.Close();
                }              
            }

            return tempFileName;
        }

        public void ResetSelfUpdateFileName()
        {
            RegistryKey key = null;

            try
            {
				key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\OccuRec", RegistryKeyPermissionCheck.ReadWriteSubTree);
                if (key == null)
					key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\OccuRec", RegistryKeyPermissionCheck.ReadWriteSubTree);
            }
            catch (UnauthorizedAccessException) { }
            catch (SecurityException) { }

            if (key != null)
            {
                try
                {
                    key.DeleteValue("SelfOccuRecUpdateTempFile", false);
					key.DeleteValue("CopySelfOccuRecUpdateFrom", false);
					key.DeleteValue("CopySelfOccuRecUpdateTo", false);
					key.DeleteValue("OccuRecUpdateOccuRecLocation", false);
					key.DeleteValue("OccuRecUpdateAcceptBeta", false);
                }
                catch (Exception)
                { }
                finally
                {
                    key.Close();
                }
            }
        }

        public int CurrentlyInstalledFileVersion(string occuRecLocation, string relativeFilePath)
        {
			if (Directory.Exists(occuRecLocation))
            {
				string owExeFilePath = Path.GetFullPath(occuRecLocation + "/" + relativeFilePath);
                if (File.Exists(owExeFilePath))
                {
                    try
                    {
                        AssemblyName an = AssemblyName.GetAssemblyName(owExeFilePath);
                        Version owVer = an.Version;
                        return 10000 * owVer.Major + 1000 * owVer.Minor + 100 * owVer.Build + owVer.Revision;
                    }
                    catch
                    { }
                }
                else
                    return -1;
            }
            else
            {
				Directory.CreateDirectory(occuRecLocation);
            }

            return 0;
        }

		public int CurrentlyInstalledOccuRecVersion(string occuRecLocation)
        {
			if (Directory.Exists(occuRecLocation))
            {
				string owExeFilePath = Path.GetFullPath(occuRecLocation + "/OccuRec.exe");
                if (File.Exists(owExeFilePath))
                {
                    try
                    {
                        AssemblyName an = AssemblyName.GetAssemblyName(owExeFilePath);
                        Version owVer = an.Version;
                        return 10000 * owVer.Major + 1000 * owVer.Minor + 100 * owVer.Build + owVer.Revision;
                    }
                    catch
                    { }
                }
            }
            else
            {
				Directory.CreateDirectory(occuRecLocation);
            }

            return 0;
        }

        public string VersionToVersionString(int version)
        {
            var outVer = new StringBuilder();
            outVer.Append(Convert.ToString(version / 10000));
            outVer.Append(".");

            version = version % 10000;
            outVer.Append(Convert.ToString(version / 1000));
            outVer.Append(".");

            version = version % 1000;
            outVer.Append(Convert.ToString(version / 100));
            outVer.Append(".");
            outVer.Append(Convert.ToString(version % 100));

            return outVer.ToString();
        }

        public int VersionStringToVersion(string versionString)
        {
            string[] tokens = versionString.Split('.');
            int version =
                10000 * int.Parse(tokens[0]) +
                1000 * int.Parse(tokens[1]) +
                (tokens.Length > 2 ? 100 * int.Parse(tokens[2]) : 0) + 
                (tokens.Length > 3 ? int.Parse(tokens[3]) : 0);
            return version;
        }

        public int OccuRecUpdateVersionStringToVersion(string versionString)
        {
            string[] tokens = versionString.Split('.');
            int version = 
                10000 * int.Parse(tokens[0]) + 
                1000 * int.Parse(tokens[1]) +
                (tokens.Length > 2 ? 100 * int.Parse(tokens[2]) : 0) + 
                (tokens.Length > 2 ? int.Parse(tokens[3]) : 0);

            return version;
        }
    }
}
