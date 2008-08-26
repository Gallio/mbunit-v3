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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Framework;
using Gallio.Framework.Assertions;
using Gallio.Framework.Text;
using Gallio.Model.Diagnostics;
using Gallio.Model.Logging;
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
            builder.SetStackTrace("Stack");
            Assert.AreEqual("Stack", builder.ToAssertionFailure().StackTrace);
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
            Assert.Contains(builder.ToAssertionFailure().StackTrace, "AutomaticStackTraceUsedIfNotSet");
        }

        [Test]
        public void CanSetRawExpectedValue()
        {
            AssertionFailureBuilder builder = new AssertionFailureBuilder("Description");
            builder.SetRawExpectedValue("Abc");
            Assert.AreEqual(new[]
            {
                new AssertionFailure.LabeledValue("Expected Value", "\"Abc\"")
            }, builder.ToAssertionFailure().LabeledValues);

            builder.SetRawExpectedValue(null);
            Assert.AreEqual(new[]
            {
                new AssertionFailure.LabeledValue("Expected Value", "null")
            }, builder.ToAssertionFailure().LabeledValues);
        }

        [Test]
        public void CanSetRawActualValue()
        {
            AssertionFailureBuilder builder = new AssertionFailureBuilder("Description");
            builder.SetRawActualValue("Abc");
            Assert.AreEqual(new[]
            {
                new AssertionFailure.LabeledValue("Actual Value", "\"Abc\"")
            }, builder.ToAssertionFailure().LabeledValues);

            builder.SetRawActualValue(null);
            Assert.AreEqual(new[]
            {
                new AssertionFailure.LabeledValue("Actual Value", "null")
            }, builder.ToAssertionFailure().LabeledValues);
        }

        [Test]
        public void CanSetRawLabeledValue()
        {
            AssertionFailureBuilder builder = new AssertionFailureBuilder("Description");
            builder.SetRawLabeledValue("Abc", 123);
            builder.SetRawLabeledValue("Def", 3.0m);
            Assert.AreEqual(new[]
            {
                new AssertionFailure.LabeledValue("Abc", "123"),
                new AssertionFailure.LabeledValue("Def", "3.0m")
            }, builder.ToAssertionFailure().LabeledValues);

            builder.SetRawLabeledValue("Abc", null);
            Assert.AreEqual(new[]
            {
                new AssertionFailure.LabeledValue("Def", "3.0m"),
                new AssertionFailure.LabeledValue("Abc", "null")
            }, builder.ToAssertionFailure().LabeledValues);
        }

        [Test]
        public void CanSetFormattedLabeledValueAsPlainTextString()
        {
            AssertionFailureBuilder builder = new AssertionFailureBuilder("Description");
            builder.SetLabeledValue("Abc", "123");
            Assert.AreEqual(new[]
            {
                new AssertionFailure.LabeledValue("Abc", "123")
            }, builder.ToAssertionFailure().LabeledValues);
        }

        [Test]
        public void CanSetFormattedLabeledValueAsStructuredTextString()
        {
            AssertionFailureBuilder builder = new AssertionFailureBuilder("Description");
            builder.SetLabeledValue("Abc", new StructuredText("123"));
            Assert.AreEqual(new[]
            {
                new AssertionFailure.LabeledValue("Abc", new StructuredText("123"))
            }, builder.ToAssertionFailure().LabeledValues);
        }

        [Test]
        public void CanSetFormattedLabeledValueAsLabeledValueStruct()
        {
            AssertionFailureBuilder builder = new AssertionFailureBuilder("Description");
            builder.SetLabeledValue(new AssertionFailure.LabeledValue("Abc", new StructuredText("123")));
            Assert.AreEqual(new[]
            {
                new AssertionFailure.LabeledValue("Abc", new StructuredText("123"))
            }, builder.ToAssertionFailure().LabeledValues);
        }

        [Test]
        public void ShowsExpectedAndActualValueWithDiffs_ReferentialEquality()
        {
            const string str = "123";

            AssertionFailureBuilder builder = new AssertionFailureBuilder("description");
            builder.SetRawExpectedAndActualValueWithDiffs(str, str);

            Assert.AreEqual(new[]
            {
                new AssertionFailure.LabeledValue("Expected & Actual Value", new StructuredText("\"123\"")),
                new AssertionFailure.LabeledValue("Remark", "The expected and actual values are the same instance.")
            }, builder.ToAssertionFailure().LabeledValues);
        }

        [Test]
        public void ShowsExpectedAndActualValueWithDiffs_RepresentationalEquality()
        {
            AssertionFailureBuilder builder = new AssertionFailureBuilder("description");
            builder.SetRawExpectedAndActualValueWithDiffs(1, 1u);

            Assert.AreEqual(new[]
            {
                new AssertionFailure.LabeledValue("Expected & Actual Value", new StructuredText("1")),
                new AssertionFailure.LabeledValue("Remark", "The expected and actual values are distinct instances but their formatted representations look the same.")
            }, builder.ToAssertionFailure().LabeledValues);
        }

        [Test]
        public void ShowsExpectedAndActualValueWithDiffs_Difference()
        {
            AssertionFailureBuilder builder = new AssertionFailureBuilder("description");
            builder.SetRawExpectedAndActualValueWithDiffs("acde", "bcef");

            DiffSet diffSet = DiffSet.GetDiffSet("\"acde\"", "\"bcef\"");
            StructuredTextWriter expectedValueWriter = new StructuredTextWriter();
            diffSet.WriteTo(expectedValueWriter, DiffStyle.LeftOnly);
            StructuredTextWriter actualValueWriter = new StructuredTextWriter();
            diffSet.WriteTo(actualValueWriter, DiffStyle.RightOnly);

            Assert.AreEqual(new[]
            {
                new AssertionFailure.LabeledValue("Expected Value", expectedValueWriter.ToStructuredText()),
                new AssertionFailure.LabeledValue("Actual Value", actualValueWriter.ToStructuredText())
            }, builder.ToAssertionFailure().LabeledValues);
        }

        [Test]
        public void TruncatesDiffContextWhenTooLong()
        {
            string expectedValue = "z" + new string('x', AssertionFailure.MaxFormattedValueLength) + "z";
            string actualValue = "Z" + new string('x', AssertionFailure.MaxFormattedValueLength) + "Z";

            AssertionFailureBuilder builder = new AssertionFailureBuilder("description");
            builder.SetRawExpectedAndActualValueWithDiffs(expectedValue, actualValue);
            AssertionFailure failure = builder.ToAssertionFailure();
            TestLog.Write(failure);

            int split = AssertionFailureBuilder.CompressedDiffContextLength / 2;
            Assert.AreEqual("\"z" + new string('x', split) + "..." + new string('x', split) + "z\"",
                failure.LabeledValues[0].FormattedValue.ToString());
            Assert.AreEqual("\"Z" + new string('x', split) + "..." + new string('x', split) + "Z\"",
                failure.LabeledValues[1].FormattedValue.ToString());
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

            Assert.Over.Sequence(new[] { "Boom 1", "Boom 2" }, builder.ToAssertionFailure().Exceptions,
                (expectedSubstring, actual) => Assert.Contains(actual.ToString(), expectedSubstring));
        }
    }
}
