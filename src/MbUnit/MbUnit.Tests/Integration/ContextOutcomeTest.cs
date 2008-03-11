// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using Gallio.Model.Execution;
using Gallio.Reflection;
using Gallio.Runner.Reports;
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

            AssertTestResult("passed\npassed\npassed\n", TestStatus.Passed, typeof(ContextOutcomePassingTestSample), "Test");
            AssertFixtureResult("passed\n", TestStatus.Passed, typeof(ContextOutcomePassingTestSample));
        }

        [Test]
        public void FailedSetUpTestOutcomeBecomesFailedAfterSetUp()
        {
            RunFixtures(typeof(ContextOutcomeFailingSetUpSample));

            AssertTestResult("passed\nfailed\n", TestStatus.Failed, typeof(ContextOutcomeFailingSetUpSample), "Test");
            AssertFixtureResult("failed\n", TestStatus.Failed, typeof(ContextOutcomeFailingSetUpSample));
        }

        [Test]
        public void FailedTestOutcomeBecomesFailedAfterTest()
        {
            RunFixtures(typeof(ContextOutcomeFailingTestSample));

            AssertTestResult("passed\npassed\nfailed\n", TestStatus.Failed, typeof(ContextOutcomeFailingTestSample), "Test");
            AssertFixtureResult("failed\n", TestStatus.Failed, typeof(ContextOutcomeFailingTestSample));
        }

        [Test]
        public void FailedTearDownOutcomeBecomesFailedAfterTearDown()
        {
            RunFixtures(typeof(ContextOutcomeFailingTearDownSample));

            AssertTestResult("passed\npassed\npassed\n", TestStatus.Failed, typeof(ContextOutcomeFailingTearDownSample), "Test");
            AssertFixtureResult("failed\n", TestStatus.Failed, typeof(ContextOutcomeFailingTearDownSample));
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