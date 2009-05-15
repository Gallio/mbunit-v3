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

using System.Collections;
using Aga.Controls.Tree;
using Gallio.Icarus.Models.PluginNodes;
using Gallio.Runtime.Extensibility;

namespace Gallio.Icarus.Models
{
    public class ComponentDetailsTreeModel : TreeModelBase
    {
        private readonly IComponentDescriptor componentDescriptor;

        public ComponentDetailsTreeModel(IComponentDescriptor componentDescriptor)
        {
            this.componentDescriptor = componentDescriptor;
        }

        public override IEnumerable GetChildren(TreePath treePath)
        {
            if (treePath.IsEmpty())
            {
                yield return new PluginDetailsNode("Component Handler Factory", componentDescriptor.ComponentHandlerFactory.ToString());
                yield return new PluginDetailsNode("Component Properties", string.Empty);
                yield return new PluginDetailsNode("Component Type Name", componentDescriptor.ComponentTypeName.FullName);
                yield return new PluginDetailsNode("Disabled", componentDescriptor.IsDisabled.ToString());
                yield return new PluginDetailsNode("Traits Properties", string.Empty);
            }
            else
            {
                var node = (PluginDetailsNode) treePath.LastNode;
                if (node.Name == "Component Properties")
                {
                    if (componentDescriptor.ComponentProperties != null)
                        foreach (var property in componentDescriptor.ComponentProperties)
                            yield return new PluginDetailsNode(property.Key, property.Value);
                }
                else if (node.Name == "Disabled" && componentDescriptor.IsDisabled)
                {
                    yield return new PluginDetailsNode("Disabled Reason", componentDescriptor.DisabledReason);
                }
                else if (node.Name == "Traits Properties")
                {
                    if (componentDescriptor.TraitsProperties != null)
                        foreach (var property in componentDescriptor.TraitsProperties)
                            yield return new PluginDetailsNode(property.Key, property.Value);
                }
            }
        }

        public override bool IsLeaf(TreePath treePath)
        {
            return !treePath.IsEmpty();
        }
    }
}
