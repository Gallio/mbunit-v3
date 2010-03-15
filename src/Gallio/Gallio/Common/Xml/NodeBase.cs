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
using Gallio.Common.Collections;
using Gallio.Common.Xml.Diffing;
using Gallio.Common.Xml.Paths;

namespace Gallio.Common.Xml
{
    /// <summary>
    /// The abstract base node for the XML composite tree representing an XML fragment.
    /// </summary>
    public abstract class NodeBase : INode
    {
        private readonly NodeType type;
        private readonly int index;
        private readonly int count;
        private readonly NodeCollection children;

        /// <inheritdoc />
        public NodeType Type
        {
            get
            {
                return type;
            }
        }

        /// <inheritdoc />
        public int Index
        {
            get
            {
                return index;
            }
        }

        /// <inheritdoc />
        public NodeCollection Children
        {
            get
            {
                return children;
            }
        }

        /// <inheritdoc />
        public virtual string Name
        {
            get
            {
                return String.Empty;
            }
        }

        /// <inheritdoc />
        public bool IsFirst
        {
            get
            {
                return index == 0;
            }
        }

        /// <inheritdoc />
        public bool IsLast
        {
            get
            {
                return index == count - 1;
            }
        }

        /// <summary>
        /// Constructs a node instance.
        /// </summary>
        /// <param name="type">The type of the node.</param>
        /// <param name="index">The index of the node.</param>
        /// <param name="count">The number of elements at the same level.</param>
        /// <param name="children">The children nodes.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="children"/> is null.</exception>
        protected NodeBase(NodeType type, int index, int count, IEnumerable<INode> children)
        {
            if (children == null)
                throw new ArgumentNullException("children");

            this.type = type;
            this.index = index;
            this.count = count;
            this.children = new NodeCollection(children);
        }

        /// <inheritdoc />
        public abstract DiffSet Diff(INode expected, IXmlPathStrict path, Options options);

        /// <inheritdoc />
        public virtual bool AreNamesEqual(string otherName, Options options)
        {
            return true;
        }

        /// <inheritdoc />
        public virtual int CountAt(IXmlPathLoose searchedPath, string expectedValue, Options options)
        {
            return 0;
        }
    }
}
