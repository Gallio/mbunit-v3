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
    /// Interface for implementing a custom data converter.
    /// </summary>
    /// <remarks>
    /// <para>
    /// That interface must be used in conjonction with the attribute 
    /// <see cref="CustomConverterAttribute"/>, as shown in the example below.
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
    ///     [CustomConverter]
    ///     public class StringToFooConverter : ICustomConverter<string, Foo>
    ///     {
    ///         public Foo Convert(string source)
    ///         {
    ///             int value = Int32.Parse(source);
    ///             return new Foo(value);
    ///         }
    ///     }
    ///     
    ///     [Test]
    ///     public void SomeTest([Column("123", "456", "789")] Foo foo)
    ///     {
    ///        // Test logic here...
    ///     }
    /// }
    /// ]]></code>
    /// </example>
    public interface ICustomConverter<TSource, TTarget>
    {
        /// <summary>
        /// Converts the source object into an object of another type.
        /// </summary>
        /// <param name="source">The source object to convert.</param>
        /// <returns>The result of the conversion.</returns>
        TTarget Convert(TSource source);
    }
}
