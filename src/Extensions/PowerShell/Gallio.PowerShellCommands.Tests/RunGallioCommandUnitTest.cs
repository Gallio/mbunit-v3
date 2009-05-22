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
using Gallio.Common.Collections;
using Gallio.Model;
using Gallio.Model.Filters;
using Gallio.Common.Reflection;
using Gallio.Runner;
using Gallio.Runner.Reports;
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
                Assert.AreEqual("", launcher.ReportDirectory);
                Assert.AreElementsEqual(new string[] { }, launcher.ReportFormats);
                Assert.AreEqual("test-report-{0}-{1}", launcher.ReportNameFormat);
                Assert.IsFalse(launcher.ShowReports);
                Assert.IsNull(launcher.RunTimeLimit);

                Assert.AreEqual(StandardTestRunnerFactoryNames.IsolatedProcess, launcher.TestRunnerFactoryName);
                Assert.AreEqual(0, launcher.TestRunnerExtensions.Count);
                Assert.AreElementsEqual(new string[] { }, launcher.TestRunnerExtensionSpecifications);

                Assert.IsNull(launcher.RuntimeSetup.ConfigurationFilePath);
                Assert.AreEqual(Path.GetDirectoryName(AssemblyUtils.GetAssemblyLocalPath(typeof(RunGallioCommand).Assembly)), launcher.RuntimeSetup.RuntimePath);
                Assert.AreElementsEqual(new string[] { }, launcher.RuntimeSetup.PluginDirectories);

                Assert.AreElementsEqual(new string[] { }, launcher.TestPackageConfig.AssemblyFiles);
                Assert.AreElementsEqual(new string[] { }, launcher.TestPackageConfig.HintDirectories);

                Assert.IsNull(launcher.TestPackageConfig.HostSetup.ApplicationBaseDirectory);
                Assert.IsNull(launcher.TestPackageConfig.HostSetup.WorkingDirectory);
                Assert.IsFalse(launcher.TestPackageConfig.HostSetup.ShadowCopy);
                Assert.IsFalse(launcher.TestPackageConfig.HostSetup.Debug);
                Assert.IsEmpty(launcher.TestPackageConfig.HostSetup.Configuration.SupportedRuntimeVersions);

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
            task.Assemblies = new string[] { "assembly1", "assembly2" };
            task.HintDirectories = new string[] { "hint1", "hint2" };

            task.ApplicationBaseDirectory = "baseDir";
            task.WorkingDirectory = "workingDir";
            task.ShadowCopy = true;
            task.DebugTests = true;
            task.RuntimeVersion = "4.0.20506";

            task.RunnerProperties = new[] { "RunnerOption1=RunnerValue1", "  RunnerOption2  ", "RunnerOption3 = 'RunnerValue3'", "RunnerOption4=\"'RunnerValue4'\"" };
            task.ReportFormatterProperties = new[] { "FormatterOption1=FormatterValue1", "  FormatterOption2  ", "FormatterOption3 = 'FormatterValue3'", "FormatterOption4=\"'FormatterValue4'\"" };

            task.SetRunLauncherAction(delegate(TestLauncher launcher)
            {
                Assert.IsTrue(launcher.DoNotRun);
                Assert.IsFalse(launcher.EchoResults);
                Assert.AreEqual("Type: SimpleTest", launcher.TestExecutionOptions.FilterSet.ToFilterSetExpr());
                Assert.IsInstanceOfType(typeof(CommandLogger), launcher.Logger);
                Assert.IsInstanceOfType(typeof(CommandProgressMonitorProvider), launcher.ProgressMonitorProvider);
                Assert.AreEqual("dir", launcher.ReportDirectory);
                Assert.AreElementsEqual(new string[] { "XML", "Html" }, launcher.ReportFormats);
                Assert.AreEqual("report", launcher.ReportNameFormat);
                Assert.IsTrue(launcher.ShowReports);
                Assert.AreEqual(TimeSpan.FromMinutes(120), launcher.RunTimeLimit);

                Assert.AreEqual(StandardTestRunnerFactoryNames.Local, launcher.TestRunnerFactoryName);
                Assert.AreEqual(0, launcher.TestRunnerExtensions.Count);
                Assert.AreElementsEqual(new string[] { "DebugExtension,Gallio" }, launcher.TestRunnerExtensionSpecifications);

                Assert.IsNull(launcher.RuntimeSetup.ConfigurationFilePath);
                Assert.AreEqual(Path.GetDirectoryName(AssemblyUtils.GetAssemblyLocalPath(typeof(RunGallioCommand).Assembly)), launcher.RuntimeSetup.RuntimePath);
                Assert.AreElementsEqual(new string[] { "plugin" }, launcher.RuntimeSetup.PluginDirectories);

                Assert.AreElementsEqual(new string[] { "assembly1", "assembly2" }, launcher.TestPackageConfig.AssemblyFiles);
                Assert.AreElementsEqual(new string[] { "hint1", "hint2" }, launcher.TestPackageConfig.HintDirectories);

                Assert.AreEqual("baseDir", launcher.TestPackageConfig.HostSetup.ApplicationBaseDirectory);
                Assert.AreEqual("workingDir", launcher.TestPackageConfig.HostSetup.WorkingDirectory);
                Assert.IsTrue(launcher.TestPackageConfig.HostSetup.ShadowCopy);
                Assert.IsTrue(launcher.TestPackageConfig.HostSetup.Debug);
                Assert.AreElementsEqual(new[] { "4.0.20506" }, launcher.TestPackageConfig.HostSetup.Configuration.SupportedRuntimeVersions);

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
