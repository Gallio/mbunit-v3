using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Gallio.Collections;
using Gallio.Reflection;
using Gallio.Schema.Plugins;

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
                            string foundCodeBasePath = ProbeForCodeBase(assembly.CodeBase, baseDirectory, plugin.ProbingPaths, attemptedPaths);
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

                        var assemblyReference = new AssemblyReference(
                            new AssemblyName(assembly.FullName), absoluteCodeBase);

                        pluginRegistration.AssemblyReferences.Add(assemblyReference);
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

                        registry.RegisterService(serviceRegistration);
                    }
                    catch (Exception ex)
                    {
                        throw new RuntimeException(string.Format("Could not register service '{0}' of plugin '{1}'.",
                            service.ServiceId, plugin.PluginId), ex);
                    }
                }
            }

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
                            serviceDescriptor, component.ComponentId, new TypeName(component.ComponentType));
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

        private static string ProbeForCodeBase(string absoluteOrRelativeCodeBase, DirectoryInfo baseDirectory,
            IList<string> probingPaths, IList<string> attemptedPaths)
        {
            if (Path.IsPathRooted(absoluteOrRelativeCodeBase))
            {
                attemptedPaths.Add(absoluteOrRelativeCodeBase);

                if (File.Exists(absoluteOrRelativeCodeBase))
                    return absoluteOrRelativeCodeBase;
            }
            else
            {
                foreach (string probingPath in AssemblyProbingRules.GetProbingPathCombinations(baseDirectory.FullName, probingPaths))
                {
                    string candidatePath = Path.Combine(probingPath, absoluteOrRelativeCodeBase);
                    attemptedPaths.Add(candidatePath);

                    if (File.Exists(candidatePath))
                        return candidatePath;
                }
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
