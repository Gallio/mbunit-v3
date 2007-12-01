using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Xml.XPath;
using Gallio.Hosting;
using Gallio.ReSharperRunner;
using JetBrains.UI.Shell;

namespace Gallio.ReSharperRunner.Hosting
{
    /// <summary>
    /// Resolves and initializes the Gallio <see cref="Runtime" /> environment.
    /// This class is constructed in such a way as to conceal dependencies
    /// on the Gallio assemblies.  If for whatever reason the assemblies cannot
    /// be loaded, the runtime proxy will revert to null implementations of
    /// services to avoid generating repeated errors.
    /// </summary>
    public static class RuntimeProxy
    {
        private const string GallioInstallationPathConfigXPath = "//GallioInstallationPath";

        private static readonly object syncRoot = new object();
        private static bool? initializationState;

        private static Dictionary<Type, Type> nullComponentTypes;

        /// <summary>
        /// Resolves a component that implements a runtime service.
        /// </summary>
        /// <typeparam name="T">The service type</typeparam>
        /// <returns>The component</returns>
        /// <exception cref="RuntimeProxyException">Thrown if the service cannot be resolved</exception>
        public static T Resolve<T>()
        {
            if (Initialize())
                return Protected.Resolve<T>();

            return (T) ResolveNullComponent(typeof(T));
        }

        private static object ResolveNullComponent(Type serviceType)
        {
            InitializeNullComponentTypes();

            Type componentType;
            if (nullComponentTypes.TryGetValue(serviceType, out componentType))
                return Activator.CreateInstance(componentType);

            throw new RuntimeProxyException("Cannot resolve the service because the Gallio runtime is not initialized.");
        }

        private static void InitializeNullComponentTypes()
        {
            lock (syncRoot)
            {
                if (nullComponentTypes == null)
                {
                    nullComponentTypes = new Dictionary<Type, Type>();
                    nullComponentTypes.Add(typeof(IUnitTestProviderDelegate), typeof(NullUnitTestProviderDelegate));
                }
            }
        }

        private static bool Initialize()
        {
            for (; ; )
            {
                Exception exception;

                lock (syncRoot)
                {
                    try
                    {
                        if (initializationState.HasValue)
                            return initializationState.Value;

                        InitializeRuntime();

                        initializationState = true;
                        return true;
                    }
                    catch (Exception ex)
                    {
                        initializationState = false;
                        exception = ex;
                    }
                }

                if (!DisplayInitializationError(exception))
                    return false;

                initializationState = null;
            }
        }

        private static string GetConfigurationFilePath()
        {
            return typeof(RuntimeProxy).Assembly.Location + ".config";
        }

        private static void InitializeRuntime()
        {
            string configFilePath = GetConfigurationFilePath();
            AssemblyResolver resolver = CreateAssemblyResolverIfNeeded(configFilePath);

            try
            {
                if (resolver != null)
                    resolver.Install(AppDomain.CurrentDomain);

                Protected.InitializeRuntime(configFilePath);
            }
            catch
            {
                if (resolver != null)
                    resolver.Uninstall(AppDomain.CurrentDomain);

                throw;
            }
        }

        private static AssemblyResolver CreateAssemblyResolverIfNeeded(string configFilePath)
        {
            if (IsRuntimeAssemblyAccessible())
                return null;

            Exception exception;
            try
            {
                XPathDocument doc = new XPathDocument(configFilePath);
                XPathNavigator navigator = doc.CreateNavigator().SelectSingleNode(GallioInstallationPathConfigXPath);

                if (navigator != null)
                {
                    string installationPath = navigator.Value;

                    if (!Path.IsPathRooted(installationPath))
                    {
                        string pluginBasePath = Path.GetDirectoryName(Path.GetFullPath(typeof(RuntimeProxy).Assembly.Location));

                        if (installationPath.Length == 0)
                            installationPath = pluginBasePath;
                        else
                            installationPath = Path.GetFullPath(Path.Combine(pluginBasePath, installationPath));
                    }

                    return new AssemblyResolver(installationPath);
                }

                exception = null;
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            throw new RuntimeProxyException(String.Format("Could not obtain the Gallio installation path from the config file: '{0}'.",
                                                          configFilePath), exception);
        }

        private static bool DisplayInitializationError(Exception ex)
        {
            // TODO: Improve error reporting.
            return WindowUtil.ShowMessageBox(
                       String.Format("An exception occurred while initializing the Gallio "
                                     + "runtime components.  The Gallio test runner plugin will be "
                                     + "disabled for this session.\n\n{0}", ex),
                       "Gallio Runtime Error",
                       MessageBoxButtons.RetryCancel) == DialogResult.Retry;
        }

        private static bool IsRuntimeAssemblyAccessible()
        {
            try
            {
                Protected.ThrowAnExceptionIfTheRuntimeAssemblyIsNotAccessible();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// These methods are isolated protect the caller from type loading errors.
        /// </summary>
        private static class Protected
        {
            public static T Resolve<T>()
            {
                return Runtime.Instance.Resolve<T>();
            }

            public static void InitializeRuntime(string configFilePath)
            {
                RuntimeSetup setup = new RuntimeSetup();
                setup.ConfigurationFilePath = configFilePath;

                Runtime.Initialize(setup);
            }

            public static void ThrowAnExceptionIfTheRuntimeAssemblyIsNotAccessible()
            {
                // Check whether we can run this code without failure.
                GC.KeepAlive(Runtime.IsInitialized);
            }
        }

        /// <summary>
        /// An assembly resolver.
        /// We can't actually use <see cref="JetBrains.Util.AssemblyResolver" /> because it
        /// assumes it can compare <see cref="AssemblyName" /> instances using <see cref="object.Equals(object)"/>
        /// but that method is not defined appropriately.
        /// Obviously we also can't use the resolver defined by Gallio.
        /// </summary>
        /// <remarks author="jeff">
        /// I've implemented this code in a similar form as the ReSharper type so we can
        /// revert to it once they fix the bug.
        /// </remarks>
        private class AssemblyResolver
        {
            private readonly string dir;

            public AssemblyResolver(string dir)
            {
                this.dir = dir;
            }

            public void Install(AppDomain appDomain)
            {
                appDomain.AssemblyResolve += AssemblyResolve;
            }

            public void Uninstall(AppDomain appDomain)
            {
                appDomain.AssemblyResolve -= AssemblyResolve;
            }

            private Assembly AssemblyResolve(object sender, ResolveEventArgs args)
            {
                String[] splitName = args.Name.Split(',');
                String displayName = splitName[0];

                string assemblyFile = Path.GetFullPath(Path.Combine(dir, displayName));

                if (File.Exists(assemblyFile))
                    return Assembly.LoadFrom(assemblyFile);

                if (File.Exists(assemblyFile + @".dll"))
                    return Assembly.LoadFrom(assemblyFile + @".dll");

                if (File.Exists(assemblyFile + @".exe"))
                    return Assembly.LoadFrom(assemblyFile + @".exe");

                return null;
            }
        }
    }
}