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
//#define STATISTICS

using System;
using System.Collections.Generic;

namespace Gallio.Common
{
    /// <summary>
    /// A structure that memoizes the result of some computation for later reuse.
    /// Maintains an internal dictionary to memoize results by key.
    /// </summary>
    /// <remarks>
    /// Not thread safe.
    /// </remarks>
    /// <typeparam name="TKey">The key type</typeparam>
    /// <typeparam name="TValue">The value type</typeparam>
    /// <example>
    /// <code><![CDATA[
    /// public class MyClass
    /// {
    ///     // Do NOT put the "readonly" keyword on this field.
    ///     // Otherwise we will not be able to modify the contents of the structure and memoization will not occur.
    ///     private KeyedMemoizer<string, int> valueMemoizer = new KeyedMemoizer<string, int>();
    ///     
    ///     public int GetValue(string key)
    ///     {
    ///         return valueMemoizer.Memoize(key, () =>
    ///         {
    ///             // Expensive calculation here.
    ///             return 42;
    ///         });
    ///     }
    /// }
    /// ]]></code>
    /// </example>
    public struct KeyedMemoizer<TKey, TValue>
    {
        private const int HybridThreshold = 8;

        private ITable table;

        /// <summary>
        /// Gets the memoized value for the given key if available, otherwise populates it
        /// using the specified populator function and stores it in association with its keys
        /// for later reuse.
        /// </summary>
        /// <param name="key">The key by which to look up a memoized result</param>
        /// <param name="populator">The populator for the value associated with the key</param>
        /// <returns>The value returned by the populator, possibly memoized</returns>
        public TValue Memoize(TKey key, Func<TValue> populator)
        {
            TValue value;
            if (table != null)
            {
                if (table.Lookup(key, out value))
                {
#if STATISTICS
                    OnHit();
                    OnReturn();
#endif
                    return value;
                }
            }
            else
            {
                table = new ArrayTable();
#if STATISTICS
                OnSmallCacheCreated();
#endif
            }

#if STATISTICS
            OnMiss();
#endif
            value = populator();
            if (! table.Store(key, value))
            {
                var newTable = new DictionaryTable();
                table.CopyTo(newTable);
                table = newTable;
                table.Store(key, value);
#if STATISTICS
                OnSmallCachePromotedToLarge();
#endif
            }

#if STATISTICS
            OnReturn();
#endif
            return value;
        }

#if STATISTICS
        private static int numSmallCaches;
        private static int numLargeCaches;
        private static int maxSize;
        private static int hits;
        private static int misses;
        private static DateTime nextSample;

        private static void OnSmallCacheCreated()
        {
            numSmallCaches += 1;
        }

        private static void OnSmallCachePromotedToLarge()
        {
            numSmallCaches -= 1;
            numLargeCaches += 1;
        }

        private static void OnHit()
        {
            hits += 1;
        }

        private void OnMiss()
        {
            misses += 1;
            if (table.Count > maxSize)
                maxSize = table.Count;
        }

        private static void OnReturn()
        {
            DateTime now = DateTime.Now;
            if (now > nextSample)
            {
                nextSample = now.AddSeconds(5);
                DiagnosticLog.WriteLine(
                    "KeyedMemoizer<{0}, {1}>: # Small = {2}, # Large = {3}, Max Size = {4}, Hits = {5}, Misses = {6}",
                    typeof(TKey).Name, typeof(TValue).Name, numSmallCaches, numLargeCaches, maxSize, hits, misses);
            }
        }
#endif

        private interface ITable
        {
            void CopyTo(ITable table);
            bool Lookup(TKey key, out TValue value);
            bool Store(TKey key, TValue value);
            int Count { get; }
        }

        private sealed class DictionaryTable : Dictionary<TKey, TValue>, ITable
        {
            private TValue nullValue;
            private bool hasNullValue;

            public void CopyTo(ITable table)
            {
                throw new NotSupportedException();
            }

            public bool Lookup(TKey key, out TValue value)
            {
                if (key == null)
                {
                    if (hasNullValue)
                    {
                        value = nullValue;
                        return true;
                    }

                    value = default(TValue);
                    return false;
                }

                return TryGetValue(key, out value);
            }

            public bool Store(TKey key, TValue value)
            {
                if (key == null)
                {
                    nullValue = value;
                    hasNullValue = true;
                }
                else
                {
                    Add(key, value);
                }

                return true;
            }
        }

        private sealed class ArrayTable : ITable
        {
            private readonly KeyValuePair<TKey, TValue>[] items = new KeyValuePair<TKey, TValue>[HybridThreshold];
            private int count;

            public void CopyTo(ITable table)
            {
                for (int i = 0; i < count; i++)
                    table.Store(items[i].Key, items[i].Value);
            }

            public bool Lookup(TKey key, out TValue value)
            {
                for (int i = 0; i < count; i++)
                {
                    if (Equals(key, items[i].Key))
                    {
                        value = items[i].Value;
                        return true;
                    }
                }

                value = default(TValue);
                return false;
            }

            public bool Store(TKey key, TValue value)
            {
                if (count == HybridThreshold)
                    return false;

                items[count++] = new KeyValuePair<TKey, TValue>(key, value);
                return true;
            }

            public int Count
            {
                get { return count; }
            }
        }
    }
}