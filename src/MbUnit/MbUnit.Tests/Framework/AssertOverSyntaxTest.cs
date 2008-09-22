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
    [TestsOn(typeof(AssertOverSyntax))]
    public class AssertOverSyntaxTest : BaseAssertTest
    {
        [Test]
        public void Pairs_Success_BothNull()
        {
            AssertionFailure[] failures = AssertTest.Capture(
                () => Assert.Over.Pairs((int[]) null, (int[]) null, Assert.GreaterThanOrEqualTo,
                    "Hello {0}.", "World"));

            Assert.AreEqual(0, failures.Length);
        }

        [Test]
        public void Pairs_Success_BothEmpty()
        {
            AssertionFailure[] failures = AssertTest.Capture(
                () => Assert.Over.Pairs(new int[] { }, new int[] { }, Assert.GreaterThanOrEqualTo,
                    "Hello {0}.", "World"));

            Assert.AreEqual(0, failures.Length);
        }

        [Test]
        public void Pairs_Success_AllPassAssertion()
        {
            AssertionFailure[] failures = AssertTest.Capture(
                () => Assert.Over.Pairs(new[] { 1, 2, 3 }, new[] { -1, 2, 0 }, Assert.GreaterThanOrEqualTo,
                    "Hello {0}.", "World"));

            Assert.AreEqual(0, failures.Length);
        }

        [Test]
        public void Pairs_Failure_AtIndex()
        {
            AssertionFailure[] failures = AssertTest.Capture(
                () => Assert.Over.Pairs(new[] { 1, 2, 3 }, new[] { -1, 2, 4 }, Assert.GreaterThanOrEqualTo,
                    "Hello {0}.", "World"));

            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("Assertion failed on two values at a particular index within both sequences.", failures[0].Description);
            Assert.AreEqual("Hello World.", failures[0].Message);
            Assert.AreElementsEqual(new[] 
                {
                    new AssertionFailure.LabeledValue("Index", "2"),
                    new AssertionFailure.LabeledValue("Left Sequence", "[1, 2, 3]"),
                    new AssertionFailure.LabeledValue("Right Sequence", "[-1, 2, 4]")
                }, failures[0].LabeledValues);

            Assert.AreEqual(1, failures[0].InnerFailures.Count);
        }

        [Test]
        public void Pairs_Failure_LeftShorterThanRight()
        {
            AssertionFailure[] failures = AssertTest.Capture(
                () => Assert.Over.Pairs(new[] { 1, 2 }, new[] { -1, 2, 4 }, Assert.GreaterThanOrEqualTo,
                    "Hello {0}.", "World"));

            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("Expected the left and right sequences to have the same number of elements.", failures[0].Description);
            Assert.AreEqual("Hello World.", failures[0].Message);
            Assert.AreElementsEqual(new[] 
                {
                    new AssertionFailure.LabeledValue("Left Sequence Count", "2"),
                    new AssertionFailure.LabeledValue("Right Sequence Count", "3"),
                    new AssertionFailure.LabeledValue("Left Sequence", "[1, 2]"),
                    new AssertionFailure.LabeledValue("Right Sequence", "[-1, 2, 4]")
                }, failures[0].LabeledValues);

            Assert.AreEqual(0, failures[0].InnerFailures.Count);
        }

        [Test]
        public void Pairs_Failure_LeftLongerThanRight()
        {
            AssertionFailure[] failures = AssertTest.Capture(
                () => Assert.Over.Pairs(new[] { 1, 2, 5, 0 }, new[] { -1, 2, 4 }, Assert.GreaterThanOrEqualTo,
                    "Hello {0}.", "World"));

            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("Expected the left and right sequences to have the same number of elements.", failures[0].Description);
            Assert.AreEqual("Hello World.", failures[0].Message);
            Assert.AreElementsEqual(new[] 
                {
                    new AssertionFailure.LabeledValue("Left Sequence Count", "4"),
                    new AssertionFailure.LabeledValue("Right Sequence Count", "3"),
                    new AssertionFailure.LabeledValue("Left Sequence", "[1, 2, 5, 0]"),
                    new AssertionFailure.LabeledValue("Right Sequence", "[-1, 2, 4]")
                }, failures[0].LabeledValues);

            Assert.AreEqual(0, failures[0].InnerFailures.Count);
        }

        [Test]
        public void Pairs_Failure_LeftNullButNotRight()
        {
            AssertionFailure[] failures = AssertTest.Capture(
                () => Assert.Over.Pairs((int[])null, new[] { -1, 2, 4 }, Assert.GreaterThanOrEqualTo,
                    "Hello {0}.", "World"));

            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("Expected the left and right sequences to either both be null or both be non-null.", failures[0].Description);
            Assert.AreEqual("Hello World.", failures[0].Message);
            Assert.AreElementsEqual(new[] 
                {
                    new AssertionFailure.LabeledValue("Left Sequence", "null"),
                    new AssertionFailure.LabeledValue("Right Sequence", "[-1, 2, 4]")
                }, failures[0].LabeledValues);

            Assert.AreEqual(0, failures[0].InnerFailures.Count);
        }

        [Test]
        public void Pairs_Failure_RightNullButNotLeft()
        {
            AssertionFailure[] failures = AssertTest.Capture(
                () => Assert.Over.Pairs(new[] { -1, 2, 4 }, (int[])null, Assert.GreaterThanOrEqualTo,
                    "Hello {0}.", "World"));

            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("Expected the left and right sequences to either both be null or both be non-null.", failures[0].Description);
            Assert.AreEqual("Hello World.", failures[0].Message);
            Assert.AreElementsEqual(new[] 
                {
                    new AssertionFailure.LabeledValue("Left Sequence", "[-1, 2, 4]"),
                    new AssertionFailure.LabeledValue("Right Sequence", "null")
                }, failures[0].LabeledValues);

            Assert.AreEqual(0, failures[0].InnerFailures.Count);
        }

        [Test]
        public void KeyedPairs_Success_BothNull()
        {
            AssertionFailure[] failures = AssertTest.Capture(
                () => Assert.Over.KeyedPairs(
                    (IDictionary<int, string>)null,
                    (IDictionary<int, string>)null,
                    Assert.GreaterThanOrEqualTo,
                    "Hello {0}.", "World"));

            Assert.AreEqual(0, failures.Length);
        }

        [Test]
        public void KeyedPairs_Success_BothEmpty()
        {
            AssertionFailure[] failures = AssertTest.Capture(
                () => Assert.Over.KeyedPairs(
                    new Dictionary<int, string>(),
                    new Dictionary<int, string>(),
                    Assert.GreaterThanOrEqualTo,
                    "Hello {0}.", "World"));

            Assert.AreEqual(0, failures.Length);
        }

        [Test]
        public void KeyedPairs_Success_AllPassAssertion()
        {
            AssertionFailure[] failures = AssertTest.Capture(
                () => Assert.Over.KeyedPairs(
                    new Dictionary<int, string> { { 1, "a" }, { 2, "c" }},
                    new Dictionary<int, string> { { 1, "a" }, { 2, "b" } },
                    Assert.GreaterThanOrEqualTo,
                    "Hello {0}.", "World"));

            Assert.AreEqual(0, failures.Length);
        }

        [Test]
        public void KeyedPairs_Failure_AtIndex()
        {
            AssertionFailure[] failures = AssertTest.Capture(
                () => Assert.Over.KeyedPairs(
                    new Dictionary<int, string> { { 1, "a" }, { 2, "b" } },
                    new Dictionary<int, string> { { 1, "a" }, { 2, "c" } },
                    Assert.GreaterThanOrEqualTo,
                    "Hello {0}.", "World"));

            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("Assertion failed on two pairs with a particular key in both dictionaries.", failures[0].Description);
            Assert.AreEqual("Hello World.", failures[0].Message);
            Assert.AreElementsEqual(new[] 
                {
                    new AssertionFailure.LabeledValue("Key", "2"),
                    new AssertionFailure.LabeledValue("Left Dictionary", "[1: \"a\", 2: \"b\"]"),
                    new AssertionFailure.LabeledValue("Right Dictionary", "[1: \"a\", 2: \"c\"]")
                }, failures[0].LabeledValues);

            Assert.AreEqual(1, failures[0].InnerFailures.Count);
        }

        [Test]
        public void KeyedPairs_Failure_LeftShorterThanRight()
        {
            AssertionFailure[] failures = AssertTest.Capture(
                () => Assert.Over.KeyedPairs(
                    new Dictionary<int, string> { { 2, "b" } },
                    new Dictionary<int, string> { { 1, "a" }, { 2, "b" } },
                    Assert.GreaterThanOrEqualTo,
                    "Hello {0}.", "World"));

            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("Expected the left and right dictionaries to have the same number of items.", failures[0].Description);
            Assert.AreEqual("Hello World.", failures[0].Message);
            Assert.AreElementsEqual(new[] 
                {
                    new AssertionFailure.LabeledValue("Left Dictionary Count", "1"),
                    new AssertionFailure.LabeledValue("Right Dictionary Count", "2"),
                    new AssertionFailure.LabeledValue("Left Dictionary", "[2: \"b\"]"),
                    new AssertionFailure.LabeledValue("Right Dictionary", "[1: \"a\", 2: \"b\"]")
                }, failures[0].LabeledValues);

            Assert.AreEqual(0, failures[0].InnerFailures.Count);
        }

        [Test]
        public void KeyedPairs_Failure_LeftLongerThanRight()
        {
            AssertionFailure[] failures = AssertTest.Capture(
                () => Assert.Over.KeyedPairs(
                    new Dictionary<int, string> { { 1, "a" }, { 2, "b" } },
                    new Dictionary<int, string> { { 2, "b" } },
                    Assert.GreaterThanOrEqualTo,
                    "Hello {0}.", "World"));

            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("Expected the left and right dictionaries to have the same number of items.", failures[0].Description);
            Assert.AreEqual("Hello World.", failures[0].Message);
            Assert.AreElementsEqual(new[] 
                {
                    new AssertionFailure.LabeledValue("Left Dictionary Count", "2"),
                    new AssertionFailure.LabeledValue("Right Dictionary Count", "1"),
                    new AssertionFailure.LabeledValue("Left Dictionary", "[1: \"a\", 2: \"b\"]"),
                    new AssertionFailure.LabeledValue("Right Dictionary", "[2: \"b\"]")
                }, failures[0].LabeledValues);

            Assert.AreEqual(0, failures[0].InnerFailures.Count);
        }

        [Test]
        public void KeyedPairs_Failure_LeftNullButNotRight()
        {
            AssertionFailure[] failures = AssertTest.Capture(
                () => Assert.Over.KeyedPairs(
                    (IDictionary<int, string>)null,
                    new Dictionary<int, string>(),
                    Assert.GreaterThanOrEqualTo,
                    "Hello {0}.", "World"));

            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("Expected the left and right dictionaries to either both be null or both be non-null.", failures[0].Description);
            Assert.AreEqual("Hello World.", failures[0].Message);
            Assert.AreElementsEqual(new[] 
                {
                    new AssertionFailure.LabeledValue("Left Dictionary", "null"),
                    new AssertionFailure.LabeledValue("Right Dictionary", "[]")
                }, failures[0].LabeledValues);

            Assert.AreEqual(0, failures[0].InnerFailures.Count);
        }

        [Test]
        public void KeyedPairs_Failure_RightNullButNotLeft()
        {
            AssertionFailure[] failures = AssertTest.Capture(
                () => Assert.Over.KeyedPairs(
                    new Dictionary<int, string>(),
                    (IDictionary<int, string>)null,
                    Assert.GreaterThanOrEqualTo,
                    "Hello {0}.", "World"));

            Assert.AreEqual(1, failures.Length);
            Assert.AreEqual("Expected the left and right dictionaries to either both be null or both be non-null.", failures[0].Description);
            Assert.AreEqual("Hello World.", failures[0].Message);
            Assert.AreElementsEqual(new[] 
                {
                    new AssertionFailure.LabeledValue("Left Dictionary", "[]"),
                    new AssertionFailure.LabeledValue("Right Dictionary", "null")
                }, failures[0].LabeledValues);

            Assert.AreEqual(0, failures[0].InnerFailures.Count);
        }
    }
}
