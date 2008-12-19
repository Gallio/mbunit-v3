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
using System.Text;
using Gallio.Framework.Assertions;
using Gallio.Framework.Data;
using Gallio.Framework.Pattern;
using Gallio.Model;
using Gallio.Reflection;
using MbUnit.Framework.ContractVerifiers.Patterns;
using MbUnit.Framework.ContractVerifiers.Patterns.Comparison;

namespace MbUnit.Framework.ContractVerifiers
{
    /// <summary>
    /// <para>
    /// Field-based contract verifier for the implementation of
    /// the generic <see cref="IComparable{T}"/> interface. 
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
    /// the <see cref="VerifyComparisonContract{T}.ImplementsOperatorOverloads"/> 
    /// property to <c>false</c>.</description>
    /// </item>
    /// <item>
    /// <term>OperatorGreaterThanOrEqual</term>
    /// <description>The type has a static "Greater Than Or Equal" operator overload which behaves
    /// correctly against the provided equivalence classes. Disable that test by setting 
    /// the <see cref="VerifyComparisonContract{T}.ImplementsOperatorOverloads"/> 
    /// property to <c>false</c>.</description>
    /// </item>
    /// <item>
    /// <term>OperatorLessThan</term>
    /// <description>The type has a static "Less Than" operator overload which behaves
    /// correctly against the provided equivalence classes. Disable that test by setting 
    /// the <see cref="VerifyComparisonContract{T}.ImplementsOperatorOverloads"/> 
    /// property to <c>false</c>.</description>
    /// </item>
    /// <item>
    /// <term>OperatorLessThanOrEqual</term>
    /// <description>The type has a static "Less Than Or Equal" operator overload which behaves
    /// correctly against the provided equivalence classes. Disable that test by setting 
    /// the <see cref="VerifyComparisonContract{T}.ImplementsOperatorOverloads"/> 
    /// property to <c>false</c>.</description>
    /// </item>
    /// </list>
    /// </para>
    /// <example>
    /// The following example shows a simple class implementing the 
    /// <see cref="IComparable{T}"/> interface, and a test fixture which uses the
    /// comparison contract verifier to test it.
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
    ///     [ContractVerifier]
    ///     public readonly IContractVerifier EqualityTests = new ComparisonContractVerifier<SampleComparable>()
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
    public class VerifyComparisonContract<TTarget> : AbstractContractVerifier
        where TTarget : IComparable<TTarget>
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public VerifyComparisonContract()
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
        public override IEnumerable<ContractVerifierPattern> GetContractPatterns()
        {
            var equivalenceClassSource = GetType().GetProperty("EquivalenceClasses", BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.Instance);

            // Is IComparable.CompareTo implementation OK?
            yield return new ComparisonPatternBuilder<TTarget, int>()
                .SetName("ComparableCompareTo")
                .SetSignatureDescription(String.Format("public bool CompareTo({0})", typeof(TTarget).Name))
                .SetComparisonMethodInfo(GetIComparableInterface().GetMethod("CompareTo", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(TTarget) }, null))
                .SetFunctionRefers((i, j) => i.CompareTo(j))
                .SetFunctionFormats(x => (x == 0) ? "0" : ((x > 0) ? "A Positive Value" : "A Negative Value"))
                .SetFunctionPostProcesses(x => Math.Sign(x))
                .SetEquivalenceClassSource(equivalenceClassSource)
                .ToPattern();

            if (ImplementsOperatorOverloads)
            {
                // Is "Greater Than" operator overload implementation OK?
                yield return new ComparisonPatternBuilder<TTarget, bool>()
                    .SetName("OperatorGreaterThan")
                    .SetSignatureDescription(String.Format("static bool operator >({0}, {0})", typeof(TTarget).Name))
                    .SetComparisonMethodInfo(typeof(TTarget).GetMethod("op_GreaterThan", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static, null, new Type[] { typeof(TTarget), typeof(TTarget) }, null))
                    .SetFunctionRefers((i, j) => i > j)
                    .SetEquivalenceClassSource(equivalenceClassSource)
                    .ToPattern();

                // Is "Greater Than Or Equal" operator overload implementation OK?
                yield return new ComparisonPatternBuilder<TTarget, bool>()
                   .SetName("OperatorGreaterThanOrEqual")
                   .SetSignatureDescription(String.Format("static bool operator >=({0}, {0})", typeof(TTarget).Name))
                   .SetComparisonMethodInfo(typeof(TTarget).GetMethod("op_GreaterThanOrEqual", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static, null, new Type[] { typeof(TTarget), typeof(TTarget) }, null))
                   .SetFunctionRefers((i, j) => i >= j)
                   .SetEquivalenceClassSource(equivalenceClassSource)
                   .ToPattern();

                // Is "Less Than" operator overload implementation OK?
                yield return new ComparisonPatternBuilder<TTarget, bool>()
                    .SetName("OperatorLessThan")
                    .SetSignatureDescription(String.Format("static bool operator <({0}, {0})", typeof(TTarget).Name))
                    .SetComparisonMethodInfo(typeof(TTarget).GetMethod("op_LessThan", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static, null, new Type[] { typeof(TTarget), typeof(TTarget) }, null))
                    .SetFunctionRefers((i, j) => i < j)
                    .SetEquivalenceClassSource(equivalenceClassSource)
                    .ToPattern();

                // Is "Less Than Or Equal" operator overload implementation OK?
                yield return new ComparisonPatternBuilder<TTarget, bool>()
                    .SetName("OperatorLessThanOrEqual")
                    .SetSignatureDescription(String.Format("static bool operator <=({0}, {0})", typeof(TTarget).Name))
                    .SetComparisonMethodInfo(typeof(TTarget).GetMethod("op_LessThanOrEqual", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static, null, new Type[] { typeof(TTarget), typeof(TTarget) }, null))
                    .SetFunctionRefers((i, j) => i <= j)
                    .SetEquivalenceClassSource(equivalenceClassSource)
                    .ToPattern();
            }
        }

        private Type GetIComparableInterface()
        {
            return GetInterface(typeof(TTarget), typeof(IComparable<>)
                .MakeGenericType(typeof(TTarget)));
        }
    }
}
