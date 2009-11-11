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

namespace Gallio.Runtime.Conversions
{
    /// <summary>
    /// Represents a method that converts an object into an object of another type.
    /// </summary>
    /// <param name="source">The object to convert.</param>
    /// <returns>The result of the conversion.</returns>
    public delegate object Conversion(object source);

    /// <summary>
    /// Represents a strongly-typed method that converts an object into an object of another type.
    /// </summary>
    /// <typeparam name="TSource">The type of the object to convert.</typeparam>
    /// <typeparam name="TTarget">The type of the result of the conversion,</typeparam>
    /// <param name="source">The object to convert.</param>
    /// <returns>The result of the conversion.</returns>
    public delegate TTarget Conversion<TSource, TTarget>(TSource source);
}