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

namespace MbUnit.Framework
{
    /// <summary>
    /// Interface for implementing a custom comparer.
    /// </summary>
    /// <remarks>
    /// <para>
    /// That interface must be used in conjonction with the attribute 
    /// <see cref="CustomComparerAttribute"/>, as shown in the example below.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code><![CDATA[
    /// public class Foo // Non comparable class.
    /// {
    ///     private readonly int value;
    /// 
    ///     public int Value
    ///     {
    ///         get { return value; }
    ///     }
    /// 
    ///     public Foo(int value)
    ///     {
    ///         this.value = value;
    ///     }
    /// }
    /// 
    /// [TestFixture]
    /// public class FooTest
    /// {
    ///     [CustomComparer]
    ///     public class FooComparer : ICustomComparer<Foo>
    ///     {
    ///         public int Compare(Foo x, Foo y)
    ///         {
    ///             // The inner comparison engine of Gallio handles with the cases
    ///             // where x or y is null. Therefore, we can safely assume than x
    ///             // and y are never null here.
    /// 
    ///             return x.Value.CompareTo(y.Value); // Custom comparison logic.
    ///         }
    ///     }
    ///     
    ///     [Test]
    ///     public void SomeTest()
    ///     {
    ///         var foo1 = new Foo(123);
    ///         var foo2 = new Foo(456);
    ///         var foo3 = new Foo(789);
    ///         Assert.GreaterThan(foo2, foo1); // The assertions will use the custom comparer defined above.
    ///         Assert.LessThan(foo2, foo3);
    ///     }
    /// }
    /// ]]></code>
    /// </example>
    /// <seealso cref="CustomEqualityComparerAttribute"/>
    public interface ICustomComparer<T> : IComparer<T>
    {
    }
}
