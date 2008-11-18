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
using Gallio.Framework.Formatting;
using MbUnit.Framework;

namespace Gallio.Tests.Framework.Formatting
{
    [TestFixture]
    [TestsOn(typeof(StructuralFormattingRule))]
    public class StructuralFormattingRuleTest : BaseFormattingRuleTest<StructuralFormattingRule>
    {
        [Test]
        public void Format()
        {
            SampleObject value = new SampleObject
            {
                Field = 42,
                Property = 101,
                AnotherProperty = null
            };

            string expectedResult = "{Gallio.Tests.Framework.Formatting.StructuralFormattingRuleTest+SampleObject: AnotherProperty = null, Field = {42}, Property = {101}, ReadOnlyProperty = {blah}}";
            Assert.AreEqual(expectedResult, Formatter.Format(value));
        }

        [Test]
        public void Format_ReentrantCallShouldPrintPlaceholderForObjectAndNotCauseStackOverflowException()
        {
            SampleObject value = new SampleObject();
            value.AnotherProperty = value;

            string expectedResult = "{Gallio.Tests.Framework.Formatting.StructuralFormattingRuleTest+SampleObject: AnotherProperty = {...}, Field = {0}, Property = {0}, ReadOnlyProperty = {blah}}";
            Assert.AreEqual(expectedResult, Formatter.Format(value));
        }

        [Test]
        public void Format_ReentrantCallShouldStopAfterTwoLevels()
        {
            SampleObject value = new SampleObject
            {
                AnotherProperty = new SampleObject
                {
                    AnotherProperty = new SampleObject
                    {
                        Field = 3
                    },
                    Field = 2
                },
                Field = 1
            };

            string expectedResult = "{Gallio.Tests.Framework.Formatting.StructuralFormattingRuleTest+SampleObject: AnotherProperty = {Gallio.Tests.Framework.Formatting.StructuralFormattingRuleTest+SampleObject: AnotherProperty = {...}, Field = {2}, Property = {0}, ReadOnlyProperty = {blah}}, Field = {1}, Property = {0}, ReadOnlyProperty = {blah}}";
            Assert.AreEqual(expectedResult, Formatter.Format(value));
        }

        [Test]
        [Row(typeof(SampleObject), FormattingRulePriority.Generic)]
        public void GetPriority(Type type, int? expectedPriority)
        {
            Assert.AreEqual(expectedPriority, FormattingRule.GetPriority(type));
        }

        private sealed class SampleObject
        {
            private int PrivateField = 0;
            private int PrivateProperty { get { return PrivateField; } }

            public int Field;

            public object AnotherProperty { get; set; }

            public int Property { get; set; }

            public int WriteOnlyProperty
            {
                set { throw new NotImplementedException(); }
            }

            public string ReadOnlyProperty
            {
                get { return "blah"; }
            }

            public int ReadOnlyPropertyWithError
            {
                get { throw new NotImplementedException(); }
            }
        }
    }
}
