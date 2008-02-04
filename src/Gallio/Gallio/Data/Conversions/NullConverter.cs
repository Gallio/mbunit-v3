// Copyright 2008 MbUnit Project - http://www.mbunit.com/
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

namespace Gallio.Data.Conversions
{
    /// <summary>
    /// A null implementation of a converter that does not actually support
    /// converting values.  Consequently <see cref="IConverter.CanConvert" /> returns
    /// false whenever a conversion would be required.
    /// </summary>
    public class NullConverter : BaseConverter
    {
        /// <inheritdoc />
        protected override ConversionCost GetConversionCostInternal(Type sourceType, Type targetType)
        {
            return sourceType.Equals(targetType) ? ConversionCost.Zero : ConversionCost.Invalid;
        }

        /// <inheritdoc />
        protected override object ConvertInternal(object sourceValue, Type targetType)
        {
            if (sourceValue == null || targetType.IsInstanceOfType(sourceValue))
                return sourceValue;

            throw new InvalidOperationException("The null converter does not support conversions.");
        }
    }
}
