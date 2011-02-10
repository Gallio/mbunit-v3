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
using System.Linq;
using Gallio.Framework;
using Gallio.Framework.Assertions;
using Gallio.Common.Text;
using Gallio.Common.Diagnostics;
using Gallio.Common.Markup;
using MbUnit.Framework;

namespace Gallio.Tests.Framework.Assertions
{
    [TestsOn(typeof(AssertionFailureBuilder))]
    public class AssertionFailureBuilderTest
    {
        [Test, ExpectedArgumentNullException]
        public void ConstructorThrowsExceptionWhenDescriptionIsNull()
        {
            new AssertionFailureBuilder(null);
        }

        [Test, ExpectedArgumentNullException]
        public void ConstructorThrowsExceptionWhenFormatterIsNull()
        {
            new AssertionFailureBuilder("abc", null);
        }

        [Test]
        public void ConstructorSetsDescription()
        {
            AssertionFailureBuilder builder = new AssertionFailureBuilder("Description");
            Assert.AreEqual("Description", builder.ToAssertionFailure().Description);
        }

        [Test]
        public void CanSetMessage()
        {
            AssertionFailureBuilder builder = new AssertionFailureBuilder("Description");
            builder.SetMessage("Message");
            Assert.AreEqual("Message", builder.ToAssertionFailure().Message);

            builder.SetMessage(null);
            Assert.IsNull(builder.ToAssertionFailure().Message);

            builder.SetMessage("New Message", null);
            Assert.AreEqual("New Message", builder.ToAssertionFailure().Message);

            builder.SetMessage("New Message: {0}", "Hello!");
            Assert.AreEqual("New Message: Hello!", builder.ToAssertionFailure().Message);

            builder.SetMessage(null, null);
            Assert.IsNull(builder.ToAssertionFailure().Message);
        }

        [Test]
        public void CanSetStackTrace()
        {
            AssertionFailureBuilder builder = new AssertionFailureBuilder("Description");
            builder.SetStackTrace(new StackTraceData("Stack"));
            Assert.AreEqual("Stack", builder.ToAssertionFailure().StackTrace.ToString());
        }

        [Test]
        public void CanSetStackTraceToNullToOmit()
        {
            AssertionFailureBuilder builder = new AssertionFailureBuilder("Description");
            builder.SetStackTrace(null);
            Assert.IsNull(builder.ToAssertionFailure().StackTrace);
        }

        [Test]
        public void AutomaticStackTraceUsedIfNotSet()
        {
            AssertionFailureBuilder builder = new AssertionFailureBuilder("Description");
            Assert.Contains(builder.ToAssertionFailure().StackTrace.ToString(), "AutomaticStackTraceUsedIfNotSet");
        }

        [Test]
        public void CanAddRawExpectedValue()
        {
            AssertionFailureBuilder builder = new AssertionFailureBuilder("Description");
            builder.AddRawExpectedValue("Abc");
            Assert.AreElementsEqual(new[]
            {
                new AssertionFailure.LabeledValue("Expected Value", "\"Abc\"")
            }, builder.ToAssertionFailure().LabeledValues);
        }

        [Test]
        public void CanAddRawActualValue()
        {
            AssertionFailureBuilder builder = new AssertionFailureBuilder("Description");
            builder.AddRawActualValue("Abc");
            Assert.AreElementsEqual(new[]
            {
                new AssertionFailure.LabeledValue("Actual Value", "\"Abc\"")
            }, builder.ToAssertionFailure().LabeledValues);
        }

        [Test]
        public void CanAddRawLabeledValue()
        {
            AssertionFailureBuilder builder = new AssertionFailureBuilder("Description");
            builder.AddRawLabeledValue("Abc", 123);
            builder.AddRawLabeledValue("Def", 3.0m);
            Assert.AreElementsEqual(new[]
            {
                new AssertionFailure.LabeledValue("Abc", "123"),
                new AssertionFailure.LabeledValue("Def", "3.0m")
            }, builder.ToAssertionFailure().LabeledValues);
        }

        [Test]
        public void CanAddFormattedLabeledValueAsPlainTextString()
        {
            AssertionFailureBuilder builder = new AssertionFailureBuilder("Description");
            builder.AddLabeledValue("Abc", "123");
            Assert.AreElementsEqual(new[]
            {
                new AssertionFailure.LabeledValue("Abc", "123")
            }, builder.ToAssertionFailure().LabeledValues);
        }

        [Test]
        public void CanAddFormattedLabeledValueAsStructuredTextString()
        {
            AssertionFailureBuilder builder = new AssertionFailureBuilder("Description");
            builder.AddLabeledValue("Abc", new StructuredText("123"));
            Assert.AreElementsEqual(new[]
            {
                new AssertionFailure.LabeledValue("Abc", new StructuredText("123"))
            }, builder.ToAssertionFailure().LabeledValues);
        }

