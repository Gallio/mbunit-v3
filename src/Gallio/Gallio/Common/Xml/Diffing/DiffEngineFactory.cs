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
using Gallio.Common.Xml.Paths;

namespace Gallio.Common.Xml.Diffing
{
    /// <summary>
    /// Diffing engine abstract factory.
    /// </summary>
    public static class DiffEngineFactory
    {
        /// <summary>
        /// Makes a diffing engine for collections of attributes.
        /// </summary>
        /// <param name="expected">The expected attributes.</param>
        /// <param name="actual">The actual attributes.</param>
        /// <param name="path">The current path of the parent node.</param>
        /// <param name="options">Equality options.</param>
        /// <returns>The resulting diffing engine.</returns>
        public static IDiffEngine<NodeAttributeCollection> ForAttributes(NodeAttributeCollection expected, NodeAttributeCollection actual, IXmlPathStrict path, Options options)
        {
            if ((options & Options.IgnoreAttributesOrder) != 0)
            {
                return new DiffEngineForUnorderedAttributes(expected, actual, path, options);
            }
            
            return new DiffEngineForOrderedItems<NodeAttributeCollection, NodeAttribute>(expected, actual, path, options, OrderedItemType.Attribute);
        }

        /// <summary>
        /// Makes a diffing engine for collections of elements.
        /// </summary>
        /// <param name="expected">The expected elements.</param>
        /// <param name="actual">The actual elements.</param>
        /// <param name="path">The current path of the parent node.</param>
        /// <param name="options">Equality options.</param>
        /// <returns>The resulting diffing engine.</returns>
        public static IDiffEngine<NodeCollection> ForElements(NodeCollection expected, NodeCollection actual, IXmlPathStrict path, Options options)
        {
            if ((options & Options.IgnoreElementsOrder) != 0)
            {
                return new DiffEngineForUnorderedElements(expected, actual, path, options);
            }

            return new DiffEngineForOrderedItems<NodeCollection, INode>(expected, actual, path, options, OrderedItemType.Element);
        }
    }
}
