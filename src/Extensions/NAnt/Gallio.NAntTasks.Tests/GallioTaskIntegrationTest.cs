// Copyright 2008 MbUnit Project - http://www.mbunit.com/
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

using Gallio.Concurrency;
using Gallio.Framework;
using MbUnit.Framework;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Gallio.Hosting;
using Gallio.Runner;

namespace Gallio.NAntTasks.Tests
{
    /// <summary>
    /// These are basically integration tests that call the NAnt executable and
    /// verify the Gallio's Exit Code and the process' Exit Code.
    /// </summary>
    [TestFixture]
    [Author("Julian Hidalgo")]
    [TestsOn(typeof(GallioTask))]
    [Category("IntegrationTests")]
    public class GallioTaskIntegrationTest
    {
        private string executablePath;
        private string workingDirectory;

        [FixtureSetUp]
        public void FixtureSetUp()
        {
            string binPath = Path.GetDirectoryName(Loader.GetAssemblyLocalPath(GetType().Assembly));

            workingDirectory = Path.Combine(binPath, @"..\TestBuildFiles");
            executablePath = Path.Combine(binPath, @"..\..\libs\NAnt.exe");

            Assert.IsTrue(File.Exists(executablePath), "Cannot find the NAnt executable!");
        }

        [Test]
        [Row("PassingTests", true, Description = @"
            This target only runs tests that pass,
            so the task should have set the ExitCode property to ResultCode.Success.")]
        [Row("FailingTests", false, Description = @"
            This target only runs tests that fail,
            so the task should have set the ExitCode property to ResultCode.Failure.
            NAnt should return a failure result since we are not ignoring failures.")]
        [Row("FailingTestsWithIgnoreFailures", true, Description = @"
            This target only runs tests that fail,
            so the task should have set the ExitCode property to ResultCode.Failure.
            However, we are ignoring failures so NAnt should return a success result.")]
        [Row("NoAssemblies", true, Description = @"
            There are no test assemblies specified in this target
            so the task should have set the ExitCode property to ResultCode.NoTests.
            We are ignoring failures in this so NAnt should return a success result.")]
        [Row("NoTests", true, Description = @"
            There are no tests selected by the filters specified in this target
            so the task should have set the ExitCode property to ResultCode.NoTests.
            We are ignoring failures in this so NAnt should return a success result.")]
        [Row("NoFilter", true, Description = @"
            This target runs tests without a filter.  We are ignoring failures
            so NAnt should return true.")]
        [Row("UnhandledException", true, Description = @"
            This target runs tests with unhandled exceptions that nevertheless pass.
            NAnt should not terminate abruptly when this occurs so it should return a successful result code.")]
        public void RunNAnt(string target, bool expectedResult)
        {
            ProcessTask task = Tasks.StartProcessTask(executablePath,
                String.Concat("/f:Integration.build ", target,
                " /D:GallioPath=\"", Loader.InstallationPath, "\""),
                workingDirectory);

            Assert.IsTrue(task.Run(TimeSpan.FromSeconds(60)), "A timeout occurred.");
            Assert.AreEqual(expectedResult, task.ExitCode == 0, "Unexpected exit code.");
        }
    }
}
