extern alias MbUnit2;
using MbUnit2::MbUnit.Framework;
using System.Reflection;
using MbUnit.Core.Runner;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace MbUnit.Tasks.MSBuild.Tests
{
    /// <summary>
    /// These are basically integration tests that call the MSBuild executable and
    /// verify the MbUnit's Exit Code and the process' Exit Code.
    /// </summary>
    /// <todo>
    /// Add clean up code to clear the logs created by MbUnit when running these tests.
    /// </todo>
    [TestFixture]
    [Author("Julian Hidalgo")]
    [TestsOn(typeof(MbUnit))]
    public class MbUnitTaskTests
    {
        private string MSBuildExecutablePath = null;
        string workingDirectory = null;

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            string frameworkPath = RuntimeEnvironment.GetRuntimeDirectory();
            MSBuildExecutablePath = frameworkPath + @"MSBuild.exe";
            workingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        [Test]
        public void RunMSBuild_NoTests()
        {
            Process p = RunMSBuild(@"..\TestBuildFiles\NoTests.xml", ResultCode.NoTests);
            p.WaitForExit();
            // There are not test assemblies specified in the NoTests.xml build file,
            // so the task should have set ExitCode=ResultCode.NoTests)
            // We are ignoring failures in the build file so the task should always
            // return true unless the condition ExitCode=ResultCode.NoTests is false
            Assert.AreEqual(p.ExitCode, 0, " There are not test assemblies in this build file so we should have got ResultCode.NoTests");
            p.Close();
        }

        [Test]
        public void RunMSBuild_PassingTests()
        {
            Process p = RunMSBuild(@"..\TestBuildFiles\PassingTests.xml", ResultCode.Success);
            p.WaitForExit();
            // MbUnit.TestResources.PassingTests only contains tests that pass,
            // so the task should have set ExitCode=ResultCode.Success)
            // We are ignoring failures in the build file so the task should always
            // return true unless the condition ExitCode=ResultCode.Success is false
            Assert.AreEqual(p.ExitCode, 0, "This assembly passes so we should have got ResultCode.Success");
            p.Close();
        }

        [Test]
        public void RunMSBuild_FailingTests_FailuresIgnored()
        {
            Process p = RunMSBuild(@"..\TestBuildFiles\FailingTests-FailuresIgnored.xml", ResultCode.Failure);
            p.WaitForExit();
            // MbUnit.TestResources.FailingTests only contains tests that fail,
            // so the task should have set ExitCode=ResultCode.Failure)
            // We are ignoring failures in the build file so the task should always
            // return true unless the condition ExitCode=ResultCode.Failure is false
            Assert.AreEqual(p.ExitCode, 0, "This assembly fails so we should have got ResultCode.Failure");
            p.Close();
        }

        [Test]
        public void RunMSBuild_FailingTests()
        {
            Process p = RunMSBuild(@"..\TestBuildFiles\FailingTests.xml", ResultCode.Failure);
            p.WaitForExit();
            // MbUnit.TestResources.FailingTests only contains tests that fail,
            // so the task should have set ExitCode=ResultCode.Failure)
            // We are not ignoring failures in the build file so the task should
            // return false.
            Assert.AreEqual(p.ExitCode, 1, "This assembly fails so we should have got ExitCode == 1");
            p.Close();
        }

        /// <summary>
        /// This test verifies that the task doesn't crash when no filter has been
        /// specified.
        /// </summary>
        [Test]
        public void RunMSBuild_NoFilter()
        {
            Process p = RunMSBuild(@"..\TestBuildFiles\NoFilter.xml", ResultCode.Failure);
            p.WaitForExit();
            // We are not ignoring failures in the build file so the task should
            // return true.
            Assert.AreEqual(p.ExitCode, 0);
            p.Close();
        }

        private Process RunMSBuild(string buildFile, int expectedMbUnitExitCode)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(MSBuildExecutablePath);
            startInfo.WorkingDirectory = workingDirectory;
            startInfo.Arguments = buildFile +
                " /p:\"ExpectedMbUnitExitCode=" + expectedMbUnitExitCode + "\"";           
            return Process.Start(startInfo);
        }
    }
}
