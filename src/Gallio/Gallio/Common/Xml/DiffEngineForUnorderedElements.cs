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
    /// Diffing engine for collections of unordered elements.
    /// </summary>
    public class DiffEngineForUnorderedElements : IDiffEngine<ElementCollection>
    {
        private readonly ElementCollection expected;
        private readonly ElementCollection actual;
        private readonly Path path;
        private readonly Options options;

        /// <summary>
        /// Constructs the diffing engine.
        /// </summary>
        /// <param name="expected">The expected object.</param>
        /// <param name="actual">The actual object.</param>
        /// <param name="path">The current path of the parent node.</param>
        /// <param name="options">Equality options.</param>
        public DiffEngineForUnorderedElements(ElementCollection expected, ElementCollection actual, Path path, Options options)
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
        }

        /// <inheritdoc />
        public DiffSet Diff()
        {
            var notified = new List<int>();
            return new DiffSetBuilder()
                .Add(FindElements(notified, true, "Missing element."))
                .Add(FindElements(notified, false, "Unexpected element found."))
                .ToDiffSet();
        }

        private DiffSet FindElements(IList<int> notified, bool invert, string message)
        {
            var builder = new DiffSetBuilder();
            var mask = new List<int>();
            var noExactMatch = new List<int>();
            var source = invert ? expected : actual;
            var pool = invert ? actual : expected;

            // Find first exact match (= empty diff)
            for (int i = 0; i < source.Count; i++)
            {
                int j = pool.FindIndex(x => !mask.Contains(x) && source[i].Diff(pool[x], Path.Empty, options).IsEmpty);

                if (j < 0)
                {
                    noExactMatch.Add(i);
                }
                else
                {
                    mask.Add(j);
                }
            }

            // Find first name-only match for the remaining items without exact match.
            foreach (int i in noExactMatch)
            {
                int j = pool.FindIndex(x => !mask.Contains(x) && source[i].AreNamesEqual(pool[x].Name, options));

                if (j < 0)
                {
                    builder.Add(new Diff(path.ToString(), message, 
                        invert ? expected[i].Name : String.Empty,
                        invert ? String.Empty : actual[i].Name));
                }
                else
                {
                    int k = invert ? j : i;
                    DiffSet diffSet = actual[k].Diff(expected[invert ? i : j], path, options);

                    if (!diffSet.IsEmpty && !notified.Contains(k))
                    {
                        builder.Add(diffSet);
                        notified.Add(k);
                    }

                    mask.Add(j);
                }
            }

            return builder.ToDiffSet();
        }
    }
}
