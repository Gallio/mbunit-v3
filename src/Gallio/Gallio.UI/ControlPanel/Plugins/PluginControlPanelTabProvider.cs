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
        /// <param name="registry">The plugin registry, not null.</param>
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
