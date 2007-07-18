using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using Castle.Core;
using Castle.Core.Resource;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using Castle.Windsor.Configuration.Interpreters.XmlProcessor;
using MbUnit.Framework.Services.Runtime;

namespace MbUnit.Core.Services.Runtime
{
    /// <summary>
    /// Default implementation of <see cref="IRuntime" /> based on the
    /// Castle inversion of control microkernel.
    /// </summary>
    /// <remarks>
    /// Loads plugins automatically when initialized by searching for configuration
    /// files named "MbUnit.*.plugin" in the plugin load directories and installing
    /// any Windsor configuration sections into the container.
    /// </remarks>
    [Singleton]
    public class WindsorRuntime : IRuntime, IContainerAccessor, IInitializable, IDisposable
    {
        private WindsorContainer container;
        private IAssemblyResolverManager assemblyResolverManager;
        private List<string> pluginDirectories;

        /// <summary>
        /// Initializes the runtime.
        /// </summary>
        /// <param name="assemblyResolverManager">The assembly resolver to use</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="assemblyResolverManager"/> is null</exception>
        public WindsorRuntime(IAssemblyResolverManager assemblyResolverManager)
        {
            if (assemblyResolverManager == null)
                throw new ArgumentNullException("assemblyResolverManager");

            this.assemblyResolverManager = assemblyResolverManager;

            container = new WindsorContainer();
            pluginDirectories = new List<string>();

            SetDefaultPluginDirectories();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (container != null)
            {
                container.Dispose();
                container = null;
            }
        }

        /// <inheritdoc />
        public IWindsorContainer Container
        {
            get { return container; }
        }

        /// <summary>
        /// Gets a mutable list of directories to be searched for plugins configuration files.
        /// </summary>
        /// <value>
        /// Initially contains the MbUnit core directory.
        /// </value>
        public IList<string> PluginDirectories
        {
            get { return pluginDirectories; }
        }

        /// <inheritdoc />
        public void Initialize()
        {
            ThrowIfDisposed();

            container.Kernel.AddComponentInstance("Core.Runtime", typeof(IRuntime), this);
            container.Kernel.AddComponentInstance("Core.AssemblyResolverManager", typeof(IAssemblyResolverManager), assemblyResolverManager);

            LoadAllPluginConfiguration();
            RunContainerInstaller();
        }

        /// <inheritdoc />
        public object Resolve(Type service)
        {
            return container.Resolve(service);
        }

        /// <inheritdoc />
        public T Resolve<T>()
        {
            return container.Resolve<T>();
        }

        /// <inheritdoc />
        public T[] ResolveAll<T>()
        {
            return container.Kernel.ResolveServices<T>();
        }

        private void ThrowIfDisposed()
        {
            if (container == null)
                throw new ObjectDisposedException("The runtime has been disposed.");
        }

        private void SetDefaultPluginDirectories()
        {
            string coreCodebase = new Uri(typeof(WindsorRuntime).Assembly.CodeBase).LocalPath;

            pluginDirectories.Add(Path.GetDirectoryName(Path.GetFullPath(coreCodebase)));
        }

        private void LoadAllPluginConfiguration()
        {
            foreach (string pluginDirectory in pluginDirectories)
            {
                DirectoryInfo pluginDirectoryInfo = new DirectoryInfo(pluginDirectory);
                if (pluginDirectoryInfo.Exists)
                {
                    foreach (FileInfo pluginConfigFile in pluginDirectoryInfo.GetFiles("MbUnit.*.plugin"))
                    {
                        LoadConfigurationFromFile(pluginConfigFile.FullName);
                    }
                }
            }
        }

        private void LoadConfigurationFromFile(string configFile)
        {
            // Ensure that we can resolve any types referenced by the config file.
            assemblyResolverManager.AddHintDirectoryContainingFile(configFile);

            XmlElement rootElement;
            try
            {
                FileResource pluginManifestResource = new FileResource(configFile);
                XmlProcessor xmlProcessor = new XmlProcessor();
                rootElement = xmlProcessor.Process(pluginManifestResource) as XmlElement;
            }
            catch (Exception ex)
            {
                throw new ConfigurationErrorsException(String.Format(CultureInfo.CurrentCulture,
                    "Unable to load or process config file '{0}'.", configFile), ex);
            }
            if (rootElement == null)
                throw new ConfigurationErrorsException(String.Format(CultureInfo.CurrentCulture,
                    "Plugin manifest Xml file '{0}' yielded unexpected Xml node after pre-processing.", configFile));

            XmlElement castleElement = rootElement.SelectSingleNode("//castle") as XmlElement;
            if (castleElement != null)
                LoadConfigurationFromResource(new StaticContentResource(castleElement.OuterXml));
        }

        private void LoadConfigurationFromResource(IResource resource)
        {
            XmlInterpreter configInterpreter = new XmlInterpreter(resource);
            configInterpreter.ProcessResource(resource, container.Kernel.ConfigurationStore);
        }

        private void RunContainerInstaller()
        {
            container.Installer.SetUp(container, container.Kernel.ConfigurationStore);
        }
    }
}
