using System;
using System.Collections.Generic;
using System.Text;

namespace MbUnit.Core.Collections
{
    /// <summary>
    /// Interface for a collection of value objects that can be looked up by.
    /// </summary>
    /// <typeparam name="TKey">The type of the key</typeparam>
    /// <typeparam name="TValue">The type of the value</typeparam>
    public interface IKeyedCollection<TKey, TValue> : ICollection<TValue>
    {
        /// <summary>
        /// Gets the root synchronization object of the collection.
        /// </summary>
        object SyncRoot { get; }

        /// <summary>
        /// Adds all of the specified values to the collection.
        /// </summary>
        /// <param name="values">The values to add</param>
        void AddAll(IEnumerable<TValue> values);

        /// <summary>
        /// Returns true if the collection contains a value with the specified key.
        /// </summary>
        /// <param name="key">The key to lookup</param>
        /// <returns>True if the key is associated with a value in the collection</returns>
        bool ContainsKey(TKey key);

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key to lookup</param>
        /// <returns>The associated value</returns>
        TValue this[TKey key] { get; }

        /// <summary>
        /// Tries to get the values associated with the specified key.
        /// Returns true if a value was found, false otherwise.
        /// </summary>
        /// <param name="key">The key to lookup</param>
        /// <param name="value">The associated value, or <code>default(TValue)</code> if not found</param>
        /// <returns>True if a value was found</returns>
        bool TryGetValue(TKey key, out TValue value);

        /// <summary>
        /// Returns the contents of the collection as an array.
        /// </summary>
        /// <returns>The array of collection value</returns>
        TValue[] ToArray();
    }
}
