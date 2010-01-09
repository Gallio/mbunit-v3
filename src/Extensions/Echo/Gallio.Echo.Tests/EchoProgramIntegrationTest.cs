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
using System.IO;
using Gallio.Common.Concurrency;
using Gallio.Runtime;
using Gallio.Framework;
using Gallio.Common.Reflection;
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
            ProcessTask task = RunEcho("/ignore-annotations /filter:Type:PassingTests");
            Assert.Contains(task.ConsoleOutput, "2 run, 2 passed, 0 failed, 0 inconclusive, 0 skipped");
            Assert.AreEqual(task.ExitCode, 0, "Exit code for passing tests should be zero.");
        }

        [Test]
        public void EchoPrintsCorrectOutputForPassingAndFailingTestsAndReturnsAnExitCodeOfOne()
        {
            ProcessTask task = RunEcho("/ignore-annotations /filter:Type:SimpleTest");
            Assert.Contains(task.ConsoleOutput, "2 run, 1 passed, 1 failed, 0 inconclusive, 0 skipped");
            Assert.AreEqual(task.ExitCode, 1, "Exit code for failing tests should be one.");
        }

        [Test]
        public void EchoDoesNotTerminateAbruptlyOnUnhandledExceptions()
        {
            ProcessTask task = RunEcho("/ignore-annotations /filter:Type:UnhandledExceptionTest");
            Assert.Contains(task.ConsoleOutput, "2 run, 2 passed, 0 failed, 0 inconclusive, 0 skipped");
            Assert.Contains(task.ConsoleOutput, "Unhandled!");
            Assert.AreEqual(task.ExitCode, 0, "Exit code should be zero because the unhandled exception test still passes.");
        }

        [Test]
        public void EchoSupportsCustomExtensions()
        {
            ProcessTask task = RunEcho("/ignore-annotations /filter:Type:PassingTests /runner-extension:DebugExtension,Gallio /v:Debug");
            Assert.Contains(task.ConsoleOutput, "2 run, 2 passed, 0 failed, 0 inconclusive, 0 skipped");
            Assert.Contains(task.ConsoleOutput, "Runner Extensions: DebugExtension,Gallio");
            Assert.Contains(task.ConsoleOutput, "TestStepStarted");
            Assert.AreEqual(task.ExitCode, 0, "Exit code should be zero because the unhandled exception test still passes.");
        }

        private ProcessTask RunEcho(string options)
        {
            string testAssemblyPath = AssemblyUtils.GetAssemblyLocalPath(typeof(SimpleTest).Assembly);
            string workingDirectory = Path.GetDirectoryName((AssemblyUtils.GetAssemblyLocalPath(GetType().Assembly)));
#if DEBUG
            string executablePath = Path.Combine(workingDirectory, "Gallio.Echo.exe");
#else
            string executablePath = Path.Combine(RuntimeAccessor.Instance.GetRuntimeSetup().RuntimePath, "Gallio.Echo.exe");
#endif

            ProcessTask task = Tasks.StartProcessTask(executablePath,
                "\"" + testAssemblyPath + "\" /pd:\"" + RuntimeAccessor.RuntimePath + "\" " + options,
                workingDirectory);

            Assert.IsTrue(task.Run(TimeSpan.FromSeconds(60)), "A timeout occurred.");
            return task;
        }
    }
}
