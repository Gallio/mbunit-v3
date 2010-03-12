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
using System.Collections.ObjectModel;
using System.Collections;

namespace Gallio.Common.Xml
{
    /// <summary>
    /// An immutable collection of XML markups.
    /// </summary>
    public class MarkupCollection : IDiffableCollection<MarkupCollection, IMarkup>
    {
        private readonly IList<IMarkup> markups = new List<IMarkup>();

        /// <summary>
        /// An empty collection singleton instance.
        /// </summary>
        public readonly static MarkupCollection Empty = new MarkupCollection();

        private MarkupCollection()
        {
        }

        /// <inheritdoc />
        public int Count
        {
            get
            {
                return markups.Count;
            }
        }

        /// <inheritdoc />
        public IMarkup this[int index]
        {
            get
            {
                return markups[index];
            }
        }

        /// <summary>
        /// Constructs a collection of XML markups from the specified enumeration.
        /// </summary>
        /// <param name="markups">An enumeration of markups.</param>
        public MarkupCollection(IEnumerable<IMarkup> markups)
        {
            if (markups == null)
                throw new ArgumentNullException("markups");

            this.markups = new List<IMarkup>(markups);
        }

        /// <inheritdoc />
        public IEnumerator<IMarkup> GetEnumerator()
        {
            return markups.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var markup in markups)
            {
                yield return markup;
            }
        }

        /// <inheritdoc />
        public DiffSet Diff(MarkupCollection expected, IXmlPathStrict path, Options options)
        {
            return DiffEngineFactory.ForElements(expected, this, path, options).Diff();
        }

        /// <inheritdoc />
        public int FindIndex(Predicate<int> predicate)
        {
            for (int i = 0; i < markups.Count; i++)
            {
                if (predicate(i))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Returns the number of times the searched markup was found at the specified path.
        /// </summary>
        /// <param name="searchedPath">The path of the searched markup.</param>
        /// <param name="expectedValue">The expected value found in the searched markup, or null if the value must be ignored.</param>
        /// <param name="options">Options for the search.</param>
        /// <returns>The number of matching items found; zero if not found.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="searchedPath"/> is null.</exception>
        public int CountAt(IXmlPathLoose searchedPath, string expectedValue, Options options)
        {
            if (searchedPath == null)
                throw new ArgumentNullException("searchedPath");

            int count = 0;

            foreach(IMarkup markup in markups)
            {
                count += markup.CountAt(searchedPath, expectedValue, options);
            }

            return count;
        }
    }
}
