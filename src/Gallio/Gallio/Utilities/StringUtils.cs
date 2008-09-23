// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using System.Text;

namespace Gallio.Utilities
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
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="str"/> is null</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="maxLength"/> is negative</exception>
        public static string Truncate(string str, int maxLength)
        {
            if (str == null)
                throw new ArgumentNullException("str");
            if (maxLength < 0)
                throw new ArgumentOutOfRangeException("maxLength", maxLength, "Max length must be non-negative.");

            if (str.Length > maxLength)
                return str.Substring(0, maxLength);

            return str;
        }

        /// <summary>
        /// If the string is longer than the specified maximum length, truncates
        /// it and appends an ellipsis mark ("...").  If the maximum length is
        /// less than or equal to 3, omits the ellipsis mark on truncation.
        /// </summary>
        /// <param name="str">The string to truncate</param>
        /// <param name="maxLength">The maximum length of the string to retain
        /// including the ellipsis mark when used</param>
        /// <returns>The truncated string</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="str"/> is null</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="maxLength"/> is negative</exception>
        public static string TruncateWithEllipsis(string str, int maxLength)
        {
            if (str == null)
                throw new ArgumentNullException("str");
            if (maxLength < 0)
                throw new ArgumentOutOfRangeException("maxLength", maxLength, "Max length must be non-negative.");

            if (str.Length > maxLength)
            {
                if (maxLength > 3)
                    return str.Substring(0, maxLength - 3) + @"...";
                else
                    return str.Substring(0, maxLength);
            }

            return str;
        }

        /// <summary>
        /// Gets a lowercase hexadecimal digit corresponding to the least significant nybble of
        /// the specified value.
        /// </summary>
        /// <param name="value">The value, only the last 4 bits of which are used</param>
        /// <returns>The hexadecimal digit</returns>
        public static char ToHexDigit(int value)
        {
            value = value & 0xf;
            return (char)(value < 10 ? 48 + value : 87 + value);
        }

        /// <summary>
        /// Formats a character value as "'x'" or "'\n'" with support for escaped characters
        /// as a valid literal value.  Encloses the char in single quotes (').
        /// </summary>
        /// <param name="value">The character value to format</param>
        /// <returns>The formatted character</returns>
        public static string ToCharLiteral(char value)
        {
            StringBuilder str = new StringBuilder(8);
            str.Append('\'');
            AppendEscapedChar(str, value);
            str.Append('\'');
            return str.ToString();
        }

        /// <summary>
        /// Escapes a character value as "x" or "\n".  Unlike <see cref="ToCharLiteral"/>,
        /// does not enclose the literal in single quotes (').
        /// </summary>
        /// <param name="value">The character value to format</param>
        /// <returns>The unquoted char literal</returns>
        public static string ToUnquotedCharLiteral(char value)
        {
            StringBuilder str = new StringBuilder(6);
            AppendEscapedChar(str, value);
            return str.ToString();
        }

        /// <summary>
        /// Formats a string value as ""abc\ndef"" with support for escaped characters
        /// as a valid literal value.  Encloses the string in quotes (").
        /// </summary>
        /// <param name="value">The string value to format</param>
        /// <returns>The formatted string</returns>
        public static string ToStringLiteral(string value)
        {
            StringBuilder str = new StringBuilder(value.Length + 2);
            str.Append('"');

            foreach (char c in value)
                AppendEscapedChar(str, c);

            str.Append('"');
            return str.ToString();
        }

        /// <summary>
        /// Escapes a string value such as "abc\ndef".  Unlike <see cref="ToStringLiteral"/>,
        /// does not enclose the literal in quotes (").
        /// </summary>
        /// <param name="value">The string value to format</param>
        /// <returns>The unquoted string literal</returns>
        public static string ToUnquotedStringLiteral(string value)
        {
            StringBuilder str = new StringBuilder(value.Length);

            foreach (char c in value)
                AppendEscapedChar(str, c);

            return str.ToString();
        }

        private static void AppendEscapedChar(StringBuilder str, char c)
        {
            switch (c)
            {
                case '\0':
                    str.Append(@"\0");
                    break;

                case '\a':
                    str.Append(@"\a");
                    break;

                case '\b':
                    str.Append(@"\b");
                    break;

                case '\f':
                    str.Append(@"\f");
                    break;

                case '\n':
                    str.Append(@"\n");
                    break;

                case '\r':
                    str.Append(@"\r");
                    break;

                case '\t':
                    str.Append(@"\t");
                    break;

                case '\v':
                    str.Append(@"\v");
                    break;

                case '\'':
                    str.Append(@"\'");
                    break;

                case '\"':
                    str.Append(@"\""");
                    break;

                case '\\':
                    str.Append(@"\\");
                    break;

                default:
                    if (char.IsLetterOrDigit(c) || char.IsPunctuation(c) || char.IsSymbol(c) || char.IsWhiteSpace(c))
                    {
                        str.Append(c);
                    }
                    else
                    {
                        str.Append('\\');
                        str.Append('u');
                        str.Append(ToHexDigit(c >> 12));
                        str.Append(ToHexDigit(c >> 8));
                        str.Append(ToHexDigit(c >> 4));
                        str.Append(ToHexDigit(c));
                    }
                    break;
            }
        }
    }
}