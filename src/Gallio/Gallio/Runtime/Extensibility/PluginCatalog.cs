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
using System.IO;
using System.Reflection;
using System.Text;
using Gallio.Common.Collections;
using Gallio.Common.Reflection;
using Gallio.Runtime.Extensibility.Schema;

namespace Gallio.Runtime.Extensibility
{
    /// <summary>
    /// A plugin catalog implementation that verifies plugin dependencies and assembly references.
    /// </summary>
    public class PluginCatalog : IPluginCatalog
    {
        private readonly List<PluginData> plugins;

        /// <summary>
        /// Creates an empty plugin catalog.
        /// </summary>
        public PluginCatalog()
        {
            plugins = new List<PluginData>();
        }

        /// <inheritodc />
        public void AddPlugin(Plugin plugin, DirectoryInfo baseDirectory)
        {
            if (plugin == null)
                throw new ArgumentNullException("plugin");
            if (baseDirectory == null)
                throw new ArgumentNullException("baseDirectory");

            plugins.Add(new PluginData(plugin, baseDirectory));
        }

        /// <inheritodc />
        public void ApplyTo(IRegistry registry)
        {
            if (registry == null)
                throw new ArgumentNullException("registry");

            var topologicallySortedPlugins = TopologicalSortByDependencies(plugins);

            IList<IPluginDescriptor> pluginDescriptors = RegisterPlugins(registry,
                topologicallySortedPlugins);
            RegisterServices(registry, topologicallySortedPlugins, pluginDescriptors);
            RegisterComponents(registry, topologicallySortedPlugins, pluginDescriptors);
        }

        private static IList<IPluginDescriptor> RegisterPlugins(IRegistry registry, IList<PluginData> topologicallySortedPlugins)
        {
            IPluginDescriptor[] pluginDescriptors = new IPluginDescriptor[topologicallySortedPlugins.Count];
            for (int i = 0; i < topologicallySortedPlugins.Count; i++)
            {
                Plugin plugin = topologicallySortedPlugins[i].Plugin;
                DirectoryInfo baseDirectory = topologicallySortedPlugins[i].BaseDirectory;

                try
                {
                    var pluginType = plugin.PluginType != null
                        ? new TypeName(plugin.PluginType)
                        : new TypeName(typeof(DefaultPlugin));

                    List<string> disabledReasons = new List<string>();

                    var pluginRegistration = new PluginRegistration(plugin.PluginId,
                        pluginType, baseDirectory);
                    if (plugin.Parameters != null)
                        pluginRegistration.PluginProperties = plugin.Parameters.PropertySet;
                    if (plugin.Traits != null)
                        pluginRegistration.TraitsProperties = plugin.Traits.PropertySet;

                    pluginRegistration.ProbingPaths = plugin.ProbingPaths;
                    pluginRegistration.RecommendedInstallationPath = plugin.RecommendedInstallationPath;

                    foreach (var file in plugin.Files)
                        pluginRegistration.FilePaths.Add(file.Path);

                    foreach (var dependency in plugin.Dependencies)
                    {
                        string pluginDependencyId = dependency.PluginId;

                        IPluginDescriptor pluginDependency = registry.Plugins[pluginDependencyId];
                        if (pluginDependency == null)
                        {
                            disabledReasons.Add(string.Format("Could not find plugin '{0}' upon which this plugin depends.", pluginDependencyId));
                        }
                        else
                        {
                            pluginRegistration.PluginDependencies.Add(pluginDependency);
                        }
                    }

                    foreach (var assembly in plugin.Assemblies)
                    {
                        Uri absoluteCodeBase;
                        if (assembly.CodeBase != null)
                        {
                            List<string> attemptedPaths = new List<string>();
                            string foundCodeBasePath = ProbeForCodeBase(baseDirectory, plugin.ProbingPaths, assembly.CodeBase, attemptedPaths);
                            if (foundCodeBasePath == null)
                            {
                                StringBuilder formattedPaths = new StringBuilder();
                                foreach (string path in attemptedPaths)
                                {
                                    if (formattedPaths.Length != 0)
                                        formattedPaths.Append(", ");
                                    formattedPaths.Append("'").Append(path).Append("'");
                                }

                                disabledReasons.Add(string.Format("Could not find assembly '{0}' after probing for its code base in {1}.",
                                    assembly.FullName, formattedPaths));
                                absoluteCodeBase = null;
                            }
                            else
                            {
                                absoluteCodeBase = new Uri(foundCodeBasePath);
                            }
                        }
                        else
                        {
                            if (!IsAssemblyRegisteredInGAC(assembly.FullName))
                            {
                                disabledReasons.Add(
                                    string.Format("Could not find assembly '{0}' in the global assembly cache.",
                                        assembly.FullName));
                            }

                            absoluteCodeBase = null;
                        }

                        var assemblyBinding = new AssemblyBinding(new AssemblyName(assembly.FullName))
                        {
                            CodeBase = absoluteCodeBase,
                            QualifyPartialName = assembly.QualifyPartialName,
                            ApplyPublisherPolicy = assembly.ApplyPublisherPolicy
                        };

                        foreach (BindingRedirect redirect in assembly.BindingRedirects)
                            assemblyBinding.AddBindingRedirect(new AssemblyBinding.BindingRedirect(redirect.OldVersion));

                        pluginRegistration.AssemblyBindings.Add(assemblyBinding);
                    }

                    IPluginDescriptor pluginDescriptor = registry.RegisterPlugin(pluginRegistration);
                    pluginDescriptors[i] = pluginDescriptor;

                    if (disabledReasons.Count != 0)
                        pluginDescriptor.Disable(disabledReasons[0]);
                }
                catch (Exception ex)
                {
                    throw new RuntimeException(string.Format("Could not register plugin '{0}'.",
                        plugin.PluginId), ex);
                }
            }
            return pluginDescriptors;
        }

