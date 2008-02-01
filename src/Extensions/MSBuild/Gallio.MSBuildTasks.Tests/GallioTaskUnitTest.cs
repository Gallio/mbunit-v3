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

using System;
using System.IO;
using Gallio.Hosting;
using Gallio.Hosting.ProgressMonitoring;
using Gallio.Model;
using Gallio.Model.Filters;
using Gallio.Runner.Reports;
using MbUnit.Framework;
using Gallio.Runner;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Gallio.MSBuildTasks.Tests
{
    /// <summary>
    /// A set of unit tests for the Gallio class (a custom Gallio task for MSBuild).
    /// </summary>
    /// <remarks>
    /// If you modify these tests please make sure to review the corresponding
    /// tests for the Gallio NAnt task, since the both the tasks for NAnt and MSBuild
    /// should exhibit a similar behavior and features set.
    /// </remarks>
    [TestFixture]
    [TestsOn(typeof(Gallio))]
    [Category("UnitTests")]
    public class GallioTaskUnitTest
    {
        [Test]
        public void TaskPassesDefaultArgumentsToLauncher()
        {
            StubbedGallioTask task = new StubbedGallioTask();

            task.SetRunLauncherAction(delegate(TestLauncher launcher)
            {
                Assert.IsFalse(launcher.DoNotRun);
                Assert.IsFalse(launcher.EchoResults);
                Assert.IsInstanceOfType(typeof(AnyFilter<ITest>), launcher.Filter);
                Assert.IsInstanceOfType(typeof(TaskLogger), launcher.Logger);
                Assert.IsInstanceOfType(typeof(LogProgressMonitorProvider), launcher.ProgressMonitorProvider);
                Assert.AreEqual("", launcher.ReportDirectory);
                Assert.AreEqual(0, launcher.ReportFormatOptions.Count);
                CollectionAssert.AreElementsEqual(new string[] { }, launcher.ReportFormats);
                Assert.AreEqual("test-report-{0}-{1}", launcher.ReportNameFormat);
                Assert.IsFalse(launcher.ShowReports);
                Assert.AreEqual(StandardTestRunnerFactoryNames.IsolatedProcess, launcher.TestRunnerFactoryName);
                Assert.AreEqual(0, launcher.TestRunnerOptions.Count);

                Assert.AreEqual(1, launcher.CustomMonitors.Count);
                Assert.IsInstanceOfType(typeof(TaskTestRunnerMonitor), launcher.CustomMonitors[0]);

                Assert.IsNull(launcher.RuntimeSetup.ConfigurationFilePath);
                Assert.AreEqual(Path.GetDirectoryName(Loader.GetAssemblyLocalPath(typeof(Gallio).Assembly)), launcher.RuntimeSetup.InstallationPath);
                CollectionAssert.AreElementsEqual(new string[] { }, launcher.RuntimeSetup.PluginDirectories);
                Assert.IsNull(launcher.RuntimeSetup.RuntimeFactoryType);

                CollectionAssert.AreElementsEqual(new string[] { }, launcher.TestPackageConfig.AssemblyFiles);
                CollectionAssert.AreElementsEqual(new string[] { }, launcher.TestPackageConfig.HintDirectories);

                Assert.AreEqual("", launcher.TestPackageConfig.HostSetup.ApplicationBaseDirectory);
                Assert.IsFalse(launcher.TestPackageConfig.HostSetup.ShadowCopy);
                Assert.AreEqual("", launcher.TestPackageConfig.HostSetup.WorkingDirectory);

                TestLauncherResult result = new TestLauncherResult(new Report());
                result.SetResultCode(ResultCode.Success);
                return result;
            });

            Assert.IsTrue(task.InternalExecute());
        }

        [Test]
        public void TaskPassesSpecifiedArgumentsToLauncher()
        {
            StubbedGallioTask task = new StubbedGallioTask();
            task.DoNotRun = true;
            task.EchoResults = false;
            task.Filter = "Type: SimpleTest";
            task.ReportDirectory = new TaskItem("dir");
            task.ReportTypes = new string[] { "XML", "Html" };
            task.ReportNameFormat = "report";
            task.ShowReports = true;
            task.RunnerType = StandardTestRunnerFactoryNames.LocalAppDomain;

            task.PluginDirectories = new ITaskItem[] { new TaskItem("plugin") };
            task.Assemblies = new ITaskItem[] { new TaskItem("assembly1"), new TaskItem("assembly2") };
            task.HintDirectories = new ITaskItem[] { new TaskItem("hint1"), new TaskItem("hint2") };

            task.ApplicationBaseDirectory = new TaskItem("baseDir");
            task.ShadowCopy = true;
            task.WorkingDirectory = new TaskItem("workingDir");

            task.SetRunLauncherAction(delegate(TestLauncher launcher)
            {
                Assert.IsTrue(launcher.DoNotRun);
                Assert.IsFalse(launcher.EchoResults);
                Assert.AreEqual("Type: SimpleTest", launcher.Filter.ToFilterExpr());
                Assert.IsInstanceOfType(typeof(TaskLogger), launcher.Logger);
                Assert.IsInstanceOfType(typeof(LogProgressMonitorProvider), launcher.ProgressMonitorProvider);
                Assert.AreEqual("dir", launcher.ReportDirectory);
                Assert.AreEqual(0, launcher.ReportFormatOptions.Count);
                CollectionAssert.AreElementsEqual(new string[] { "XML", "Html" }, launcher.ReportFormats);
                Assert.AreEqual("report", launcher.ReportNameFormat);
                Assert.IsTrue(launcher.ShowReports);
                Assert.AreEqual(StandardTestRunnerFactoryNames.LocalAppDomain, launcher.TestRunnerFactoryName);
                Assert.AreEqual(0, launcher.TestRunnerOptions.Count);

                Assert.AreEqual(0, launcher.CustomMonitors.Count);

                Assert.IsNull(launcher.RuntimeSetup.ConfigurationFilePath);
                Assert.AreEqual(Path.GetDirectoryName(Loader.GetAssemblyLocalPath(typeof(Gallio).Assembly)), launcher.RuntimeSetup.InstallationPath);
                CollectionAssert.AreElementsEqual(new string[] { "plugin" }, launcher.RuntimeSetup.PluginDirectories);
                Assert.IsNull(launcher.RuntimeSetup.RuntimeFactoryType);

                CollectionAssert.AreElementsEqual(new string[] { "assembly1", "assembly2" }, launcher.TestPackageConfig.AssemblyFiles);
                CollectionAssert.AreElementsEqual(new string[] { "hint1", "hint2" }, launcher.TestPackageConfig.HintDirectories);

                Assert.AreEqual("baseDir", launcher.TestPackageConfig.HostSetup.ApplicationBaseDirectory);
                Assert.IsTrue(launcher.TestPackageConfig.HostSetup.ShadowCopy);
                Assert.AreEqual("workingDir", launcher.TestPackageConfig.HostSetup.WorkingDirectory);

                TestLauncherResult result = new TestLauncherResult(new Report());
                result.SetResultCode(ResultCode.NoTests);
                return result;
            });

            Assert.IsTrue(task.InternalExecute());
        }

        [Test]
        public void TaskExposesResultsReturnedByLauncher()
        {
            StubbedGallioTask task = new StubbedGallioTask();

            task.SetRunLauncherAction(delegate
            {
                Report report = new Report();
                report.PackageRun = new PackageRun();
                report.PackageRun.Statistics.AssertCount = 42;
                report.PackageRun.Statistics.Duration = 1.5;
                report.PackageRun.Statistics.FailureCount = 5;
                report.PackageRun.Statistics.IgnoreCount = 2;
                report.PackageRun.Statistics.InconclusiveCount = 11;
                report.PackageRun.Statistics.PassCount = 21;
                report.PackageRun.Statistics.SkipCount = 1;
                report.PackageRun.Statistics.StepCount = 30;
                report.PackageRun.Statistics.TestCount = 28;

                TestLauncherResult result = new TestLauncherResult(report);
                result.SetResultCode(ResultCode.Failure);
                return result;
            });

            Assert.IsFalse(task.InternalExecute());

            Assert.AreEqual(ResultCode.Failure, task.ExitCode);
            Assert.AreEqual(42, task.AssertCount);
            Assert.AreEqual(1.5, task.Duration);
            Assert.AreEqual(5, task.FailureCount);
            Assert.AreEqual(2, task.IgnoreCount);
            Assert.AreEqual(11, task.InconclusiveCount);
            Assert.AreEqual(21, task.PassCount);
            Assert.AreEqual(1, task.SkipCount);
            Assert.AreEqual(30, task.StepCount);
            Assert.AreEqual(28, task.TestCount);
        }

        [Test]
        public void IgnoreFailuresCausesTrueToBeReturnedEvenWhenFailuresOccur()
        {
            StubbedGallioTask task = new StubbedGallioTask();
            task.IgnoreFailures = true;

            task.SetRunLauncherAction(delegate
            {
                TestLauncherResult result = new TestLauncherResult(new Report());
                result.SetResultCode(ResultCode.Failure);
                return result;
            });

            Assert.IsTrue(task.Execute());
        }

        [Test]
        public void ExceptionsCauseTheTaskToFailRegardlessOfIgnoreFailuresFlag()
        {
            StubbedGallioTask task = new StubbedGallioTask();
            task.IgnoreFailures = true;

            task.SetRunLauncherAction(delegate
            {
                throw new Exception("Simulated error.");
            });

            Assert.IsFalse(task.Execute());
        }
    }
}
