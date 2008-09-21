// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Gallio.Framework
{
    /// <summary>
    /// <para>
    /// Describes the semantics of how objects should be compared.
    /// </para>
    /// <para>
    /// This class encapsulates a default set of rules for comparing objects.  These rules may be
    /// used as the foundation of a suite of standard assertion functions for comparing identity,
    /// equality and relations.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// At this time, there is no extensiblity mechanism provided for comparison semantics but
    /// it would be nice to have one.  Feel free to add your comments to the issue:
    /// http://code.google.com/p/mb-unit/issues/detail?id=304
    /// </para>
    /// </remarks>
    public static class ComparisonSemantics
    {
        private static readonly Type[] EqualsParams = new[] { typeof(object) };
        private static readonly Dictionary<Type, bool> SimpleEnumerableTypeCache = new Dictionary<Type, bool>();

        #region Private stuff
        [EditorBrowsable(EditorBrowsableState.Never)]
        private static new void ReferenceEquals(object a, object b)
        {
        }
        #endregion

        /// <summary>
        /// <para>
        /// Returns true if two objects are the same.
        /// </para>
        /// <para>
        /// Rules applied:
        /// <list type="bullet">
        /// <item>Objects are the same if they are referentially
        /// equal according to <see cref="Object.ReferenceEquals"/>.</item>
        /// </list>
        /// </para>
        /// </summary>
        /// <param name="left">The left object, may be null</param>
        /// <param name="right">The right object, may be null</param>
        /// <returns>True if both objects are the same</returns>
        /// <typeparam name="T">The object type, which must be a reference type (class) since a value type
        /// (struct) has no concept of referential identity.</typeparam>
        public static bool Same<T>(T left, T right)
            where T : class
        {
            return Object.ReferenceEquals(left, right);
        }

        /// <summary>
        /// <para>
        /// Returns true if two objects are equal.
        /// </para>
        /// <seealso cref="Equals{T}"/> for details.
        /// </summary>
        /// <param name="left">The left object, may be null</param>
        /// <param name="right">The right object, may be null</param>
        /// <returns>True if the objects are equal</returns>
        public new static bool Equals(object left, object right)
        {
            return Equals<object>(left, right);
        }

        /// <summary>
        /// <para>
        /// Returns true if two objects are equal.
        /// </para>
        /// <para>
        /// Rules applied:
        /// <list type="bullet">
        /// <item>If both objects are null, returns true.</item>
        /// <item>If one object is null but not the other, returns false.</item>
        /// <item>If both objects are referentially equal, returns true.</item>
        /// <item>If the objects are both instances of the same simple enumerable type (<seealso cref="IsSimpleEnumerableType" />),
        /// returns true if the collections have equal length and contents.</item>
        /// <item>Otherwise uses <see cref="Object.Equals(Object)" /> to determine equality.</item>
        /// </list>
        /// </para>
        /// </summary>
        /// <param name="left">The left object, may be null</param>
        /// <param name="right">The right object, may be null</param>
        /// <returns>True if the objects are equal</returns>
        /// <typeparam name="T">The object type</typeparam>
        public static bool Equals<T>(T left, T right)
        {
            if (Object.ReferenceEquals(left, right))
                return true;
            if (left == null || right == null)
                return false;

            Type leftType = left.GetType();
            if (leftType == right.GetType() && IsSimpleEnumerableType(leftType))
            {
                IEnumerable leftEnumerable = (IEnumerable)left;
                IEnumerable rightEnumerable = (IEnumerable)right;
                return CompareEnumerables(leftEnumerable, rightEnumerable, CompareEqualsShim) == 0;
            }

            return left.Equals(right);
        }

        private static int CompareEqualsShim(object left, object right)
        {
            return Equals(left, right) ? 0 : 1;
        }

        /// <summary>
        /// <para>
        /// Compares two objects.
        /// </para>
        /// <para>
        /// Rules applied:
        /// <list type="bullet">
        /// <item>If both objects are null, returns 0.</item>
        /// <item>If both objects are referentially equal, returns true.</item>
        /// <item>If the objects are both instances of the same simple enumerable type (<seealso cref="IsSimpleEnumerableType" />),
        /// then compares each pair of values until a non-equal pair is found at which point the
        /// result of that comparison is returned.  If one enumeration runs out of values than the other,
        /// then it is considered to be less than the other.</item>
        /// <item>If the objects implement <see cref="IComparable{T}" />, uses it to determine order.</item>
        /// <item>If the objects implement <see cref="IComparable" />, uses it to determine order.</item>
        /// <item>Otherwise no deterministic comparison is possible so throws an exception.</item>
        /// </list>
        /// </para>
        /// </summary>
        /// <remarks>
        /// <para>
        /// The ordering of nulls is determined by <see cref="IComparable.CompareTo(Object)" /> except
        /// when comparing collections.  If one collection happens to be null but not the other then it
        /// is considered less than the other.
        /// </para>
        /// </remarks>
        /// <param name="left">The left object, may be null</param>
        /// <param name="right">The right object, may be null</param>
        /// <returns>A value less than zero if the left object if less than
        /// the right object, zero if the left and right objects are equal, or greater than zero if
        /// the left object is greater than the right object</returns>
        /// <exception cref="InvalidOperationException">Thrown if the values cannot be ordered.</exception>
        /// <typeparam name="T">The object type</typeparam>
        public static int Compare<T>(T left, T right)
        {
            if (Object.ReferenceEquals(left, right))
                return 0;
            if (left == null)
                return - Compare(right, left);

            Type leftType = left.GetType();
            Type rightType = right != null ? right.GetType() : null;
            if (IsSimpleEnumerableType(leftType))
            {
                if (leftType == rightType)
                {
                    IEnumerable leftEnumerable = (IEnumerable)left;
                    IEnumerable rightEnumerable = (IEnumerable)right;
                    return CompareEnumerables(leftEnumerable, rightEnumerable, Compare);
                }

                if (rightType == null)
                    return 1;
            }

            // Try generic comparisons.
            IComparable<T> genericComparable = left as IComparable<T>;
            if (genericComparable != null)
                return genericComparable.CompareTo(right);

            genericComparable = right as IComparable<T>;
            if (genericComparable != null)
                return -genericComparable.CompareTo(left);

            // Try non-generic comparisons.
            IComparable comparable = left as IComparable;
            if (comparable != null)
                return comparable.CompareTo(right);

            comparable = right as IComparable;
            if (comparable != null)
                return -comparable.CompareTo(left);

            // Maybe T is just a poor choice (maybe it's "object"), search for another
            // specialization of the generic interface.
            Comparison<T> comparison = FindCompatibleGenericComparison<T>(leftType, rightType);
            if (comparison != null)
                return comparison(left, right);

            if (leftType != rightType && rightType != null)
            {
                comparison = FindCompatibleGenericComparison<T>(rightType, leftType);
                if (comparison != null)
                    return - comparison(right, left);
            }

            // Give up.
            throw new InvalidOperationException(
                String.Format("No ordering comparison defined on values of type {0} and {1}.", leftType, rightType ?? typeof(T)));
        }

        private static Comparison<T> FindCompatibleGenericComparison<T>(Type leftType, Type rightType)
        {
            foreach (Type leftInterface in leftType.GetInterfaces())
            {
                if (leftInterface.IsGenericType
                    && leftInterface.GetGenericTypeDefinition() == typeof(IComparable<>))
                {
                    Type argumentType = leftInterface.GetGenericArguments()[0];
                    if (rightType == null || argumentType.IsAssignableFrom(rightType))
                        return (left, right) => (int) leftInterface.GetMethod("CompareTo").Invoke(left, new object[] { right });
                }
            }

            return null;
        }

        /// <summary>
        /// <para>
        /// Returns true if the specified type is a simple enumerable type.  A simple enumerable
        /// type is one that that does not override <see cref="Object.Equals(Object)"/>.
        /// </para>
        /// <para>
        /// The set of simple enumerable types includes arrays, lists, dictionaries and other
        /// standard collection types in the .Net framework.
        /// </para>
        /// </summary>
        /// <param name="type">The object type</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="type"/> is null</exception>
        public static bool IsSimpleEnumerableType(Type type)
        {
            lock (SimpleEnumerableTypeCache)
            {
                bool result;
                if (! SimpleEnumerableTypeCache.TryGetValue(type, out result))
                {
                    result = IsSimpleEnumerableTypeUncached(type);
                    SimpleEnumerableTypeCache.Add(type, result);
                }

                return result;
            }
        }

        private static bool IsSimpleEnumerableTypeUncached(Type type)
        {
            if (type.IsArray)
                return type.GetArrayRank() == 1;
            if (!typeof(IEnumerable).IsAssignableFrom(type))
                return false;

            MethodInfo equalsMethod = type.GetMethod("Equals",
                BindingFlags.Public | BindingFlags.Instance, null, EqualsParams, null);
            return equalsMethod.DeclaringType == typeof(object);
        }

        private static int CompareEnumerables(IEnumerable leftEnumerable, IEnumerable rightEnumerable,
            Comparison<object> comparison)
        {
            IEnumerator leftEnumerator = leftEnumerable.GetEnumerator();
            IEnumerator rightEnumerator = rightEnumerable.GetEnumerator();
            while (leftEnumerator.MoveNext())
            {
                if (!rightEnumerator.MoveNext())
                    return 1;

                int discriminator = comparison(leftEnumerator.Current, rightEnumerator.Current);
                if (discriminator != 0)
                    return discriminator;
            }

            if (rightEnumerator.MoveNext())
                return -1;

            return 0;
        }
    }
}
