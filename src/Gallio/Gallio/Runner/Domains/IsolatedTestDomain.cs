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
using Gallio.Core.ProgressMonitoring;
using Gallio.Model;
using Gallio.Runner;
using Gallio.Hosting;
using Gallio.Runner.Domains;

namespace Gallio.Runner.Domains
{
    /// <summary>
    /// An isolated test domain loads test assemblies and runs tests within
    /// the context of a separate isolated <see cref="AppDomain" /> in the
    /// current process.  It uses .Net remoting for communication across
    /// <see cref="AppDomain" /> boundaries.  The <see cref="AppDomain" /> is
    /// created when a test project is loaded and is released on disposal.
    /// </summary>
    /// <remarks>
    /// The <see cref="Runtime" /> does not need to be initialized in order
    /// to use a <see cref="IsolatedTestDomain" />.  However, the domain
    /// will automatically initialize the <see cref="Runtime" /> within
    /// the isolated <see cref="AppDomain" /> that it creates.
    /// </remarks>
    public class IsolatedTestDomain : RemoteTestDomain
    {
        private readonly RuntimeSetup runtimeSetup;
        private readonly Dictionary<Assembly, bool> bootstrapAssemblies;

        private AppDomain appDomain;
        private Bootstrapper bootstrapper;

        /// <summary>
        /// Creates an isolated test domain.
        /// </summary>
        /// <param name="runtimeSetup">The runtime setup for the runtime to initialize within the isolated <see cref="AppDomain" /></param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="runtimeSetup"/> is null</exception>
        public IsolatedTestDomain(RuntimeSetup runtimeSetup)
        {
            if (runtimeSetup == null)
                throw new ArgumentNullException("runtimeSetup");

            this.runtimeSetup = runtimeSetup;

            bootstrapAssemblies = new Dictionary<Assembly, bool>();

            AddBootstrapAssembly(typeof(Castle.Core.Logging.ILogger).Assembly, false);
            AddBootstrapAssembly(typeof(Castle.MicroKernel.IKernel).Assembly, false);
            AddBootstrapAssembly(typeof(Castle.Windsor.WindsorContainer).Assembly, false);
            AddBootstrapAssembly(typeof(Castle.DynamicProxy.ProxyGenerator).Assembly, false);
            AddBootstrapAssembly(typeof(IsolatedTestDomain).Assembly, false);
        }

        /// <summary>
        /// Adds a bootstrap assembly entry that forces a particular assembly
        /// to be loaded into the isolated test domain.
        /// </summary>
        /// <param name="assembly">The assembly</param>
        /// <param name="useBindingRedirect">If true, uses a binding redirect to
        /// ensure that this exact version of the assembly is used</param>
        public void AddBootstrapAssembly(Assembly assembly, bool useBindingRedirect)
        {
            bootstrapAssemblies.Add(assembly, useBindingRedirect);
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
        protected override ITestDomain InternalConnect(TestPackageConfig packageConfig, IProgressMonitor progressMonitor)
        {
            progressMonitor.SetStatus("Creating the isolated AppDomain.");
            CreateAppDomain(packageConfig);
            progressMonitor.Worked(0.7);

            progressMonitor.SetStatus("Initializing the test runner.");
            ITestDomain testDomain = InitializeBootstrapper();
            progressMonitor.Worked(0.3);
            return testDomain;
        }

        /// <inheritdoc />
        protected override void InternalDisconnect(IProgressMonitor progressMonitor)
        {
            try
            {
                progressMonitor.SetStatus("Shutting down the test runner.");
                ShutdownBootstrapper();
                progressMonitor.Worked(0.3);
            }
            finally
            {
                progressMonitor.SetStatus("Unloading the isolated AppDomain.");
                UnloadAppDomain();
                progressMonitor.Worked(0.7);
            }
        }

        private void CreateAppDomain(TestPackageConfig packageConfig)
        {
            try
            {
                AppDomainSetup setup = new AppDomainSetup();

                if (packageConfig.ApplicationBase.Length == 0
                    || !Path.IsPathRooted(packageConfig.ApplicationBase))
                    setup.ApplicationBase = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, packageConfig.ApplicationBase);
                else
                    setup.ApplicationBase = packageConfig.ApplicationBase;

                setup.ApplicationName = "IsolatedTestDomain";

                if (packageConfig.EnableShadowCopy)
                {
                    setup.ShadowCopyFiles = @"true";
                    setup.ShadowCopyDirectories = packageConfig.ApplicationBase;
                    // FIXME: should we also shadow-copy all assembly reference paths?
                }

                // NOTE: Loader optimization and override configuration not supported by Mono.
                if (!Runtime.IsUsingMono)
                    ConfigureAppDomainSetupForCLR(setup);

                Evidence evidence = new Evidence(AppDomain.CurrentDomain.Evidence); // FIXME: should be more careful here
                appDomain = AppDomain.CreateDomain("IsolatedTestDomain", evidence, setup);
            }
            catch (Exception ex)
            {
                throw new RunnerException("Could not create the isolated AppDomain.", ex);
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
                throw new RunnerException("Could not unload the isolated AppDomain.", ex);
            }
            finally
            {
                appDomain = null;
            }
        }

