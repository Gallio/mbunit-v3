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
using MbUnit.Framework;

namespace MbUnit.Tests.Framework
{
    [TestsOn(typeof(AssertionFailureBuilder))]
    public class AssertionFailureBuilderTest
    {
        [Test, ExpectedArgumentNullException]
        public void ConstructorThrowsExceptionWhenDescriptionIsNull()
        {
            new AssertionFailureBuilder(null);
        }

        [Test]
        public void ConstructorSetsDescription()
        {
            AssertionFailureBuilder builder = new AssertionFailureBuilder("Description");
            NewAssert.AreEqual("Description", builder.ToAssertionFailure().Description);
        }

        [Test]
        public void CanSetMessage()
        {
            AssertionFailureBuilder builder = new AssertionFailureBuilder("Description");
            builder.SetMessage("Message");
            NewAssert.AreEqual("Message", builder.ToAssertionFailure().Message);

            builder.SetMessage(null);
            NewAssert.IsNull(builder.ToAssertionFailure().Message);

            builder.SetMessage("New Message", null);
            NewAssert.AreEqual("New Message", builder.ToAssertionFailure().Message);

            builder.SetMessage("New Message: {0}", "Hello!");
            NewAssert.AreEqual("New Message: Hello!", builder.ToAssertionFailure().Message);

            builder.SetMessage(null, null);
            NewAssert.IsNull(builder.ToAssertionFailure().Message);
        }

        [Test]
        public void CanSetStackTrace()
        {
            AssertionFailureBuilder builder = new AssertionFailureBuilder("Description");
            builder.SetStackTrace("Stack");
            NewAssert.AreEqual("Stack", builder.ToAssertionFailure().StackTrace);
        }

        [Test]
        public void CanSetStackTraceToNullToOmit()
        {
            AssertionFailureBuilder builder = new AssertionFailureBuilder("Description");
            builder.SetStackTrace(null);
            NewAssert.IsNull(builder.ToAssertionFailure().StackTrace);
        }

        [Test]
        public void AutomaticStackTraceUsedIfNotSet()
        {
            AssertionFailureBuilder builder = new AssertionFailureBuilder("Description");
            NewAssert.Contains(builder.ToAssertionFailure().StackTrace, "AutomaticStackTraceUsedIfNotSet");
        }

        [Test]
        public void CanSetExpectedValue()
        {
            AssertionFailureBuilder builder = new AssertionFailureBuilder("Description");
            builder.SetExpectedValue("Abc");
            NewAssert.AreEqual(new[]
            {
                new KeyValuePair<string, string>("Expected Value", "\"Abc\"")
            }, builder.ToAssertionFailure().LabeledValues);

            builder.SetExpectedValue(null);
            NewAssert.AreEqual(new[]
            {
                new KeyValuePair<string, string>("Expected Value", "null")
            }, builder.ToAssertionFailure().LabeledValues);
        }

        [Test]
        public void CanSetActualValue()
        {
            AssertionFailureBuilder builder = new AssertionFailureBuilder("Description");
            builder.SetActualValue("Abc");
            NewAssert.AreEqual(new[]
            {
                new KeyValuePair<string, string>("Actual Value", "\"Abc\"")
            }, builder.ToAssertionFailure().LabeledValues);

            builder.SetActualValue(null);
            NewAssert.AreEqual(new[]
            {
                new KeyValuePair<string, string>("Actual Value", "null")
            }, builder.ToAssertionFailure().LabeledValues);
        }

        [Test]
        public void CanSetLabeledValue()
        {
            AssertionFailureBuilder builder = new AssertionFailureBuilder("Description");
            builder.SetLabeledValue("Abc", 123);
            builder.SetLabeledValue("Def", 3.0m);
            NewAssert.AreEqual(new[]
            {
                new KeyValuePair<string, string>("Abc", "123"),
                new KeyValuePair<string, string>("Def", "3.0m")
            }, builder.ToAssertionFailure().LabeledValues);

            builder.SetLabeledValue("Abc", null);
            NewAssert.AreEqual(new[]
            {
                new KeyValuePair<string, string>("Def", "3.0m"),
                new KeyValuePair<string, string>("Abc", "null")
            }, builder.ToAssertionFailure().LabeledValues);
        }

        [Test]
        public void AddExceptionThrowsIfArgumentIsNull()
        {
            AssertionFailureBuilder builder = new AssertionFailureBuilder("Description");
            NewAssert.Throws<ArgumentNullException>(() => builder.AddException(null));
        }

        [Test]
        public void CanAddExceptions()
        {
            AssertionFailureBuilder builder = new AssertionFailureBuilder("Description");
            builder.AddException(new InvalidOperationException("Boom 1"));
            builder.AddException(new InvalidOperationException("Boom 2"));

            NewAssert.Over.Sequence(new[] { "Boom 1", "Boom 2" }, builder.ToAssertionFailure().Exceptions,
                (expectedSubstring, actual) => NewAssert.Contains(actual, expectedSubstring));
        }
    }
}
