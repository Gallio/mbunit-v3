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
using Gallio.Model.Logging;
using Gallio.Reflection;
using Gallio.Runner.Reports;
using Gallio.Model;
using Gallio.Tests;
using MbUnit.TestResources.Fixtures;
using MbUnit.Framework;

namespace MbUnit.Tests.Integration
{
    [TestFixture]
    [RunSample(typeof(ContextOutcomePassingTestSample))]
    [RunSample(typeof(ContextOutcomeFailingSetUpSample))]
    [RunSample(typeof(ContextOutcomeFailingTestSample))]
    [RunSample(typeof(ContextOutcomeFailingTearDownSample))]
    public class ContextOutcomeTest : BaseTestWithSampleRunner
    {
        [Test]
        public void PassingTestOutcomeIsAlwaysPass()
        {
            AssertTestResult("passed\npassed\npassed\n", TestStatus.Passed, typeof(ContextOutcomePassingTestSample), "Test");
            AssertFixtureResult("passed\n", TestStatus.Passed, typeof(ContextOutcomePassingTestSample));
        }

        [Test]
        public void FailedSetUpTestOutcomeBecomesFailedAfterSetUp()
        {
            AssertTestResult("passed\nfailed\n", TestStatus.Failed, typeof(ContextOutcomeFailingSetUpSample), "Test");
            AssertFixtureResult("failed\n", TestStatus.Failed, typeof(ContextOutcomeFailingSetUpSample));
        }

        [Test]
        public void FailedTestOutcomeBecomesFailedAfterTest()
        {
            AssertTestResult("passed\npassed\nfailed\n", TestStatus.Failed, typeof(ContextOutcomeFailingTestSample), "Test");
            AssertFixtureResult("failed\n", TestStatus.Failed, typeof(ContextOutcomeFailingTestSample));
        }

        [Test]
        public void FailedTearDownOutcomeBecomesFailedAfterTearDown()
        {
            AssertTestResult("passed\npassed\npassed\n", TestStatus.Failed, typeof(ContextOutcomeFailingTearDownSample), "Test");
            AssertFixtureResult("failed\n", TestStatus.Failed, typeof(ContextOutcomeFailingTearDownSample));
        }

        private void AssertTestResult(string expectedOutput, TestStatus expectedStatus, Type fixtureType, string methodName)
        {
            CodeReference codeReference = CodeReference.CreateFromMember(fixtureType.GetMethod(methodName));

            TestStepRun run = Runner.GetPrimaryTestStepRun(codeReference);
            Assert.AreEqual(expectedStatus, run.Result.Outcome.Status);

            string actualOutput = run.TestLog.GetStream(TestLogStreamNames.Default).ToString();
            Assert.AreEqual(expectedOutput, actualOutput);
        }

        private void AssertFixtureResult(string expectedOutput, TestStatus expectedStatus, Type fixtureType)
        {
            CodeReference codeReference = CodeReference.CreateFromType(fixtureType);

            TestStepRun run = Runner.GetPrimaryTestStepRun(codeReference);
            Assert.AreEqual(expectedStatus, run.Result.Outcome.Status);

            string actualOutput = run.TestLog.GetStream(TestLogStreamNames.Default).ToString();
            Assert.AreEqual(expectedOutput, actualOutput);
        }
    }
}