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
    /// Converts arrays from one-dimensional arrays of one element type to another.
    /// </summary>
    public sealed class ArrayToArrayConversionRule : IConversionRule
    {
        /// <inheritdoc />
        public ConversionCost GetConversionCost(Type sourceType, Type targetType, IConverter elementConverter)
        {
            if (sourceType.IsArray && targetType.IsArray
                && sourceType.GetArrayRank() == 1 && targetType.GetArrayRank() == 1)
                return elementConverter.GetConversionCost(sourceType.GetElementType(), targetType.GetElementType())
                    .Add(ConversionCost.Typical);

            return ConversionCost.Invalid;
        }

        /// <inheritdoc />
        public object Convert(object sourceValue, Type targetType, IConverter elementConverter, bool nullable)
        {
            var sourceArray = (Array)sourceValue;
            int length = sourceArray.Length;

            Type targetElementType = targetType.GetElementType();
            Array targetArray = Array.CreateInstance(targetElementType, length);

            for (int i = 0; i < length; i++)
                targetArray.SetValue(elementConverter.Convert(sourceArray.GetValue(i), targetElementType), i);

            return targetArray;
        }
    }
}
