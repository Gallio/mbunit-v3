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

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using Gallio.Model.Diagnostics;

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
    [TestFrameworkInternal]
    public static class ComparisonSemantics
    {
        private static readonly Type[] EqualsParams = new[] { typeof(object) };
        private static readonly Dictionary<Type, bool> SimpleEnumerableTypeCache = new Dictionary<Type, bool>();
        private static readonly Dictionary<Type, Pair<Type, Delegate>> PrimitiveSubtractionFuncs = new Dictionary<Type, Pair<Type, Delegate>>();
        private static readonly Dictionary<Type, Func<object, object, bool>> SpecialEqualityFuncs = new Dictionary<Type,Func<object,object,bool>>();

        static ComparisonSemantics()
        {
            AddPrimitiveDifferencer<SByte, Int32>((a, b) => a - b);
            AddPrimitiveDifferencer<Byte, Int32>((a, b) => a - b);
            AddPrimitiveDifferencer<Int16, Int32>((a, b) => a - b);
            AddPrimitiveDifferencer<UInt16, Int32>((a, b) => a - b);
            AddPrimitiveDifferencer<Int32, Int32>((a, b) => a - b);
            AddPrimitiveDifferencer<UInt32, UInt32>((a, b) => a - b);
            AddPrimitiveDifferencer<Int64, Int64>((a, b) => a - b);
            AddPrimitiveDifferencer<UInt64, UInt64>((a, b) => a - b);
            AddPrimitiveDifferencer<Single, Single>((a, b) => a - b);
            AddPrimitiveDifferencer<Double, Double>((a, b) => a - b);
            AddPrimitiveDifferencer<Char, Int32>((a, b) => a - b);

            AddSpecialEqualityFunc<AssemblyName>((a, b) => a.FullName == b.FullName);
            AddSpecialEqualityFunc<DirectoryInfo>((a, b) => a.ToString() == b.ToString());
            AddSpecialEqualityFunc<FileInfo>((a, b) => a.ToString() == b.ToString());
        }

        private static void AddPrimitiveDifferencer<T, D>(SubtractionFunc<T, D> subtractionFunc)
        {
            PrimitiveSubtractionFuncs.Add(typeof(T), new Pair<Type, Delegate>(typeof(D), subtractionFunc));
        }

        private static void AddSpecialEqualityFunc<T>(Func<T, T, bool> equalityFunc)
        {
            SpecialEqualityFuncs.Add(typeof(T), (a, b) => equalityFunc((T)a, (T)b));
        }

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
        /// </summary>
        /// <remarks>
        /// <para>
        /// Rules applied:
        /// <list type="bullet">
        /// <item>Objects are the same if they are referentially
        /// equal according to <see cref="Object.ReferenceEquals"/>.</item>
        /// </list>
        /// </para>
        /// </remarks>
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
        /// </summary>
        /// <param name="left">The left object, may be null</param>
        /// <param name="right">The right object, may be null</param>
        /// <returns>True if the objects are equal</returns>
        /// <seealso cref="Equals{T}"/> for details.
        public new static bool Equals(object left, object right)
        {
            return Equals<object>(left, right);
        }

        /// <summary>
        /// <para>
        /// Returns true if two objects are equal.
        /// </para>
        /// </summary>
        /// <remarks>
        /// <para>
        /// Rules applied:
        /// <list type="bullet">
        /// <item>If both objects are null, returns true.</item>
        /// <item>If one object is null but not the other, returns false.</item>
        /// <item>If both objects are referentially equal, returns true.</item>
        /// <item>If both objects are of a special type, applies special type euqality semantics (see below)</item>
        /// <item>If the objects are both instances of the same simple enumerable type (<seealso cref="IsSimpleEnumerableType" />),
        /// returns true if the collections have equal length and contents.</item>
        /// <item>Otherwise uses <see cref="Object.Equals(Object)" /> to determine equality.</item>
        /// </list>
        /// </para>
        /// <para>
        /// Special type semantics:
        /// <list type="bullet">
        /// <item><see cref="FileInfo" /> and <see cref="DirectoryInfo" />: Compared by their ToString().</item>
        /// <item><see cref="AssemblyName" />: Compared by their full name.</item>
        /// </list>
        /// </para>
        /// </remarks>
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
            if (leftType == right.GetType())
            {
                Func<object, object, bool> equalityFunc = GetSpecialEqualityFunc(leftType);
                if (equalityFunc != null)
                {
                    return equalityFunc(left, right);
                }

                if (IsSimpleEnumerableType(leftType))
                {
                    IEnumerable leftEnumerable = (IEnumerable) left;
                    IEnumerable rightEnumerable = (IEnumerable) right;
                    return CompareEnumerables(leftEnumerable, rightEnumerable, CompareEqualsShim) == 0;
                }
            }

            return left.Equals(right);
        }

        private static int CompareEqualsShim(object left, object right)
        {
            return Equals(left, right) ? 0 : 1;
        }

        private static Func<object, object, bool> GetSpecialEqualityFunc(Type type)
        {
            Func<object, object, bool> func;
            SpecialEqualityFuncs.TryGetValue(type, out func);
            return func;
        }

        /// <summary>
        /// <para>
        /// Compares two objects.
        /// </para>
        /// </summary>
        /// <remarks>
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
                return -Compare(right, left);

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
                    return -comparison(right, left);
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
                        return (left, right) => (int)leftInterface.GetMethod("CompareTo").Invoke(left, new object[] { right });
                }
            }

            return null;
        }

        /// <summary>
        /// <para>
        /// Returns true if the specified type is a simple enumerable type.  A simple enumerable
        /// type is one that that does not override <see cref="Object.Equals(Object)"/>.
        /// </para>
        /// </summary>
        /// <remarks>
        /// <para>
        /// The set of simple enumerable types includes arrays, lists, dictionaries and other
        /// standard collection types in the .Net framework.
        /// </para>
        /// </remarks>
        /// <param name="type">The object type</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="type"/> is null</exception>
        public static bool IsSimpleEnumerableType(Type type)
        {
            lock (SimpleEnumerableTypeCache)
            {
                bool result;
                if (!SimpleEnumerableTypeCache.TryGetValue(type, out result))
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

        /// <summary>
        /// <para>
        /// Returns true if two values are equal to within a specified delta.
        /// </para>
        /// </summary>
        /// <remarks>
        /// <para>
        /// The values are considered approximately equal if the absolute value of their difference
        /// is less than or equal to the delta.
        /// </para>
        /// <para>
        /// This method works with any comparable type that also supports a subtraction operator
        /// including <see cref="Single" />, <see cref="Double" />, <see cref="Decimal" />,
        /// <see cref="Int32" />, <see cref="DateTime" /> (using a <see cref="TimeSpan" /> delta),
        /// and many others.
        /// </para>
        /// </remarks>
        /// <typeparam name="TValue">The type of values to be compared</typeparam>
        /// <typeparam name="TDifference">The type of the difference produced when the values are
        /// subtracted, for numeric types this is the same as <typeparamref name="TValue"/> but it
        /// may differ for other types</typeparam>
        /// <param name="left">The expected value</param>
        /// <param name="right">The actual value</param>
        /// <param name="delta">The inclusive delta between the values</param>
        /// <returns>True if the values are approximately equal</returns>
        public static bool ApproximatelyEqual<TValue, TDifference>(TValue left, TValue right, TDifference delta)
        {
            SubtractionFunc<TValue, TDifference> subtractionFunc = GetSubtractionFunc<TValue, TDifference>();

            int discriminator = Compare(left, right);
            TDifference difference = discriminator < 0 ? subtractionFunc(right, left) : subtractionFunc(left, right);
            return Compare(difference, delta) <= 0;
        }

        /// <summary>
        /// Gets a function for subtracting values of a given type.
        /// </summary>
        /// <typeparam name="TValue">The type of values to be compared</typeparam>
        /// <typeparam name="TDifference">The type of the difference produced when the values are
        /// subtracted, for numeric types this is the same as <typeparamref name="TValue"/> but it
        /// may differ for other types</typeparam>
        /// <returns>The subtraction function</returns>
        /// <exception cref="InvalidOperationException">Thrown if no subtraction function exists
        /// or if the difference type is incorrect.</exception>
        internal static SubtractionFunc<TValue, TDifference> GetSubtractionFunc<TValue, TDifference>()
        {
            Type valueType = typeof(TValue);
            Type differenceType = typeof(TDifference);

            Pair<Type, Delegate> subtractionInfo;
            if (PrimitiveSubtractionFuncs.TryGetValue(valueType, out subtractionInfo))
            {
                CheckDifferenceType(valueType, differenceType, subtractionInfo.First);
                return (SubtractionFunc<TValue, TDifference>)subtractionInfo.Second;
            }
            else
            {
                MethodInfo opSubtraction = valueType.GetMethod("op_Subtraction",
                    BindingFlags.Static | BindingFlags.Public,
                    null, new[] {valueType, valueType}, null);

                if (opSubtraction != null)
                {
                    CheckDifferenceType(valueType, differenceType, opSubtraction.ReturnType);
                    return (a, b) => (TDifference) opSubtraction.Invoke(null, new object[] {a, b});
                }
            }

            throw new InvalidOperationException(String.Format("There is no defined subtraction operator for type {0}.", valueType));
        }

        private static void CheckDifferenceType(Type valueType, Type expectedDifferenceType, Type actualDifferenceType)
        {
            if (actualDifferenceType != expectedDifferenceType)
                throw new InvalidOperationException(String.Format("The subtraction operator for type {0} returns a difference of type {1} but the caller expected a difference of type {2}.", valueType, actualDifferenceType, expectedDifferenceType));
        }
    }
}