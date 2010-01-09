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
        [Row("flow", null, null, ExpectedException = typeof(ArgumentNullException))]
        [Row(null, "abc\n\r|']def",
            "##teamcity[progressMessage 'abc|n|r|||'|]def']")]
        [Row("flow", "abc\n\r|']def",
            "##teamcity[progressMessage 'abc|n|r|||'|]def' flowId='flow']")]
        public void TestProgressMessage(string flowId, string message, string expectedOutput)
        {
            writer.WriteProgressMessage(flowId, message);
            Assert.AreEqual(expectedOutput, actualOutput);
        }

        [Test]
        [Row("flow", null, null, ExpectedException = typeof(ArgumentNullException))]
        [Row(null, "abc\n\r|']def",
            "##teamcity[progressStart 'abc|n|r|||'|]def']")]
        [Row("flow", "abc\n\r|']def",
            "##teamcity[progressStart 'abc|n|r|||'|]def' flowId='flow']")]
        public void TestProgressStart(string flowId, string message, string expectedOutput)
        {
            writer.WriteProgressStart(flowId, message);
            Assert.AreEqual(expectedOutput, actualOutput);
        }

        [Test]
        [Row("flow", null, null, ExpectedException = typeof(ArgumentNullException))]
        [Row(null, "abc\n\r|']def",
            "##teamcity[progressFinish 'abc|n|r|||'|]def']")]
        [Row("flow", "abc\n\r|']def",
            "##teamcity[progressFinish 'abc|n|r|||'|]def' flowId='flow']")]
        public void TestProgressFinish(string flowId, string message, string expectedOutput)
        {
            writer.WriteProgressFinish(flowId, message);
            Assert.AreEqual(expectedOutput, actualOutput);
        }

        [Test]
        [Row("flow", null, null, ExpectedException = typeof(ArgumentNullException))]
        [Row(null, "abc\n\r|']def",
            "##teamcity[testSuiteStarted name='abc|n|r|||'|]def']")]
        [Row("flow", "abc\n\r|']def",
            "##teamcity[testSuiteStarted name='abc|n|r|||'|]def' flowId='flow']")]
        public void TestSuiteStarted(string flowId, string name, string expectedOutput)
        {
            writer.WriteTestSuiteStarted(flowId, name);
            Assert.AreEqual(expectedOutput, actualOutput);
        }

        [Test]
        [Row("flow", null, null, ExpectedException = typeof(ArgumentNullException))]
        [Row(null, "abc\n\r|']def",
            "##teamcity[testSuiteFinished name='abc|n|r|||'|]def']")]
        [Row("flow", "abc\n\r|']def",
            "##teamcity[testSuiteFinished name='abc|n|r|||'|]def' flowId='flow']")]
        public void TestSuiteFinished(string flowId, string name, string expectedOutput)
        {
            writer.WriteTestSuiteFinished(flowId, name);
            Assert.AreEqual(expectedOutput, actualOutput);
        }

        [Test]
        [Row("flow", null, true, null, ExpectedException = typeof(ArgumentNullException))]
        [Row(null, "abc\n\r|']def", true,
            "##teamcity[testStarted name='abc|n|r|||'|]def' captureStandardOutput='true']")]
        [Row(null, "abc\n\r|']def", false,
            "##teamcity[testStarted name='abc|n|r|||'|]def' captureStandardOutput='false']")]
        [Row("flow", "abc\n\r|']def", false,
            "##teamcity[testStarted name='abc|n|r|||'|]def' captureStandardOutput='false' flowId='flow']")]
        public void TestStarted(string flowId, string name, bool captureStandardOutput, string expectedOutput)
        {
            writer.WriteTestStarted(flowId, name, captureStandardOutput);
            Assert.AreEqual(expectedOutput, actualOutput);
        }

        [Test]
        [Row("flow", null, 0, null, ExpectedException = typeof(ArgumentNullException))]
        [Row(null, "abc\n\r|']def", 333,
            "##teamcity[testFinished name='abc|n|r|||'|]def' duration='333']")]
        [Row("flow", "abc\n\r|']def", 333,
            "##teamcity[testFinished name='abc|n|r|||'|]def' duration='333' flowId='flow']")]
        public void TestFinished(string flowId, string name, int durationMillis, string expectedOutput)
        {
            writer.WriteTestFinished(flowId, name, TimeSpan.FromMilliseconds(durationMillis));
            Assert.AreEqual(expectedOutput, actualOutput);
        }

        [Test]
        [Row("flow", null, "abc", null, ExpectedException = typeof(ArgumentNullException))]
        [Row("flow", "abc", null, null, ExpectedException = typeof(ArgumentNullException))]
        [Row(null, "abc\n\r|']def", "def\n\r|']ghi",
            "##teamcity[testIgnored name='abc|n|r|||'|]def' message='def|n|r|||'|]ghi']")]
        [Row("flow", "abc\n\r|']def", "def\n\r|']ghi",
            "##teamcity[testIgnored name='abc|n|r|||'|]def' message='def|n|r|||'|]ghi' flowId='flow']")]
        public void TestIgnored(string flowId, string name, string message, string expectedOutput)
        {
            writer.WriteTestIgnored(flowId, name, message);
            Assert.AreEqual(expectedOutput, actualOutput);
        }

        [Test]
        [Row("flow", null, "abc", null, ExpectedException = typeof(ArgumentNullException))]
        [Row("flow", "abc", null, null, ExpectedException = typeof(ArgumentNullException))]
        [Row(null, "abc\n\r|']def", "def\n\r|']ghi",
            "##teamcity[testStdOut name='abc|n|r|||'|]def' out='def|n|r|||'|]ghi']")]
        [Row("flow", "abc\n\r|']def", "def\n\r|']ghi",
            "##teamcity[testStdOut name='abc|n|r|||'|]def' out='def|n|r|||'|]ghi' flowId='flow']")]
        public void TestStdOut(string flowId, string name, string text, string expectedOutput)
        {
            writer.WriteTestStdOut(flowId, name, text);
            Assert.AreEqual(expectedOutput, actualOutput);
        }

        [Test]
        [Row("flow", null, "abc", null, ExpectedException = typeof(ArgumentNullException))]
        [Row("flow", "abc", null, null, ExpectedException = typeof(ArgumentNullException))]
        [Row(null, "abc\n\r|']def", "def\n\r|']ghi",
            "##teamcity[testStdErr name='abc|n|r|||'|]def' out='def|n|r|||'|]ghi']")]
        [Row("flow", "abc\n\r|']def", "def\n\r|']ghi",
            "##teamcity[testStdErr name='abc|n|r|||'|]def' out='def|n|r|||'|]ghi' flowId='flow']")]
        public void TestStdErr(string flowId, string name, string text, string expectedOutput)
        {
            writer.WriteTestStdErr(flowId, name, text);
            Assert.AreEqual(expectedOutput, actualOutput);
        }

        [Test]
        [Row("flow", null, "abc", "def", null, ExpectedException = typeof(ArgumentNullException))]
        [Row("flow", "abc", null, "def", null, ExpectedException = typeof(ArgumentNullException))]
        [Row("flow", "abc", "def", null, null, ExpectedException = typeof(ArgumentNullException))]
        [Row(null, "abc\n\r|']def", "def\n\r|']ghi", "ghi\n\r|']jkl",
            "##teamcity[testFailed name='abc|n|r|||'|]def' message='def|n|r|||'|]ghi' details='ghi|n|r|||'|]jkl']")]
        [Row("flow", "abc\n\r|']def", "def\n\r|']ghi", "ghi\n\r|']jkl",
            "##teamcity[testFailed name='abc|n|r|||'|]def' message='def|n|r|||'|]ghi' details='ghi|n|r|||'|]jkl' flowId='flow']")]
        public void TestFailed(string flowId, string name, string message, string detail, string expectedOutput)
        {
            writer.WriteTestFailed(flowId, name, message, detail);
            Assert.AreEqual(expectedOutput, actualOutput);
        }

        [Test]
        [Row("flow", null, "abc", "def", "ghi", "jkl", null, ExpectedException = typeof(ArgumentNullException))]
        [Row("flow", "abc", null, "def", "ghi", "jkl", null, ExpectedException = typeof(ArgumentNullException))]
        [Row("flow", "abc", "def", null, "ghi", "jkl", null, ExpectedException = typeof(ArgumentNullException))]
        [Row("flow", "abc", "def", "ghi", null, "jkl", null, ExpectedException = typeof(ArgumentNullException))]
        [Row("flow", "abc", "def", "ghi", "jkl", null, null, ExpectedException = typeof(ArgumentNullException))]
        [Row(null, "abc\n\r|']def", "def\n\r|']ghi", "ghi\n\r|']jkl", "jkl\n\r|']mno", "mno\n\r|']pqr",
            "##teamcity[testFailed name='abc|n|r|||'|]def' type='comparisonFailure' message='def|n|r|||'|]ghi' details='ghi|n|r|||'|]jkl' expected='jkl|n|r|||'|]mno' actual='mno|n|r|||'|]pqr']")]
        [Row("flow", "abc\n\r|']def", "def\n\r|']ghi", "ghi\n\r|']jkl", "jkl\n\r|']mno", "mno\n\r|']pqr",
            "##teamcity[testFailed name='abc|n|r|||'|]def' type='comparisonFailure' message='def|n|r|||'|]ghi' details='ghi|n|r|||'|]jkl' expected='jkl|n|r|||'|]mno' actual='mno|n|r|||'|]pqr' flowId='flow']")]
        public void TestFailedWithComparisonFailure(string flowId, string name, string message, string detail, string expected, string actual, string expectedOutput)
        {
            writer.WriteTestFailedWithComparisonFailure(flowId, name, message, detail, expected, actual);
            Assert.AreEqual(expectedOutput, actualOutput);
        }
    }
}