        private ITestDomain InitializeBootstrapper()
        {
            try
            {
                bootstrapper = CreateRemoteInstance<Bootstrapper>();
                bootstrapper.Initialize(runtimeSetup);

                return bootstrapper.CreateTestDomain();
            }
            catch (Exception ex)
            {
                throw new RunnerException("Could not initialize the isolated runtime environment.", ex);
            }
        }

        private void ShutdownBootstrapper()
        {
            try
            {
                if (bootstrapper != null)
                    bootstrapper.Shutdown();
            }
            catch (Exception ex)
            {
                throw new RunnerException("Could not cleanly shutdown the isolated runtime environment.", ex);
            }
            finally
            {
                bootstrapper = null;
            }
        }

        private void ConfigureAppDomainSetupForCLR(AppDomainSetup setup)
        {
            setup.LoaderOptimization = LoaderOptimization.MultiDomain;
            setup.SetConfigurationBytes(CreateConfiguration());
        }

        private T CreateRemoteInstance<T>(params object[] args)
        {
            Type type = typeof(T);
            string assemblyFile = Loader.GetFriendlyAssemblyLocation(type.Assembly);
            return (T) appDomain.CreateInstanceFromAndUnwrap(assemblyFile, type.FullName, false,
                BindingFlags.Default, null, args, null, null, null);
        }

        /// <summary>
        /// Creates a configuration for the new AppDomain that enables the legacy exception
        /// policy and sets a binding redirect for Gallio.  The binding redirect is used to
        /// reduce the impact of version conflicts between the runner and the test assemblies.
        /// It's not a perfect solution however.  A better approach would be to load assemblies
        /// compiled against different framework versions into distinct AppDomains
        /// (indeed, perhaps just create one AppDomain per assembly).
        /// FIXME: Do something better...
        /// </summary>
        /// <returns>The configuration</returns>
        private byte[] CreateConfiguration()
        {
            Type domainType = typeof(IsolatedTestDomain);
            string templateResourceName = domainType.FullName + @".config.template";

            string template;
            using (TextReader reader = new StreamReader(domainType.Assembly.GetManifestResourceStream(templateResourceName)))
            {
                template = reader.ReadToEnd();
            }

            StringBuilder assemblyBindings = new StringBuilder();
            foreach (KeyValuePair<Assembly, bool> entry in bootstrapAssemblies)
            {
                AppendAssemblyBinding(assemblyBindings, entry.Key, entry.Value);
            }

            template = template.Replace(@"{ASSEMBLY_BINDINGS}", assemblyBindings.ToString());
            return Encoding.ASCII.GetBytes(template);
        }

        private static void AppendAssemblyBinding(StringBuilder assemblyBindings, Assembly assembly, bool bindingRedirect)
        {
            AssemblyName assemblyName = assembly.GetName();

            assemblyBindings.AppendFormat("<qualifyAssembly partialName=\"{0}\" fullName=\"{1}\" />",
                assembly.GetName().Name, assembly.FullName);

            assemblyBindings.Append(@"<dependentAssembly>");

            assemblyBindings.AppendFormat("<assemblyIdentity name=\"{0}\" ", assemblyName.Name);
            if (assemblyName.GetPublicKeyToken() != null)
                assemblyBindings.AppendFormat("publicKeyToken=\"{0}\" ", ToHex(assemblyName.GetPublicKeyToken()));
            //if (assemblyName.CultureInfo != null)
            //    assemblyBindings.AppendFormat("culture=\"{0}\" ", assemblyName.CultureInfo.Name);
            assemblyBindings.Append(@"/>");

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