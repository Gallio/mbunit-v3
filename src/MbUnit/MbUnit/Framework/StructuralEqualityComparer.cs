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
using System.Collections.Generic;
using System.Text;
using Gallio.Common;
using Gallio.Framework;
using System.Collections;

namespace MbUnit.Framework
{
    /// <summary>
    /// A general-purpose structural equality comparer that defines a fully customizable equality operation without the 
    /// need to implement <see cref="IEquatable{T}"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// That equality comparer can be used in any MbUnit assertion that takes an <see cref="IEqualityComparer{T}"/> 
    /// object as argument, such as <see cref="Assert.AreEqual{T}(T, T, IEqualityComparer{T})"/>.
    /// </para>
    /// <para>
    /// The comparer must be initialized with a list of one or several matching criteria. Two instances are considered
    /// equal by the comparer if and only if all the criteria are true for that pair of instances.
    /// </para>
    /// </remarks>
    /// <example>
    /// The following example shows a test fixture that checks for the equality between two <strong>Foo</strong> 
    /// objects by using the well known <see cref="Assert.AreEqual{T}(T, T, IEqualityComparer{T})"/> assertion. The custom equality comparer which 
    /// is provided to the assertion method, declares two <strong>Foo</strong> objects equal when the <strong>Value</strong> 
    /// fields have the same parity, and when the <strong>Text</strong> fields are equal (case insensitive):
    /// <code><![CDATA[
    /// public class Foo
    /// {
    ///     public int Value;
    ///     public string Text;
    /// }
    /// 
    /// [TestFixture]
    /// public class FooTest
    /// {
    ///     [Test]
    ///     public void MyTest()
    ///     {
    ///         var foo1 = new Foo() { Value = 123, Text = "Hello" };    
    ///         var foo2 = new Foo() { Value = 789, Text = "hElLo" };
    ///         
    ///         Assert.AreEqual(foo1, foo2, new StructuralEqualityComparer<Foo>
    ///         {
    ///             { x => x.Value, (x, y) => x % 2 == y % 2 },
    ///             { x => x.Text, (x, y) => String.Compare(x, y, true) == 0 }
    ///         });
    ///     }
    /// }
    /// ]]></code>
    /// </example>
    /// <typeparam name="T">The type of the objects. to compare.</typeparam>
    public class StructuralEqualityComparer<T> : IEqualityComparer<T>, IEnumerable<EqualityComparison<T>>
    {
        private readonly List<EqualityComparison<T>> conditions = new List<EqualityComparison<T>>();

        /// <summary>
        /// Gets a default neutral structural equality comparer for the tested type.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The default comparer uses <see cref="Object.Equals(Object)"/> to determine whether
        /// two objects are equal. Is is usually sufficient for primitive types, and
        /// user types implementing <see cref="IEquatable{T}"/>.
        /// </para>
        /// </remarks>
        /// <seealso cref="ComparisonSemantics.Equals"/>
        public static StructuralEqualityComparer<T> Default
        {
            get
            {
                return new StructuralEqualityComparer<T> { { x => x, ComparisonSemantics.Equals<T> } };
            }
        }

        /// <summary>
        /// Adds a matching criterion to the structural equality comparer.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The values returned by the accessor are compared by using a default comparison evaluator.
        /// </para>
        /// </remarks>
        /// <example>
        /// <code><![CDATA[
        /// public class Foo
        /// {
        ///     public int Value;
        /// }
        /// 
        /// [TestFixture]
        /// public class FooTest
        /// {
        ///     [Test]
        ///     public void MyTest()
        ///     {
        ///         var foo1 = new Foo() { Value = 123 };    
        ///         var foo2 = new Foo() { Value = 123 };
        ///         
        ///         Assert.AreEqual(foo1, foo2, new StructuralEqualityComparer<Foo>
        ///         {
        ///             { x => x.Value },
        ///         });
        ///     }
        /// }
        /// ]]></code>
        /// </example>
        /// <typeparam name="TValue">The type of the value returned by the accessor.</typeparam>
        /// <param name="accessor">An accessor that gets a value from the tested object.</param>
        /// <exception cref="ArgumentNullException">The specified accessor argument is a null reference.</exception>
        public void Add<TValue>(Accessor<T, TValue> accessor)
        {
            Add<TValue>(accessor, StructuralEqualityComparer<TValue>.Default);
        }

