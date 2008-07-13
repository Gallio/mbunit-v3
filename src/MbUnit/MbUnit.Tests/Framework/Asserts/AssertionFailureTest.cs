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
using System.IO;
using System.Linq;
using System.Text;
using Gallio.Framework;
using Gallio.Model.Execution;
using MbUnit.Framework;

namespace MbUnit.Tests.Framework
{
    [TestsOn(typeof(AssertionFailure))]
    [DependsOn(typeof(AssertionFailureBuilderTest))]
    public class AssertionFailureTest
    {
        [Test]
        public void LogThrowsIfArgumentIsNull()
        {
            AssertionFailure failure = new AssertionFailureBuilder("Description").ToAssertionFailure();
            NewAssert.Throws<ArgumentNullException>(() => failure.Log(null));
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
                .SetExpectedValue("Expected value")
                .SetActualValue("Actual value")
                .SetLabeledValue("Very Long Label That Will Not Be Padded", "")
                .SetLabeledValue("x", 42)
                .AddException(new Exception("Boom"))
                .AddException(new Exception("Kaput"))
                .ToAssertionFailure();
            NewAssert.AreEqual("Description\nMessage goes here\n* Expected Value : \"Expected value\"\n* Actual Value   : \"Actual value\"\n* Very Long Label That Will Not Be Padded : \"\"\n* x              : 42\n\nSystem.Exception: Boom\n\nSystem.Exception: Kaput\n\nStack goes here\n", failure.ToString());
        }

        [Test]
        public void LogBareBones()
        {
            AssertionFailure failure = new AssertionFailureBuilder("Description")
                .SetStackTrace(null)
                .ToAssertionFailure();
            StubLogWriter writer = new StubLogWriter();
            failure.Log(new TestLogWriter(writer));

            NewAssert.AreEqual(">>> Description\n<<<\n", writer.Output.ToString());
        }

        [Test]
        public void LogBareEverything()
        {
            AssertionFailure failure = new AssertionFailureBuilder("Description")
                .SetMessage("Message goes here")
                .SetStackTrace("Stack goes here")
                .SetExpectedValue("Expected value")
                .SetActualValue("Actual value")
                .SetLabeledValue("Very Long Label That Will Not Be Padded", "")
                .SetLabeledValue("x", 42)
                .AddException(new Exception("Boom"))
                .AddException(new Exception("Kaput"))
                .ToAssertionFailure();
            StubLogWriter writer = new StubLogWriter();
            failure.Log(new TestLogWriter(writer));

            NewAssert.AreEqual(">>> Description\nMessage goes here\n* Expected Value : \"Expected value\"\n* Actual Value   : \"Actual value\"\n* Very Long Label That Will Not Be Padded : \"\"\n* x              : 42\n\nSystem.Exception: Boom\n\nSystem.Exception: Kaput\n\nStack goes here\n<<<\n", writer.Output.ToString());
        }

        private sealed class StubLogWriter : ITestLogWriter
        {
            public StubLogWriter()
            {
                Output = new StringWriter();
                Output.NewLine = "\n";
            }

            public StringWriter Output { get; private set; }

            public void Close()
            {
                throw new System.NotImplementedException();
            }

            public void AttachText(string attachmentName, string contentType, string text)
            {
                throw new System.NotImplementedException();
            }

            public void AttachBytes(string attachmentName, string contentType, byte[] bytes)
            {
                throw new System.NotImplementedException();
            }

            public void Write(string streamName, string text)
            {
                NewAssert.AreEqual(LogStreamNames.Failures, streamName);
                Output.Write(text);
            }

            public void Embed(string streamName, string attachmentName)
            {
                throw new System.NotImplementedException();
            }

            public void BeginSection(string streamName, string sectionName)
            {
                NewAssert.AreEqual(LogStreamNames.Failures, streamName);
                Output.Write(">>> ");
                Output.WriteLine(sectionName);
            }

            public void EndSection(string streamName)
            {
                NewAssert.AreEqual(LogStreamNames.Failures, streamName);
                Output.WriteLine("<<<");
            }
        }
    }
}