        [Test]
        public void CanAddFormattedLabeledValueAsLabeledValueStruct()
        {
            AssertionFailureBuilder builder = new AssertionFailureBuilder("Description");
            builder.AddLabeledValue(new AssertionFailure.LabeledValue("Abc", new StructuredText("123")));
            Assert.AreElementsEqual(new[]
            {
                new AssertionFailure.LabeledValue("Abc", new StructuredText("123"))
            }, builder.ToAssertionFailure().LabeledValues);
        }

        [Test]
        public void ShowsExpectedAndActualValueWithDiffs_ReferentialEquality()
        {
            const string str = "123";

            AssertionFailureBuilder builder = new AssertionFailureBuilder("description");
            builder.AddRawExpectedAndActualValuesWithDiffs(str, str);

            Assert.AreElementsEqual(new[]
            {
                new AssertionFailure.LabeledValue("Expected Value & Actual Value", new StructuredText("\"123\"")),
                new AssertionFailure.LabeledValue("Remark", "Both values are the same instance.")
            }, builder.ToAssertionFailure().LabeledValues);
        }

        [Test]
        public void ShowsExpectedAndActualValueWithDiffs_RepresentationalEquality()
        {
            AssertionFailureBuilder builder = new AssertionFailureBuilder("description");
            builder.AddRawExpectedAndActualValuesWithDiffs(1, 1u);

            Assert.AreElementsEqual(new[]
            {
                new AssertionFailure.LabeledValue("Expected Value & Actual Value", new StructuredText("1")),
                new AssertionFailure.LabeledValue("Remark", "Both values look the same when formatted but they are distinct instances.")
            }, builder.ToAssertionFailure().LabeledValues);
        }

        [Test]
        public void ShowsExpectedAndActualValueWithDiffs_Difference()
        {
            AssertionFailureBuilder builder = new AssertionFailureBuilder("description");
            builder.AddRawExpectedAndActualValuesWithDiffs("acde", "bcef");

            DiffSet diffSet = DiffSet.GetDiffSet("\"acde\"", "\"bcef\"").Simplify();
            StructuredTextWriter expectedValueWriter = new StructuredTextWriter();
            diffSet.WriteTo(expectedValueWriter, DiffStyle.LeftOnly);
            StructuredTextWriter actualValueWriter = new StructuredTextWriter();
            diffSet.WriteTo(actualValueWriter, DiffStyle.RightOnly);

            Assert.AreElementsEqual(new[]
            {
                new AssertionFailure.LabeledValue("Expected Value", expectedValueWriter.ToStructuredText()),
                new AssertionFailure.LabeledValue("Actual Value", actualValueWriter.ToStructuredText())
            }, builder.ToAssertionFailure().LabeledValues);
        }

        [Test]
        public void ShowsLabeledValuesWithDiffs_Difference()
        {
            AssertionFailureBuilder builder = new AssertionFailureBuilder("description");
            builder.AddRawLabeledValuesWithDiffs("Left", "acde", "Right", "bcef");

            DiffSet diffSet = DiffSet.GetDiffSet("\"acde\"", "\"bcef\"").Simplify();
            StructuredTextWriter expectedValueWriter = new StructuredTextWriter();
            diffSet.WriteTo(expectedValueWriter, DiffStyle.LeftOnly);
            StructuredTextWriter actualValueWriter = new StructuredTextWriter();
            diffSet.WriteTo(actualValueWriter, DiffStyle.RightOnly);

            Assert.AreElementsEqual(new[]
            {
                new AssertionFailure.LabeledValue("Left", expectedValueWriter.ToStructuredText()),
                new AssertionFailure.LabeledValue("Right", actualValueWriter.ToStructuredText())
            }, builder.ToAssertionFailure().LabeledValues);
        }

        [Test]
        public void TruncatesDiffContextWhenTooLong()
        {
            string expectedValue = "z" + new string('x', AssertionFailure.MaxFormattedValueLength) + "z";
            string actualValue = "Z" + new string('x', AssertionFailure.MaxFormattedValueLength) + "Z";

            AssertionFailureBuilder builder = new AssertionFailureBuilder("description");
            builder.AddRawExpectedAndActualValuesWithDiffs(expectedValue, actualValue);
            AssertionFailure failure = builder.ToAssertionFailure();
            TestLog.Write(failure);

            int split = AssertionFailureBuilder.CompressedDiffContextLength / 2;
            Assert.AreEqual("\"z" + new string('x', split) + "..." + new string('x', split) + "z\"",
                failure.LabeledValues[0].FormattedValue.ToString());
            Assert.AreEqual("\"Z" + new string('x', split) + "..." + new string('x', split) + "Z\"",
                failure.LabeledValues[1].FormattedValue.ToString());
        }

        [Test]
        public void AddRawLabeledValuesWithDiffsThrowsIfLeftLabelIsNull()
        {
            AssertionFailureBuilder builder = new AssertionFailureBuilder("Description");
            Assert.Throws<ArgumentNullException>(() => builder.AddRawLabeledValuesWithDiffs(null, "abc", "xxx", "def"));
        }

