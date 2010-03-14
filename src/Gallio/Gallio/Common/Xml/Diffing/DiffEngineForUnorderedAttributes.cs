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
    /// Diffing engine for collections of unordered attributes.
    /// </summary>
    internal sealed class DiffEngineForUnorderedAttributes : IDiffEngine<NodeAttributeCollection>
    {
        private readonly NodeAttributeCollection expected;
        private readonly NodeAttributeCollection actual;
        private readonly IXmlPathStrict path;
        private readonly Options options;

        /// <summary>
        /// Constructs the diffing engine.
        /// </summary>
        /// <param name="expected">The expected object.</param>
        /// <param name="actual">The actual object.</param>
        /// <param name="path">The current path of the parent node.</param>
        /// <param name="options">Equality options.</param>
        public DiffEngineForUnorderedAttributes(NodeAttributeCollection expected, NodeAttributeCollection actual, IXmlPathStrict path, Options options)
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
                .Add(FindAttributes(notified, true, "Missing attribute."))
                .Add(FindAttributes(notified, false, "Unexpected attribute found."))
                .ToDiffSet();
        }

        private DiffSet FindAttributes(IList<int> notified, bool invert, string message)
        {
            var builder = new DiffSetBuilder();
            var mask = new List<int>();
            var source = invert ? expected : actual;
            var pool = invert ? actual : expected;

            for (int i = 0; i < source.Count; i++)
            {
                int j = Find(pool, source[i], mask);

                if (j < 0)
                {
                    var targets = invert ? DiffTargets.Expected : DiffTargets.Actual;
                    builder.Add(new Diff(message, path.Attribute(i), targets));
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

        private int Find(NodeAttributeCollection source, NodeAttribute attribute, IList<int> mask)
        {
            int index = source.FindIndex(i => !mask.Contains(i) && attribute.Diff(source[i], XmlPathStrictElement.Root.Element(0), options).IsEmpty);

            if (index < 0)
            {
                index = source.FindIndex(i => !mask.Contains(i) && attribute.AreNamesEqual(source[i].Name, options));
            }

            return index;
        }
    }
}