        /// <summary>
        /// Adds a matching criterion to the structural equality comparer.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The values returned by the accessor are compared by using the specified comparer object.
        /// </para>
        /// </remarks>
        /// <example>
        /// <code><![CDATA[
        /// public class Foo
        /// {
        ///     public int Value;
        /// }
        /// 
        /// public class MyComparer : IEqualityComparer<int>
        /// {
        ///     public bool Equals(int x, int y)
        ///     {
        ///         return x == y;    
        ///     }
        ///     
        ///     public int GetHashCode(int obj)
        ///     {
        ///         return obj;
        ///     }
        /// }
        /// 
        /// [TestFixture]
        /// public class FooTest
        /// {
        ///     [Test]
        ///     public void MyTest()
        ///     {
        ///         var foo1 = new Foo() { Value = 123 };    
        ///         var foo2 = new Foo() { Value = 123 };
        ///         
        ///         Assert.AreEqual(foo1, foo2, new StructuralEqualityComparer<Foo>
        ///         {
        ///             { x => x.Value, new MyComparer() },
        ///         });
        ///     }
        /// }
        /// ]]></code>
        /// </example>
        /// <typeparam name="TValue">The type of the value returned by the accessor.</typeparam>
        /// <param name="accessor">An accessor that gets a value from the tested object.</param>
        /// <param name="comparer">A comparer instance, or null to use the default one (<see cref="StructuralEqualityComparer{T}.Default"/>).</param>
        /// <exception cref="ArgumentNullException">The specified accessor argument is a null reference.</exception>
        public void Add<TValue>(Accessor<T, TValue> accessor, IEqualityComparer<TValue> comparer)
        {
            if (comparer == null)
                comparer = StructuralEqualityComparer<TValue>.Default;

            Add<TValue>(accessor, comparer.Equals);
        }

        /// <summary>
        /// Adds a matching criterion to the structural equality comparer.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The values returned by the accessor are compared by using the specified comparison delegate.
        /// </para>
        /// </remarks>
        /// <example>
        /// <code><![CDATA[
        /// public class Foo
        /// {
        ///     public int Value;
        /// }
        /// 
        /// [TestFixture]
        /// public class FooTest
        /// {
        ///     [Test]
        ///     public void MyTest()
        ///     {
        ///         var foo1 = new Foo() { Value = 123 };    
        ///         var foo2 = new Foo() { Value = 123 };
        ///         
        ///         Assert.AreEqual(foo1, foo2, new StructuralEqualityComparer<Foo>
        ///         {
        ///             { x => x.Value, (x, y) => x == y },
        ///         });
        ///     }
        /// }
        /// ]]></code>
        /// </example>
        /// <typeparam name="TValue">The type of the value returned by the accessor.</typeparam>
        /// <param name="accessor">An accessor that gets a value from the tested object.</param>
        /// <param name="comparer">A equality comparison delegate to compare the values returned by the accessor, or null to use the default one.</param>
        /// <exception cref="ArgumentNullException">The specified accessor argument is a null reference.</exception>
        public void Add<TValue>(Accessor<T, TValue> accessor, EqualityComparison<TValue> comparer)
        {
            if (accessor == null)
                throw new ArgumentNullException("accessor");

            if (comparer == null)
                comparer = ComparisonSemantics.Equals<TValue>;

            conditions.Add((x, y) => comparer(accessor(x), accessor(y)));
        }

        /// <summary>
        /// Adds a matching criterion to the structural equality comparer.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The evaluation process is done through the specified comparison delegate.
        /// </para>
        /// </remarks>
        /// <example>
        /// <code><![CDATA[
        /// public class Foo
        /// {
        ///     public int Value;
        /// }
        /// 
        /// [TestFixture]
        /// public class FooTest
        /// {
        ///     [Test]
        ///     public void MyTest()
        ///     {
        ///         var foo1 = new Foo() { Value = 123 };    
        ///         var foo2 = new Foo() { Value = 123 };
        ///         
        ///         Assert.AreEqual(foo1, foo2, new StructuralEqualityComparer<Foo>
        ///         {
        ///             { (x, y) => x.Value == y.Value },
        ///         });
        ///     }
        /// }
        /// ]]></code>
        /// </example>
        /// <param name="comparer">An equality comparison delegate to directly compare two instances, or null to use the default one.</param>
        public void Add(EqualityComparison<T> comparer)
        {
            conditions.Add(comparer ?? ComparisonSemantics.Equals<T>);
        }

