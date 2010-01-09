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
    /// Extensibility point for object comparison managed by <see cref="ComparisonSemantics"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Use the static methods <see cref="Register"/> and <see cref="Unregister"/> to add and remove 
    /// custom type comparers.
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
    /// CustomComparers.Register<IFoo>((x, y) => x.Value.CompareTo(y.Value));
    /// ]]></code>
    /// </example>
    public static class CustomComparers
    {
        private static readonly IDictionary<Type, Comparison> Comparers = new Dictionary<Type, Comparison>();

        /// <summary>
        /// Registers a custom comparer for the specified type.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The Gallio inner comparison engine handles with the case of null objects comparison prior to
        /// the custom comparers. Therefore, while defining a custom comparer, you can safely assume that the 
        /// objects to compare are never null.
        /// </para>
        /// </remarks>
        /// <param name="type">The type for which the custom comparer operates.</param>
        /// <param name="comparer">A comparer that returns a negative value if the first object represents less that the second object, 
        /// zero if they represents the same value; or a positive value if the first object represents more than the second object.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="type"/> or <paramref name="comparer"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if a custom comparer for the specified type was already registered.</exception>
        public static void Register(Type type, Comparison comparer)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if (comparer == null)
                throw new ArgumentNullException("comparer");

            if (Comparers.ContainsKey(type))
                throw new InvalidOperationException(
                    String.Format("A custom comparer for the type '{0}' was already registered.", type));

            Comparers.Add(type, comparer);
        }

        /// <summary>
        /// Registers a strongly-tped custom comparer for the specified type.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The Gallio inner comparison engine handles with the case of null objects comparison prior to
        /// the custom comparers. Therefore, while defining a custom comparer, you can safely assume that the 
        /// objects to compare are never null.
        /// </para>
        /// </remarks>
        /// <typeparam name="T">The type for which the custom comparer operates.</typeparam>
        /// <param name="comparer">A comparer that returns a negative value if the first object represents less that the second object, 
        /// zero if they represents the same value; or a positive value if the first object represents more than the second object.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="comparer"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if a custom comparer for the specified type was already registered.</exception>
        public static void Register<T>(Comparison<T> comparer)
        {
            if (comparer == null)
                throw new ArgumentNullException("comparer");

            Register(typeof(T), (x, y) => comparer((T)x, (T)y));
        }

        /// <summary>
        /// Unregisters the custom comparer for the specified type.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If no custom comparer exists for the specified type,
        /// the method has no effect and no exception is thrown.
        /// </para>
        /// </remarks>
        /// <param name="type">The searched type.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="type"/> is null.</exception>
        public static void Unregister(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            Comparers.Remove(type);
        }

        /// <summary>
        /// Unregisters the custom comparer for the specified type.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If no custom comparer exists for the specified type,
        /// the method has no effect and no exception is thrown.
        /// </para>
        /// </remarks>
        /// <typeparam name="T">The searched type.</typeparam>
        public static void Unregister<T>()
        {
            Unregister(typeof(T));
        }

        // Returns the comparer for the searched type, or null if none was registered.
        internal static Comparison Find(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            Comparison func;
            return Comparers.TryGetValue(type, out func) ? func : null;
        }

        // Removes all the registered custom comparers.
        internal static void UnregisterAll()
        {
            Comparers.Clear();
        }
    }
}
