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
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Gallio;
using Gallio.Collections;
using Gallio.Framework.Data;
using Gallio.Framework.Pattern;
using Gallio.Model;
using Gallio.Reflection;
using Gallio.Framework.Assertions;
using MbUnit.Framework.ContractVerifiers.Patterns;
using MbUnit.Framework.ContractVerifiers.Patterns.ObjectHashCode;
using MbUnit.Framework.ContractVerifiers.Patterns.Equality;

namespace MbUnit.Framework.ContractVerifiers
{
    /// <summary>
    /// <para>
    /// Attribute for test fixtures that verify the implementation 
    /// contract of a type implementing the generic <see cref="IEquatable{T}"/> interface. 
    /// </para>
    /// <para>
    /// The test fixture must implement the <see cref="IEquivalenceClassProvider{T}"/> interface 
    /// which provides a set of equivalence classes of distinct object instances to be used by
    /// the contract verifier.
    /// </para>
    /// <para>
    /// By default, the verifier will evaluated the behavior of the <see cref="Object.Equals(object)"/>,
    /// <see cref="Object.GetHashCode"/>, and <see cref="IEquatable{T}.Equals"/> methods. It will
    /// verify as well the good implementation of the equality (==) and the inequality (!=) operator overloads.
    /// Use the named parameters <see cref="VerifyEqualityContractAttribute.ImplementsOperatorOverloads"/>
    /// to disable that verification.
    /// </para>
    /// <example>
    /// <para>
    /// The following example declares a simple equatable "Foo" class and tests it using the equality
    /// contract verifier. The "Foo" class has a contructor which takes two Int32 arguments. 
    /// The equality contract is based on the result of the multiplication of those two arguments.
    /// Thus, Foo(25, 2) should be equal to (5, 10) because 25 * 2 = 5 * 10.
    /// <code><![CDATA[
    /// public class Foo : IEquatable<Foo>
    /// {
    ///     private int value;
    ///     
    ///     public Foo(int left, int right)
    ///     {
    ///         value = left * right;
    ///     }
    /// 
    ///     public override int GetHashCode() 
    ///     {
    ///         return value.GetHashCode(); 
    ///     }
    ///     
    ///     public override bool Equals(object other) 
    ///     { 
    ///         return Equals(other as Foo);
    ///     }
    ///     
    ///     public bool Equals(Foo other) 
    ///     { 
    ///         return (other != null) && (value == other.value);
    ///     }
    /// 
    ///     public static bool operator ==(Foo left, Foo right) 
    ///     {
    ///         return (Object.ReferenceEquals(left, null) 
    ///             && Object.ReferenceEquals(right, null)) 
    ///             || (!Object.ReferenceEquals(left, null) 
    ///             && left.Equals(right));
    ///     }
    ///     
    ///     public static bool operator !=(Foo left, Foo right) 
    ///     { 
    ///         return !(left == right);
    ///     }
    /// }
    /// 
    /// [VerifyEqualityContract(typeof(Foo)]
    /// public class FooTest : IEquivalenceClassProvider<Foo>
    /// {
    ///     public EquivalenceClassCollection<Foo> GetEquivalenceClasses()
    ///     {
    ///         return new EquivalenceClassCollection<Foo>(
    ///             new EquivalenceClass<Foo>(
    ///                 new Foo(7, 2));
    ///             new EquivalenceClass<Foo>(
    ///                 new Foo(25, 2), 
    ///                 new Foo(10, 5));
    ///             new EquivalenceClass<Foo>(
    ///                 new Foo(3, 4), 
    ///                 new Foo(2, 6), 
    ///                 new Foo(1, 12)));
    ///     }
    /// }
    /// ]]></code>
    /// </para>
    /// </example>
    /// <para>
    /// When testing a nullable type such as a reference type, or a value type decorated 
    /// with <see cref="Nullable{T}"/>, it is not necessary to provide a null reference as an
    /// object instance to the constructor of the equivalence classes. 
    /// The contract verifier will check for you that the tested type handles correctly 
    /// with null references. In the scope of the equality contract, it means that:
    /// <list type="bullet">
    /// <item>Any null reference should be not equal to any non-null reference.</item>
    /// <item>Two null references should be equal together.</item>
    /// </list>
    /// </para>
    /// </summary>
    [CLSCompliant(false)]
    [AttributeUsage(PatternAttributeTargets.TestType, AllowMultiple = false, Inherited = true)]
    public class VerifyEqualityContractAttribute : VerifyContractAttribute
    {
        /// <summary>
        /// <para>
        /// Determines whether the verifier will evaluate the presence and the 
        /// behavior of the equality and the inequality operator overloads.
        /// The default value is 'true'.
        /// </para>
        /// Built-in verifications:
        /// <list type="bullet">
        /// <item>The type has a static equality operator (==) overload which 
        /// behaves correctly against the provided equivalence classes.</item>
        /// <item>The type has a static inequality operator (!=) overload which 
        /// behaves correctly against the provided equivalence classes.</item>
        /// </list>
        /// </summary>
        public bool ImplementsOperatorOverloads
        {
            get;
            set;
        }

