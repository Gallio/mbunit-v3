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

namespace MbUnit.Framework.Kernel.Conversions
{
    /// <summary>
    /// A converter converts a value from a source type to a target type.
    /// </summary>
    public interface IConverter
    {
        /// <summary>
        /// Determines whether the converter can convert values from the
        /// source type to the target type.
        /// </summary>
        /// <param name="sourceType">The source type</param>
        /// <param name="targetType">The target type</param>
        /// <returns>True if the converter supports this conversion</returns>
        bool CanConvert(Type sourceType, Type targetType);

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="sourceValue">The value to convert</param>
        /// <param name="targetType">The target type</param>
        /// <returns>The converted value</returns>
        object Convert(object sourceValue, Type targetType);
    }
}