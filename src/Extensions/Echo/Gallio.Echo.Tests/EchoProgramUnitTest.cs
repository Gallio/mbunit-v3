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
using System.Reflection;
using Gallio.Common.Collections;
using Gallio.Model;
using Gallio.Model.Filters;
using Gallio.Common.Reflection;
using Gallio.Runner;
using Gallio.Runner.Reports;
using MbUnit.Framework;

namespace Gallio.Echo.Tests
{
    [TestFixture]
    [TestsOn(typeof(EchoProgram))]
    [Category("UnitTests")]
    public class EchoProgramUnitTest
    {
        [Test]
        public void TaskPassesDefaultArgumentsToLauncher()
        {
            TestLauncher launcher = new TestLauncher();
            EchoArguments arguments = new EchoArguments();

            EchoProgram.ConfigureLauncherFromArguments(launcher, arguments);

            Assert.IsFalse(launcher.DoNotRun);
            Assert.IsTrue(launcher.EchoResults);
            Assert.IsTrue(launcher.TestExecutionOptions.FilterSet.IsEmpty);
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
            Assert.IsNull(launcher.RuntimeSetup.RuntimePath);
            Assert.AreElementsEqual(new string[] { }, launcher.RuntimeSetup.PluginDirectories);

            Assert.AreElementsEqual(new string[] { }, from x in launcher.FilePatterns select x.ToString());
            Assert.AreElementsEqual(new string[] { }, from x in launcher.TestProject.TestPackage.HintDirectories select x.ToString());

            Assert.IsNull(launcher.TestProject.TestPackage.ApplicationBaseDirectory);
            Assert.IsFalse(launcher.TestProject.TestPackage.IsApplicationBaseDirectorySpecified);
            Assert.IsNull(launcher.TestProject.TestPackage.WorkingDirectory);
            Assert.IsFalse(launcher.TestProject.TestPackage.IsWorkingDirectorySpecified);
            Assert.IsFalse(launcher.TestProject.TestPackage.ShadowCopy);
            Assert.IsFalse(launcher.TestProject.TestPackage.IsShadowCopySpecified);
            Assert.IsNull(launcher.TestProject.TestPackage.DebuggerSetup);
            Assert.IsFalse(launcher.TestProject.TestPackage.IsDebuggerSetupSpecified);
            Assert.IsNull(launcher.TestProject.TestPackage.RuntimeVersion);
            Assert.IsFalse(launcher.TestProject.TestPackage.IsRuntimeVersionSpecified);

            Assert.AreEqual(new PropertySet(), launcher.TestRunnerOptions.Properties);
            Assert.AreEqual(new PropertySet(), launcher.ReportFormatterOptions.Properties);
        }

        [Test]
        public void TaskPassesSpecifiedArgumentsToLauncher()
        {
            TestLauncher launcher = new TestLauncher();
            EchoArguments arguments = new EchoArguments();

            arguments.DoNotRun = true;
            arguments.NoEchoResults = true;
            arguments.Filter = "Type: SimpleTest";
            arguments.ReportDirectory = "dir";
            arguments.ReportTypes = new string[] { "XML", "Html" };
            arguments.ReportNameFormat = "report";
            arguments.ShowReports = true;
            arguments.RunTimeLimitInSeconds = 7200; // seconds = 120 minutes

            arguments.RunnerType = StandardTestRunnerFactoryNames.Local;
            arguments.RunnerExtensions = new string[] { "DebugExtension,Gallio" };

            arguments.PluginDirectories = new string[] { "plugin" };
            arguments.Files = new[] { Assembly.GetExecutingAssembly().CodeBase };
            arguments.HintDirectories = new string[] { "hint1", "hint2" };

            arguments.ApplicationBaseDirectory = "baseDir";
            arguments.WorkingDirectory = "workingDir";
            arguments.ShadowCopy = true;
            arguments.Debug = true;
            arguments.RuntimeVersion = "v4.0.30319";

            arguments.RunnerProperties = new[] { "RunnerOption1=RunnerValue1", "  RunnerOption2  ", "RunnerOption3 = 'RunnerValue3'", "RunnerOption4=\"'RunnerValue4'\"" };
            arguments.ReportFormatterProperties = new[] { "FormatterOption1=FormatterValue1", "  FormatterOption2  ", "FormatterOption3 = 'FormatterValue3'", "FormatterOption4=\"'FormatterValue4'\"" };

            EchoProgram.ConfigureLauncherFromArguments(launcher, arguments);

            Assert.IsTrue(launcher.DoNotRun);
            Assert.IsFalse(launcher.EchoResults);
            Assert.AreEqual("Type: SimpleTest", launcher.TestExecutionOptions.FilterSet.ToFilterSetExpr());
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
            Assert.AreElementsEqual(new[] { "DebugExtension,Gallio" }, launcher.TestProject.TestRunnerExtensionSpecifications);

            Assert.IsNull(launcher.RuntimeSetup.ConfigurationFilePath);
            Assert.IsNull(launcher.RuntimeSetup.RuntimePath);
            Assert.AreElementsEqual(new[] { "plugin" }, launcher.RuntimeSetup.PluginDirectories);

            Assert.AreEqual(1, launcher.FilePatterns.Count);
            Assert.AreElementsEqual(new[] { "hint1", "hint2" }, from x in launcher.TestProject.TestPackage.HintDirectories select x.ToString());

            Assert.AreEqual("baseDir", launcher.TestProject.TestPackage.ApplicationBaseDirectory.ToString());
            Assert.IsTrue(launcher.TestProject.TestPackage.IsApplicationBaseDirectorySpecified);
            Assert.AreEqual("workingDir", launcher.TestProject.TestPackage.WorkingDirectory.ToString());
            Assert.IsTrue(launcher.TestProject.TestPackage.IsWorkingDirectorySpecified);
            Assert.IsTrue(launcher.TestProject.TestPackage.ShadowCopy);
            Assert.IsTrue(launcher.TestProject.TestPackage.IsShadowCopySpecified);
            Assert.IsNotNull(launcher.TestProject.TestPackage.DebuggerSetup);
            Assert.IsTrue(launcher.TestProject.TestPackage.IsDebuggerSetupSpecified);
            Assert.AreEqual("v4.0.30319", launcher.TestProject.TestPackage.RuntimeVersion);
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
        }
    }
}
