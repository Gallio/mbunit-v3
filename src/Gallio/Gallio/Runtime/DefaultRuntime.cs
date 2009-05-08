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
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Xml;
using Gallio.Common.Collections;
using Gallio.Common.Platform;
using Gallio.Properties;
using Gallio.Common.Reflection;
using Gallio.Runtime.Extensibility;
using Gallio.Runtime.Loader;
using Gallio.Runtime.Logging;
using Gallio.Schema;

namespace Gallio.Runtime
{
    /// <summary>
    /// Default implementation of <see cref="IRuntime" />.
    /// </summary>
    public class DefaultRuntime : IRuntime
    {
        private IRegistry registry;
        private readonly IPluginLoader pluginLoader;
        private readonly IAssemblyResolverManager assemblyResolverManager;
        private readonly RuntimeSetup runtimeSetup;
        private readonly List<string> pluginDirectories;

        private ILogger logger;

        /// <summary>
        /// Initializes the runtime.
        /// </summary>
        /// <param name="registry">The registry</param>
        /// <param name="pluginLoader">The plugin loader</param>
        /// <param name="assemblyResolverManager">The assembly resolver to use</param>
        /// <param name="runtimeSetup">The runtime setup options</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="registry"/>,
        /// <paramref name="pluginLoader"/>, <paramref name="assemblyResolverManager"/> or
        /// <paramref name="runtimeSetup" /> is null</exception>
        public DefaultRuntime(IRegistry registry, IPluginLoader pluginLoader,
            IAssemblyResolverManager assemblyResolverManager, RuntimeSetup runtimeSetup)
        {
            if (registry == null)
                throw new ArgumentNullException("registry");
            if (pluginLoader == null)
                throw new ArgumentNullException("pluginLoader");
            if (assemblyResolverManager == null)
                throw new ArgumentNullException(@"assemblyResolverManager");
            if (runtimeSetup == null)
                throw new ArgumentNullException(@"runtimeSetup");

            this.registry = registry;
            this.pluginLoader = pluginLoader;
            this.assemblyResolverManager = assemblyResolverManager;
            this.runtimeSetup = runtimeSetup.Copy();

            pluginDirectories = new List<string>();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (registry != null)
            {
                registry.Dispose();
                registry = null;
            }
        }

        /// <inheritdoc />
        public IRegistry Registry
        {
            get
            {
                ThrowIfDisposed();
                return registry;
            }
        }

        /// <inheritdoc />
        public IServiceLocator ServiceLocator
        {
            get
            {
                ThrowIfDisposed();
                return registry.ServiceLocator;
            }
        }

        /// <inheritdoc />
        public void Initialize(ILogger logger)
        {
            if (logger == null)
                throw new ArgumentNullException("logger");

            ThrowIfDisposed();

            try
            {
                this.logger = logger;

                ConfigureForDebugging();

                SetRuntimePath();
                SetInstallationConfiguration();

                ConfigureDefaultPluginDirectories();
                ConfigurePluginDirectoriesFromSetup();
                ConfigurePluginDirectoriesFromInstallationConfiguration();
                ConfigurePluginLoaderForEnvironment();

                RegisterBuiltInComponents();
                RegisterLoadedPlugins();

                foreach (IPluginDescriptor pluginDescriptor in registry.Plugins)
                {
                    if (! pluginDescriptor.IsDisabled)
                    {
                        foreach (string probingPath in AssemblyProbingRules.GetProbingPathCombinations(
                            pluginDescriptor.BaseDirectory.FullName, pluginDescriptor.ProbingPaths))
                        {
                            assemblyResolverManager.AddHintDirectory(probingPath);
                        }
                    }
                }

                LogDisabledPlugins();
            }
            catch (Exception ex)
            {
                throw new RuntimeException(Resources.DefaultRuntime_RuntimeCannotBeInitialized, ex);
            }
        }