        private static void RegisterServices(IRegistry registry, IList<PluginData> topologicallySortedPlugins, IList<IPluginDescriptor> pluginDescriptors)
        {
            for (int i = 0; i < topologicallySortedPlugins.Count; i++)
            {
                Plugin plugin = topologicallySortedPlugins[i].Plugin;
                IPluginDescriptor pluginDescriptor = pluginDescriptors[i];

                foreach (Service service in plugin.Services)
                {
                    try
                    {
                        var serviceRegistration = new ServiceRegistration(pluginDescriptor,
                            service.ServiceId, new TypeName(service.ServiceType));
                        if (service.DefaultComponentType != null)
                            serviceRegistration.DefaultComponentTypeName = new TypeName(service.DefaultComponentType);

                        registry.RegisterService(serviceRegistration);
                    }
                    catch (Exception ex)
                    {
                        throw new RuntimeException(string.Format("Could not register service '{0}' of plugin '{1}'.",
                            service.ServiceId, plugin.PluginId), ex);
                    }
                }
            }
        }

        private static void RegisterComponents(IRegistry registry, IList<PluginData> topologicallySortedPlugins, IList<IPluginDescriptor> pluginDescriptors)
        {
            for (int i = 0; i < topologicallySortedPlugins.Count; i++)
            {
                Plugin plugin = topologicallySortedPlugins[i].Plugin;
                IPluginDescriptor pluginDescriptor = pluginDescriptors[i];

                foreach (Component component in plugin.Components)
                {
                    var serviceDescriptor = registry.Services[component.ServiceId];
                    if (serviceDescriptor == null)
                        throw new RuntimeException(string.Format("Could not register component '{0}' of plugin '{1}' because it implements service '{2}' which was not found in the registry.",
                            component.ComponentId, plugin.PluginId, component.ServiceId));

                    try
                    {
                        var componentRegistration = new ComponentRegistration(pluginDescriptor,
                            serviceDescriptor, component.ComponentId,
                            component.ComponentType != null ? new TypeName(component.ComponentType) : null);
                        if (component.Parameters != null)
                            componentRegistration.ComponentProperties = component.Parameters.PropertySet;
                        if (component.Traits != null)
                            componentRegistration.TraitsProperties = component.Traits.PropertySet;

                        registry.RegisterComponent(componentRegistration);
                    }
                    catch (Exception ex)
                    {
                        throw new RuntimeException(string.Format("Could not register component '{0}' of plugin '{1}'.",
                            component.ComponentId, plugin.PluginId), ex);
                    }
                }
            }
        }

