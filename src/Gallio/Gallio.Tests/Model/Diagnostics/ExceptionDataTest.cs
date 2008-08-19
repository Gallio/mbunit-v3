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
using Gallio.Model.Diagnostics;
using Gallio.Model.Logging;
using MbUnit.Framework;

namespace Gallio.Tests.Model.Diagnostics
{
    [TestsOn(typeof(ExceptionData))]
    public class ExceptionDataTest
    {
        [Test, ExpectedArgumentNullException]
        public void ConstructorThrowsIfExceptionIsNull()
        {
            new ExceptionData(null);
        }

        [Test, ExpectedArgumentNullException]
        public void ConstructorThrowsIfTypeIsNull()
        {
            new ExceptionData(null, "message", "stacktrace", null);
        }

        [Test, ExpectedArgumentNullException]
        public void ConstructorThrowsIfMessageIsNull()
        {
            new ExceptionData("type", null, "stacktrace", null);
        }

        [Test, ExpectedArgumentNullException]
        public void ConstructorThrowsIfStackTraceIsNull()
        {
            new ExceptionData("type", "message", null, null);
        }

        [Test]
        public void PopulatesPropertiesFromException()
        {
            Exception inner = new Exception("Foo");
            Exception outer = new Exception("Bar", inner);

            ExceptionData outerData = new ExceptionData(outer);
            NewAssert.AreEqual(outer.GetType().FullName, outerData.Type);
            NewAssert.AreEqual(outer.Message, outerData.Message);
            NewAssert.AreEqual(outer.StackTrace, outerData.StackTrace);
            NewAssert.IsNotNull(outerData.InnerException);

            ExceptionData innerData = outerData.InnerException;
            NewAssert.AreEqual(inner.GetType().FullName, innerData.Type);
            NewAssert.AreEqual(inner.Message, innerData.Message);
            NewAssert.AreEqual(inner.StackTrace, innerData.StackTrace);
            NewAssert.IsNull(innerData.InnerException);
        }

        [Test]
        public void PopulatesPropertiesFromRawValues()
        {
            ExceptionData innerData = new ExceptionData("type", "message", "stacktrace", null);
            ExceptionData outerData = new ExceptionData("type", "message", "stacktrace", innerData);

            NewAssert.AreEqual("type", innerData.Type);
            NewAssert.AreEqual("message", innerData.Message);
            NewAssert.AreEqual("stacktrace", innerData.StackTrace);
            NewAssert.IsNull(innerData.InnerException);

            NewAssert.AreEqual("type", outerData.Type);
            NewAssert.AreEqual("message", outerData.Message);
            NewAssert.AreEqual("stacktrace", outerData.StackTrace);
            NewAssert.AreSame(innerData, outerData.InnerException);
        }

        [Test]
        public void WriteToThrowsIfArgumentIsNull()
        {
            ExceptionData data = new ExceptionData("type", "message", "stacktrace", null);
            NewAssert.Throws<ArgumentNullException>(() => data.WriteTo(null));
        }

        [Test]
        public void ToStringBareBones()
        {
            ExceptionData data = new ExceptionData("type", "message", "stacktrace", null);
            NewAssert.AreEqual("Description\n", data.ToString());
        }

        [Test]
        public void ToStringEverything()
        {
            ExceptionData innerData = new ExceptionData("type", "message", "stacktrace", null);
            ExceptionData outerData = new ExceptionData("type", "message", "stacktrace", innerData);
            NewAssert.AreEqual("Description\n", outerData.ToString());
        }

        [Test]
        public void WriteToBareBones()
        {
            ExceptionData data = new ExceptionData("type", "message", "stacktrace", null);
            StringTestLogWriter writer = new StringTestLogWriter(true);
            data.WriteTo(writer.Failures);

            NewAssert.AreEqual("[Marker 'assertionFailure'][Section 'Description']\n[End]\n[End]", writer.ToString());
        }

        [Test]
        public void WriteToEverything()
        {
            ExceptionData innerData = new ExceptionData("type", "message", "stacktrace", null);
            ExceptionData outerData = new ExceptionData("type", "message", "stacktrace", innerData);

            StringTestLogWriter writer = new StringTestLogWriter(true);
            outerData.WriteTo(writer.Failures);

            NewAssert.AreEqual("[Marker 'assertionFailure'][Section 'Description']\nMessage goes here\n* Expected Value : \"Expected value\"\n* Actual Value   : \"Actual value\"\n* Very Long Label That Will Not Be Padded : \"\"\n* x              : 42\n\n[Marker 'exception'][Marker 'exceptionType']System.Exception[End]: [Marker 'exceptionMessage']Boom[End][End]\n\n[Marker 'exception'][Marker 'exceptionType']System.Exception[End]: [Marker 'exceptionMessage']Kaput[End][End]\n\n[Marker 'stackTrace']Stack goes here\n[End][End]\n[End]", writer.ToString());
        }
    }
}
