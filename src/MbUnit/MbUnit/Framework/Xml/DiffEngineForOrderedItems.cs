using System;
using System.Collections.Generic;
using System.Text;

namespace MbUnit.Framework.Xml
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
        private readonly Path path;
        private readonly XmlEqualityOptions options;
        private readonly string itemName;

        /// <summary>
        /// Constructs the diffing engine.
        /// </summary>
        /// <param name="expected">The expected object.</param>
        /// <param name="actual">The actual object.</param>
        /// <param name="path">The current path of the parent node.</param>
        /// <param name="options">Equality options.</param>
        /// <param name="itemName">A friendly name for the items.</param>
        public DiffEngineForOrderedItems(TCollection expected, TCollection actual, Path path, XmlEqualityOptions options, string itemName)
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