        /// <summary>
        /// Adds a matching criterion to the structural equality comparer.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The evaluation process is done through the specified comparer object.
        /// </para>
        /// </remarks>
        /// <example>
        /// <code><![CDATA[
        /// public class Foo
        /// {
        ///     public int Value;
        /// }
        /// 
        /// public class MyComparer : IEqualityComparer<Foo>
        /// {
        ///     public bool Equals(Foo x, Foo y)
        ///     {
        ///         return x.Value == y.Value;    
        ///     }
        ///     
        ///     public int GetHashCode(int obj)
        ///     {
        ///         return obj;
        ///     }
        /// }
        /// 
        /// [TestFixture]
        /// public class FooTest
        /// {
        ///     [Test]
        ///     public void MyTest()
        ///     {
        ///         var foo1 = new Foo() { Value = 123 };    
        ///         var foo2 = new Foo() { Value = 123 };
        ///         
        ///         Assert.AreEqual(foo1, foo2, new StructuralEqualityComparer<Foo>
        ///         {
        ///             { new MyComparer() },
        ///         });
        ///     }
        /// }
        /// ]]></code>
        /// </example>
        /// <param name="comparer">An comparer object to directly compare two instances, or null to use the default one.</param>
        public void Add(IEqualityComparer<T> comparer)
        {
            if (comparer == null)
                comparer = StructuralEqualityComparer<T>.Default;

            Add(comparer.Equals);
        }

        /// <summary>
        /// Adds a matching criterion to the structural equality comparer.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The enumerations of values returned by the accessor are compared one by one, 
        /// by using the specified comparer.
        /// </para>
        /// </remarks>
        /// <example>
        /// <code><![CDATA[
        /// public class Foo
        /// {
        ///     public int[] Values;
        /// }
        /// 
        /// [TestFixture]
        /// public class FooTest
        /// {
        ///     [Test]
        ///     public void MyTest()
        ///     {
        ///         var foo1 = new Foo() { Values = new int[] { 1, 2, 3, 4, 5 } };    
        ///         var foo2 = new Foo() { Values = new int[] { 1, 2, 3, 4, 5 } };
        ///         
        ///         Assert.AreEqual(foo1, foo2, new StructuralEqualityComparer<Foo>
        ///         {
        ///             { x => x.Values,
        ///               new StructuralEqualityComparer<int> { { x => x } }
        ///             },
        ///         });
        ///     }
        /// }
        /// ]]></code>
        /// </example>
        /// <typeparam name="TValue">The type of the value returned by the accessor.</typeparam>
        /// <param name="accessor">An accessor that gets an enumeration of values from the tested object.</param>
        /// <param name="comparer">A comparer instance for the values returned by the accessor, or null to use the default one.</param>
        /// <exception cref="ArgumentNullException">The specified accessor argument is a null reference.</exception>
        public void Add<TValue>(Accessor<T, IEnumerable<TValue>> accessor, IEqualityComparer<TValue> comparer)
        {
            Add(accessor, comparer, StructuralEqualityComparerOptions.Default);
        }

        /// <summary>
        /// Adds a matching criterion to the structural equality comparer.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The enumerations of values returned by the accessor are compared by using the specified comparer.
        /// </para>
        /// </remarks>
        /// <example>
        /// <code><![CDATA[
        /// public class Foo
        /// {
        ///     public int[] Values;
        /// }
        /// 
        /// [TestFixture]
        /// public class FooTest
        /// {
        ///     [Test]
        ///     public void MyTest()
        ///     {
        ///         var foo1 = new Foo() { Values = new int[] { 1, 2, 3, 4, 5 } };    
        ///         var foo2 = new Foo() { Values = new int[] { 5, 4, 3, 2, 1 } };
        ///         
        ///         Assert.AreEqual(foo1, foo2, new StructuralEqualityComparer<Foo>
        ///         {
        ///             { x => x.Values,
        ///               new StructuralEqualityComparer<int> { { x => x } },
        ///               StructuralEqualityComparerOptions.IgnoreEnumerableOrder
        ///             },
        ///         });
        ///     }
        /// }
        /// ]]></code>
        /// </example>
        /// <typeparam name="TValue">The type of the value returned by the accessor.</typeparam>
        /// <param name="accessor">An accessor that gets an enumeration of values from the tested object.</param>
        /// <param name="comparer">A comparer instance for the values returned by the accessor, or null to use the default one.</param>
        /// <param name="options">Some options indicating how to compare the enumeration of values returned by the accessor.</param>
        /// <exception cref="ArgumentNullException">The specified accessor argument is a null reference.</exception>
        public void Add<TValue>(Accessor<T, IEnumerable<TValue>> accessor, IEqualityComparer<TValue> comparer, StructuralEqualityComparerOptions options)
        {
            Add(accessor, (comparer ?? StructuralEqualityComparer<TValue>.Default).Equals, options);
        }

