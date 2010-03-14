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
    /// Abstract node that extends and closes a loose XML path.
    /// </summary>
    internal abstract class XmlPathLooseClosed : IXmlPathLooseClosed
    {
        private readonly IXmlPathLooseClosed parent;
        private IXmlPathLoose[] array;

        /// <summary>
        /// Gets the parent node of the path.
        /// </summary>
        protected IXmlPathLooseClosed Parent
        {
            get
            {
                return parent;
            }
        }

        /// <summary>
        /// Determines whether the current node represents the root path.
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return parent == null;
            }
        }

        /// <summary>
        /// Constructs an empty node that represents the root path.
        /// </summary>
        protected XmlPathLooseClosed()
        {
        }

        /// <summary>
        /// Constructs a node that extends the specified parent node with an additional level of depth.
        /// </summary>
        /// <param name="parent">The parent node to encapsulate.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="parent"/> is null.</exception>
        protected XmlPathLooseClosed(IXmlPathLooseClosed parent)
        {
            if (parent == null)
                throw new ArgumentNullException("parent");
            
            this.parent = parent;
        }

        /// <inheritdoc/>
        public abstract string ToString(NodeFragment fragment);

        /// <inheritdoc/>
        public abstract IXmlPathLoose Copy(IXmlPathLoose child);

        /// <inheritdoc/>
        public IXmlPathLoose Trail()
        {
            IXmlPathLoose[] all = AsArray();
            IXmlPathLoose path = XmlPathLooseOpenElement.Root;

            for (int i = 1; i < all.Length; i++)
            {
                path = all[i].Copy(path);
            }

            return path;
        }

        /// <inheritdoc/>
        public IXmlPathLoose[] AsArray()
        {
            if (array == null)
            {
                var list = new List<XmlPathLooseClosed>();
                XmlPathLooseClosed current = this;

                while (!current.IsEmpty)
                {
                    list.Add(current);
                    current = (XmlPathLooseClosed) current.Parent;
                }

                list.Reverse();
                array = list.ToArray();
            }

            return array;
        }
    }
}
