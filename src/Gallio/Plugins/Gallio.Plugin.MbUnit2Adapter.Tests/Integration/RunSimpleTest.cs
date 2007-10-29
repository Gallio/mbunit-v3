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
using Gallio.Runner.Reports;
using Gallio.Logging;
using Gallio.Model;
using Gallio.Tests.Integration;
using Gallio.TestResources.MbUnit2;
using MbUnit.Framework;

namespace Gallio.Plugin.MbUnit2Adapter.Tests.Integration
{
    [TestFixture]
    public class RunSimpleTest : BaseSampleTest
    {
        [TestFixtureSetUp]
        public void RunSample()
        {
            RunFixtures(typeof(SimpleTest));
        }

        [Test]
        public void PassTestPassed()
        {
            TestRun testRun = GetTestRun(CodeReference.CreateFromMember(typeof(SimpleTest).GetMethod("Pass")));
            Assert.AreEqual(TestOutcome.Passed, testRun.RootStepRun.Result.Outcome);
        }

        [Test]
        public void FailTestFailed()
        {
            TestRun testRun = GetTestRun(CodeReference.CreateFromMember(typeof(SimpleTest).GetMethod("Fail")));
            Assert.AreEqual(TestOutcome.Failed, testRun.RootStepRun.Result.Outcome);
            StringAssert.Contains(testRun.RootStepRun.ExecutionLog.GetStream(LogStreamNames.Failures).ToString(), "Boom");
        }
    }
}
