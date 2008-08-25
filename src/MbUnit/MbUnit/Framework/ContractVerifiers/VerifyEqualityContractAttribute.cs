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

namespace MbUnit.Framework.ContractVerifiers
{
    /// <summary>
    /// <para>
    /// Attribute for test fixtures that verify the implementation 
    /// contract of a type implementing the generic <see cref="T:IEquatable"/> interface. 
    /// </para>
    /// <para>
    /// The test fixture must implement the <see cref="T:IEquivalenceClassProvider"/> interface 
    /// which provides a set of equivalence classes of distinct object instances to be used by
    /// the contract verifier.
    /// </para>
    /// <para>
    /// By default, the verifier will evaluated the behavior of the <see cref="Object.Equals(object)"/>,
    /// <see cref="Object.GetHashCode"/>, and <see cref="T:IEquatable.Equals"/> methods. It will
    /// verify as well the good implementation of the equality (==) and the inequality (!=) operator overloads.
    /// Use the named parameters <see cref="VerifyEqualityContractAttribute.ImplementsOperatorOverloads"/>
    /// to disable that verification.
    /// </para>
    /// <example>
    /// <para>
    /// The following example declares a simple equatable "Foo" class and tests it using the equality
    /// contract verifier. The Foo class has a contructor which takes two Int32 arguments. 
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
    ///         return (((object)left == null) && ((object)right == null)) ||
    ///            (((object)left != null) && left.Equals(right));
    ///     }
    ///     
    ///     public static bool operator !=(Foo left, Foo right) 
    ///     { 
    ///         return !(left == right);
    ///     }
    /// }
    /// 
    /// [VerifyEqualityContract(typeof(Foo), ImplementsOperatorOverloads = true)]
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
    /// </summary>
    [CLSCompliant(false)]
    [AttributeUsage(PatternAttributeTargets.TestType, AllowMultiple = false, Inherited = true)]
    public class VerifyEqualityContractAttribute : VerifyContractAttribute
    {
        /// <summary>
        /// <para>
        /// Determines whether the verifier will evaluate the presence and the 
        /// behavior of the equality and the inequality operator overloads.
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
        /// Gets the type of the object to verify. The type must implement
        /// the generic <see cref="IEquatable{T}"/> interface.
        /// </summary>
        public Type Type
        {
            get;
            private set;
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
        /// <param name="type">the type of the object to verify. The type must implement
        /// the generic <see cref="IEquatable{T}"/> interface.</param>
        public VerifyEqualityContractAttribute(Type type)
            : base("EqualityContract")
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            
            this.ImplementsOperatorOverloads = true;
            this.Type = type;
        }

        /// <inheritdoc />
        protected override void Validate(PatternEvaluationScope scope, ICodeElementInfo codeElement)
        {
            base.Validate(scope, codeElement);

            if (GetIEquatableInterface() == null)
                ThrowUsageErrorException("The specified type must implement the generic 'System.IEquatable<T>' interface.");
        }

        /// <inheritdoc />
        protected override void AddContractTests(PatternEvaluationScope scope)
        {
            AddObjectEqualsTest(scope);
            AddObjectGetHashCodeTest(scope);
            AddEquatableEqualsTest(scope);

            if (ImplementsOperatorOverloads)
            {
                AddOperatorEqualsTest(scope);
                AddOperatorNotEqualsTest(scope);
            }
        }

        /// <summary>
        /// Verifies the implementation and the behavior of <see cref="Object.Equals(object)" />.
        /// </summary>
        /// <param name="scope">The pattern evaluation scope.</param>
        private void AddObjectEqualsTest(PatternEvaluationScope scope)
        {
            AddContractTest(
                scope,
                "ObjectEquals",
                "Verify the implementation of 'Object.Equals()' on the type '" + Type.FullName + "'.",
                state =>
                {
                    VerifyEqualityContract(state.FixtureType, state.FixtureInstance, false,
                        (a, b) =>
                        {
                            return ((object)a).Equals((object)b);
                        });
                });
        }

