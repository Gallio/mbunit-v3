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
    /// The base node for the XML composite tree representing an XML fragment.
    /// </summary>
    public interface INode : IDiffable<INode>
    {
        /// <summary>
        /// The child node.
        /// </summary>
        INode Child
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether the current node represents a null instance.
        /// </summary>
        bool IsNull
        {
            get;
        }

        /// <summary>
        /// Returns the XML fragment for the current node and its children.
        /// </summary>
        /// <returns>The resulting XML fragment representing the node and its children.</returns>
        string ToXml();

        /// <summary>
        /// Returns a value indicating whether the attribute or the element exists
        /// at the specified path.
        /// </summary>
        /// <param name="searchedItem">The path of the searched element or attribute.</param>
        /// <param name="expectedValue">The expected value found in the searched element or attribute, or null if the value must be ignored.</param>
        /// <param name="options">Options for the search.</param>
        /// <returns>True if the item was found; otherwise false.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="searchedItem"/> is null.</exception>
        bool Contains(XmlPathClosed searchedItem, string expectedValue, Options options);
    }
}
