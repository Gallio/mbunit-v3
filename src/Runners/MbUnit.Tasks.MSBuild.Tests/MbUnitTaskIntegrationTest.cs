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
using System.Text;
using MbUnit2::MbUnit.Framework;
using MbUnit.Hosting;
using System;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using ProcessRunner=MbUnit.Framework.ProcessRunner;

namespace MbUnit.Tasks.MSBuild.Tests
{
    /// <summary>
    /// These are basically integration tests that call the MSBuild executable
    /// and run a series of pre-configured targets that exercise the MbUnit task
    /// in various ways.
    /// </summary>
    [TestFixture]
    [Author("Julian Hidalgo")]
    [TestsOn(typeof(MbUnit))]
    [FixtureCategory("IntegrationTests")]
    public class MbUnitTaskIntegrationTest
    {
        private string executablePath;
        private string workingDirectory;

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            string frameworkPath = RuntimeEnvironment.GetRuntimeDirectory();
            executablePath = frameworkPath + @"MSBuild.exe";

            Assert.IsTrue(File.Exists(executablePath), "Cannot find the MSBuild executable!");

            workingDirectory = Path.Combine(Path.GetDirectoryName(Loader.GetAssemblyLocalPath(GetType().Assembly)), @"..\TestBuildFiles");
        }

        [RowTest]
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
        public void RunMSBuild(string target, bool expectedResult)
        {
            ProcessRunner runner = new ProcessRunner(executablePath,
                String.Concat("Integration.proj /t:", target,
                " /p:GallioPath=\"", Loader.InstallationPath, "\""));
            runner.WorkingDirectory = workingDirectory;

            runner.Run(10000);
            Assert.AreEqual(expectedResult, runner.ExitCode == 0, "Unexpected exit code.");
        }
    }
}
