// Copyright 2007 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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
    /// Determines whether two values satisfy a binary relation and
    /// returns true if they do.  Examples of relations are equality,
    /// inequality, less-than, greater-than, and any other well-defined
    /// function that operates on two values yielding true or false.
    /// </summary>
    /// <typeparam name="T">The type of values to compare</typeparam>
    /// <param name="x">The first value</param>
    /// <param name="y">The second value</param>
    /// <returns>True if the values satisfy the relation</returns>
    public delegate bool Relation<T>(T x, T y);
}
