// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using Gallio.Runtime.Extensibility;

namespace Gallio.Copy
{
    internal class PluginTreeModel : TreeModelBase
    {
        private readonly Registry registry = new Registry();
        private readonly Node root;

        public PluginTreeModel(string pluginFolder)
        {
            var pluginLoader = new PluginLoader();
            pluginLoader.AddPluginPath(pluginFolder);

            var pluginCatalog = new PluginCatalog();
            pluginLoader.PopulateCatalog(pluginCatalog);

            pluginCatalog.ApplyTo(registry);

            root = new Node("Plugins");
        }

        public override IEnumerable GetChildren(TreePath treePath)
        {
            if (treePath.IsEmpty())
            {
                yield return root;
            }
            else if (treePath.LastNode == root)
            {
                foreach (var pluginDescriptor in registry.Plugins)
                {
                    var pluginNode = new PluginNode(pluginDescriptor.PluginId);
                    foreach (var file in pluginDescriptor.FilePaths)
                    {
                        pluginNode.Nodes.Add(new FileNode(file));
                    }
                    yield return pluginNode;
                }
            }
            else if (treePath.LastNode is PluginNode)
            {
                var pluginNode = (PluginNode) treePath.LastNode;
                foreach (var child in pluginNode.Nodes)
                {
                    yield return child;
                }
            }
        }

        public override bool IsLeaf(TreePath treePath)
        {
            if (treePath.LastNode is FileNode)
                return true;

            return treePath.LastNode is PluginNode && ((PluginNode)treePath.LastNode).Nodes.Count == 0;
        }
    }
}
