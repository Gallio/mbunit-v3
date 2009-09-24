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
using System.Collections.ObjectModel;
using System.Text;
using System.Collections;

namespace Gallio.Common.Xml
{
    /// <summary>
    /// A collection of nodes in an XML fragment.
    /// </summary>
    public class ElementCollection : Node, IDiffableCollection<ElementCollection, Element>
    {
        private readonly List<Element> elements;

        /// <summary>
        /// An empty collection singleton instance.
        /// </summary>
        public readonly static ElementCollection Empty = new ElementCollection();

        private ElementCollection()
            : base(Null.Instance)
        {
            this.elements = new List<Element>();
        }

        /// <inheritdoc />
        public int Count
        {
            get
            {
                return elements.Count;
            }
        }

        /// <inheritdoc />
        public Element this[int index]
        {
            get
            {
                return elements[index];
            }
        }

        /// <summary>
        /// Constructs a collection from the specified enumeration of elements.
        /// </summary>
        /// <param name="elements"></param>
        public ElementCollection(IEnumerable<Element> elements)
            : base(Null.Instance)
        {
            if (elements == null)
                throw new ArgumentNullException("elements");

            this.elements = new List<Element>(elements);
        }

        /// <inheritdoc />
        public override string ToXml()
        {
            var builder = new StringBuilder();
            
            foreach (var element in elements)
            {
                builder.Append(element.ToXml());
            }

            return builder.ToString();
        }

        /// <inheritdoc />
        public IEnumerator<Element> GetEnumerator()
        {
            return elements.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var element in elements)
            {
                yield return element;
            }
        }

        /// <inheritdoc />
        public override DiffSet Diff(INode expected, IXmlPathOpen path, Options options)
        {
            return Diff(expected.IsNull ? ElementCollection.Empty : (ElementCollection)expected, path, options);
        }

        /// <inheritdoc />
        public DiffSet Diff(ElementCollection expected, IXmlPathOpen path, Options options)
        {
            return DiffEngineFactory.ForElements(expected, this, path, options).Diff();
        }

        /// <inheritdoc />
        public int FindIndex(Predicate<int> predicate)
        {
            for (int i = 0; i < elements.Count; i++)
            {
                if (predicate(i))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <inheritdoc />
        public override bool Contains(XmlPathClosed searchedItem, Options options)
        {
            return elements.Exists(element => element.Contains(searchedItem, options));
        }
    }
}
