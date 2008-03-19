// Copyright 2005-2008 Gallio Project - http://www.gallio.org/
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

using System;
using System.IO;
using Gallio.Concurrency;
using Gallio.Framework;
using Gallio.Hosting;
using MbUnit.Framework;
using MbUnit.TestResources;

namespace Gallio.Echo.Tests
{
    [TestFixture]
    [TestsOn(typeof(EchoProgram))]
    public class EchoProgramIntegrationTest
    {
        [Test]
        public void EchoPrintsCorrectOutputForPassingTestsAndReturnsAnExitCodeOfZero()
        {
            ProcessTask task = RunEcho("/filter:Type:PassingTests");
            Assert.Contains(task.ConsoleOutput, "2 run, 2 passed, 0 failed, 0 inconclusive, 0 skipped");
            Assert.AreEqual(task.ExitCode, 0, "Exit code for passing tests should be zero.");
        }

        [Test]
        public void EchoPrintsCorrectOutputForPassingAndFailingTestsAndReturnsAnExitCodeOfOne()
        {
            ProcessTask task = RunEcho("/filter:Type:SimpleTest");
            Assert.Contains(task.ConsoleOutput, "2 run, 1 passed, 1 failed, 0 inconclusive, 0 skipped");
            Assert.AreEqual(task.ExitCode, 1, "Exit code for failing tests should be one.");
        }

        [Test]
        public void EchoDoesNotTerminateAbruptlyOnUnhandledExceptions()
        {
            ProcessTask task = RunEcho("/filter:Type:UnhandledExceptionTest");
            Assert.Contains(task.ConsoleOutput, "2 run, 2 passed, 0 failed, 0 inconclusive, 0 skipped");
            Assert.AreEqual(task.ExitCode, 0, "Exit code should be zero because the unhandled exception test still passes.");
        }

        private ProcessTask RunEcho(string options)
        {
            string testAssemblyPath = Loader.GetAssemblyLocalPath(typeof(SimpleTest).Assembly);
            string workingDirectory = Path.GetDirectoryName((Loader.GetAssemblyLocalPath(GetType().Assembly)));
            string executablePath = Path.Combine(workingDirectory, "Gallio.Echo.exe");

            ProcessTask task = Tasks.StartProcessTask(executablePath,
                "\"" + testAssemblyPath + "\" /pd:\"" + Runtime.InstallationPath + "\" " + options,
                workingDirectory);

            Assert.IsTrue(task.Run(TimeSpan.FromSeconds(60)), "A timeout occurred.");
            return task;
        }
    }
}
