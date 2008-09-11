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
using System.Reflection;
using Gallio;
using Gallio.Framework.Pattern;
using Gallio.Reflection;
using Gallio.Framework.Assertions;
using MbUnit.Framework.ContractVerifiers.Patterns;
using System.Collections.Generic;

namespace MbUnit.Framework.ContractVerifiers
{
    /// <summary>
    /// <para>
    /// Attribute for test fixtures that verify the implementation 
    /// contract of a type implementing the generic <see cref="IComparable{T}"/> interface. 
    /// </para>
    /// <para>
    /// The test fixture must implement the <see cref="IEquivalenceClassProvider{T}"/> interface 
    /// which provides a set of equivalence classes of distinct object instances to be used by
    /// the contract verifier.
    /// </para>
    /// <para>
    /// By default, the verifier will evaluate the behavior of the 
    /// <see cref="IComparable{T}.CompareTo"/> method. It will verify as well the good 
    /// implementation of the four comparison operator overloads (Greater Than, Less Than, 
    /// Greater Than Or Equal, and Less Than Or Equal). Use the named parameters 
    /// <see cref="VerifyEqualityContractAttribute.ImplementsOperatorOverloads"/>
    /// to disable that verification.
    /// </para>
    /// <example>
    /// <para>
    /// The following example declares a simple comparable "Foo" class and tests it using the comparison
    /// contract verifier. The Foo class has a contructor which takes one single Int32 argument, which
    /// is used internally by the class for the implementation of the comparison contract.
    /// <code><![CDATA[
    /// public class Foo : IComparable<Foo>
    /// {
    ///     private int value;
    ///     
    ///     public Foo(int value)
    ///     {
    ///         this.value = value;
    ///     }
    /// 
    ///     public int CompareTo(Foo other) 
    ///     { 
    ///         return (other == null) ? Int32.MaxValue : value.CompareTo(other.value);
    ///     }
    /// 
    ///     public static bool operator >=(Foo left, Foo right)
    ///     {
    ///         return ((left == null) && (right == null)) || ((left != null) && (left.CompareTo(right) >= 0));
    ///     }
    ///
    ///     public static bool operator <=(Foo left, Foo right)
    ///     {
    ///         return (left == null) || (left.CompareTo(right) <= 0);
    ///     }
    ///
    ///     public static bool operator >(Foo left, Foo right)
    ///     {
    ///         return (left != null) && (left.CompareTo(right) > 0);
    ///     }
    ///
    ///     public static bool operator <(Foo left, Foo right)
    ///     {
    ///         return ((left != null) || (right != null)) && ((left == null) || (left.CompareTo(right) < 0));
    ///     }
    /// }
    /// 
    /// [VerifyComparisonContract(typeof(Foo))]
    /// public class FooTest : IEquivalenceClassProvider<Foo>
    /// {
    ///     public EquivalenceClassCollection<Foo> GetEquivalenceClasses()
    ///     {
    ///         return EquivalenceClassCollection<Foo>.FromDistinctInstances(
    ///             new Foo(1),
    ///             new Foo(2),
    ///             new Foo(5),
    ///             new Foo(36));
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
    /// with null references. In the scope of the comparison contract, it means that:
    /// <list type="bullet">
    /// <item>Any null reference should compare less than any non-null reference.</item>
    /// <item>Two null references should compare equal.</item>
    /// </list>
    /// </para>
    /// </summary>
    [CLSCompliant(false)]
    [AttributeUsage(PatternAttributeTargets.TestType, AllowMultiple = false, Inherited = true)]
    public class VerifyComparisonContractAttribute : VerifyContractAttribute
    {
        /// <summary>
        /// <para>
        /// Determines whether the verifier will evaluate the presence and the 
        /// behavior of the four comparison operator overloads. 
        /// The default value is 'true'.
        /// </para>
        /// Built-in verifications:
        /// <list type="bullet">
        /// <item>The type has a static "greater than" operator overload which 
        /// behaves correctly against the provided equivalence classes.</item>
        /// <item>The type has a static "less than" operator overload which 
        /// behaves correctly against the provided equivalence classes.</item>
        /// <item>The type has a static "greater than or equal" operator overload which 
        /// behaves correctly against the provided equivalence classes.</item>
        /// <item>The type has a static "less than or equal" operator overload which 
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
        /// contract of a type implementing the generic <see cref="IComparable{T}"/> interface. 
        /// </para>
        /// <para>
        /// The test fixture must implement the <see cref="IEquivalenceClassProvider{T}"/> interface 
        /// which provides a set of equivalence classes of object instances to be used by
        /// the contract verifier.
        /// </para>
        /// <para>
        /// By default, the verifier will evaluated the behavior of the 
        /// <see cref="IComparable{T}.CompareTo"/> method. It will verify as well the good 
        /// implementation of the four comparison operator overloads (greater than, less than, 
        /// greater than or equal, and less than or equal). Use the named parameters 
        /// <see cref="VerifyEqualityContractAttribute.ImplementsOperatorOverloads"/>
        /// to disable that verification.
        /// </para>
        /// </summary>
        /// <param name="targetType">the type of the object to verify. The type must implement
        /// the generic <see cref="IComparable{T}"/> interface</param>
        public VerifyComparisonContractAttribute(Type targetType)
            : base("ComparisonContract", targetType)
        {
            this.ImplementsOperatorOverloads = true;
        }