        [Test]
        public void AddRawLabeledValuesWithDiffsThrowsIfRightLabelIsNull()
        {
            AssertionFailureBuilder builder = new AssertionFailureBuilder("Description");
            Assert.Throws<ArgumentNullException>(() => builder.AddRawLabeledValuesWithDiffs("xxx", "abc", null, "def"));
        }

        [Test]
        public void AddRawLabeledValueThrowsIfLabelIsNull()
        {
            AssertionFailureBuilder builder = new AssertionFailureBuilder("Description");
            Assert.Throws<ArgumentNullException>(() => builder.AddRawLabeledValue(null, "abc"));
        }

        [Test]
        public void AddLabeledValueWithStringThrowsIfLabelIsNull()
        {
            AssertionFailureBuilder builder = new AssertionFailureBuilder("Description");
            Assert.Throws<ArgumentNullException>(() => builder.AddLabeledValue(null, "abc"));
        }

        [Test]
        public void AddLabeledValueWithStringThrowsIfFormattedValueIsNull()
        {
            AssertionFailureBuilder builder = new AssertionFailureBuilder("Description");
            Assert.Throws<ArgumentNullException>(() => builder.AddLabeledValue("xxx", (string)null));
        }

        [Test]
        public void AddLabeledValueWithStructuredTextThrowsIfLabelIsNull()
        {
            AssertionFailureBuilder builder = new AssertionFailureBuilder("Description");
            Assert.Throws<ArgumentNullException>(() => builder.AddLabeledValue(null, new StructuredText("abc")));
        }

        [Test]
        public void AddLabeledValueWithStructuredTextThrowsIfFormattedValueIsNull()
        {
            AssertionFailureBuilder builder = new AssertionFailureBuilder("Description");
            Assert.Throws<ArgumentNullException>(() => builder.AddLabeledValue("xxx", (StructuredText)null));
        }

        [Test]
        public void AddExceptionThrowsIfArgumentIsNull()
        {
            AssertionFailureBuilder builder = new AssertionFailureBuilder("Description");
            Assert.Throws<ArgumentNullException>(() => builder.AddException((Exception) null));
        }

        [Test]
        public void AddExceptionDataThrowsIfArgumentIsNull()
        {
            AssertionFailureBuilder builder = new AssertionFailureBuilder("Description");
            Assert.Throws<ArgumentNullException>(() => builder.AddException((ExceptionData)null));
        }

        [Test]
        public void CanAddExceptions()
        {
            AssertionFailureBuilder builder = new AssertionFailureBuilder("Description");
            builder.AddException(new InvalidOperationException("Boom 1"));
            builder.AddException(new InvalidOperationException("Boom 2"));

            Assert.Over.Pairs(new[] { "Boom 1", "Boom 2" }, builder.ToAssertionFailure().Exceptions,
                (expectedSubstring, actual) => Assert.Contains(actual.ToString(), expectedSubstring));
        }

        [Test]
        public void AddInnerFailureThrowsIfArgumentIsNull()
        {
            AssertionFailureBuilder builder = new AssertionFailureBuilder("Description");
            Assert.Throws<ArgumentNullException>(() => builder.AddInnerFailure(null));
        }

        [Test]
        public void AddInnerFailuresThrowsIfArgumentIsNull()
        {
            AssertionFailureBuilder builder = new AssertionFailureBuilder("Description");
            Assert.Throws<ArgumentNullException>(() => builder.AddInnerFailures(null));
        }

        [Test]
        public void CanAddInnerFailures()
        {
            AssertionFailure inner1 = new AssertionFailureBuilder("Inner1").ToAssertionFailure();
            AssertionFailure inner2 = new AssertionFailureBuilder("Inner2").ToAssertionFailure();
            AssertionFailure inner3 = new AssertionFailureBuilder("Inner3").ToAssertionFailure();

            AssertionFailureBuilder builder = new AssertionFailureBuilder("Description");
            builder.AddInnerFailures(new[] { inner1, inner2 });
            builder.AddInnerFailure(inner3);

            Assert.Over.Pairs(new[] { inner1, inner2, inner3 }, builder.ToAssertionFailure().InnerFailures,
                Assert.AreEqual);
        }

        [Test]
        public void Conditionally_builds_false()
        {
            var failure = new AssertionFailureBuilder("Description")
                .If(false, builder => builder.AddLabeledValue("Label", "Value"))
                .ToAssertionFailure();
            Assert.ForAll(failure.LabeledValues, x => x.Label != "Label");
        }

        [Test]
        public void Conditionally_builds_true()
        {
            var failure = new AssertionFailureBuilder("Description")
                .If(true, builder => builder.AddLabeledValue("Label", "Value"))
                .ToAssertionFailure();
            Assert.Exists(failure.LabeledValues, x => x.Label == "Label");
        }
    }
}
