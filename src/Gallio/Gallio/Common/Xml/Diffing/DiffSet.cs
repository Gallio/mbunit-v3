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
using Gallio.Framework.Assertions;

namespace Gallio.Common.Xml.Diffing
{
    /// <summary>
    /// An immutable set of diff items representing the differences between two XML fragments.
    /// </summary>
    public sealed class DiffSet : IEnumerable<Diff>
    {
        private readonly IList<Diff> diffs;

        /// <summary>
        /// A empty diff set singleton instance.
        /// </summary>
        public readonly static DiffSet Empty = new DiffSet();

        /// <summary>
        /// Gets a value indicating if the set is empty (i.e. no differences found)
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return diffs.Count == 0;
            }
        }

        /// <summary>
        /// Constructs a sets from the specified enumeration of diff items.
        /// </summary>
        /// <param name="diffs">An enumeration of diff items.</param>
        public DiffSet(IEnumerable<Diff> diffs)
        {
            if (diffs == null)
                throw new ArgumentNullException("diffs");

            this.diffs = new List<Diff>(diffs).AsReadOnly();
        }

        private DiffSet()
        {
            this.diffs = new List<Diff>().AsReadOnly();
        }

        /// <inheritdoc />
        public IEnumerator<Diff> GetEnumerator()
        {
            return diffs.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var diff in diffs)
            {
                yield return diff;
            }
        }

        /// <summary>
        /// Returns the diff items as an enumeration of assertion failures.
        /// </summary>
        /// <param name="expected">The expected fragment used to format the diff.</param>
        /// <param name="actual">The actual fragment used to format the diff.</param>
        /// <returns>The resulting enumeration of assertion failures.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="expected"/> or <paramref name="actual"/> is null.</exception>
        public IEnumerable<AssertionFailure> ToAssertionFailures(NodeFragment expected, NodeFragment actual)
        {
            foreach (Diff diff in diffs)
            {
                yield return diff.ToAssertionFailure(expected, actual);
            }
        }
    }
}
