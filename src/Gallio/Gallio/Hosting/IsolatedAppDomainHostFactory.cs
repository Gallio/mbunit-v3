// Copyright 2008 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Text;

namespace Gallio.Hosting
{
    /// <summary>
    /// <para>
    /// An isolated app domain host factory creates a <see cref="IHost"/> within an
    /// isolated <see cref="AppDomain" /> of this process.  Communication with the
    /// <see cref="AppDomain" /> occurs over an inter-<see cref="AppDomain" /> .Net
    /// remoting channel.
    /// </para>
    /// </summary>
    public class IsolatedAppDomainHostFactory : BaseHostFactory
    {
        /// <inheritdoc />
        protected override IHost CreateHostImpl(HostSetup hostSetup)
        {
            AppDomain appDomain = null;
            string oldWorkingDirectory = null;
            try
            {
                oldWorkingDirectory = Environment.CurrentDirectory;
                Environment.CurrentDirectory = hostSetup.WorkingDirectory;

                appDomain = CreateAppDomain(hostSetup);

                IRemoteHostService remoteHostService = CreateRemoteInstance<HostService>(appDomain, (TimeSpan?)null);
                return new IsolatedAppDomainHost(remoteHostService, appDomain, oldWorkingDirectory);
            }
            catch (Exception)
            {
                if (appDomain != null)
                    AppDomain.Unload(appDomain);
                if (oldWorkingDirectory != null)
                    Environment.CurrentDirectory = oldWorkingDirectory;
                throw;
            }
        }

        private AppDomain CreateAppDomain(HostSetup hostSetup)
        {
            try
            {
                AppDomainSetup appDomainSetup = new AppDomainSetup();

                appDomainSetup.ApplicationBase = hostSetup.ApplicationBaseDirectory;
                appDomainSetup.ApplicationName = @"IsolatedAppDomainHost";

                if (hostSetup.ShadowCopy)
                {
                    appDomainSetup.ShadowCopyFiles = @"true";
                    appDomainSetup.ShadowCopyDirectories = appDomainSetup.ApplicationBase;
                    // FIXME: should we also shadow-copy all assembly reference paths?
                }

                // NOTE: Loader optimization and override configuration not supported by Mono.
                if (!Runtime.IsUsingMono)
                    ConfigureAppDomainSetupForCLR(appDomainSetup, hostSetup);

                // TODO: Might need to be more careful about how the Evidence is derived.
                Evidence evidence = AppDomain.CurrentDomain.Evidence;
                PermissionSet defaultPermissionSet = new PermissionSet(PermissionState.Unrestricted);
                StrongName[] fullTrustAssemblies = new StrongName[0];
                return AppDomain.CreateDomain(appDomainSetup.ApplicationName, evidence, appDomainSetup, defaultPermissionSet, fullTrustAssemblies);
            }
            catch (Exception ex)
            {
                throw new HostException("Could not create the isolated AppDomain.", ex);
            }
        }

        private static T CreateRemoteInstance<T>(AppDomain appDomain, params object[] args)
        {
            Type type = typeof(T);
            string assemblyFile = Loader.GetFriendlyAssemblyLocation(type.Assembly);
            return (T)appDomain.CreateInstanceFromAndUnwrap(assemblyFile, type.FullName, false,
                BindingFlags.Default, null, args, null, null, null);
        }

        private static void ConfigureAppDomainSetupForCLR(AppDomainSetup appDomainSetup, HostSetup hostSetup)
        {
            appDomainSetup.LoaderOptimization = LoaderOptimization.MultiDomain;

            string configurationXml = hostSetup.Configuration.ToString();
            byte[] configurationBytes = Encoding.ASCII.GetBytes(configurationXml);
            appDomainSetup.SetConfigurationBytes(configurationBytes);
        }

        private sealed class IsolatedAppDomainHost : RemoteHost
        {
            private AppDomain appDomain;
            private string oldWorkingDirectory;

            public IsolatedAppDomainHost(IRemoteHostService remoteHostService, AppDomain appDomain, string oldWorkingDirectory)
                : base(remoteHostService, null)
            {
                this.appDomain = appDomain;
                this.oldWorkingDirectory = oldWorkingDirectory;
            }

            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);

                if (disposing)
                {
                    try
                    {
                        if (appDomain != null)
                            AppDomain.Unload(appDomain);

                        appDomain = null;
                    }
                    finally
                    {
                        if (oldWorkingDirectory != null)
                            Environment.CurrentDirectory = oldWorkingDirectory;

                        oldWorkingDirectory = null;
                    }
                }
            }
        }
    }
}
