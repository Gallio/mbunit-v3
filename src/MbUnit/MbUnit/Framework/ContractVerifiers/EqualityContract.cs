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
    /// Contract for verifying the implementation of the generic <see cref="IEquatable{T}"/> interface. 
    /// </para>
    /// <para>
    /// Built-in verifications:
    /// <list type="bullet">
    /// <item>
    /// <term>ObjectEquals</term>
    /// <description>The <see cref="Object.Equals(object)"/> method was overriden and 
    /// behaves correctly against the provided equivalence classes.</description>
    /// </item>
    /// <item>
    /// <term>ObjectGetHashCode</term>
    /// <description>The <see cref="Object.GetHashCode"/> method was overriden and behaves 
    /// correctly against the provided equivalence classes.</description>
    /// </item>
    /// <item>
    /// <term>EquatableEquals</term>
    /// <description>The <see cref="IEquatable{T}.Equals"/> method is implemented
    /// and behaves as expected agains the provided equivalence classes.</description>
    /// </item>
    /// <item>
    /// <term>OperatorEquals</term>
    /// <description>The type has a static equality operator (==) overload which behaves
    /// correctly against the provided equivalence classes. Disable that test by setting 
    /// the <see cref="EqualityContract{TTarget}.ImplementsOperatorOverloads"/> 
    /// property to <code>false</code>.</description>
    /// </item>
    /// <item>
    /// <term>OperatorNotEquals</term>
    /// <description>The type has a static inequality operator (!=) overload which 
    /// behaves correctly against the provided equivalence classes. Disable that test by 
    /// setting the <see cref="EqualityContract{TTarget}.ImplementsOperatorOverloads"/> 
    /// property to <code>false</code>.
    /// </description>
    /// </item>
    /// </list>
    /// </para>
    /// <example>
    /// The following example shows a simple class implementing the 
    /// <see cref="IEquatable{T}"/> interface, and a test fixture which uses the
    /// equality contract to test it.
    /// <code><![CDATA[
    /// public class SampleEquatable : IEquatable<SampleEquatable>
    /// {
    ///     private int value;
    /// 
    ///     public SampleEquatable(int value)
    ///     {
    ///         this.value = value;
    ///     }
    /// 
    ///     public override int GetHashCode()
    ///     {
    ///         return value.GetHashCode();
    ///     }
    /// 
    ///     public override bool Equals(object obj)
    ///     {
    ///         return Equals(obj as SampleEquatable);
    ///     }
    /// 
    ///     public bool Equals(SampleEquatable other)
    ///     {
    ///         return !Object.ReferenceEquals(other, null) 
    ///             && (value == other.value);
    ///     }
    /// 
    ///     public static bool operator ==(SampleEquatable left, SampleEquatable right)
    ///     {
    ///         return (Object.ReferenceEquals(left, null)
    ///             && Object.ReferenceEquals(right, null))
    ///             || (!Object.ReferenceEquals(left, null) 
    ///             && left.Equals(right));
    ///     }
    ///     
    ///     public static bool operator !=(SampleEquatable left, SampleEquatable right)
    ///     {
    ///         return !(left == right);
    ///     }
    /// }
    /// 
    /// public class SampleEquatableTest
    /// {
    ///     [VerifyContract]
    ///     public readonly IContract EqualityTests = new EqualityContract<SampleEquatable>()
    ///     {
    ///         ImplementsOperatorOverloads = true, // Optional (default is true)
    ///         EquivalenceClasses = EquivalenceClassCollection<SampleEquatable>.FromDistinctInstances(
    ///             new SampleEquatable(1),
    ///             new SampleEquatable(2),
    ///             new SampleEquatable(3),
    ///             new SampleEquatable(4),
    ///             new SampleEquatable(5)),
    ///     };
    /// }
    /// ]]></code>
    /// </example>
    /// </summary>
    /// <typeparam name="TTarget">The target tested type which implements the 
    /// generic <see cref="IEquatable{T}"/> interface. </typeparam>
    /// <seealso cref="VerifyContractAttribute"/>
    public class EqualityContract<TTarget> : AbstractContract
        where TTarget : IEquatable<TTarget>
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public EqualityContract()
        {
            this.ImplementsOperatorOverloads = true;
        }

        /// <summary>
        /// <para>
        /// Determines whether the verifier will evaluate the presence and the 
        /// behavior of the equality and the inequality operator overloads.
        /// The default value is <code>true</code>.
        /// </para>
        /// <para>
        /// Built-in verifications:
        /// <list type="bullet">
        /// <item>The type has a static equality operator (==) overload which 
        /// behaves correctly against the provided equivalence classes.</item>
        /// <item>The type has a static inequality operator (!=) overload which 
        /// behaves correctly against the provided equivalence classes.</item>
        /// <item></item>
        /// </list>
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
            // Is Object equality method OK?
            yield return CreateEqualityTest("ObjectEquals",
                typeof(TTarget).GetMethod("Equals", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(object) }, null),
                "bool Equals(Object)",
                (leftIndex, rightIndex) => leftIndex == rightIndex);

            // Is Object hash code calculcation well implemented?
            yield return CreateHashCodeTest("HashCode");

            // Is IEquatable equality method OK?
            yield return CreateEqualityTest("EquatableEquals",
                GetIEquatableInterface().GetMethod("Equals", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(TTarget) }, null),
                String.Format("bool Equals({0})", typeof(TTarget).Name),
                (leftIndex, rightIndex) => leftIndex == rightIndex);

            if (ImplementsOperatorOverloads)
            {
                // Is equality operator overload OK?
                yield return CreateEqualityTest("OperatorEquals",
                    typeof(TTarget).GetMethod("op_Equality", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static, null, new Type[] { typeof(TTarget), typeof(TTarget) }, null),
                    String.Format("static bool operator ==({0}, {0})", 
                    typeof(TTarget).Name),
                    (leftIndex, rightIndex) => leftIndex == rightIndex);

                // Is inequality operator overload OK?
                yield return CreateEqualityTest("OperatorNotEquals",
                    typeof(TTarget).GetMethod("op_Inequality", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static, null, new Type[] { typeof(TTarget), typeof(TTarget) }, null),
                    String.Format("static bool operator !=({0}, {0})", 
                    typeof(TTarget).Name),
                    (leftIndex, rightIndex) => leftIndex != rightIndex);
            }
        }

        private Test CreateEqualityTest(string name, MethodInfo method, string methodSignature, Func<int, int, bool> referenceComparer)
        {
            return new TestCase(name, () =>
            {
                AssertMethodExists(method, methodSignature);

                Func<object, object, bool> comparer = BinaryComparerFactory(method);

                Assert.Multiple(() =>
                {
                    if (!typeof(TTarget).IsValueType && method.IsStatic)
                    {
                        VerifyEquality(null, null, int.MinValue, int.MinValue, comparer, referenceComparer);
                    }

                    int leftIndex = 0;
                    foreach (EquivalenceClass<TTarget> leftClass in EquivalenceClasses)
                    {
                        foreach (TTarget leftValue in leftClass)
                        {
                            if (!typeof(TTarget).IsValueType)
                            {
                                VerifyEquality(leftValue, null, 0, int.MinValue, comparer, referenceComparer);

                                if (method.IsStatic)
                                {
                                    VerifyEquality(null, leftValue, int.MinValue, 0, comparer, referenceComparer);
                                }
                            }

                            int rightIndex = 0;
                            foreach (EquivalenceClass<TTarget> rightClass in EquivalenceClasses)
                            {
                                foreach (TTarget rightValue in rightClass)
                                {
                                    VerifyEquality(leftValue, rightValue, leftIndex, rightIndex, comparer, referenceComparer);
                                }

                                rightIndex += 1;
                            }
                        }

                        leftIndex += 1;
                    }
                });
            });
        }

        private Test CreateHashCodeTest(string name)
        {
            return new TestCase(name, () =>
            {
                Assert.Multiple(() =>
                {
                    foreach (EquivalenceClass<TTarget> @class in EquivalenceClasses)
                    {
                        foreach (TTarget leftValue in @class)
                        {
                            foreach (TTarget rightValue in @class)
                            {
                                VerifyHashCodeOfEquivalentInstances(leftValue, rightValue);
                            }
                        }
                    }
                });
            });
        }

        private static void VerifyEquality(object leftValue, object rightValue, int leftIndex, int rightIndex,
            Func<object, object, bool> comparer, Func<int, int, bool> referenceComparer)
        {
            AssertionHelper.Verify(() =>
            {
                bool actualResult = comparer(leftValue, rightValue);
                bool expectedResult = referenceComparer(leftIndex, rightIndex);

                if (actualResult == expectedResult)
                    return null;

                return new AssertionFailureBuilder("The equality comparison between left and right values did not produce the expected result.")
                    .AddRawLabeledValue("Left Value", leftValue)
                    .AddRawLabeledValue("Right Value", rightValue)
                    .AddRawLabeledValue("Expected Result", expectedResult)
                    .AddRawLabeledValue("Actual Result", actualResult)
                    .ToAssertionFailure();
            });
        }

        private static void VerifyHashCodeOfEquivalentInstances(object leftValue, object rightValue)
        {
            AssertionHelper.Verify(() =>
            {
                int leftHashCode = leftValue.GetHashCode();
                int rightHashCode = rightValue.GetHashCode();

                if (leftHashCode == rightHashCode)
                    return null;

                return new AssertionFailureBuilder("Expected the hash codes of two values within the same equivalence class to be equal.")
                    .AddRawLabeledValue("Left Value", leftValue)
                    .AddRawLabeledValue("Left HashCode", leftHashCode)
                    .AddRawLabeledValue("Right Value", rightValue)
                    .AddRawLabeledValue("Right HashCode", rightHashCode)
                    .ToAssertionFailure();
            });
        }

        private static Func<object, object, bool> BinaryComparerFactory(MethodInfo method)
        {
            return (leftValue, rightValue) => method.IsStatic
                ? (bool)method.Invoke(null, new object[] { leftValue, rightValue })
                : (bool)method.Invoke(leftValue, new object[] { rightValue });
        }

        private static Type GetIEquatableInterface()
        {
            return GetInterface(typeof(TTarget), typeof(IEquatable<>)
                .MakeGenericType(typeof(TTarget)));
        }
    }
}