        /// <inheritdoc />
        protected override void Validate(PatternEvaluationScope scope, ICodeElementInfo codeElement)
        {
            base.Validate(scope, codeElement);

            if (GetIComparableInterface() == null)
                ThrowUsageErrorException("The specified type must implement the generic 'System.IComparable<T>' interface.");
        }

        /// <inheritdoc />
        protected override IEnumerable<PatternTestBuilder> GetPatternTestBuilders()
        {
            // Is IComparable.CompareTo implementation OK?
            yield return new PatternTestBuilderCompares<int>(
                TargetType, "ComparableCompareTo",
                "public bool CompareTo(" + TargetType.Name + ")",
                GetIComparableInterface().GetMethod("CompareTo", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance, null, new Type[] { TargetType }, null),
                (i, j) => i.CompareTo(j),
                x => (x == 0) ? "0" : ((x > 0) ? "A Positive Value" : "A Negative Value"),
                x => Math.Sign(x));

            if (ImplementsOperatorOverloads)
            {
                // Is "Greater Than" operator overload implementation OK?
                yield return new PatternTestBuilderCompares<bool>(
                    TargetType, "OperatorGreaterThan",
                    "static bool operator >(" + TargetType.Name + ", " + TargetType.Name + ")",
                    TargetType.GetMethod("op_GreaterThan", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static, null, new Type[] { TargetType, TargetType }, null),
                    (i, j) => i > j);

                // Is "Greater Than Or Equal" operator overload implementation OK?
                yield return new PatternTestBuilderCompares<bool>(
                    TargetType, "OperatorGreaterThanOrEqual",
                    "static bool operator >=(" + TargetType.Name + ", " + TargetType.Name + ")",
                    TargetType.GetMethod("op_GreaterThanOrEqual", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static, null, new Type[] { TargetType, TargetType }, null),
                    (i, j) => i >= j);

                // Is "Less Than" operator overload implementation OK?
                yield return new PatternTestBuilderCompares<bool>(
                    TargetType, "OperatorLessThan",
                    "static bool operator <(" + TargetType.Name + ", " + TargetType.Name + ")",
                    TargetType.GetMethod("op_LessThan", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static, null, new Type[] { TargetType, TargetType }, null),
                    (i, j) => i < j);

                // Is "Less Than Or Equal" operator overload implementation OK?
                yield return new PatternTestBuilderCompares<bool>(
                    TargetType, "OperatorLessThanOrEqual",
                    "static bool operator <=(" + TargetType.Name + ", " + TargetType.Name + ")",
                    TargetType.GetMethod("op_LessThanOrEqual", BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static, null, new Type[] { TargetType, TargetType }, null),
                    (i, j) => i <= j);
            }
        }

        private Type GetIComparableInterface()
        {
            return GetInterface(TargetType, typeof(IComparable<>).MakeGenericType(TargetType));
        }
    }
}
