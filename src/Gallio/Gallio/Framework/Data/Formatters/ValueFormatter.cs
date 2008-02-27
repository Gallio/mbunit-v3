// Copyright 2008 MbUnit Project - http://www.mbunit.com/
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
using System.Collections;
using System.Globalization;
using System.Text;
using System.Xml.XPath;

namespace Gallio.Framework.Data.Formatters
{
    /// <summary>
    /// <para>
    /// A simple formatter that distinguishes between different value types.
    /// </para>
    /// <para>
    /// The values are formatted according to type:
    /// <list type="item">
    /// <item>Booleans: true, false</item>
    /// <item>Dates: yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffffffzz (invariant round-trip format)</item>
    /// <item>Numbers: 0, 1.2, 3.3m, 2.5f</item>
    /// <item>Strings: "abc", "def", "blah\nfoo"</item>
    /// <item>Chars: 'a', '\n'</item>
    /// <item>Bytes: 0xa5</item>
    /// <item>Enumerations: [1, 2, 3]</item>
    /// <item>XPathNavigable: (as outer xml)</item>
    /// <item>Objects: {tostring}</item>
    /// <item>Nulls: null</item>
    /// <item>DBNulls: dbnull</item>
    /// </list>
    /// </para>
    /// </summary>
    /// <todo author="jeff">
    /// Long term we will want to create an extensible rule-based formatter.
    /// However, this value formatter should be good enough for now.
    /// </todo>
    public class ValueFormatter : IFormatter
    {
        /// <inheritdoc />
        public string Format(object value)
        {
            return FormatAny(value);
        }

        private static string FormatAny(object value)
        {
            if (value == null)
                return @"null";

            switch (Type.GetTypeCode(value.GetType()))
            {
                case TypeCode.Boolean:
                    return FormatBoolean((bool)value);

                case TypeCode.Byte:
                    return FormatByte((byte)value);

                case TypeCode.SByte:
                    return FormatByte((sbyte)value);

                case TypeCode.DateTime:
                    return FormatDateTime((DateTime)value);

                case TypeCode.DBNull:
                    return @"dbnull";

                case TypeCode.Decimal:
                    return FormatDecimal((decimal)value);

                case TypeCode.Single:
                    return FormatSingle((float)value);

                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return value.ToString();

                case TypeCode.Char:
                    return FormatChar((char)value);

                case TypeCode.String:
                    return FormatString((string)value);

                default:
                case TypeCode.Object:
                    return FormatObject(value);
            }
        }

        private static string FormatBoolean(bool value)
        {
            return value ? @"true" : @"false";
        }

        private static string FormatByte(int value)
        {
            StringBuilder str = new StringBuilder(4, 4);
            str[0] = '0';
            str[1] = 'x';
            str[2] = FormatHexDigit(value >> 4);
            str[3] = FormatHexDigit(value);
            return str.ToString();
        }

        private static char FormatHexDigit(int value)
        {
            value = value & 0xf;
            return (char) (value < 10 ? 48 + value : 55 + value);
        }

        private static string FormatDateTime(DateTime value)
        {
            return value.ToString("o");
        }

        private static string FormatDecimal(decimal value)
        {
            return value.ToString(CultureInfo.InvariantCulture) + @"m";
        }

        private static string FormatSingle(float value)
        {
            return value.ToString(CultureInfo.InvariantCulture) + @"f";
        }

        private static string FormatChar(char value)
        {
            StringBuilder str = new StringBuilder(8);
            str.Append('\'');
            AppendEscapedChar(str, value);
            str.Append('\'');
            return str.ToString();
        }

        private static string FormatString(string value)
        {
            StringBuilder str = new StringBuilder(value.Length + 2);
            str.Append('"');

            foreach (char c in value)
                AppendEscapedChar(str, c);

            str.Append('"');
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

                case '\"':
                    str.Append(@"\""");
                    break;

                case '\\':
                    str.Append(@"\\");
                    break;

                default:
                    if (char.IsControl(c))
                    {
                        str.Append('\\');
                        str.Append('u');
                        str.Append(FormatHexDigit(c >> 12));
                        str.Append(FormatHexDigit(c >> 8));
                        str.Append(FormatHexDigit(c >> 4));
                        str.Append(FormatHexDigit(c));
                    }
                    else
                    {
                        str.Append(c);
                    }
                    break;
            }
        }

        private static string FormatObject(object value)
        {
            IXPathNavigable xpathNavigable = value as IXPathNavigable;
            if (xpathNavigable != null)
                return FormatXPathNavigable(xpathNavigable);

            IEnumerable enumerable = value as IEnumerable;
            if (enumerable != null)
                return FormatEnumerable(enumerable);

            return string.Concat("{", value.ToString(), "}");
        }

        private static string FormatEnumerable(IEnumerable enumerable)
        {
            StringBuilder str = new StringBuilder();
            str.Append('[');

            bool first = true;
            foreach (object value in enumerable)
            {
                if (first)
                    first = false;
                else
                    str.Append(", ");

                str.Append(FormatAny(value));
            }

            str.Append(']');
            return str.ToString();
        }

        private static string FormatXPathNavigable(IXPathNavigable xpathNavigable)
        {
            return xpathNavigable.CreateNavigator().OuterXml;
        }
    }
}
