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
using MbUnit.Framework.ContractVerifiers.Patterns.Equality;
using MbUnit.Framework.ContractVerifiers.Patterns.HasAttribute;
using MbUnit.Framework.ContractVerifiers.Patterns.ObjectHashCode;

namespace MbUnit.Framework.ContractVerifiers
{
    /// <summary>
    /// <para>
    /// Field-based contract verifier for the implementation of
    /// the generic <see cref="IEquatable{T}"/> interface. 
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
    /// the <see cref="VerifyEqualityContract{T}.ImplementsOperatorOverloads"/> 
    /// property to <code>false</code>.</description>
    /// </item>
    /// <item>
    /// <term>OperatorNotEquals</term>
    /// <description>The type has a static inequality operator (!=) overload which 
    /// behaves correctly against the provided equivalence classes. Disable that test by 
    /// setting the <see cref="VerifyEqualityContract{T}.ImplementsOperatorOverloads"/> 
    /// property to <code>false</code>.
    /// </description>
    /// </item>
    /// </list>
    /// </para>
    /// <example>
    /// The following example shows a simple class implementing the 
    /// <see cref="IEquatable{T}"/> interface, and a test fixture which uses the
    /// equality contract verifier to test it.
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
    ///     [ContractVerifier]
    ///     public readonly IContractVerifier EqualityTests = new EqualityContractVerifier<SampleEquatable>()
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
    public class VerifyEqualityContract<TTarget> : AbstractContractVerifier
        where TTarget : IEquatable<TTarget>
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public VerifyEqualityContract()
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
        public override IEnumerable<ContractVerifierPattern> GetContractPatterns()
        {
            var equivalenceClassSource = GetType().GetProperty("EquivalenceClasses", BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.Instance);

            // Is Object equality method OK?
            yield return new EqualityPatternBuilder<TTarget>()
                .SetName("ObjectEquals")
                .SetSignatureDescription("bool Equals(Object)")
                .SetEqualityMethodInfo(typeof(TTarget).GetMethod("Equals", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(object) }, null))
                .SetEquivalenceClassSource(equivalenceClassSource)
                .ToPattern();

            // Is Object hash code calculcation well implemented?
            yield return new ObjectHashCodePatternBuilder<TTarget>()
                .SetEquivalenceClassSource(equivalenceClassSource)
                .ToPattern();

            // Is IEquatable equality method OK?
            yield return new EqualityPatternBuilder<TTarget>()
                .SetName("EquatableEquals")
                .SetSignatureDescription(String.Format("bool Equals({0})", typeof(TTarget).Name))
                .SetEqualityMethodInfo(GetIEquatableInterface().GetMethod("Equals", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(TTarget) }, null))
                .SetEquivalenceClassSource(equivalenceClassSource)
                .ToPattern();

            if (ImplementsOperatorOverloads)
            {
                // Is equality operator overload OK?
                yield return new EqualityPatternBuilder<TTarget>()
                    .SetName("OperatorEquals")
                    .SetSignatureDescription(String.Format("static bool operator ==({0}, {0})", typeof(TTarget).Name))
                    .SetEqualityMethodInfo(typeof(TTarget).GetMethod("op_Equality", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static, null, new Type[] { typeof(TTarget), typeof(TTarget) }, null))
                    .SetEquivalenceClassSource(equivalenceClassSource)
                    .ToPattern();

                // Is inequality operator overload OK?
                yield return new EqualityPatternBuilder<TTarget>()
                    .SetName("OperatorNotEquals")
                    .SetSignatureDescription(String.Format("static bool operator !=({0}, {0})", typeof(TTarget).Name))
                    .SetEqualityMethodInfo(typeof(TTarget).GetMethod("op_Inequality", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static, null, new Type[] { typeof(TTarget), typeof(TTarget) }, null))
                    .SetInequality(true)
                    .SetEquivalenceClassSource(equivalenceClassSource)
                    .ToPattern();
            }
        }

        private Type GetIEquatableInterface()
        {
            return GetInterface(typeof(TTarget), typeof(IEquatable<>)
                .MakeGenericType(typeof(TTarget)));
        }
    }
}
