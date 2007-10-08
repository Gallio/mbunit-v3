// Copyright 2007 MbUnit Project - http://www.mbunit.com/
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
using MbUnit.Core.Reporting;
using MbUnit.Logging;
using MbUnit.Model;
using MbUnit.TestResources.Gallio.Fixtures;
using MbUnit.Framework;

namespace MbUnit.Tests.Integration
{
    [TestFixture]
    public class FixtureInheritanceTest : BaseSampleTest
    {
        [TestFixtureSetUp]
        public void RunSample()
        {
            RunFixtures(typeof(FixtureInheritanceSample), typeof(FixtureInheritanceSample.DerivedFixture));
        }

        [Test]
        public void BaseTestOnBaseFixture()
        {
            AssertOutput("BaseTestFixtureSetUp\nBaseTestFixtureTearDown\n",
                typeof(FixtureInheritanceSample), null);
            AssertOutput("BaseSetUp\nBaseTest\nBaseTearDown\n",
                typeof(FixtureInheritanceSample), "BaseTest");
        }

        [Test]
        public void BaseTestOnDerivedFixtureIncludesBaseFixtureContributionsFirst()
        {
            AssertOutput("BaseTestFixtureSetUp\nDerivedTestFixtureSetUp\nDerivedTestFixtureTearDown\nBaseTestFixtureTearDown\n",
                typeof(FixtureInheritanceSample.DerivedFixture), null);
            AssertOutput("BaseSetUp\nDerivedSetUp\nBaseTest\nDerivedTearDown\nBaseTearDown\n",
                typeof(FixtureInheritanceSample.DerivedFixture), "BaseTest");
        }

        [Test]
        public void DerivedTestOnDerivedFixtureIncludesBaseFixtureContributionsFirst()
        {
            AssertOutput("BaseTestFixtureSetUp\nDerivedTestFixtureSetUp\nDerivedTestFixtureTearDown\nBaseTestFixtureTearDown\n",
                typeof(FixtureInheritanceSample.DerivedFixture), null);
            AssertOutput("BaseSetUp\nDerivedSetUp\nDerivedTest\nDerivedTearDown\nBaseTearDown\n",
                typeof(FixtureInheritanceSample.DerivedFixture), "DerivedTest");
        }

        private void AssertOutput(string expectedOutput, Type fixtureType, string memberName)
        {
            CodeReference codeReference = CodeReference.CreateFromType(fixtureType);
            codeReference.MemberName = memberName;

            TestRun testRun = GetTestRun(codeReference);
            Assert.AreEqual(TestOutcome.Passed, testRun.RootStepRun.Result.Outcome);
            Assert.AreEqual(TestStatus.Executed, testRun.RootStepRun.Result.Status);

            string actualOutput = testRun.RootStepRun.ExecutionLog.GetStream(LogStreamNames.ConsoleOutput).ToString();
            Assert.AreEqual(expectedOutput, actualOutput);
        }
    }
}
