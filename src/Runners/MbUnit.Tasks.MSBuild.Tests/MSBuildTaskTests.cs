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
using MbUnit.TestResources.MbUnit2;
using MbUnit2::MbUnit.Framework;
using System;
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
    /// <comments>
    /// The tests are very similar, so it's tempting to refactor them as a single
    /// RowTest, but that would hurt the readability, so it's adviced not to do it. 
    /// </comments>
    /// <todo>
    /// Add clean up code to clear the logs created by MbUnit when running these tests.
    /// </todo>
    [TestFixture]
    [Author("Julian Hidalgo")]
    [TestsOn(typeof(MbUnit))]
    [TestCategory("IntegrationTests")]
    public class MbUnitTaskTests
    {
        private string MSBuildExecutablePath = null;
        string workingDirectory = null;
        string testAssemblyPath = null;

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            string frameworkPath = RuntimeEnvironment.GetRuntimeDirectory();
            MSBuildExecutablePath = frameworkPath + @"MSBuild.exe";
            if (!File.Exists(MSBuildExecutablePath))
            {
                Assert.Fail("Cannot find the MSBuild executable!");
            }
            testAssemblyPath = new Uri(typeof(SimpleTest).Assembly.CodeBase).LocalPath;
            workingDirectory = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
        }

        [Test]
        public void RunMSBuild_NoTestAssembly()
        {
            Process p = RunMSBuild(@"..\TestBuildFiles\NoTests.xml", ResultCode.NoTests);
            p.WaitForExit();
            // There are not test assemblies specified in the NoTests.xml build file,
            // so the task should have set the ExitCode property to ResultCode.NoTests.
            // We are ignoring failures in the build file so MSBuild should always
            // return true unless the condition ExitCode=ResultCode.NoTests is false.
            Assert.AreEqual(p.ExitCode, 0, " There are not test assemblies in this build file so it should have succeeded.");
            p.Close();
        }

        [Test]
        public void RunMSBuild_PassingTests()
        {
            Process p = RunMSBuild(@"..\TestBuildFiles\PassingTests.xml", ResultCode.Success);
            p.WaitForExit();
            // MbUnit.TestResources.PassingTests only contains tests that pass,
            // so the task should have set the ExitCode property to ResultCode.Success.
            // We are ignoring failures in the build file so MSBuild should always
            // return true unless the condition ExitCode=ResultCode.Success is false.
            Assert.AreEqual(p.ExitCode, 0, "This build should have succeeded.");
            p.Close();
        }

        [Test]
        public void RunMSBuild_FailingTests_FailuresIgnored()
        {
            Process p = RunMSBuild(@"..\TestBuildFiles\FailingTests-FailuresIgnored.xml", ResultCode.Failure);
            p.WaitForExit();
            // MbUnit.TestResources.FailingTests only contains tests that fail,
            // so the task should have set the ExitCode property to ResultCode.Failure.
            // We are ignoring failures in the build file so MSBuild should always
            // return true unless the condition ExitCode=ResultCode.Failure is false
            Assert.AreEqual(p.ExitCode, 0, "This build should have succeeded.");
            p.Close();
        }

        [Test]
        public void RunMSBuild_FailingTests()
        {
            Process p = RunMSBuild(@"..\TestBuildFiles\FailingTests.xml", ResultCode.Failure);
            p.WaitForExit();
            // MbUnit.TestResources.FailingTests only contains tests that fail,
            // so the task should have set the ExitCode property to ResultCode.Failure.
            // We are NOT ignoring failures in the build file so MSBuild should
            // return 1.
            Assert.AreEqual(p.ExitCode, 1, "This build should have failed.");
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
            // We are not ignoring failures in the build file so MSBuild should
            // return true.
            Assert.AreEqual(p.ExitCode, 0, "This build should have succeeded.");
            p.Close();
        }

        private Process RunMSBuild(string buildFile, int expectedMbUnitExitCode)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(MSBuildExecutablePath);
            // This is to avoid having a lot of windows popping up when running the tets
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
            startInfo.WorkingDirectory = workingDirectory;
            startInfo.Arguments = "\"" + buildFile + "\"" +
                " /p:\"ExpectedMbUnitExitCode=" + expectedMbUnitExitCode + "\"";
            if (!String.IsNullOrEmpty(testAssemblyPath))
            {
                startInfo.Arguments += " /p:\"TestAssembly=" + testAssemblyPath + "\"";
            }
            
            return Process.Start(startInfo);
        }
    }
}
