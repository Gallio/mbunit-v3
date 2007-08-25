using System;
using System.Collections.Generic;
using System.Text;

namespace MbUnit.Framework.Kernel.Formatting
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
