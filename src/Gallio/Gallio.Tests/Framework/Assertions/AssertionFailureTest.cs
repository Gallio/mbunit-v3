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
using Gallio.Framework;
using Gallio.Framework.Assertions;
using Gallio.Runtime.Diagnostics;
using Gallio.Model.Logging;
using MbUnit.Framework;

namespace Gallio.Tests.Framework.Assertions
{
    [TestsOn(typeof(AssertionFailure))]
    [DependsOn(typeof(AssertionFailureBuilderTest))]
    public class AssertionFailureTest
    {
        [Test]
        public void WriteToThrowsIfArgumentIsNull()
        {
            AssertionFailure failure = new AssertionFailureBuilder("Description").ToAssertionFailure();
            Assert.Throws<ArgumentNullException>(() => failure.WriteTo(null));
        }

        [Test]
        public void ToStringBareBones()
        {
            AssertionFailure failure = new AssertionFailureBuilder("Description")
                .SetStackTrace(null)
                .ToAssertionFailure();

            Assert.AreEqual("Description\n", failure.ToString());
        }

        [Test]
        public void ToStringEverything()
        {
            AssertionFailure failure = new AssertionFailureBuilder("Description")
                .SetMessage("Message goes here")
                .SetStackTrace(new StackTraceData("Stack goes here"))
                .AddRawExpectedValue("Expected value")
                .AddRawActualValue("Actual value")
                .AddRawLabeledValue("Very Long Label That Will Not Be Padded", "")
                .AddRawLabeledValue("x", 42)
                .AddException(new Exception("Boom"))
                .AddException(new Exception("Kaput"))
                .AddInnerFailure(new AssertionFailureBuilder("Inner").SetStackTrace(null).ToAssertionFailure())
                .ToAssertionFailure();

            Assert.AreEqual("Description\nMessage goes here\n\nExpected Value : \"Expected value\"\nActual Value   : \"Actual value\"\nVery Long Label That Will Not Be Padded : \"\"\nx              : 42\n\nSystem.Exception: Boom\n\nSystem.Exception: Kaput\n\nStack goes here\nInner\n", failure.ToString());
        }

        [Test]
        public void WriteToBareBones()
        {
            AssertionFailure failure = new AssertionFailureBuilder("Description")
                .SetStackTrace(null)
                .ToAssertionFailure();
            TestLog.Write(failure);

            StringTestLogWriter writer = new StringTestLogWriter(true);
            failure.WriteTo(writer.Failures);

            Assert.AreEqual("[Marker \'AssertionFailure\'][Section \'Description\']\n[End]\n[End]", writer.ToString());
        }

        [Test]
        public void WriteToEverything()
        {
            AssertionFailure failure = new AssertionFailureBuilder("Description")
                .SetMessage("Message goes here")
                .SetStackTrace(new StackTraceData("Stack goes here"))
                .AddRawExpectedValue("Expected value")
                .AddRawActualValue("Actual value")
                .AddRawLabeledValue("Very Long Label That Will Not Be Padded", "")
                .AddRawLabeledValue("x", 42)
                .AddException(new Exception("Boom"))
                .AddException(new Exception("Kaput"))
                .AddInnerFailure(new AssertionFailureBuilder("Inner").SetStackTrace(null).ToAssertionFailure())
                .ToAssertionFailure();
            TestLog.Write(failure);

            StringTestLogWriter writer = new StringTestLogWriter(true);
            failure.WriteTo(writer.Failures);

            Assert.AreEqual("[Marker \'AssertionFailure\'][Section \'Description\']\nMessage goes here\n\n[Marker \'Monospace\'][Marker \'Label\']Expected Value : [End]\"Expected value\"\n[Marker \'Label\']Actual Value   : [End]\"Actual value\"\n[Marker \'Label\']Very Long Label That Will Not Be Padded : [End]\"\"\n[Marker \'Label\']x              : [End]42\n[End]\n[Marker \'Exception\'][Marker \'ExceptionType\']System.Exception[End]: [Marker \'ExceptionMessage\']Boom[End][End]\n\n[Marker \'Exception\'][Marker \'ExceptionType\']System.Exception[End]: [Marker \'ExceptionMessage\']Kaput[End][End]\n\n[Marker \'StackTrace\']Stack goes here[End]\n[Marker \'AssertionFailure\'][Section \'Inner\']\n[End]\n[End][End]\n[End]", writer.ToString());
        }

        [Test]
        public void TruncatesLabelsAndFormattedValues()
        {
            AssertionFailure failure = new AssertionFailureBuilder("Description")
                .SetStackTrace(null)
                .AddLabeledValue(new string('x', AssertionFailure.MaxLabelLengthBeforeTruncation + 1),
                    new string('y', AssertionFailure.MaxFormattedValueLength + 1))
                .ToAssertionFailure();

            Assert.AreEqual("Description\n\n"
                + new string('x', AssertionFailure.MaxLabelLengthBeforeTruncation) + "... : "
                + new string('y', AssertionFailure.MaxFormattedValueLength) + "...\n", failure.ToString());
        }
    }
}
