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
using System.Collections.Generic;
using System.IO;
using Aga.Controls.Tree;
using Gallio.Common.IO;
using Gallio.Runtime.Extensibility;

namespace Gallio.Copy.Model
{
    public class PluginTreeModel : TreeModelBase
    {
        private readonly IFileSystem fileSystem;
        private readonly Node root = new RootNode();

        public List<IPluginDescriptor> PluginDescriptors { get; private set; }

        public PluginTreeModel(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
            PluginDescriptors = new List<IPluginDescriptor>();
        }

        public void UpdatePluginList(IRegistry registry)
        {
            PluginDescriptors = new List<IPluginDescriptor>(registry.Plugins);
            PluginDescriptors.Sort((l, r) => l.PluginId.CompareTo(r.PluginId));

            OnStructureChanged(new TreePathEventArgs(new TreePath(root)));
        }

        public override IEnumerable GetChildren(TreePath treePath)
        {
            if (treePath.IsEmpty())
            {
                yield return root;
            }
            else if (treePath.LastNode == root)
            {                
                foreach (var pluginDescriptor in PluginDescriptors)
                {
                    var pluginNode = new PluginNode(pluginDescriptor);
                    root.Nodes.Add(pluginNode);
                    foreach (var file in pluginDescriptor.FilePaths)
                    {
                        var fullPath = Path.Combine(pluginDescriptor.BaseDirectory.FullName, file);
                        var exists = fileSystem.FileExists(fullPath);
                        pluginNode.Nodes.Add(new FileNode(file, exists));
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

            return treePath.LastNode is PluginNode && 
                ((PluginNode)treePath.LastNode).Nodes.Count == 0;
        }

        public IList<IPluginDescriptor> GetSelectedPlugins()
        {
            var pluginDescriptors = new List<IPluginDescriptor>();

            foreach (PluginNode pluginNode in root.Nodes)
            {
                if (pluginNode.IsChecked)
                    pluginDescriptors.Add(pluginNode.Plugin);
            }

            return pluginDescriptors;
        }
    }
}
