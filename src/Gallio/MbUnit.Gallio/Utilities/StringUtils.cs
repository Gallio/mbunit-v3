// Copyright 2007 MbUnit Project - http://www.mbunit.com/
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
using System.Collections.Generic;
using System.Text;

namespace MbUnit.Utilities
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
                    return str.Substring(0, maxLength - 3) + @"...";
                else
                    return str.Substring(0, maxLength);
            }

            return str;
        }
    }
}