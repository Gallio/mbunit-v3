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
using System.Text;

namespace Gallio.Framework.Data.Formatters
{
    /// <summary>
    /// <para>
    /// A formatting rule for <see cref="IEnumerable" />.
    /// </para>
    /// <para>
    /// Formats values as "[1, 2, 3]".
    /// </para>
    /// </summary>
    public sealed class EnumerableFormattingRule : IFormattingRule
    {
        /// <inheritdoc />
        public int? GetPriority(Type type)
        {
            if (typeof(IEnumerable).IsAssignableFrom(type))
                return FormattingRulePriority.Typical;
            return null;
        }

        /// <inheritdoc />
        public string Format(object obj, IFormatter formatter)
        {
            IEnumerable enumerable = (IEnumerable)obj;

            StringBuilder str = new StringBuilder();
            str.Append('[');

            bool first = true;
            foreach (object value in enumerable)
            {
                if (first)
                    first = false;
                else
                    str.Append(", ");

                str.Append(formatter.Format(value));
            }

            str.Append(']');
            return str.ToString();
        }
    }
}