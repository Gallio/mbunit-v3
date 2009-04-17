using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using Gallio.Collections;
using Gallio.Reflection;
using Gallio.Schema.Plugins;

namespace Gallio.Runtime.Extensibility
{
    /// <summary>
    /// Simple implementation of a plugin catalog.
    /// </summary>
    public class PluginCatalog : IPluginCatalog
    {
        private readonly List<Pair<Plugin, DirectoryInfo>> plugins;

        /// <summary>
        /// Creates an empty plugin catalog.
        /// </summary>
        public PluginCatalog()
        {
            plugins = new List<Pair<Plugin, DirectoryInfo>>();
        }

        /// <inheritodc />
        public void AddPlugin(Plugin plugin, DirectoryInfo baseDirectory)
        {
            if (plugin == null)
                throw new ArgumentNullException("plugin");
            if (baseDirectory == null)
                throw new ArgumentNullException("baseDirectory");

            plugins.Add(new Pair<Plugin, DirectoryInfo>(plugin, baseDirectory));
        }

        /// <inheritodc />
        public void ApplyTo(IRegistry registry)
        {
            if (registry == null)
                throw new ArgumentNullException("registry");

            IPluginDescriptor[] pluginDescriptors = new IPluginDescriptor[plugins.Count];

            for (int i = 0; i <plugins.Count; i++)
            {
                Plugin plugin = plugins[i].First;
                DirectoryInfo baseDirectory = plugins[i].Second;

                try
                {
                    var pluginType = plugin.PluginType != null
                        ? new TypeName(plugin.PluginType)
                        : new TypeName(typeof(DefaultPlugin));

                    var pluginRegistration = new PluginRegistration(plugin.PluginId,
                        pluginType, baseDirectory)
                    {
                        PluginProperties = plugin.Parameters,
                        TraitsProperties = plugin.Traits
                    };

                    pluginDescriptors[i] = registry.RegisterPlugin(pluginRegistration);
                }
                catch (Exception ex)
                {
                    throw new RuntimeException(string.Format("Could not register plugin '{0}'.",
                        plugin.PluginId), ex);
                }
            }

            for (int i = 0; i < plugins.Count; i++)
            {
                Plugin plugin = plugins[i].First;
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

            for (int i = 0; i < plugins.Count; i++)
            {
                Plugin plugin = plugins[i].First;
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
                            serviceDescriptor, component.ComponentId, new TypeName(component.ComponentType))
                        {
                            ComponentProperties = component.Parameters,
                            TraitsProperties = component.Traits
                        };

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
    }
}
