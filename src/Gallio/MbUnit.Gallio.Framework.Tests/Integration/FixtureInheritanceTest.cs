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

extern alias MbUnit2;
using MbUnit.Core.Reporting;
using MbUnit.Core.Runtime;
using MbUnit.Framework.Kernel.ExecutionLogs;
using MbUnit.Framework.Kernel.Model;
using MbUnit.Framework.Kernel.Results;
using MbUnit.Framework.Kernel.Runtime;
using MbUnit.Framework.Tests;
using MbUnit.TestResources.Gallio.Fixtures;
using MbUnit2::MbUnit.Framework;

using System;
using System.Collections.Generic;
using System.Text;

namespace MbUnit._Framework.Tests.Integration
{
    [TestFixture, Explicit]
    public class FixtureInheritanceTest : BaseSampleTest
    {
        [TestFixtureSetUp]
        public void RunSample()
        {
            RunFixtures(typeof(FixtureInheritanceSample.DerivedFixture));
        }

        [Test]
        public void BaseTestIncludesBaseFixtureContributionsFirst()
        {
            CodeReference codeReference = CodeReference.CreateFromType(typeof(FixtureInheritanceSample.DerivedFixture));
            codeReference.MemberName = "BaseTest";

            Assert.AreEqual(TestOutcome.Passed, GetTestRun(codeReference).RootStepRun.Result.Outcome);
            Assert.AreEqual("", GetStreamText(codeReference, ExecutionLogStreamName.ConsoleOutput));
        }

        [Test]
        public void DerivedTestIncludesBaseFixtureContributionsFirst()
        {
            CodeReference codeReference = CodeReference.CreateFromType(typeof(FixtureInheritanceSample.DerivedFixture));
            codeReference.MemberName = "DerivedTest";

            Assert.AreEqual(TestOutcome.Passed, GetTestRun(codeReference).RootStepRun.Result.Outcome);
            Assert.AreEqual("", GetStreamText(codeReference, ExecutionLogStreamName.ConsoleOutput));
        }
    }
}
