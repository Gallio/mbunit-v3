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
using Gallio.Common.Reflection;

namespace Gallio.Runtime.Conversions
{
    /// <summary>
    /// Converts values among <see cref="IConvertible" /> types.
    /// </summary>
    public sealed class ConvertibleToConvertibleConversionRule : IConversionRule
    {
        /// <inheritdoc />
        public ConversionCost GetConversionCost(Type sourceType, Type targetType, IConverter elementConverter)
        {
            if (typeof(IConvertible).IsAssignableFrom(sourceType)
                && typeof(IConvertible).IsAssignableFrom(targetType))
                return ConversionCost.Typical;

            return ConversionCost.Invalid;
        }

        /// <inheritdoc />
        public object Convert(object sourceValue, Type targetType, IConverter elementConverter, bool nullable)
        {
            return EmptyStringToNull(sourceValue, nullable) ? null : System.Convert.ChangeType(sourceValue, targetType);
        }

        // Special case: convert empty and blank strings to 'null' if the target type is nullable.
        private static bool EmptyStringToNull(object sourceValue, bool nullable)
        {
            return nullable
                && sourceValue is string
                && ((string)sourceValue).Trim().Length == 0;
        }
    }
}