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
    /// Represents the declaration element at the beginning of an XML document.
    /// </summary>
    public sealed class NodeDeclaration : NodeBase, IDiffable<NodeDeclaration>
    {
        private readonly NodeAttributeCollection attributes;

        /// <summary>
        /// Gets the declaration attributes.
        /// </summary>
        public NodeAttributeCollection Attributes
        {
            get
            {
                return attributes;
            }
        }

        /// <summary>
        /// Constructs the declaration element.
        /// </summary>
        /// <param name="attributes">The attributes of the element.</param>
        public NodeDeclaration(IEnumerable<NodeAttribute> attributes)
            : base(NodeType.Declaration, -1, 1, EmptyArray<INode>.Instance)
        {
            if (attributes == null)
                throw new ArgumentNullException("attributes");

            this.attributes = new NodeAttributeCollection(attributes);
        }

        /// <inheritdoc />
        public override DiffSet Diff(INode expected, IXmlPathStrict path, IXmlPathStrict pathExpected, Options options)
        {
            return Diff((NodeDeclaration)expected, path, pathExpected, options);
        }

        /// <inheritdoc />
        public DiffSet Diff(NodeDeclaration expected, IXmlPathStrict path, IXmlPathStrict pathExpected, Options options)
        {
            if (expected == null)
                throw new ArgumentNullException("expected");
            if (path == null)
                throw new ArgumentNullException("path");

            return attributes.Diff(expected.Attributes, path, pathExpected, options);
        }
    }
}
