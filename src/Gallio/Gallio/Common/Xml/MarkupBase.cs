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

namespace Gallio.Common.Xml
{
    /// <summary>
    /// The abstract base markup for the XML composite tree representing an XML fragment.
    /// </summary>
    public abstract class MarkupBase : IMarkup
    {
        private readonly int index;
        private readonly MarkupCollection children;

        /// <inheritdoc />
        public int Index
        {
            get
            {
                return index;
            }
        }

        /// <inheritdoc />
        public MarkupCollection Children
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

        /// <summary>
        /// Constructs a markup instance.
        /// </summary>
        /// <param name="index">The index of the markup.</param>
        /// <param name="children">The children markups.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="children"/> is null.</exception>
        protected MarkupBase(int index, IEnumerable<IMarkup> children)
        {
            if (children == null)
                throw new ArgumentNullException("children");

            this.index = index;
            this.children = new MarkupCollection(children);
        }

        /// <inheritdoc />
        public abstract DiffSet Diff(IMarkup expected, IXmlPathStrict path, Options options);

        /// <inheritdoc />
        public virtual bool AreNamesEqual(string otherName, Options options)
        {
            return true;
        }

        /// <inheritdoc />
        public virtual void Aggregate(XmlPathFormatAggregator aggregator)
        {
        }

        /// <inheritdoc />
        public virtual int CountAt(IXmlPathLoose searchedPath, string expectedValue, Options options)
        {
            return 0;
        }
    }
}
