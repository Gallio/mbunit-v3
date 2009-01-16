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
using System.Collections.Generic;
using System.Reflection;
using Gallio;
using Gallio.Framework.Assertions;

namespace MbUnit.Framework.ContractVerifiers
{
    /// <summary>
    /// <para>
    /// Contract for verifying the implementation of the generic <see cref="IComparable{T}"/> interface. 
    /// </para>
    /// <para>
    /// Built-in verifications:
    /// <list type="bullet">
    /// <item>
    /// <term>ComparableCompareTo</term>
    /// <description>The type implements the method <see cref="IComparable{T}.CompareTo"/>.
    /// The method behaves as expected agains the provided equivalence classes.</description>
    /// </item>
    /// <item>
    /// <term>OperatorGreaterThan</term>
    /// <description>The type has a static "Greater Than" operator overload which behaves
    /// correctly against the provided equivalence classes. Disable that test by setting 
    /// the <see cref="ComparisonContract{TTarget}.ImplementsOperatorOverloads"/> 
    /// property to <c>false</c>.</description>
    /// </item>
    /// <item>
    /// <term>OperatorGreaterThanOrEqual</term>
    /// <description>The type has a static "Greater Than Or Equal" operator overload which behaves
    /// correctly against the provided equivalence classes. Disable that test by setting 
    /// the <see cref="ComparisonContract{TTarget}.ImplementsOperatorOverloads"/> 
    /// property to <c>false</c>.</description>
    /// </item>
    /// <item>
    /// <term>OperatorLessThan</term>
    /// <description>The type has a static "Less Than" operator overload which behaves
    /// correctly against the provided equivalence classes. Disable that test by setting 
    /// the <see cref="ComparisonContract{TTarget}.ImplementsOperatorOverloads"/> 
    /// property to <c>false</c>.</description>
    /// </item>
    /// <item>
    /// <term>OperatorLessThanOrEqual</term>
    /// <description>The type has a static "Less Than Or Equal" operator overload which behaves
    /// correctly against the provided equivalence classes. Disable that test by setting 
    /// the <see cref="ComparisonContract{TTarget}.ImplementsOperatorOverloads"/> 
    /// property to <c>false</c>.</description>
    /// </item>
    /// </list>
    /// </para>
    /// <example>
    /// The following example shows a simple class implementing the 
    /// <see cref="IComparable{T}"/> interface, and a test fixture which uses the
    /// comparison contract to test it.
    /// <code><![CDATA[
    /// public class SampleComparable : IComparable<SampleComparable>
    /// {
    ///     private int value;
    /// 
    ///     public SampleComparable(int value)
    ///     {
    ///         this.value = value;
    ///     }
    /// 
    ///     public int CompareTo(SampleComparable other)
    ///     {
    ///         return Object.ReferenceEquals(other, null) 
    ///             ? Int32.MaxValue 
    ///             : value.CompareTo(other.value);
    ///     }
    /// 
    ///     public static bool operator >=(SampleComparable left, SampleComparable right)
    ///     {
    ///         return (Object.ReferenceEquals(left, null) 
    ///             && Object.ReferenceEquals(right, null)) 
    ///             || (!Object.ReferenceEquals(left, null) 
    ///             && (left.CompareTo(right) >= 0));
    ///     }
    /// 
    ///     public static bool operator <=(SampleComparable left, SampleComparable right)
    ///     {
    ///         return Object.ReferenceEquals(left, null) 
    ///             || (left.CompareTo(right) <= 0);
    ///     }
    /// 
    ///     public static bool operator >(SampleComparable left, SampleComparable right)
    ///     {
    ///         return !Object.ReferenceEquals(left, null) 
    ///             && (left.CompareTo(right) > 0);
    ///     }
    /// 
    ///     public static bool operator <(SampleComparable left, SampleComparable right)
    ///     {
    ///         return (!Object.ReferenceEquals(left, null) 
    ///             || !Object.ReferenceEquals(right, null)) 
    ///             && (Object.ReferenceEquals(left, null) 
    ///             || (left.CompareTo(right) < 0));
    ///     }
    /// 
    /// public class SampleComparableTest
    /// {
    ///     [VerifyContract]
    ///     public readonly IContract EqualityTests = new ComparisonContract<SampleComparable>()
    ///     {
    ///         ImplementsOperatorOverloads = true, // Optional (default is true)
    ///         EquivalenceClasses = EquivalenceClassCollection<SampleComparable>.FromDistinctInstances(
    ///             new SampleComparable(1),
    ///             new SampleComparable(2),
    ///             new SampleComparable(3),
    ///             new SampleComparable(4),
    ///             new SampleComparable(5)),
    ///     };
    /// }
    /// ]]></code>
    /// </example>
    /// </summary>
    /// <typeparam name="TTarget"></typeparam>
    /// <seealso cref="VerifyContractAttribute"/>
    public class ComparisonContract<TTarget> : AbstractContract
        where TTarget : IComparable<TTarget>
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ComparisonContract()
        {
            this.ImplementsOperatorOverloads = true;
        }

