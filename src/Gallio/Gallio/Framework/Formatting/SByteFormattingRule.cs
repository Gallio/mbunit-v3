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
using System.Text;
using Gallio.Utilities;

namespace Gallio.Framework.Formatting
{
    /// <summary>
    /// <para>
    /// A formatting rule for <see cref="sbyte"/>.
    /// </para>
    /// <para>
    /// Formats values as two digit signed hex values like "0x55" and "-0x55".
    /// </para>
    /// </summary>
    public sealed class SByteFormattingRule : IFormattingRule
    {
        /// <inheritdoc />
        public int? GetPriority(Type type)
        {
            if (type == typeof(sbyte))
                return FormattingRulePriority.Best;
            return null;
        }

        /// <inheritdoc />
        public string Format(object obj, IFormatter formatter)
        {
            sbyte value = (sbyte)obj;

            StringBuilder str = new StringBuilder(5, 5);

            if (value < 0)
            {
                str.Append('-');
                value = (sbyte)-value;
            }

            str.Append("0x");
            str.Append(StringUtils.ToHexDigit(value >> 4));
            str.Append(StringUtils.ToHexDigit(value));
            return str.ToString();
        }
    }
}