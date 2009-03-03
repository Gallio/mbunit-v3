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
                Left = null
            };

            string expectedResult = "{Gallio.Tests.Framework.Formatting.StructuralFormattingRuleTest+SampleObject: Field = {42}, Left = null, Property = {101}, ReadOnlyProperty = {blah}, Right = null}";
            Assert.AreEqual(expectedResult, Formatter.Format(value));
        }

        [Test]
        public void Format_CyclicGraphShouldPrintPlaceholderForObjectAndNotCauseStackOverflowException()
        {
            SampleObject value = new SampleObject();
            value.Left = value;

            string expectedResult = "{Gallio.Tests.Framework.Formatting.StructuralFormattingRuleTest+SampleObject: Field = {0}, Left = {...}, Property = {0}, ReadOnlyProperty = {blah}, Right = null}";
            Assert.AreEqual(expectedResult, Formatter.Format(value));
        }

        [Test]
        public void Format_DeepGraphShouldStopAfterTwoLevels()
        {
            SampleObject value = new SampleObject
            {
                Left = new SampleObject
                {
                    Left = new SampleObject
                    {
                        Field = 3
                    },
                    Field = 2
                },
                Field = 1
            };

            string expectedResult = "{Gallio.Tests.Framework.Formatting.StructuralFormattingRuleTest+SampleObject: Field = {1}, Left = {Gallio.Tests.Framework.Formatting.StructuralFormattingRuleTest+SampleObject: Field = {2}, Left = {...}, Property = {0}, ReadOnlyProperty = {blah}, Right = null}, Property = {0}, ReadOnlyProperty = {blah}, Right = null}";
            Assert.AreEqual(expectedResult, Formatter.Format(value));
        }

        [Test]
        public void Format_ShouldRemoveObjectFromReentanceVisitedSetWhenAllPrinted()
        {
            SampleObject value = new SampleObject();

            string expectedResult = "{Gallio.Tests.Framework.Formatting.StructuralFormattingRuleTest+SampleObject: Field = {0}, Left = null, Property = {0}, ReadOnlyProperty = {blah}, Right = null}";
            Assert.AreEqual(expectedResult, Formatter.Format(value));

            // This is the actual regression test.  Previously the value remained in the visited
            // set such that all subsequent printings were elided.
            Assert.AreEqual(expectedResult, Formatter.Format(value), "Value should be printed again.");
        }

        [Test]
        public void Format_ShouldUseReferentialEqualityForDeterminingReentrance()
        {
            SampleObject value = new SampleObject
            {
                Field = 1,
                Left = new SampleObject
                {
                    Field = 1
                }
            };

            string expectedResult = "{Gallio.Tests.Framework.Formatting.StructuralFormattingRuleTest+SampleObject: Field = {1}, Left = {Gallio.Tests.Framework.Formatting.StructuralFormattingRuleTest+SampleObject: Field = {1}, Left = null, Property = {0}, ReadOnlyProperty = {blah}, Right = null}, Property = {0}, ReadOnlyProperty = {blah}, Right = null}";
            Assert.AreEqual(expectedResult, Formatter.Format(value), "Both the outer and inner objects should be printed even through they are considered equal by value.");
        }

        [Test]
        [Row(typeof(SampleObject), FormattingRulePriority.Generic)]
        public void GetPriority(Type type, int? expectedPriority)
        {
            Assert.AreEqual(expectedPriority, FormattingRule.GetPriority(type));
        }

        private sealed class SampleObject : IEquatable<SampleObject>
        {
            private int PrivateField = 0;
            private int PrivateProperty { get { return PrivateField; } }

            public int Field;

            public object Left { get; set; }
            public object Right { get; set; }

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

            public override bool Equals(object obj)
            {
                return Equals(obj as SampleObject);
            }

            public bool Equals(SampleObject other)
            {
                return other != null
                    && PrivateField == other.PrivateField
                    && Field == other.Field
                    && Property == other.Property;
                    //&& Equals(Left, other.Left)
                    //&& Equals(Right, other.Right)
            }

            public override int GetHashCode()
            {
                return 0;
            }
        }
    }
}
