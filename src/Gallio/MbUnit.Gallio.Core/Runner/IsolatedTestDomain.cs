// Copyright 2007 MbUnit Project - http://www.mbunit.com/
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
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using MbUnit.Core.ProgressMonitoring;
using MbUnit.Core.RuntimeSupport;
using MbUnit.Core.Utilities;
using MbUnit.Framework.Kernel.RuntimeSupport;

namespace MbUnit.Core.Runner
{
    /// <summary>
    /// An isolated test domain loads test assemblies and runs tests within
    /// the context of a separate isolated <see cref="AppDomain" /> in the
    /// current process.  It uses .Net remoting for communication across
    /// <see cref="AppDomain" /> boundaries.  The <see cref="AppDomain" /> is
    /// created when a test project is loaded and is released on disposal.
    /// </summary>
    public class IsolatedTestDomain : RemoteTestDomain
    {
        private ICoreRuntime runtime;

        private AppDomain appDomain;
        private Bootstrapper bootstrapper;
        private Dictionary<Assembly, bool> bootstrapAssemblies;

        /// <summary>
        /// Creates an isolated test domain.
        /// </summary>
        /// <param name="runtime">The runtime from which to obtain services</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="runtime"/> is null</exception>
        public IsolatedTestDomain(ICoreRuntime runtime)
        {
            if (runtime == null)
                throw new ArgumentNullException("runtime");

            this.runtime = runtime;
        }

        /// <summary>
        /// Gets the list of assemblies from the host domain that must be loaded into
        /// the isolated AppDomain in order to bootstrap the test framework.  The value
        /// specifies whether a binding redirect should be used to force this version
        /// of the assembly to be used.
        /// </summary>
        /// <remarks>
        /// This list is pre-populated with Castle.Core, Castle.MicroKernel,
        /// MbUnit.Gallio.Core and MbUnit.Gallio.Framework.
        /// </remarks>
        public IDictionary<Assembly, bool> BootstrapAssemblies
        {
            get
            {
                if (bootstrapAssemblies == null)
                {
                    bootstrapAssemblies = new Dictionary<Assembly, bool>();
                    bootstrapAssemblies.Add(typeof(Castle.Core.Logging.ILogger).Assembly, false);
                    bootstrapAssemblies.Add(typeof(Castle.MicroKernel.IKernel).Assembly, false);
                    bootstrapAssemblies.Add(typeof(Castle.Windsor.WindsorContainer).Assembly, false);
                    bootstrapAssemblies.Add(typeof(Castle.DynamicProxy.ProxyGenerator).Assembly, false);
                    bootstrapAssemblies.Add(typeof(MbUnit.Core.Runner.ITestDomain).Assembly, false);
                    bootstrapAssemblies.Add(typeof(MbUnit.Framework.Assert).Assembly, true);
                }

                return bootstrapAssemblies;
            }
        }

        /// <summary>
        /// Adds a contributor to the test domain.
        /// </summary>
        /// <param name="contributor">The contributor</param>
        public void AddContributor(IIsolatedTestDomainContributor contributor)
        {
            contributor.Apply(this);
        }

        /// <inheritdoc />
        protected override ITestDomain InternalConnect(IProgressMonitor progressMonitor)
        {
            try
            {
                progressMonitor.SetStatus("Creating the isolated AppDomain.");

                CreateAppDomain();

                progressMonitor.Worked(0.7);
            }
            catch (Exception ex)
            {
                throw new FatalRunnerException("Could not create the isolated AppDomain.", ex);
            }

            try
            {
                progressMonitor.SetStatus("Initializing the test runner.");

                bootstrapper = CreateRemoteInstance<Bootstrapper>();
                bootstrapper.Initialize(runtime.GetRuntimeSetup());

                ITestDomain testDomain = bootstrapper.CreateTestDomain();

                progressMonitor.Worked(0.3);
                return testDomain;
            }
            catch (Exception ex)
            {
                throw new FatalRunnerException("Could not initialize the isolated runtime environment.", ex);
            }
        }

        /// <inheritdoc />
        protected override void InternalDisconnect(IProgressMonitor progressMonitor)
        {
            try
            {
                try
                {
                    progressMonitor.SetStatus("Shutting down the test runner.");

                    if (bootstrapper != null)
                        bootstrapper.Shutdown();

                    progressMonitor.Worked(0.3);
                }
                finally
                {
                    progressMonitor.SetStatus("Unloading the isolate AppDomain.");

                    bootstrapper = null;

                    if (appDomain != null)
                        AppDomain.Unload(appDomain);

                    progressMonitor.Worked(0.7);
                }
            }
            catch (Exception ex)
            {
                throw new FatalRunnerException("Could not unload the isolated AppDomain.", ex);
            }
            finally
            {
                appDomain = null;
            }
        }

