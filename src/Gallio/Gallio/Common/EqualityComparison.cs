// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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

namespace Gallio.Common
{
    /// <summary>
    /// Represents a method that determines whether two objects of the same type are equal.
    /// </summary>
    /// <param name="x">The first object to compare.</param>
    /// <param name="y">The second object to compare.</param>
    /// <returns>True if the object are equal; otherwise false.</returns>
    public delegate bool EqualityComparison(object x, object y);

    /// <summary>
    /// Represents a strongly-typed method that determines whether two objects of the same type are equal.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This delegate is for <see cref="IEquatable{T}"/>, what <see cref="Comparison{T}"/> is
    /// for <see cref="IComparable{T}"/>.
    /// </para>
    /// </remarks>
    /// <typeparam name="T">The type of the objects to compare.</typeparam>
    /// <param name="x">The first object to compare.</param>
    /// <param name="y">The second object to compare.</param>
    /// <returns>True if the object are equal; otherwise false.</returns>
    public delegate bool EqualityComparison<T>(T x, T y);
}