        private void RegisterBuiltInComponents()
        {
            IPluginDescriptor builtInPlugin = registry.RegisterPlugin(
                new PluginRegistration("BuiltIn", new TypeName(typeof(DefaultPlugin)), new DirectoryInfo(runtimeSetup.RuntimePath))
                {
                    TraitsProperties =
                    {
                        { "Name", "Gallio Built-In Components" },
                        { "AboutDescription", "Provides built-in runtime service." }
                    }
                });

            RegisterBuiltInComponent(builtInPlugin, "BuiltIn.Registry", typeof(IRegistry), registry);
            RegisterBuiltInComponent(builtInPlugin, "BuiltIn.ServiceLocator", typeof(IServiceLocator), registry.ServiceLocator);
            RegisterBuiltInComponent(builtInPlugin, "BuiltIn.Logger", typeof(ILogger), logger);
            RegisterBuiltInComponent(builtInPlugin, "BuiltIn.Runtime", typeof(IRuntime), this);
            RegisterBuiltInComponent(builtInPlugin, "BuiltIn.AssemblyResolverManager", typeof(IAssemblyResolverManager), assemblyResolverManager);
            RegisterBuiltInComponent(builtInPlugin, "BuiltIn.PluginLoader", typeof(IPluginLoader), pluginLoader);
        }

        private void RegisterBuiltInComponent(IPluginDescriptor builtInPluginDescriptor,
            string serviceId, Type serviceType, object component)
        {
            IServiceDescriptor serviceDescriptor = registry.RegisterService(
                new ServiceRegistration(builtInPluginDescriptor, serviceId, new TypeName(serviceType)));

            registry.RegisterComponent(
                new ComponentRegistration(builtInPluginDescriptor, serviceDescriptor, serviceId, new TypeName(component.GetType()))
                {
                    ComponentHandlerFactory = new InstanceHandlerFactory(component)
                });
        }

        private void RegisterLoadedPlugins()
        {
            string configurationFilePath = runtimeSetup.ConfigurationFilePath;
            if (configurationFilePath != null)
            {
                FileInfo configurationFile = new FileInfo(configurationFilePath);
                if (configurationFile.Exists)
                {
                    var document = new XmlDocument();
                    document.Load(configurationFilePath);

                    var gallioElement = document.SelectSingleNode("/configuration/gallio") as XmlElement;
                    if (gallioElement != null)
                        LoadConfigurationData(gallioElement, pluginLoader, configurationFile.Directory);
                }

                pluginLoader.AddPluginPath(configurationFilePath);
            }
            else
            {
                XmlNode sectionData = (XmlNode)ConfigurationManager.GetSection(GallioSectionHandler.SectionName);
                if (sectionData != null)
                {
                    var gallioElement = sectionData as XmlElement;
                    if (gallioElement != null)
                        LoadConfigurationData(gallioElement, pluginLoader, new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory));
                }
            }

            foreach (string path in pluginDirectories)
                pluginLoader.AddPluginPath(path);

            var pluginCatalog = new PluginCatalog();
            pluginLoader.PopulateCatalog(pluginCatalog);

            pluginCatalog.ApplyTo(registry);
        }

        private static void LoadConfigurationData(XmlElement gallioElement, IPluginLoader pluginLoader, DirectoryInfo baseDirectory)
        {
            foreach (XmlElement pluginElement in gallioElement.GetElementsByTagName("plugin", SchemaConstants.XmlNamespace))
                pluginLoader.AddPluginXml(pluginElement.OuterXml, baseDirectory);
        }

        private void LogDisabledPlugins()
        {
            foreach (IPluginDescriptor plugin in registry.Plugins)
            {
                if (plugin.IsDisabled)
                {
                    logger.Log(LogSeverity.Debug, string.Format("Disabled plugin '{0}': {1}", plugin.PluginId, plugin.DisabledReason));
                }
            }
        }

        /// <inheritdoc />
        public RuntimeSetup GetRuntimeSetup()
        {
            return runtimeSetup.Copy();
        }

        /// <inheritdoc />
        public IList<AssemblyReference> GetAllPluginAssemblyReferences()
        {
            ThrowIfDisposed();

            var result = new List<AssemblyReference>();

            foreach (IPluginDescriptor plugin in registry.Plugins)
                result.AddRange(plugin.AssemblyReferences);

            return result;
        }

