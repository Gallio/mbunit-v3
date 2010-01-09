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

namespace Gallio.Framework
{
    /// <summary>
    /// Subtracts the right value from the left and returns the difference.
    /// </summary>
    /// <typeparam name="TValue">The type of values to be compared.</typeparam>
    /// <typeparam name="TDifference">The type of the difference produced when the values are
    /// subtracted, for numeric types this is the same as <typeparamref name="TValue"/> but it
    /// may differ for other types.</typeparam>
    /// <param name="left">The left value.</param>
    /// <param name="right">The right value.</param>
    /// <returns>The difference when the right value is subtracted from the left.</returns>
    internal delegate TDifference SubtractionFunc<TValue, TDifference>(TValue left, TValue right);
}
