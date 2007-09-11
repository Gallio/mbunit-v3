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
using System.Diagnostics;
using System.IO;
using System.Reflection;
using MbUnit.Core.Runner;

namespace MbUnit.Tasks.NAnt.Tests
{
    /// <summary>
    /// These are basically integration tests that call the NAnt executable and
    /// verify the MbUnit's Exit Code and the process' Exit Code.
    /// </summary>
    /// <todo>
    /// Add clean up code to clear the logs created by MbUnit when running these tests.
    /// </todo>
    [TestFixture]
    [Author("Julian Hidalgo")]
    [TestsOn(typeof(MbUnitNAntTask))]
    [TestCategory("IntegrationTests")]
    public class MbUnitNAntTaskTests
    {
        private string nantExecutablePath = null;
        private string workingDirectory = null;
        string testAssemblyPath = null;

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            workingDirectory = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
            nantExecutablePath = Path.Combine(workingDirectory, @"..\..\..\..\libs\NAnt\NAnt.exe");

            if (!File.Exists(nantExecutablePath))
                Assert.Fail("Cannot find the NAnt executable in {0}!", nantExecutablePath);

            testAssemblyPath = new Uri(typeof(SimpleTest).Assembly.CodeBase).LocalPath;
        }

        [Test]
        public void RunNAnt_NoTestAssembly()
        {
            Process p = RunNAnt(@"..\TestBuildFiles\NoTests.build", ResultCode.NoTests);
            p.WaitForExit();
            // There are not test assemblies specified in the NoTests.build file,
            // so the task should have set the ExitCode property to ResultCode.NoTests.
            // We are ignoring failures in the build file so NAnt should always
            // return true unless the condition ExitCode=ResultCode.NoTests is false.
            Assert.AreEqual(p.ExitCode, 0, " This build should have succeeded.");
            p.Close();
        }

        [Test]
        public void RunNAnt_PassingTests()
        {
            Process p = RunNAnt(@"..\TestBuildFiles\PassingTests.build", ResultCode.Success);
            p.WaitForExit();
            // MbUnit.TestResources.PassingTests only contains tests that pass,
            // so the task should have set the ExitCode property to ResultCode.Success.
            // We are ignoring failures in the build file so NAnt should always
            // return true unless the condition ExitCode=ResultCode.Success is false.
            Assert.AreEqual(p.ExitCode, 0, "This build should have succeeded.");
            p.Close();
        }

        [Test]
        public void RunNAnt_FailingTests_FailuresIgnored()
        {
            Process p = RunNAnt(@"..\TestBuildFiles\FailingTests-FailuresIgnored.build", ResultCode.Failure);
            p.WaitForExit();
            // MbUnit.TestResources.FailingTests only contains tests that fail,
            // so the task should have set the ExitCode property to ResultCode.Failure.
            // We are ignoring failures in the build file so NAnt should always
            // return true unless the condition ExitCode=ResultCode.Failure is false
            Assert.AreEqual(p.ExitCode, 0, "This build should have succeeded.");
            p.Close();
        }

        [Test]
        public void RunNAnt_FailingTests()
        {
            Process p = RunNAnt(@"..\TestBuildFiles\FailingTests.build", ResultCode.Failure);
            p.WaitForExit();
            // MbUnit.TestResources.FailingTests only contains tests that fail,
            // so the task should have set the ExitCode property to ResultCode.Failure.
            // We are NOT ignoring failures in the build file so NAnt should
            // return 1.
            Assert.AreEqual(p.ExitCode, 1, "This build should have failed.");
            p.Close();
        }

        /// <summary>
        /// This test verifies that the task doesn't crash when no filter has been
        /// specified.
        /// </summary>
        [Test]
        public void RunNAnt_NoFilter()
        {
            Process p = RunNAnt(@"..\TestBuildFiles\NoFilter.build", ResultCode.Failure);
            p.WaitForExit();
            // We are not ignoring failures in the build file so NAnt should
            // return true.
            Assert.AreEqual(p.ExitCode, 0, "This build should have succeeded.");
            p.Close();
        }
                
        private Process RunNAnt(string buildFile, int expectedMbUnitExitCode)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(nantExecutablePath);
            // This is to avoid having a lot of windows popping up when running the tets
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
            startInfo.WorkingDirectory = workingDirectory;
            startInfo.Arguments = "/f:" + buildFile +
                " /D:ExpectedMbUnitExitCode=" + expectedMbUnitExitCode;
            if (!String.IsNullOrEmpty(testAssemblyPath))
            {
                startInfo.Arguments += " /D:TestAssembly=\"" + testAssemblyPath + "\"";
            }

            return Process.Start(startInfo);
        }
    }
}
