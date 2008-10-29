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
using Gallio.Framework;
using Gallio.Model;
using Gallio.Reflection;
using Gallio.Runner.Reports;
using Gallio.Tests;
using MbUnit.Framework;

namespace MbUnit.Tests.Framework
{
    [TestFixture]
    [TestsOn(typeof(ExplicitAttribute))]
    public class ExplicitTest : BaseTestWithSampleRunner
    {
        [FixtureSetUp]
        public void RunSample()
        {
            Runner.AddMethod(typeof(ExplicitSample), "SelectedExplicitTest");
            Runner.Run();
        }

        [Test]
        public void SelectedExplicitTestWillRun()
        {
            TestStepRun run = Runner.GetPrimaryTestStepRun(
                CodeReference.CreateFromMember(typeof(ExplicitSample).GetMethod("SelectedExplicitTest")));

            Assert.AreEqual(TestOutcome.Passed, run.Result.Outcome);
            AssertLogContains(run, "Got here");
        }

        [Test]
        public void SelectedExplicitTestWillNotRunAndWillNotAppearInTheReport()
        {
            TestStepRun run = Runner.GetPrimaryTestStepRun(
                CodeReference.CreateFromMember(typeof(ExplicitSample).GetMethod("UnselectedExplicitTest")));
            Assert.IsNull(run);
        }

        [TestFixture, Explicit("Sample")]
        internal class ExplicitSample
        {
            [Test, Explicit("Explicit")]
            public void SelectedExplicitTest()
            {
                TestLog.WriteLine("Got here");
            }

            [Test, Explicit("Explicit")]
            public void UnselectedExplicitTest()
            {
                TestLog.WriteLine("Got here");
            }
        }
    }
}
