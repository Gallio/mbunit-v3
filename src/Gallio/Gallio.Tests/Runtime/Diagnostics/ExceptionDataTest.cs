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
using Gallio.Runtime.Diagnostics;
using Gallio.Model.Logging;
using MbUnit.Framework;

namespace Gallio.Tests.Runtime.Diagnostics
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
            new ExceptionData("type", "message", (string) null, null);
        }

        [Test, ExpectedArgumentNullException]
        public void ConstructorThrowsIfStackTraceDataIsNull()
        {
            new ExceptionData("type", "message", (StackTraceData)null, null);
        }

        [Test]
        public void PopulatesPropertiesFromException()
        {
            Exception inner = new Exception("Foo");
            PopulateStackTrace(inner);

            Exception outer = new Exception("Bar", inner);
            PopulateStackTrace(outer);

            ExceptionData outerData = new ExceptionData(outer);
            Assert.AreEqual(outer.GetType().FullName, outerData.Type);
            Assert.AreEqual(outer.Message, outerData.Message);
            Assert.AreEqual(outer.StackTrace, outerData.StackTrace.ToString());
            Assert.IsNotNull(outerData.InnerException);

            ExceptionData innerData = outerData.InnerException;
            Assert.AreEqual(inner.GetType().FullName, innerData.Type);
            Assert.AreEqual(inner.Message, innerData.Message);
            Assert.AreEqual(inner.StackTrace, innerData.StackTrace.ToString());
            Assert.IsNull(innerData.InnerException);
        }

        [Test]
        public void PopulatesPropertiesFromRawValues()
        {
            ExceptionData innerData = new ExceptionData("type", "message", "stacktrace", null);
            ExceptionData outerData = new ExceptionData("type", "message", "stacktrace", innerData);

            Assert.AreEqual("type", innerData.Type);
            Assert.AreEqual("message", innerData.Message);
            Assert.AreEqual("stacktrace", innerData.StackTrace.ToString());
            Assert.IsNull(innerData.InnerException);

            Assert.AreEqual("type", outerData.Type);
            Assert.AreEqual("message", outerData.Message);
            Assert.AreEqual("stacktrace", outerData.StackTrace.ToString());
            Assert.AreSame(innerData, outerData.InnerException);
        }

        [Test]
        public void WriteToThrowsIfArgumentIsNull()
        {
            ExceptionData data = new ExceptionData("type", "message", "stacktrace", null);
            Assert.Throws<ArgumentNullException>(() => data.WriteTo(null));
        }

        [Test]
        public void ToStringBareBones()
        {
            ExceptionData data = new ExceptionData("type", "message", "stacktrace", null);
            Assert.AreEqual("type: message\nstacktrace", data.ToString());
        }

        [Test]
        public void ToStringEverything()
        {
            ExceptionData innerData = new ExceptionData("type", "message", "stacktrace", null);
            ExceptionData outerData = new ExceptionData("type", "message", "stacktrace", innerData);
            Assert.AreEqual("type: message ---> type: message\nstacktrace\n   --- End of inner exception stack trace ---\nstacktrace", outerData.ToString());
        }

        [Test]
        public void WriteToBareBones()
        {
            ExceptionData data = new ExceptionData("type", "message", "stacktrace", null);
            StringTestLogWriter writer = new StringTestLogWriter(true);
            data.WriteTo(writer.Failures);

            Assert.AreEqual("[Marker \'Exception\'][Marker \'ExceptionType\']type[End]: [Marker \'ExceptionMessage\']message[End]\n[Marker \'StackTrace\']stacktrace[End][End]", writer.ToString());
        }

        [Test]
        public void WriteToEverything()
        {
            ExceptionData innerData = new ExceptionData("type", "message", "stacktrace", null);
            ExceptionData outerData = new ExceptionData("type", "message", "stacktrace", innerData);

            StringTestLogWriter writer = new StringTestLogWriter(true);
            outerData.WriteTo(writer.Failures);

            Assert.AreEqual("[Marker \'Exception\'][Marker \'ExceptionType\']type[End]: [Marker \'ExceptionMessage\']message[End] ---> [Marker \'Exception\'][Marker \'ExceptionType\']type[End]: [Marker \'ExceptionMessage\']message[End]\n[Marker \'StackTrace\']stacktrace[End][End]\n   --- End of inner exception stack trace ---\n[Marker \'StackTrace\']stacktrace[End][End]", writer.ToString());
        }

        private static void PopulateStackTrace(Exception ex)
        {
            try
            {
                throw ex;
            }
            catch
            {
            }
        }
    }
}
