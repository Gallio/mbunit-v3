// Copyright 2005-2009 Gallio Project - http://www.gallio.org/
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
using System.Linq;
using Gallio.Common.Collections;
using Gallio.Runner.Reports.Schema;
using Gallio.Runtime.Logging;
using Gallio.Runtime.ProgressMonitoring;
using Gallio.Model;
using Gallio.Model.Filters;
using Gallio.Common.Reflection;
using Gallio.Runner;
using MbUnit.Framework;
using Gallio.NAntTasks;
using NAnt.Core;
using NAnt.Core.Types;

namespace Gallio.NAntTasks.Tests
{
    /// <summary>
    /// A set of unit tests for the <see cref="GallioTask" /> class (a custom Gallio task for NAnt).
    /// </summary>
    /// <remarks>
    /// <para>
    /// If you modify these tests please make sure to review the corresponding
    /// tests for the Gallio MSBuild task, since the both the tasks for NAnt and MSBuild
    /// should exhibit a similar behavior and features set.
    /// </para>
    /// </remarks>
    [TestFixture]
    [TestsOn(typeof(GallioTask))]
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
                Assert.IsTrue(launcher.EchoResults);
                Assert.IsTrue(launcher.TestExecutionOptions.FilterSet.IsEmpty);
                Assert.AreEqual(LogSeverity.Important, ((FilteredLogger)launcher.Logger).MinSeverity);
                Assert.IsInstanceOfType(typeof(LogProgressMonitorProvider), launcher.ProgressMonitorProvider);
                Assert.AreEqual("Reports", launcher.TestProject.ReportDirectory);
                Assert.IsFalse(launcher.TestProject.IsReportDirectorySpecified);
                Assert.AreElementsEqual(new string[] { }, launcher.ReportFormats);
                Assert.AreEqual("test-report-{0}-{1}", launcher.TestProject.ReportNameFormat);
                Assert.IsFalse(launcher.TestProject.IsReportNameFormatSpecified);
                Assert.IsFalse(launcher.ShowReports);
                Assert.IsNull(launcher.RunTimeLimit);

                Assert.AreEqual(StandardTestRunnerFactoryNames.IsolatedProcess, launcher.TestProject.TestRunnerFactoryName);
                Assert.IsFalse(launcher.TestProject.IsTestRunnerFactoryNameSpecified);
                Assert.AreEqual(0, launcher.TestProject.TestRunnerExtensions.Count);
                Assert.AreElementsEqual(new string[] { }, launcher.TestProject.TestRunnerExtensionSpecifications);

                Assert.IsNull(launcher.RuntimeSetup.ConfigurationFilePath);
                Assert.AreEqual(Path.GetDirectoryName(AssemblyUtils.GetAssemblyLocalPath(typeof(GallioTask).Assembly)), launcher.RuntimeSetup.RuntimePath);
                Assert.AreElementsEqual(new string[] { }, launcher.RuntimeSetup.PluginDirectories);

                Assert.AreElementsEqual(new string[] { }, from x in launcher.FilePatterns select x.ToString());
                Assert.AreElementsEqual(new string[] { }, from x in launcher.TestProject.TestPackage.HintDirectories select x.ToString());

                Assert.IsNull(launcher.TestProject.TestPackage.ApplicationBaseDirectory);
                Assert.IsFalse(launcher.TestProject.TestPackage.IsApplicationBaseDirectorySpecified);
                Assert.IsNull(launcher.TestProject.TestPackage.WorkingDirectory);
                Assert.IsFalse(launcher.TestProject.TestPackage.IsWorkingDirectorySpecified);
                Assert.IsFalse(launcher.TestProject.TestPackage.ShadowCopy);
                Assert.IsFalse(launcher.TestProject.TestPackage.IsShadowCopySpecified);
                Assert.IsFalse(launcher.TestProject.TestPackage.Debug);
                Assert.IsFalse(launcher.TestProject.TestPackage.IsDebugSpecified);
                Assert.IsNull(launcher.TestProject.TestPackage.RuntimeVersion);
                Assert.IsFalse(launcher.TestProject.TestPackage.IsRuntimeVersionSpecified);

                Assert.AreEqual(new PropertySet(), launcher.TestRunnerOptions.Properties);
                Assert.AreEqual(new PropertySet(), launcher.ReportFormatterOptions.Properties);

