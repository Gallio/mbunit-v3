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
using MbUnit.Framework.Kernel.Model;
using MbUnit.Framework.Kernel.Metadata;

namespace MbUnit.Framework.Kernel.Model
{
    /// <summary>
    /// The builder for a tree of model objects.  The builder holds the root of
    /// the tree and provides a registry for cooperatively resolving internal
    /// references among objects within the tree.  When internal references
    /// are present, two passes through the tree must be made to resolve
    /// them.  The tree builder supports this lifecycle directly by providing
    /// callbacks to perform fixup activities once all objects have been
    /// registered.
    /// </summary>
    public class ModelTreeBuilder<T> where T : class, IModelTreeNode<T>
    {
        private T root;
        private Dictionary<object, T> registry;

        /// <summary>
        /// Creates a tree builder initially populated with the specified root node.
        /// </summary>
        /// <param name="root">The root of the tree</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="root"/> is null</exception>
        public ModelTreeBuilder(T root)
        {
            if (root == null)
                throw new ArgumentNullException("root");

            this.root = root;

            registry = new Dictionary<object, T>();
        }

        /// <summary>
        /// Gets the root node of the tree.
        /// </summary>
        public T Root
        {
            get { return root; }
        }

        /// <summary>
        /// This event is fired once all nodes have been added to the tree to
        /// allow internal cross-references among nodes to be resolved.  New nodes
        /// should not be added to the tree at this time.
        /// </summary>
        public event EventHandler<EventArgs> ResolveReferences;

        /// <summary>
        /// This event is fired after <see cref="ResolveReferences" /> to finalize
        /// any remaining node construction issues.  New nodes should not
        /// be added to the tree at this time.
        /// </summary>
        public event EventHandler<EventArgs> PostProcess;

        /// <summary>
        /// Registers a given node with a given key (such as a type).
        /// This allows the node to be referenced elsewhere.
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="node">The node to register</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="key"/> or
        /// <paramref name="node"/> is null</exception>
        public void RegisterNode(object key, T node)
        {
            if (key == null)
                throw new ArgumentNullException("key");
            if (node == null)
                throw new ArgumentNullException("node");

            registry.Add(key, node);
        }

        /// <summary>
        /// Gets the node registered with the specified key.
        /// </summary>
        /// <param name="key">The key</param>
        /// <returns>The registered node, or null if not found</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="key"/> is null</exception>
        public T GetNode(string key)
        {
            if (key == null)
                throw new ArgumentNullException("key");

            T node;
            return registry.TryGetValue(key, out node) ? node : null;
        }

        /// <summary>
        /// Once all declarative node information has been added to the tree, this method
        /// should be called to run the <see cref="ResolveReferences" />
        /// and <see cref="PostProcess" /> event handlers.
        /// </summary>
        /// <remarks>
        /// The event handler references are cleared by this method since they are 
        /// no longer of any use.
        /// </remarks>
        public void FinishBuilding()
        {
            if (ResolveReferences != null)
            {
                ResolveReferences(this, EventArgs.Empty);
                ResolveReferences = null;
            }

            if (PostProcess != null)
            {
                PostProcess(this, EventArgs.Empty);
                PostProcess = null;
            }
        }
    }
}