        /// <inheritdoc />
        public string MapUriToLocalPath(Uri uri)
        {
            if (uri.IsFile)
            {
                return uri.LocalPath;
            }

            if (uri.Scheme == @"plugin")
            {
                string pluginId = uri.Host;
                string relativePath = uri.PathAndQuery;
                if (pluginId.Length == 0)
                    throw new InvalidOperationException(String.Format("Malformed plugin relative Uri: '{0}'.", uri));

                IPluginDescriptor plugin = GenericCollectionUtils.Find(registry.Plugins,
                    p => string.Compare(pluginId, p.PluginId, true) == 0);
                if (plugin == null)
                    throw new InvalidOperationException(String.Format("Unrecognized plugin id in Uri: '{0}'.", uri));

                string pluginPath = plugin.BaseDirectory.FullName;
                if (relativePath.Length == 0 || relativePath == @"/")
                    return pluginPath;

                return Path.Combine(pluginPath, relativePath.Substring(1));
            }

            throw new InvalidOperationException(String.Format("Unsupported Uri scheme in Uri: '{0}'.", uri));
        }

        /// <inheritdoc />
        public bool VerifyInstallation()
        {
            ThrowIfDisposed();

            logger.Log(LogSeverity.Info, "Checking plugins.");

            bool success = true;

            VerifyPlugins(ref success);
            VerifyServices(ref success);
            VerifyComponents(ref success);

            return success;
        }

