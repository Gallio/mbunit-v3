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

using Gallio.Model.Schema;
using Gallio.Common.Reflection;
using Gallio.Runner.Reports.Schema;
using Gallio.Tests;
using MbUnit.Framework;
using MbUnit.Framework.Xml;
using System;

namespace MbUnit.Tests.Framework.Xml
{
    [TestFixture]
    public class DiffTest
    {
        [Test]
        [ExpectedArgumentNullException]
        public void Constructs_with_null_path_should_throw_exception()
        {
            new Diff(null, "Message", "Expected", "Actual");
        }

        [Test]
        [ExpectedArgumentNullException]
        public void Constructs_with_null_message_should_throw_exception()
        {
            new Diff("Path", null, "Expected", "Actual");
        }

        [Test]
        [ExpectedArgumentNullException]
        public void Constructs_with_null_expected_value_should_throw_exception()
        {
            new Diff("Path", "Message", null, "Actual");
        }

        [Test]
        [ExpectedArgumentNullException]
        public void Constructs_with_null_actual_value_should_throw_exception()
        {
            new Diff("Path", "Message", "Expected", null);
        }

        [Test]
        public void Constructs_ok()
        {
            var diff = new Diff("Path", "Message", "Expected", "Actual");
            Assert.AreEqual("Path", diff.Path);
            Assert.AreEqual("Message", diff.Message);
            Assert.AreEqual("Expected", diff.Expected);
            Assert.AreEqual("Actual", diff.Actual);
        }

        [Test]
        public void ToString_without_expected_and_actual_values()
        {
            var diff = new Diff("Path", "This is the message.", String.Empty, String.Empty);
            Assert.AreEqual("This is the message.", diff.ToString());
        }

        [Test]
        public void ToString_with_expected_and_actual_values()
        {
            var diff = new Diff("Path", "This is the message.", "ExpectedValue", "ActualValue");
            Assert.AreEqual("This is the message. Expected = 'ExpectedValue'. Found = 'ActualValue'.", diff.ToString());
        }

        [Test]
        public void ToString_with_expected_value_only()
        {
            var diff = new Diff("Path", "This is the message.", "ExpectedValue", String.Empty);
            Assert.AreEqual("This is the message. Expected = 'ExpectedValue'.", diff.ToString());
        }

        [Test]
        public void ToString_with_actual_value_only()
        {
            var diff = new Diff("Path", "This is the message.", String.Empty, "ActualValue");
            Assert.AreEqual("This is the message. Found = 'ActualValue'.", diff.ToString());
        }
    }
}
