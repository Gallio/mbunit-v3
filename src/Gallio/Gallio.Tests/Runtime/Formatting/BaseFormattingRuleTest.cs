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

using Gallio.Runtime.Conversions;
using Gallio.Runtime.Extensibility;
using Gallio.Runtime.Formatting;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Tests.Runtime.Formatting
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
        private IExtensionPoints extensionPoints;

        protected IFormatter Formatter
        {
            get
            {
                return formatter;
            }
        }

        protected IFormattingRule FormattingRule
        {
            get
            {
                return formattingRule;
            }
        }

        protected IExtensionPoints ExtensionPoints
        {
            get
            {
                return extensionPoints;
            }
        }

        [SetUp]
        public void SetUpFormatter()
        {
            extensionPoints = new DefaultExtensionPoints();

            IConverter converter = new RuleBasedConverter(extensionPoints, new IConversionRule[]
            {
                new ObjectToStringConversionRule()
            });

            formattingRule = new T();

            formatter = new RuleBasedFormatter(extensionPoints, new IFormattingRule[]
            {
                formattingRule,
                new ConvertToStringFormattingRule(converter)
            });
        }
    }
}
