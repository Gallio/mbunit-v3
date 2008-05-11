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
using Gallio.Framework.Data.Conversions;
using Gallio.Utilities;

namespace Gallio.Framework.Data.Formatters
{
    /// <summary>
    /// A default formatting rule for objects based on using the <see cref="IConverter" />
    /// to convert the value to a string.  This rule has minimum priority so that
    /// all other formatting rules should override it in principle.
    /// </summary>
    public sealed class ConvertToStringFormattingRule : IFormattingRule
    {
        private readonly IConverter converter;

        /// <summary>
        /// Creates a conversion to string formatting rule.
        /// </summary>
        /// <param name="converter">The converter to use</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="converter"/> is null</exception>
        public ConvertToStringFormattingRule(IConverter converter)
        {
            if (converter == null)
                throw new ArgumentNullException("converter");

            this.converter = converter;
        }

        /// <inheritdoc />
        public int? GetPriority(Type type)
        {
            return FormattingRulePriority.Default;
        }

        /// <inheritdoc />
        public string Format(object obj, IFormatter formatter)
        {
            string result = (string) converter.Convert(obj, typeof(string));
            if (string.IsNullOrEmpty(result))
                return null;

            return string.Concat("{", StringUtils.ToUnquotedStringLiteral(result), "}");
        }
    }
}