        /// <summary>
        /// <para>
        /// Determines whether the verifier will evaluate the presence and the 
        /// behavior of the four operator overloads "Greater Than", "Greater Than
        /// Or Equal", "Less Than", and "Less Than Or Equal".
        /// The default value is 'true'.
        /// </para>
        /// </summary>
        public bool ImplementsOperatorOverloads
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the collection of equivalence classes of instances
        /// to feed the contract verifier.
        /// </summary>
        public EquivalenceClassCollection<TTarget> EquivalenceClasses
        {
            get;
            set;
        }

        /// <inheritdoc />
        public override IEnumerable<Test> GetContractVerificationTests()
        {
            // Is IComparable.CompareTo implementation OK?
            yield return CreateComparisonTest("ComparableCompareTo",
                GetIComparableInterface().GetMethod("CompareTo", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(TTarget) }, null),
                String.Format("bool CompareTo({0})", typeof(TTarget).Name),
                TernaryComparerFactory,
                TernaryClassifier, 
                (leftIndex, rightIndex) => leftIndex.CompareTo(rightIndex));

            if (ImplementsOperatorOverloads)
            {
                // Is "Greater Than" operator overload implementation OK?
                yield return CreateComparisonTest("OperatorGreaterThan",
                    typeof(TTarget).GetMethod("op_GreaterThan", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static, null, new Type[] { typeof(TTarget), typeof(TTarget) }, null),
                    String.Format("static bool operator >({0}, {0})", typeof(TTarget).Name),
                    BinaryComparerFactory,
                    BinaryClassifier, 
                    (leftIndex, rightIndex) => leftIndex > rightIndex);

                // Is "Greater Than Or Equal" operator overload implementation OK?
                yield return CreateComparisonTest("OperatorGreaterThanOrEqual",
                    typeof(TTarget).GetMethod("op_GreaterThanOrEqual", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static, null, new Type[] { typeof(TTarget), typeof(TTarget) }, null),
                    String.Format("static bool operator >=({0}, {0})", typeof(TTarget).Name),
                    BinaryComparerFactory,
                    BinaryClassifier, 
                    (leftIndex, rightIndex) => leftIndex >= rightIndex);

                // Is "Less Than" operator overload implementation OK?
                yield return CreateComparisonTest("OperatorLessThan",
                    typeof(TTarget).GetMethod("op_LessThan", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static, null, new Type[] { typeof(TTarget), typeof(TTarget) }, null),
                    String.Format("static bool operator <({0}, {0})", typeof(TTarget).Name),
                    BinaryComparerFactory,
                    BinaryClassifier, 
                    (leftIndex, rightIndex) => leftIndex < rightIndex);

                // Is "Less Than Or Equal" operator overload implementation OK?
                yield return CreateComparisonTest("OperatorLessThanOrEqual",
                    typeof(TTarget).GetMethod("op_LessThanOrEqual", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static, null, new Type[] { typeof(TTarget), typeof(TTarget) }, null),
                    String.Format("static bool operator <=({0}, {0})", typeof(TTarget).Name),
                    BinaryComparerFactory,
                    BinaryClassifier, 
                    (leftIndex, rightIndex) => leftIndex <= rightIndex);
            }
        }

        private Test CreateComparisonTest<TResult>(string name, MethodInfo method, string methodSignature, Func<MethodInfo, Func<object, object, TResult>> comparerFactory, Func<TResult, string> classifier, Func<int, int, TResult> referenceComparer)
        {
            return new TestCase(name, () =>
            {
                AssertMethodExists(method, methodSignature);

                Func<object, object, TResult> comparer = comparerFactory(method);

                Assert.Multiple(() =>
                {
                    if (!typeof(TTarget).IsValueType && method.IsStatic)
                    {
                        VerifyComparison(null, null, int.MinValue, int.MinValue, comparer, classifier, referenceComparer);
                    }

                    int leftIndex = 0;

                    foreach (EquivalenceClass<TTarget> leftClass in EquivalenceClasses)
                    {
                        int rightIndex = 0;

                        foreach (TTarget leftValue in leftClass)
                        {
                            if (!typeof (TTarget).IsValueType)
                            {
                                VerifyComparison(leftValue, null, 0, int.MinValue, comparer, classifier, referenceComparer);

                                if (method.IsStatic)
                                {
                                    VerifyComparison(null, leftValue, int.MinValue, 0, comparer, classifier,
                                        referenceComparer);
                                }
                            }

                            foreach (EquivalenceClass<TTarget> rightClass in EquivalenceClasses)
                            {
                                foreach (TTarget rightValue in rightClass)
                                {
                                    VerifyComparison(leftValue, rightValue, leftIndex, rightIndex, comparer, classifier,
                                        referenceComparer);
                                }

                                rightIndex += 1;
                            }
                        }

                        leftIndex += 1;
                    }
                });
            });
        }

        private static void VerifyComparison<TResult>(object leftValue, object rightValue, int leftIndex, int rightIndex,
            Func<object, object, TResult> comparer, Func<TResult, string> classifier,
            Func<int, int, TResult> referenceComparer)
        {
            AssertionHelper.Verify(() =>
            {
                TResult actualResult = comparer(leftValue, rightValue);
                TResult expectedResult = referenceComparer(leftIndex, rightIndex);

                if (classifier(actualResult) == classifier(expectedResult))
                    return null;

                return new AssertionFailureBuilder("The comparison between left and right values did not produce the expected result.")
                    .AddRawLabeledValue("Left Value", leftValue)
                    .AddRawLabeledValue("Right Value", rightValue)
                    .AddLabeledValue("Expected Result", classifier(expectedResult))
                    .AddLabeledValue("Actual Result", classifier(actualResult))
                    .ToAssertionFailure();
            });
        }

        private static Func<object, object, int> TernaryComparerFactory(MethodInfo method)
        {
            return (leftValue, rightValue) => method.IsStatic
                ? (int) method.Invoke(null, new object[] { leftValue, rightValue })
                : (int) method.Invoke(leftValue, new object[] { rightValue });
        }

        private static string TernaryClassifier(int discriminator)
        {
            return discriminator < 0 ? "negative" : discriminator > 0 ? "positive" : "zero";
        }

        private static Func<object, object, bool> BinaryComparerFactory(MethodInfo method)
        {
            return (leftValue, rightValue) => method.IsStatic
                ? (bool) method.Invoke(null, new object[] { leftValue, rightValue })
                : (bool) method.Invoke(leftValue, new object[] { rightValue });
        }

        private static string BinaryClassifier(bool discriminator)
        {
            return discriminator ? "true" : "false";
        }

        private static Type GetIComparableInterface()
        {
            return GetInterface(typeof(TTarget), typeof(IComparable<>)
                .MakeGenericType(typeof(TTarget)));
        }
    }
}
