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
using Gallio.Model;
using Gallio.Model.Filters;
using Gallio.Common.Reflection;
using Gallio.Runner;
using Gallio.Runner.Reports.Schema;
using MbUnit.Framework;
using Gallio.PowerShellCommands;

namespace Gallio.PowerShellCommands.Tests
{
    [TestFixture]
    [TestsOn(typeof(RunGallioCommand))]
    [Category("UnitTests")]
    public class RunGallioCommandUnitTest
    {
        [Test]
        public void TaskPassesDefaultArgumentsToLauncher()
        {
            StubbedRunGallioCommand task = new StubbedRunGallioCommand();

            task.SetRunLauncherAction(delegate(TestLauncher launcher)
            {
                Assert.IsFalse(launcher.DoNotRun);
                Assert.IsTrue(launcher.EchoResults);
                Assert.IsTrue(launcher.TestExecutionOptions.FilterSet.IsEmpty);
                Assert.IsInstanceOfType(typeof(CommandLogger), launcher.Logger);
                Assert.IsInstanceOfType(typeof(CommandProgressMonitorProvider), launcher.ProgressMonitorProvider);
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
                Assert.AreEqual(Path.GetDirectoryName(AssemblyUtils.GetAssemblyLocalPath(typeof(RunGallioCommand).Assembly)), launcher.RuntimeSetup.RuntimePath);
                Assert.AreElementsEqual(new string[] { }, launcher.RuntimeSetup.PluginDirectories);

                Assert.AreElementsEqual(new string[] { }, from x in launcher.TestProject.TestPackage.Files select x.ToString());
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

                return new TestLauncherResult(new Report());
            });

            task.ExecuteWithMessagePump();
        }

        [Test]
        public void TaskPassesSpecifiedArgumentsToLauncher()
        {
            StubbedRunGallioCommand task = new StubbedRunGallioCommand();
            task.DoNotRun = true;
            task.NoEchoResults = true;
            task.Filter = "Type: SimpleTest";
            task.ReportDirectory = "dir";
            task.ReportTypes = new string[] { "XML", "Html" };
            task.ReportNameFormat = "report";
            task.ShowReports = true;
            task.RunTimeLimit = 7200; // seconds = 120 minutes

            task.RunnerType = StandardTestRunnerFactoryNames.Local;
            task.RunnerExtensions = new string[] { "DebugExtension,Gallio" };

            task.PluginDirectories = new string[] { "plugin" };
            task.Files = new string[] { "assembly1", "assembly2" };
            task.HintDirectories = new string[] { "hint1", "hint2" };

            task.ApplicationBaseDirectory = "baseDir";
            task.WorkingDirectory = "workingDir";
            task.ShadowCopy = true;
            task.DebugTests = true;
            task.RuntimeVersion = "v4.0.20506";

            task.RunnerProperties = new[] { "RunnerOption1=RunnerValue1", "  RunnerOption2  ", "RunnerOption3 = 'RunnerValue3'", "RunnerOption4=\"'RunnerValue4'\"" };
            task.ReportFormatterProperties = new[] { "FormatterOption1=FormatterValue1", "  FormatterOption2  ", "FormatterOption3 = 'FormatterValue3'", "FormatterOption4=\"'FormatterValue4'\"" };

            task.SetRunLauncherAction(delegate(TestLauncher launcher)
            {
                Assert.IsTrue(launcher.DoNotRun);
                Assert.IsFalse(launcher.EchoResults);
                Assert.AreEqual("Type: SimpleTest", launcher.TestExecutionOptions.FilterSet.ToFilterSetExpr());
                Assert.IsInstanceOfType(typeof(CommandLogger), launcher.Logger);
                Assert.IsInstanceOfType(typeof(CommandProgressMonitorProvider), launcher.ProgressMonitorProvider);
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
                Assert.AreEqual(Path.GetDirectoryName(AssemblyUtils.GetAssemblyLocalPath(typeof(RunGallioCommand).Assembly)), launcher.RuntimeSetup.RuntimePath);
                Assert.AreElementsEqual(new string[] { "plugin" }, launcher.RuntimeSetup.PluginDirectories);

                Assert.AreElementsEqual(new string[] { "assembly1", "assembly2" }, from x in launcher.TestProject.TestPackage.Files select x.ToString());
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

                return new TestLauncherResult(new Report());
            });

            task.ExecuteWithMessagePump();
        }

        [Test]
        public void TaskExposesResultsReturnedByLauncher()
        {
            StubbedRunGallioCommand task = new StubbedRunGallioCommand();
            TestLauncherResult expectedResult = new TestLauncherResult(new Report());

            task.SetRunLauncherAction(delegate
            {
                return expectedResult;
            });

            Assert.AreSame(expectedResult, task.ExecuteWithMessagePump());
        }
    }
}
