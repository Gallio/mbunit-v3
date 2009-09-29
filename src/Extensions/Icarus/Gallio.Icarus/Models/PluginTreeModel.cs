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

using Aga.Controls.Tree;
using System.Collections;
using Gallio.Runtime.Extensibility;
using Gallio.Icarus.Models.PluginNodes;

namespace Gallio.Icarus.Models
{
    public class PluginTreeModel : TreeModelBase
    {
        private readonly IRegistry registry;

        public PluginTreeModel(IRegistry registry)
        {
            this.registry = registry;
        }

        public override IEnumerable GetChildren(TreePath treePath)
        {
            if (treePath.IsEmpty())
            {
                foreach (var plugin in registry.Plugins)
                    yield return new PluginNode(plugin.PluginId);
            }
            else if (treePath.LastNode is PluginNode)
            {
                var pluginNode = (PluginNode)treePath.LastNode;
                foreach (var service in registry.Services)
                    if (service.Plugin.PluginId == pluginNode.Text)
                        yield return new ServiceNode(service.ServiceId);
            }
            else if (treePath.LastNode is ServiceNode)
            {
                var serviceNode = (ServiceNode)treePath.LastNode;
                foreach (var component in registry.Components)
                    if (component.Service.ServiceId == serviceNode.Text)
                        yield return new ComponentNode(component.ComponentId);
            }
        }

        public override bool IsLeaf(TreePath treePath)
        {
            return treePath.LastNode is ComponentNode;
        }

        public IPluginDescriptor GetPluginDetails(string pluginId)
        {
            return registry.Plugins[pluginId];
        }

        public IServiceDescriptor GetServiceDetails(string serviceId)
        {
            return registry.Services[serviceId];
        }

        public IComponentDescriptor GetComponentDetails(string componentId)
        {
            return registry.Components[componentId];
        }
    }
}
