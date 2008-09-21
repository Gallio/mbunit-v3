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
using Gallio.Framework.Assertions;
using MbUnit.Framework;

namespace MbUnit.Tests.Framework
{
    [TestsOn(typeof(Assert))]
    public class AssertTest_Comparisons : BaseAssertTest
    {
        #region AreEqual

        [Test]
        public void AreEqual_simple_Test()
        {
            Assert.AreEqual("1", "1");
        }

        [Test]
        public void AreEqual_with_IEqualityComparer()
        {
            Assert.AreEqual("1", "1", new AssertTest.TestComparer { EqualsReturn = true });
        }

        [Test]
        public void AreEqual_with_comparer_delegate()
        {
            Assert.AreEqual("1", "1", (left, right) => left.Length == right.Length);
        }

        [Test]
        public void AreEqual_fails_when_simple_values_different()
        {
            AssertionFailure[] failures = Capture(() => Assert.AreEqual(1, 2));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("Expected values to be equal.", failures[0].Description);
            Assert.AreEqual("Expected Value", failures[0].LabeledValues[0].Label);
            Assert.AreEqual("1", failures[0].LabeledValues[0].FormattedValue.ToString());
            Assert.AreEqual("Actual Value", failures[0].LabeledValues[1].Label);
            Assert.AreEqual("2", failures[0].LabeledValues[1].FormattedValue.ToString());
        }

        [Test]
        public void AreEqual_fails_when_expected_value_is_null()
        {
            AssertionFailure[] failures = Capture(() => Assert.AreEqual(null, "2"));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("null", failures[0].LabeledValues[0].FormattedValue.ToString());
            Assert.AreEqual("\"2\"", failures[0].LabeledValues[1].FormattedValue.ToString());
        }

        [Test]
        public void AreEqual_fails_when_actual_value_is_null()
        {
            AssertionFailure[] failures = Capture(() => Assert.AreEqual("2", null));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("\"2\"", failures[0].LabeledValues[0].FormattedValue.ToString());
            Assert.AreEqual("null", failures[0].LabeledValues[1].FormattedValue.ToString());
        }

        [Test]
        public void AreEqual_fails_with_IEnumrables()
        {
            AssertionFailure[] failures = Capture(() =>
                Assert.AreEqual(new List<string>(new[] { "1", "2" }), new List<string>(new[] { "1", "2", "3" })));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("[\"1\", \"2\"]", failures[0].LabeledValues[0].FormattedValue.ToString());
            Assert.AreEqual("[\"1\", \"2\", \"3\"]", failures[0].LabeledValues[1].FormattedValue.ToString());
        }

        [Test]
        public void AreEqual_fails_with_custom_message()
        {
            AssertionFailure[] failures = Capture(() => Assert.AreEqual(1, 2, "{0} message", "custom"));
            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("custom message", failures[0].Message);
        }

        #endregion

    }
}
