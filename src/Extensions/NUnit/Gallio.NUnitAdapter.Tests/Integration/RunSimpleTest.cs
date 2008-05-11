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
using Gallio.Model.Execution;
using Gallio.Reflection;
using Gallio.Runner.Reports;
using MbUnit.Framework;
using Gallio.Model;
using Gallio.Tests.Integration;
using Gallio.NUnitAdapter.TestResources;

namespace Gallio.NUnitAdapter.Tests.Integration
{
    [TestFixture]
    public class RunSimpleTest : BaseSampleTest
    {
        [FixtureSetUp]
        public void RunSample()
        {
            RunFixtures(typeof(SimpleTest));
        }

        [Test]
        public void PassTestPassed()
        {
            TestStepRun run = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromMember(typeof(SimpleTest).GetMethod("Pass")));
            Assert.AreEqual(TestStatus.Passed, run.Result.Outcome.Status);
        }

        [Test]
        public void FailTestFailed()
        {
            TestStepRun run = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromMember(typeof(SimpleTest).GetMethod("Fail")));
            Assert.AreEqual(TestStatus.Failed, run.Result.Outcome.Status);
            StringAssert.Contains(run.ExecutionLog.GetStream(LogStreamNames.Failures).ToString(), "Boom");
        }
    }
}
