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

namespace AAVRecUpdate
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

        public string DEFAULT_INSTALL_PATH = Path.GetFullPath(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\AAVRec\\");

        public void Load(string aavRecPath, bool acceptBetaUpdates)
        {
            UpdateLocation = "http://www.hristopavlov.net/AAVRec/";
            UpdatesXmlFileName = acceptBetaUpdates ? "/Beta.xml" : "/Updates.xml";

            RegistryKey key = null;

            try
            {
                key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\AAVRec", RegistryKeyPermissionCheck.ReadSubTree);
                if (key == null)
                    key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\AAVRec", RegistryKeyPermissionCheck.ReadWriteSubTree);
            }
            catch (UnauthorizedAccessException) { }
            catch (SecurityException) { }

            if (key != null)
            {
                try
                {
                    UpdateLocation = Convert.ToString(key.GetValue("UpdateLocation", "http://www.hristopavlov.net/AAVRec/"), CultureInfo.InvariantCulture);
                    UpdateLocation = UpdateLocation.TrimEnd(new char[] { '/' });
                    SelfUpdateFileNameToDelete = Convert.ToString(key.GetValue("SelfAAVRecUpdateTempFile", null));
                }
                finally
                {
                    key.Close();
                }
            }

			IsFirtsTimeRun = !File.Exists(AAVRecExePath(aavRecPath));

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

        public string AAVRecExePath(string aavRecLocation)
        {
			return Path.GetFullPath(aavRecLocation + @"/AAVRec.exe");
        }

        public string PrepareAAVRecSelfUpdateTempFile(string aavRecLocation, bool acceptBetaUpdates, string newVersionLocation)
        {
            RegistryKey key = null;

            try
            {
                key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\AAVRec", RegistryKeyPermissionCheck.ReadWriteSubTree);
                if (key == null)
                    key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\AAVRec", RegistryKeyPermissionCheck.ReadWriteSubTree);
            }
            catch (UnauthorizedAccessException) { }
            catch (SecurityException) { }

            string tempFileName = Path.ChangeExtension(Path.GetTempFileName(), ".exe");
            if (key != null)
            {
                try
                {
                    key.SetValue("SelfAAVRecUpdateTempFile", tempFileName);
                    key.SetValue("CopySelfAAVRecUpdateFrom", newVersionLocation);
                    key.SetValue("CopySelfAAVRecUpdateTo", AppDomain.CurrentDomain.BaseDirectory);
					key.SetValue("AAVRecUpdateAAVRecLocation", aavRecLocation);
					key.SetValue("AAVRecUpdateAcceptBeta", acceptBetaUpdates);
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
                key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\AAVRec", RegistryKeyPermissionCheck.ReadWriteSubTree);
                if (key == null)
                    key = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\AAVRec", RegistryKeyPermissionCheck.ReadWriteSubTree);
            }
            catch (UnauthorizedAccessException) { }
            catch (SecurityException) { }

            if (key != null)
            {
                try
                {
                    key.DeleteValue("SelfAAVRecUpdateTempFile", false);
                    key.DeleteValue("CopySelfAAVRecUpdateFrom", false);
                    key.DeleteValue("CopySelfAAVRecUpdateTo", false);
					key.DeleteValue("AAVRecUpdateAAVRecLocation", false);
					key.DeleteValue("AAVRecUpdateAcceptBeta", false);
                }
                catch (Exception)
                { }
                finally
                {
                    key.Close();
                }
            }
        }

        public int CurrentlyInstalledFileVersion(string aavRecLocation, string relativeFilePath)
        {
			if (Directory.Exists(aavRecLocation))
            {
				string owExeFilePath = Path.GetFullPath(aavRecLocation + "/" + relativeFilePath);
                if (File.Exists(owExeFilePath))
                {
                    try
                    {
                        AssemblyName an = AssemblyName.GetAssemblyName(owExeFilePath);
                        Version owVer = an.Version;
                        return 1000000 * owVer.Major + 100000 * owVer.Minor + 10000 * owVer.Build + owVer.Revision;
                    }
                    catch
                    { }
                }
                else
                    return -1;
            }
            else
            {
				Directory.CreateDirectory(aavRecLocation);
            }

            return 0;
        }

		public int CurrentlyInstalledAAVRecVersion(string aavRecLocation)
        {
			if (Directory.Exists(aavRecLocation))
            {
				string owExeFilePath = Path.GetFullPath(aavRecLocation + "/AAVRec.exe");
                if (File.Exists(owExeFilePath))
                {
                    try
                    {
                        AssemblyName an = AssemblyName.GetAssemblyName(owExeFilePath);
                        Version owVer = an.Version;
                        return 1000000 * owVer.Major + 100000 * owVer.Minor + 10000 * owVer.Build + owVer.Revision;
                    }
                    catch
                    { }
                }
            }
            else
            {
				Directory.CreateDirectory(aavRecLocation);
            }

            return 0;
        }

        public string VersionToVersionString(int version)
        {
            //  1000000 * owVer.Major + 100000 * owVer.Minor + 10000 * owVer.Build + owVer.Revision
            StringBuilder outVer = new StringBuilder();
            outVer.Append(Convert.ToString(version / 1000000));
            outVer.Append(".");

            version = version % 1000000;
            outVer.Append(Convert.ToString(version / 100000));
            outVer.Append(".");

            version = version % 100000;
            outVer.Append(Convert.ToString(version / 10000));
            outVer.Append(".");
            outVer.Append(Convert.ToString(version % 10000));

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

        public int AAVRecUpdateVersionStringToVersion(string versionString)
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
