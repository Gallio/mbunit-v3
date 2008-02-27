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
using System.Collections.Generic;
using System.Text;

namespace Gallio.Framework.Data.Formatters
{
    /// <summary>
    /// A rule-based formatter uses a set of <see cref="IFormattingRule" />s to
    /// format values appropriately.
    /// </summary>
    public class RuleBasedFormatter : IFormatter
    {
        private readonly List<IFormattingRule> rules;

        /// <summary>
        /// Creates a rule-based formatter.
        /// </summary>
        /// <param name="rules">The rules to use</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="rules"/> is null</exception>
        public RuleBasedFormatter(IEnumerable<IFormattingRule> rules)
        {
            if (rules == null)
                throw new ArgumentNullException("rules");

            this.rules = new List<IFormattingRule>(rules);
        }

        /// <inheritdoc />
        public string Format(object value)
        {
        }
    }
}
