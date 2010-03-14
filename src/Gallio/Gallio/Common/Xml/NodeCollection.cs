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

using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections;
using Gallio.Common.Collections;
using Gallio.Common.Xml.Diffing;
using Gallio.Common.Xml.Paths;

namespace Gallio.Common.Xml
{
    /// <summary>
    /// Immutable collection of XML nodes.
    /// </summary>
    public abstract class NodeCollection<TNode> : IEnumerable<TNode>
        where TNode : INode
    {
        private readonly List<TNode> nodes;

        /// <inheritdoc />
        public int Count
        {
            get
            {
                return nodes.Count;
            }
        }

        /// <inheritdoc />
        public TNode this[int index]
        {
            get
            {
                return nodes[index];
            }
        }

        /// <summary>
        /// Constructs a collection of XML nodes from the specified enumeration.
        /// </summary>
        /// <param name="nodes">An enumeration of nodes.</param>
        protected NodeCollection(IEnumerable<TNode> nodes)
        {
            if (nodes == null)
                throw new ArgumentNullException("nodes");

            this.nodes = new List<TNode>(nodes);
        }

        /// <inheritdoc />
        public IEnumerator<TNode> GetEnumerator()
        {
            return nodes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var node in nodes)
            {
                yield return node;
            }
        }

       /// <inheritdoc />
        public int FindIndex(Predicate<int> predicate)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                if (predicate(i))
                {
                    return i;
                }
            }

            return -1;
        }


        /// <summary>
        /// Determines whether the collection contains elements that match the conditions defined by the specified predicate. 
        /// </summary>
        /// <param name="match">The predicate.</param>
        /// <returns>True if the collection contains at least one element that match the conditions; otherwise, false.</returns>
        protected bool Exists(Predicate<TNode> match)
        {
            return nodes.Exists(match);
        }

        /// <summary>
        /// Returns the number of times the searched node was found at the specified path.
        /// </summary>
        /// <param name="searchedPath">The path of the searched node.</param>
        /// <param name="expectedValue">The expected value found in the searched node, or null if the value must be ignored.</param>
        /// <param name="options">Options for the search.</param>
        /// <returns>The number of matching items found; zero if not found.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="searchedPath"/> is null.</exception>
        public int CountAt(IXmlPathLoose searchedPath, string expectedValue, Options options)
        {
            if (searchedPath == null)
                throw new ArgumentNullException("searchedPath");

            int count = 0;

            foreach(INode node in nodes)
            {
                count += node.CountAt(searchedPath, expectedValue, options);
            }

            return count;
        }
    }

    /// <summary>
    /// Immutable collection of XML nodes.
    /// </summary>
    public class NodeCollection : NodeCollection<INode>, IDiffableCollection<NodeCollection, INode>
    {
        /// <summary>
        /// An empty collection singleton instance.
        /// </summary>
        public readonly static NodeCollection Empty = new NodeCollection(EmptyArray<INode>.Instance);

        /// <summary>
        /// Constructs a collection of XML nodes from the specified enumeration.
        /// </summary>
        /// <param name="nodes">An enumeration of nodes.</param>
        public NodeCollection(IEnumerable<INode> nodes)
            : base(nodes)
        {
        }

        /// <inheritdoc />
        public DiffSet Diff(NodeCollection expected, IXmlPathStrict path, Options options)
        {
            return DiffEngineFactory.ForElements(expected, this, path, options).Diff();
        }
    }
}
