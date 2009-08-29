using System;
using System.Collections.Generic;
using System.Text;

namespace Gallio.Common.Xml
{
    /// <summary>
    /// A collection of XML items that can be compared another 
    /// collection of the same type, in order to get the differences.
    /// </summary>
    /// <typeparam name="TCollection">The type of the collection.</typeparam>
    /// <typeparam name="TItem">The type of the items in the collection.</typeparam>
    public interface IDiffableCollection<TCollection, TItem> : IDiffable<TCollection>, IEnumerable<TItem>
    {
        /// <summary>
        /// Gets the number of items in the collection.
        /// </summary>
        int Count
        {
            get;
        }

        /// <summary>
        /// Gets the item located at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index.</param>
        /// <returns>The item found at the searched index.</returns>
        TItem this[int index]
        {
            get;
        }

        /// <summary>
        /// Returns the index of the first item which matches the specified predicates.
        /// </summary>
        /// <param name="predicate">The predicate to evaluate.</param>
        /// <returns>The index of the first item found; otherwise -1 if no item was found.</returns>
        int FindIndex(Predicate<int> predicate);
    }
}