        private void VerifyPlugins(ref bool success)
        {
            foreach (var plugin in registry.Plugins)
            {
                if (plugin.IsDisabled)
                {
                    logger.Log(LogSeverity.Warning, string.Format("Plugin '{0}' is disabled: {1}'",
                        plugin.PluginId, plugin.DisabledReason));
                    continue;
                }

                foreach (AssemblyReference assemblyReference in plugin.AssemblyReferences)
                {
                    try
                    {
                        if (assemblyReference.CodeBase != null && assemblyReference.CodeBase.IsFile)
                        {
                            var assemblyName = AssemblyName.GetAssemblyName(assemblyReference.CodeBase.LocalPath);
                            if (assemblyName.FullName != assemblyReference.AssemblyName.FullName)
                            {
                                success = false;
                                logger.Log(LogSeverity.Error, string.Format(
                                    "Plugin '{0}' has an incorrect assembly reference.  Accoding to the plugin metadata we expected assembly name '{1}' but it was actually '{2}' when loaded.",
                                    plugin.PluginId, assemblyReference.AssemblyName.FullName, assemblyName.FullName));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        success = false;
                        logger.Log(LogSeverity.Error, string.Format("Plugin '{0}' has an assembly reference for that could not be loaded with code base '{1}'.",
                            plugin.PluginId, assemblyReference.CodeBase), ex);
                    }
                }

                try
                {
                    plugin.ResolvePlugin();
                }
                catch (Exception ex)
                {
                    success = false;
                    logger.Log(LogSeverity.Error, "Unresolvable plugin.", ex);
                }

                try
                {
                    plugin.ResolveTraits();
                }
                catch (Exception ex)
                {
                    success = false;
                    logger.Log(LogSeverity.Error, "Unresolvable plugin traits.", ex);
                }
            }
        }

        private void VerifyServices(ref bool success)
        {
            foreach (var service in registry.Services)
            {
                if (service.IsDisabled)
                    continue;

                try
                {
                    service.ResolveServiceType();
                }
                catch (Exception ex)
                {
                    success = false;
                    logger.Log(LogSeverity.Error, "Unresolvable service type.", ex);
                }

                try
                {
                    service.ResolveTraitsType();
                }
                catch (Exception ex)
                {
                    success = false;
                    logger.Log(LogSeverity.Error, "Unresolvable service traits type.", ex);
                }
            }
        }

        private void VerifyComponents(ref bool success)
        {
            foreach (var component in registry.Components)
            {
                if (component.IsDisabled)
                    continue;

                try
                {
                    component.ResolveComponent();
                }
                catch (Exception ex)
                {
                    success = false;
                    logger.Log(LogSeverity.Error, "Unresolvable component.", ex);
                }

                try
                {
                    component.ResolveTraits();
                }
                catch (Exception ex)
                {
                    success = false;
                    logger.Log(LogSeverity.Error, "Unresolvable component traits.", ex);
                }
            }
        }

        private void ThrowIfDisposed()
        {
            if (registry == null)
                throw new ObjectDisposedException("The runtime has been disposed.");
        }

        private void AddPluginDirectory(string pluginDirectory)
        {
            if (!pluginDirectories.Contains(pluginDirectory))
                pluginDirectories.Add(pluginDirectory);
        }

        private void SetRuntimePath()
        {
            if (runtimeSetup.RuntimePath == null)
                runtimeSetup.RuntimePath = Path.GetDirectoryName(AssemblyUtils.GetFriendlyAssemblyLocation(typeof(IRuntime).Assembly));

            runtimeSetup.RuntimePath = Path.GetFullPath(runtimeSetup.RuntimePath);
        }

        private void SetInstallationConfiguration()
        {
            if (runtimeSetup.InstallationConfiguration == null)
                runtimeSetup.InstallationConfiguration = InstallationConfiguration.LoadFromRegistry();
        }

        private void ConfigureDefaultPluginDirectories()
        {
            AddPluginDirectory(runtimeSetup.RuntimePath);
        }

        private void ConfigurePluginDirectoriesFromSetup()
        {
            foreach (string pluginDirectory in runtimeSetup.PluginDirectories)
                AddPluginDirectory(pluginDirectory);
        }

        private void ConfigurePluginDirectoriesFromInstallationConfiguration()
        {
            if (runtimeSetup.InstallationConfiguration != null)
            {
                foreach (string pluginDirectory in runtimeSetup.InstallationConfiguration.AdditionalPluginDirectories)
                    AddPluginDirectory(pluginDirectory);
            }
        }

        private void ConfigurePluginLoaderForEnvironment()
        {
            switch (DotNetFrameworkSupport.FrameworkVersion)
            {
                case DotNetFrameworkVersion.DotNet40:
                    pluginLoader.DefinePreprocessorConstant("NET40");
                    break;
                case DotNetFrameworkVersion.DotNet35:
                    pluginLoader.DefinePreprocessorConstant("NET35");
                    break;
                default:
                    pluginLoader.DefinePreprocessorConstant("NET20");
                    break;
            }
        }

        /// <summary>
        /// Configure the runtime for debugging purposes within Visual Studio.
        /// This code makes assumptions about the layout of the projects on disk that
        /// help to make debugging work "magically".  Unless a specific runtime
        /// path has been set, it is overridden with the location of the Gallio project
        /// "bin" folder and the root directory of the source tree is added
        /// the list of plugin directories to ensure that plugins can be resolved.
        /// </summary>
        [Conditional("DEBUG")]
        private void ConfigureForDebugging()
        {
            // Find the root "src" dir.
            string initPath;
            if (! string.IsNullOrEmpty(runtimeSetup.RuntimePath))
                initPath = runtimeSetup.RuntimePath;
            else if (! string.IsNullOrEmpty(runtimeSetup.ConfigurationFilePath))
                initPath = runtimeSetup.ConfigurationFilePath;
            else
                initPath = AssemblyUtils.GetAssemblyLocalPath(Assembly.GetExecutingAssembly());

            string srcDir = initPath;
            while (srcDir != null && Path.GetFileName(srcDir) != @"src")
                srcDir = Path.GetDirectoryName(srcDir);

            if (srcDir == null)
                return; // not found!

            // Force the runtime path to be set to where the primary Gallio assemblies and Gallio.Host.exe
            // are located.
            runtimeSetup.RuntimePath = Path.Combine(srcDir, @"Gallio\Gallio\bin");

            // Add the solution folder to the list of plugin directories so that we can resolve
            // all plugins that have been compiled within the solution. 
            AddPluginDirectory(srcDir);
        }
    }
}
