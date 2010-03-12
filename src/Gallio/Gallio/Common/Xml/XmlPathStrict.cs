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

namespace Gallio.Common.Xml
{
    /// <summary>
    /// Abstract node that extends a strict XML path, and let it opened for further extensions.
    /// </summary>
    internal abstract class XmlPathStrict : IXmlPathStrict
    {
        private readonly IXmlPathStrict parent;
        private readonly int index;

        /// <summary>
        /// Gets the parent path node.
        /// </summary>
        protected IXmlPathStrict Parent
        {
            get
            {
                return parent;
            }
        }

        /// <inheritdoc/>
        public bool IsEmpty
        {
            get
            {
                return parent == null;
            }
        }

        /// <summary>
        /// Gets the index of the attribute.
        /// </summary>
        public int Index
        {
            get
            {
                return index;
            }
        }

        /// <summary>
        /// Constructs an empty node that represents the root path.
        /// </summary>
        protected XmlPathStrict()
        {
        }

        /// <summary>
        /// Constructs a node that extends the specified parent node with an additional level of depth.
        /// </summary>
        /// <param name="parent">The parent node to encapsulate.</param>
        /// <param name="index">The index of the inner attribute.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="parent"/> is null.</exception>
        protected XmlPathStrict(IXmlPathStrict parent, int index)
        {
            if (parent == null)
                throw new ArgumentNullException("parent");

            this.parent = parent;
            this.index = index;
        }

        /// <inheritdoc/>
        public virtual IXmlPathStrict Element(int index)
        {
            throw new InvalidOperationException();
        }

        /// <inheritdoc/>
        public virtual IXmlPathStrict Attribute(int index)
        {
            throw new InvalidOperationException();
        }

        /// <inheritdoc/>
        public virtual IXmlPathStrict Declaration()
        {
            throw new InvalidOperationException();
        }

        /// <inheritdoc/>
        public string Format(Fragment fragment)
        {
            if (fragment == null)
                throw new ArgumentNullException("fragment");

            var aggregator = new XmlPathFormatAggregator();
            Format(fragment, aggregator);
            return aggregator.GetResult();
        }

        /// <inheritdoc/>
        public virtual void Format(Fragment fragment, XmlPathFormatAggregator aggregator)
        {
            IMarkup markup = fragment.Find(this);
            markup.Aggregate(aggregator);

            if (Parent != null && !Parent.IsEmpty)
            {
                Parent.Format(fragment, aggregator);
            }
        }

        /// <inheritdoc/>
        public IXmlPathStrict[] ToArray()
        {
            var list = new List<IXmlPathStrict>();
            XmlPathStrict current = this;

            while (!current.IsEmpty)
            {
                list.Add(current);
                current = (XmlPathStrict)current.Parent;
            }

            list.Reverse();
            return list.ToArray();
        }

        /// <inheritdoc/>
        public virtual IMarkup FindInParent(IMarkup parent)
        {
            if (parent == null)
                throw new ArgumentNullException("parent");

            if (index < 0)
                return ((Fragment)parent).Declaration;

            return parent.Children[index];
        }
    }
}
