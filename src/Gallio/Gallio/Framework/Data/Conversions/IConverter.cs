// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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

namespace Gallio.Framework.Data.Conversions
{
    /// <summary>
    /// A converter converts a value from a source type to a target type.
    /// </summary>
    public interface IConverter
    {
        /// <summary>
        /// Determines whether the converter can convert a value of type <paramref name="sourceType"/>
        /// to type <paramref name="targetType"/>.
        /// </summary>
        /// <param name="sourceType">The source type</param>
        /// <param name="targetType">The target type</param>
        /// <returns>True if the converter supports this conversion</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="sourceType" /> or
        /// <paramref name="targetType"/> is null</exception>
        bool CanConvert(Type sourceType, Type targetType);

        /// <summary>
        /// Gets the cost of converting a value of type <paramref name="sourceType"/>
        /// to type <paramref name="targetType"/>.
        /// </summary>
        /// <param name="sourceType">The source type</param>
        /// <param name="targetType">The target type</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="sourceType" /> or
        /// <paramref name="targetType"/> is null</exception>
        /// <returns>The conversion cost</returns>
        ConversionCost GetConversionCost(Type sourceType, Type targetType);

        /// <summary>
        /// Converts the value <paramref name="sourceValue"/> to type <paramref name="targetType"/>.
        /// </summary>
        /// <param name="sourceValue">The value to convert</param>
        /// <param name="targetType">The target type</param>
        /// <returns>The converted value</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="targetType"/> is null</exception>
        /// <exception cref="InvalidOperationException">Thrown the requested conversion was not supported</exception>
        object Convert(object sourceValue, Type targetType);
    }
}
