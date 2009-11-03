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
using Gallio.Common;
using Gallio.Framework;

namespace MbUnit.Framework
{
    /// <summary>
    /// Declares a container class for one or several custom type comparers.
    /// </summary>
    /// <remarks>
    /// <para>
    /// That attribute must be used on a type implementing the interface <see cref="ICustomComparer{T}"/>.
    /// </para>
    /// <para>
    /// It is possible for a container class to define more than one custom comparer. 
    /// Implement <see cref="ICustomComparer{T}"/> as many times as it is necessary for every type
    /// you need to compare.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code><![CDATA[
    /// [CustomComparer]
    /// public class FooComparer : ICustomComparer<Foo>
    /// {
    ///     public int Compare(Foo x, Foo y)
    ///     {
    ///         return /* Insert comparison logic here... */
    ///     }
    /// }
    /// ]]></code>
    /// </example>
    /// <seealso cref="ICustomComparer{T}"/>
    /// <seealso cref="CustomEqualityComparerAttribute"/>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class CustomComparerAttribute : AbstractCustomComparerAttribute
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public CustomComparerAttribute()
            : base(typeof(ICustomComparer<>), "Compare", typeof(int))
        {
        }

        /// <inheritdoc />
        protected override void Register(Type type, Func<object, object, object> operation)
        {
            CustomComparers.Register(type, (x, y) => (int)operation(x, y));
        }

        /// <inheritdoc />
        protected override void Unregister(Type type)
        {
            CustomComparers.Unregister(type);
        }
    }
}
