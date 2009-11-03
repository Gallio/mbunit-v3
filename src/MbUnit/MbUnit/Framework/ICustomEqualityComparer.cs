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
    /// Interface for implementing a custom equality comparer.
    /// </summary>
    /// <remarks>
    /// <para>
    /// That interface must be used in conjonction with the attribute 
    /// <see cref="CustomEqualityComparerAttribute"/>, as shown in the example below.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code><![CDATA[
    /// public class Foo // Non equatable class.
    /// {
    ///     private readonly string text;
    /// 
    ///     public string Text
    ///     {
    ///         get { return text; }
    ///     }
    /// 
    ///     public Foo(string text)
    ///     {
    ///         this.text = text;
    ///     }
    /// }
    /// 
    /// [TestFixture]
    /// public class FooTest
    /// {
    ///     [CustomEqualityComparer]
    ///     public class FooEqualityComparer : ICustomEqualityComparer<Foo>
    ///     {
    ///         public bool Equals(Foo x, Foo y)
    ///         {
    ///             // The inner comparison engine of Gallio handles with the cases
    ///             // where x or y is null. Therefore we can safely assume than x
    ///             // and y are never null here.
    /// 
    ///             return x.Text.Equals(y.Text, StringComparison.OrdinalIgnoreCase); // Custom equality logic.
    ///         }
    ///     }
    ///     
    ///     [Test]
    ///     public void SomeTest()
    ///     {
    ///         var foo1 = new Foo("Hello World!");
    ///         var foo2 = new Foo("HELLO WORLD!");
    ///         var foo3 = new Foo("Goodbye World!");
    ///         Assert.AreEqual(foo1, foo2); // The assertions will use the custom comparer defined above.
    ///         Assert.AreNotEqual(foo1, foo3); 
    ///     }
    /// }
    /// ]]></code>
    /// </example>
    /// <seealso cref="CustomEqualityComparerAttribute"/>
    public interface ICustomEqualityComparer<T>
    {
        // Note: Although ICustomComparer<T> derives from IComparer<T>, ICustomEqualityComparer<T> does not
        // derives symetrically from IEqualityComparer<T> because of the unwanted hash code provider method.

        /// <summary>
        /// Determines whether the specified objects are equivalent.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The Gallio inner comparison engine handles with the case of null objects comparison prior to
        /// the custom comparers. Therefore, you can safely assume that <paramref name="x"/> and <paramref name="y"/> 
        /// are never null.
        /// </para>
        /// </remarks>
        /// <param name="x">The first object to compare (never null).</param>
        /// <param name="y">The second object to compare (never null).</param>
        /// <returns>True if the objects are equivalent; false otherwise.</returns>
        bool Equals(T x, T y);
    }
}
