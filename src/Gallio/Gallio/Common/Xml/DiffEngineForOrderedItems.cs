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
    /// Diffing engine for collections of ordered XML items.
    /// </summary>
    /// <typeparam name="TCollection">The type of the collection.</typeparam>
    /// <typeparam name="TItem">The type of the items in the collection.</typeparam>
    public class DiffEngineForOrderedItems<TCollection, TItem> : IDiffEngine<TCollection>
        where TCollection : IDiffableCollection<TCollection, TItem>
        where TItem : IDiffable<TItem>, INamed
    {
        private readonly TCollection expected;
        private readonly TCollection actual;
        private readonly IXmlPathOpen path;
        private readonly Options options;
        private readonly string itemName;

        /// <summary>
        /// Constructs the diffing engine.
        /// </summary>
        /// <param name="expected">The expected object.</param>
        /// <param name="actual">The actual object.</param>
        /// <param name="path">The current path of the parent node.</param>
        /// <param name="options">Equality options.</param>
        /// <param name="itemName">A friendly name for the items.</param>
        public DiffEngineForOrderedItems(TCollection expected, TCollection actual, IXmlPathOpen path, Options options, string itemName)
        {
            if (expected == null)
                throw new ArgumentNullException("expected");
            if (actual == null)
                throw new ArgumentNullException("actual");
            if (path == null)
                throw new ArgumentNullException("path");
            if (itemName == null)
                throw new ArgumentNullException("itemName");

            this.expected = expected;
            this.actual = actual;
            this.path = path;
            this.options = options;
            this.itemName = itemName;
        }

        /// <inheritdoc />
        public DiffSet Diff()
        {
            var builder = new DiffSetBuilder();
            int index = 0;

            while (index < expected.Count)
            {
                if (index >= actual.Count)
                {
                    builder.Add(new Diff(path.ToString(), String.Format("Missing {0}.", itemName), expected[index].Name, String.Empty));
                }
                else
                {
                    DiffSet diffSet = actual[index].Diff(expected[index], path, options);
                    builder.Add(diffSet);

                    if (!diffSet.IsEmpty && !actual[index].AreNamesEqual(expected[index].Name, options))
                        return builder.ToDiffSet();
                }

                index++;
            }

            return builder
                .Add(ProcessExcessAttributes(index))
                .ToDiffSet();
        }

        private DiffSet ProcessExcessAttributes(int startIndex)
        {
            var builder = new DiffSetBuilder();

            for (int i = startIndex; i < actual.Count; i++)
            {
                builder.Add(new Diff(path.ToString(), String.Format("Unexpected {0} found.", itemName), String.Empty, actual[i].Name));
            }

            return builder.ToDiffSet();
        }
    }
}
