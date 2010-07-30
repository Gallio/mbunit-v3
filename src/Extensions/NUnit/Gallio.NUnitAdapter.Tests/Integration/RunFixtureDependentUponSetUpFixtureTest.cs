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
using Gallio.Common.Markup;
using Gallio.Common.Reflection;
using Gallio.NUnitAdapter.TestResources.SetUpFixtureSample;
using Gallio.Runner.Reports.Schema;
using Gallio.Tests;
using MbUnit.Framework;
using Gallio.Model;
using Gallio.NUnitAdapter.TestResources;

namespace Gallio.NUnitAdapter.Tests.Integration
{
#if !NUNIT248
    [TestFixture]
    [RunSample(typeof(FixtureDependentUponSetUpFixture))]
    public class FixtureDependentUponSetUpFixtureTest : BaseTestWithSampleRunner
    {
        [Test]
        public void VerifyThatSetUpFixtureRan()
        {
            TestStepRun methodRun = Runner.GetPrimaryTestStepRun(CodeReference.CreateFromMember(typeof(FixtureDependentUponSetUpFixture).GetMethod("VerifyThatSetUpFixtureRan")));
            Assert.IsNotNull(methodRun);
            Assert.AreEqual(TestStatus.Passed, methodRun.Result.Outcome.Status);

            TestStepRun namespaceRun = Runner.GetPrimaryTestStepRun(x => x.Step.Name == "SetUpFixtureSample");
            AssertLogContains(namespaceRun, "[SetUpFixture] SetUp", MarkupStreamNames.ConsoleOutput);
            AssertLogContains(namespaceRun, "[SetUpFixture] TearDown", MarkupStreamNames.ConsoleOutput);
        }
    }
#endif
}
