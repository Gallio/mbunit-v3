// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using Gallio.Common;
using Gallio.Runtime.Conversions;

namespace Gallio.Runtime.Formatting
{
    /// <summary>
    /// Formatting rule for custom user formatters.
    /// </summary>
    public sealed class CustomFormattingRule : IFormattingRule
    {
        private readonly FormattingFunc formatterFunc;

        /// <summary>
        /// Constructs a custom conversion rule.
        /// </summary>
        /// <param name="formatterFunc">The formatting operation.</param>
        public CustomFormattingRule(FormattingFunc formatterFunc)
        {
            if (formatterFunc == null)
                throw new ArgumentNullException("formatterFunc");

            this.formatterFunc = formatterFunc;
        }

        /// <inheritdoc />
        public int? GetPriority(Type type)
        {
            return FormattingRulePriority.Best;
        }

        /// <inheritdoc />
        public string Format(object obj, IFormatter formatter)
        {
            return formatterFunc(obj);
        }
    }
}