using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using AAVRec.Properties;

namespace AAVRec.Helpers
{
    public class UpdateManager
    {
		public static int CurrentlyInstalledAAVRecVersion()
		{
			try
			{
				Assembly asm = Assembly.GetExecutingAssembly();
				Version owVer = asm.GetName().Version;
				return 1000000 * owVer.Major + 100000 * owVer.Minor + 10000 * owVer.Build + owVer.Revision;
			}
			catch
			{ }

			return 0;
		}

		public static int CurrentlyInstalledAAVRecUpdateVersion()
		{
			try
			{
				string woupdatePath = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + "\\AAVRecUpdate.exe");
				if (File.Exists(woupdatePath))
				{
					AssemblyName an = AssemblyName.GetAssemblyName(woupdatePath);
					Version owVer = an.Version;
					return 10000 * owVer.Major + 1000 * owVer.Minor + 100 * owVer.Build + owVer.Revision;
				}
				else
					return 0;
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex.ToString());
			}

			return 0;
		}

		public static int VersionStringToVersion(string versionString)
		{
			string[] tokens = versionString.Split('.');
			int version = 10000 * int.Parse(tokens[0]) + 1000 * int.Parse(tokens[1]) + 100 * int.Parse(tokens[2]) + int.Parse(tokens[3]);
			return version;
		}

		public static int CurrentlyInstalledModuleVersion(string moduleFileName)
		{
			try
			{
				string modulePath = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + "\\" + moduleFileName);
				if (File.Exists(modulePath))
				{
					Assembly asm = Assembly.ReflectionOnlyLoadFrom(modulePath);

					IList<CustomAttributeData> atts = CustomAttributeData.GetCustomAttributes(asm);
					foreach (CustomAttributeData cad in atts)
					{
						if (cad.Constructor.DeclaringType.FullName == "System.Reflection.AssemblyFileVersionAttribute")
						{
							string currVersionString = (string)cad.ConstructorArguments[0].Value;
							return VersionStringToVersion(currVersionString);
						}
					}
				}
				else
					return 0;
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex.ToString());
			}

			return 1000000;
		}

	    public static string UpdateLocation
	    {
			get { return "http://www.hristopavlov.net/AAVRec"; }
	    }

		public static string UpdatesXmlFileLocation
		{
			get
			{
				return UpdateLocation + (Settings.Default.AcceptBetaUpdates ? "/Updates.xml" : "/Beta.xml");
			}
		}
	}
}