        /// <summary>
        /// Adds a matching criterion to the structural equality comparer.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The enumerations of values returned by the accessor are compared by using the specified comparer.
        /// </para>
        /// </remarks>
        /// <example>
        /// <code><![CDATA[
        /// public class Foo
        /// {
        ///     public int[] Values;
        /// }
        /// 
        /// [TestFixture]
        /// public class FooTest
        /// {
        ///     [Test]
        ///     public void MyTest()
        ///     {
        ///         var foo1 = new Foo() { Values = new int[] { 1, 2, 3, 4, 5 } };    
        ///         var foo2 = new Foo() { Values = new int[] { 5, 4, 3, 2, 1 } };
        ///         
        ///         Assert.AreEqual(foo1, foo2, new StructuralEqualityComparer<Foo>
        ///         {
        ///             { x => x.Values,
        ///               new StructuralEqualityComparer<int> { { x => x } },
        ///               StructuralEqualityComparerOptions.IgnoreEnumerableOrder
        ///             },
        ///         });
        ///     }
        /// }
        /// ]]></code>
        /// </example>
        /// <typeparam name="TValue">The type of the value returned by the accessor.</typeparam>
        /// <param name="accessor">An accessor that gets an enumeration of values from the tested object.</param>
        /// <param name="comparer">A comparer instance for the values returned by the accessor, or null to use the default one.</param>
        /// <param name="options">Some options indicating how to compare the enumeration of values returned by the accessor.</param>
        /// <exception cref="ArgumentNullException">The specified accessor argument is a null reference.</exception>
        public void Add<TValue>(Accessor<T, IEnumerable<TValue>> accessor, EqualityComparison<TValue> comparer, StructuralEqualityComparerOptions options)
        {
            if (comparer == null)
                comparer = ComparisonSemantics.Equals<TValue>;

            if ((options & StructuralEqualityComparerOptions.IgnoreEnumerableOrder) != 0)
            {
                conditions.Add((x, y) => CompareEnumerablesIgnoringOrder(accessor(x), accessor(y), comparer));
            }
            else
            {
                conditions.Add((x, y) => CompareEnumerables(accessor(x), accessor(y), comparer));
            }
        }

        private bool CompareEnumerables<TValue>(IEnumerable<TValue> xEnumerable, IEnumerable<TValue> yEnumerable, EqualityComparison<TValue> comparer)
        {
            var xEnumerator = xEnumerable.GetEnumerator();
            var yEnumerator = yEnumerable.GetEnumerator();

            while (xEnumerator.MoveNext())
            {
                if (!yEnumerator.MoveNext())
                {
                    return false;
                }

                TValue xValue = xEnumerator.Current;
                TValue yValue = yEnumerator.Current;

                if (!comparer(xValue, yValue))
                {
                    return false;
                }
            }

            if (yEnumerator.MoveNext())
            {
                return false;
            }

            return true;
        }

        private bool CompareEnumerablesIgnoringOrder<TValue>(IEnumerable<TValue> xEnumerable, IEnumerable<TValue> yEnumerable, EqualityComparison<TValue> comparer)
        {
            var table = new MatchTable<TValue>(comparer);

            foreach (TValue xElement in xEnumerable)
                table.AddLeftValue(xElement);

            foreach (TValue yElement in yEnumerable)
                table.AddRightValue(yElement);

            return (table.NonEqualCount == 0);
        }

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object of type T to compare.</param>
        /// <param name="y">The second object of type T to compare.</param>
        /// <returns>true if the specified objects are equal; otherwise, false.</returns>
        public bool Equals(T x, T y)
        {
            return conditions.TrueForAll(predicate => predicate(x, y));
        }

        /// <summary>
        /// Returns a hash code for the specified object.
        /// </summary>
        /// <param name="obj">The object for which a hash code is to be returned.</param>
        /// <returns>A hash code for the specified object.</returns>
        /// <exception cref="ArgumentNullException">The type of obj is a reference type and obj is null.</exception>
        public int GetHashCode(T obj)
        {
            return obj.GetHashCode();
        }

        /// <summary>
        /// Returns a strongly-typed enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A strongly-typed enumerator that iterates through the collection.</returns>
        public IEnumerator<EqualityComparison<T>> GetEnumerator()
        {
            return conditions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var predicate in conditions)
            {
                yield return predicate;
            }
        }
    }
}
