using System;
using System.Collections;
using System.Collections.Generic;

namespace MbUnit.Core.Collections
{
    /// <summary>
    /// A synchronized keyed collection implementation based on a dictionary.
    /// </summary>
    /// <remarks>
    /// The operations on this class are thread-safe, including enumeration.
    /// </remarks>
    /// <typeparam name="TKey">The type of the key</typeparam>
    /// <typeparam name="TValue">The type of the value</typeparam>
    [Serializable]
    public abstract class SyncUnorderedKeyedCollection<TKey, TValue> : IKeyedCollection<TKey, TValue>
    {
        private IDictionary<TKey, TValue> collection;

        [NonSerialized]
        private TValue[] cachedArray;

        /// <summary>
        /// Creates an empty collection.
        /// </summary>
        public SyncUnorderedKeyedCollection()
        {
            collection = new Dictionary<TKey, TValue>();
        }

        /// <summary>
        /// Gets the underlying dictionary.
        /// </summary>
        protected IDictionary<TKey, TValue> Collection
        {
            get { return collection; }
        }

        public object SyncRoot
        {
            get { return this; }
        }

        public virtual bool ContainsKey(TKey key)
        {
            lock (SyncRoot)
            {
                return collection.ContainsKey(key);
            }
        }

        public virtual TValue this[TKey key]
        {
            get
            {
                lock (SyncRoot)
                {
                    return collection[key];
                }
            }
        }

        public virtual bool TryGetValue(TKey key, out TValue value)
        {
            lock (SyncRoot)
            {
                return collection.TryGetValue(key, out value);
            }
        }


        public virtual void Add(TValue item)
        {
            lock (SyncRoot)
            {
                collection.Add(GetKeyForItem(item), item);
                ClearCachedArray();
            }
        }

        public virtual void AddAll(IEnumerable<TValue> items)
        {
            lock (SyncRoot)
            {
                foreach (TValue item in items)
                    collection.Add(GetKeyForItem(item), item);

                ClearCachedArray();
            }
        }

        public virtual void Clear()
        {
            lock (SyncRoot)
            {
                collection.Clear();
                ClearCachedArray();
            }
        }

        public virtual bool Contains(TValue item)
        {
            lock (SyncRoot)
            {
                return collection.Values.Contains(item);
            }
        }

        public virtual void CopyTo(TValue[] array, int arrayIndex)
        {
            lock (SyncRoot)
            {
                foreach (TValue value in collection.Values)
                    array[arrayIndex++] = value;
            }
        }

        public virtual bool Remove(TValue item)
        {
            lock (SyncRoot)
            {
                TKey key = GetKeyForItem(item);
                TValue existingValue;
                
                if (! collection.TryGetValue(key, out existingValue))
                    return false;

                ClearCachedArray();
                return collection.Remove(key);
            }
        }

        public virtual int Count
        {
            get
            {
                lock (SyncRoot)
                {
                    return collection.Count;
                }
            }
        }

        public virtual bool IsReadOnly
        {
            get { return false; }
        }

        public virtual TValue[] ToArray()
        {
            lock (SyncRoot)
            {
                if (cachedArray == null)
                {
                    ICollection<TValue> values = collection.Values;

                    cachedArray = new TValue[values.Count];
                    values.CopyTo(cachedArray, 0);
                }

                return cachedArray;
            }
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            foreach (TValue value in ToArray())
                yield return value;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ToArray().GetEnumerator();
        }

        /// <summary>
        /// Gets the key for the specified item in the collection.
        /// </summary>
        /// <param name="item">The item</param>
        /// <returns>The associated key</returns>
        protected abstract TKey GetKeyForItem(TValue item);

        /// <summary>
        /// Clears the cached array used to accelerate <see cref="ToArray" /> and <see cref="GetEnumerator" />.
        /// </summary>
        protected void ClearCachedArray()
        {
            cachedArray = null;
        }
    }
}
