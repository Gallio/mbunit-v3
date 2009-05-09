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
using System.Collections.Generic;
using System.Text;

namespace Gallio.Common.Text
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
        /// <remarks>
        /// Replaces common escaped characters with C# style escape codes.  Unprintable characters
        /// are represented by a Unicode character escape.
        /// </remarks>
        /// <param name="value">The character value to format</param>
        /// <returns>The formatted character</returns>
        public static string ToCharLiteral(char value)
        {
            StringBuilder str = new StringBuilder(8);
            str.Append('\'');
            AppendUnquotedCharLiteral(str, value);
            str.Append('\'');
            return str.ToString();
        }

        /// <summary>
        /// Escapes a character value as "x" or "\n".  Unlike <see cref="ToCharLiteral"/>,
        /// does not enclose the literal in single quotes (').
        /// </summary>
        /// <remarks>
        /// Replaces common escaped characters with C# style escape codes.  Unprintable characters
        /// are represented by a Unicode character escape.
        /// </remarks>
        /// <param name="value">The character value to format</param>
        /// <returns>The unquoted char literal</returns>
        public static string ToUnquotedCharLiteral(char value)
        {
            StringBuilder str = new StringBuilder(6);
            AppendUnquotedCharLiteral(str, value);
            return str.ToString();
        }

        private static void AppendUnquotedCharLiteral(StringBuilder str, char value)
        {
            char previousChar = '\0';
            AppendEscapedChar(str, value, ref previousChar);
        }

        /// <summary>
        /// Formats a string value as ""abc\ndef"" with support for escaped characters
        /// as a valid literal value.  Encloses the string in quotes (").
        /// </summary>
        /// <remarks>
        /// Replaces common escaped characters with C# style escape codes.  Unprintable characters
        /// are represented by a Unicode character escape.
        /// </remarks>
        /// <param name="value">The string value to format</param>
        /// <returns>The formatted string</returns>
        public static string ToStringLiteral(string value)
        {
            StringBuilder str = new StringBuilder(value.Length + 2);
            str.Append('"');
            AppendUnquotedStringLiteral(str, value);
            str.Append('"');
            return str.ToString();
        }

        /// <summary>
        /// Escapes a string value such as "abc\ndef".  Unlike <see cref="ToStringLiteral"/>,
        /// does not enclose the literal in quotes (").
        /// </summary>
        /// <remarks>
        /// Replaces common escaped characters with C# style escape codes.  Unprintable characters
        /// are represented by a Unicode character escape.
        /// </remarks>
        /// <param name="value">The string value to format</param>
        /// <returns>The unquoted string literal</returns>
        public static string ToUnquotedStringLiteral(string value)
        {
            StringBuilder str = new StringBuilder(value.Length);
            AppendUnquotedStringLiteral(str, value);
            return str.ToString();
        }

        private static void AppendUnquotedStringLiteral(StringBuilder str, string value)
        {
            char previousChar = '\0';
            foreach (char c in value)
                AppendEscapedChar(str, c, ref previousChar);
        }

        private static void AppendEscapedChar(StringBuilder str, char c, ref char previousChar)
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
                        int code;
                        if (char.IsHighSurrogate(previousChar) && char.IsLowSurrogate(c))
                        {
                            code = char.ConvertToUtf32(previousChar, c);

                            str.Length -= 5;
                            str.Append('U');
                            str.Append('0');
                            str.Append('0');
                            str.Append(ToHexDigit(code >> 20));
                            str.Append(ToHexDigit(code >> 16));
                        }
                        else
                        {
                            code = c;

                            str.Append('\\');
                            str.Append('u');
                        }

                        str.Append(ToHexDigit(code >> 12));
                        str.Append(ToHexDigit(code >> 8));
                        str.Append(ToHexDigit(code >> 4));
                        str.Append(ToHexDigit(code));
                    }
                    break;
            }

            previousChar = c;
        }

        /// <summary>
        /// Parses a key/value pair from an input string of the form "key=value", with the
        /// value optionally quoted and optional surrounding whitespace removed.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If the value is quoted, the outermost quotes will be removed.
        /// If the equals or value is absent then an empty string will be used as the value.
        /// Also trims whitespace around the key and value.
        /// </para>
        /// </remarks>
        /// <param name="input">The input string</param>
        /// <returns>The key value pair</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="input"/> is null</exception>
        public static KeyValuePair<string, string> ParseKeyValuePair(string input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            int equalsPos = input.IndexOf('=');
            if (equalsPos < 0)
                return new KeyValuePair<string, string>(input.Trim(), "");

            string key = input.Substring(0, equalsPos).Trim();
            string value = input.Substring(equalsPos + 1).Trim();

            return new KeyValuePair<string, string>(key, Unquote(value));
        }

        private static string Unquote(string value)
        {
            return IsQuoted(value) ? value.Substring(1, value.Length - 2) : value;
        }

        private static bool IsQuoted(string value)
        {
            if (value.Length < 2)
                return false;

            char firstChar = value[0];
            if (firstChar != '"' && firstChar != '\'')
                return false;

            return value[value.Length - 1] == firstChar;
        }

        /// <summary>
        /// Parses a string of whitespace delimited and possibly quoted arguments and
        /// returns an array of each one unquoted.
        /// </summary>
        /// <param name="arguments">The arguments string, eg. "/foo 'quoted arg' /bar</param>
        /// <returns>The parsed and unquoted arguments</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="arguments"/> is null</exception>
        public static string[] ParseArguments(string arguments)
        {
            if (arguments == null)
                throw new ArgumentNullException("arguments");

            List<string> result = new List<string>();

            char quoteChar = '\0';
            bool inQuotes = false;

            int startPos, currentPos;
            for (startPos = 0, currentPos = 0; currentPos < arguments.Length; currentPos++)
            {
                char c = arguments[currentPos];
                if (currentPos > startPos)
                {
                    if (inQuotes && c == quoteChar && (currentPos + 1 == arguments.Length || char.IsWhiteSpace(arguments[currentPos + 1]))
                        || !inQuotes && char.IsWhiteSpace(c))
                    {
                        result.Add(arguments.Substring(startPos, currentPos - startPos));
                        startPos = currentPos + 1;
                        inQuotes = false;
                    }
                }
                else
                {
                    if (c == '"' || c == '\'')
                    {
                        inQuotes = true;
                        quoteChar = c;
                        startPos = currentPos + 1;
                    }
                    else if (char.IsWhiteSpace(c))
                    {
                        startPos = currentPos + 1;
                    }
                }
            }

            if (currentPos > startPos)
                result.Add(arguments.Substring(startPos, currentPos - startPos));

            return result.ToArray();
        }
    }
}