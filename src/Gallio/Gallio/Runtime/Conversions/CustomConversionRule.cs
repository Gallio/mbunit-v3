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

namespace Gallio.Runtime.Conversions
{
    /// <summary>
    /// Conversion rule for custom user conversions.
    /// </summary>
    public sealed class CustomConversionRule : IConversionRule
    {
        private readonly Conversion conversion;

        /// <summary>
        /// Constructs a custom conversion rule.
        /// </summary>
        /// <param name="conversion">The conversion operation.</param>
        public CustomConversionRule(Conversion conversion)
        {
            if (conversion == null)
                throw new ArgumentNullException("conversion");

            this.conversion = conversion;
        }

        /// <inheritdoc />
        public ConversionCost GetConversionCost(Type sourceType, Type targetType, IConverter elementConverter)
        {
            return elementConverter.GetConversionCost(sourceType, targetType).Add(ConversionCost.Best);
        }

        /// <inheritdoc />
        public object Convert(object sourceValue, Type targetType, IConverter elementConverter)
        {
            return conversion(sourceValue);
        }
    }
}