/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Lifetime;
using System.Security;
using System.Security.Policy;
using System.Text;
using OccuRec.ASCOM.Interfaces;
using OccuRec.ASCOM.Interfaces.System;
using OccuRec.Utilities;

namespace OccuRec.ASCOM.Wrapper
{
	[Serializable]
	internal class DeviceClient : IDisposable
	{
        private static TraceSwitch TraceSwitchASCOMClient = new TraceSwitch("ASCOMClient", "ASCOMServer tracing.");

        private static string APP_DOMAIN_PREFIX = "OccuRec.ASCOM.Isolation.";
		private AppDomain m_HostDomain;
		private string m_DomainName;
		private AssemblyName m_AssemblyName;
		protected IIsolatedDevice m_Instance;

		protected void LoadInAppDomain(string fullTypeName, ASCOMClient ascomClient)
		{
			string[] tokens = fullTypeName.Split(new char[] { ',' }, 2);
			m_AssemblyName = new AssemblyName(tokens[1]);

			var appSetup = new AppDomainSetup()
			{
				ApplicationName = "OccuRec",
				ApplicationBase = AppDomain.CurrentDomain.BaseDirectory,
				ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile
			};

            m_DomainName = string.Format("{0}{1}.v{2}", APP_DOMAIN_PREFIX, m_AssemblyName.Name, m_AssemblyName.Version != null ? m_AssemblyName.Version.ToString() : "XX");

			var e = new Evidence();
			e.AddHostEvidence(new Zone(SecurityZone.MyComputer));
			PermissionSet pset = SecurityManager.GetStandardSandbox(e);

			m_HostDomain = AppDomain.CreateDomain(m_DomainName, AppDomain.CurrentDomain.Evidence, appSetup, pset, null);
			m_HostDomain.AssemblyResolve += m_HostDomain_AssemblyResolve;
			m_HostDomain.ReflectionOnlyAssemblyResolve += m_HostDomain_AssemblyResolve;
			m_HostDomain.UnhandledException += m_HostDomain_UnhandledException;
            m_HostDomain.DomainUnload += m_HostDomain_DomainUnload;

			object obj = m_HostDomain.CreateInstanceAndUnwrap(tokens[1], tokens[0]);

			ascomClient.RegisterLifetimeService(obj as MarshalByRefObject);

			m_Instance = (IIsolatedDevice)obj;
			m_Instance.Initialise(new OccuRecHostDelegate(tokens[0], ascomClient));
		}

		protected void LoadInCurrentDomain(string fullTypeName, ASCOMClient ascomClient)
		{
			string[] tokens = fullTypeName.Split(new char[] { ',' }, 2);
			m_AssemblyName = new AssemblyName(tokens[1]);

			m_HostDomain = null;
			Assembly asm = Assembly.Load(m_AssemblyName);
			object obj = asm.CreateInstance(tokens[0]);

			m_Instance = (IIsolatedDevice)obj;
			m_Instance.Initialise(new OccuRecHostDelegate(tokens[0], ascomClient));
		}

        void m_HostDomain_DomainUnload(object sender, EventArgs e)
        {
            if (TraceSwitchASCOMClient.TraceVerbose)
                Trace.WriteLine(string.Format("OccuRec: AppDomain('{0}').DomainUnload()", m_DomainName.Replace(APP_DOMAIN_PREFIX, "")));
        }

		void m_HostDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			if (e.ExceptionObject is Exception)
			{
                if (TraceSwitchASCOMClient.TraceError)
                    Trace.WriteLine(string.Format("OccuRec: AppDomain('{0}').UnhandledException = {1}", m_DomainName.Replace(APP_DOMAIN_PREFIX, ""), ((Exception)e.ExceptionObject).GetFullStackTrace()));
			}
		}

		Assembly m_HostDomain_AssemblyResolve(object sender, ResolveEventArgs args)
		{
			Assembly resolvedAssembly = null;

			string assemblyPath = Path.GetFullPath(string.Format(@"{0}\{1}.dll", Environment.CurrentDirectory, new AssemblyName(args.Name).Name));
			try
			{
				if (File.Exists(assemblyPath))
					resolvedAssembly = Assembly.LoadFile(assemblyPath);
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex);
			}


			return resolvedAssembly;
		}

		public void Dispose()
		{
		    if (m_Instance != null)
		        m_Instance.Finalise();

            m_Instance = null;

			if (m_HostDomain != null)
			{
				try
				{
					AppDomain.Unload(m_HostDomain);	
				}
				catch(CannotUnloadAppDomainException)
				{ }
			}

			m_HostDomain = null;
		}
	}
}