        /// <summary>
        /// Verifies the implementation and the behavior of <see cref="Object.GetHashCode" />.
        /// </summary>
        /// <param name="scope">The pattern evaluation scope.</param>
        private void AddObjectGetHashCodeTest(PatternEvaluationScope scope)
        {
            AddContractTest(
                scope,
                "ObjectGetHashCode",
                "Verify the implementation of 'Object.GetHashCode()' on the type '" + Type.FullName + "'.",
                state =>
                {
                    IEnumerator enumerator1 = GetEquivalentClasses(state.FixtureType, state.FixtureInstance).GetEnumerator();
                    IEnumerator enumerator2 = GetEquivalentClasses(state.FixtureType, state.FixtureInstance).GetEnumerator();

                    while (enumerator1.MoveNext() && enumerator2.MoveNext())
                    {
                        foreach (object x in (IEnumerable)enumerator1.Current)
                            foreach (object y in (IEnumerable)enumerator2.Current)
                                Assert.AreEqual(x.GetHashCode(), y.GetHashCode(),
                                    "Hash code should return the same value for '{0}' and '{1}'.", x, y);
                    }
                });
        }

        /// <summary>
        /// Verifies the implementation and the behavior of <see cref="IEquatable{T}.Equals" />.
        /// </summary>
        /// <param name="scope">The pattern evaluation scope.</param>
        private void AddEquatableEqualsTest(PatternEvaluationScope scope)
        {
            AddContractTest(
                scope,
                "EquatableEquals",
                "Verify the implementation of 'IEquatable<T>.Equals()' on the type '" + Type.FullName + "'.",
                state =>
                {
                    MethodInfo equals = GetIEquatableInterface().GetMethod("Equals",
                        BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance,
                        null, new Type[] { Type }, null);
                    Assert.IsNotNull(equals, "The type '" + Type.FullName + "' should implement the method 'bool Equals(" + Type.Name + ")'.");
      
                    VerifyEqualityContract(state.FixtureType, state.FixtureInstance, false,
                        (a, b) =>
                        {
                            return (bool)equals.Invoke(a, new object[] { b });
                        });
                });
        }

        /// <summary>
        /// Verifies the implementation and the behavior of the static equality operator overload.
        /// </summary>
        /// <param name="scope">The pattern evaluation scope.</param>
        private void AddOperatorEqualsTest(PatternEvaluationScope scope)
        {
            AddContractTest(
                scope,
                "OperatorEquals",
                "Verify the implementation of the equality operator (==) overload on the type '" + Type.FullName + "'.",
                state =>
                {
                    MethodInfo @operator = Type.GetMethod("op_Equality",
                        BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static,
                        null, new Type[] { Type, Type }, null);
                    Assert.IsNotNull(@operator, "The type '" + Type.FullName + "' should implement the static equality operator (==) overload.");

                    VerifyEqualityContract(state.FixtureType, state.FixtureInstance, true,
                        (a, b) =>
                        {
                            return (bool)@operator.Invoke(null, new object[] { a, b });
                        });
                });
        }

        /// <summary>
        /// Verifies the implementation and the behavior of the static inequality operator overload.
        /// </summary>
        /// <param name="scope">The pattern evaluation scope.</param>
        private void AddOperatorNotEqualsTest(PatternEvaluationScope scope)
        {
            AddContractTest(
                scope,
                "OperatorNotEquals",
                "Verify the implementation of the inequality operator (!=) overload on the type '" + Type.FullName + "'.",
                state =>
                {
                    MethodInfo @operator = Type.GetMethod("op_Inequality",
                        BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static,
                        null, new Type[] { Type, Type }, null);
                    Assert.IsNotNull(@operator, "The type '" + Type.FullName + "' should implement the static inequality operator (!=) overload.");

                    VerifyEqualityContract(state.FixtureType, state.FixtureInstance, true,
                        (a, b) =>
                        {
                            return !(bool)@operator.Invoke(null, new object[] { a, b });
                        });
                });
        }

        /// <summary>
        /// Casts the instance of the test fixture into a provider of equivalence classes, 
        /// then returns the resulting collection as an enumeration.
        /// </summary>
        /// <param name="fixtureType">The type of the fixture.</param>
        /// <param name="fixtureInstance">The fixture instance.</param>
        /// <returns></returns>
        protected IEnumerable GetEquivalentClasses(Type fixtureType, object fixtureInstance)
        {
            Type interfaceType = GetIEquivalenceClassProviderInterface(fixtureType);
            Assert.IsNotNull(interfaceType, "The equality contract verifier for the test fixture '{0}' must implement the interface 'IEquivalentClassProvider'.", fixtureType.ToString());
            return (IEnumerable)interfaceType.InvokeMember("GetEquivalenceClasses",
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.InvokeMethod,
                null, fixtureInstance, null);
        }

