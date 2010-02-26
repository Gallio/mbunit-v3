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
using Gallio.Common.Collections;
using Gallio.Runtime.Extensibility;
using Gallio.Runtime.Formatting;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Gallio.Tests.Runtime.Formatting
{
    [TestFixture]
    [TestsOn(typeof(RuleBasedFormatter))]
    public class RuleBasedFormatterTest
    {
        [Test, ExpectedArgumentNullException]
        public void Constructs_with_null_extensionPoints_should_throw_Exception()
        {
            new RuleBasedFormatter(null, EmptyArray<IFormattingRule>.Instance);
        }

        [Test, ExpectedArgumentNullException]
        public void Constructs_with_null_formatting_rules_should_throw_Exception()
        {
            var mockExtensionPoints = MockRepository.GenerateStub<IExtensionPoints>();
            new RuleBasedFormatter(mockExtensionPoints, null);
        }

        [Test]
        public void NullValueProducesStringContainingNull()
        {
            var mockExtensionPoints = MockRepository.GenerateStub<IExtensionPoints>();
            var formatter = new RuleBasedFormatter(mockExtensionPoints, EmptyArray<IFormattingRule>.Instance);
            Assert.AreEqual("null", formatter.Format(null));
        }

        [Test]
        [Column(true, false)]
        public void FormatterSubstitutesNameOfTypeIfRuleReturnsNullOrEmpty(bool useNull)
        {
            var mockRule = MockRepository.GenerateMock<IFormattingRule>();
            mockRule.Expect(x => x.GetPriority(typeof(int))).Return(0);
            mockRule.Expect(x => x.Format(null, null)).IgnoreArguments().Return(useNull ? null : String.Empty);
            var formatter = new RuleBasedFormatter(new DefaultExtensionPoints(), new[] { mockRule });
            Assert.AreEqual("{System.Int32}", formatter.Format(42));
            mockRule.VerifyAllExpectations();
        }
        
        [Test]
        public void FormatterSubstitutesNameOfTypeIfRuleThrows()
        {
            var mockRule = MockRepository.GenerateMock<IFormattingRule>();
            mockRule.Expect(x => x.GetPriority(typeof(int))).Return(0);
            mockRule.Expect(x => x.Format(null, null)).IgnoreArguments().Throw(new ApplicationException("Boom!"));
            var formatter = new RuleBasedFormatter(new DefaultExtensionPoints(), new[] { mockRule });
            Assert.AreEqual("{System.Int32}", formatter.Format(42));
            mockRule.VerifyAllExpectations();
        }

        [Test]
        public void FormatterChoosesTheBestRuleAndCachesLookups()
        {
            var mockRule1 = MockRepository.GenerateMock<IFormattingRule>();
            var mockRule2 = MockRepository.GenerateMock<IFormattingRule>();
            mockRule1.Expect(x => x.GetPriority(typeof(int))).Return(0);
            mockRule2.Expect(x => x.GetPriority(typeof(int))).Return(1);
            mockRule2.Expect(x => x.Format(null, null)).IgnoreArguments().Repeat.Once().Return("42");
            mockRule2.Expect(x => x.Format(null, null)).IgnoreArguments().Repeat.Once().Return("53");
            var formatter = new RuleBasedFormatter(new DefaultExtensionPoints(), new[] { mockRule1, mockRule2 });
            Assert.AreEqual("42", formatter.Format(42));
            Assert.AreEqual("53", formatter.Format(53));
            mockRule1.VerifyAllExpectations();
            mockRule2.VerifyAllExpectations();
        }

        internal class Foo
        {
            private readonly int value;

            public int Value
            {
                get
                {
                    return value;
                }
            }

            public Foo(int value)
            {
                this.value = value;
            }
        }

        [Test]
        public void Custom_formatting()
        {
            var extentionPoints = new DefaultExtensionPoints();
            var formatter = new RuleBasedFormatter(extentionPoints, EmptyArray<IFormattingRule>.Instance);
            extentionPoints.CustomFormatters.Register<Foo>(x => String.Format("Foo's value is {0}.", x.Value));
            string output = formatter.Format(new Foo(123));
            Assert.AreEqual("Foo's value is 123.", output);
        }
    }
}