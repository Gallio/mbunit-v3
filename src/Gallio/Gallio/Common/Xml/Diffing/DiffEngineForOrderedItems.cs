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
    /// Diffing engine for collections of ordered XML items.
    /// </summary>
    /// <typeparam name="TCollection">The type of the collection.</typeparam>
    /// <typeparam name="TItem">The type of the items in the collection.</typeparam>
    internal sealed class DiffEngineForOrderedItems<TCollection, TItem> : IDiffEngine<TCollection>
        where TCollection : class, IDiffableCollection<TCollection, TItem>
        where TItem : IDiffable<TItem>, INamed
    {
        private readonly TCollection expected;
        private readonly TCollection actual;
        private readonly IXmlPathStrict path;
        private readonly Options options;
        private readonly OrderedItemType itemType;

        private static readonly IDictionary<OrderedItemType, Func<IXmlPathStrict, int, IXmlPathStrict>> extend 
            = new Dictionary<OrderedItemType, Func<IXmlPathStrict, int, IXmlPathStrict>>
        {
            { OrderedItemType.Attribute, (path, i) => path.Attribute(i)},
            { OrderedItemType.Element, (path, i) => path.Element(i)},
        };

        /// <summary>
        /// Constructs the diffing engine.
        /// </summary>
        /// <param name="expected">The expected object.</param>
        /// <param name="actual">The actual object.</param>
        /// <param name="path">The current path of the parent node.</param>
        /// <param name="options">Equality options.</param>
        /// <param name="itemType">A type of the diffed items.</param>
        public DiffEngineForOrderedItems(TCollection expected, TCollection actual, IXmlPathStrict path, Options options, OrderedItemType itemType)
        {
            if (expected == null)
                throw new ArgumentNullException("expected");
            if (actual == null)
                throw new ArgumentNullException("actual");
            if (path == null)
                throw new ArgumentNullException("path");

            this.expected = expected;
            this.actual = actual;
            this.path = path;
            this.options = options;
            this.itemType = itemType;
        }

        /// <inheritdoc />
        public DiffSet Diff()
        {
            var builder = new DiffSetBuilder();
            int i = 0;

            while (i < expected.Count)
            {
                if (i >= actual.Count)
                {
                    builder.Add(new Diff(String.Format("Missing {0}.", 
                        itemType.ToString().ToLower()), extend[itemType](path, i), DiffTargets.Expected));
                }
                else
                {
                    DiffSet diffSet = actual[i].Diff(expected[i], path, options);
                    builder.Add(diffSet);

                    if (!diffSet.IsEmpty && !actual[i].AreNamesEqual(expected[i].Name, options))
                        return builder.ToDiffSet();
                }

                i++;
            }

            return builder
                .Add(ProcessExcessAttributes(i))
                .ToDiffSet();
        }

        private DiffSet ProcessExcessAttributes(int startIndex)
        {
            var builder = new DiffSetBuilder();

            for (int i = startIndex; i < actual.Count; i++)
            {
                builder.Add(new Diff(String.Format("Unexpected {0} found.", 
                    itemType.ToString().ToLower()), extend[itemType](path, i), DiffTargets.Actual));
            }

            return builder.ToDiffSet();
        }
    }

    /// <summary>
    /// The target diffed items.
    /// </summary>
    internal enum OrderedItemType
    {
        /// <summary>
        /// Attribute.
        /// </summary>
        Attribute,

        /// <summary>
        /// Element
        /// </summary>
        Element,
    }
}
