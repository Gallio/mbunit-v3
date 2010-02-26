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
using Gallio.Common;

namespace Gallio.Framework
{
    /// <summary>
    /// Extensibility point for object equality managed by <see cref="IComparisonSemantics"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Use the methods <see cref="Register"/> and <see cref="Unregister"/> to add and remove 
    /// custom type equality comparers.
    /// </para>
    /// <para>
    /// The Gallio inner comparison engine handles with the case of null objects comparison prior to
    /// the custom comparers. Therefore, while defining a custom comparer, you can safely assume that the 
    /// objects to compare are never null.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code><![CDATA[
    /// public interface IFoo
    /// {
    ///     int Value
    ///     {
    ///         get;
    ///     }
    /// }
    /// 
    /// var customEqualityComparers = new CustomEqualityComparers();
    /// customEqualityComparers.Register<IFoo>((x, y) => x.Value == y.Value);
    /// ]]></code>
    /// </example>
    public class CustomEqualityComparers
    {
        private readonly IDictionary<Type, Data> equalityComparers = new Dictionary<Type, Data>();
        private readonly object syncRoot = new object();

        /// <summary>
        /// Registers a custom equality comparer for the specified type.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The Gallio inner comparison engine handles with the case of null objects comparison prior to
        /// the custom comparers. Therefore, while defining a custom comparer, you can safely assume that the 
        /// objects to compare are never null.
        /// </para>
        /// </remarks>
        /// <param name="type">The type for which the custom comparer operates.</param>
        /// <param name="equalityComparer">An equality comparer that returns true is the objects are equivalent; false otherwise.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="type"/> or <paramref name="equalityComparer"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if a custom comparer for the specified type was already registered.</exception>
        public void Register(Type type, EqualityComparison equalityComparer)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if (equalityComparer == null)
                throw new ArgumentNullException("equalityComparer");

            lock (syncRoot)
            {
                Data data;

                if (equalityComparers.TryGetValue(type, out data))
                {
                    data.Count++;
                }
                else
                {
                    equalityComparers[type] = new Data(equalityComparer);
                }
            }
        }

        /// <summary>
        /// Registers a strongly-typed custom equality comparer for the specified type.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The Gallio inner comparison engine handles with the case of null objects comparison prior to
        /// the custom comparers. Therefore, while defining a custom comparer, you can safely assume that the 
        /// objects to compare are never null.
        /// </para>
        /// </remarks>
        /// <typeparam name="T">The type for which the custom comparer operates.</typeparam>
        /// <param name="equalityComparer">An equality comparer that returns true is the objects are equivalent.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="equalityComparer"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if a custom comparer for the specified type was already registered.</exception>
        public void Register<T>(EqualityComparison<T> equalityComparer)
        {
            if (equalityComparer == null)
                throw new ArgumentNullException("equalityComparer");

            Register(typeof(T), (x, y) => equalityComparer((T)x, (T)y));
        }

        /// <summary>
        /// Unregisters the custom equality comparer for the specified type.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If no custom comparer exists for the specified type,
        /// the method has no effect and no exception is thrown.
        /// </para>
        /// </remarks>
        /// <param name="type">The searched type.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="type"/> is null.</exception>
        public void Unregister(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            lock (syncRoot)
            {
                Data data;

                if (equalityComparers.TryGetValue(type, out data))
                {
                    if (data.Count > 0)
                    {
                        data.Count--;
                    }
                    else
                    {
                        equalityComparers.Remove(type);
                    }
                }
            }
        }

        /// <summary>
        /// Unregisters the custom equality comparer for the specified type.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If no custom comparer exists for the specified type,
        /// the method has no effect and no exception is thrown.
        /// </para>
        /// </remarks>
        /// <typeparam name="T">The searched type.</typeparam>
        public void Unregister<T>()
        {
            Unregister(typeof(T));
        }

        // Returns the equality comparer for the searched type, or null if none was registered.
        internal EqualityComparison Find(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            lock (syncRoot)
            {
                Data data;
                return equalityComparers.TryGetValue(type, out data) ? data.EqualityComparer : null;
            }
        }

        // Removes all the registered custom comparers.
        internal void UnregisterAll()
        {
            lock (syncRoot)
            {
                equalityComparers.Clear();
            }
        }

        private sealed class Data
        {
            private readonly EqualityComparison equalityComparer;

            public EqualityComparison EqualityComparer
            {
                get
                {
                    return equalityComparer;
                }
            }

            public int Count
            {
                get;
                set;
            }

            public Data(EqualityComparison equalityComparer)
            {
                this.equalityComparer = equalityComparer;
            }
        }
    }
}
