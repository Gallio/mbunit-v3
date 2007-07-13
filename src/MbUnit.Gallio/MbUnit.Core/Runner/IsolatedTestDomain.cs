using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using MbUnit.Core.Services.Runtime;

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
        private AppDomain appDomain;

        protected override ITestDomain InternalConnect()
        {
            try
            {
                AppDomainSetup setup = new AppDomainSetup();

                if (TestProject.ApplicationBase.Length == 0
                    || !Path.IsPathRooted(TestProject.ApplicationBase))
                    setup.ApplicationBase = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, TestProject.ApplicationBase);
                else
                    setup.ApplicationBase = TestProject.ApplicationBase;

                setup.ApplicationName = "IsolatedTestDomain";

                if (TestProject.EnableShadowCopy)
                {
                    setup.ShadowCopyFiles = "true";
                    setup.ShadowCopyDirectories = TestProject.ApplicationBase;
                    // FIXME: should we also shadow-copy all assembly reference paths?
                }

                setup.LoaderOptimization = LoaderOptimization.MultiDomain;
                setup.SetConfigurationBytes(CreateConfiguration());

                Evidence evidence = new Evidence(AppDomain.CurrentDomain.Evidence); // FIXME: should be more careful here
                appDomain = AppDomain.CreateDomain("IsolatedTestDomain", evidence, setup);

                return Bootstrap();
            }
            catch (Exception ex)
            {
                throw new FatalRunnerException("Could not create the isolated AppDomain.", ex);
            }
        }

        protected override void InternalDisconnect()
        {
            try
            {
                if (appDomain != null)
                    AppDomain.Unload(appDomain);
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

        private ITestDomain Bootstrap()
        {
            // Initialize the assembly resolver.
            DefaultAssemblyResolverManager assemblyResolverManager = CreateRemoteInstance<DefaultAssemblyResolverManager>();
            assemblyResolverManager.AddMbUnitDirectories();

            // Get the test domain.
            LocalTestDomain testDomain = CreateRemoteInstance<LocalTestDomain>(assemblyResolverManager);
            return testDomain;
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
        private static byte[] CreateConfiguration()
        {
            Assembly assembly = typeof(IsolatedTestDomain).Assembly;
            string templateResourceName = typeof(IsolatedTestDomain).FullName + ".config.template";

            string template;
            using (TextReader reader = new StreamReader(assembly.GetManifestResourceStream(templateResourceName)))
            {
                template = reader.ReadToEnd();
            }

            AssemblyName assemblyName = assembly.GetName();
            template = template.Replace("{MBUNIT_ASSEMBLY_NAME}", assemblyName.Name);
            template = template.Replace("{MBUNIT_ASSEMBLY_PUBLIC_KEY}", ToHex(assemblyName.GetPublicKeyToken()));
            template = template.Replace("{MBUNIT_ASSEMBLY_VERSION}", assemblyName.Version.ToString());
            template = template.Replace("{MBUNIT_ASSEMBLY_CODEBASE}", assemblyName.CodeBase);
            return Encoding.ASCII.GetBytes(template);
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