        private void CreateAppDomain()
        {
            AppDomainSetup setup = new AppDomainSetup();

            if (Package.ApplicationBase.Length == 0
                || !Path.IsPathRooted(Package.ApplicationBase))
                setup.ApplicationBase = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Package.ApplicationBase);
            else
                setup.ApplicationBase = Package.ApplicationBase;

            setup.ApplicationName = "IsolatedTestDomain";

            if (Package.EnableShadowCopy)
            {
                setup.ShadowCopyFiles = "true";
                setup.ShadowCopyDirectories = Package.ApplicationBase;
                // FIXME: should we also shadow-copy all assembly reference paths?
            }

            // NOTE: Loader optimization and override configuration not supported by Mono.
            if (!MonoSupport.IsUsingMonoRuntime)
                ConfigureAppDomainSetupForCLR(setup);

            Evidence evidence = new Evidence(AppDomain.CurrentDomain.Evidence); // FIXME: should be more careful here
            appDomain = AppDomain.CreateDomain("IsolatedTestDomain", evidence, setup);
        }

        private void ConfigureAppDomainSetupForCLR(AppDomainSetup setup)
        {
            setup.LoaderOptimization = LoaderOptimization.MultiDomain;
            setup.SetConfigurationBytes(CreateConfiguration());
        }

        private T CreateRemoteInstance<T>(params object[] args)
        {
            Type type = typeof(T);
            string assemblyFile = type.Assembly.Location;
            return (T) appDomain.CreateInstanceFromAndUnwrap(assemblyFile, type.FullName, false,
                BindingFlags.Default, null, args, null, null, null);
        }

        /// <summary>
        /// Creates a configuration for the new AppDomain that enables the legacy exception
        /// policy and sets a binding redirect for MbUnit.  The binding redirect is used to
        /// reduce the impact of version conflicts between the runner and the test assemblies.
        /// It's not a perfect solution however.  A better approach would be to load assemblies
        /// compiled against different MbUnit framework versions into distinct AppDomains
        /// (indeed, perhaps just create one AppDomain per assembly) and use the correct
        /// version of MbUnit for each one.
        /// FIXME: Do something better...
        /// </summary>
        /// <returns>The configuration</returns>
        private byte[] CreateConfiguration()
        {
            Type domainType = typeof(IsolatedTestDomain);
            string templateResourceName = domainType.FullName + ".config.template";

            string template;
            using (TextReader reader = new StreamReader(domainType.Assembly.GetManifestResourceStream(templateResourceName)))
            {
                template = reader.ReadToEnd();
            }

            StringBuilder assemblyBindings = new StringBuilder();
            foreach (KeyValuePair<Assembly, bool> entry in BootstrapAssemblies)
            {
                AppendAssemblyBinding(assemblyBindings, entry.Key, entry.Value);
            }

            template = template.Replace("{ASSEMBLY_BINDINGS}", assemblyBindings.ToString());
            return Encoding.ASCII.GetBytes(template);
        }

        private static void AppendAssemblyBinding(StringBuilder assemblyBindings, Assembly assembly, bool bindingRedirect)
        {
            AssemblyName assemblyName = assembly.GetName();

            assemblyBindings.Append("<dependentAssembly>");

            assemblyBindings.AppendFormat("<assemblyIdentity name=\"{0}\" ", assemblyName.Name);
            if (assemblyName.GetPublicKeyToken() != null)
                assemblyBindings.AppendFormat("publicKeyToken=\"{0}\" ", ToHex(assemblyName.GetPublicKeyToken()));
            //if (assemblyName.CultureInfo != null)
            //    assemblyBindings.AppendFormat("culture=\"{0}\" ", assemblyName.CultureInfo.Name);
            assemblyBindings.Append("/>");

            if (bindingRedirect)
                assemblyBindings.AppendFormat("<bindingRedirect oldVersion=\"0.0.0.0-65535.65535.65535.65535\" newVersion=\"{0}\" />",
                    assemblyName.Version);

            assemblyBindings.AppendFormat("<codeBase version=\"{0}\" href=\"{1}\" />",
                assemblyName.Version, assembly.CodeBase);

            assemblyBindings.Append("</dependentAssembly>");
        }

        private static string ToHex(byte[] bytes)
        {
            StringBuilder builder = new StringBuilder();
            foreach (byte b in bytes)
                builder.Append(Convert.ToString(b, 16));
            return builder.ToString();
        }
    }
}
