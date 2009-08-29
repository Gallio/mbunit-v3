using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Common.Xml
{
    /// <summary>
    /// Diffing engine for collections of unordered attributes.
    /// </summary>
    public class DiffEngineForUnorderedAttributes : IDiffEngine<AttributeCollection>
    {
        private readonly AttributeCollection expected;
        private readonly AttributeCollection actual;
        private readonly Path path;
        private readonly Options options;

        /// <summary>
        /// Constructs the diffing engine.
        /// </summary>
        /// <param name="expected">The expected object.</param>
        /// <param name="actual">The actual object.</param>
        /// <param name="path">The current path of the parent node.</param>
        /// <param name="options">Equality options.</param>
        public DiffEngineForUnorderedAttributes(AttributeCollection expected, AttributeCollection actual, Path path, Options options)
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

        private int Find(AttributeCollection source, Attribute attribute, IList<int> mask)
        {
            int index = source.FindIndex(i => !mask.Contains(i) && attribute.Diff(source[i], Path.Empty, options).IsEmpty);

            if (index < 0)
            {
                index = source.FindIndex(i => !mask.Contains(i) && attribute.AreNamesEqual(source[i].Name, options));
            }

            return index;
        }
    }
}
