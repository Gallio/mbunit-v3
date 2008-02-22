// Copyright 2008 MbUnit Project - http://www.mbunit.com/
// Portions Copyright 2000-2004 Jonathan De Halleux, Jamie Cansdale
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
using Gallio.Reflection;
using Gallio.Runner.Reports;
using Gallio.Logging;
using Gallio.Model;
using Gallio.Tests.Integration;
using MbUnit.TestResources.Fixtures;
using MbUnit.Framework;

namespace MbUnit.Tests.Integration
{
    [TestFixture]
    public class ContextOutcomeTest : BaseSampleTest
    {
        [Test]
        public void PassingTestOutcomeIsAlwaysPass()
        {
            RunFixtures(typeof(ContextOutcomePassingTestSample));

            AssertTestResult("Passed\nPassed\nPassed\n", TestStatus.Passed, typeof(ContextOutcomePassingTestSample), "Test");
            AssertFixtureResult("Passed\n", TestStatus.Passed, typeof(ContextOutcomePassingTestSample));
        }

        [Test]
        public void FailedSetUpTestOutcomeBecomesFailedAfterSetUp()
        {
            RunFixtures(typeof(ContextOutcomeFailingSetUpSample));

            AssertTestResult("Passed\nFailed\n", TestStatus.Failed, typeof(ContextOutcomeFailingSetUpSample), "Test");
            AssertFixtureResult("Failed\n", TestStatus.Failed, typeof(ContextOutcomeFailingSetUpSample));
        }

        [Test]
        public void FailedTestOutcomeBecomesFailedAfterTest()
        {
            RunFixtures(typeof(ContextOutcomeFailingTestSample));

            AssertTestResult("Passed\nPassed\nFailed\n", TestStatus.Failed, typeof(ContextOutcomeFailingTestSample), "Test");
            AssertFixtureResult("Failed\n", TestStatus.Failed, typeof(ContextOutcomeFailingTestSample));
        }

        [Test]
        public void FailedTearDownOutcomeBecomesFailedAfterTearDown()
        {
            RunFixtures(typeof(ContextOutcomeFailingTearDownSample));

            AssertTestResult("Passed\nPassed\nPassed\n", TestStatus.Failed, typeof(ContextOutcomeFailingTearDownSample), "Test");
            AssertFixtureResult("Failed\n", TestStatus.Failed, typeof(ContextOutcomeFailingTearDownSample));
        }

        private void AssertTestResult(string expectedOutput, TestStatus expectedStatus, Type fixtureType, string memberName)
        {
            CodeReference codeReference = CodeReference.CreateFromType(fixtureType);
            codeReference.MemberName = memberName;

            TestInstanceRun run = GetFirstTestInstanceRun(codeReference);
            Assert.AreEqual(expectedStatus, run.RootTestStepRun.Result.Outcome.Status);

            string actualOutput = run.RootTestStepRun.ExecutionLog.GetStream(LogStreamNames.Default).ToString();
            Assert.AreEqual(expectedOutput, actualOutput);
        }

        private void AssertFixtureResult(string expectedOutput, TestStatus expectedStatus, Type fixtureType)
        {
            CodeReference codeReference = CodeReference.CreateFromType(fixtureType);

            TestInstanceRun run = GetFirstTestInstanceRun(codeReference);
            Assert.AreEqual(expectedStatus, run.RootTestStepRun.Result.Outcome.Status);

            string actualOutput = run.RootTestStepRun.ExecutionLog.GetStream(LogStreamNames.Default).ToString();
            Assert.AreEqual(expectedOutput, actualOutput);
        }
    }
}