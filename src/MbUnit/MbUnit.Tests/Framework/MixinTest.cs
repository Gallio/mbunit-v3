using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gallio.Reflection;
using Gallio.Runner.Reports;
using Gallio.Tests;
using MbUnit.Framework;

namespace MbUnit.Tests.Framework
{
    [TestFixture]
    [TestsOn(typeof(MixinAttribute))]
    public class MixinTest : BaseTestWithSampleRunner
    {
        [Test]
        public void VerifySampleOutput(Type fixtureType, string sampleName, string[] output)
        {
            IList<TestStepRun> runs = Runner.GetTestCaseRunsWithin(
                CodeReference.CreateFromMember(fixtureType.GetMethod(sampleName)));

            Assert.AreEqual(output.Length, runs.Count, "Different number of runs than expected.");

            for (int i = 0; i < output.Length; i++)
                AssertLogContains(runs[i], output[i]);
        }
    }
}
