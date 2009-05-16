using System;
using System.Collections.Generic;
using System.Text;
using Gallio.Runtime.Extensibility;

namespace Gallio.UI.ControlPanel.Plugins
{
    /// <summary>
    /// A control panel tab for managing installed plugins.
    /// </summary>
    public class PluginControlPanelTabProvider : IControlPanelTabProvider
    {
        private readonly IRegistry registry;

        /// <summary>
        /// Creates a control panel tab for managing installed plugins.
        /// </summary>
        /// <param name="registry">The plugin registry, not null</param>
        public PluginControlPanelTabProvider(IRegistry registry)
        {
            this.registry = registry;
        }

        /// <inheritdoc />
        public ControlPanelTab CreateControlPanelTab()
        {
            var tab =  new PluginControlPanelTab();

            foreach (IPluginDescriptor pluginDescriptor in registry.Plugins)
            {
                PluginTraits traits = pluginDescriptor.ResolveTraits();

                tab.AddPlugin(pluginDescriptor.PluginId, traits.Name, traits.Version,
                    traits.Icon, traits.Description,
                    pluginDescriptor.IsDisabled ? pluginDescriptor.DisabledReason : null);
            }

            return tab;
        }
    }
}
