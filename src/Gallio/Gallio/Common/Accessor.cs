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

namespace Gallio.Common
{
    /// <summary>
    /// <para>
    /// Represents a method that returns a value of a specified type from the instance of an object.
    /// </para>
    /// <para>
    /// It is usually used to represent the invocation of a type property.
    /// </para>
    /// </summary>
    /// <typeparam name="T">The type of the object to get the value from.</typeparam>
    /// <typeparam name="TValue">The type of the returned value.</typeparam>
    /// <param name="obj">The object to get the value from.</param>
    /// <returns>The returned value.</returns>
    public delegate TValue Accessor<T, TValue>(T obj);
}