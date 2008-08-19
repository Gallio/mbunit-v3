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
using Gallio.Model.Logging;
using MbUnit.Framework;

namespace MbUnit.Tests.Framework
{
    [TestsOn(typeof(AssertionFailure))]
    [DependsOn(typeof(AssertionFailureBuilderTest))]
    public class AssertionFailureTest
    {
        [Test]
        public void WriteToThrowsIfArgumentIsNull()
        {
            AssertionFailure failure = new AssertionFailureBuilder("Description").ToAssertionFailure();
            NewAssert.Throws<ArgumentNullException>(() => failure.WriteTo(null));
        }

        [Test]
        public void ToStringBareBones()
        {
            AssertionFailure failure = new AssertionFailureBuilder("Description")
                .SetStackTrace(null)
                .ToAssertionFailure();
            NewAssert.AreEqual("Description\n", failure.ToString());
        }

        [Test]
        public void ToStringEverything()
        {
            AssertionFailure failure = new AssertionFailureBuilder("Description")
                .SetMessage("Message goes here")
                .SetStackTrace("Stack goes here")
                .SetRawExpectedValue("Expected value")
                .SetRawActualValue("Actual value")
                .SetRawLabeledValue("Very Long Label That Will Not Be Padded", "")
                .SetRawLabeledValue("x", 42)
                .AddException(new Exception("Boom"))
                .AddException(new Exception("Kaput"))
                .ToAssertionFailure();
            NewAssert.AreEqual("Description\nMessage goes here\n* Expected Value : \"Expected value\"\n* Actual Value   : \"Actual value\"\n* Very Long Label That Will Not Be Padded : \"\"\n* x              : 42\n\nSystem.Exception: Boom\n\nSystem.Exception: Kaput\n\nStack goes here\n", failure.ToString());
        }

        [Test]
        public void WriteToBareBones()
        {
            AssertionFailure failure = new AssertionFailureBuilder("Description")
                .SetStackTrace(null)
                .ToAssertionFailure();
            StringTestLogWriter writer = new StringTestLogWriter(true);
            failure.WriteTo(writer.Failures);

            NewAssert.AreEqual("[Marker 'assertionFailure'][Section 'Description']\n[End]\n[End]", writer.ToString());
        }

        [Test]
        public void WriteToEverything()
        {
            AssertionFailure failure = new AssertionFailureBuilder("Description")
                .SetMessage("Message goes here")
                .SetStackTrace("Stack goes here")
                .SetRawExpectedValue("Expected value")
                .SetRawActualValue("Actual value")
                .SetRawLabeledValue("Very Long Label That Will Not Be Padded", "")
                .SetRawLabeledValue("x", 42)
                .AddException(new Exception("Boom"))
                .AddException(new Exception("Kaput"))
                .ToAssertionFailure();
            StringTestLogWriter writer = new StringTestLogWriter(true);
            failure.WriteTo(writer.Failures);

            NewAssert.AreEqual("[Marker 'assertionFailure'][Section 'Description']\nMessage goes here\n* Expected Value : \"Expected value\"\n* Actual Value   : \"Actual value\"\n* Very Long Label That Will Not Be Padded : \"\"\n* x              : 42\n\n[Marker 'exception'][Marker 'exceptionType']System.Exception[End]: [Marker 'exceptionMessage']Boom[End][End]\n\n[Marker 'exception'][Marker 'exceptionType']System.Exception[End]: [Marker 'exceptionMessage']Kaput[End][End]\n\n[Marker 'stackTrace']Stack goes here\n[End][End]\n[End]", writer.ToString());
        }
    }
}
