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
    public class PluginDetailsTreeModel : TreeModelBase
    {
        private readonly IPluginDescriptor pluginDescriptor;

        public PluginDetailsTreeModel(IPluginDescriptor pluginDescriptor)
        {
            this.pluginDescriptor = pluginDescriptor;
        }

        public override IEnumerable GetChildren(TreePath treePath)
        {
            if (treePath.IsEmpty())
            {
                yield return new PluginDetailsNode("Assembly References", string.Empty);
                yield return new PluginDetailsNode("Base Directory", pluginDescriptor.BaseDirectory.FullName);
                yield return new PluginDetailsNode("Disabled", pluginDescriptor.IsDisabled.ToString());
                yield return new PluginDetailsNode("Plugin Dependencies", string.Empty);
                yield return
                    new PluginDetailsNode("Plugin Handler Factory", pluginDescriptor.PluginHandlerFactory.ToString())
                    ;
                yield return new PluginDetailsNode("Plugin Properties", string.Empty);
                yield return new PluginDetailsNode("Plugin Type Name", pluginDescriptor.PluginTypeName.FullName);
                yield return new PluginDetailsNode("Probing Paths", string.Empty);
                yield return new PluginDetailsNode("Resource Locator", pluginDescriptor.ResourceLocator.ToString());
                yield return new PluginDetailsNode("Traits Properties", string.Empty);
            }
            else
            {
                var node = (PluginDetailsNode) treePath.LastNode;
                if (node.Name == "Assembly References" && pluginDescriptor.AssemblyReferences != null)
                {
                    foreach (var assemblyReference in pluginDescriptor.AssemblyReferences)
                    {
                        string codeBase = assemblyReference.CodeBase != null ? 
                            assemblyReference.CodeBase.ToString() : "(unknown)";
                        yield return new PluginDetailsNode(assemblyReference.AssemblyName.ToString(), 
                            codeBase);
                    }
                }
                else if (node.Name == "Disabled" && pluginDescriptor.IsDisabled)
                {
                    yield return new PluginDetailsNode("Disabled Reason", pluginDescriptor.DisabledReason);
                }
                else
                    switch (node.Name)
                    {
                        case "Plugin Dependencies":
                            if (pluginDescriptor.PluginDependencies != null)
                                foreach (var pluginDependency in pluginDescriptor.PluginDependencies)
                                    yield return new PluginDetailsNode(pluginDependency.PluginId, string.Empty);
                            break;
                        case "Plugin Properties":
                            if (pluginDescriptor.PluginProperties != null)
                                foreach (var pluginProperty in pluginDescriptor.PluginProperties)
                                    yield return new PluginDetailsNode(pluginProperty.Key, pluginProperty.Value);
                            break;
                        case "Probing Paths":
                            if (pluginDescriptor.ProbingPaths != null)
                                foreach (var probingPath in pluginDescriptor.ProbingPaths)
                                    yield return new PluginDetailsNode(probingPath, string.Empty);
                            break;
                        case "Traits Properties":
                            if (pluginDescriptor.TraitsProperties != null)
                                foreach (var traitsProperty in pluginDescriptor.TraitsProperties)
                                    yield return new PluginDetailsNode(traitsProperty.Key, traitsProperty.Value);
                            break;
                    }
            }
        }

        public override bool IsLeaf(TreePath treePath)
        {
            return false;
        }
    }
}
