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
using Gallio.Collections;
using Gallio.Framework.Data.Formatters;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Tests.Framework.Data.Formatters
{
    [TestFixture]
    [TestsOn(typeof(RuleBasedFormatter))]
    public class RuleBasedFormatterTest : BaseUnitTest
    {
        [Test, ExpectedArgumentNullException]
        public void ConstructorThrowsIfRuntimeIsNull()
        {
            new RuleBasedFormatter(null);
        }

        [Test]
        public void NullValueProducesStringContainingNull()
        {
            RuleBasedFormatter formatter = new RuleBasedFormatter(EmptyArray<IFormattingRule>.Instance);
            Assert.AreEqual("null", formatter.Format(null));
        }

        [Test]
        [Column(true, false)]
        public void FormatterSubstitutesNameOfTypeIfRuleReturnsNullOrEmpty(bool useNull)
        {
            IFormattingRule rule;
            using (Mocks.Record())
            {
                rule = Mocks.CreateMock<IFormattingRule>();
                Expect.Call(rule.GetPriority(typeof(int))).Return(0);
                Expect.Call(rule.Format(null, null)).IgnoreArguments().Return(useNull ? null : "");
            }

            RuleBasedFormatter formatter = new RuleBasedFormatter(new IFormattingRule[] { rule });
            Assert.AreEqual("{System.Int32}", formatter.Format(42));
        }

        [Test]
        public void FormatterSubstitutesNameOfTypeIfRuleThrows()
        {
            IFormattingRule rule;
            using (Mocks.Record())
            {
                rule = Mocks.CreateMock<IFormattingRule>();
                Expect.Call(rule.GetPriority(typeof(int))).Return(0);
                Expect.Call(rule.Format(null, null)).IgnoreArguments().Throw(new ApplicationException("Boom!"));
            }

            RuleBasedFormatter formatter = new RuleBasedFormatter(new IFormattingRule[] { rule });
            Assert.AreEqual("{System.Int32}", formatter.Format(42));
        }

        [Test]
        public void FormatterChoosesTheBestRuleAndCachesLookups()
        {
            IFormattingRule rule1;
            IFormattingRule rule2;
            using (Mocks.Record())
            {
                rule1 = Mocks.CreateMock<IFormattingRule>();
                Expect.Call(rule1.GetPriority(typeof(int))).Return(0);

                rule2 = Mocks.CreateMock<IFormattingRule>();
                Expect.Call(rule2.GetPriority(typeof(int))).Return(1);

                Expect.Call(rule2.Format(null, null)).IgnoreArguments().Return("42");
                Expect.Call(rule2.Format(null, null)).IgnoreArguments().Return("53");
            }

            RuleBasedFormatter formatter = new RuleBasedFormatter(new IFormattingRule[] { rule1, rule2 });
            Assert.AreEqual("42", formatter.Format(42));
            Assert.AreEqual("53", formatter.Format(53));
        }
    }
}