using System;
using System.Collections.Generic;
using System.Text;

namespace MbUnit.Core.Utilities
{
    /// <summary>
    /// Provides utility functions for working with strings.
    /// </summary>
    public static class StringUtils
    {
        /// <summary>
        /// Truncates the string to the specified maximum length.
        /// Discards characters at the end of the string with indices greater than
        /// or equal to <paramref name="maxLength"/>.
        /// </summary>
        /// <param name="str">The string to truncate</param>
        /// <param name="maxLength">The maximum length of the string to retain</param>
        /// <returns>The truncated string</returns>
        public static string Truncate(string str, int maxLength)
        {
            if (str.Length > maxLength)
                return str.Substring(0, maxLength);

            return str;
        }

        /// <summary>
        /// If the string is longer than the specified maximum length, truncates
        /// it and appends an ellipsis mark ("...").  If the maximum length is
        /// less than 3, omits the ellipsis mark on truncation.
        /// </summary>
        /// <param name="str">The string to truncate</param>
        /// <param name="maxLength">The maximum length of the string to retain
        /// including the ellipsis mark when used</param>
        /// <returns>The truncated string</returns>
        public static string TruncateWithEllipsis(string str, int maxLength)
        {
            if (str.Length > maxLength)
            {
                if (maxLength >= 3)
                    return str.Substring(0, maxLength - 3) + "...";
                else
                    return str.Substring(0, maxLength);
            }

            return str;
        }
    }
}
