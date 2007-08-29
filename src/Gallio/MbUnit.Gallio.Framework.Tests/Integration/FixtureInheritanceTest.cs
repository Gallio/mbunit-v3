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