                TestLauncherResult result = new TestLauncherResult(new Report());
                result.SetResultCode(ResultCode.Success);
                return result;
            });

            Assert.DoesNotThrow(delegate { task.InternalExecute(); });
        }

        [Test]
        public void TaskPassesSpecifiedArgumentsToLauncher()
        {
            StubbedGallioTask task = new StubbedGallioTask();
            task.DoNotRun = true;
            task.EchoResults = false;
            task.Filter = "Type: SimpleTest";
            task.ReportDirectory = "dir";
            task.ReportTypes = "XML;Html";
            task.ReportNameFormat = "report";
            task.ShowReports = true;
            task.RunTimeLimit = 7200; // seconds
            task.Verbosity = Verbosity.Debug;

            task.RunnerType = StandardTestRunnerFactoryNames.Local;
            task.RunnerExtensions.Add(new Argument("DebugExtension,Gallio"));

            task.PluginDirectories = new DirSet[] { CreateDirSet("plugin") };
            task.Files = new FileSet[] { CreateFileSet("assembly1"), CreateFileSet("assembly2") };
            task.HintDirectories = new DirSet[] { CreateDirSet("hint1"), CreateDirSet("hint2") };

            task.ApplicationBaseDirectory = "baseDir";
            task.ShadowCopy = true;
            task.Debug = true;
            task.WorkingDirectory = "workingDir";
            task.RuntimeVersion = "v4.0.20506";

            task.RunnerProperties.AddRange(new[] { new Argument("RunnerOption1=RunnerValue1"), new Argument("  RunnerOption2  "), new Argument("RunnerOption3 = 'RunnerValue3'"), new Argument("RunnerOption4=\"'RunnerValue4'\"") });
            task.ReportFormatterProperties.AddRange(new[] { new Argument("FormatterOption1=FormatterValue1"), new Argument("  FormatterOption2  "), new Argument("FormatterOption3 = 'FormatterValue3'"), new Argument("FormatterOption4=\"'FormatterValue4'\"") });

            task.SetRunLauncherAction(delegate(TestLauncher launcher)
            {
                Assert.IsTrue(launcher.DoNotRun);
                Assert.IsFalse(launcher.EchoResults);
                Assert.AreEqual("Type: SimpleTest", launcher.TestExecutionOptions.FilterSet.ToFilterSetExpr());
                Assert.AreEqual(LogSeverity.Debug, ((FilteredLogger)launcher.Logger).MinSeverity);
                Assert.IsInstanceOfType(typeof(LogProgressMonitorProvider), launcher.ProgressMonitorProvider);
                Assert.AreEqual("dir", launcher.TestProject.ReportDirectory);
                Assert.IsTrue(launcher.TestProject.IsReportDirectorySpecified);
                Assert.AreElementsEqual(new string[] { "XML", "Html" }, launcher.ReportFormats);
                Assert.AreEqual("report", launcher.TestProject.ReportNameFormat);
                Assert.IsTrue(launcher.TestProject.IsReportNameFormatSpecified);
                Assert.IsTrue(launcher.ShowReports);
                Assert.AreEqual(TimeSpan.FromMinutes(120), launcher.RunTimeLimit);

                Assert.AreEqual(StandardTestRunnerFactoryNames.Local, launcher.TestProject.TestRunnerFactoryName);
                Assert.IsTrue(launcher.TestProject.IsTestRunnerFactoryNameSpecified);
                Assert.AreEqual(0, launcher.TestProject.TestRunnerExtensions.Count);
                Assert.AreElementsEqual(new string[] { "DebugExtension,Gallio" }, launcher.TestProject.TestRunnerExtensionSpecifications);

                Assert.IsNull(launcher.RuntimeSetup.ConfigurationFilePath);
                Assert.AreEqual(Path.GetDirectoryName(AssemblyUtils.GetAssemblyLocalPath(typeof(GallioTask).Assembly)), launcher.RuntimeSetup.RuntimePath);
                Assert.AreElementsEqual(new string[] { "plugin" }, launcher.RuntimeSetup.PluginDirectories);

                Assert.AreElementsEqual(new string[] { "assembly1", "assembly2" }, from x in launcher.FilePatterns select x.ToString());
                Assert.AreElementsEqual(new string[] { "hint1", "hint2" }, from x in launcher.TestProject.TestPackage.HintDirectories select x.ToString());

                Assert.AreEqual("baseDir", launcher.TestProject.TestPackage.ApplicationBaseDirectory.ToString());
                Assert.IsTrue(launcher.TestProject.TestPackage.IsApplicationBaseDirectorySpecified);
                Assert.AreEqual("workingDir", launcher.TestProject.TestPackage.WorkingDirectory.ToString());
                Assert.IsTrue(launcher.TestProject.TestPackage.IsWorkingDirectorySpecified);
                Assert.IsTrue(launcher.TestProject.TestPackage.ShadowCopy);
                Assert.IsTrue(launcher.TestProject.TestPackage.IsShadowCopySpecified);
                Assert.IsTrue(launcher.TestProject.TestPackage.Debug);
                Assert.IsTrue(launcher.TestProject.TestPackage.IsDebugSpecified);
                Assert.AreEqual("v4.0.20506", launcher.TestProject.TestPackage.RuntimeVersion);
                Assert.IsTrue(launcher.TestProject.TestPackage.IsRuntimeVersionSpecified);

                Assert.AreEqual(new PropertySet()
                {
                    { "RunnerOption1", "RunnerValue1" },
                    { "RunnerOption2", "" },
                    { "RunnerOption3", "RunnerValue3" },
                    { "RunnerOption4", "'RunnerValue4'" }
                }, launcher.TestRunnerOptions.Properties);

                Assert.AreEqual(new PropertySet()
                {
                    { "FormatterOption1", "FormatterValue1" },
                    { "FormatterOption2", "" },
                    { "FormatterOption3", "FormatterValue3" },
                    { "FormatterOption4", "'FormatterValue4'" }
                }, launcher.ReportFormatterOptions.Properties);

                TestLauncherResult result = new TestLauncherResult(new Report());
                result.SetResultCode(ResultCode.NoTests);
                return result;
            });

            Assert.DoesNotThrow(delegate { task.InternalExecute(); });
        }

        [Test]
        public void TaskExposesResultsReturnedByLauncher()
        {
            StubbedGallioTask task = new StubbedGallioTask();
            task.ResultProperty = "ExitCode";
            task.StatisticsPropertiesPrefix = "Stats.";

            task.SetRunLauncherAction(delegate
            {
                Report report = new Report();
                report.TestPackageRun = new TestPackageRun();
                report.TestPackageRun.Statistics.AssertCount = 42;
                report.TestPackageRun.Statistics.Duration = 1.5;
                report.TestPackageRun.Statistics.FailedCount = 5;
                report.TestPackageRun.Statistics.InconclusiveCount = 11;
                report.TestPackageRun.Statistics.PassedCount = 21;
                report.TestPackageRun.Statistics.SkippedCount = 1;
                report.TestPackageRun.Statistics.StepCount = 30;
                report.TestPackageRun.Statistics.TestCount = 28;

                TestLauncherResult result = new TestLauncherResult(report);
                result.SetResultCode(ResultCode.Failure);
                return result;
            });

            Assert.Throws<BuildException>(delegate { task.InternalExecute(); });

            Assert.AreEqual(ResultCode.Failure.ToString(), task.Properties["ExitCode"]);
            Assert.AreEqual("42", task.Properties["Stats.AssertCount"]);
            Assert.AreEqual("1.5", task.Properties["Stats.Duration"]);
            Assert.AreEqual("5", task.Properties["Stats.FailedCount"]);
            Assert.AreEqual("11", task.Properties["Stats.InconclusiveCount"]);
            Assert.AreEqual("21", task.Properties["Stats.PassedCount"]);
            Assert.AreEqual("1", task.Properties["Stats.SkippedCount"]);
            Assert.AreEqual("30", task.Properties["Stats.StepCount"]);
            Assert.AreEqual("28", task.Properties["Stats.TestCount"]);
        }

        [Test]
        public void FailOnErrorWhenSetToFalseCausesABuildExceptionToNotBeThrownOnFailures()
        {
            StubbedGallioTask task = new StubbedGallioTask();
            task.FailOnError = false;

            task.SetRunLauncherAction(delegate
            {
                TestLauncherResult result = new TestLauncherResult(new Report());
                result.SetResultCode(ResultCode.Failure);
                return result;
            });

            Assert.DoesNotThrow(delegate { task.InternalExecute(); });
        }

        [Test]
        public void ExceptionsCauseTheTaskToFailRegardlessOfFailOnError()
        {
            StubbedGallioTask task = new StubbedGallioTask();
            task.FailOnError = false;

            task.SetRunLauncherAction(delegate
            {
                throw new Exception("Simulated error.");
            });

            Assert.Throws<Exception>(delegate { task.InternalExecute(); });
        }

        private static DirSet CreateDirSet(string dirName)
        {
            DirSet dirSet = new DirSet();
            dirSet.DirectoryNames.Add(dirName);
            return dirSet;
        }

        private static FileSet CreateFileSet(string fileName)
        {
            FileSet fileSet = new FileSet();
            fileSet.FileNames.Add(fileName);
            return fileSet;
        }
    }
}
