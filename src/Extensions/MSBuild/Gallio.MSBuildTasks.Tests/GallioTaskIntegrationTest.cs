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

using System.Text;
using Gallio.Concurrency;
using Gallio.Runtime;
using Gallio.Framework;
using Gallio.Reflection;
using MbUnit.Framework;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Gallio.MSBuildTasks.Tests
{
    /// <summary>
    /// These are basically integration tests that call the MSBuild executable
    /// and run a series of pre-configured targets that exercise the Gallio task
    /// in various ways.
    /// </summary>
    [TestFixture]
    [Author("Julian Hidalgo")]
    [TestsOn(typeof(Gallio))]
    [Category("IntegrationTests")]
    public class GallioTaskIntegrationTest
    {
        private string executablePath;
        private string workingDirectory;

        [FixtureSetUp]
        public void FixtureSetUp()
        {
            string frameworkPath = RuntimeEnvironment.GetRuntimeDirectory();
            executablePath = frameworkPath + @"MSBuild.exe";

            Assert.IsTrue(File.Exists(executablePath), "Cannot find the MSBuild executable!");

            workingDirectory = Path.Combine(Path.GetDirectoryName(AssemblyUtils.GetAssemblyLocalPath(GetType().Assembly)), @"..\TestBuildFiles");
        }

        [Test]
        public void TaskSupportsCustomExtensions()
        {
            ProcessTask task = RunMSBuild("Extensions");
            Assert.Contains(task.ConsoleOutput, "TestStepStarted"); // text appears in the debug output
            Assert.AreEqual(0, task.ExitCode);
        }

        [Test]
        [Row("PassingTests", true, Description=@"
            This target only runs tests that pass,
            so the task should have set the ExitCode property to ResultCode.Success.")]
        [Row("FailingTests", false, Description = @"
            This target only runs tests that fail,
            so the task should have set the ExitCode property to ResultCode.Failure.
            MSBuild should return a failure result since we are not ignoring failures.")]
        [Row("FailingTestsWithIgnoreFailures", true, Description = @"
            This target only runs tests that fail,
            so the task should have set the ExitCode property to ResultCode.Failure.
            However, we are ignoring failures so MSBuild should return a success result.")]
        [Row("NoAssemblies", true, Description=@"
            There are no test assemblies specified in this target
            so the task should have set the ExitCode property to ResultCode.NoTests.
            We are ignoring failures in this so MSBuild should return a success result.")]
        [Row("NoTests", true, Description = @"
            There are no tests selected by the filters specified in this target
            so the task should have set the ExitCode property to ResultCode.NoTests.
            We are ignoring failures in this so MSBuild should return a success result.")]
        [Row("NoFilter", true, Description = @"
            This target runs tests without a filter.  We are ignoring failures
            so MSBuild should return true.")]
        [Row("UnhandledException", true, Description = @"
            This target runs tests with unhandled exceptions that nevertheless pass.
            MSBuild should not terminate abruptly when this occurs so it should return a successful result code.")]
        public void ExpectedResultIsObtained(string target, bool expectedResult)
        {
            ProcessTask task = RunMSBuild(target);
            Assert.AreEqual(expectedResult, task.ExitCode == 0, "Unexpected exit code.");
        }

        private ProcessTask RunMSBuild(string target)
        {
            ProcessTask task = Tasks.StartProcessTask(executablePath,
                String.Concat("Integration.proj /v:detailed /t:", target,
                " /p:GallioPath=\"", RuntimeAccessor.InstallationPath, "\""),
                workingDirectory);

            Assert.IsTrue(task.Run(TimeSpan.FromSeconds(60)), "A timeout occurred.");
            return task;
        }
    }
}
