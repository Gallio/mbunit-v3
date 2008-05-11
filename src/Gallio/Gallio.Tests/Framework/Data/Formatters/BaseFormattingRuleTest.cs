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

using Gallio.Framework.Data.Conversions;
using Gallio.Framework.Data.Formatters;
using MbUnit.Framework;

namespace Gallio.Tests.Framework.Data.Formatters
{
    /// <summary>
    /// Abstract base class for <see cref="IFormattingRule" /> tests.
    /// Automatically sets up a <see cref="RuleBasedFormatter" /> populated with
    /// the rule and a <see cref="ConvertToStringFormattingRule"/> with a
    /// basic <see cref="ObjectToStringConversionRule"/> converter.
    /// </summary>
    public abstract class BaseFormattingRuleTest<T>
        where T : IFormattingRule, new()
    {
        private IFormatter formatter;
        private IFormattingRule formattingRule;

        public IFormatter Formatter
        {
            get { return formatter; }
        }

        public IFormattingRule FormattingRule
        {
            get { return formattingRule; }
        }

        [SetUp]
        public void SetUpFormatter()
        {
            IConverter converter = new RuleBasedConverter(new IConversionRule[]
            {
                new ObjectToStringConversionRule()
            });

            formattingRule = new T();
            formatter = new RuleBasedFormatter(new IFormattingRule[]
            {
                formattingRule,
                new ConvertToStringFormattingRule(converter)
            });
        }
    }
}
