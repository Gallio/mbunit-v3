// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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
using System.Diagnostics;
using System.IO;
using Gallio.Concurrency;
using Gallio.Framework;
using Gallio.Reflection;
using MbUnit.Framework;
using MbUnit.TestResources;
using Microsoft.Win32;

namespace Gallio.VisualStudio.Tip.Tests
{
    /// <summary>
    /// Simple integration tests that ensure that MSTest.exe can run tests using Gallio.
    /// </summary>
    [TestFixture]
    [TestsOn(typeof(GallioTip))]
    [Category("IntegrationTests")]
    [Explicit("This test modifies the registry and may corrupt the development environment.")]
    public class GallioTipIntegrationTest
    {
        [FixtureSetUp]
        public void InstallTip()
        {
            RunInstallScript("Y");
        }

        [FixtureTearDown]
        public void UninstallSnapIn()
        {
            RunInstallScript("N");
        }

        [Test]
        public void PrintsCorrectOutputForPassingTestsAndReturnsAResultCodeOfZero()
        {
            ProcessTask task = RunMSTest("\"/test:MbUnit v0.0.0.0/MbUnit.TestResources/PassingTests/Pass\"");
            OldStringAssert.Like(task.ConsoleOutput, "Passed *MbUnit v0.0.0.0/MbUnit.TestResources/PassingTests/Pass");
            OldStringAssert.Like(task.ConsoleOutput, "Passed *MbUnit v0.0.0.0/MbUnit.TestResources/PassingTests/PassAgain");
            Assert.Contains(task.ConsoleOutput, "2/2 test(s) Passed");
            Assert.AreEqual(0, task.ExitCode);
        }

        [Test]
        public void PrintsCorrectOutputForFailingTestsAndReturnsAResultCodeOfZero()
        {
            ProcessTask task = RunMSTest("\"/test:MbUnit v0.0.0.0/MbUnit.TestResources/FailingTests/Fail\"");
            OldStringAssert.Like(task.ConsoleOutput, "Failed *MbUnit v0.0.0.0/MbUnit.TestResources/FailingTests/Fail");
            OldStringAssert.Like(task.ConsoleOutput, "Failed *MbUnit v0.0.0.0/MbUnit.TestResources/FailingTests/FailAgain");
            Assert.Contains(task.ConsoleOutput, "0/2 test(s) Passed, 2 Failed");
            Assert.AreEqual(1, task.ExitCode);
        }

        private static ProcessTask RunMSTest(string options)
        {
            string value = (string) Registry.GetValue(@"HKEY_LOCAL_MACHINE\Software\Microsoft\VisualStudio\9.0", "InstallDir", null);
            if (value == null)
                Assert.Inconclusive("Visual Studio 2008 does not appear to be installed.");

            string executablePath = Path.Combine(value, "MSTest.exe");
            if (! File.Exists(executablePath))
                Assert.Inconclusive("Visual Studio 2008 appears to be installed but MSTest.exe was not found.");

            string testAssemblyPath = AssemblyUtils.GetAssemblyLocalPath(typeof(SimpleTest).Assembly);
            string workingDirectory = Path.GetDirectoryName(AssemblyUtils.GetAssemblyLocalPath(typeof(GallioTipIntegrationTest).Assembly));

            ProcessTask task = Tasks.StartProcessTask(executablePath,
                "\"/testcontainer:" + testAssemblyPath + "\" " + options,
               workingDirectory);

            Assert.IsTrue(task.Run(TimeSpan.FromSeconds(60)), "A timeout occurred.");
            return task;
        }

        private static void RunInstallScript(string options)
        {
            string installScriptDir = Path.Combine(AssemblyUtils.GetAssemblyLocalPath(typeof(GallioTipIntegrationTest).Assembly), @"..\..\..");
            string installScriptPath = Path.Combine(installScriptDir, "Install-MSTestRunner.bat");

            // We start our process manually because if we use the tasks, it will redirect output
            // which causes the "sed" program used by the script to malfunction.  Crazy Windows shell...
            Process process = Process.Start(installScriptPath, options);
            process.WaitForExit();
            Assert.AreEqual(0, process.ExitCode, "The install script failed.");
        }
    }
}
