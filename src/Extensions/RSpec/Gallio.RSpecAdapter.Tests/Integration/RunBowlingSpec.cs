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
using Gallio.Model.Schema;
using Gallio.Model.Tree;
using Gallio.Runner.Reports.Schema;
using Gallio.Tests;
using MbUnit.Framework;
using Gallio.Model;

namespace Gallio.RSpecAdapter.Tests.Integration
{
    [TestFixture]
    [RunSampleFile(@"..\Scripts\bowling_spec.rb")]
    public class RunBowlingSpec : BaseTestWithSampleRunner
    {
        [Test]
        public void Adapter_GeneratesCorrectTestModel()
        {
            TestModelData testModel = Runner.Report.TestModel;

            Assert.AreEqual(1, testModel.RootTest.Children.Count, "Root test contain top level test.");

            TestData fileTest = testModel.RootTest.Children[0];
            Assert.AreEqual("bowling_spec", fileTest.Name, "Top level test is named for the file.");
            Assert.EndsWith(fileTest.Metadata.GetValue(MetadataKeys.File), "bowling_spec.rb", "Top level test has correct file path metadata.");
            Assert.AreEqual("RSpec File", fileTest.Metadata.GetValue(MetadataKeys.TestKind), "Top level test should have correct kind.");
            Assert.AreEqual(1, fileTest.Children.Count, "Top level test contains example group.");

            TestData exampleGroupTest = fileTest.Children[0];
            Assert.AreEqual("Bowling", exampleGroupTest.Name, "Example group is named as in 'describe' syntax.");
            Assert.AreEqual("RSpec Example Group", exampleGroupTest.Metadata.GetValue(MetadataKeys.TestKind), "Example group test should have correct kind.");
            Assert.AreEqual(3, exampleGroupTest.Children.Count, "Example group test contains examples.");

            TestData passingExampleTest = exampleGroupTest.Children[0];
            Assert.AreEqual("should score 0 for gutter game", passingExampleTest.Name, "Example is named as in 'it' syntax.");
            Assert.AreEqual("RSpec Example", passingExampleTest.Metadata.GetValue(MetadataKeys.TestKind), "Example test should have correct kind.");

            TestData failingExampleTest = exampleGroupTest.Children[1];
            Assert.AreEqual("should score 300 for perfect game", failingExampleTest.Name, "Example is named as in 'it' syntax.");
            Assert.AreEqual("RSpec Example", failingExampleTest.Metadata.GetValue(MetadataKeys.TestKind), "Example test should have correct kind.");

            TestData pendingExampleTest = exampleGroupTest.Children[2];
            Assert.AreEqual("should score 20 for single pin hit each ball", pendingExampleTest.Name, "Example is named as in 'it' syntax.");
            Assert.AreEqual("RSpec Example", pendingExampleTest.Metadata.GetValue(MetadataKeys.TestKind), "Example test should have correct kind.");
        }

        [Test]
        public void PassingExample_ReportsSuccess()
        {
            TestStepRun run = Runner.GetPrimaryTestStepRun(x => x.Step.Name == "should score 0 for gutter game");
            Assert.AreEqual(TestOutcome.Passed, run.Result.Outcome);
        }

        [Test]
        public void FailingExample_ReportsFailure()
        {
            TestStepRun run = Runner.GetPrimaryTestStepRun(x => x.Step.Name == "should score 300 for perfect game");
            Assert.AreEqual(TestOutcome.Failed, run.Result.Outcome);
            Assert.Contains(run.TestLog.GetStream(MarkupStreamNames.Failures).ToString(), "'Bowling should score 300 for perfect game' FAILED");
            Assert.Contains(run.TestLog.GetStream(MarkupStreamNames.Failures).ToString(), "expected: 300");
            Assert.Contains(run.TestLog.GetStream(MarkupStreamNames.Failures).ToString(), "bowling_spec.rb");
        }

        [Test]
        public void PendingExample_ReportsReason()
        {
            TestStepRun run = Runner.GetPrimaryTestStepRun(x => x.Step.Name == "should score 20 for single pin hit each ball");
            Assert.AreEqual(TestOutcome.Pending, run.Result.Outcome);
            Assert.AreEqual("Scoring to be implemented.", run.Step.Metadata.GetValue(MetadataKeys.PendingReason));
            Assert.Contains(run.TestLog.GetStream(MarkupStreamNames.Warnings).ToString(), "Scoring to be implemented.");
            Assert.Contains(run.TestLog.GetStream(MarkupStreamNames.Warnings).ToString(), "Pending");
        }
    }
}
