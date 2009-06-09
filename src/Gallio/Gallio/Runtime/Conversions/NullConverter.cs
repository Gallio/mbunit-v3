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

namespace Gallio.Runtime.Conversions
{
    /// <summary>
    /// A null implementation of a converter that does not actually support
    /// converting values.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Consequently <see cref="IConverter.CanConvert" /> returns
    /// false whenever a conversion would be required.
    /// </para>
    /// </remarks>
    public class NullConverter : BaseConverter
    {
        /// <summary>
        /// Gets a singleton instance of the null converter.
        /// </summary>
        public readonly static NullConverter Instance = new NullConverter();

        /// <inheritdoc />
        protected override ConversionCost GetConversionCostImpl(Type sourceType, Type targetType)
        {
            return sourceType.Equals(targetType) ? ConversionCost.Zero : ConversionCost.Invalid;
        }

        /// <inheritdoc />
        protected override object ConvertImpl(object sourceValue, Type targetType)
        {
            if (sourceValue == null || targetType.IsInstanceOfType(sourceValue))
                return sourceValue;

            throw new InvalidOperationException("The null converter does not support conversions.");
        }
    }
}