        /// <summary>
        /// <para>
        /// Attribute for test fixtures that verify the implementation 
        /// contract of a type implementing the generic <see cref="IEquatable{T}"/> interface. 
        /// </para>
        /// <para>
        /// The test fixture must implement the <see cref="IEquivalenceClassProvider{T}"/> interface 
        /// which provides a set of equivalence classes of object instances to be used by
        /// the contract verifier.
        /// </para>
        /// <para>
        /// By default, the verifier will evaluated the behavior of the <see cref="Object.Equals(object)"/>,
        /// <see cref="Object.GetHashCode"/>, and <see cref="IEquatable{T}.Equals"/> methods. It will
        /// verify as well the good implementation of the equality (==) and the inequality (!=) operator overloads.
        /// Use the named parameters <see cref="VerifyEqualityContractAttribute.ImplementsOperatorOverloads"/>
        /// to disable that verification.
        /// </para>
        /// </summary>
        /// <param name="targetType">the type of the object to verify. The type must implement
        /// the generic <see cref="IEquatable{T}"/> interface.</param>
        public VerifyEqualityContractAttribute(Type targetType)
            : base("EqualityContract", targetType)
        {
            this.ImplementsOperatorOverloads = true;
        }

        /// <inheritdoc />
        protected override void Validate(PatternEvaluationScope scope, ICodeElementInfo codeElement)
        {
            base.Validate(scope, codeElement);

            if (GetIEquatableInterface() == null)
                ThrowUsageErrorException("The specified type must implement the generic 'System.IEquatable<T>' interface.");
        }

        /// <inheritdoc />
        protected override IEnumerable<ContractVerifierPattern> GetPatterns()
        {
            // Is Object equality method OK?
            yield return new EqualityPatternBuilder()
                .SetTargetType(TargetType)
                .SetName("ObjectEquals")
                .SetSignatureDescription("bool Equals(Object)")
                .SetEqualityMethodInfo(TargetType.GetMethod("Equals", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(object) }, null))
                .ToPattern();
            
            // Is Object hash code calculcation well implemented?
            yield return new ObjectHashCodePatternBuilder()
                .SetTargetType(TargetType)
                .ToPattern();

            // Is IEquatable equality method OK?
            yield return new EqualityPatternBuilder()
                .SetTargetType(TargetType)
                .SetName("EquatableEqual")
                .SetSignatureDescription(String.Format("bool Equals({0})", TargetType.Name))
                .SetEqualityMethodInfo(GetIEquatableInterface().GetMethod("Equals", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { TargetType }, null))
                .ToPattern();

            if (ImplementsOperatorOverloads)
            {
                // Is equality operator overload OK?
                yield return new EqualityPatternBuilder()
                    .SetTargetType(TargetType)
                    .SetName("OperatorEquals")
                    .SetSignatureDescription(String.Format("static bool operator ==({0}, {0})", TargetType.Name))
                    .SetEqualityMethodInfo(TargetType.GetMethod("op_Equality", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static, null, new Type[] { TargetType, TargetType }, null))
                    .ToPattern();

                // Is inequality operator overload OK?
                yield return new EqualityPatternBuilder()
                   .SetTargetType(TargetType)
                   .SetName("OperatorNotEquals")
                   .SetSignatureDescription(String.Format("static bool operator !=({0}, {0})", TargetType.Name))
                   .SetEqualityMethodInfo(TargetType.GetMethod("op_Inequality", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static, null, new Type[] { TargetType, TargetType }, null))
                   .SetInequality(true)
                   .ToPattern();
            }
        }

        private Type GetIEquatableInterface()
        {
            return GetInterface(TargetType, typeof(IEquatable<>)
                .MakeGenericType(TargetType));
        }
    }
}
