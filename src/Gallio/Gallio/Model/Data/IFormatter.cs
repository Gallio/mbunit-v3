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

namespace Gallio.Model.Data
{
    /// <summary>
    /// A formatter produces a string representation of a value of some type.
    /// </summary>
    public interface IFormatter
    {
        /// <summary>
        /// Determines whether the formatter can format values of the
        /// specified type.
        /// </summary>
        /// <param name="valueType">The value type</param>
        /// <returns>True if the formatter can format values of that type</returns>
        bool CanFormat(Type valueType);

        /// <summary>
        /// Formats a value to a string.
        /// </summary>
        /// <param name="value">The value to format</param>
        /// <returns>The formatted string, never null</returns>
        string Format(object value);
    }
}