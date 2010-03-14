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

namespace Gallio.Common.Xml.Paths
{
    /// <summary>
    /// Represents an open, immutable and strict path to a node in an XML fragment.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A strict path is built based on the index of the elements that lead to it. 
    /// Thus a strict path unambiguously designates one and only one node in an XML fragment.
    /// </para>
    /// </remarks>
    public interface IXmlPathStrict : IXmlPath
    {
        /// <summary>
        /// Gets the index of the node.
        /// </summary>
        int Index
        { 
            get;
        }

        /// <summary>
        /// Extends the path to a child element.
        /// </summary>
        /// <param name="index">The index of the child element.</param>
        /// <returns>A new immutable strict path that encapsulates the current path.</returns>
        IXmlPathStrict Element(int index);

        /// <summary>
        /// Extends the path to a child attribute.
        /// </summary>
        /// <param name="index">The index of the child attribute.</param>
        /// <returns>A new immutable strict path that encapsulates the current path.</returns>
        IXmlPathStrict Attribute(int index);

        /// <summary>
        /// Extends the path to the declaration node.
        /// </summary>
        /// <returns>A new immutable strict path that encapsulates the current path.</returns>
        IXmlPathStrict Declaration();

        /// <summary>
        /// Formats the current path against the specified XML fragment.
        /// </summary>
        /// <param name="fragment">The reference XML fragment.</param>
        /// <returns>The resulting formatted path.</returns>
        string Format(NodeFragment fragment);

        /// <inheritdoc/>
        void Format(NodeFragment fragment, XmlPathFormatAggregator aggregator);

        /// <summary>
        /// Splits the current path into an array containing all the inner child nodes.
        /// </summary>
        /// <returns>An array of path nodes.</returns>
        IXmlPathStrict[] ToArray();

        /// <summary>
        /// Returns the node with the current index from the child collection of the specified parent node.
        /// </summary>
        /// <param name="parent">The parent node.</param>
        /// <returns>The node in the collection of the child node with the current index.</returns>
        INode FindInParent(INode parent);
    }
}
