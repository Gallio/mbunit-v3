using System;
using System.IO;
using Gallio.Model;
using Gallio.Reflection;
using Gallio.Runner.Reports;
using Gallio.Tests.Integration;
using MbUnit.Framework;

namespace MbUnit.Tests.Integration
{
    /// <summary>
    /// Tests the test assembly working directory and application base directory defaults.
    /// </summary>
    [TestFixture]
    public class WorkingDirectoryAndApplicationBaseTest : BaseSampleTest
    {
        [FixtureSetUp]
        public void RunSample()
        {
            RunFixtures(typeof(WorkingDirectoryAndApplicationBaseSample));
        }

        [Test]
        public void WorkingDirectoryIsSameAsDirectoryContainingTestAssembly()
        {
            TestStepRun run = Runner.GetPrimaryTestStepRun(
                CodeReference.CreateFromMember(typeof(WorkingDirectoryAndApplicationBaseSample).GetMethod("WorkingDirectoryIsSameAsDirectoryContainingTestAssembly")));
            Assert.AreEqual(TestOutcome.Passed, run.Result.Outcome);
        }

        [Test]
        public void ApplicationBaseIsSameAsDirectoryContainingTestAssembly()
        {
            TestStepRun run = Runner.GetPrimaryTestStepRun(
                CodeReference.CreateFromMember(typeof(WorkingDirectoryAndApplicationBaseSample).GetMethod("ApplicationBaseIsSameAsDirectoryContainingTestAssembly")));
            Assert.AreEqual(TestOutcome.Passed, run.Result.Outcome);
        }
    }

    [TestFixture, Explicit("Sample")]
    internal class WorkingDirectoryAndApplicationBaseSample
    {
        [Test]
        public void WorkingDirectoryIsSameAsDirectoryContainingTestAssembly()
        {
            Assert.AreEqual(Path.GetDirectoryName(AssemblyUtils.GetAssemblyLocalPath(GetType().Assembly)),
                Environment.CurrentDirectory);
        }

        [Test]
        public void ApplicationBaseIsSameAsDirectoryContainingTestAssembly()
        {
            Assert.AreEqual(Path.GetDirectoryName(AssemblyUtils.GetAssemblyLocalPath(GetType().Assembly)),
                AppDomain.CurrentDomain.BaseDirectory);
        }
    }
}