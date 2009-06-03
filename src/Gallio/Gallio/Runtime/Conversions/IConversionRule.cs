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
    /// <para>
    /// A conversion rule encapsulates an algorithm for converting a value from a source
    /// type to a target type.
    /// </para>
    /// </summary>
    /// <remarks>
    /// This type is not intended to be used directly by clients.  Instead refer to
    /// <see cref="IConverter" /> for a simpler abstraction that wraps <see cref="IConversionRule" />.
    /// </remarks>
    public interface IConversionRule
    {
        /// <summary>
        /// Gets the cost of converting a value of type <paramref name="sourceType"/>
        /// to type <paramref name="targetType"/>.
        /// </summary>
        /// <param name="sourceType">The source type, never null.</param>
        /// <param name="targetType">The target type, never null.</param>
        /// <param name="elementConverter">A converter that may be used to recursively
        /// convert the contents of a composite object from one type to another, never null.</param>
        /// <returns>The conversion cost.</returns>
        ConversionCost GetConversionCost(Type sourceType, Type targetType, IConverter elementConverter);

        /// <summary>
        /// Converts the value <paramref name="sourceValue"/> to type <paramref name="targetType"/>.
        /// </summary>
        /// <remarks>
        /// This method must not be called if <see cref="GetConversionCost" /> returned <see cref="ConversionCost.Invalid" />
        /// when asked about the <paramref name="sourceValue"/> type and <paramref name="targetType"/>.
        /// It must also not be called if <paramref name="sourceValue"/> is <c>null</c> or
        /// if it is already an instance of <paramref name="targetType"/>.
        /// </remarks>
        /// <param name="sourceValue">The value to convert, never null.</param>
        /// <param name="targetType">The target type, never null.</param>
        /// <param name="elementConverter">A converter that may be used to recursively
        /// convert the contents of a composite object from one type to another, never null.</param>
        /// <returns>The converted value.</returns>
        object Convert(object sourceValue, Type targetType, IConverter elementConverter);
    }
}
