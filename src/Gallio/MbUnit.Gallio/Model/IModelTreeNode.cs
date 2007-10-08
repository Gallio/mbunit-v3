// Copyright 2007 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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

namespace MbUnit.Model
{
    /// <summary>
    /// Interface implemented by a node in a tree of model objects.
    /// </summary>
    /// <typeparam name="T">The node type</typeparam>
    public interface IModelTreeNode<T> where T : class, IModelTreeNode<T>
    {
        /// <summary>
        /// Gets or sets the parent of this node, or null if this node is the root of the tree.
        /// </summary>
        T Parent { get; set; }

        /// <summary>
        /// Gets the list of the children of this node.
        /// </summary>
        IList<T> Children { get; }

        /// <summary>
        /// Adds a child node and sets its parent property to refer to this node.
        /// </summary>
        /// <param name="node">The node to add</param>
        void AddChild(T node);
    }
}
