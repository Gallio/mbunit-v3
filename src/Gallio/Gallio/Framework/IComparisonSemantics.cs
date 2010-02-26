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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using Gallio.Common;
using Gallio.Common.Diagnostics;
using Gallio.Runtime.Extensibility;

namespace Gallio.Framework
{
    /// <summary>
    /// Describes the semantics of how objects should be compared.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class encapsulates a default set of rules for comparing objects.  These rules may be
    /// used as the foundation of a suite of standard assertion functions for comparing identity,
    /// equality and relations.
    /// </para>
    /// <para>
    /// The comparison engine has extension points available:
    /// <list type="bullet">Extend the object comparison by registring custom comparers through <see cref="CustomComparers"/>.</list>
    /// <list type="bullet">Extend the object eqaulity by registring custom equality comparers through <see cref="CustomEqualityComparers"/>.</list>
    /// </para>
    /// <para>
    /// Custom comparers defined through <see cref="CustomComparers"/> or <see cref="CustomEqualityComparers"/> 
    /// have always a higher priority than any built-in comparer, including inner type comparers such as 
    /// <see cref="IEquatable{T}"/> or <see cref="IComparable{T}"/>.
    /// </para>
    /// </remarks>
    public interface IComparisonSemantics
    {
        /// <summary>
        /// Returns <c>true</c> if two objects are the same.
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
        /// <param name="left">The left object, may be null.</param>
        /// <param name="right">The right object, may be null.</param>
        /// <returns>True if both objects are the same.</returns>
        /// <typeparam name="T">The object type, which must be a reference type (class) since a value type
        /// (struct) has no concept of referential identity.</typeparam>
        bool Same<T>(T left, T right)
            where T : class;

        /// <summary>
        /// Returns true if two objects are equal.
        /// </summary>
        /// <param name="left">The left object, may be null.</param>
        /// <param name="right">The right object, may be null.</param>
        /// <returns>True if the objects are equal.</returns>
        /// <seealso cref="Equals{T}"/> for details.
        bool Equals(object left, object right);

        /// <summary>
        /// Returns true if two objects are equal.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Rules applied:
        /// <list type="bullet">
        /// <item>If both objects are null, returns true.</item>
        /// <item>If one object is null but not the other, returns false.</item>
        /// <item>If both objects are referentially equal, returns true.</item>
        /// <item>If both objects are of a special type, applies special type equality semantics (see below)</item>
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
        /// <param name="left">The left object, may be null.</param>
        /// <param name="right">The right object, may be null.</param>
        /// <returns>True if the objects are equal.</returns>
        /// <typeparam name="T">The object type.</typeparam>
        bool Equals<T>(T left, T right);

        /// <summary>
        /// Compares two objects.
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
        /// <param name="left">The left object, may be null.</param>
        /// <param name="right">The right object, may be null.</param>
        /// <returns>A value less than zero if the left object if less than
        /// the right object, zero if the left and right objects are equal, or greater than zero if
        /// the left object is greater than the right object.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the values cannot be ordered.</exception>
        /// <typeparam name="T">The object type.</typeparam>
        int Compare<T>(T left, T right);

        /// <summary>
        /// Returns <c>true</c> if the specified type is a simple enumerable type.
        /// </summary>
        /// <remarks>
        /// <para>
        /// A simple enumerable type is one that that does not override <see cref="Object.Equals(Object)"/>.
        /// </para>
        /// <para>
        /// The set of simple enumerable types includes arrays, lists, dictionaries and other
        /// standard collection types in the .Net framework.
        /// </para>
        /// </remarks>
        /// <param name="type">The object type.</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="type"/> is null.</exception>
        bool IsSimpleEnumerableType(Type type);

        /// <summary>
        /// Returns true if two values are equal to within a specified delta.
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
        /// <typeparam name="TValue">The type of values to be compared.</typeparam>
        /// <typeparam name="TDifference">The type of the difference produced when the values are
        /// subtracted, for numeric types this is the same as <typeparamref name="TValue"/> but it
        /// may differ for other types.</typeparam>
        /// <param name="left">The expected value.</param>
        /// <param name="right">The actual value.</param>
        /// <param name="delta">The inclusive delta between the values.</param>
        /// <returns>True if the values are approximately equal.</returns>
        bool ApproximatelyEqual<TValue, TDifference>(TValue left, TValue right, TDifference delta);
    }
}