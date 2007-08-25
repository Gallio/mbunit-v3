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