        private static string ProbeForCodeBase(DirectoryInfo baseDirectory, IList<string> probingPaths, string codeBase, ICollection<string> attemptedPaths)
        {
            foreach (string searchPath in ResourceSearchRules.GetSearchPaths(baseDirectory, probingPaths, codeBase))
            {
                attemptedPaths.Add(searchPath);

                if (System.IO.File.Exists(searchPath))
                    return searchPath;
            }

            return null;
        }

        private static bool IsAssemblyRegisteredInGAC(string assemblyName)
        {
            try
            {
                System.Reflection.Assembly.ReflectionOnlyLoad(assemblyName);
                return true;
            }
            catch (FileNotFoundException)
            {
                return false;
            }
        }

        private static IList<PluginData> TopologicalSortByDependencies(IList<PluginData> plugins)
        {
            Dictionary<string, PluginData> pluginsById = new Dictionary<string, PluginData>();
            Dictionary<PluginData, int> outgoingDependencyCounts = new Dictionary<PluginData, int>();
            MultiMap<PluginData, PluginData> incomingPluginDependencies = new MultiMap<PluginData, PluginData>();
            Queue<PluginData> isolatedPlugins = new Queue<PluginData>();

            foreach (PluginData plugin in plugins)
            {
                pluginsById[plugin.Plugin.PluginId] = plugin;
                outgoingDependencyCounts[plugin] = 0;
            }

            foreach (PluginData plugin in plugins)
            {
                foreach (Dependency dependency in plugin.Plugin.Dependencies)
                {
                    PluginData pluginDependency;
                    if (pluginsById.TryGetValue(dependency.PluginId, out pluginDependency))
                    {
                        incomingPluginDependencies.Add(pluginDependency, plugin);
                        outgoingDependencyCounts[plugin] += 1;
                    }
                }
            }

            foreach (var pair in outgoingDependencyCounts)
            {
                if (pair.Value == 0)
                    isolatedPlugins.Enqueue(pair.Key);
            }

            List<PluginData> result = new List<PluginData>(plugins.Count);

            while (isolatedPlugins.Count != 0)
            {
                PluginData plugin = isolatedPlugins.Dequeue();

                result.Add(plugin);

                foreach (PluginData incomingPluginDependency in incomingPluginDependencies[plugin])
                {
                    int newCount = outgoingDependencyCounts[incomingPluginDependency] -= 1;
                    if (newCount == 0)
                        isolatedPlugins.Enqueue(incomingPluginDependency);
                }
            }

            if (result.Count != plugins.Count)
            {
                StringBuilder message = new StringBuilder();
                message.Append("Could not topologically sort the following plugins either due to dependency cycles or duplicate dependencies: ");

                bool first = true;
                foreach (var pair in outgoingDependencyCounts)
                {
                    if (pair.Value != 0)
                    {
                        if (first)
                            first = false;
                        else
                            message.Append(", ");

                        message.Append("'").Append(pair.Key.Plugin.PluginId).Append("'");
                    }
                }

                message.Append(".");
                throw new RuntimeException(message.ToString());
            }

            return result;
        }

        private sealed class PluginData
        {
            public readonly Plugin Plugin;
            public readonly DirectoryInfo BaseDirectory;

            public PluginData(Plugin plugin, DirectoryInfo baseDirectory)
            {
                Plugin = plugin;
                BaseDirectory = baseDirectory;
            }
        }
    }
}
