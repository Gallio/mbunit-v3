// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
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
using System.Diagnostics;
using System.IO;
using Gallio.Common.IO;
using Gallio.Common.Remoting;
using Gallio.Runtime.Debugging;
using Gallio.Runtime.Loader;
using Gallio.Runtime.Logging;
using Gallio.Common.Reflection;

namespace Gallio.Runtime.Hosting
{
    /// <summary>
    /// An isolated app domain host is a <see cref="IHost"/> the runs code within an
    /// isolated <see cref="AppDomain" /> of this process.  Communication with the
    /// <see cref="AppDomain" /> occurs over an inter-<see cref="AppDomain" /> .Net
    /// remoting channel.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <see cref="IsolatedAppDomainHost" /> does not support the
    /// <see cref="HostConfiguration.LegacyUnhandledExceptionPolicyEnabled" />
    /// option because it seems to be ignored by the .Net runtime except when set on
    /// the default AppDomain of the process.
    /// </para>
    /// </remarks>
    public class IsolatedAppDomainHost : RemoteHost
    {
        private readonly IDebuggerManager debuggerManager;

        private AppDomain appDomain;
        private CurrentDirectorySwitcher currentDirectorySwitcher;
        private string temporaryConfigurationFilePath;

        /// <summary>
        /// Creates an uninitialized host.
        /// </summary>
        /// <param name="hostSetup">The host setup.</param>
        /// <param name="logger">The logger for host message output.</param>
        /// <param name="debuggerManager">The debugger manager.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="hostSetup"/>, <paramref name="logger"/>
        /// or <paramref name="debuggerManager"/> is null.</exception>
        public IsolatedAppDomainHost(HostSetup hostSetup, ILogger logger, IDebuggerManager debuggerManager)
            : base(hostSetup, logger, null)
        {
            if (debuggerManager == null)
                throw new ArgumentNullException("debuggerManager");

            this.debuggerManager = debuggerManager;
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
                FreeResources();
        }

        /// <inheritdoc />
        protected override IRemoteHostService AcquireRemoteHostService()
        {
            try
            {
                SetWorkingDirectory();
                CreateTemporaryConfigurationFile();
                CreateAppDomain();
                AttachDebuggerIfNeeded(debuggerManager, Process.GetCurrentProcess());

                var endpoint = (IsolatedAppDomainEndpoint)AppDomainUtils.CreateRemoteInstance(appDomain, typeof(IsolatedAppDomainEndpoint));

                foreach (string hintDirectory in HostSetup.HintDirectories)
                    endpoint.AddHintDirectory(hintDirectory);

                return endpoint.CreateRemoteHostService(null);
            }
            catch (Exception)
            {
                FreeResources();
                throw;
            }
        }

        private void FreeResources()
        {
            DetachDebuggerIfNeeded();
            UnloadAppDomain();
            ResetWorkingDirectory();
            DeleteTemporaryConfigurationFile();
        }

        private void SetWorkingDirectory()
        {
            if (! string.IsNullOrEmpty(HostSetup.WorkingDirectory))
                currentDirectorySwitcher = new CurrentDirectorySwitcher(HostSetup.WorkingDirectory);
        }

        private void CreateTemporaryConfigurationFile()
        {
            try
            {
                HostSetup patchedSetup = HostSetup.Copy();
                patchedSetup.Configuration.AddAssemblyBinding(new AssemblyBinding(typeof(IsolatedAppDomainHost).Assembly));

                temporaryConfigurationFilePath = patchedSetup.WriteTemporaryConfigurationFile();
            }
            catch (Exception ex)
            {
                throw new HostException("Could not write the temporary configuration file.", ex);
            }
        }

        private void CreateAppDomain()
        {
            try
            {
                appDomain = AppDomainUtils.CreateAppDomain(@"IsolatedAppDomainHost",
                    HostSetup.ApplicationBaseDirectory, temporaryConfigurationFilePath, HostSetup.ShadowCopy);
            }
            catch (Exception ex)
            {
                throw new HostException("Could not create the isolated AppDomain.", ex);
            }
        }

        private void UnloadAppDomain()
        {
            try
            {
                if (appDomain != null)
                    AppDomain.Unload(appDomain);
            }
            catch (Exception ex)
            {
                Logger.Log(LogSeverity.Warning, "Could not unload AppDomain.", ex);
            }
            finally
            {
                appDomain = null;
            }
        }

        private void ResetWorkingDirectory()
        {
            try
            {
                if (currentDirectorySwitcher != null)
                    currentDirectorySwitcher.Dispose();
            }
            catch (Exception ex)
            {
                Logger.Log(LogSeverity.Warning, "Could not reset working directory.", ex);
            }
            finally
            {
                currentDirectorySwitcher = null;
            }
        }

        private void DeleteTemporaryConfigurationFile()
        {
            try
            {
                if (temporaryConfigurationFilePath != null)
                    File.Delete(temporaryConfigurationFilePath);
            }
            catch (Exception ex)
            {
                Logger.Log(LogSeverity.Warning, "Could not delete temporary configuration file.", ex);
            }
            finally
            {
                temporaryConfigurationFilePath = null;
            }
        }

        internal class IsolatedAppDomainEndpoint : LongLivedMarshalByRefObject
        {
            private DefaultAssemblyLoader assemblyLoader;

            public void AddHintDirectory(string hintDirectory)
            {
                if (assemblyLoader == null)
                    assemblyLoader = new DefaultAssemblyLoader();

                assemblyLoader.AddHintDirectory(hintDirectory);
            }

            public IRemoteHostService CreateRemoteHostService(TimeSpan? watchdogTimeout)
            {
                return new RemoteHostService(watchdogTimeout);
            }
        }
    }
}
