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
using System.Linq;
using Gallio.Common.Reflection;
using Gallio.Runner.Reports.Schema;
using Gallio.Tests;
using MbUnit.Framework;
using Gallio.Model;
using Gallio.XunitAdapter.TestResources;

namespace Gallio.XunitAdapter.Tests.Integration
{
    [TestFixture]
    [RunSample(typeof(TheorySample))]
    public class RunTheoryTest : BaseTestWithSampleRunner
    {
        [Test]
        public void Pythagoras()
        {
            TestStepRun theoryRun = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromMember(typeof(TheorySample).GetMethod("Pythagoras")));
            Assert.AreEqual("Pythagoras", theoryRun.Step.Name);
            Assert.AreEqual(TestOutcome.Failed, theoryRun.Result.Outcome);
            Assert.IsTrue(theoryRun.Step.IsPrimary);
            Assert.IsFalse(theoryRun.Step.IsDynamic);
            Assert.AreEqual(3, theoryRun.Children.Count);

            TestStepRun run1 = (from x in theoryRun.Children where x.Step.Name.Contains("3, 4, 5") select x).Single();
            Assert.AreEqual("Pythagoras(3, 4, 5)", run1.Step.Name);
            Assert.IsFalse(run1.Step.IsPrimary);
            Assert.IsTrue(run1.Step.IsDynamic);
            Assert.AreEqual(TestOutcome.Passed, run1.Result.Outcome);

            TestStepRun run2 = (from x in theoryRun.Children where x.Step.Name.Contains("6, 8, 10") select x).Single();
            Assert.AreEqual("Pythagoras(6, 8, 10)", run2.Step.Name);
            Assert.IsFalse(run2.Step.IsPrimary);
            Assert.IsTrue(run2.Step.IsDynamic);
            Assert.AreEqual(TestOutcome.Passed, run2.Result.Outcome);

            TestStepRun run3 = (from x in theoryRun.Children where x.Step.Name.Contains("1, 1, 1") select x).Single();
            Assert.AreEqual("Pythagoras(1, 1, 1)", run3.Step.Name);
            Assert.IsFalse(run3.Step.IsPrimary);
            Assert.IsTrue(run3.Step.IsDynamic);
            Assert.AreEqual(TestOutcome.Failed, run3.Result.Outcome);
        }
    }
}
