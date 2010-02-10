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
using Gallio.Common.Diagnostics;
using Gallio.Common.Markup;
using MbUnit.Framework;

namespace Gallio.Tests.Common.Diagnostics
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
            new ExceptionData(null, "message", "stacktrace", ExceptionData.NoProperties, null);
        }

        [Test, ExpectedArgumentNullException]
        public void ConstructorThrowsIfMessageIsNull()
        {
            new ExceptionData("type", null, "stacktrace", ExceptionData.NoProperties, null);
        }

        [Test, ExpectedArgumentNullException]
        public void ConstructorThrowsIfStackTraceIsNull()
        {
            new ExceptionData("type", "message", (string)null, ExceptionData.NoProperties, null);
        }

        [Test, ExpectedArgumentNullException]
        public void ConstructorThrowsIfStackTraceDataIsNull()
        {
            new ExceptionData("type", "message", (StackTraceData)null, ExceptionData.NoProperties, null);
        }

        [Test, ExpectedArgumentNullException]
        public void ConstructorThrowsIfPropertiesIsNull()
        {
            new ExceptionData("type", "message", "stacktrace", null, null);
        }

        [Test]
        public void PopulatesPropertiesFromException()
        {
            Exception inner = new Exception("Foo");
            PopulateStackTrace(inner);

            SampleException outer = new SampleException("Bar", inner)
            {
                Prop1 = "Value1",
                Prop2 = "Value2"
            };
            PopulateStackTrace(outer);

            ExceptionData outerData = new ExceptionData(outer);
            Assert.AreEqual(outer.GetType().FullName, outerData.Type);
            Assert.AreEqual(outer.Message, outerData.Message);
            Assert.AreEqual(new PropertySet() {
                    { "Prop1", "Value1" },
                    { "Prop2", "Value2" }
                }, outerData.Properties);
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
            ExceptionData innerData = new ExceptionData("type", "message", "stacktrace", ExceptionData.NoProperties, null);
            ExceptionData outerData = new ExceptionData("type", "message", "stacktrace", 
                new PropertySet() {
                    { "Prop1", "Value1" },
                    { "Prop2", "Value2" }
                }, innerData);

            Assert.AreEqual("type", innerData.Type);
            Assert.AreEqual("message", innerData.Message);
            Assert.AreEqual("stacktrace", innerData.StackTrace.ToString());
            Assert.IsNull(innerData.InnerException);

            Assert.AreEqual("type", outerData.Type);
            Assert.AreEqual("message", outerData.Message);
            Assert.AreEqual("stacktrace", outerData.StackTrace.ToString());
            Assert.AreEqual(new PropertySet() {
                    { "Prop1", "Value1" },
                    { "Prop2", "Value2" }
                }, outerData.Properties);
            Assert.AreSame(innerData, outerData.InnerException);
        }

        [Test]
        public void WriteToThrowsIfArgumentIsNull()
        {
            ExceptionData data = new ExceptionData("type", "message", "stacktrace", ExceptionData.NoProperties, null);
            Assert.Throws<ArgumentNullException>(() => data.WriteTo(null));
        }

        [Test]
        public void ToStringBareBones()
        {
            ExceptionData data = new ExceptionData("type", "message", "stacktrace", ExceptionData.NoProperties, null);
            Assert.AreEqual("type: message\nstacktrace",
                data.ToString());
        }

        [Test]
        [Row(false), Row(true)]
        public void ToStringEverything(bool useStandardFormatting)
        {
            ExceptionData innerData = new ExceptionData("type", "message", "stacktrace", ExceptionData.NoProperties, null);
            ExceptionData outerData = new ExceptionData("type", "message", "stacktrace", 
                new PropertySet() {
                    { "Prop1", "Value1" },
                    { "Prop2", "Value2" }
                }, innerData);
            Assert.AreEqual("type: message ---> type: message\n"
                + "stacktrace\n   --- End of inner exception stack trace ---\n"
                + (useStandardFormatting ? "" : "Prop1: Value1\nProp2: Value2\n")
                + "stacktrace",
                outerData.ToString(useStandardFormatting));
        }

        [Test]
        public void WriteToBareBones()
        {
            ExceptionData data = new ExceptionData("type", "message", "stacktrace", ExceptionData.NoProperties, null);
            StringMarkupDocumentWriter writer = new StringMarkupDocumentWriter(true);
            data.WriteTo(writer.Failures);

            Assert.AreEqual("[Marker \'Exception\'][Marker \'ExceptionType\']type[End]: [Marker \'ExceptionMessage\']message[End]\n[Marker \'StackTrace\']stacktrace[End][End]",
                writer.ToString());
        }

        [Test]
        [Row(false), Row(true)]
        public void WriteToEverything(bool useStandardFormatting)
        {
            ExceptionData innerData = new ExceptionData("type", "message", "stacktrace", ExceptionData.NoProperties, null);
            ExceptionData outerData = new ExceptionData("type", "message", "stacktrace",
                new PropertySet() {
                    { "Prop1", "Value1" },
                    { "Prop2", "Value2" }
                }, innerData);

            StringMarkupDocumentWriter writer = new StringMarkupDocumentWriter(true);
            outerData.WriteTo(writer.Failures, useStandardFormatting);

            Assert.AreEqual("[Marker \'Exception\'][Marker \'ExceptionType\']type[End]: [Marker \'ExceptionMessage\']message[End] ---> [Marker \'Exception\'][Marker \'ExceptionType\']type[End]: [Marker \'ExceptionMessage\']message[End]\n"
                + "[Marker \'StackTrace\']stacktrace[End][End]\n   --- End of inner exception stack trace ---\n"
                + (useStandardFormatting ? "" : "[Marker \'ExceptionPropertyName\']Prop1[End]: [Marker \'ExceptionPropertyValue\']Value1[End]\n[Marker \'ExceptionPropertyName\']Prop2[End]: [Marker \'ExceptionPropertyValue\']Value2[End]\n")
                + "[Marker \'StackTrace\']stacktrace[End][End]",
                writer.ToString());
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

        private class SampleException : Exception
        {
            public SampleException(string message, Exception innerException)
                : base(message, innerException)
            {
            }

            public object Prop1 { get; set; }

            public string Prop2 { get; set; }

            // will be ignored because the value is null
            public string PropNull { get { return null; } }

            // will be ignored because of the attribute
            [SystemInternal]
            public object PropInternal { get; set; }
        }
    }
}
