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
    /// <inheritdoc />
    [SystemInternal]
    public class DefaultComparisonSemantics : IComparisonSemantics
    {
        private readonly Type[] equalsParams = new[] { typeof(object) };
        private readonly IDictionary<Type, bool> simpleEnumerableTypeCache = new Dictionary<Type, bool>();
        private readonly IDictionary<Type, Pair<Type, Delegate>> primitiveSubtractionFuncs = new Dictionary<Type, Pair<Type, Delegate>>();
        private readonly IDictionary<Type, EqualityComparison> specialEqualityFuncs = new Dictionary<Type, EqualityComparison>();
        private readonly IExtensionPoints extensionPoints;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="extensionPoints"></param>
        public DefaultComparisonSemantics(IExtensionPoints extensionPoints)
        {
            if (extensionPoints == null)
                throw new ArgumentNullException("extensionPoints");
            
            this.extensionPoints = extensionPoints;

            // Register primitive differences.
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

        private void AddPrimitiveDifferencer<T, D>(SubtractionFunc<T, D> subtractionFunc)
        {
            primitiveSubtractionFuncs.Add(typeof(T), new Pair<Type, Delegate>(typeof(D), subtractionFunc));
        }

        private void AddSpecialEqualityFunc<T>(Func<T, T, bool> equalityFunc)
        {
            specialEqualityFuncs.Add(typeof(T), (a, b) => equalityFunc((T)a, (T)b));
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        private static new void ReferenceEquals(object a, object b)
        {
        }

        /// <inheritdoc />
        public bool Same<T>(T left, T right)
            where T : class
        {
            return Object.ReferenceEquals(left, right);
        }

        /// <inheritdoc />
        public new bool Equals(object left, object right)
        {
            return Equals<object>(left, right);
        }

        /// <inheritdoc />
        public bool Equals<T>(T left, T right)
        {
            if (Object.ReferenceEquals(left, right))
                return true;
            if (Object.ReferenceEquals(null, left) || 
                Object.ReferenceEquals(null, right))
                return false;

            Type leftType = left.GetType();

            if (leftType == right.GetType())
            {
                EqualityComparison comparer = extensionPoints.CustomEqualityComparers.Find(leftType) ?? GetSpecialEqualityFunc(leftType);

                if (comparer != null)
                {
                    return comparer(left, right);
                }

                if (IsSimpleEnumerableType(leftType))
                {
                    if (IsMultidimensionalArrayType(leftType))
                    {
                        return CompareMultidimensionalArrays(left as Array, right as Array, CompareEqualsShim) == 0;
                    }

                    var leftEnumerable = (IEnumerable)left;
                    var rightEnumerable = (IEnumerable)right;
                    return CompareEnumerables(leftEnumerable, rightEnumerable, CompareEqualsShim) == 0;
                }
            }

            return left.Equals(right);
        }

        private int CompareEqualsShim(object left, object right)
        {
            return Equals(left, right) ? 0 : 1;
        }

        private EqualityComparison GetSpecialEqualityFunc(Type type)
        {
            EqualityComparison comparer;
            specialEqualityFuncs.TryGetValue(type, out comparer);
            return comparer;
        }

        /// <inheritdoc />
        public int Compare<T>(T left, T right)
        {
            if (Object.ReferenceEquals(left, right))
                return 0;
            if (Object.ReferenceEquals(null, left))
                return -Compare(right, left);

            Type leftType = left.GetType();
            Type rightType = Object.ReferenceEquals(null, right) ? null : right.GetType();

            if (leftType == rightType)
            {
                Comparison comparer = extensionPoints.CustomComparers.Find(leftType);

                if (comparer != null)
                    return comparer(left, right);
            }

            if (IsSimpleEnumerableType(leftType))
            {
                if (leftType == rightType)
                {
                    var leftEnumerable = (IEnumerable)left;
                    var rightEnumerable = (IEnumerable)right;
                    return CompareEnumerables(leftEnumerable, rightEnumerable, Compare);
                }

                if (rightType == null)
                    return 1;
            }

            // Try generic comparisons.
            var genericComparable = left as IComparable<T>;
            if (genericComparable != null)
                return genericComparable.CompareTo(right);

            genericComparable = right as IComparable<T>;
            if (genericComparable != null)
                return -genericComparable.CompareTo(left);

            // Try non-generic comparisons.
            var comparable = left as IComparable;
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
            throw new InvalidOperationException(String.Format("No ordering comparison defined on values of type {0} and {1}.", leftType, rightType ?? typeof(T)));
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

        /// <inheritdoc />
        public bool IsSimpleEnumerableType(Type type)
        {
            lock (simpleEnumerableTypeCache)
            {
                bool result;
                if (!simpleEnumerableTypeCache.TryGetValue(type, out result))
                {
                    result = IsSimpleEnumerableTypeUncached(type);
                    simpleEnumerableTypeCache.Add(type, result);
                }

                return result;
            }
        }

        private bool IsSimpleEnumerableTypeUncached(Type type)
        {
            if (!typeof(IEnumerable).IsAssignableFrom(type))
                return false;

            MethodInfo equalsMethod = type.GetMethod("Equals", BindingFlags.Public | BindingFlags.Instance, null, equalsParams, null);
            return equalsMethod.DeclaringType == typeof(object);
        }
        
        private static bool IsMultidimensionalArrayType(Type type)
        {
            return type.IsArray && type.GetArrayRank() > 1;
        }

        private static int CompareEnumerables(IEnumerable leftEnumerable, IEnumerable rightEnumerable, Comparison<object> comparison)
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

        private static int CompareMultidimensionalArrays(Array left, Array right, Comparison<object> comparison)
        {
            if (left.Rank != right.Rank)
                throw new InvalidOperationException("Expected the compared arrays to have the same rank.");

            for (int rank = 0; rank < left.Rank; rank++)
            {
                if (left.GetLongLength(rank) != right.GetLongLength(rank))
                    throw new InvalidOperationException("Expected the compared arrays to have the same dimensional lengths.");
            }

            return CompareEnumerables(left, right, comparison);
        }

        /// <inheritdoc />
        public bool ApproximatelyEqual<TValue, TDifference>(TValue left, TValue right, TDifference delta)
        {
            SubtractionFunc<TValue, TDifference> subtractionFunc = GetSubtractionFunc<TValue, TDifference>();
            int discriminator = Compare(left, right);
            TDifference difference = discriminator < 0 ? subtractionFunc(right, left) : subtractionFunc(left, right);
            return Compare(difference, delta) <= 0;
        }

        /// <summary>
        /// Gets a function for subtracting values of a given type.
        /// </summary>
        /// <typeparam name="TValue">The type of values to be compared.</typeparam>
        /// <typeparam name="TDifference">The type of the difference produced when the values are
        /// subtracted, for numeric types this is the same as <typeparamref name="TValue"/> but it
        /// may differ for other types.</typeparam>
        /// <returns>The subtraction function.</returns>
        /// <exception cref="InvalidOperationException">Thrown if no subtraction function exists
        /// or if the difference type is incorrect.</exception>
        internal SubtractionFunc<TValue, TDifference> GetSubtractionFunc<TValue, TDifference>()
        {
            Type valueType = typeof(TValue);
            Type differenceType = typeof(TDifference);

            Pair<Type, Delegate> subtractionInfo;
            if (primitiveSubtractionFuncs.TryGetValue(valueType, out subtractionInfo))
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