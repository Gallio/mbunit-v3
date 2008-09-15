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

namespace Gallio.TeamCityIntegration.Tests
{
    [TestsOn(typeof(ServiceMessageWriter))]
    public class ServiceMessageWriterTest
    {
        private ServiceMessageWriter writer;
        private string actualOutput;

        [SetUp]
        public void SetUp()
        {
            actualOutput = null;
            writer = new ServiceMessageWriter(output => actualOutput = output);
        }

        [Test, ExpectedArgumentNullException]
        public void ConstructorThrowsIfActionIsNull()
        {
            new ServiceMessageWriter(null);
        }

        [Test]
        [Row(null, null, ExpectedException = typeof(ArgumentNullException))]
        [Row("abc\n\r|']def",
            "##teamcity[progressMessage 'abc|n|r|||'|]def']")]
        public void TestProgressMessage(string message, string expectedOutput)
        {
            writer.WriteProgressMessage(message);
            Assert.AreEqual(expectedOutput, actualOutput);
        }

        [Test]
        [Row(null, null, ExpectedException=typeof(ArgumentNullException))]
        [Row("abc\n\r|']def",
            "##teamcity[progressStart 'abc|n|r|||'|]def']")]
        public void TestProgressStart(string message, string expectedOutput)
        {
            writer.WriteProgressStart(message);
            Assert.AreEqual(expectedOutput, actualOutput);
        }

        [Test]
        [Row(null, null, ExpectedException = typeof(ArgumentNullException))]
        [Row("abc\n\r|']def",
            "##teamcity[progressFinish 'abc|n|r|||'|]def']")]
        public void TestProgressFinish(string message, string expectedOutput)
        {
            writer.WriteProgressFinish(message);
            Assert.AreEqual(expectedOutput, actualOutput);
        }

        [Test]
        [Row(null, null, ExpectedException = typeof(ArgumentNullException))]
        [Row("abc\n\r|']def",
            "##teamcity[testSuiteStarted name='abc|n|r|||'|]def']")]
        public void TestSuiteStarted(string name, string expectedOutput)
        {
            writer.WriteTestSuiteStarted(name);
            Assert.AreEqual(expectedOutput, actualOutput);
        }

        [Test]
        [Row(null, null, ExpectedException = typeof(ArgumentNullException))]
        [Row("abc\n\r|']def",
            "##teamcity[testSuiteFinished name='abc|n|r|||'|]def']")]
        public void TestSuiteFinished(string name, string expectedOutput)
        {
            writer.WriteTestSuiteFinished(name);
            Assert.AreEqual(expectedOutput, actualOutput);
        }

        [Test]
        [Row(null, null, ExpectedException = typeof(ArgumentNullException))]
        [Row("abc\n\r|']def",
            "##teamcity[testStarted name='abc|n|r|||'|]def']")]
        public void TestStarted(string name, string expectedOutput)
        {
            writer.WriteTestStarted(name);
            Assert.AreEqual(expectedOutput, actualOutput);
        }

        [Test]
        [Row(null, null, ExpectedException = typeof(ArgumentNullException))]
        [Row("abc\n\r|']def",
            "##teamcity[testFinished name='abc|n|r|||'|]def']")]
        public void TestFinished(string name, string expectedOutput)
        {
            writer.WriteTestFinished(name);
            Assert.AreEqual(expectedOutput, actualOutput);
        }

        [Test]
        [Row(null, "abc", null, ExpectedException = typeof(ArgumentNullException))]
        [Row("abc", null, null, ExpectedException = typeof(ArgumentNullException))]
        [Row("abc\n\r|']def", "def\n\r|']ghi",
            "##teamcity[testIgnored name='abc|n|r|||'|]def' message='def|n|r|||'|]ghi']")]
        public void TestIgnored(string name, string message, string expectedOutput)
        {
            writer.WriteTestIgnored(name, message);
            Assert.AreEqual(expectedOutput, actualOutput);
        }

        [Test]
        [Row(null, "abc", null, ExpectedException = typeof(ArgumentNullException))]
        [Row("abc", null, null, ExpectedException = typeof(ArgumentNullException))]
        [Row("abc\n\r|']def", "def\n\r|']ghi",
            "##teamcity[testStdOut name='abc|n|r|||'|]def' out='def|n|r|||'|]ghi']")]
        public void TestStdOut(string name, string text, string expectedOutput)
        {
            writer.WriteTestStdOut(name, text);
            Assert.AreEqual(expectedOutput, actualOutput);
        }

        [Test]
        [Row(null, "abc", null, ExpectedException = typeof(ArgumentNullException))]
        [Row("abc", null, null, ExpectedException = typeof(ArgumentNullException))]
        [Row("abc\n\r|']def", "def\n\r|']ghi",
            "##teamcity[testStdErr name='abc|n|r|||'|]def' out='def|n|r|||'|]ghi']")]
        public void TestStdErr(string name, string text, string expectedOutput)
        {
            writer.WriteTestStdErr(name, text);
            Assert.AreEqual(expectedOutput, actualOutput);
        }

        [Test]
        [Row(null, "abc", "def", null, ExpectedException = typeof(ArgumentNullException))]
        [Row("abc", null, "def", null, ExpectedException = typeof(ArgumentNullException))]
        [Row("abc", "def", null, null, ExpectedException = typeof(ArgumentNullException))]
        [Row("abc\n\r|']def", "def\n\r|']ghi", "ghi\n\r|']jkl",
            "##teamcity[testFailed name='abc|n|r|||'|]def' message='def|n|r|||'|]ghi' details='ghi|n|r|||'|]jkl']")]
        public void TestFailed(string name, string message, string detail, string expectedOutput)
        {
            writer.WriteTestFailed(name, message, detail);
            Assert.AreEqual(expectedOutput, actualOutput);
        }

        [Test]
        [Row(null, "abc", "def", "ghi", "jkl", null, ExpectedException = typeof(ArgumentNullException))]
        [Row("abc", null, "def", "ghi", "jkl", null, ExpectedException = typeof(ArgumentNullException))]
        [Row("abc", "def", null, "ghi", "jkl", null, ExpectedException = typeof(ArgumentNullException))]
        [Row("abc", "def", "ghi", null, "jkl", null, ExpectedException = typeof(ArgumentNullException))]
        [Row("abc", "def", "ghi", "jkl", null, null, ExpectedException = typeof(ArgumentNullException))]
        [Row("abc\n\r|']def", "def\n\r|']ghi", "ghi\n\r|']jkl", "jkl\n\r|']mno", "mno\n\r|']pqr",
            "##teamcity[testFailed name='abc|n|r|||'|]def' type='comparisonFailure' message='def|n|r|||'|]ghi' details='ghi|n|r|||'|]jkl' expected='jkl|n|r|||'|]mno' actual='mno|n|r|||'|]pqr']")]
        public void TestFailedWithComparisonFailure(string name, string message, string detail, string expected, string actual, string expectedOutput)
        {
            writer.WriteTestFailedWithComparisonFailure(name, message, detail, expected, actual);
            Assert.AreEqual(expectedOutput, actualOutput);
        }
    }
}
