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

using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Common.Xml
{
    /// <summary>
    /// The abstract base node for the XML composite tree representing an XML fragment.
    /// </summary>
    public abstract class Node : INode
    {
        private readonly INode child;

        /// <inheritdoc />
        public INode Child
        {
            get
            {
                return child;
            }
        }

        /// <inheritdoc />
        public bool IsNull
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Constructs a node instance.
        /// </summary>
        /// <param name="child">The child node.</param>
        protected Node(INode child)
        {
            if (child == null)
                throw new ArgumentNullException("child");

           this.child = child;
        }

        /// <inheritdoc />
        public abstract string ToXml();

        /// <inheritdoc />
        public abstract DiffSet Diff(INode expected, IXmlPathOpen path, Options options);

        /// <inheritdoc />
        public abstract bool Contains(XmlPathClosed searchedItem, int depth);
    }
}
