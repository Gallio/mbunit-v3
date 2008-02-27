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
using System.Text;
using Gallio.Utilities;

namespace Gallio.Framework.Data.Formatters
{
    /// <summary>
    /// <para>
    /// A formatting rule for <see cref="byte" /> and <see cref="sbyte"/>.
    /// </para>
    /// <para>
    /// Formats values as two digit hex values like "0xa5".
    /// </para>
    /// </summary>
    public sealed class ByteFormattingRule : IFormattingRule
    {
        /// <inheritdoc />
        public int? GetPriority(Type type)
        {
            if (type == typeof(byte) || type == typeof(sbyte))
                return FormattingRulePriority.Best;
            return null;
        }

        /// <inheritdoc />
        public string Format(object obj, IFormatter formatter)
        {
            if (obj is byte)
                return FormatByte((byte)obj);

            return FormatByte((sbyte)obj);
        }

        private static string FormatByte(int value)
        {
            StringBuilder str = new StringBuilder(4, 4);
            str[0] = '0';
            str[1] = 'x';
            str[2] = StringUtils.ToHexDigit(value >> 4);
            str[3] = StringUtils.ToHexDigit(value);
            return str.ToString();
        }
    }
}