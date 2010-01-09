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

namespace Gallio.Icarus.Models
{
    public abstract class TreeModelDecorator : TreeModelBase
    {
        protected readonly ITreeModel innerTreeModel;

        protected TreeModelDecorator(ITreeModel innerTreeModel)
        {
            this.innerTreeModel = innerTreeModel;

            innerTreeModel.NodesChanged += (sender, e) => OnNodesChanged(e);
            innerTreeModel.NodesInserted += (sender, e) => OnNodesInserted(e);
            innerTreeModel.NodesRemoved += (sender, e) => OnNodesRemoved(e);
            innerTreeModel.StructureChanged += (sender, e) => OnStructureChanged(e);
        }

        public override IEnumerable GetChildren(TreePath treePath)
        {
            return innerTreeModel.GetChildren(treePath);
        }

        public override bool IsLeaf(TreePath treePath)
        {
            return innerTreeModel.IsLeaf(treePath);
        }

        protected Node GetRoot()
        {
            var children = innerTreeModel.GetChildren(new TreePath());
            foreach (var child in children)
            {
                return (Node)child;
            }
            return null;
        }
    }
}