        /// <summary>
        /// Ensures that an equality operation is correctly implemented.
        /// </summary>
        /// <param name="fixtureType">The type of the fixture.</param>
        /// <param name="fixtureInstance">The fixture instance.</param>
        /// <param name="isStaticMethodInvoked">Indicates whether the equality operation 
        /// is based on the invocation of a static method (true) or an instance method (false).</param>
        /// <param name="equals">The equality operation.</param>
        protected void VerifyEqualityContract(Type fixtureType, object fixtureInstance, 
            bool isStaticMethodInvoked, Func<object, object, bool> equals)
        {
            VerifyEqualityBetweenTwoNullReferences(isStaticMethodInvoked, equals);
            int i = 0;

            foreach (object a in GetEquivalentClasses(fixtureType, fixtureInstance))
            {
                int j = 0;

                foreach (object b in GetEquivalentClasses(fixtureType, fixtureInstance))
                {
                    CompareEquivalentInstances((IEnumerable)a, (IEnumerable)b, i == j, isStaticMethodInvoked, equals);
                    j++;
                }

                i++;
            }
        }

        /// <summary>
        /// Evaluates the equivalence of two classes of object instances.
        /// </summary>
        /// <param name="a">The first class of equivalent object instances.</param>
        /// <param name="b">The second class of equivalent object instances.</param>
        /// <param name="equalityExpected">Indicates whether all the object instances which 
        /// belong to the two classes are expected to be equal or not.</param>
        /// <param name="isStaticMethodInvoked">Indicates whether the equality operation 
        /// is based on the invocation of a static method (true) or an instance method (false).</param>
        /// <param name="equals">The equality function used for the evaluation.</param>
        protected void CompareEquivalentInstances(IEnumerable a, IEnumerable b, 
            bool equalityExpected, bool isStaticMethodInvoked, Func<object, object, bool> equals)
        {
            foreach (object x in a)
            {
                Assert.IsTrue(equals(x, x), "The equality operator should consider '{0}' equal to itself.", x);
                VerifyNullReferenceEquality(x, isStaticMethodInvoked, equals);
                
                foreach (object y in b)
                {
                    if (equalityExpected)
                    {
                        Assert.IsTrue(equals(x, y), "The equality operator should consider '{0}' equal to '{1}'.", x, y);
                        Assert.IsTrue(equals(y, x), "The equality operator should condider '{0}' equal to '{1}'.", y, x);
                    }
                    else
                    {
                        Assert.IsFalse(equals(x, y), "The equality operator should consider '{0}' not equal to '{1}'.", x, y);
                        Assert.IsFalse(equals(y, x), "The equality operator should consider '{0}' not equal to '{1}'.", y, x);
                    }
                }
            }
        }

        private void VerifyEqualityBetweenTwoNullReferences(bool isStaticMethodInvoked, Func<object, object, bool> equals)
        {
            if (!Type.IsValueType && isStaticMethodInvoked)
            {
                try
                {
                    Assert.IsTrue(equals(null, null), "The equality operator should consider two null references equal.");
                }
                catch (TargetInvocationException)
                {
                    Assert.Fail("The equality operator should consider two null references equal.");
                }
                catch (NullReferenceException)
                {
                    Assert.Fail("The equality operator should consider two null references equal.");
                }
            }
        }

        private void VerifyNullReferenceEquality(object x, bool isStaticMethodInvoked, Func<object, object, bool> equals)
        {
            if (!Type.IsValueType)
            {
                try
                {
                    Assert.IsFalse(equals(x, null), "Equality operator should consider '{0}' not equal to a null reference.", x);
                    
                    if (isStaticMethodInvoked)
                        Assert.IsFalse(equals(null, x), "Equality operator should consider '{0}' not equal to a null reference.", x);
                }
                catch (TargetInvocationException)
                {
                    Assert.Fail("Equality operator should consider '{0}' not equal to a null reference.", x);
                }
                catch (NullReferenceException)
                {
                    Assert.Fail("Equality operator should consider '{0}' not equal to a null reference.", x);
                }
            }
        }

        private Type GetIEquatableInterface()
        {
            return GetInterface(Type, typeof(IEquatable<>).MakeGenericType(Type));
        }

        private Type GetIEquivalenceClassProviderInterface(Type fixtureType)
        {
            return GetInterface(fixtureType, typeof(IEquivalenceClassProvider<>).MakeGenericType(Type));
        }
    }
}
